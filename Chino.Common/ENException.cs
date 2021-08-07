using System;

namespace Chino
{
    public class ENException : Exception
    {
        public readonly int Code;

        public ENException(int code, string message) : base(message)
        {
            Code = code;
        }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatusCodes
        public static class Code_Android
        {

            /// <summary>
            /// The operation failed, without any more information.
            /// </summary>
            public const int FAILED = (int)PlatformType.Android + 0;

            /// <summary>
            /// The app was already in the requested state so the call did nothing.
            /// </summary>
            public const int FAILED_ALREADY_STARTED = (int)PlatformType.Android + 1;

            /// <summary>
            /// The bluetooth was powered off.
            /// </summary>
            public const int FAILED_BLUETOOTH_DISABLED = (int)PlatformType.Android + 2;

            /// <summary>
            /// The operation failed during a disk read/write.
            /// </summary>
            public const int FAILED_DISK_IO = (int)PlatformType.Android + 3;

            /// <summary>
            /// The client hasn't previously requested the pre-release of keys, or else the user denied the request.
            /// </summary>
            public const int FAILED_KEY_RELEASE_NOT_PREAUTHORIZED = (int)PlatformType.Android + 4;

            /// <summary>
            /// The client is not currently in the foreground, which is required for the API to return results.
            /// </summary>
            public const int FAILED_NOT_IN_FOREGROUND = (int)PlatformType.Android + 5;

            /// <summary>
            /// The hardware capability of the device was not supported.
            /// </summary>
            public const int FAILED_NOT_SUPPORTED = (int)PlatformType.Android + 6;

            /// <summary>
            /// The client has been rate limited for access to this API.
            /// </summary>
            public const int FAILED_RATE_LIMITED = (int)PlatformType.Android + 7;

            /// <summary>
            /// The user rejected the opt-in state.
            /// </summary>
            public const int FAILED_REJECTED_OPT_IN = (int)PlatformType.Android + 8;

            /// <summary>
            /// The functionality was disabled by the user or the phone.
            /// </summary>
            public const int FAILED_SERVICE_DISABLED = (int)PlatformType.Android + 9;

            /// <summary>
            /// The service was disabled for some reasons temporarily.
            /// </summary>
            public const int FAILED_TEMPORARILY_DISABLED = (int)PlatformType.Android + 10;

            /// <summary>
            /// The client is unauthorized to access the APIs.
            /// </summary>
            public const int FAILED_UNAUTHORIZED = (int)PlatformType.Android + 11;
        }

        // https://developer.apple.com/documentation/exposurenotification/enerror
        public static class Code_iOS
        {

            /// <summary>
            /// The API use is incorrect.
            /// </summary>
            public const int ApiMisuse = (int)PlatformType.iOS + 0;

            /// <summary>
            /// A file is formated incorrectly.
            /// </summary>
            public const int BadFormat = (int)PlatformType.iOS + 1;

            /// <summary>
            /// The parameter is missing or incorrect.
            /// </summary>
            public const int BadParameter = (int)PlatformType.iOS + 2;

            /// <summary>
            /// Bluetooth is turned off.
            /// </summary>
            public const int BluetoothOff = (int)PlatformType.iOS + 3;

            /// <summary>
            /// The memory is insufficient to perform the operation.
            /// </summary>
            public const int InsufficientMemory = (int)PlatformType.iOS + 4;

            /// <summary>
            /// The storage is insufficient to enable notifications.
            /// </summary>
            public const int InsufficientStorage = (int)PlatformType.iOS + 5;

            /// <summary>
            /// A bug in the internal notification framework.
            /// </summary>
            public const int Internal = (int)PlatformType.iOS + 6;

            /// <summary>
            /// A call to invalidate before the operation completes normally.
            /// </summary>
            public const int Invalidated = (int)PlatformType.iOS + 7;

            /// <summary>
            /// The user has denied access to the notification framework.
            /// </summary>
            public const int NotAuthorized = (int)PlatformType.iOS + 8;

            /// <summary>
            /// Notification is not enabled.
            /// </summary>
            public const int NotEnabled = (int)PlatformType.iOS + 9;

            /// <summary>
            /// Process of calling is not entitled.
            /// </summary>
            public const int NotEntitled = (int)PlatformType.iOS + 10;

            /// <summary>
            /// API calls are too frequent.
            /// </summary>
            public const int RateLimited = (int)PlatformType.iOS + 11;

            /// <summary>
            /// Exposure notification is disabled due to system policies.
            /// </summary>
            public const int Restricted = (int)PlatformType.iOS + 12;

            /// <summary>
            /// Failure has an unknown cause.
            /// </summary>
            public const int Unknown = (int)PlatformType.iOS + 13;

            /// <summary>
            /// Operation is not supported.
            /// </summary>
            public const int Unsupported = (int)PlatformType.iOS + 14;

            /// <summary>
            /// The user must unlock the device before it can access data.
            /// </summary>
            public const int DataInaccessible = (int)PlatformType.iOS + 15;

            /// <summary>
            /// The system can’t determine whether the user is traveling.
            /// </summary>
            public const int TravelStatusNotAvailable = (int)PlatformType.iOS + 16;
        }
    }
}

