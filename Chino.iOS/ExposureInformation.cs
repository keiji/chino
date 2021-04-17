using System;
using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enexposureinfo
    public class ExposureInformation : IExposureInformation
    {
        public readonly ENExposureInfo Source;

        public ExposureInformation(ENExposureInfo source)
        {
            Source = source;
        }

        public int[] AttenuationDurationsInMinutes => Source.AttenuationDurations;

        public int AttenuationValue => Source.AttenuationValue;

        public long DateMillisSinceEpoch => Utils.GetDateMillisSinceEpoch(Source.Date);

        public double Duration => Source.Duration;

        public int TotalRiskScore => Source.TotalRiskScore;

        public RiskLevel TransmissionRiskLevel => (RiskLevel)Enum.ToObject(typeof(RiskLevel), Source.TransmissionRiskLevel);
    }
}
