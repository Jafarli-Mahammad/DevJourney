using Application;
using Application.Behaviors;
using Application.Seeder;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Devjourney.AppCode.DI;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseServiceProviderFactory(new DevJourneyServiceProviderFactory());

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddHttpContextAccessor();

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
            var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "DevJourneySuperSecretKey1234567890!@#$";
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
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
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

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        app.UseMiddleware<Devjourney.Middlewares.GlobalExceptionMiddleware>();

        app.MapGet("/", () => "Hello World!");

        using (var scope = app.Services.CreateScope())
        {
            var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
            foreach (var seeder in seeders)
                await seeder.SeedAsync();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseSwagger();

        app.UseSwaggerUI();

        //app.UseRouting();

        //app.UseSession();

        //app.UseCors("allowAll");

        app.UseAuthentication();

        app.UseAuthorization();

        //app.MapRazorPages();

        app.MapControllers();

        app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(name: "default", pattern: "{controller=auth}/{action=login}/{id?}");

        app.Run();
    }
}