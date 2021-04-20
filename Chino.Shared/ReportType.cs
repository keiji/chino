namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ReportType
    public enum ReportType
    {
        Unknown = 0,
        ConfirmedTest = 1,
        ConfirmedClinicalDiagnosis = 2,
        SelfReport = 3,
        Recursive = 4,
        Revoked = 5
    }
}
