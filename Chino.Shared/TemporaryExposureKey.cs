namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public class TemporaryExposureKey
    {

        public TemporaryExposureKey() { }

        public TemporaryExposureKey(byte[] keyData, int rollingPeriod, int rollingStartIntervalNumber, RiskLevel riskLevel)
        {
            KeyData = keyData;
            RollingPeriod = rollingPeriod;
            RollingStartIntervalNumber = rollingStartIntervalNumber;
            RiskLevel = riskLevel;
        }

        public TemporaryExposureKey(byte[] keyData, ReportType reportType, int rollingPeriod, int rollingStartIntervalNumber, RiskLevel transmissionRiskLevel)
            : this(keyData, rollingPeriod, rollingStartIntervalNumber, transmissionRiskLevel)
        {
            ReportType = reportType;
        }

        public int DaysSinceOnsetOfSymptoms { get; set; }

        public byte[] KeyData { get; set; }

        public ReportType ReportType { get; set; }

        public int RollingPeriod { get; set; }

        public int RollingStartIntervalNumber { get; set; }

        public RiskLevel RiskLevel { get; set; }
    }

}
