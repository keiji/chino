using System;

using AndroidExposureSummary = Android.Gms.Nearby.ExposureNotification.ExposureSummary;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    [Obsolete]
    public class ExposureSummary: IExposureSummary
    {
        public readonly AndroidExposureSummary Source;

        public ExposureSummary(AndroidExposureSummary source)
        {
            Source = source;
        }

        public int[] AttenuationDurationsInMinutes => Source.GetAttenuationDurationsInMinutes();

        public int DaysSinceLastExposure => Source.DaysSinceLastExposure;

        public long MatchedKeyCount => Source.MatchedKeyCount;

        public int MaximumRiskScore => Source.MaximumRiskScore;

        public int SummationRiskScore => Source.SummationRiskScore;
    }
}
