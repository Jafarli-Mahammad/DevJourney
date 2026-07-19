using Application.Behaviors;
using Application.Modules.Posts.Notifications;
using Autofac;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //builder.RegisterType<CryptoService>()
            //    .As<ICryptoService>()
            //    .InstancePerLifetimeScope();

            //builder.RegisterType<FileService>()
            //.As<IFileService>()
            //.InstancePerLifetimeScope();


            builder.RegisterAssemblyTypes(typeof(PostCreatedNotification).Assembly)
               .AsClosedTypesOf(typeof(INotificationHandler<>))
               .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(ValidationBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(VerifiedAuthorBehaviour<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IApplicationReferance).Assembly)
                .AsClosedTypesOf(typeof(IValidator<>))
                .InstancePerLifetimeScope();

            // ... inside your Load(ContainerBuilder builder) method:

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                // This scans your assembly exactly like you want
                cfg.AddMaps(typeof(IApplicationReferance).Assembly);
            }, NullLoggerFactory.Instance)) // <-- PASS THIS AS THE SECOND ARGUMENT
            .AsSelf()
            .SingleInstance();

            builder.Register(context =>
            {
                var ctx = context.Resolve<IComponentContext>();
                var config = ctx.Resolve<MapperConfiguration>();
                return config.CreateMapper(ctx.Resolve);
            }).As<IMapper>().InstancePerLifetimeScope();
        }
    }
}