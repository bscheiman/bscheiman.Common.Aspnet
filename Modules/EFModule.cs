#region
using Autofac;

#endregion

namespace bscheiman.Common.Aspnet.Modules {
    public class EFModule<TContext> : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType(typeof (TContext)).AsSelf().InstancePerRequest();
        }
    }
}