using System;
namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public class TemporaryExposureKey : ITemporaryExposureKey
    {

        public readonly Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey Source;

        public TemporaryExposureKey(Android.Gms.Nearby.ExposureNotification.TemporaryExposureKey source)
        {
            Source = source;
        }

        public int DaysSinceOnsetOfSymptoms => throw new NotImplementedException();

        public byte[] KeyData => Source.GetKeyData();

        public ReportType ReportType => (ReportType)Enum.ToObject(typeof(ReportType), Source.ReportType);

        public int RollingPeriod => Source.RollingPeriod;

        public int RollingStartIntervalNumber => Source.RollingStartIntervalNumber;

        public RiskLevel RiskLevel => Source.TransmissionRiskLevel.ToRiskLevel();
    }
}
