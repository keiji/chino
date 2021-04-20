using System.Collections.Generic;

namespace Chino
{
    public class ExposureConfiguration
    {
        public GoogleExposureConfiguration GoogleExposureConfig { get; set; }
        public DailySummariesConfig GoogleDailySummariesConfig { get; set; } // for ExposureWindow Mode

        public AppleExposureConfiguration AppleExposureConfig { get; set; }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        public class GoogleExposureConfiguration
        {
            public int[] AttenuationScores { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            public int AttenuationWeight { get; set; } = 50;

            public int[] DaysSinceLastExposureScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            public int DaysSinceLastExposureWeight { get; set; } = 50;

            public int[] DurationAtAttenuationThresholds { get; set; } = { 50, 70 };

            public int[] DurationScores { get; set; } = { 1, 1, 1, 1, 1, 1, 1, 1 };

            public int DurationWeight { get; set; } = 50;

            public int MinimumRiskScore { get; set; } = 21;

            public int[] TransmissionRiskScores { get; set; } = { 7, 7, 7, 7, 7, 7, 7, 7 };

            public int TransmissionRiskWeight { get; set; } = 50;
        }

        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration
        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration/exposure_risk_value_calculation_in_exposurenotification_version_1
        public class AppleExposureConfiguration
        {
            // Configuring Duration

            #region These properties are available in iOS 12.5, and in iOS 13.5 and later.

            public double ImmediateDurationWeight { get; set; } = 100;
            public double MediumDurationWeight { get; set; } = 100;
            public double NearDurationWeight { get; set; } = 100;
            public double OtherDurationWeight { get; set; } = 100;

            public int DaysSinceLastExposureThreshold { get; set; } = 0;

            // Configuring Infectiousness

            // Must Specify v2
            public IDictionary<int, int> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; } = new Dictionary<int, int>();

            public double InfectiousnessHighWeight { get; set; } = 100.0; // The range of this value is 0-250%
            public double InfectiousnessStandardWeight { get; set; } = 100.0; // The range of this value is 0-250%
            // public int DaysSinceOnsetOfSymptomsUnknown { get; set; }

            // Configuring Report Types

            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; } = 100.0;
            public double ReportTypeConfirmedTestWeight { get; set; } = 100.0;
            public double ReportTypeRecursiveWeight { get; set; } = 100.0;
            public double ReportTypeSelfReportedWeight { get; set; } = 100.0;
            public ReportType ReportTypeNoneMap { get; set; } = ReportType.Unknown;

            public int[] AttenuationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            public int[] DaysSinceLastExposureLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

            public int[] DurationLevelValues { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8 };

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

            public int[] AttenuationDurationThreshold { get; set; } = { 50, 70 };

            public double MinimumRiskScoreFullRange { get; set; } = 0.0;

            #endregion

            public AppleExposureConfiguration()
            {
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-14, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-13, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-12, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-11, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-10, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-9, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-8, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-7, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-6, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-5, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-4, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-3, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-2, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(-1, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(0, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(1, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(2, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(3, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(4, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(5, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(6, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(7, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(8, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(9, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(10, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(11, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(12, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(13, 1);
                InfectiousnessForDaysSinceOnsetOfSymptoms.Add(14, 1);
            }
        }
    }
}
