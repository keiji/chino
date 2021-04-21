using Foundation;

namespace Chino
{
    public static class Extensions
    {
        public static void LogD(this NSErrorException nsErrorException)
        {
            Logger.D($"Error occurred {nsErrorException.Code} - {nsErrorException.Message}");
        }

        public static byte ToByte(this RiskLevel riskLevel)
        {
            return riskLevel switch
            {
                RiskLevel.Lowest => 1,
                RiskLevel.Low => 2,
                RiskLevel.LowMedium => 3,
                RiskLevel.Medium => 4,
                RiskLevel.MediumHigh => 5,
                RiskLevel.High => 6,
                RiskLevel.VeryHigh => 7,
                RiskLevel.Highest => 8,
                _ => 0,
            };
        }

        public static RiskLevel ToRiskLevel(this byte byteValue)
        {
            return byteValue switch
            {
                1 => RiskLevel.Lowest,
                2 => RiskLevel.Low,
                3 => RiskLevel.LowMedium,
                4 => RiskLevel.Medium,
                5 => RiskLevel.MediumHigh,
                6 => RiskLevel.High,
                7 => RiskLevel.VeryHigh,
                8 => RiskLevel.Highest,
                _ => RiskLevel.Invalid,
            };
        }
    }
}
