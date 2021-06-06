using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using AndroidExposureWindow = Android.Gms.Nearby.ExposureNotification.ExposureWindow;
using AndroidScanInstance = Android.Gms.Nearby.ExposureNotification.ScanInstance;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public class ExposureWindow : IExposureWindow
    {
        [JsonIgnore]
        public readonly AndroidExposureWindow Source;

        public ExposureWindow(AndroidExposureWindow source)
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
        [JsonIgnore]
        public readonly AndroidScanInstance Source;

        public ScanInstance(AndroidScanInstance source)
        {
            Source = source;
        }

        public int MinAttenuationDb => Source.MinAttenuationDb;

        public int SecondsSinceLastScan => Source.SecondsSinceLastScan;

        public int TypicalAttenuationDb => Source.TypicalAttenuationDb;
    }
}
