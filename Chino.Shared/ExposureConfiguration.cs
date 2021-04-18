using System.Collections.Generic;

namespace Chino
{
    public class ExposureConfiguration
    {
        public GoogleExposureConfiguration GoogleExposureConfig { get; set; }
        public AppleExposureConfiguration AppleExposureConfig { get; set; }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        public class GoogleExposureConfiguration {
            public int[] AttenuationScores { get; set; }

            public int AttenuationWeight { get; set; }

            public int[] DaysSinceLastExposureScores { get; set; }

            public int DaysSinceLastExposureWeight { get; set; }

            public int[] DurationAtAttenuationThresholds { get; set; }

            public int[] DurationScores { get; set; }

            public int DurationWeight { get; set; }

            public int MinimumRiskScore { get; set; }

            public int[] TransmissionRiskScores { get; set; }

            public int TransmissionRiskWeight { get; set; }
        }

        // https://developer.apple.com/documentation/exposurenotification/enexposureconfiguration
        public class AppleExposureConfiguration
        {
            // Configuring Duration
            public int[] AttenuationDurationThreshold { get; set; } = { 50, 70 };

            public double ImmediateDurationWeight { get; set; } = 100;
            public double MediumDurationWeight { get; set; } = 100;
            public double NearDurationWeight { get; set; } = 100;
            public double OtherDurationWeight { get; set; } = 100;

            public int DaysSinceLastExposureThreshold { get; set; }

            // Configuring Infectiousness

            // Must Specify v2
            public IDictionary<int, int> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; } = new Dictionary<int, int>();
            public double InfectiousnessHighWeight { get; set; }
            public double InfectiousnessStandardWeight { get; set; }
            // public int DaysSinceOnsetOfSymptomsUnknown { get; set; }

            // Configuring Report Types
            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; }
            public double ReportTypeConfirmedTestWeight { get; set; }
            public double ReportTypeRecursiveWeight { get; set; }
            public double ReportTypeSelfReportedWeight { get; set; }
            public ReportType ReportTypeNoneMap { get; set; } = ReportType.Unknown;

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
