using Application.Repositories;
using Autofac;
using DataAccessLayer.Repositories;

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

            base.Load(builder);
        }
    }
}
