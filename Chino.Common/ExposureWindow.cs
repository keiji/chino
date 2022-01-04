using System;
using System.Collections.Generic;
using System.Linq;

namespace Chino
{
    /// <summary>
    /// A duration of up to 30 minutes during which beacons from a TEK were observed.
    ///
    /// Each ExposureWindow corresponds to a single TEK, but one TEK can lead to several ExposureWindow due to random 15-30 minutes cuts.
    /// See getExposureWindows() for more info.
    /// The TEK itself isn't exposed by the API.
    /// </summary>
    ///
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureWindow
    public class ExposureWindow
    {
        /// <summary>
        /// Confidence of the BLE Transmit power calibration of the transmitting device.
        /// </summary>
        public CalibrationConfidence CalibrationConfidence { get; set; }

        /// <summary>
        /// Returns the epoch time in milliseconds the exposure occurred.
        /// </summary>
        public long DateMillisSinceEpoch { get; set; }

        /// <summary>
        /// Infectiousness of the TEK that caused this exposure, computed from the days since onset of symptoms using the daysToInfectiousnessMapping.
        /// </summary>
        public Infectiousness Infectiousness { get; set; }

        /// <summary>
        /// Report Type of the TEK that caused this exposure
        /// TEKs with no report type set are returned with reportType = CONFIRMED_TEST.
        /// </summary>
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Sightings of this ExposureWindow, time-ordered.
        /// </summary>
        public IList<ScanInstance> ScanInstances { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ExposureWindow window))
            {
                return false;
            }

            bool scanInstanceEqual;
            if (ScanInstances == window.ScanInstances)
            {
                scanInstanceEqual = true;
            }
            else if (ScanInstances == null || window.ScanInstances == null)
            {
                scanInstanceEqual = false;
            }
            else
            {
                scanInstanceEqual = ScanInstances.SequenceEqual(
                    window.ScanInstances, new ScanInstance.EqualityComparer());
            }

            return
                   CalibrationConfidence == window.CalibrationConfidence &&
                   DateMillisSinceEpoch == window.DateMillisSinceEpoch &&
                   Infectiousness == window.Infectiousness &&
                   ReportType == window.ReportType &&
                   scanInstanceEqual;
        }

        public override int GetHashCode()
        {
            const int seed = 487;
            const int modifier = 31;

            int scanInstancesHashCode;
            if (ScanInstances == null)
            {
                scanInstancesHashCode = 0;
            }
            else
            {
                scanInstancesHashCode = ScanInstances.Aggregate(seed, (current, item) => (current * modifier) + item.GetHashCode());
            }

            return HashCode.Combine(CalibrationConfidence, DateMillisSinceEpoch, Infectiousness, ReportType, scanInstancesHashCode);
        }

        public class EqualityComparer : IEqualityComparer<ExposureWindow>
        {
            public bool Equals(ExposureWindow x, ExposureWindow y)
                => x.Equals(y);

            public int GetHashCode(ExposureWindow obj)
                => obj.GetHashCode();
        }

        public class Comparer : Comparer<ExposureWindow>
        {
            public override int Compare(ExposureWindow x, ExposureWindow y)
            {
                if (x.DateMillisSinceEpoch < y.DateMillisSinceEpoch)
                {
                    return -1;
                }
                else if (x.DateMillisSinceEpoch > y.DateMillisSinceEpoch)
                {
                    return 1;
                }
                else if (x.ReportType < y.ReportType)
                {
                    return -1;
                }
                else if (x.ReportType > y.ReportType)
                {
                    return 1;
                }
                else if (x.Infectiousness < y.Infectiousness)
                {
                    return 1;
                }
                else if (x.Infectiousness > y.Infectiousness)
                {
                    return -1;
                }
                else if (x.CalibrationConfidence < y.CalibrationConfidence)
                {
                    return 1;
                }
                else if (x.CalibrationConfidence > y.CalibrationConfidence)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

    }

    /// <summary>
    /// Calibration confidence defined for an ExposureWindow.
    /// </summary>
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/CalibrationConfidence
    public enum CalibrationConfidence
    {

        /// <summary>
        /// No calibration data, using fleet-wide as default options.
        /// </summary>
        Lowest = 0,

        /// <summary>
        /// Using average calibration over models from manufacturer.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Using single-antenna orientation for a similar model.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Using significant calibration data for this model.
        /// </summary>
        High = 3
    }

    /// <summary>
    /// Information about the sighting of a TEK within a BLE scan (of a few seconds).
    ///
    /// The TEK itself isn't exposed by the API.
    /// </summary>
    ///
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ScanInstance
    public class ScanInstance
    {
        /// <summary>
        /// Minimum attenuation of all of this TEK's beacons received during the scan, in dB.
        /// </summary>
        public int MinAttenuationDb { get; set; }

        /// <summary>
        /// Seconds elapsed since the previous scan, typically used as a weight.
        /// </summary>
        public int SecondsSinceLastScan { get; set; }

        /// <summary>
        /// Aggregation of the attenuations of all of this TEK's beacons received during the scan, in dB.
        /// </summary>
        public int TypicalAttenuationDb { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ScanInstance instance &&
                   MinAttenuationDb == instance.MinAttenuationDb &&
                   SecondsSinceLastScan == instance.SecondsSinceLastScan &&
                   TypicalAttenuationDb == instance.TypicalAttenuationDb;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MinAttenuationDb, SecondsSinceLastScan, TypicalAttenuationDb);
        }

        public class EqualityComparer : IEqualityComparer<ScanInstance>
        {
            public bool Equals(ScanInstance x, ScanInstance y)
                => x.Equals(y);

            public int GetHashCode(ScanInstance obj)
                => obj.GetHashCode();
        }

        public class Comparer : Comparer<ScanInstance>
        {
            public override int Compare(ScanInstance x, ScanInstance y)
            {
                if (x.MinAttenuationDb < y.MinAttenuationDb)
                {
                    return 1;
                }
                else if (x.MinAttenuationDb > y.MinAttenuationDb)
                {
                    return -1;
                }
                else if (x.SecondsSinceLastScan < y.SecondsSinceLastScan)
                {
                    return 1;
                }
                else if (x.SecondsSinceLastScan > y.SecondsSinceLastScan)
                {
                    return -1;
                }
                else if (x.TypicalAttenuationDb < y.TypicalAttenuationDb)
                {
                    return 1;
                }
                else if (x.TypicalAttenuationDb > y.TypicalAttenuationDb)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    /// <summary>
    /// Infectiousness defined for an ExposureWindow.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/Infectiousness
    public enum Infectiousness
    {
        None = 0,
        Standard = 1,
        High = 2
    }

}
