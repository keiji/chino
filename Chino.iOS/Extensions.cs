using System;
using System.Collections.Generic;
using System.Linq;
using Chino.Common;
using ExposureNotifications;
using Foundation;
using UIKit;

using Logger = Chino.ChinoLogger;

namespace Chino.iOS
{
    public static class Extensions
    {
        public static void LogD(this NSErrorException nsErrorException)
        {
            Logger.D($"Error occurred {nsErrorException.Code} - {nsErrorException.Domain} - {nsErrorException.Message}");
        }

        private const string ENErrorDomain = "ENErrorDomain";

        private const int ApiMisuse = (int)ENErrorCode.ApiMisuse;
        private const int BadFormat = (int)ENErrorCode.BadFormat;
        private const int BadParameter = (int)ENErrorCode.BadParameter;
        private const int BluetoothOff = (int)ENErrorCode.BluetoothOff;
        private const int DataInaccessible = (int)ENErrorCode.DataInaccessible;
        private const int InsufficientMemory = (int)ENErrorCode.InsufficientMemory;
        private const int InsufficientStorage = (int)ENErrorCode.InsufficientStorage;
        private const int Internal = (int)ENErrorCode.Internal;
        private const int Invalidated = (int)ENErrorCode.Invalidated;
        private const int NotAuthorized = (int)ENErrorCode.NotAuthorized;
        private const int NotEnabled = (int)ENErrorCode.NotEnabled;
        private const int NotEntitled = (int)ENErrorCode.NotEntitled;
        private const int RateLimited = (int)ENErrorCode.RateLimited;
        private const int Restricted = (int)ENErrorCode.Restricted;
        private const int TravelStatusNotAvailable = (int)ENErrorCode.TravelStatusNotAvailable;
        private const int Unknown = (int)ENErrorCode.Unknown;
        private const int Unsupported = (int)ENErrorCode.Unsupported;

        public static bool IsENException(this NSErrorException nsErrorException) => nsErrorException.Domain == ENErrorDomain;

        public static ENException ToENException(this NSErrorException nsErrorException)
        {
            int code = (int)nsErrorException.Code switch
            {
                ApiMisuse => ENException.Code_iOS.ApiMisuse,
                BadFormat => ENException.Code_iOS.BadFormat,
                BadParameter => ENException.Code_iOS.BadParameter,
                BluetoothOff => ENException.Code_iOS.BluetoothOff,
                DataInaccessible => ENException.Code_iOS.DataInaccessible,
                InsufficientMemory => ENException.Code_iOS.InsufficientMemory,
                InsufficientStorage => ENException.Code_iOS.InsufficientStorage,
                Internal => ENException.Code_iOS.Internal,
                Invalidated => ENException.Code_iOS.Invalidated,
                NotAuthorized => ENException.Code_iOS.NotAuthorized,
                NotEnabled => ENException.Code_iOS.NotEnabled,
                NotEntitled => ENException.Code_iOS.NotEntitled,
                RateLimited => ENException.Code_iOS.RateLimited,
                Restricted => ENException.Code_iOS.Restricted,
                TravelStatusNotAvailable => ENException.Code_iOS.TravelStatusNotAvailable,
                Unknown => ENException.Code_iOS.Unknown,
                Unsupported => ENException.Code_iOS.Unsupported,
                _ => ENException.Code_iOS.Unknown,
            };

            return new ENException(code, nsErrorException.Message);
        }

