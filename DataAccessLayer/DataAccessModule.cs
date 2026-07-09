using Application.Repositories;
using Application.Seeder;
using Application.Services;
using Autofac;
using DataAccessLayer.Repositories;
using DataAccessLayer.Seeders;
using DataAccessLayer.Services;

namespace DataAccessLayer
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(AsyncRepository<>))
                .As(typeof(IAsyncRepository<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<StudentProfileRepository>()
                .As<IStudentProfileRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SkillRepository>()
                .As<ISkillRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<LanguageRepository>()
                .As<ILanguageRepository>()
                .InstancePerLifetimeScope();


            builder.RegisterType<AuthService>()
                .As<IAuthService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SkillSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
