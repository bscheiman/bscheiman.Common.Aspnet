#region
using System.Text;
using System.Web.Mvc;
using bscheiman.Common.Extensions;
using Newtonsoft.Json;

#endregion

namespace bscheiman.Common.Aspnet.Results {
    public class JsonNetResult : ActionResult {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }
        public Formatting Formatting { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }

        public JsonNetResult() {
            SerializerSettings = new JsonSerializerSettings();
        }

        public override void ExecuteResult(ControllerContext context) {
            context.ThrowIfNull("context");

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null) {
                var writer = new JsonTextWriter(response.Output) {
                    Formatting = Formatting
                };

                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}