using System.Collections.Generic;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummariesConfig
    public class DailySummariesConfig
    {
        public List<int> AttenuationBucketThresholdDb { get; set; }

        public List<double> AttenuationBucketWeights { get; set; }

        public int DaysSinceExposureThreshold { get; set; }

        public IDictionary<Infectiousness, double> InfectiousnessWeights { get; set; }

        public double MinimumWindowScore { get; set; }

        public IDictionary<ReportType, double> ReportTypeWeights { get; set; }

    }
}
