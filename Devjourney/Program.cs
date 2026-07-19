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

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

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