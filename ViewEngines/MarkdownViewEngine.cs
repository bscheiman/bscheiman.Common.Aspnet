#region
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.ViewEngines {
    public class MarkdownViewEngine : IViewEngine {
        internal string[] ViewSearchPaths = {
            "~/Views/{0}/{1}.md", "~/Views/Shared/{1}.md"
        };

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            return null;
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            var ctrl = controllerContext.RouteData.Values["controller"];
            var result = new ViewEngineResult(ViewSearchPaths.Select(v => string.Format(v, ctrl, viewName)));

            foreach (string path in ViewSearchPaths) {
                string fullPath = HostingEnvironment.MapPath(string.Format(path, ctrl, viewName));

                if (File.Exists(fullPath))
                    result = new ViewEngineResult(new MarkdownView(fullPath), this);
            }

            return result;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
        }
    }
}