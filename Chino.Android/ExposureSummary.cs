using System;
using System.Linq;
using AndroidExposureSummary = Android.Gms.Nearby.ExposureNotification.ExposureSummary;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    [Obsolete]
    public class PlatformExposureSummary : ExposureSummary
    {
        private const int MINUTE_IN_MILLIS = 60 * 1000;

        public PlatformExposureSummary() { }

        public PlatformExposureSummary(AndroidExposureSummary source)
        {
            AttenuationDurationsInMillis = ConvertToMillis(source.GetAttenuationDurationsInMinutes());
            DaysSinceLastExposure = source.DaysSinceLastExposure;
            MatchedKeyCount = (ulong)source.MatchedKeyCount;
            MaximumRiskScore = source.MaximumRiskScore;
            SummationRiskScore = source.SummationRiskScore;
        }

        private static int[] ConvertToMillis(int[] attenuationDurationsInMinutes)
            => attenuationDurationsInMinutes.Select(d => d * MINUTE_IN_MILLIS).ToArray();
    }
}
