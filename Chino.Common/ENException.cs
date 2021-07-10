using System;

namespace Chino.Common
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
            public const int FAILED = (int)PlatformType.Android + 0;
            public const int FAILED_ALREADY_STARTED = (int)PlatformType.Android + 1;
            public const int FAILED_BLUETOOTH_DISABLED = (int)PlatformType.Android + 2;
            public const int FAILED_DISK_IO = (int)PlatformType.Android + 3;
            public const int FAILED_KEY_RELEASE_NOT_PREAUTHORIZED = (int)PlatformType.Android + 4;
            public const int FAILED_NOT_IN_FOREGROUND = (int)PlatformType.Android + 5;
            public const int FAILED_NOT_SUPPORTED = (int)PlatformType.Android + 6;
            public const int FAILED_RATE_LIMITED = (int)PlatformType.Android + 7;
            public const int FAILED_REJECTED_OPT_IN = (int)PlatformType.Android + 8;
            public const int FAILED_SERVICE_DISABLED = (int)PlatformType.Android + 9;
            public const int FAILED_TEMPORARILY_DISABLED = (int)PlatformType.Android + 10;
            public const int FAILED_UNAUTHORIZED = (int)PlatformType.Android + 11;
        }

        // https://developer.apple.com/documentation/exposurenotification/enerror
        public static class Code_iOS
        {
            public const int ApiMisuse = (int)PlatformType.iOS + 0;
            public const int BadFormat = (int)PlatformType.iOS + 1;
            public const int BadParameter = (int)PlatformType.iOS + 2;
            public const int BluetoothOff = (int)PlatformType.iOS + 3;
            public const int InsufficientMemory = (int)PlatformType.iOS + 4;
            public const int InsufficientStorage = (int)PlatformType.iOS + 5;
            public const int Internal = (int)PlatformType.iOS + 6;
            public const int Invalidated = (int)PlatformType.iOS + 7;
            public const int NotAuthorized = (int)PlatformType.iOS + 8;
            public const int NotEnabled = (int)PlatformType.iOS + 9;
            public const int NotEntitled = (int)PlatformType.iOS + 10;
            public const int RateLimited = (int)PlatformType.iOS + 11;
            public const int Restricted = (int)PlatformType.iOS + 12;
            public const int Unknown = (int)PlatformType.iOS + 13;
            public const int Unsupported = (int)PlatformType.iOS + 14;
            public const int DataInaccessible = (int)PlatformType.iOS + 15;
            public const int TravelStatusNotAvailable = (int)PlatformType.iOS + 16;
        }
    }
}

