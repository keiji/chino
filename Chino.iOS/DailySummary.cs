using System;
using ExposureNotifications;
using Newtonsoft.Json;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposuredaysummary
    public class DailySummary : IDailySummary
    {
        [JsonIgnore]
        public ENExposureDaySummary Source;

        public DailySummary(ENExposureDaySummary source)
        {
            Source = source;
        }

        public long DaysSinceEpoch => Source.Date.GetDateMillisSinceEpoch();

        public IDailySummary.IExposureSummaryData? DaySummary => GetExposureSummaryData(Source.DaySummary);

        public IDailySummary.IExposureSummaryData? ConfirmedClinicalDiagnosisSummary => GetExposureSummaryData(Source.ConfirmedClinicalDiagnosisSummary);

        public IDailySummary.IExposureSummaryData? ConfirmedTestSummary => GetExposureSummaryData(Source.ConfirmedTestSummary);

        public IDailySummary.IExposureSummaryData? RecursiveSummary => GetExposureSummaryData(Source.RecursiveSummary);

        public IDailySummary.IExposureSummaryData? SelfReportedSummary => GetExposureSummaryData(Source.SelfReportedSummary);

        private static IDailySummary.IExposureSummaryData? GetExposureSummaryData(ENExposureSummaryItem? summaryItem)
        {
            if (summaryItem == null)
            {
                return null;
            }
            return new ExposureSummaryData(summaryItem);
        }

        public class ExposureSummaryData : IDailySummary.IExposureSummaryData
        {
            [JsonIgnore]
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
