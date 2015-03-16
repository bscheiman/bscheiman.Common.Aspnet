#region
using System.IO;
using bscheiman.Common.Extensions;
using RazorEngine;
using RazorEngine.Templating;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    public static class RazorHelper {
        public static string Transform(string fullPath, object model) {
            string template = File.ReadAllText(fullPath);

            model = model ?? new object();

            return Engine.Razor.RunCompile(template, template.ToMD5(), model.GetType(), model);
        }
    }
}