using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common
{
    public interface IEnServer
    {
        public Task UploadDiagnosisKeysAsync(
            string clusterId,
            IList<ITemporaryExposureKey> temporaryExposureKeyList,
            ReportType defaultRportType = ReportType.ConfirmedClinicalDiagnosis,
            RiskLevel defaultTrasmissionRisk = RiskLevel.Medium
            );

        public Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync(string clusterId);

        public Task DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string path);

    }

    [JsonObject]
    public class DiagnosisKeyEntry
    {
        [JsonProperty("region")]
        public int Region;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("created")]
        public long Created;
    }
}