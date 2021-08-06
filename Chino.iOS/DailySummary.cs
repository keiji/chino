using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredaysummary
    public class PlatformDailySummary : DailySummary
    {
        public PlatformDailySummary() { }

        public PlatformDailySummary(ENExposureDaySummary source)
        {
            DateMillisSinceEpoch = source.Date.GetDateMillisSinceEpoch();
            DaySummary = GetExposureSummaryData(source.DaySummary);
            ConfirmedClinicalDiagnosisSummary = GetExposureSummaryData(source.ConfirmedClinicalDiagnosisSummary);
            ConfirmedTestSummary = GetExposureSummaryData(source.ConfirmedTestSummary);
            RecursiveSummary = GetExposureSummaryData(source.RecursiveSummary);
            SelfReportedSummary = GetExposureSummaryData(source.SelfReportedSummary);
        }

        private static PlatformExposureSummaryData? GetExposureSummaryData(ENExposureSummaryItem? summaryItem)
        {
            if (summaryItem == null)
            {
                return null;
            }
            return new PlatformExposureSummaryData(summaryItem);
        }

        public class PlatformExposureSummaryData : ExposureSummaryData
        {
            public PlatformExposureSummaryData() { }

            public PlatformExposureSummaryData(ENExposureSummaryItem source)
            {
                MaximumScore = source.MaximumScore;
                ScoreSum = source.ScoreSum;
                WeightedDurationSum = source.WeightedDurationSum;
            }

            public PlatformExposureSummaryData(Chino.ExposureSummaryData source)
            {
                MaximumScore = source.MaximumScore;
                ScoreSum = source.ScoreSum;
                WeightedDurationSum = source.WeightedDurationSum;
            }
        }
    }
}
