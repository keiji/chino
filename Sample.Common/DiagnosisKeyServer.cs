using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common
{
    public interface IDiagnosisKeyServer
    {
        public Task UploadDiagnosisKeysAsync(
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeyList,
            string idempotencyKey,
            ReportType defaultRportType = ReportType.ConfirmedTest
            );

        public Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync();

        public Task DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string path);

    }

    public class DiagnosisKeyServer : IDiagnosisKeyServer
    {
        private const long BUFFER_LENGTH = 4 * 1024 * 1024;
        private const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        private readonly DiagnosisKeyServerConfiguration _serverConfiguration;
        private readonly HttpClient _client;

        public DiagnosisKeyServer(DiagnosisKeyServerConfiguration serverConfiguration)
        {
            _serverConfiguration = serverConfiguration;
            _client = new HttpClient();
        }

        public async Task UploadDiagnosisKeysAsync(
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeyList,
            string idempotencyKey,
            ReportType defaultRportType = ReportType.ConfirmedTest
            )
        {
            var request = new RequestDiagnosisKey(
                symptomOnsetDate.ToString(FORMAT_SYMPTOM_ONSET_DATE),
                temporaryExposureKeyList,
                idempotencyKey,
                defaultRportType
                );
            var requestJson = JsonConvert.SerializeObject(request);
            Debug.Print(requestJson);

            var httpContent = new StringContent(requestJson);

            Uri uri = new Uri($"{_serverConfiguration.ApiEndpoint}/{_serverConfiguration.ClusterId}/chino-diagnosis-keys.json");
            HttpResponseMessage response = await _client.PutAsync(uri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Debug.Print(content);
            }
            else
            {
                Debug.Print($"UploadDiagnosisKeysAsync {response.StatusCode}");
            }
        }

        public async Task<IList<DiagnosisKeyEntry>> GetDiagnosisKeysListAsync()
        {
            Uri uri = new Uri($"{_serverConfiguration.ApiEndpoint}/{_serverConfiguration.ClusterId}/list.json");
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Debug.Print(content);
                return JsonConvert.DeserializeObject<List<DiagnosisKeyEntry>>(content);
            }
            else
            {
                Debug.Print($"GetDiagnosisKeysListAsync {response.StatusCode}");
            }

            return new List<DiagnosisKeyEntry>();
        }

        public async Task DownloadDiagnosisKeysAsync(DiagnosisKeyEntry diagnosisKeyEntry, string path)
        {
            Uri uri = new Uri(diagnosisKeyEntry.Url);
            HttpResponseMessage response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string fileName = uri.Segments[uri.Segments.Length - 1];
                string outputPath = Path.Combine(path, fileName);

                byte[] buffer = new byte[BUFFER_LENGTH];

                using BufferedStream bs = new BufferedStream(await response.Content.ReadAsStreamAsync());
                using FileStream fs = File.OpenWrite(outputPath);

                int len = 0;
                while ((len = await bs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, len);
                }
            }
            else
            {
                Debug.Print($"DownloadDiagnosisKeysAsync {response.StatusCode}");
            }
        }

    }

    [JsonObject]
    public class RequestDiagnosisKey
    {
        [JsonProperty("symptomOnsetDate")]
        public string SymptomOnsetDate { get; set; }

        [JsonProperty("temporaryExposureKeys")]
        public IList<Tek> temporaryExposureKeys;

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        public RequestDiagnosisKey(
            string symptomOnsetDate,
            IList<TemporaryExposureKey> teks,
            string idempotencyKey,
            ReportType defaultRportType = ReportType.ConfirmedTest
            )
        {
            SymptomOnsetDate = symptomOnsetDate;
            temporaryExposureKeys = teks.Select(tek =>
            {
                return new Tek(tek)
                {
                    reportType = (int)defaultRportType,
                };
            }).ToList();
            IdempotencyKey = idempotencyKey;
        }
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


    public class Tek
    {
        public readonly string key;
        public readonly long rollingStartNumber;
        public readonly long rollingPeriod;
        public int reportType;
        public int transmissionRisk;
        public int daysSinceOnsetOfSymptoms;

        public Tek(TemporaryExposureKey tek)
        {
            key = Convert.ToBase64String(tek.KeyData);
            rollingStartNumber = tek.RollingStartIntervalNumber;
            rollingPeriod = tek.RollingPeriod;
            reportType = (int)ReportType.ConfirmedClinicalDiagnosis;
            transmissionRisk = (int)RiskLevel.VeryHigh;
        }
    }
}
