namespace Chino
{
    /// <summary>
    /// Detail status for exposure notification service.
    /// </summary>
    public class ExposureNotificationStatus
    {
        public readonly int Code;

        public ExposureNotificationStatus(int code)
        {
            Code = code;
        }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatus
        public static class Code_Android
        {
            /// <summary>
            /// Exposure notification is running.
            /// </summary>
            public const int ACTIVATED = (int)PlatformType.Android + 0;

            /// <summary>
            /// Bluetooth is not enabled.
            /// </summary>
            public const int BLUETOOTH_DISABLED = (int)PlatformType.Android + 1;

            /// <summary>
            /// Can't detected the BLE supporting of this device due to bluetooth is not enabled.
            /// </summary>
            public const int BLUETOOTH_SUPPORT_UNKNOWN = (int)PlatformType.Android + 2;

            /// <summary>
            /// Exposure notification is not supported.
            /// </summary>
            public const int EN_NOT_SUPPORT = (int)PlatformType.Android + 3;

            /// <summary>
            /// There is another client running as active client.
            /// </summary>
            public const int FOCUS_LOST = (int)PlatformType.Android + 4;

            /// <summary>
            /// Hardware of this device doesn't support exposure notification.
            /// </summary>
            public const int HW_NOT_SUPPORT = (int)PlatformType.Android + 5;

            /// <summary>
            /// Exposure notification is not running.
            /// </summary>
            public const int INACTIVATED = (int)PlatformType.Android + 6;

            /// <summary>
            /// Location is not enabled.
            /// </summary>
            public const int LOCATION_DISABLED = (int)PlatformType.Android + 7;

            /// <summary>
            /// Device storage is not sufficient for exposure notification.
            /// </summary>
            public const int LOW_STORAGE = (int)PlatformType.Android + 8;

            /// <summary>
            /// The client is not in approved client list.
            /// </summary>
            public const int NOT_IN_ALLOWLIST = (int)PlatformType.Android + 9;

            /// <summary>
            /// User is not consent for the client.
            /// </summary>
            public const int NO_CONSENT = (int)PlatformType.Android + 10;

            /// <summary>
            /// Current status is unknown.
            /// </summary>
            public const int UNKNOWN = (int)PlatformType.Android + 11;

            /// <summary>
            /// Exposure notification is not supported for current user profile.
            /// </summary>
            public const int USER_PROFILE_NOT_SUPPORT = (int)PlatformType.Android + 12;
        }

        // https://developer.apple.com/documentation/exposurenotification/enstatus
        public static class Code_iOS
        {
            /// <summary>
            /// Notification is active.
            /// </summary>
            public const int Active = (int)PlatformType.iOS + 0;

            /// <summary>
            /// Bluetooth is turned off.
            /// </summary>
            public const int BluetoothOff = (int)PlatformType.iOS + 1;

            /// <summary>
            /// Notification is disabled.
            /// </summary>
            public const int Disabled = (int)PlatformType.iOS + 2;

            /// <summary>
            /// Notification is not active due to system restrictions, such as parental controls.
            /// </summary>
            public const int Restricted = (int)PlatformType.iOS + 3;

            /// <summary>
            /// Notification is unknown.
            /// </summary>
            public const int Unknown = (int)PlatformType.iOS + 4;

            /// <summary>
            /// The user paused Exposure Notification.
            /// </summary>
            public const int Paused = (int)PlatformType.iOS + 5;

            /// <summary>
            /// The user hasn’t authorized Exposure Notification.
            /// </summary>
            public const int Unauthorized = (int)PlatformType.iOS + 6;
        }
    }
}
