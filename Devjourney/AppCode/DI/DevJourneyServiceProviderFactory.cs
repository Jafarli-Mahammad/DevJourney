using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Devjourney.AppCode.DI
{
    public class DevJourneyServiceProviderFactory : AutofacServiceProviderFactory
    {
        public DevJourneyServiceProviderFactory() : base(OnRegister)
        {
        }

        private static void OnRegister(ContainerBuilder builder)
        {
            builder.RegisterModule<DevJourneyModule>();
        }
    }
}
