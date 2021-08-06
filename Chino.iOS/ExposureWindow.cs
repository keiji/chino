using System;
using System.Linq;
using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/enexposurewindow
    public class PlatformExposureWindow : ExposureWindow
    {
        public PlatformExposureWindow() { }

        public PlatformExposureWindow(ENExposureWindow source)
        {
            CalibrationConfidence = (CalibrationConfidence)Enum.ToObject(typeof(CalibrationConfidence), source.CalibrationConfidence);
            DateMillisSinceEpoch = source.Date.GetDateMillisSinceEpoch();
            Infectiousness = (Infectiousness)Enum.ToObject(typeof(Infectiousness), source.Infectiousness);
            ReportType = (ReportType)Enum.ToObject(typeof(ReportType), source.DiagnosisReportType);
            ScanInstances = source.ScanInstances.Select(si => (ScanInstance)new PlatformScanInstance(si)).ToList();
        }
    }

    // https://developer.apple.com/documentation/exposurenotification/enscaninstance
    public class PlatformScanInstance : ScanInstance
    {
        public PlatformScanInstance() { }

        public PlatformScanInstance(ENScanInstance source)
        {
            MinAttenuationDb = source.MinimumAttenuation;
            SecondsSinceLastScan = (int)source.SecondsSinceLastScan;
            TypicalAttenuationDb = source.TypicalAttenuation;
        }
    }
}
