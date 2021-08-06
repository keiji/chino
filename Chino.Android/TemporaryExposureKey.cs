using System;

using AndroidTemporaryExposureKey = Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public class PlatformTemporaryExposureKey : TemporaryExposureKey
    {
        public PlatformTemporaryExposureKey() { }

        public PlatformTemporaryExposureKey(AndroidTemporaryExposureKey source)
        {
            DaysSinceOnsetOfSymptoms = source.DaysSinceOnsetOfSymptoms;
            KeyData = source.GetKeyData();
            RollingPeriod = source.RollingPeriod;
            RollingStartIntervalNumber = source.RollingStartIntervalNumber;
            RiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
            ReportType = (ReportType)Enum.ToObject(typeof(ReportType), source.ReportType);
        }
    }
}
