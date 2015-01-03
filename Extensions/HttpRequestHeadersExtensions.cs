#region
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class HttpRequestHeadersExtensions {
        public static string Get(this HttpRequestHeaders req, string key) {
            if (!req.Contains(key))
                return "";

            IEnumerable<string> values;

            req.TryGetValues(key, out values);

            return values.First();
        }
    }
}