using System;
using Newtonsoft.Json;
using AndroidExposureInformation = Android.Gms.Nearby.ExposureNotification.ExposureInformation;

namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureInformation
    [Obsolete]
    public class ExposureInformation : IExposureInformation
    {
        [JsonIgnore]
        public AndroidExposureInformation Source;

        public ExposureInformation(AndroidExposureInformation source)
        {
            Source = source;
        }

        public int[] AttenuationDurationsInMinutes => Source.GetAttenuationDurationsInMinutes();

        public int AttenuationValue => Source.AttenuationValue;

        public long DateMillisSinceEpoch => Source.DateMillisSinceEpoch;

        public double Duration => Source.DurationMinutes;

        public int TotalRiskScore => Source.TotalRiskScore;

        public RiskLevel TransmissionRiskLevel => (RiskLevel)Enum.ToObject(typeof(RiskLevel), Source.TransmissionRiskLevel);
    }
}
