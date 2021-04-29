using AndroidRiskLevel = Android.Gms.Nearby.ExposureNotification.RiskLevel;
using AndroidExposureConfiguration = Android.Gms.Nearby.ExposureNotification.ExposureConfiguration;
using AndroidDailySummariesConfig = Android.Gms.Nearby.ExposureNotification.DailySummariesConfig;
using System.Linq;

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

        public static AndroidDailySummariesConfig ToAndroidDailySummariesConfig(this DailySummariesConfig dailySummariesConfig)
        {
            AndroidDailySummariesConfig.DailySummariesConfigBuilder builder
                = new AndroidDailySummariesConfig.DailySummariesConfigBuilder()
                .SetAttenuationBuckets(
                dailySummariesConfig.AttenuationBucketThresholdDb.Select(value => new Java.Lang.Integer(value)).ToList(),
                dailySummariesConfig.AttenuationBucketWeights.Select(value => new Java.Lang.Double(value)).ToList()
                )
                .SetDaysSinceExposureThreshold(dailySummariesConfig.DaysSinceExposureThreshold)
                .SetMinimumWindowScore(dailySummariesConfig.MinimumWindowScore);

            dailySummariesConfig.InfectiousnessWeights.Keys.Zip(
                dailySummariesConfig.InfectiousnessWeights.Values,
                (key, value) => builder.SetInfectiousnessWeight((int)key, value)
                );

            dailySummariesConfig.ReportTypeWeights.Keys.Zip(
                dailySummariesConfig.ReportTypeWeights.Values,
                (key, value) => builder.SetReportTypeWeight((int)key, value)
                );

            return builder.Build();
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public static AndroidExposureConfiguration ToAndroidExposureConfiguration(this ExposureConfiguration exposureConfiguration)
        {
            ExposureConfiguration.GoogleExposureConfiguration googleExposureConfiguration = exposureConfiguration.GoogleExposureConfig;

            return new AndroidExposureConfiguration.ExposureConfigurationBuilder()
                .SetAttenuationScores(googleExposureConfiguration.AttenuationScores)
                .SetAttenuationWeight(googleExposureConfiguration.AttenuationWeight)
                .SetDaysSinceLastExposureScores(googleExposureConfiguration.DaysSinceLastExposureScores)
                .SetDaysSinceLastExposureWeight(googleExposureConfiguration.DaysSinceLastExposureWeight)
                .SetDurationAtAttenuationThresholds(googleExposureConfiguration.DurationAtAttenuationThresholds)
                .SetDurationScores(googleExposureConfiguration.DurationScores)
                .SetDurationWeight(googleExposureConfiguration.DurationWeight)
                .SetMinimumRiskScore(googleExposureConfiguration.MinimumRiskScore)
                .SetTransmissionRiskScores(googleExposureConfiguration.TransmissionRiskScores)
                .SetTransmissionRiskWeight(googleExposureConfiguration.TransmissionRiskWeight)
                .Build();
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
