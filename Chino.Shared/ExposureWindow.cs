using System.Collections.Generic;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public class ExposureWindow
    {
        public ExposureWindow()
        {
        }

        public CalibrationConfidence CalibrationConfidence { get; set; }

        public long DateMillisSinceEpoch { get; set; }

        public Infectiousness Infectiousness { get; set; }

        public ReportType ReportType { get; set; }

        public List<ScanInstance> ScanInstances { get; set; }
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/CalibrationConfidence
    public enum CalibrationConfidence {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        Hight = 3
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ScanInstance
    public class ScanInstance
    {
        public int MinAttenuationDb { get; set; }
        public int SecondsSinceLastScan { get; set; }
        public int TypicalAttenuationDb { get; set; }
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/Infectiousness
    public enum Infectiousness
    {
        None = 0,
        Standard = 1,
        Hight = 2
    }

}
