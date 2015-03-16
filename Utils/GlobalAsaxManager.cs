#region
using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using bscheiman.Common.Aspnet.Database;
using bscheiman.Common.Aspnet.Modules;
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

        public static void Config<TContext>(HttpApplication app, bool addLogger = true, Action<ContainerBuilder> builderFunc = null) {
            Config(app, addLogger);

            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof (TContext).Assembly).PropertiesAutowired();
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new EFModule<TContext>());

            if (builderFunc != null)
                builderFunc(builder);

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}