using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredetectionsummary
    public class ExposureSummary : IExposureSummary
    {
        public ENExposureDetectionSummary Source;

        public ExposureSummary(ENExposureDetectionSummary source)
        {
            Source = source;
        }

        public int[] AttenuationDurationsInMinutes => Source.AttenuationDurations;

        public int DaysSinceLastExposure => (int)Source.DaysSinceLastExposure;

        public long MatchedKeyCount => (long)Source.MatchedKeyCount;

        public int MaximumRiskScore => Source.MaximumRiskScore;

        public int SummationRiskScore => (int)Source.RiskScoreSumFullRange;
    }
}
