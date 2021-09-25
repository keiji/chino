using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common
{
    public interface IEnServer
    {
        public Task UploadDiagnosisKeysAsync(
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeyList,
            string idempotencyKey,
            ReportType defaultRportType = ReportType.ConfirmedTest);

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