using System;
using System.Collections.Generic;

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

        public override bool Equals(object obj)
        {
            return obj is DailySummary summary &&
                   DateMillisSinceEpoch == summary.DateMillisSinceEpoch &&
                   EqualityComparer<ExposureSummaryData>.Default.Equals(DaySummary, summary.DaySummary) &&
                   EqualityComparer<ExposureSummaryData>.Default.Equals(ConfirmedClinicalDiagnosisSummary, summary.ConfirmedClinicalDiagnosisSummary) &&
                   EqualityComparer<ExposureSummaryData>.Default.Equals(ConfirmedTestSummary, summary.ConfirmedTestSummary) &&
                   EqualityComparer<ExposureSummaryData>.Default.Equals(RecursiveSummary, summary.RecursiveSummary) &&
                   EqualityComparer<ExposureSummaryData>.Default.Equals(SelfReportedSummary, summary.SelfReportedSummary);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DateMillisSinceEpoch, DaySummary, ConfirmedClinicalDiagnosisSummary, ConfirmedTestSummary, RecursiveSummary, SelfReportedSummary);
        }
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

        public override bool Equals(object obj)
        {
            return obj is ExposureSummaryData data &&
                   MaximumScore == data.MaximumScore &&
                   ScoreSum == data.ScoreSum &&
                   WeightedDurationSum == data.WeightedDurationSum;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MaximumScore, ScoreSum, WeightedDurationSum);
        }
    }
}
