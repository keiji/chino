using System;
using System.Linq;

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
    public class ExposureSummary
    {
        /// <summary>
        /// Array of durations in milliseconds at certain radio signal attenuations.
        /// </summary>
        public int[] AttenuationDurationsInMillis { get; set; }

        /// <summary>
        /// Days since last match to a diagnosis key from the server.
        /// </summary>
        public int DaysSinceLastExposure { get; set; }

        /// <summary>
        /// Number of matched diagnosis keys.
        /// </summary>
        public ulong MatchedKeyCount { get; set; }

        /// <summary>
        /// The highest risk score of all exposure incidents, it will be a value 0-4096.
        /// </summary>
        public int MaximumRiskScore { get; set; }

        /// <summary>
        /// The summation of risk scores of all exposure incidents.
        /// </summary>
        public int SummationRiskScore { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ExposureSummary summary))
            {
                return false;
            }

            bool attenuationDurationsInMillisEqual;
            if (AttenuationDurationsInMillis == summary.AttenuationDurationsInMillis)
            {
                attenuationDurationsInMillisEqual = true;
            }
            else if (AttenuationDurationsInMillis == null || summary.AttenuationDurationsInMillis == null)
            {
                attenuationDurationsInMillisEqual = false;
            }
            else
            {
                attenuationDurationsInMillisEqual = AttenuationDurationsInMillis.SequenceEqual(summary.AttenuationDurationsInMillis);
            }

            return
                   attenuationDurationsInMillisEqual &&
                   DaysSinceLastExposure == summary.DaysSinceLastExposure &&
                   MatchedKeyCount == summary.MatchedKeyCount &&
                   MaximumRiskScore == summary.MaximumRiskScore &&
                   SummationRiskScore == summary.SummationRiskScore;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AttenuationDurationsInMillis, DaysSinceLastExposure, MatchedKeyCount, MaximumRiskScore, SummationRiskScore);
        }
    }
}
