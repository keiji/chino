using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Chino
{
    /// <summary>
    /// The container class that contains configurations for each platform and version.
    /// </summary>
    [JsonObject]
    public class ExposureConfiguration
    {
        /// <summary>
        /// Exposure configuration parameters that can be provided when initializing the service.
        /// </summary>
        [JsonProperty("google_exposure_config")]
        public GoogleExposureConfiguration GoogleExposureConfig { get; set; } = new GoogleExposureConfiguration();

        /// <summary>
        /// Mappings from diagnosis keys data to concepts returned by the API.
        /// </summary>
        ///
        /// for ExposureWindow Mode
        [JsonProperty("google_diagnosis_keys_data_mapping_config")]
        public GoogleDiagnosisKeysDataMappingConfiguration GoogleDiagnosisKeysDataMappingConfig { get; set; } = new GoogleDiagnosisKeysDataMappingConfiguration();

        /// <summary>
        /// Configuration of per-day summary of exposures.
        /// </summary>
        [JsonProperty("google_daily_summaries_config")]
        public DailySummariesConfig GoogleDailySummariesConfig { get; set; } = new DailySummariesConfig();

        /// <summary>
        /// The object that contains parameters for configuring exposure notification risk scoring behavior for v1.
        /// </summary>
        [JsonProperty("apple_exposure_config_v1")]
        public AppleExposureConfigurationV1 AppleExposureConfigV1 { get; set; } = new AppleExposureConfigurationV1();

        /// <summary>
        /// The object that contains parameters for configuring exposure notification risk scoring behavior for v2.
        /// </summary>
        [JsonProperty("apple_exposure_config_v2")]
        public AppleExposureConfigurationV2 AppleExposureConfigV2 { get; set; } = new AppleExposureConfigurationV2();

        /// <summary>
        /// Exposure configuration parameters that can be provided when initializing the service.
        /// </summary>
        ///
        /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        [JsonObject]
        public class GoogleExposureConfiguration
        {
            /*
             * Timber.d(ExposureConfiguration.ExposureConfigurationBuilder().build().toString())
             *
             * ExposureConfiguration<minimumRiskScore: 4,
             * attenuationScores: [4, 4, 4, 4, 4, 4, 4, 4],
             * attenuationWeight: 50,
             * daysSinceLastExposureScores: [4, 4, 4, 4, 4, 4, 4, 4],
             * daysSinceLastExposureWeight: 50,
             * durationScores: [4, 4, 4, 4, 4, 4, 4, 4],
             * durationWeight: 50,
             * transmissionRiskScores: [4, 4, 4, 4, 4, 4, 4, 4],
             * transmissionRiskWeight: 50,
             * durationAtAttenuationThresholds: [50, 74]>
             */

            /// <summary>
            /// Scores for attenuation buckets.
            /// </summary>
            ///
            /// [0] when Attenuation > 73 dB
            /// [1] when 73 >= Attenuation > 63
            /// [2] when 63 >= Attenuation > 51
            /// [3] when 51 >= Attenuation > 33
            /// [4] when 33 >= Attenuation > 27
            /// [5] when 27 >= Attenuation > 15
            /// [6] when 15 >= Attenuation > 10
            /// [7] when 10 >= Attenuation
            ///
            [JsonProperty("attenuation_scores")]
            public int[] AttenuationScores { get; set; } = { 4, 4, 4, 4, 4, 4, 4, 4 };

            /// <summary>
            /// Weight to apply to the attenuation score.
            /// </summary>
            [JsonProperty("attenuation_weight")]
            public int AttenuationWeight { get; set; } = 50;

            /// <summary>
            /// Scores for days since last exposure buckets.
            /// </summary>
            ///
            /// [0] when Days >= 14
            /// [1] when Days >= 12
            /// [2] when Days >= 10
            /// [3] when Days >= 8
            /// [4] when Days >= 6
            /// [5] when Days >= 4
            /// [6] when Days >= 2
            /// [7] when Days >= 0
            ///
            [JsonProperty("days_since_last_exposure_scores")]
            public int[] DaysSinceLastExposureScores { get; set; } = { 4, 4, 4, 4, 4, 4, 4, 4 };

            /// <summary>
            /// Weight to apply to the days since last exposure score.
            /// </summary>
            [JsonProperty("days_since_last_exposure_weight")]
            public int DaysSinceLastExposureWeight { get; set; } = 50;

            /// <summary>
            /// Attenuation thresholds to apply when calculating duration at attenuation.
            /// </summary>
            [JsonProperty("duration_at_attenuation_thresholds")]
            public int[] DurationAtAttenuationThresholds { get; set; } = { 50, 74 };

            /// <summary>
            /// Scores for duration buckets.
            /// </summary>
            ///
            /// [0] when Duration == 0 min
            /// [1] when Duration <= 5
            /// [2] when Duration <= 10
            /// [3] when Duration <= 15
            /// [4] when Duration <= 20
            /// [5] when Duration <= 25
            /// [6] when Duration <= 30
            /// [7] when Duration > 30
            ///
            [JsonProperty("duration_scores")]
            public int[] DurationScores { get; set; } = { 4, 4, 4, 4, 4, 4, 4, 4 };

            /// <summary>
            /// Weight to apply to the duration score.
            /// </summary>
            [JsonProperty("duration_weight")]
            public int DurationWeight { get; set; } = 50;

            /// <summary>
            /// Minimum risk score.
            /// </summary>
            [JsonProperty("minimum_risk_score")]
            public int MinimumRiskScore { get; set; } = 4;

            /// <summary>
            /// Scores for transmission risk buckets.
            /// </summary>
            ///
            /// [0]-[7] App Defined
            ///
            [JsonProperty("transmission_risk_scores")]
            public int[] TransmissionRiskScores { get; set; } = { 4, 4, 4, 4, 4, 4, 4, 4 };

            /// <summary>
            /// Weight to apply to the transmission risk score.
            /// </summary>
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

        /// <summary>
        /// Mappings from diagnosis keys data to concepts returned by the API.
        /// </summary>
        ///
        /// https://developers.google.com/android/exposure-notifications/meaningful-exposures
        [JsonObject]
        public class GoogleDiagnosisKeysDataMappingConfiguration
        {
            // Configuring Infectiousness
            // Must Specify

            /// <summary>
            /// Mapping from diagnosisKey.daysSinceOnsetOfSymptoms to Infectiousness.
            /// </summary>
            ///
            /// Default values from https://developers.google.com/android/exposure-notifications/meaningful-exposures#map-diag-keys
            [JsonProperty("infectiousness_for_days_since_onset_of_symptoms")]
            public IDictionary<int, Infectiousness> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; } = new Dictionary<int, Infectiousness>() {
                { -14, Infectiousness.None },
                { -13, Infectiousness.None },
                { -12, Infectiousness.None },
                { -11, Infectiousness.None },
                { -10, Infectiousness.None },
                { -9, Infectiousness.None },
                { -8, Infectiousness.None },
                { -7, Infectiousness.None },
                { -6, Infectiousness.None },
                { -5, Infectiousness.Standard },
                { -4, Infectiousness.Standard },
                { -3, Infectiousness.Standard },
                { -2, Infectiousness.High },
                { -1, Infectiousness.High },
                { 0, Infectiousness.High },
                { 1, Infectiousness.High },
                { 2, Infectiousness.High },
                { 3, Infectiousness.High },
                { 4, Infectiousness.High },
                { 5, Infectiousness.High },
                { 6, Infectiousness.Standard },
                { 7, Infectiousness.Standard },
                { 8, Infectiousness.Standard },
                { 9, Infectiousness.Standard },
                { 10, Infectiousness.Standard },
                { 11, Infectiousness.None },
                { 12, Infectiousness.None },
                { 13, Infectiousness.None },
                { 14, Infectiousness.None },
            };

            /// <summary>
            /// Infectiousness of TEKs for which onset of symptoms is not set.
            /// </summary>
            [JsonProperty("infectiousness_when_days_since_onset_missing")]
            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;

            /// <summary>
            /// Report type to default to when a TEK has no report type set.
            /// </summary>
            [JsonProperty("report_type_when_missing")]
            public ReportType ReportTypeWhenMissing = ReportType.ConfirmedTest;

            public override bool Equals(object obj)
            {
                return obj is GoogleDiagnosisKeysDataMappingConfiguration configuration &&
                       InfectiousnessForDaysSinceOnsetOfSymptoms.SequenceEqual(configuration.InfectiousnessForDaysSinceOnsetOfSymptoms) &&
                       InfectiousnessWhenDaysSinceOnsetMissing == configuration.InfectiousnessWhenDaysSinceOnsetMissing &&
                       ReportTypeWhenMissing == configuration.ReportTypeWhenMissing;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(InfectiousnessForDaysSinceOnsetOfSymptoms, InfectiousnessWhenDaysSinceOnsetMissing, ReportTypeWhenMissing);
            }
        }

        /// <summary>
        /// The object that contains parameters for configuring exposure notification risk scoring behavior for v1.
        /// </summary>
        ///
        /// https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration/exposure_risk_value_calculation_in_exposurenotification_version_1
        [JsonObject]
        public class AppleExposureConfigurationV1
        {
            /*
             * print(ENExposureConfiguration())
             *
             * NConfig, Flags 0x0 < >,
             * DurW { I 100, N 100, M 100, O 100 },
             * InfW { S 100, H 100 },
             * RpTyW { CTest 100, CClin 100, SelfR 100, Recurs 100 },
             * RpTyNM 1,
             * AttnDurThres[50, 70, 90],
             * DaysTh 0,
             * MinScore 0(0.000),
             * AttnLV[1, 2, 3, 4, 5, 6, 7, 8],
             * DaysLV[1, 2, 3, 4, 5, 6, 7, 8],
             * DurLV[1, 2, 3, 4, 5, 6, 7, 8],
             * TRskLV[1, 2, 3, 4, 5, 6, 7, 8]
             */

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

            /// <summary>
            /// The level values for attenuation.
            /// </summary>
            ///
            /// [0] when Attenuation > 73 dB
            /// [1] when 73 >= Attenuation > 63
            /// [2] when 63 >= Attenuation > 51
            /// [3] when 51 >= Attenuation > 33
            /// [4] when 33 >= Attenuation > 27
            /// [5] when 27 >= Attenuation > 15
            /// [6] when 15 >= Attenuation > 10
            /// [7] when 10 >= Attenuation
            ///
            [JsonProperty("attenuation_level_values")]
            public int[] AttenuationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /// <summary>
            /// The level values for days since last exposure.
            /// </summary>
            ///
            /// [0] when Days >= 14
            /// [1] when Days >= 12
            /// [2] when Days >= 10
            /// [3] when Days >= 8
            /// [4] when Days >= 6
            /// [5] when Days >= 4
            /// [6] when Days >= 2
            /// [7] when Days >= 0
            ///
            [JsonProperty("days_since_last_exposure_level_values")]
            public int[] DaysSinceLastExposureLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /// <summary>
            /// The level values for duration.
            /// </summary>
            ///
            /// [0] when Duration == 0 min
            /// [1] when Duration <= 5
            /// [2] when Duration <= 10
            /// [3] when Duration <= 15
            /// [4] when Duration <= 20
            /// [5] when Duration <= 25
            /// [6] when Duration <= 30
            /// [7] when Duration > 30
            ///
            [JsonProperty("duration_level_values")]
            public int[] DurationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /// <summary>
            /// The level values for transmission risk.
            /// </summary>
            ///
            /// [0]-[7] App Defined
            ///
            [JsonProperty("transmission_risk_level_values")]
            public int[] TransmissionRiskLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            /// <summary>
            /// The value that is the user’s minimum risk score.
            /// </summary>
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

            /// <summary>
            /// The value that is the user’s full-range minimum risk score.
            /// </summary>
            [JsonProperty("minimum_risk_score_full_range")]
            public double MinimumRiskScoreFullRange { get; set; } = 0.0;

            #endregion

            public override bool Equals(object obj)
            {
                return obj is AppleExposureConfigurationV1 configuration &&
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

        /// <summary>
        /// The object that contains parameters for configuring exposure notification risk scoring behavior for v2.
        /// </summary>
        ///
        /// https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration
        [JsonObject]
        public class AppleExposureConfigurationV2
        {
            /*
             * print(ENExposureConfiguration())
             *
             * NConfig, Flags 0x0 < >,
             * DurW { I 100, N 100, M 100, O 100 },
             * InfW { S 100, H 100 },
             * RpTyW { CTest 100, CClin 100, SelfR 100, Recurs 100 },
             * RpTyNM 1,
             * AttnDurThres[50, 70, 90],
             * DaysTh 0,
             * MinScore 0(0.000),
             * AttnLV[1, 2, 3, 4, 5, 6, 7, 8],
             * DaysLV[1, 2, 3, 4, 5, 6, 7, 8],
             * DurLV[1, 2, 3, 4, 5, 6, 7, 8],
             * TRskLV[1, 2, 3, 4, 5, 6, 7, 8]
             */

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

            /// <summary>
            /// The configurable signal-loss thresholds for calculating exposure risk.
            /// </summary>
            ///
            /// [0] The immediate duration threshold.
            /// [1] The near attenuation threshold.
            /// [2] The medium attenuation threshold.
            [JsonProperty("attenuation_duration_thresholds")]
            public int[] AttenuationDurationThresholds { get; set; } = { 50, 70, 90 };

            /// <summary>
            /// The weight assigned to a risk level indicating the duration of the user’s exposure at immediate distance.
            /// </summary>
            [JsonProperty("immediate_duration_weight")]
            public double ImmediateDurationWeight { get; set; } = 100;

            /// <summary>
            /// The weight assigned to a risk level indicating the duration of the user’s exposure at close distance.
            /// </summary>
            [JsonProperty("near_duration_weight")]
            public double NearDurationWeight { get; set; } = 100;

            /// <summary>
            /// The weight assigned to a risk level indicating the duration of the user’s exposure at medium distance.
            /// </summary>
            [JsonProperty("medium_duration_weight")]
            public double MediumDurationWeight { get; set; } = 100;

            /// <summary>
            /// The weight assigned to a risk level indicating the duration of the user’s exposure at a large distance.
            /// </summary>
            [JsonProperty("other_duration_weight")]
            public double OtherDurationWeight { get; set; } = 100;

            /// <summary>
            /// The number of days to consider when calculating the risk level.
            /// </summary>
            public int DaysSinceLastExposureThreshold { get; set; } = 0;

            /// <summary>
            /// The mapping between the days since onset of symptoms to the degree of infectiousness.
            /// </summary>
            ///
            /// Default values from ExposureNotification reference app created by Apple.
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

            /// <summary>
            /// Infectiousness of TEKs for which onset of symptoms is not set.
            /// </summary>
            [JsonProperty("infectiousness_when_days_since_onset_missing")]
            public Infectiousness InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.Standard;

            /// <summary>
            /// The weight to apply for severe infectiousness.
            /// </summary>
            [JsonProperty("infectiousness_high_weight")]
            public double InfectiousnessHighWeight { get; set; } = 100.0; // The range of this value is 0-250%

            /// <summary>
            /// The weight to apply for mild infectiousness.
            /// </summary>
            [JsonProperty("infectiousness_standard_weight")]
            public double InfectiousnessStandardWeight { get; set; } = 100.0; // The range of this value is 0-250%

            // Configuring Report Types

            /// <summary>
            /// The weight assigned to a risk level based on a confirmed clinical diagnosis.
            /// </summary>
            [JsonProperty("report_type_confirmed_clinical_diagnosis_weight")]
            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; } = 100.0;

            /// <summary>
            /// The weight assigned to a risk level based on a confirmed test.
            /// </summary>
            [JsonProperty("report_type_confirmed_test_weight")]
            public double ReportTypeConfirmedTestWeight { get; set; } = 100.0;

            /// <summary>
            /// The weight assigned to a risk level based on an exposure to someone exposed to someone else.
            /// </summary>
            [JsonProperty("report_type_recursive_weight")]
            public double ReportTypeRecursiveWeight { get; set; } = 100.0;

            /// <summary>
            /// The weight assigned to a risk level based on a self-reported diagnoisis.
            /// </summary>
            [JsonProperty("report_type_self_reported_weight")]
            public double ReportTypeSelfReportedWeight { get; set; } = 100.0;

            /// <summary>
            /// The report type to map an unknown diagnosis to.
            /// </summary>
            [JsonProperty("report_type_none_map")]
            public ReportType ReportTypeNoneMap { get; set; } = ReportType.ConfirmedTest;

            public override bool Equals(object obj)
            {
                return obj is AppleExposureConfigurationV2 configuration &&
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
                   EqualityComparer<AppleExposureConfigurationV1>.Default.Equals(AppleExposureConfigV1, configuration.AppleExposureConfigV1) &&
                   EqualityComparer<AppleExposureConfigurationV2>.Default.Equals(AppleExposureConfigV2, configuration.AppleExposureConfigV2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GoogleExposureConfig, GoogleDiagnosisKeysDataMappingConfig, GoogleDailySummariesConfig, AppleExposureConfigV1, AppleExposureConfigV2);
        }
    }
}
