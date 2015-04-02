#region
using Newtonsoft.Json;

#endregion

namespace bscheiman.Common.Aspnet.Objects {
    public class BaseJson {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }
    }

    public class BaseJson<T> : BaseJson {
        [JsonProperty("value")]
        public T Value { get; set; }

        public BaseJson() {
        }

        public BaseJson(T value) {
            Value = value;
        }
    }
}