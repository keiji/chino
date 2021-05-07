namespace Chino
{
    /// <summary>
    /// Daily exposure summary to pass to client side.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary
    public interface IDailySummary
    {
        /// <summary>
        /// Returns days since epoch of the ExposureWindows that went into this summary.
        /// </summary>
        public long DaysSinceEpoch { get; }

        /// <summary>
        /// Summary of all exposures on this day.
        /// </summary>
        public IExposureSummaryData DaySummary { get; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.ClinicalDiagnosis.
        /// </summary>
        public IExposureSummaryData ConfirmedClinicalDiagnosisSummary { get; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.ConfirmedTest.
        /// </summary>
        public IExposureSummaryData ConfirmedTestSummary { get; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.Recursive.
        /// </summary>
        public IExposureSummaryData RecursiveSummary { get; }

        /// <summary>
        /// Summary of all exposures on this day of a specific diagnosis ReportType.SelfReported.
        /// </summary>
        public IExposureSummaryData SelfReportedSummary { get; }

        /// <summary>
        /// Stores different scores for specific ReportType.
        /// </summary>
        ///
        /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummary.ExposureSummaryData
        public interface IExposureSummaryData {

            /// <summary>
            /// Highest score of all ExposureWindows aggregated into this summary.
            /// </summary>
            public double MaximumScore { get; }

            /// <summary>
            /// Sum of scores for all ExposureWindows aggregated into this summary.
            /// </summary>
            public double ScoreSum { get; }

            /// <summary>
            /// Sum of weighted durations for all ExposureWindows aggregated into this summary.
            /// </summary>
            public double WeightedDurationSum { get; }
        }
    }
}
