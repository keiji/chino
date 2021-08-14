using System;
using System.Linq;
using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposureinfo
    public class PlatformExposureInformation : ExposureInformation
    {
        private const int SECOND_IN_MILLIS = 1000;

        public PlatformExposureInformation() { }

        public PlatformExposureInformation(ENExposureInfo source)
        {
            AttenuationDurationsInMillis = ConvertToMillis(source.AttenuationDurations);
            AttenuationValue = source.AttenuationValue;
            DateMillisSinceEpoch = source.Date.GetDateMillisSinceEpoch();
            DurationInMillis = source.Duration * SECOND_IN_MILLIS;
            TotalRiskScore = source.TotalRiskScore;
            TransmissionRiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
        }

        private static int[] ConvertToMillis(int[] attenuationDurations)
            => attenuationDurations.Select(d => d * SECOND_IN_MILLIS).ToArray();
    }
}
