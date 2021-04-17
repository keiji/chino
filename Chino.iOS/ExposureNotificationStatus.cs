using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/enstatus
    public class ExposureNotificationStatus : IExposureNotificationStatus
    {

        public readonly ENStatus EnStatus;

        public ExposureNotificationStatus(ENStatus eNStatus)
        {
            EnStatus = eNStatus;
        }

        public Status Status()
        {
            switch (EnStatus)
            {
                case ENStatus.Active:
                    return Chino.Status.Active;
                case ENStatus.BluetoothOff:
                    return Chino.Status.BluetoothOff;
                case ENStatus.Disabled:
                    return Chino.Status.NotActive;
                case ENStatus.Unauthorized:
                    return Chino.Status.Unauthorized;
                case ENStatus.Unknown:
                    return Chino.Status.Unknown;
                //case ENStatus.Restricted:
                //    break;
                //case ENStatus.Paused:
                //    break;
                default:
                    return Chino.Status.Misc;
            }
        }
    }
}
