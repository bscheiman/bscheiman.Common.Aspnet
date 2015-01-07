#region
using System.Collections.Generic;
using bscheiman.Common.Aspnet.Utils;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    public static class TemplateHelper {
        public static MarkdownMailer.MarkdownResult Transform(string markdownSource, string cssSource, Dictionary<string, string> variables) {
            var md = new MarkdownMailer(markdownSource, cssSource);
            var res = md.Transform(variables);

            if (res.MissingVariables)
                Log.Error("Missing template variables");

            return res;
        }
    }
}