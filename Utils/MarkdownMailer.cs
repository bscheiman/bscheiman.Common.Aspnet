#region
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MarkdownSharp;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public class MarkdownMailer {
        public string Css { get; set; }
        public string Template { get; set; }

        public MarkdownMailer(Stream template, Stream css) : this(new StreamReader(template).ReadToEnd(), new StreamReader(css).ReadToEnd()) {
        }

        public MarkdownMailer(Stream template) : this(new StreamReader(template).ReadToEnd()) {
        }

        public MarkdownMailer(string template) : this(template, string.Empty) {
        }

        public MarkdownMailer(string template, Stream css) : this(template, new StreamReader(css).ReadToEnd()) {
        }

        public MarkdownMailer(string template, string css) {
            Template = template;
            Css = css;
        }

        public MarkdownResult Transform(Dictionary<string, string> variables = null) {
            var markdown = new Markdown(new MarkdownOptions {
                AutoHyperlink = true,
                EncodeProblemUrlCharacters = true,
                LinkEmails = true
            });

            string tempTemplate = Template;

            if (variables != null)
                tempTemplate = variables.Aggregate(tempTemplate, (current, t) => current.Replace(t.Key, t.Value));

            string html = markdown.Transform(tempTemplate);

            html += "<style>" + Css + "</style>";
            var inlined = PreMailer.Net.PreMailer.MoveCssInline(html, true);

            var m = Regex.Match(tempTemplate, @"\$\w+\$");

            return new MarkdownResult {
                Markdown = tempTemplate,
                Html = inlined.Html,
                Plaintext = Regex.Replace(inlined.Html, "<[^>]*>", ""),
                MissingVariables = m.Success
            };
        }

        public class MarkdownResult {
            public string Html { get; set; }
            public string Markdown { get; set; }
            public bool MissingVariables { get; set; }
            public string Plaintext { get; set; }
        }
    }
}