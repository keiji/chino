using AndroidRiskLevel = Android.Gms.Nearby.ExposureNotification.RiskLevel;

namespace Chino
{
    public static class Extensions
    {

        public static int ToInt(this RiskLevel riskLevel)
        {
            return riskLevel switch
            {
                RiskLevel.Lowest => AndroidRiskLevel.RiskLevelLowest,
                RiskLevel.Low => AndroidRiskLevel.RiskLevelLow,
                RiskLevel.LowMedium => AndroidRiskLevel.RiskLevelLowMedium,
                RiskLevel.Medium => AndroidRiskLevel.RiskLevelMedium,
                RiskLevel.MediumHigh => AndroidRiskLevel.RiskLevelMediumHigh,
                RiskLevel.High => AndroidRiskLevel.RiskLevelHigh,
                RiskLevel.VeryHigh => AndroidRiskLevel.RiskLevelVeryHigh,
                RiskLevel.Highest => AndroidRiskLevel.RiskLevelHighest,
                _ => AndroidRiskLevel.RiskLevelInvalid,
            };
        }

        public static RiskLevel ToRiskLevel(this int intValue)
        {
            return intValue switch
            {
                AndroidRiskLevel.RiskLevelLowest => RiskLevel.Lowest,
                AndroidRiskLevel.RiskLevelLow => RiskLevel.Low,
                AndroidRiskLevel.RiskLevelLowMedium => RiskLevel.LowMedium,
                AndroidRiskLevel.RiskLevelMedium => RiskLevel.Medium,
                AndroidRiskLevel.RiskLevelMediumHigh => RiskLevel.MediumHigh,
                AndroidRiskLevel.RiskLevelHigh => RiskLevel.High,
                AndroidRiskLevel.RiskLevelVeryHigh => RiskLevel.VeryHigh,
                AndroidRiskLevel.RiskLevelHighest => RiskLevel.Highest,
                _ => RiskLevel.Invalid,
            };
        }
    }
}
