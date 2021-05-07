namespace Chino
{
    /// <summary>
    /// Summary information about recent exposures.
    ///
    /// This class is deprecated.
    /// no longer used with Exposure Window API.
    ///
    /// The client can get this information via ExposureNotificationClient.getExposureSummary(String).
    /// </summary>
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    public interface IExposureSummary
    {
        /// <summary>
        /// Array of durations in minutes at certain radio signal attenuations.
        /// </summary>
        public int[] AttenuationDurationsInMinutes { get; }

        /// <summary>
        /// Days since last match to a diagnosis key from the server.
        /// </summary>
        public int DaysSinceLastExposure { get; }

        /// <summary>
        /// Number of matched diagnosis keys.
        /// </summary>
        public long MatchedKeyCount { get; }

        /// <summary>
        /// The highest risk score of all exposure incidents, it will be a value 0-4096.
        /// </summary>
        public int MaximumRiskScore { get; }

        /// <summary>
        /// The summation of risk scores of all exposure incidents.
        /// </summary>
        public int SummationRiskScore { get; }

    }
}
