namespace Chino
{
    /// <summary>
    /// Information about an exposure, meaning a single diagnosis key over a contiguous period of time specified by durationMinutes.
    ///
    /// This class is deprecated.
    /// no longer used with Exposure Window API.
    ///
    /// The client can get the exposure information via ExposureNotificationClient.getExposureInformation(String).
    /// </summary>
    public class ExposureInformation
    {
        /// <summary>
        /// Array of durations in milliseconds at certain radio signal attenuations.
        /// </summary>
        public int[] AttenuationDurationsInMillis { get; set; }

        /// <summary>
        /// The time-weighted signal strength attenuation value which goes into getTotalRiskScore().
        /// </summary>
        public int AttenuationValue { get; set; }

        /// <summary>
        /// Day-level resolution that the exposure occurred, in milliseconds since epoch.
        /// </summary>
        public long DateMillisSinceEpoch { get; set; }

        /// <summary>
        /// Length of exposure in milliseconds, with a 30 minute maximum.
        /// </summary>
        public double DurationInMillis { get; set; }

        /// <summary>
        /// The total risk calculated for the exposure.
        /// </summary>
        public int TotalRiskScore { get; set; }

        /// <summary>
        /// The transmission risk associated with the matched diagnosis key.
        /// </summary>
        public RiskLevel TransmissionRiskLevel { get; set; }
    }
}
