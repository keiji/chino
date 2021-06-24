using System;
using System.Collections.Generic;
using System.Linq;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common.Model
{
    public class TemporaryExposureKeys
    {
        [JsonProperty("id")]
        public string Id;

        private string _device = "unknown_device";
        public string Device
        {
            set
            {
                string device = value.Replace(" ", "_");
                _device = device;

                UpdateId();
            }
        }

        public readonly IList<Tek> temporaryExposureKeys;

        [JsonProperty("generated_at")]
        public readonly string generatedAt;

        public TemporaryExposureKeys(
            IList<ITemporaryExposureKey> teks,
            DateTime generatedAt,
            ReportType defaultRportType = ReportType.ConfirmedClinicalDiagnosis,
            RiskLevel defaultTrasmissionRisk = RiskLevel.Medium
            )
        {
            temporaryExposureKeys = teks.Select(tek => {
                return new Tek(tek)
                {
                    reportType = (int)defaultRportType,
                    transmissionRisk = (int)defaultTrasmissionRisk,
                };
            }).ToList();

            // to ISO 8601
            this.generatedAt = generatedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

            UpdateId();
        }

        private void UpdateId()
        {
            if (temporaryExposureKeys.Count == 0)
            {
                Id = $"{_device}";
                return;
            }

            var latest = temporaryExposureKeys.Max(tek => tek.rollingStartNumber + tek.rollingPeriod);
            Id = $"{_device}-{latest}";
        }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class Tek
    {
        public readonly string key;
        public readonly long rollingStartNumber;
        public readonly long rollingPeriod;
        public int reportType;
        public int transmissionRisk;

        public Tek(ITemporaryExposureKey tek)
        {
            key = Convert.ToBase64String(tek.KeyData);
            rollingStartNumber = tek.RollingStartIntervalNumber;
            rollingPeriod = tek.RollingPeriod;
            reportType = (int)ReportType.ConfirmedClinicalDiagnosis;
            transmissionRisk = (int)RiskLevel.VeryHigh;
        }
    }
}
