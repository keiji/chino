using System;
using System.Linq;
using AndroidExposureWindow = Android.Gms.Nearby.ExposureNotification.ExposureWindow;
using AndroidScanInstance = Android.Gms.Nearby.ExposureNotification.ScanInstance;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public class PlatformExposureWindow : ExposureWindow
    {
        public PlatformExposureWindow() { }

        public PlatformExposureWindow(AndroidExposureWindow source)
        {
            CalibrationConfidence = (CalibrationConfidence)Enum.ToObject(typeof(CalibrationConfidence), source.CalibrationConfidence);
            DateMillisSinceEpoch = source.DateMillisSinceEpoch;
            Infectiousness = (Infectiousness)Enum.ToObject(typeof(Infectiousness), source.Infectiousness);
            ReportType = (ReportType)Enum.ToObject(typeof(ReportType), source.ReportType);
            ScanInstances = source.ScanInstances.Select(si => (ScanInstance)new PlatformScanInstance(si)).ToList();
        }
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ScanInstance
    public class PlatformScanInstance : ScanInstance
    {
        public PlatformScanInstance(AndroidScanInstance source)
        {
            MinAttenuationDb = source.MinAttenuationDb;
            SecondsSinceLastScan = source.SecondsSinceLastScan;
            TypicalAttenuationDb = source.TypicalAttenuationDb;
        }
    }
}
