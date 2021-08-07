using System;
using System.Collections.Generic;
using Chino;
using Newtonsoft.Json;

namespace Sample.Common.Model
{
    public class ExposureResult
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

        [JsonProperty("en_version")]
        public string EnVersion = null;

        [JsonProperty("exposure_summary")]
        public readonly ExposureSummary? exposureSummary;

        [JsonProperty("exposure_informations")]
        public readonly IList<ExposureInformation>? exposureInformations;

        [JsonProperty("daily_summaries")]
        public readonly IList<DailySummary>? dailySummaries;

        [JsonProperty("exposure_windows")]
        public readonly IList<ExposureWindow>? exposureWindows;

        [JsonProperty("generated_at")]
        public readonly string generatedAt;

        [JsonProperty("exposure_configuration")]
        public readonly ExposureConfiguration exposureConfiguration;

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt)
            : this(exposureConfiguration, generatedAt, null, null, null, null) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
            : this(exposureConfiguration, generatedAt, exposureSummary, exposureInformations, null, null) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
            : this(exposureConfiguration, generatedAt, null, null, dailySummaries, exposureWindows) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            this.exposureConfiguration = exposureConfiguration;

            // to ISO 8601
            this.generatedAt = generatedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

            this.exposureSummary = exposureSummary;
            this.exposureInformations = exposureInformations;
            this.dailySummaries = dailySummaries;
            this.exposureWindows = exposureWindows;

            UpdateId();
        }

        private void UpdateId()
        {
            var hashCode = GetHashCode();
            Id = $"{_device}-{hashCode}";
        }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public override bool Equals(object obj)
        {
            return obj is ExposureResult result &&
                   _device == result._device &&
                   EnVersion == result.EnVersion &&
                   EqualityComparer<ExposureSummary>.Default.Equals(exposureSummary, result.exposureSummary) &&
                   EqualityComparer<IList<ExposureInformation>>.Default.Equals(exposureInformations, result.exposureInformations) &&
                   EqualityComparer<IList<DailySummary>>.Default.Equals(dailySummaries, result.dailySummaries) &&
                   EqualityComparer<IList<ExposureWindow>>.Default.Equals(exposureWindows, result.exposureWindows) &&
                   EqualityComparer<ExposureConfiguration>.Default.Equals(exposureConfiguration, result.exposureConfiguration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_device, EnVersion, exposureSummary, exposureInformations, dailySummaries, exposureWindows, exposureConfiguration);
        }
    }
}
