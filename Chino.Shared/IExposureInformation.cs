namespace Chino
{
    public interface IExposureInformation
    {

        public int[] AttenuationDurationsInMinutes { get; }

        public int AttenuationValue { get; }

        public long DateMillisSinceEpoch { get; }

        public double Duration { get; }

        public int TotalRiskScore { get; }

        public RiskLevel TransmissionRiskLevel { get; }
    }
}
