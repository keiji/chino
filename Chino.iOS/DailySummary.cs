using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredaysummary
    public class DailySummary: IDailySummary
    {
        public ENExposureDaySummary Source;

        public DailySummary(ENExposureDaySummary source)
        {
            Source = source;
        }

        public long DaysSinceEpoch => Utils.GetDateMillisSinceEpoch(Source.Date);

        public IDailySummary.IExposureSummaryData DaySummary => new ExposureSummaryData(Source.DaySummary);

        public IDailySummary.IExposureSummaryData ConfirmedClinicalDiagnosisSummary => new ExposureSummaryData(Source.ConfirmedClinicalDiagnosisSummary);

        public IDailySummary.IExposureSummaryData ConfirmedTestSummary => new ExposureSummaryData(Source.ConfirmedTestSummary);

        public IDailySummary.IExposureSummaryData RecursiveSummary => new ExposureSummaryData(Source.RecursiveSummary);

        public IDailySummary.IExposureSummaryData SelfReportedSummary => new ExposureSummaryData(Source.SelfReportedSummary);

        public class ExposureSummaryData : IDailySummary.IExposureSummaryData
        {
            public ENExposureSummaryItem Source;

            public ExposureSummaryData(ENExposureSummaryItem source)
            {
                Source = source;
            }

            public double MaximumScore => Source.MaximumScore;

            public double ScoreSum => Source.ScoreSum;

            public double WeightedDurationSum => Source.WeightedDurationSum;
        }
    }
}
