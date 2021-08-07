using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredetectionsummary
    public class PlatformExposureSummary : ExposureSummary
    {
        public PlatformExposureSummary() { }

        public PlatformExposureSummary(ENExposureDetectionSummary source)
        {
            AttenuationDurationsInMinutes = source.AttenuationDurations;
            DaysSinceLastExposure = (int)source.DaysSinceLastExposure;
            MatchedKeyCount = (long)source.MatchedKeyCount;
            MaximumRiskScore = source.MaximumRiskScore;
            SummationRiskScore = (int)source.RiskScoreSumFullRange;
        }
    }
}
