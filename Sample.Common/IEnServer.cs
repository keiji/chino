using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common
{
    public interface IEnServer
    {
        public Task UploadDiagnosisKeysAsync(
            IList<TemporaryExposureKey> temporaryExposureKeyList,
            ReportType defaultRportType = ReportType.ConfirmedClinicalDiagnosis,
            RiskLevel defaultTrasmissionRisk = RiskLevel.Medium
            );

        public Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync();

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