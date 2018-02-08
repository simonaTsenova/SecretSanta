using Autofac;
using Autofac.Integration.WebApi;
using Autofac.TypedFactories;
using Microsoft.Owin;
using Owin;
using SecretSanta.Data;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Services;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Infrastructure.Factories;
using SecretSanta.Web.Infrastructure.Filters;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartup(typeof(SecretSanta.Web.Startup))]

namespace SecretSanta.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<UsersController>().InstancePerRequest();
            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<GroupsController>().InstancePerRequest();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            AutofacTypedFactoryExtensions.RegisterTypedFactory<IGroupFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IUserFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IInvitationFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IUserSessionFactory>(builder).ReturningConcreteType();

            AutofacTypedFactoryExtensions.RegisterTypedFactory<IDisplayUserViewModelFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IDisplayGroupViewModelFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IInvitationViewModelFactory>(builder).ReturningConcreteType();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<GroupService>().As<IGroupService>().InstancePerRequest();
            builder.RegisterType<SessionService>().As<ISessionService>().InstancePerRequest();
            builder.RegisterType<InvitationService>().As<IInvitationService>().InstancePerRequest();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<SecretSantaDbContext>().As<ISecretSantaDbContext>().InstancePerRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IEfRepository<>)).InstancePerRequest();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;
            builder.RegisterWebApiFilterProvider(config);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureAuth(app);
        }
    }
}
