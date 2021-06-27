using AndroidExposureNotificationStatus = Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus;

namespace Chino.Android.Google
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatus
    public class ExposureNotificationStatus : IExposureNotificationStatus
    {

        public readonly AndroidExposureNotificationStatus EnStatus;

        public ExposureNotificationStatus(AndroidExposureNotificationStatus exposureNotificationStatus)
        {
            EnStatus = exposureNotificationStatus;
        }

        public Status Status()
        {
            if (EnStatus == AndroidExposureNotificationStatus.Activated)
            {
                return Chino.Status.Active;
            }
            else if (EnStatus == AndroidExposureNotificationStatus.Inactivated)
            {
                return Chino.Status.NotActive;
            }
            else if (EnStatus == AndroidExposureNotificationStatus.BluetoothDisabled)
            {
                return Chino.Status.BluetoothOff;
            }
            else if (EnStatus == AndroidExposureNotificationStatus.NoConsent)
            {
                return Chino.Status.Unauthorized;
            }
            else if (EnStatus == AndroidExposureNotificationStatus.Unknown)
            {
                return Chino.Status.Unknown;
            }
            else
            {
                return Chino.Status.Misc;
            }
        }
    }
}
