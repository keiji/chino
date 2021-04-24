using System.Collections.Generic;

namespace Chino
{
    public class ExposureConfiguration
    {
        public GoogleExposureConfiguration GoogleExposureConfig { get; set; } = new GoogleExposureConfiguration();

        // for ExposureWindow Mode
        public GoogleDiagnosisKeysDataMappingConfiguration GoogleDiagnosisKeysDataMappingConfig { get; set; } = new GoogleDiagnosisKeysDataMappingConfiguration();
        public DailySummariesConfig GoogleDailySummariesConfig { get; set; } = new DailySummariesConfig();

        public AppleExposureConfiguration AppleExposureConfig { get; set; } = new AppleExposureConfiguration();

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        public class GoogleExposureConfiguration
        {

            /*
             * [0] when Attenuation > 73 dB
             * [1] when 73 >= Attenuation > 63
             * [2] when 63 >= Attenuation > 51
             * [3] when 51 >= Attenuation > 33
             * [4] when 33 >= Attenuation > 27
             * [5] when 27 >= Attenuation > 15
             * [6] when 15 >= Attenuation > 10
             * [7] when 10 >= Attenuation
             */
            public int[] AttenuationScores { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            public int AttenuationWeight { get; set; } = 50;

            /*
             * [0] when Days >= 14
             * [1] when Days >= 12
             * [2] when Days >= 10
             * [3] when Days >= 8
             * [4] when Days >= 6
             * [5] when Days >= 4
             * [6] when Days >= 2
             * [7] when Days >= 0
             */
            public int[] DaysSinceLastExposureScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            public int DaysSinceLastExposureWeight { get; set; } = 50;

            public int[] DurationAtAttenuationThresholds { get; set; } = { 50, 70 };

            /*
             * [0] when Duration == 0 min
             * [1] when Duration <= 5
             * [2] when Duration <= 10
             * [3] when Duration <= 15
             * [4] when Duration <= 20
             * [5] when Duration <= 25
             * [6] when Duration <= 30
             * [7] when Duration > 30
             */
            public int[] DurationScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            public int DurationWeight { get; set; } = 50;

            public int MinimumRiskScore { get; set; } = 21;

            /*
             * [0]-[7] App Defined
             */
            public int[] TransmissionRiskScores { get; set; } = { 7, 7, 7, 7, 7, 7, 7, 7 };

            public int TransmissionRiskWeight { get; set; } = 50;
        }

        // https://developers.google.com/android/exposure-notifications/meaningful-exposures
        public class GoogleDiagnosisKeysDataMappingConfiguration
        {
            public IDictionary<int, Infectiousness> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; } = new Dictionary<int, Infectiousness>() {
                { -14, Infectiousness.High },
                { -13, Infectiousness.High },
                { -12, Infectiousness.High },
                { -11, Infectiousness.High },
                { -10, Infectiousness.High },
                { -9, Infectiousness.High },
                { -8, Infectiousness.High },
                { -7, Infectiousness.High },
                { -6, Infectiousness.High },
                { -5, Infectiousness.High },
                { -4, Infectiousness.High },
                { -3, Infectiousness.High },
                { -2, Infectiousness.High },
                { -1, Infectiousness.High },
                { 0, Infectiousness.High },
                { 1, Infectiousness.High },
                { 2, Infectiousness.High },
                { 3, Infectiousness.High },
                { 4, Infectiousness.High },
                { 5, Infectiousness.High },
                { 6, Infectiousness.High },
                { 7, Infectiousness.High },
                { 8, Infectiousness.High },
                { 9, Infectiousness.High },
                { 10, Infectiousness.High },
                { 11, Infectiousness.High },
                { 12, Infectiousness.High },
                { 13, Infectiousness.High },
                { 14, Infectiousness.High },
            };

            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;
        }

        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration
        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration/exposure_risk_value_calculation_in_exposurenotification_version_1
        public class AppleExposureConfiguration
        {
            // Configuring Duration

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

            /*
             * [0] The immediate duration threshold.
             * [1] The near attenuation threshold.
             * [2] The medium attenuation threshold.
             */
            public int[] AttenuationDurationThresholds { get; set; } = { 15, 33, 73 };

