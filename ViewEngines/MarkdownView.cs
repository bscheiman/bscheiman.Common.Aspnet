#region
using System.IO;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Helpers;

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

            writer.WriteLine(MarkdownHelper.Transform(RazorHelper.Transform(FullPath, viewContext.ViewData.Model), Css, Inline));
        }
    }
}