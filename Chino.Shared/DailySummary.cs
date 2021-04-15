namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class DailySummary
    {
        public int DaysSinceEpoch { get; set; }

        public ExposureSummaryData SummaryData { get; set; }

        //public ExposureSummaryData GetSummaryDataForReportType(int reportType) {
        //}

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public class ExposureSummaryData {

            public double MaximumScore { get; set; }
            public double ScoreSum { get; set; }
            public double WeightedDurationSum { get; set; }
        }
    }
}
