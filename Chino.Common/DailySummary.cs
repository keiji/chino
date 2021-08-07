namespace Chino
{
    /// <summary>
    /// Daily exposure summary to pass to client side.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public class DailySummary
    {
        /// <summary>
        /// Returns days since epoch of the ExposureWindows that went into this summary.
        /// </summary>
        public long DateMillisSinceEpoch { get; set; }

        /// <summary>
        /// Summary of all exposures on this day.
        /// </summary>
        public ExposureSummaryData DaySummary { get; set; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.ClinicalDiagnosis.
        /// </summary>
        public ExposureSummaryData ConfirmedClinicalDiagnosisSummary { get; set; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.ConfirmedTest.
        /// </summary>
        public ExposureSummaryData ConfirmedTestSummary { get; set; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.Recursive.
        /// </summary>
        public ExposureSummaryData RecursiveSummary { get; set; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.SelfReported.
        /// </summary>
        public ExposureSummaryData SelfReportedSummary { get; set; }
    }

    /// <summary>
    /// Stores different scores for specific ReportType.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
    public class ExposureSummaryData
    {

        /// <summary>
        /// Highest score of all ExposureWindows aggregated into this summary.
        /// </summary>
        public double MaximumScore { get; set; }

        /// <summary>
        /// Sum of scores for all ExposureWindows aggregated into this summary.
        /// </summary>
        public double ScoreSum { get; set; }

        /// <summary>
        /// Sum of weighted durations for all ExposureWindows aggregated into this summary.
        /// </summary>
        public double WeightedDurationSum { get; set; }
    }
}
