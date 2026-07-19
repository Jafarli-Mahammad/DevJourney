using Application.Repositories;
using Application.Repositories.Post;
using Application.Seeder;
using Application.Services;
using Autofac;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Post;
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

            builder.RegisterType<CompanyProfileRepository>()
                .As<ICompanyProfileRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UniversityProfileRepository>()
                .As<IUniversityProfileRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PartnerProfileRepository>()
                .As<IPartnerProfileRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SkillRepository>()
                .As<ISkillRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<LanguageRepository>()
                .As<ILanguageRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RoleRepository>()
                .As<IRoleRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IdeaFieldRepository>()
                .As<IIdeaFieldRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TeamMemberSearchPostRepository>()
                .As<ITeamMemberSearchPostRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TeamSearchPostRepository>()
                .As<ITeamSearchPostRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NetworkingEventPostRepository>()
                .As<INetworkingEventPostRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CorporateEventPostRepository>()
                .As<ICorporateEventPostRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<B2BCoursePromoPostRepository>()
                .As<IB2BCoursePromoPostRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AuthService>()
                .As<IAuthService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JwtService>()
                .As<IJwtService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CurrentUserService>()
               .As<ICurrentUserService>()
               .InstancePerLifetimeScope();

            builder.RegisterType<SkillSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
            builder.RegisterType<RoleSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
            builder.RegisterType<IdeaFieldSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();


            base.Load(builder);
        }
    }
}
