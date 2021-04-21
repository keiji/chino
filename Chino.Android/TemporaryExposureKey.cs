using System;

using AndroidTemporaryExposureKey = Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public class TemporaryExposureKey : ITemporaryExposureKey
    {

        public readonly AndroidTemporaryExposureKey Source;

        public TemporaryExposureKey(AndroidTemporaryExposureKey source)
        {
            Source = source;
        }

        public int DaysSinceOnsetOfSymptoms => Source.DaysSinceOnsetOfSymptoms;

        public byte[] KeyData => Source.GetKeyData();

        public ReportType ReportType => (ReportType)Enum.ToObject(typeof(ReportType), Source.ReportType);

        public int RollingPeriod => Source.RollingPeriod;

        public int RollingStartIntervalNumber => Source.RollingStartIntervalNumber;

        public RiskLevel RiskLevel => Source.TransmissionRiskLevel.ToRiskLevel();
    }
}
