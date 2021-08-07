using System.Collections.Generic;

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
