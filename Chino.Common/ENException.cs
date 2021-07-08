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

        enum Type
        {
            Android = 1 << 7,
            iOS = 1 << 8
        }

        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatusCodes
        public static class Code_Android
        {
            public const int FAILED = (int)Type.Android + 0;
            public const int FAILED_ALREADY_STARTED = (int)Type.Android + 1;
            public const int FAILED_BLUETOOTH_DISABLED = (int)Type.Android + 2;
            public const int FAILED_DISK_IO = (int)Type.Android + 3;
            public const int FAILED_KEY_RELEASE_NOT_PREAUTHORIZED = (int)Type.Android + 4;
            public const int FAILED_NOT_IN_FOREGROUND = (int)Type.Android + 5;
            public const int FAILED_NOT_SUPPORTED = (int)Type.Android + 6;
            public const int FAILED_RATE_LIMITED = (int)Type.Android + 7;
            public const int FAILED_REJECTED_OPT_IN = (int)Type.Android + 8;
            public const int FAILED_SERVICE_DISABLED = (int)Type.Android + 9;
            public const int FAILED_TEMPORARILY_DISABLED = (int)Type.Android + 10;
            public const int FAILED_UNAUTHORIZED = (int)Type.Android + 11;
        }

        // https://developer.apple.com/documentation/exposurenotification/enerror
        public static class Code_iOS
        {
            public const int ApiMisuse = (int)Type.iOS + 0;
            public const int BadFormat = (int)Type.iOS + 1;
            public const int BadParameter = (int)Type.iOS + 2;
            public const int BluetoothOff = (int)Type.iOS + 3;
            public const int InsufficientMemory = (int)Type.iOS + 4;
            public const int InsufficientStorage = (int)Type.iOS + 5;
            public const int Internal = (int)Type.iOS + 6;
            public const int Invalidated = (int)Type.iOS + 7;
            public const int NotAuthorized = (int)Type.iOS + 8;
            public const int NotEnabled = (int)Type.iOS + 9;
            public const int NotEntitled = (int)Type.iOS + 10;
            public const int RateLimited = (int)Type.iOS + 11;
            public const int Restricted = (int)Type.iOS + 12;
            public const int Unknown = (int)Type.iOS + 13;
            public const int Unsupported = (int)Type.iOS + 14;
            public const int DataInaccessible = (int)Type.iOS + 15;
            public const int TravelStatusNotAvailable = (int)Type.iOS + 16;
        }
    }
}

