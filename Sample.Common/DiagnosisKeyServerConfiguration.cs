using Newtonsoft.Json;

namespace Sample.Common
{
    [JsonObject]
    public class DiagnosisKeyServerConfiguration
    {
        [JsonProperty("api_endpoint")]
        public string ApiEndpoint = "https://en.keiji.dev/diagnosis_keys";

        [JsonProperty("cluster_id")]
        public string ClusterId = "212458"; // 6 digits
    }
}
