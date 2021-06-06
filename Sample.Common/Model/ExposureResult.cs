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
        public readonly IExposureSummary? exposureSummary;

        [JsonProperty("exposure_informations")]
        public readonly IList<IExposureInformation>? exposureInformations;

        [JsonProperty("daily_summaries")]
        public readonly IList<IDailySummary>? dailySummaries;

        [JsonProperty("exposure_windows")]
        public readonly IList<IExposureWindow>? exposureWindows;

        [JsonProperty("generated_at")]
        public readonly string generatedAt;

        [JsonProperty("exposure_configuration")]
        public readonly ExposureConfiguration exposureConfiguration;

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt)
            : this(exposureConfiguration, generatedAt, null, null, null, null) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            IExposureSummary exposureSummary, IList<IExposureInformation> exposureInformations)
            : this(exposureConfiguration, generatedAt, exposureSummary, exposureInformations, null, null) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            IList<IDailySummary> dailySummaries, IList<IExposureWindow> exposureWindows)
            : this(exposureConfiguration, generatedAt, null, null, dailySummaries, exposureWindows) { }

        public ExposureResult(ExposureConfiguration exposureConfiguration,
            DateTime generatedAt,
            IExposureSummary exposureSummary, IList<IExposureInformation> exposureInformations,
            IList<IDailySummary> dailySummaries, IList<IExposureWindow> exposureWindows)
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
                   EqualityComparer<IExposureSummary>.Default.Equals(exposureSummary, result.exposureSummary) &&
                   EqualityComparer<IList<IExposureInformation>>.Default.Equals(exposureInformations, result.exposureInformations) &&
                   EqualityComparer<IList<IDailySummary>>.Default.Equals(dailySummaries, result.dailySummaries) &&
                   EqualityComparer<IList<IExposureWindow>>.Default.Equals(exposureWindows, result.exposureWindows) &&
                   EqualityComparer<ExposureConfiguration>.Default.Equals(exposureConfiguration, result.exposureConfiguration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_device, EnVersion, exposureSummary, exposureInformations, dailySummaries, exposureWindows, exposureConfiguration);
        }
    }
}
