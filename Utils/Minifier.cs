#region
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using bscheiman.Common.Extensions;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public class Minifier {
        internal static Dictionary<string, string> UrlCache = new Dictionary<string, string>();
        public MinifierType ContentType { get; set; }
        public string File { get; set; }
        public HashSet<string> Files { get; set; }
        public bool IsDebug { get; set; }
        public Dictionary<string, string> Variables { get; set; }

        internal Minifier() {
            IsDebug = false;
            ContentType = MinifierType.None;
            Variables = new Dictionary<string, string>();
        }

        public Minifier Css() {
            ContentType = MinifierType.Css;

            return this;
        }

        public static Minifier Debug(string file = "/LoadResources.ashx") {
            return new Minifier {
                File = file,
                IsDebug = false
            };
        }

        public static Minifier Default(string file = "/LoadResources.ashx") {
            return new Minifier {
                File = file
            };
        }

        public static string GetUrl(string key) {
            return UrlCache.ContainsKey(key) ? UrlCache[key] : string.Empty;
        }

        public Minifier Js() {
            ContentType = MinifierType.Javascript;

            return this;
        }

        public static implicit operator string(Minifier m) {
            return m.ToString();
        }

        public static implicit operator Uri(Minifier m) {
            return new Uri(m.ToString());
        }

        public static Minifier Release(string file = "/LoadResources.ashx") {
            return new Minifier {
                File = file,
                IsDebug = false
            };
        }

        public IHtmlString Render() {
            return MvcHtmlString.Create(ToString());
        }

        public override string ToString() {
            var list = new HashSet<string> {
                string.Format("d={0}", IsDebug.ToString().ToLower()),
                string.Format("t={0}", ContentType == MinifierType.Javascript ? "js" : "css")
            };

            foreach (string f in Files)
                list.Add(string.Format("f={0}", f));

            foreach (var v in Variables)
                list.Add(string.Format("{0}={1}", v.Key, v.Value));

            string str = string.Join("&", list);
            string sha1 = str.ToSHA1();

            UrlCache[sha1] = str;

            if (ContentType == MinifierType.Javascript)
                return string.Format(@"<script type=""text/javascript"" src=""{0}?h={1}""></script>", File, sha1);

            return string.Format(@"<link href=""{0}?h={1}"" rel=""stylesheet"" type=""text/css"">", File, sha1);
        }

        public Minifier Var(string key, object obj) {
            Variables[key] = obj.ToString();

            return this;
        }

        public Minifier WithFiles(params string[] files) {
            Files = new HashSet<string>(files);

            return this;
        }
    }
}