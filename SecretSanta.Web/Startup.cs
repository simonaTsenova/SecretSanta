using Autofac;
using Autofac.Integration.WebApi;
using Autofac.TypedFactories;
using Microsoft.Owin;
using Owin;
using SecretSanta.Authentication;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common.Filters;
using SecretSanta.Data;
using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Providers;
using SecretSanta.Providers.Contracts;
using SecretSanta.Services;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Mapper;
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

            builder.RegisterType<AuthenticationProvider>().As<IAuthenticationProvider>().InstancePerRequest();
            builder.RegisterType<HttpContextProvider>().As<IHttpContextProvider>().InstancePerRequest();
            builder.RegisterType<ViewModelsMapper>().As<IMapper>().InstancePerRequest();

            builder.RegisterType<CustomErrorFilterAttribute>().AsWebApiExceptionFilterFor<ApiController>();
            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<UsersController>().InstancePerRequest();
            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<GroupsController>().InstancePerRequest();
            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<InvitationsController>().InstancePerRequest();
            builder.RegisterType<AuthorizeFilterAttribute>().AsWebApiActionFilterFor<LinksController>().InstancePerRequest();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            AutofacTypedFactoryExtensions.RegisterTypedFactory<IGroupFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IUserFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<ILinkFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IInvitationFactory>(builder).ReturningConcreteType();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<GroupService>().As<IGroupService>().InstancePerRequest();
            builder.RegisterType<InvitationService>().As<IInvitationService>().InstancePerRequest();
            builder.RegisterType<LinkService>().As<ILinkService>().InstancePerRequest();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<SecretSantaDbContext>().As<ISecretSantaDbContext>().InstancePerRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IEfRepository<>)).InstancePerRequest();

            var config = GlobalConfiguration.Configuration;
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureAuth(app);
        }
    }
}
