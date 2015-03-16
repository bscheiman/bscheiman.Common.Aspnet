#region
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

#endregion

namespace bscheiman.Common.Aspnet.Modules {
    public class ServiceModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(Assembly.GetCallingAssembly())
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }
    }
}