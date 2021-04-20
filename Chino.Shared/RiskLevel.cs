namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/RiskLevel
    // https://developer.apple.com/documentation/exposurenotification/enexposureinfo/3583716-transmissionrisklevel
    public enum RiskLevel
    {
        Invalid,
        Lowest,
        Low,
        LowMedium,
        Medium,
        MediumHigh,
        High,
        VeryHigh,
        Highest
    }
}
