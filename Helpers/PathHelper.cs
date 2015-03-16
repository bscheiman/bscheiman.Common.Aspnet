#region
using System.IO;
using System.Web.Hosting;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    public static class PathHelper {
        public static bool Exists(string path) {
            return !string.IsNullOrEmpty(path) && File.Exists(path.StartsWith("~") ? MapRelative(path) : path);
        }

        public static string MapRelative(string path) {
            return string.IsNullOrEmpty(path) ? string.Empty : HostingEnvironment.MapPath(path);
        }

        public static Stream ReadAsStream(string path) {
            string fullPath = MapRelative(path);

            return Exists(fullPath) ? (Stream) File.OpenRead(fullPath) : new MemoryStream();
        }

        public static string ReadAsString(string path) {
            string fullPath = MapRelative(path);

            return Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
        }
    }
}