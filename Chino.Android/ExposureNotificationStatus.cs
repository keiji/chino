namespace Chino
{
    // https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationStatus
    public class ExposureNotificationStatus : IExposureNotificationStatus
    {

        public readonly Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus EnStatus;

        public ExposureNotificationStatus(Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus exposureNotificationStatus)
        {
            EnStatus = exposureNotificationStatus;
        }

        public Status Status()
        {
            if (EnStatus == Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus.Activated)
            {
                return Chino.Status.Active;
            }
            else if (EnStatus == Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus.Inactivated)
            {
                return Chino.Status.NotActive;
            }
            else if (EnStatus == Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus.BluetoothDisabled)
            {
                return Chino.Status.BluetoothOff;
            }
            else if (EnStatus == Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus.NoConsent)
            {
                return Chino.Status.Unauthorized;
            }
            else if (EnStatus == Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus.Unknown)
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
