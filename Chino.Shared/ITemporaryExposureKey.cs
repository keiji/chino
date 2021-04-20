namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public interface ITemporaryExposureKey
    {

        public int DaysSinceOnsetOfSymptoms { get; }

        public byte[] KeyData { get; }

        public int RollingPeriod { get; }

        public int RollingStartIntervalNumber { get; }

        public RiskLevel RiskLevel { get; }
    }

}
