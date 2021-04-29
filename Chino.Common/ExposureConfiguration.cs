using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Chino
{
    [JsonObject]
    public class ExposureConfiguration
    {
        [JsonProperty("google_exposure_config")]
        public GoogleExposureConfiguration GoogleExposureConfig { get; set; } = new GoogleExposureConfiguration();

        // for ExposureWindow Mode
        [JsonProperty("google_diagnosis_keys_data_mapping_config")]
        public GoogleDiagnosisKeysDataMappingConfiguration GoogleDiagnosisKeysDataMappingConfig { get; set; } = new GoogleDiagnosisKeysDataMappingConfiguration();

        [JsonProperty("google_daily_summaries_config")]
        public DailySummariesConfig GoogleDailySummariesConfig { get; set; } = new DailySummariesConfig();

        [JsonProperty("apple_exposure_v1_config")]
        public AppleExposureV1Configuration AppleExposureV1Config { get; set; } = new AppleExposureV1Configuration();

        [JsonProperty("apple_exposure_v2_config")]
        public AppleExposureV2Configuration AppleExposureV2Config { get; set; } = new AppleExposureV2Configuration();

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        [JsonObject]
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
            [JsonProperty("attenuation_scores")]
            public int[] AttenuationScores { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            [JsonProperty("attenuation_weight")]
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
            [JsonProperty("days_since_last_exposure_scores")]
            public int[] DaysSinceLastExposureScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            [JsonProperty("days_since_last_exposure_weight")]
            public int DaysSinceLastExposureWeight { get; set; } = 50;

            [JsonProperty("duration_at_attenuation_thresholds")]
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
            [JsonProperty("duration_scores")]
            public int[] DurationScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            [JsonProperty("duration_weight")]
            public int DurationWeight { get; set; } = 50;

            [JsonProperty("minimum_risk_score")]
            public int MinimumRiskScore { get; set; } = 21;

            /*
             * [0]-[7] App Defined
             */
            [JsonProperty("transmission_risk_scores")]
            public int[] TransmissionRiskScores { get; set; } = { 7, 7, 7, 7, 7, 7, 7, 7 };

            [JsonProperty("transmission_risk_weight")]
            public int TransmissionRiskWeight { get; set; } = 50;

            public override bool Equals(object obj)
            {
                return obj is GoogleExposureConfiguration configuration &&
                       AttenuationScores.SequenceEqual(configuration.AttenuationScores) &&
                       AttenuationWeight == configuration.AttenuationWeight &&
                       DaysSinceLastExposureScores.SequenceEqual(configuration.DaysSinceLastExposureScores) &&
                       DaysSinceLastExposureWeight == configuration.DaysSinceLastExposureWeight &&
                       DurationAtAttenuationThresholds.SequenceEqual(configuration.DurationAtAttenuationThresholds) &&
                       DurationScores.SequenceEqual(configuration.DurationScores) &&
                       DurationWeight == configuration.DurationWeight &&
                       MinimumRiskScore == configuration.MinimumRiskScore &&
                       TransmissionRiskScores.SequenceEqual(configuration.TransmissionRiskScores) &&
                       TransmissionRiskWeight == configuration.TransmissionRiskWeight;
            }

            public override int GetHashCode()
            {
                HashCode hash = new HashCode();
                hash.Add(AttenuationScores);
                hash.Add(AttenuationWeight);
                hash.Add(DaysSinceLastExposureScores);
                hash.Add(DaysSinceLastExposureWeight);
                hash.Add(DurationAtAttenuationThresholds);
                hash.Add(DurationScores);
                hash.Add(DurationWeight);
                hash.Add(MinimumRiskScore);
                hash.Add(TransmissionRiskScores);
                hash.Add(TransmissionRiskWeight);
                return hash.ToHashCode();
            }
        }

        // https://developers.google.com/android/exposure-notifications/meaningful-exposures
        [JsonObject]
        public class GoogleDiagnosisKeysDataMappingConfiguration
        {
            [JsonProperty("infectiousness_for_days_since_onset_of_symptoms")]
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

            [JsonProperty("infectiousness_when_days_since_onset_missing")]
            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;

            public override bool Equals(object obj)
            {
                return obj is GoogleDiagnosisKeysDataMappingConfiguration configuration &&
                       InfectiousnessForDaysSinceOnsetOfSymptoms.SequenceEqual(InfectiousnessForDaysSinceOnsetOfSymptoms) &&
                       InfectiousnessWhenDaysSinceOnsetMissing == configuration.InfectiousnessWhenDaysSinceOnsetMissing;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(InfectiousnessForDaysSinceOnsetOfSymptoms, InfectiousnessWhenDaysSinceOnsetMissing);
            }
        }

        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration/exposure_risk_value_calculation_in_exposurenotification_version_1
        [JsonObject]
        public class AppleExposureV1Configuration
        {
            // Configuring Duration

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

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
            [JsonProperty("attenuation_level_values")]
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
            [JsonProperty("days_since_last_exposure_level_values")]
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
            [JsonProperty("duration_level_values")]
            public int[] DurationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /*
             * [0]-[7] App Defined
             */
            [JsonProperty("transmission_risk_level_values")]
            public int[] TransmissionRiskLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            [JsonProperty("minimum_risk_score")]
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

            [JsonProperty("minimum_risk_score_full_range")]
            public double MinimumRiskScoreFullRange { get; set; } = 0.0;

            #endregion

            public override bool Equals(object obj)
            {
                return obj is AppleExposureV1Configuration configuration &&
                       AttenuationLevelValues.SequenceEqual(configuration.AttenuationLevelValues) &&
                       DaysSinceLastExposureLevelValues.SequenceEqual(configuration.DaysSinceLastExposureLevelValues) &&
                       DurationLevelValues.SequenceEqual(configuration.DurationLevelValues) &&
                       TransmissionRiskLevelValues.SequenceEqual(configuration.TransmissionRiskLevelValues) &&
                       MinimumRiskScore == configuration.MinimumRiskScore &&
                       MinimumRiskScoreFullRange == configuration.MinimumRiskScoreFullRange;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(AttenuationLevelValues, DaysSinceLastExposureLevelValues, DurationLevelValues, TransmissionRiskLevelValues, MinimumRiskScore, MinimumRiskScoreFullRange);
            }
        }

        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration
        [JsonObject]
        public class AppleExposureV2Configuration
        {
            // Configuring Duration

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

            /*
             * [0] The immediate duration threshold.
             * [1] The near attenuation threshold.
             * [2] The medium attenuation threshold.
             */
            [JsonProperty("attenuation_duration_thresholds")]
            public int[] AttenuationDurationThresholds { get; set; } = { 15, 33, 73 };

            [JsonProperty("immediate_duration_weight")]
            public double ImmediateDurationWeight { get; set; } = 100;

            [JsonProperty("near_duration_weight")]
            public double NearDurationWeight { get; set; } = 100;

            [JsonProperty("medium_duration_weight")]
            public double MediumDurationWeight { get; set; } = 100;

            [JsonProperty("other_duration_weight")]
            public double OtherDurationWeight { get; set; } = 100;

            public int DaysSinceLastExposureThreshold { get; set; } = 0;

            // Configuring Infectiousness

            // Must Specify v2
            [JsonProperty("infectiousness_for_days_since_onset_of_symptoms")]
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

            [JsonProperty("infectiousness_when_days_since_onset_missing")]
            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;

            [JsonProperty("infectiousness_high_weight")]
            public double InfectiousnessHighWeight { get; set; } = 100.0; // The range of this value is 0-250%

            [JsonProperty("infectiousness_standard_weight")]
            public double InfectiousnessStandardWeight { get; set; } = 100.0; // The range of this value is 0-250%

            // Configuring Report Types

            [JsonProperty("report_type_confirmed_clinical_diagnosis_weight")]
            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; } = 100.0;

            [JsonProperty("report_type_confirmed_test_weight")]
            public double ReportTypeConfirmedTestWeight { get; set; } = 100.0;

            [JsonProperty("report_type_recursive_weight")]
            public double ReportTypeRecursiveWeight { get; set; } = 100.0;

            [JsonProperty("report_type_self_reported_weight")]
            public double ReportTypeSelfReportedWeight { get; set; } = 100.0;

            [JsonProperty("report_type_none_map")]
            public ReportType ReportTypeNoneMap { get; set; } = ReportType.ConfirmedTest;

            public override bool Equals(object obj)
            {
                return obj is AppleExposureV2Configuration configuration &&
                       AttenuationDurationThresholds.SequenceEqual(configuration.AttenuationDurationThresholds) &&
                       ImmediateDurationWeight == configuration.ImmediateDurationWeight &&
                       NearDurationWeight == configuration.NearDurationWeight &&
                       MediumDurationWeight == configuration.MediumDurationWeight &&
                       OtherDurationWeight == configuration.OtherDurationWeight &&
                       DaysSinceLastExposureThreshold == configuration.DaysSinceLastExposureThreshold &&
                       InfectiousnessForDaysSinceOnsetOfSymptoms.SequenceEqual(configuration.InfectiousnessForDaysSinceOnsetOfSymptoms) &&
                       InfectiousnessWhenDaysSinceOnsetMissing == configuration.InfectiousnessWhenDaysSinceOnsetMissing &&
                       InfectiousnessHighWeight == configuration.InfectiousnessHighWeight &&
                       InfectiousnessStandardWeight == configuration.InfectiousnessStandardWeight &&
                       ReportTypeConfirmedClinicalDiagnosisWeight == configuration.ReportTypeConfirmedClinicalDiagnosisWeight &&
                       ReportTypeConfirmedTestWeight == configuration.ReportTypeConfirmedTestWeight &&
                       ReportTypeRecursiveWeight == configuration.ReportTypeRecursiveWeight &&
                       ReportTypeSelfReportedWeight == configuration.ReportTypeSelfReportedWeight &&
                       ReportTypeNoneMap == configuration.ReportTypeNoneMap;
            }

            public override int GetHashCode()
            {
                HashCode hash = new HashCode();
                hash.Add(AttenuationDurationThresholds);
                hash.Add(ImmediateDurationWeight);
                hash.Add(NearDurationWeight);
                hash.Add(MediumDurationWeight);
                hash.Add(OtherDurationWeight);
                hash.Add(DaysSinceLastExposureThreshold);
                hash.Add(InfectiousnessForDaysSinceOnsetOfSymptoms);
                hash.Add(InfectiousnessWhenDaysSinceOnsetMissing);
                hash.Add(InfectiousnessHighWeight);
                hash.Add(InfectiousnessStandardWeight);
                hash.Add(ReportTypeConfirmedClinicalDiagnosisWeight);
                hash.Add(ReportTypeConfirmedTestWeight);
                hash.Add(ReportTypeRecursiveWeight);
                hash.Add(ReportTypeSelfReportedWeight);
                hash.Add(ReportTypeNoneMap);
                return hash.ToHashCode();
            }

            #endregion


        }

        public override bool Equals(object obj)
        {
            return obj is ExposureConfiguration configuration &&
                   EqualityComparer<GoogleExposureConfiguration>.Default.Equals(GoogleExposureConfig, configuration.GoogleExposureConfig) &&
                   EqualityComparer<GoogleDiagnosisKeysDataMappingConfiguration>.Default.Equals(GoogleDiagnosisKeysDataMappingConfig, configuration.GoogleDiagnosisKeysDataMappingConfig) &&
                   EqualityComparer<DailySummariesConfig>.Default.Equals(GoogleDailySummariesConfig, configuration.GoogleDailySummariesConfig) &&
                   EqualityComparer<AppleExposureV1Configuration>.Default.Equals(AppleExposureV1Config, configuration.AppleExposureV1Config) &&
                   EqualityComparer<AppleExposureV2Configuration>.Default.Equals(AppleExposureV2Config, configuration.AppleExposureV2Config);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GoogleExposureConfig, GoogleDiagnosisKeysDataMappingConfig, GoogleDailySummariesConfig, AppleExposureV1Config, AppleExposureV2Config);
        }
    }
}
