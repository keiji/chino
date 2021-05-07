namespace Chino
{
    /// <summary>
    /// An interface of a key generated for advertising over a window of time.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey
    public interface ITemporaryExposureKey
    {
        /// <summary>
        /// Number of days elapsed between symptom onset and the key being used.
        /// </summary>
        public int DaysSinceOnsetOfSymptoms { get; set; }

        /// <summary>
        /// The randomly generated Temporary Exposure Key information.
        /// </summary>
        public byte[] KeyData { get; set; }

        /// <summary>
        /// A number describing how long a key is valid.
        /// </summary>
        public int RollingPeriod { get; set; }

        /// <summary>
        /// A number describing when a key starts.
        /// </summary>
        public int RollingStartIntervalNumber { get; set; }

        /// <summary>
        /// Risk of transmission associated with the person this key came from.
        /// </summary>
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Type of diagnosis associated with a key.
        /// </summary>
        public ReportType ReportType { get; set; }
    }
}
