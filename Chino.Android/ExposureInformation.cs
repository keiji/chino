using System;
using System.Linq;
using AndroidExposureInformation = Android.Gms.Nearby.ExposureNotification.ExposureInformation;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureInformation
    [Obsolete]
    public class PlatformExposureInformation : ExposureInformation
    {
        private const int MINUTE_IN_MILLIS = 60 * 1000;

        public PlatformExposureInformation() { }

        public PlatformExposureInformation(AndroidExposureInformation source)
        {
            AttenuationDurationsInMillis = ConvertToMillis(source.GetAttenuationDurationsInMinutes());
            AttenuationValue = source.AttenuationValue;
            DateMillisSinceEpoch = source.DateMillisSinceEpoch;
            DurationInMillis = source.DurationMinutes * MINUTE_IN_MILLIS;
            TotalRiskScore = source.TotalRiskScore;
            TransmissionRiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
        }

        private static int[] ConvertToMillis(int[] attenuationDurationsInMinutes)
            => attenuationDurationsInMinutes.Select(d => d * MINUTE_IN_MILLIS).ToArray();
    }
}
