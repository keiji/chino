﻿using Android.Gms.Common.Apis;

namespace Chino.Android.Google
{
    public static class ApiExceptionStatusCodes
    {
        // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatusCodes
        public const int FAILED_ALREADY_STARTED = 39500;
        public const int FAILED_BLUETOOTH_DISABLED = 39504;
        public const int FAILED_DISK_IO = 39506;
        public const int FAILED_KEY_RELEASE_NOT_PREAUTHORIZED = 39510;
        public const int FAILED_NOT_IN_FOREGROUND = 39509;
        public const int FAILED_NOT_SUPPORTED = 39501;
        public const int FAILED_RATE_LIMITED = 39508;
        public const int FAILED_REJECTED_OPT_IN = 39502;
        public const int FAILED_SERVICE_DISABLED = 39503;
        public const int FAILED_TEMPORARILY_DISABLED = 39505;
        public const int FAILED_UNAUTHORIZED = 39507;

        public static readonly int[] IGNORE_ERRORS = new int[] {
            CommonStatusCodes.ResolutionRequired,
        };
    }
}
