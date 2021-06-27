using System;

using AndroidTemporaryExposureKey = Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public class TemporaryExposureKey : ITemporaryExposureKey
    {
        public TemporaryExposureKey(AndroidTemporaryExposureKey source)
        {
            DaysSinceOnsetOfSymptoms = source.DaysSinceOnsetOfSymptoms;
            KeyData = source.GetKeyData();
            RollingPeriod = source.RollingPeriod;
            RollingStartIntervalNumber = source.RollingStartIntervalNumber;
            RiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
            ReportType = (ReportType)Enum.ToObject(typeof(ReportType), source.ReportType);
        }

        public int DaysSinceOnsetOfSymptoms { get; set; }
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public ReportType ReportType { get; set; }
    }
}
