#region
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using bscheiman.Common.Extensions;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public enum MinifierType {
        None,
        Css,
        Javascript
    }

    public class Minifier : IHttpHandler {
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

        public void ProcessRequest(HttpContext context) {
        }

        public bool IsReusable {
            get { return false; }
        }
    }

    /*public class LoadResources : IHttpHandler {
        internal static readonly Dictionary<string, DateTime> ModifiedFiles =
            new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);

        internal static readonly Dictionary<string, string> FileCache =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public void ProcessRequest(HttpContext context) {
            var qs = context.Request.QueryString;

            if (qs["h"] != null)
                qs = HttpUtility.ParseQueryString(Minifier.GetUrl(qs["h"]));

            var files = qs.GetValues("f").ToArray();
            var debug = Ignore.Exception(() => qs.AllKeys.Any(a => a == "d") && Convert.ToBoolean(qs["d"]), false);
            var type = (qs.AllKeys.Any(a => a == "t") ? qs["t"] : "js").ToLower();
            var sb = new StringBuilder();

            context.Response.ContentType = type == "js" ? "text/javascript" : "text/css";

            foreach (var fileContents in
                files.Distinct().Select(p => GetFile(p, type)).Where(fileContents => !string.IsNullOrEmpty(fileContents)))
                sb.Append(fileContents);

            var variables = Regex.Matches(sb.ToString(), @"@{(?<Var>[^}]+)}")
                .Cast<Match>()
                .ToDictionary(m => m.Groups["Var"].Value, p => "");

            foreach (string key in qs.Cast<string>().Where(key => key != "f" && key != "d" && key != "t" && key != "h"))
                variables[key] = qs[key];

            if (variables.Any(p => string.IsNullOrEmpty(p.Value))) {
                throw new Exception(string.Format("Missing variables: {0}",
                    string.Join(",", variables.Where(p => string.IsNullOrEmpty(p.Value)).Select(p => p.Key).ToArray())));
            }

            var str = variables.Aggregate(sb.ToString(), (current, k) => current.Replace(string.Format("@{{{0}}}", k.Key), k.Value));

            context.Response.AddFileDependencies(files);

            var cache = context.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["f"] = true;
            cache.SetETag(DateUtil.Now.ToString(CultureInfo.InvariantCulture));
            cache.SetLastModified(ModifiedFiles.Select(k => k.Value).Max());
            cache.SetMaxAge(TimeSpan.FromDays(14));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            if (debug)
                context.Response.Write(str);
            else {
                if (type == "js") {
                    context.Response.Write(new JavaScriptCompressor {
                        CompressionType = CompressionType.Standard,
                        ObfuscateJavascript = true,
                        PreserveAllSemicolons = true,
                        Encoding = Encoding.UTF8,
                        DisableOptimizations = false
                    }.Compress(str));
                } else if (type == "css") {
                    context.Response.Write(new CssCompressor {
                        CompressionType = CompressionType.Standard,
                        RemoveComments = true
                    }.Compress(str));
                }
            }

            context.Response.End();
        }

        public bool IsReusable {
            get { return true; }
        }

        public static string GetFile(string path, string type) {
            var files = new[] {
                HttpContext.Current.Server.MapPath(string.Format("~/Scripts/{0}.js", path)),
                HttpContext.Current.Server.MapPath(string.Format("~/Scripts/custom/{0}.js", path)),
                HttpContext.Current.Server.MapPath(string.Format("~/Scripts/ace/{0}.js", path)),
                HttpContext.Current.Server.MapPath(string.Format("~/Content/css/custom/{0}.css", path)),
                HttpContext.Current.Server.MapPath(string.Format("~/Content/css/ace/{0}.css", path)),
                HttpContext.Current.Server.MapPath(string.Format("~/Content/css/{0}.css", path))
            };

            var realFile = files.FirstOrDefault(File.Exists);

            if (realFile == null)
                return string.Empty;

            var fileInfo = new FileInfo(realFile);

            if (!ModifiedFiles.ContainsKey(realFile) || (fileInfo.LastWriteTimeUtc > ModifiedFiles[realFile])) {
                FileCache[realFile] = File.ReadAllText(realFile);
                ModifiedFiles[realFile] = DateUtil.NowDt;
            }

            return FileCache[realFile];
        }

        public static void GZipEncodePage() {
            var response = HttpContext.Current.Response;

            if (IsGZipSupported()) {
                string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

                if (acceptEncoding.Contains("gzip")) {
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    response.Headers.Remove("Content-Encoding");
                    response.AppendHeader("Content-Encoding", "gzip");
                } else {
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                    response.Headers.Remove("Content-Encoding");
                    response.AppendHeader("Content-Encoding", "deflate");
                }
            }

            response.AppendHeader("Vary", "Content-Encoding");
        }

        public static bool IsGZipSupported() {
            string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

            return !string.IsNullOrEmpty(acceptEncoding) && (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate"));
        }
    }*/
}