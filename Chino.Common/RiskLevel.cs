namespace Chino
{
    /// <summary>
    /// Risk level defined for a TemporaryExposureKey.
    /// </summary>
    ///
    /// https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/RiskLevel
    /// https://developer.apple.com/documentation/exposurenotification/enexposureinfo/3583716-transmissionrisklevel
    public enum RiskLevel
    {
        Invalid = 0,
        Lowest = 1,
        Low = 2,
        LowMedium = 3,
        Medium = 4,
        MediumHigh = 5,
        High = 6,
        VeryHigh = 7,
        Highest = 8
    }
}
