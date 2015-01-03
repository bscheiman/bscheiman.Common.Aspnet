#region
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Database;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class GlobalAsaxManager {
        public static void Config(HttpApplication app) {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DbInterception.Add(new DbLogger());
        }
    }
}