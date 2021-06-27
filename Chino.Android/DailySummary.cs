using Newtonsoft.Json;
using AndroidDailySummary = Android.Gms.Nearby.ExposureNotification.DailySummary;
using AndroidReportType = Android.Gms.Nearby.ExposureNotification.ReportType;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class DailySummary: IDailySummary
    {
        [JsonIgnore]
        public AndroidDailySummary Source;

        public DailySummary(AndroidDailySummary source)
        {
            Source = source;
        }

        public long DateMillisSinceEpoch => ConvertToMillis(Source.DaysSinceEpoch);

        public IDailySummary.IExposureSummaryData DaySummary => new ExposureSummaryData(Source.SummaryData);

        public IDailySummary.IExposureSummaryData ConfirmedClinicalDiagnosisSummary
            => GetExposureSummaryData(Source.GetSummaryDataForReportType(AndroidReportType.ConfirmedClinicalDiagnosis));

        public IDailySummary.IExposureSummaryData ConfirmedTestSummary
            => GetExposureSummaryData(Source.GetSummaryDataForReportType(AndroidReportType.ConfirmedTest));

        public IDailySummary.IExposureSummaryData RecursiveSummary
            => GetExposureSummaryData(Source.GetSummaryDataForReportType(AndroidReportType.Recursive));

        public IDailySummary.IExposureSummaryData SelfReportedSummary
            => GetExposureSummaryData(Source.GetSummaryDataForReportType(AndroidReportType.SelfReport));

        private static IDailySummary.IExposureSummaryData? GetExposureSummaryData(AndroidDailySummary.ExposureSummaryData? summaryItem)
        {
            if (summaryItem == null)
            {
                return null;
            }
            return new ExposureSummaryData(summaryItem);
        }

        private static long ConvertToMillis(int daysSinceEpoch) => daysSinceEpoch * 24 * 60 * 60 * 1000;

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public class ExposureSummaryData : IDailySummary.IExposureSummaryData
        {
            [JsonIgnore]
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
