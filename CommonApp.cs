#region
using System.Web;

#endregion

namespace bscheiman.Common.Aspnet {
    public class CommonApp : HttpApplication {
        protected void Application_PreSendRequestHeaders() {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-SourceFiles");
        }
    }
}