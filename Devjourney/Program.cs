using Application;
using Application.Behaviors;
using Application.Seeder;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Devjourney.AppCode.DI;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseServiceProviderFactory(new DevJourneyServiceProviderFactory());

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("YOUR_SERVER") || connectionString.Contains("YOUR_USER"))
        {
            throw new InvalidOperationException("Database credentials must be provided via environment variables or secret manager. The default placeholder cannot be used.");
        }

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString)
                   .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
        
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Application.Common.Interfaces.IFileStorage, DataAccessLayer.Services.LocalFileStorage>();

        // Phase 4: OpenTelemetry
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddSqlClientInstrumentation();

                if (useOtlpExporter)
                    tracing.AddOtlpExporter();
                else if (builder.Environment.IsDevelopment())
                    tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation();

                if (useOtlpExporter)
                    metrics.AddOtlpExporter();
                else if (builder.Environment.IsDevelopment())
                    metrics.AddConsoleExporter();
            });

        builder.Services.AddSingleton<Application.Common.Background.IBackgroundTaskQueue>(ctx => 
            new Application.Common.Background.DefaultBackgroundTaskQueue(100));
        builder.Services.AddHostedService<Application.Common.Background.QueuedHostedService>();
        
        // Phase 4: Data Retention Worker
        builder.Services.AddHostedService<Devjourney.BackgroundServices.DataRetentionWorker>();

        builder.Services.AddMemoryCache();
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            builder.Services.AddDistributedMemoryCache();
        }
        else
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "DevJourney:";
            });
        }
        builder.Services.AddResponseCompression();

        builder.Services.AddOutputCache(options =>
        {
            options.AddPolicy("PublicListings", builder => 
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByQuery("*")
                       .Tag("public-listings"));
            options.AddPolicy("PublicDetails", builder => 
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByRouteValue("id")
                       .Tag("public-details"));
        });

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(IApplicationReferance).Assembly));

        builder.Services.AddControllers();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = builder.Configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32 || secretKey == "YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARS_LONG")
            {
                throw new InvalidOperationException("A secure Jwt:SecretKey of at least 32 characters must be provided via environment variables. The placeholder cannot be used.");
            }
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "DevJourney";
            var audience = builder.Configuration["Jwt:Audience"] ?? "DevJourneyUsers";

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
            };

            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("accessToken"))
                    {
                        context.Token = context.Request.Cookies["accessToken"];
                    }
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 2,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: token);
            };
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DevJourney API", Version = "v1" });
            c.SwaggerDoc("partner", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Partner API", Version = "v1" });

            var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.OperationFilter<Devjourney.Filters.AuthorizeCheckOperationFilter>();
            c.OperationFilter<Devjourney.Filters.CleanMediaTypesOperationFilter>();
            c.OperationFilter<Devjourney.Filters.DefaultResponseTypesOperationFilter>();
            c.SupportNonNullableReferenceTypes();
            c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", corsBuilder =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    corsBuilder.SetIsOriginAllowed(origin => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                }
                else
                {
                    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                    if (allowedOrigins.Length > 0)
                    {
                        corsBuilder.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    }
                    else
                    {
                        // Fallback secure policy
                        corsBuilder.WithOrigins("https://trusted-domain.com").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    }
                }
            });
        });

        var app = builder.Build();

        app.UseResponseCompression();

        app.UseMiddleware<Devjourney.Middlewares.GlobalExceptionMiddleware>();

        app.MapGet("/", () => "Hello World!");

        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                await dataContext.Database.MigrateAsync();

                var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
                foreach (var seeder in seeders)
                    await seeder.SeedAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during database migration/seeding on startup.");
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevJourney API v1");
                c.SwaggerEndpoint("/swagger/partner/swagger.json", "Partner API v1");
            });
        }

        //app.UseRouting();
        app.UseRateLimiter();

        //app.UseSession();

        app.UseCors("AllowAll");

        app.UseAuthentication();

        app.UseAuthorization();
        
        app.UseOutputCache();

        //app.MapRazorPages();

        app.MapControllers();

        app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(name: "default", pattern: "{controller=auth}/{action=login}/{id?}");

        app.Run();
    }
}
