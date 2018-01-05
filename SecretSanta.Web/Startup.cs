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

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            AutofacTypedFactoryExtensions.RegisterTypedFactory<IUserFactory>(builder).ReturningConcreteType();
            AutofacTypedFactoryExtensions.RegisterTypedFactory<IUserSessionFactory>(builder).ReturningConcreteType();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<SessionService>().As<ISessionService>().InstancePerRequest();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<SecretSantaDbContext>().As<ISecretSantaDbContext>().InstancePerRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IEfRepository<>)).InstancePerRequest();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureAuth(app);
        }
    }
}