        public static long GetDateMillisSinceEpoch(this NSDate date)
        {
            DateTime dateTime = (DateTime)date;

            // TODO: Check TimeZone
            var dto = new DateTimeOffset(dateTime.Ticks, new TimeSpan(0, 00, 00));

            return dto.ToUnixTimeMilliseconds();
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

        public static ENExposureConfiguration ToENExposureConfiguration(
            this ExposureConfiguration.AppleExposureConfigurationV1 appleExposureConfiguration
        )
        {
            ENExposureConfiguration configuration = new ENExposureConfiguration();

            if (appleExposureConfiguration == null)
            {
                Logger.E("appleExposureV1Configuration is not set.");
                return new ENExposureConfiguration();
            }

            NSMutableDictionary metadata = new NSMutableDictionary();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 6))
            {
                Logger.D("Set configuration values for iOS 13.6 later.");
                configuration.MinimumRiskScoreFullRange = appleExposureConfiguration.MinimumRiskScoreFullRange;

                // MetaData
                metadata.SetValueForKey(
                    new NSNumber(appleExposureConfiguration.MinimumRiskScoreFullRange),
                    new NSString("minimumRiskScoreFullRange")
                    );

                var attKey = new NSString("attenuationDurationThresholds");
                var attValue = NSArray.FromObjects(2, 50, 70);
                metadata.SetValueForKey(attValue, attKey);
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 5))
            {
                Logger.D("Set configuration values for iOS 13.5 later.");
                configuration.AttenuationLevelValues = appleExposureConfiguration.AttenuationLevelValues;
                configuration.DaysSinceLastExposureLevelValues = appleExposureConfiguration.DaysSinceLastExposureLevelValues;
                configuration.DurationLevelValues = appleExposureConfiguration.DurationLevelValues;
                configuration.TransmissionRiskLevelValues = appleExposureConfiguration.TransmissionRiskLevelValues;
                configuration.MinimumRiskScore = appleExposureConfiguration.MinimumRiskScore;
            }

