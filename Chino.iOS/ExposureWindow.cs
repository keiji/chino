using System;
using System.Collections.Generic;
using System.Linq;
using ExposureNotifications;
using Foundation;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposurewindow
    public class ExposureWindow : IExposureWindow
    {
        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public readonly ENExposureWindow Source;

        public ExposureWindow(ENExposureWindow source)
        {
            Source = source;
        }

        public CalibrationConfidence CalibrationConfidence => (CalibrationConfidence)Enum.ToObject(typeof(CalibrationConfidence), Source.CalibrationConfidence);

        public long DateMillisSinceEpoch => GetDateMillisSinceEpoch(Source.Date);

        public Infectiousness Infectiousness => (Infectiousness)Enum.ToObject(typeof(Infectiousness), Source.Infectiousness);

        public ReportType ReportType => (ReportType)Enum.ToObject(typeof(ReportType), Source.DiagnosisReportType);

        public List<IScanInstance> ScanInstances => Source.ScanInstances.Select(si => (IScanInstance)new ScanInstance(si)).ToList();

        private static long GetDateMillisSinceEpoch(NSDate date)
        {
            DateTime dateTime = (DateTime)date;

            // TODO: Check TimeZone
            var dto = new DateTimeOffset(dateTime.Ticks, new TimeSpan(0, 00, 00));

            return dto.ToUnixTimeMilliseconds();
        }
    }

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