            public double ImmediateDurationWeight { get; set; } = 100;
            public double NearDurationWeight { get; set; } = 100;
            public double MediumDurationWeight { get; set; } = 100;
            public double OtherDurationWeight { get; set; } = 100;

            public int DaysSinceLastExposureThreshold { get; set; } = 0;

            // Configuring Infectiousness

            // Must Specify v2
            public IDictionary<long, Infectiousness> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; } = new Dictionary<long, Infectiousness>() {
                { -14, Infectiousness.Standard },
                { -13, Infectiousness.Standard },
                { -12, Infectiousness.Standard },
                { -11, Infectiousness.Standard },
                { -10, Infectiousness.Standard },
                { -9, Infectiousness.Standard },
                { -8, Infectiousness.Standard },
                { -7, Infectiousness.Standard },
                { -6, Infectiousness.Standard },
                { -5, Infectiousness.Standard },
                { -4, Infectiousness.Standard },
                { -3, Infectiousness.Standard },
                { -2, Infectiousness.Standard },
                { -1, Infectiousness.Standard },
                { 0, Infectiousness.Standard },
                { 1, Infectiousness.Standard },
                { 2, Infectiousness.Standard },
                { 3, Infectiousness.Standard },
                { 4, Infectiousness.Standard },
                { 5, Infectiousness.Standard },
                { 6, Infectiousness.Standard },
                { 7, Infectiousness.Standard },
                { 8, Infectiousness.Standard },
                { 9, Infectiousness.Standard },
                { 10, Infectiousness.Standard },
                { 11, Infectiousness.Standard },
                { 12, Infectiousness.Standard },
                { 13, Infectiousness.Standard },
                { 14, Infectiousness.Standard },
            };

            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;

            public double InfectiousnessHighWeight { get; set; } = 100.0; // The range of this value is 0-250%
            public double InfectiousnessStandardWeight { get; set; } = 100.0; // The range of this value is 0-250%

            // Configuring Report Types

            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; } = 100.0;
            public double ReportTypeConfirmedTestWeight { get; set; } = 100.0;
            public double ReportTypeRecursiveWeight { get; set; } = 100.0;
            public double ReportTypeSelfReportedWeight { get; set; } = 100.0;
            public ReportType ReportTypeNoneMap { get; set; } = ReportType.ConfirmedTest;

            /*
             * [0] when Attenuation > 73 dB
             * [1] when 73 >= Attenuation > 63
             * [2] when 63 >= Attenuation > 51
             * [3] when 51 >= Attenuation > 33
             * [4] when 33 >= Attenuation > 27
             * [5] when 27 >= Attenuation > 15
             * [6] when 15 >= Attenuation > 10
             * [7] when 10 >= Attenuation
             */
            public int[] AttenuationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /*
             * [0] when Days >= 14
             * [1] when Days >= 12
             * [2] when Days >= 10
             * [3] when Days >= 8
             * [4] when Days >= 6
             * [5] when Days >= 4
             * [6] when Days >= 2
             * [7] when Days >= 0
             */
            public int[] DaysSinceLastExposureLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /*
             * [0] when Duration == 0 min
             * [1] when Duration <= 5
             * [2] when Duration <= 10
             * [3] when Duration <= 15
             * [4] when Duration <= 20
             * [5] when Duration <= 25
             * [6] when Duration <= 30
             * [7] when Duration > 30
             */
            public int[] DurationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /*
             * [0]-[7] App Defined
             */
            public int[] TransmissionRiskLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            public IDictionary<object, object> Metadata { get; set; } = new Dictionary<object, object>();

            public byte MinimumRiskScore { get; set; } = 0;

            // This weight parameter is not used.
            // public double AttenuationWeight { get; set; }

            // This weight parameter is not used.
            // public double DaysSinceLastExposureWeight { get; set; }

            // This weight parameter is not used.
            // public double DurationWeight { get; set; }

            // This weight parameter is not used.
            // public double TransmissionRiskWeight { get; set; }

            #endregion

            #region These properties are available in iOS 12.5, and in iOS 13.6 and later.

            public double MinimumRiskScoreFullRange { get; set; } = 0.0;

            #endregion

        }
    }
}
