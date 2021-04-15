namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
    public class ExposureConfiguration
    {
        public int[] AttenuationScores { get; set; }

        public int AttenuationWeight { get; set; }

        public int DaysSinceLastExposureScores { get; set; }

        public int DaysSinceLastExposureWeight { get; set; }

        public int[] DurationAtAttenuationThresholds { get; set; }

        public int[] DurationScores { get; set; }

        public int DurationWeight { get; set; }

        public int MinimumRiskScore { get; set; }

        public int[] TransmissionRiskScores { get; set; }

        public int TransmissionRiskWeight { get; set; }
    }
}
