using System;
using System.Collections.Generic;
using System.Linq;
using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposurewindow
    public class ExposureWindow : IExposureWindow
    {
        public readonly ENExposureWindow Source;

        public ExposureWindow(ENExposureWindow source)
        {
            Source = source;
        }

        public CalibrationConfidence CalibrationConfidence => (CalibrationConfidence)Enum.ToObject(typeof(CalibrationConfidence), Source.CalibrationConfidence);

        public long DateMillisSinceEpoch => Source.Date.GetDateMillisSinceEpoch();

        public Infectiousness Infectiousness => (Infectiousness)Enum.ToObject(typeof(Infectiousness), Source.Infectiousness);

        public ReportType ReportType => (ReportType)Enum.ToObject(typeof(ReportType), Source.DiagnosisReportType);

        public List<IScanInstance> ScanInstances => Source.ScanInstances.Select(si => (IScanInstance)new ScanInstance(si)).ToList();
    }

    // https://developer.apple.com/documentation/exposurenotification/enscaninstance
    public class ScanInstance : IScanInstance
    {
        public readonly ENScanInstance Source;

        public ScanInstance(ENScanInstance source)
        {
            Source = source;
        }

        public int MinAttenuationDb => Source.MinimumAttenuation;

        public int SecondsSinceLastScan => (int)Source.SecondsSinceLastScan;

        public int TypicalAttenuationDb => Source.TypicalAttenuation;
    }
}
