using AndroidExposureNotificationStatus = Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus;
using AndroidRiskLevel = Android.Gms.Nearby.ExposureNotification.RiskLevel;
using AndroidExposureConfiguration = Android.Gms.Nearby.ExposureNotification.ExposureConfiguration;
using AndroidDailySummariesConfig = Android.Gms.Nearby.ExposureNotification.DailySummariesConfig;

using System.Linq;

using Logger = Chino.ChinoLogger;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby.ExposureNotification;
using System.Collections.Generic;

namespace Chino.Android.Google
{
    public static class Extensions
    {
        public static bool IsENException(this ApiException apiException)
            => ApiExceptionStatusCodes.ERROR_ALL.Contains(apiException.StatusCode);

        public static ENException ToENException(this ApiException apiException)
        {
            int code = apiException.StatusCode switch
            {
                CommonStatusCodes.Error => ENException.Code_Android.FAILED,
                CommonStatusCodes.ConnectionSuspendedDuringCall => ENException.Code_Android.SERVICE_CONNECTION_LOST,
                CommonStatusCodes.InternalError => ENException.Code_Android.TIME_OUT,
                ApiExceptionStatusCodes.FAILED_ALREADY_STARTED => ENException.Code_Android.FAILED_ALREADY_STARTED,
                ApiExceptionStatusCodes.FAILED_BLUETOOTH_DISABLED => ENException.Code_Android.FAILED_BLUETOOTH_DISABLED,
                ApiExceptionStatusCodes.FAILED_DISK_IO => ENException.Code_Android.FAILED_DISK_IO,
                ApiExceptionStatusCodes.FAILED_KEY_RELEASE_NOT_PREAUTHORIZED => ENException.Code_Android.FAILED_KEY_RELEASE_NOT_PREAUTHORIZED,
                ApiExceptionStatusCodes.FAILED_NOT_IN_FOREGROUND => ENException.Code_Android.FAILED_NOT_IN_FOREGROUND,
                ApiExceptionStatusCodes.FAILED_NOT_SUPPORTED => ENException.Code_Android.FAILED_NOT_SUPPORTED,
                ApiExceptionStatusCodes.FAILED_RATE_LIMITED => ENException.Code_Android.FAILED_RATE_LIMITED,
                ApiExceptionStatusCodes.FAILED_REJECTED_OPT_IN => ENException.Code_Android.FAILED_REJECTED_OPT_IN,
                ApiExceptionStatusCodes.FAILED_SERVICE_DISABLED => ENException.Code_Android.FAILED_SERVICE_DISABLED,
                ApiExceptionStatusCodes.FAILED_TEMPORARILY_DISABLED => ENException.Code_Android.FAILED_TEMPORARILY_DISABLED,
                ApiExceptionStatusCodes.FAILED_UNAUTHORIZED => ENException.Code_Android.FAILED_UNAUTHORIZED,
                _ => ENException.Code_Android.FAILED,
            };

            return new ENException(code, $"{apiException.Message},{apiException.StatusCode}");
        }

        public static ExposureNotificationStatus ToExposureNotificationStatus(this AndroidExposureNotificationStatus status)
        {
            int code = ExposureNotificationStatus.Code_Android.UNKNOWN;

            // Cannot use switch statement because AndroidExposureNotificationStatus.* is not constant.
            if (status == AndroidExposureNotificationStatus.Activated)
            {
                code = ExposureNotificationStatus.Code_Android.ACTIVATED;
            }
            else if (status == AndroidExposureNotificationStatus.BluetoothDisabled)
            {
                code = ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED;
            }
            else if (status == AndroidExposureNotificationStatus.BluetoothSupportUnknown)
            {
                code = ExposureNotificationStatus.Code_Android.BLUETOOTH_SUPPORT_UNKNOWN;
            }
            else if (status == AndroidExposureNotificationStatus.EnNotSupport)
            {
                code = ExposureNotificationStatus.Code_Android.EN_NOT_SUPPORT;
            }
            else if (status == AndroidExposureNotificationStatus.FocusLost)
            {
                code = ExposureNotificationStatus.Code_Android.FOCUS_LOST;
            }
            else if (status == AndroidExposureNotificationStatus.HwNotSupport)
            {
                code = ExposureNotificationStatus.Code_Android.HW_NOT_SUPPORT;
            }
            else if (status == AndroidExposureNotificationStatus.Inactivated)
            {
                code = ExposureNotificationStatus.Code_Android.INACTIVATED;
            }
            else if (status == AndroidExposureNotificationStatus.LocationDisabled)
            {
                code = ExposureNotificationStatus.Code_Android.LOCATION_DISABLED;
            }
            else if (status == AndroidExposureNotificationStatus.LowStorage)
            {
                code = ExposureNotificationStatus.Code_Android.LOW_STORAGE;
            }
            else if (status == AndroidExposureNotificationStatus.NotInAllowlist)
            {
                code = ExposureNotificationStatus.Code_Android.NOT_IN_ALLOWLIST;
            }
            else if (status == AndroidExposureNotificationStatus.NoConsent)
            {
                code = ExposureNotificationStatus.Code_Android.NO_CONSENT;
            }
            else if (status == AndroidExposureNotificationStatus.UserProfileNotSupport)
            {
                code = ExposureNotificationStatus.Code_Android.USER_PROFILE_NOT_SUPPORT;
            }

            return new ExposureNotificationStatus(code);
        }

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

            foreach (var key in dailySummariesConfig.InfectiousnessWeights.Keys)
            {
                if (key == Infectiousness.None)
                {
                    Logger.E("Infectiousness.None is ignored");
                    continue;
                }
                var value = dailySummariesConfig.InfectiousnessWeights[key];
                builder.SetInfectiousnessWeight((int)key, value);
            }

            foreach (var key in dailySummariesConfig.ReportTypeWeights.Keys)
            {
                if (key == ReportType.Unknown || key == ReportType.Revoked)
                {
                    Logger.E($"ReportType.{key} is ignored");
                    continue;
                }
                var value = dailySummariesConfig.ReportTypeWeights[key];
                builder.SetReportTypeWeight((int)key, value);
            }

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

        public static DiagnosisKeysDataMapping ToDiagnosisKeysDataMapping(
            this ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration googleDiagnosisKeysDataMappingConfig
            )
        {
            IDictionary<int, Infectiousness> InfectiousnessForDaysSinceOnsetOfSymptoms
                = googleDiagnosisKeysDataMappingConfig.InfectiousnessForDaysSinceOnsetOfSymptoms;

            IDictionary<Java.Lang.Integer, Java.Lang.Integer> daysSinceOnsetToInfectiousness = new Dictionary<Java.Lang.Integer, Java.Lang.Integer>();
            foreach (var key in InfectiousnessForDaysSinceOnsetOfSymptoms.Keys)
            {
                var value = InfectiousnessForDaysSinceOnsetOfSymptoms[key];
                daysSinceOnsetToInfectiousness.Add(new Java.Lang.Integer(key), new Java.Lang.Integer((int)value));
            }

            return new DiagnosisKeysDataMapping.DiagnosisKeysDataMappingBuilder()
                .SetDaysSinceOnsetToInfectiousness(daysSinceOnsetToInfectiousness)
                .SetInfectiousnessWhenDaysSinceOnsetMissing((int)googleDiagnosisKeysDataMappingConfig.InfectiousnessWhenDaysSinceOnsetMissing)
                .SetReportTypeWhenMissing((int)googleDiagnosisKeysDataMappingConfig.ReportTypeWhenMissing)
                .Build();
        }
    }
}