            // iOS 12.5
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 6)
                && !UIDevice.CurrentDevice.CheckSystemVersion(13, 5)
                && ObjCRuntime.Class.GetHandle("ENManager") != null)
            {
                Logger.D("Set configuration values for iOS 12.5.");
                configuration.MinimumRiskScoreFullRange = appleExposureConfiguration.MinimumRiskScoreFullRange;

                configuration.AttenuationLevelValues = appleExposureConfiguration.AttenuationLevelValues;
                configuration.DaysSinceLastExposureLevelValues = appleExposureConfiguration.DaysSinceLastExposureLevelValues;
                configuration.DurationLevelValues = appleExposureConfiguration.DurationLevelValues;
                configuration.TransmissionRiskLevelValues = appleExposureConfiguration.TransmissionRiskLevelValues;
                configuration.MinimumRiskScore = appleExposureConfiguration.MinimumRiskScore;
            }

            configuration.Metadata = metadata;

            return configuration;
        }

        public static ENExposureConfiguration ToENExposureConfiguration(
            this ExposureConfiguration.AppleExposureConfigurationV2 appleExposureConfiguration
        )
        {
            ENExposureConfiguration configuration = new ENExposureConfiguration();

            if (appleExposureConfiguration == null)
            {
                Logger.E("appleExposureConfiguration is not set.");
                return configuration;
            }

            NSDictionary<NSNumber, NSNumber> infectiousnessForDaysSinceOnsetOfSymptomsNSDict
                = GetInfectiousnessForDaysSinceOnsetOfSymptomsNSDict(
                    appleExposureConfiguration.InfectiousnessForDaysSinceOnsetOfSymptoms,
                    appleExposureConfiguration.InfectiousnessWhenDaysSinceOnsetMissing);

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 7))
            {
                Logger.D("Set configuration values for iOS 13.7 later.");
                configuration.ImmediateDurationWeight = appleExposureConfiguration.ImmediateDurationWeight;
                configuration.MediumDurationWeight = appleExposureConfiguration.MediumDurationWeight;
                configuration.NearDurationWeight = appleExposureConfiguration.NearDurationWeight;
                configuration.OtherDurationWeight = appleExposureConfiguration.OtherDurationWeight;
                configuration.DaysSinceLastExposureThreshold = appleExposureConfiguration.DaysSinceLastExposureThreshold;
                configuration.InfectiousnessForDaysSinceOnsetOfSymptoms = infectiousnessForDaysSinceOnsetOfSymptomsNSDict;
                configuration.InfectiousnessHighWeight = appleExposureConfiguration.InfectiousnessHighWeight;
                configuration.InfectiousnessStandardWeight = appleExposureConfiguration.InfectiousnessStandardWeight;
                configuration.ReportTypeConfirmedClinicalDiagnosisWeight = appleExposureConfiguration.ReportTypeConfirmedClinicalDiagnosisWeight;
                configuration.ReportTypeConfirmedTestWeight = appleExposureConfiguration.ReportTypeConfirmedTestWeight;
                configuration.ReportTypeRecursiveWeight = appleExposureConfiguration.ReportTypeRecursiveWeight;
                configuration.ReportTypeSelfReportedWeight = appleExposureConfiguration.ReportTypeSelfReportedWeight;
                configuration.ReportTypeNoneMap = (ENDiagnosisReportType)Enum.ToObject(typeof(ENDiagnosisReportType), appleExposureConfiguration.ReportTypeNoneMap);
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 6))
            {
                Logger.D("Set configuration values for iOS 13.6 later.");
                configuration.AttenuationDurationThresholds = appleExposureConfiguration.AttenuationDurationThresholds;
            }

            // iOS 12.5
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 6)
                && !UIDevice.CurrentDevice.CheckSystemVersion(13, 5)
                && ObjCRuntime.Class.GetHandle("ENManager") != null)
            {
                Logger.D("Set configuration values for iOS 12.5.");
                configuration.AttenuationDurationThresholds = appleExposureConfiguration.AttenuationDurationThresholds;

                configuration.ImmediateDurationWeight = appleExposureConfiguration.ImmediateDurationWeight;
                configuration.MediumDurationWeight = appleExposureConfiguration.MediumDurationWeight;
                configuration.NearDurationWeight = appleExposureConfiguration.NearDurationWeight;
                configuration.OtherDurationWeight = appleExposureConfiguration.OtherDurationWeight;
                configuration.DaysSinceLastExposureThreshold = appleExposureConfiguration.DaysSinceLastExposureThreshold;
                configuration.InfectiousnessForDaysSinceOnsetOfSymptoms = infectiousnessForDaysSinceOnsetOfSymptomsNSDict;
                configuration.InfectiousnessHighWeight = appleExposureConfiguration.InfectiousnessHighWeight;
                configuration.InfectiousnessStandardWeight = appleExposureConfiguration.InfectiousnessStandardWeight;
                configuration.ReportTypeConfirmedClinicalDiagnosisWeight = appleExposureConfiguration.ReportTypeConfirmedClinicalDiagnosisWeight;
                configuration.ReportTypeConfirmedTestWeight = appleExposureConfiguration.ReportTypeConfirmedTestWeight;
                configuration.ReportTypeRecursiveWeight = appleExposureConfiguration.ReportTypeRecursiveWeight;
                configuration.ReportTypeSelfReportedWeight = appleExposureConfiguration.ReportTypeSelfReportedWeight;
                configuration.ReportTypeNoneMap = (ENDiagnosisReportType)Enum.ToObject(typeof(ENDiagnosisReportType), appleExposureConfiguration.ReportTypeNoneMap);
            }

            return configuration;
        }

        private static NSDictionary<NSNumber, NSNumber> GetInfectiousnessForDaysSinceOnsetOfSymptomsNSDict(
            IDictionary<long, Infectiousness> infectiousnessForDaysSinceOnsetOfSymptoms,
            Infectiousness infectiousnessWhenDaysSinceOnsetMissing)
        {
            var pairs = infectiousnessForDaysSinceOnsetOfSymptoms.Keys.Zip(infectiousnessForDaysSinceOnsetOfSymptoms.Values, (k, v) => new NSNumber[] { k, (int)v });
            NSMutableDictionary<NSNumber, NSNumber> infectiousnessForDaysSinceOnsetOfSymptomsMutableDict = new NSMutableDictionary<NSNumber, NSNumber>();
            foreach (NSNumber[] pair in pairs)
            {
                infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Add(pair[0], pair[1]);
            }

            /*
             * The parameter `infectiousnessWhenDaysSinceOnsetMissing` must be set in infectiousnessForDaysSinceOnsetOfSymptoms
             * If this parameter not set, ENv2 does not work correctly(DailySummaries and ExposureWindows count always 0).
             */
            infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Add(
                new NSNumber(long.MaxValue),
                new NSNumber((int)infectiousnessWhenDaysSinceOnsetMissing)
                );

            return NSDictionary<NSNumber, NSNumber>.FromObjectsAndKeys(
                            infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Values,
                            infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Keys,
                            (nint)infectiousnessForDaysSinceOnsetOfSymptomsMutableDict.Count);
        }
    }
}
