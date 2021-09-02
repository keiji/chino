using System;
using AndroidExposureSummary = Android.Gms.Nearby.ExposureNotification.ExposureSummary;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    [Obsolete]
    public class PlatformExposureSummary : ExposureSummary
    {
        public PlatformExposureSummary() { }

        public PlatformExposureSummary(AndroidExposureSummary source)
        {
            AttenuationDurationsInMinutes = source.GetAttenuationDurationsInMinutes();
            DaysSinceLastExposure = source.DaysSinceLastExposure;
            MatchedKeyCount = (ulong)source.MatchedKeyCount;
            MaximumRiskScore = source.MaximumRiskScore;
            SummationRiskScore = source.SummationRiskScore;
        }
    }
}
