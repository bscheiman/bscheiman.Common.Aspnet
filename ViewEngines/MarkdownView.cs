#region
using System.IO;
using System.Web.Mvc;
using MarkdownSharp;
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

            string emailHtmlBody = new TemplateService().Parse(File.ReadAllText(FullPath), viewContext.ViewData.Model, null, null);
            string html = new Markdown(new MarkdownOptions {
                AutoHyperlink = true,
                EncodeProblemUrlCharacters = true,
                LinkEmails = true
            }).Transform(emailHtmlBody);

            writer.WriteLine(Inline
                ? PreMailer.Net.PreMailer.MoveCssInline(html, true, css: Css).Html
                : string.Format("<style>{0}</style>{1}", Css, html));
        }
    }
}