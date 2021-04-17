using System;
using System.Collections.Generic;
using System.Linq;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public class ExposureWindow : IExposureWindow
    {
        public readonly Android.Gms.Nearby.ExposureNotification.ExposureWindow Source;

        public ExposureWindow(Android.Gms.Nearby.ExposureNotification.ExposureWindow source)
        {
            Source = source;
        }

        public CalibrationConfidence CalibrationConfidence => (CalibrationConfidence)Enum.ToObject(typeof(CalibrationConfidence), Source.CalibrationConfidence);

        public long DateMillisSinceEpoch => Source.DateMillisSinceEpoch;

        public Infectiousness Infectiousness => (Infectiousness)Enum.ToObject(typeof(Infectiousness), Source.Infectiousness);

        public ReportType ReportType => (ReportType)Enum.ToObject(typeof(ReportType), Source.ReportType);

        public List<IScanInstance> ScanInstances => Source.ScanInstances.Select(si => (IScanInstance)new ScanInstance(si)).ToList();
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ScanInstance
    public class ScanInstance : IScanInstance
    {
        public readonly Android.Gms.Nearby.ExposureNotification.ScanInstance Source;

        public ScanInstance(Android.Gms.Nearby.ExposureNotification.ScanInstance source)
        {
            Source = source;
        }

        public int MinAttenuationDb => Source.MinAttenuationDb;

        public int SecondsSinceLastScan => Source.SecondsSinceLastScan;

        public int TypicalAttenuationDb => Source.TypicalAttenuationDb;
    }
}
