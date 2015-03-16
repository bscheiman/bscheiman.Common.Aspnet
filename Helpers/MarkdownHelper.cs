#region
using MarkdownSharp;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    public static class MarkdownHelper {
        public static string Transform(string template, string css, bool inline = false) {
            string html = new Markdown(new MarkdownOptions {
                AutoHyperlink = true,
                EncodeProblemUrlCharacters = true,
                LinkEmails = true
            }).Transform(template);

            return inline
                ? PreMailer.Net.PreMailer.MoveCssInline(html, true, css: css).Html
                : string.Format("<style>{0}</style>{1}", css, html);
        }
    }
}