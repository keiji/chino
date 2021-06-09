using System;
using System.Linq;
using ExposureNotifications;
using Newtonsoft.Json;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposureinfo
    public class ExposureInformation : IExposureInformation
    {
        [JsonIgnore]
        public readonly ENExposureInfo Source;

        public ExposureInformation(ENExposureInfo source)
        {
            Source = source;
        }

        public int[] AttenuationDurationsInMillis => ConvertToMillis(Source.AttenuationDurations);

        public int AttenuationValue => Source.AttenuationValue;

        public long DateMillisSinceEpoch => Source.Date.GetDateMillisSinceEpoch();

        public double Duration => Source.Duration;

        public int TotalRiskScore => Source.TotalRiskScore;

        public RiskLevel TransmissionRiskLevel => (RiskLevel)Enum.ToObject(typeof(RiskLevel), Source.TransmissionRiskLevel);

        private static int[] ConvertToMillis(int[] attenuationDurations)
            => attenuationDurations.Select(d => d * 1000).ToArray();
    }
}
