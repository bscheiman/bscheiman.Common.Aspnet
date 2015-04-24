#region
using System.Diagnostics;
using Autofac;
using Autofac.Core;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public class LogRequestsModule : Module {
        public static int Depth;

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration) {
            registration.Preparing += RegistrationOnPreparing;
            registration.Activating += RegistrationOnActivating;

            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        private string GetPrefix() {
            return new string('\t', Depth * 2);
        }

        private void RegistrationOnActivating(object sender, ActivatingEventArgs<object> activatingEventArgs) {
            Depth--;

            Trace.WriteLine(string.Format("{0}Activating {1}", GetPrefix(), activatingEventArgs.Component.Activator.LimitType));
        }

        private void RegistrationOnPreparing(object sender, PreparingEventArgs preparingEventArgs) {
            Trace.WriteLine(string.Format("{0}Resolving  {1}", GetPrefix(), preparingEventArgs.Component.Activator.LimitType));

            Depth++;
        }
    }
}