using System.Linq;
using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredetectionsummary
    public class PlatformExposureSummary : ExposureSummary
    {
        private const int SECOND_IN_MILLIS = 1000;

        public PlatformExposureSummary() { }

        public PlatformExposureSummary(ENExposureDetectionSummary source)
        {
            AttenuationDurationsInMillis = ConvertToMillis(source.AttenuationDurations);
            DaysSinceLastExposure = (int)source.DaysSinceLastExposure;
            MatchedKeyCount = source.MatchedKeyCount;
            MaximumRiskScore = source.MaximumRiskScore;
            SummationRiskScore = (int)source.RiskScoreSumFullRange;
        }

        private static int[] ConvertToMillis(int[] attenuationDurations)
        {
            if (attenuationDurations == null)
            {
                return new int[0];
            }
            return attenuationDurations.Select(d => d * SECOND_IN_MILLIS).ToArray();
        }
    }
}
