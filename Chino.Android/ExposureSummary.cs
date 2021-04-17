using System;
namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureSummary
    [Obsolete]
    public class ExposureSummary: IExposureSummary
    {
        public readonly Android.Gms.Nearby.ExposureNotification.ExposureSummary Source;

        public ExposureSummary(Android.Gms.Nearby.ExposureNotification.ExposureSummary source)
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
