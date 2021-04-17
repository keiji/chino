namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public interface IDailySummary
    {
        public long DaysSinceEpoch { get; }

        public IExposureSummaryData DaySummary { get; }

        public IExposureSummaryData ConfirmedClinicalDiagnosisSummary { get; }
        public IExposureSummaryData ConfirmedTestSummary { get; }
        public IExposureSummaryData RecursiveSummary { get; }
        public IExposureSummaryData SelfReportedSummary { get; }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public interface IExposureSummaryData {

            public double MaximumScore { get; }
            public double ScoreSum { get; }
            public double WeightedDurationSum { get; }
        }
    }
}
