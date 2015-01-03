#region
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet {
    public static class GlobalAsaxManager {
        public static void Config(HttpApplication app) {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DbInterception.Add(new DbLogger());
        }
    }
}