using System;
using System.Linq;
using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposureinfo
    public class PlatformExposureInformation : ExposureInformation
    {
        public PlatformExposureInformation() { }

        public PlatformExposureInformation(ENExposureInfo source)
        {
            AttenuationDurationsInMillis = ConvertToMillis(source.AttenuationDurations);
            AttenuationValue = source.AttenuationValue;
            DateMillisSinceEpoch = source.Date.GetDateMillisSinceEpoch();
            Duration = source.Duration;
            TotalRiskScore = source.TotalRiskScore;
            TransmissionRiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
        }

        private static int[] ConvertToMillis(int[] attenuationDurations)
            => attenuationDurations.Select(d => d * 1000).ToArray();
    }
}
