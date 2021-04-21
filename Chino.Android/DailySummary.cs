using AndroidDailySummary = Android.Gms.Nearby.ExposureNotification.DailySummary;
using AndroidReportType = Android.Gms.Nearby.ExposureNotification.ReportType;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class DailySummary: IDailySummary
    {
        public AndroidDailySummary Source;

        public DailySummary(AndroidDailySummary source)
        {
            Source = source;
        }

        public long DaysSinceEpoch => Source.DaysSinceEpoch;

        public IDailySummary.IExposureSummaryData DaySummary => new ExposureSummaryData(Source.SummaryData);

        public IDailySummary.IExposureSummaryData ConfirmedClinicalDiagnosisSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(AndroidReportType.ConfirmedClinicalDiagnosis)
                );

        public IDailySummary.IExposureSummaryData ConfirmedTestSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(AndroidReportType.ConfirmedTest)
                );

        public IDailySummary.IExposureSummaryData RecursiveSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(AndroidReportType.Recursive)
                );

        public IDailySummary.IExposureSummaryData SelfReportedSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(AndroidReportType.SelfReport)
                );

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public class ExposureSummaryData : IDailySummary.IExposureSummaryData
        {
            public AndroidDailySummary.ExposureSummaryData Source;

            public ExposureSummaryData(AndroidDailySummary.ExposureSummaryData source)
            {
                Source = source;
            }

            public double MaximumScore => Source.MaximumScore;

            public double ScoreSum => Source.ScoreSum;

            public double WeightedDurationSum => Source.WeightedDurationSum;
        }
    }
}
