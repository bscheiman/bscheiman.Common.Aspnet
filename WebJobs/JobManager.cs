#region
using System;
using System.Linq;

#endregion

namespace bscheiman.Common.Aspnet.WebJobs {
    public static class JobManager {
        public static async void RunSteps<TJob>() where TJob : IWebJob {
            var steps =
                typeof (TJob).Assembly.GetTypes()
                             .Where(p => typeof (IWebJob).IsAssignableFrom(p) && !p.IsInterface)
                             .Select(a => (IWebJob) Activator.CreateInstance(a))
                             .OrderBy(a => a.Priority)
                             .ToArray();

            foreach (var step in steps)
                await step.Update();
        }
    }
}