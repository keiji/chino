using Newtonsoft.Json;

namespace Sample.Common
{
    [JsonObject]
    public class ExposureDataServerConfiguration
    {
        [JsonProperty("api_endpoint")]
        public string ApiEndpoint = "https://en.keiji.dev/exposure_data";

        [JsonProperty("cluster_id")]
        public string ClusterId = "212458"; // 6 digits
    }
}
