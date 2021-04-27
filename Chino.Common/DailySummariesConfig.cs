using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/DailySummariesConfig
    [JsonObject]
    public class DailySummariesConfig
    {
        [JsonProperty("attenuation_bucket_threshold_db")]
        public IList<int> AttenuationBucketThresholdDb { get; set; } = new int[] { 50, 70, 90 };

        // Each element must be between 0 and 2.5
        [JsonProperty("attenuation_bucket_weights")]
        public IList<double> AttenuationBucketWeights { get; set; } = new double[] { 1.0, 1.0, 1.0, 1.0 };

        [JsonProperty("days_since_exposure_threshold")]
        public int DaysSinceExposureThreshold { get; set; } = 0;

        // Each element must be between 0 and 2.5
        [JsonProperty("infectiousness_weights")]
        public IDictionary<Infectiousness, double> InfectiousnessWeights { get; set; } = new Dictionary<Infectiousness, double>()
        {
            { Infectiousness.High, 1.0 },
            { Infectiousness.Standard, 1.0 },
            { Infectiousness.None, 1.0 },
        };

        [JsonProperty("minimum_risk_score")]
        public double MinimumWindowScore { get; set; } = 0.0;

        // Each element must be between 0 and 2.5
        [JsonProperty("report_type_weights")]
        public IDictionary<ReportType, double> ReportTypeWeights { get; set; } = new Dictionary<ReportType, double>()
        {
            { ReportType.ConfirmedTest, 1.0 },
            { ReportType.ConfirmedClinicalDiagnosis, 1.0 },
            { ReportType.SelfReport, 1.0 },
            { ReportType.Recursive, 1.0 },
            { ReportType.Revoked, 1.0 },
            { ReportType.Unknown, 1.0 }
        };
    }
}
