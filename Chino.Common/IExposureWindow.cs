using System.Collections.Generic;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public interface IExposureWindow
    {
        public CalibrationConfidence CalibrationConfidence { get; }

        public long DateMillisSinceEpoch { get; }

        public Infectiousness Infectiousness { get; }

        public ReportType ReportType { get; }

        public List<IScanInstance> ScanInstances { get; }
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/CalibrationConfidence
    public enum CalibrationConfidence {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        Hight = 3
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ScanInstance
    public interface IScanInstance
    {
        public int MinAttenuationDb { get; }
        public int SecondsSinceLastScan { get; }
        public int TypicalAttenuationDb { get; }
    }

    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/Infectiousness
    public enum Infectiousness
    {
        None = 0,
        Standard = 1,
        High = 2
    }

}
