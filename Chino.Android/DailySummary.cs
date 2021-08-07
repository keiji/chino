using AndroidDailySummary = Android.Gms.Nearby.ExposureNotification.DailySummary;
using AndroidReportType = Android.Gms.Nearby.ExposureNotification.ReportType;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class PlatformDailySummary : DailySummary
    {
        public PlatformDailySummary(AndroidDailySummary source)
        {
            DateMillisSinceEpoch = ConvertToMillis(source.DaysSinceEpoch);
            DaySummary = new PlatformExposureSummaryData(source.SummaryData); ;
            ConfirmedClinicalDiagnosisSummary = GetExposureSummaryData(source.GetSummaryDataForReportType(AndroidReportType.ConfirmedClinicalDiagnosis));
            ConfirmedTestSummary = GetExposureSummaryData(source.GetSummaryDataForReportType(AndroidReportType.ConfirmedTest));
            RecursiveSummary = GetExposureSummaryData(source.GetSummaryDataForReportType(AndroidReportType.Recursive));
            SelfReportedSummary = GetExposureSummaryData(source.GetSummaryDataForReportType(AndroidReportType.SelfReport));
        }

        private static ExposureSummaryData? GetExposureSummaryData(AndroidDailySummary.ExposureSummaryData? summaryItem)
        {
            if (summaryItem == null)
            {
                return null;
            }
            return new PlatformExposureSummaryData(summaryItem);
        }

        private static long ConvertToMillis(int daysSinceEpoch) => daysSinceEpoch * 24 * 60 * 60 * 1000;

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public class PlatformExposureSummaryData : ExposureSummaryData
        {
            public PlatformExposureSummaryData() { }

            public PlatformExposureSummaryData(AndroidDailySummary.ExposureSummaryData source)
            {
                MaximumScore = source.MaximumScore;
                ScoreSum = source.ScoreSum;
                WeightedDurationSum = source.WeightedDurationSum;
            }

            public PlatformExposureSummaryData(ExposureSummaryData source)
            {
                MaximumScore = source.MaximumScore;
                ScoreSum = source.ScoreSum;
                WeightedDurationSum = source.WeightedDurationSum;
            }
        }
    }
}
