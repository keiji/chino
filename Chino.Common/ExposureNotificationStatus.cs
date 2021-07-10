using Chino.Common;

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
            public const int ACTIVATED = (int)PlatformType.Android + 0;
            public const int BLUETOOTH_DISABLED = (int)PlatformType.Android + 1;
            public const int BLUETOOTH_SUPPORT_UNKNOWN = (int)PlatformType.Android + 2;
            public const int EN_NOT_SUPPORT = (int)PlatformType.Android + 3;
            public const int FOCUS_LOST = (int)PlatformType.Android + 4;
            public const int HW_NOT_SUPPORT = (int)PlatformType.Android + 5;
            public const int INACTIVATED = (int)PlatformType.Android + 6;
            public const int LOCATION_DISABLED = (int)PlatformType.Android + 7;
            public const int LOW_STORAGE = (int)PlatformType.Android + 8;
            public const int NOT_IN_ALLOWLIST = (int)PlatformType.Android + 9;
            public const int NO_CONSENT = (int)PlatformType.Android + 10;
            public const int UNKNOWN = (int)PlatformType.Android + 11;
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
