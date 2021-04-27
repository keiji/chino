using System;
namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    public interface IExposureSummary
    {
        public int[] AttenuationDurationsInMinutes { get; }

        public int DaysSinceLastExposure { get; }

        public long MatchedKeyCount { get; }

        public int MaximumRiskScore { get; }

        public int SummationRiskScore { get; }

    }
}
