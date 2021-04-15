namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureInformation
    public class ExposureInformation
    {

        public int[] AttenuationDurationsInMinutes { get; set; }

        public int AttenuationValue { get; set; }

        public long DateMillisSinceEpoch { get; set; }

        public int DurationMinutes { get; set; }

        public int TotalRiskScore { get; set; }

        public RiskLevel TransmissionRiskLevel { get; set; }
    }
}
