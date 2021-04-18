using System.Collections.Generic;

namespace Chino
{
    public class ExposureConfiguration
    {
        public IGoogleExposureConfiguration GoogleExposureConfiguration { get; set; }
        public IAppleExposureConfiguration AppleExposureConfiguration { get; set; }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureConfiguration
        public interface IGoogleExposureConfiguration {
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
        public interface IAppleExposureConfiguration
        {
            // Configuring Duration
            public int[] AttenuationDurationThreshold { get; set; }

            public double ImmediateDurationWeight { get; set; }
            public double MediumDurationWeight { get; set; }
            public double NearDurationWeight { get; set; }
            public double OtherDurationWeight { get; set; }

            public int DaysSinceLastExposureThreshold { get; set; }

            // Configuring Infectiousness
            public IDictionary<int, int> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; }
            public double InfectiousnessHighWeight { get; set; }
            public double InfectiousnessStandardWeight { get; set; }
            // public int DaysSinceOnsetOfSymptomsUnknown { get; set; }

            // Configuring Report Types
            public double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; }
            public double ReportTypeConfirmedTestWeight { get; set; }
            public double ReportTypeRecursiveWeight { get; set; }
            public double ReportTypeSelfReportedWeight { get; set; }
            public ReportType ReportTypeNoneMap { get; set; }
        }
    }
}
