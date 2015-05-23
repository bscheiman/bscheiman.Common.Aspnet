#region
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using bscheiman.Common.Aspnet.Binders;
using bscheiman.Common.Aspnet.Database;
using bscheiman.Common.Aspnet.ViewEngines;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using MvcViewEngines = System.Web.Mvc.ViewEngines;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class StartupManager {
        public static void Config(bool addLogger = true) {
            MvcViewEngines.Engines.Clear();
            MvcViewEngines.Engines.Add(new MarkdownViewEngine());
            MvcViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.Add(typeof (decimal), new NumberModalBinder());
            ModelBinders.Binders.Add(typeof (decimal?), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (float), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (float?), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (double), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (double?), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (int), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (int?), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (long), new NumberModalBinder());
            //ModelBinders.Binders.Add(typeof (long?), new NumberModalBinder());

            if (addLogger)
                DbInterception.Add(new DbLogger());
        }

        public static void Config<TContext, TController>(IAppBuilder app, bool addLogger = true, bool autoSave = true,
                                                         Action<ContainerBuilder> builderFunc = null, bool log = false) where TContext : DbContext
            where TController : Controller {
            Config(addLogger);

            var builder = new ContainerBuilder();

            // DB CONTEXT
            if (autoSave) {
                builder.RegisterType(typeof (TContext))
                       .AsSelf()
                       .AsImplementedInterfaces()
                       .As<DbContext>()
                       .InstancePerRequest()
                       .OnRelease(x => ((TContext) x).SaveChanges());
            } else
                builder.RegisterType(typeof (TContext)).AsSelf().AsImplementedInterfaces().As<DbContext>().InstancePerRequest();

            // REGISTER DEPENDENCIES
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register(c => app.GetDataProtectionProvider()).InstancePerRequest();

            // REGISTER CONTROLLERS SO DEPENDENCIES ARE CONSTRUCTOR INJECTED
            builder.RegisterControllers(typeof (TController).Assembly);
            builder.RegisterApiControllers(typeof (TController).Assembly);

            // REGISTER FILTERS
            builder.RegisterFilterProvider();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            // ADD LOGGING
            if (log)
                builder.RegisterModule<LogRequestsModule>();

            // EXTRA SETUP
            if (builderFunc != null)
                builderFunc(builder);

            // BUILD THE CONTAINER
            var container = builder.Build();

            // REPLACE THE MVC DEPENDENCY RESOLVER WITH AUTOFAC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // REGISTER WITH OWIN
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
        }
    }
}