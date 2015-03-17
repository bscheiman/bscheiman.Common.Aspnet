#region
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using bscheiman.Common.Aspnet.Database;
using bscheiman.Common.Aspnet.ViewEngines;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class GlobalAsaxManager {
        public static void Config(HttpApplication app, bool addLogger = true) {
            System.Web.Mvc.ViewEngines.Engines.Clear();
            System.Web.Mvc.ViewEngines.Engines.Add(new MarkdownViewEngine());
            System.Web.Mvc.ViewEngines.Engines.Add(new RazorViewEngine());

            if (addLogger)
                DbInterception.Add(new DbLogger());
        }

        public static void Config<TContext, TController>(HttpApplication app, bool addLogger = true,
                                                         Action<ContainerBuilder> builderFunc = null) where TContext : DbContext
            where TController : Controller {
            Config(app, addLogger);

            var builder = new ContainerBuilder();
            builder.RegisterType(typeof (TContext)).AsSelf().AsImplementedInterfaces().As<DbContext>().InstancePerRequest();
            builder.RegisterControllers(typeof (TController).Assembly).PropertiesAutowired();

            if (builderFunc != null)
                builderFunc(builder);

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}