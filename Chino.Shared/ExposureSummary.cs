using System;
namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    public class ExposureSummary
    {

        public int[] AttenuationDurationsInMinutes { get; set; }

        public int DaysSinceLastExposure { get; set; }

        public int MatchedKeyCount { get; set; }

        public int MaximumRiskScore { get; set; }

        public int SummationRiskScore { get; set; }

    }
}
