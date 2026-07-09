using Application;
using Autofac;
using DataAccessLayer;

namespace Devjourney.AppCode.DI
{
    public class DevJourneyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyModules(typeof(DataAccessModule).Assembly);
            builder.RegisterAssemblyModules(typeof(ApplicationModule).Assembly);
        }
    }
}
