#region
using System.IO;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Helpers;
using bscheiman.Common.Extensions;
using RazorEngine;
using RazorEngine.Templating;

#endregion

namespace bscheiman.Common.Aspnet.ViewEngines {
    public class MarkdownView : IView {
        public static string Css { get; set; }
        public string FullPath { get; set; }
        public static bool Inline { get; set; }

        public MarkdownView(string fullPath) {
            FullPath = fullPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer) {
            if (!File.Exists(FullPath))
                writer.WriteLine("View not found");

            string template = File.ReadAllText(FullPath);
            string htmlBody = Engine.Razor.RunCompile(template, template.ToMD5(), null, viewContext.ViewData.Model);

            writer.WriteLine(MarkdownHelper.Transform(htmlBody, Css, Inline));
        }
    }
}