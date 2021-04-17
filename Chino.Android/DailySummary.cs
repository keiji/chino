using System;
namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class DailySummary: IDailySummary
    {
        public Android.Gms.Nearby.ExposureNotification.DailySummary Source;

        public DailySummary(Android.Gms.Nearby.ExposureNotification.DailySummary source)
        {
            Source = source;
        }

        public long DaysSinceEpoch => Source.DaysSinceEpoch;

        public IDailySummary.IExposureSummaryData DaySummary => new ExposureSummaryData(Source.SummaryData);

        public IDailySummary.IExposureSummaryData ConfirmedClinicalDiagnosisSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(Android.Gms.Nearby.ExposureNotification.ReportType.ConfirmedClinicalDiagnosis)
                );

        public IDailySummary.IExposureSummaryData ConfirmedTestSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(Android.Gms.Nearby.ExposureNotification.ReportType.ConfirmedTest)
                );

        public IDailySummary.IExposureSummaryData RecursiveSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(Android.Gms.Nearby.ExposureNotification.ReportType.Recursive)
                );

        public IDailySummary.IExposureSummaryData SelfReportedSummary =>
            new ExposureSummaryData(
                Source.GetSummaryDataForReportType(Android.Gms.Nearby.ExposureNotification.ReportType.SelfReport)
                );

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public class ExposureSummaryData : IDailySummary.IExposureSummaryData
        {
            public Android.Gms.Nearby.ExposureNotification.DailySummary.ExposureSummaryData Source;

            public ExposureSummaryData(Android.Gms.Nearby.ExposureNotification.DailySummary.ExposureSummaryData source)
            {
                Source = source;
            }

            public double MaximumScore => Source.MaximumScore;

            public double ScoreSum => Source.ScoreSum;

            public double WeightedDurationSum => Source.WeightedDurationSum;
        }
    }
}
