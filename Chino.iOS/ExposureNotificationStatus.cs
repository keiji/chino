using ExposureNotifications;

using Logger = Chino.ChinoLogger;

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
            Logger.D($"EnStatus: {EnStatus}");

            return EnStatus switch
            {
                ENStatus.Active => Chino.Status.Active,
                ENStatus.BluetoothOff => Chino.Status.BluetoothOff,
                ENStatus.Disabled => Chino.Status.NotActive,
                ENStatus.Unauthorized => Chino.Status.Unauthorized,
                ENStatus.Unknown => Chino.Status.Unknown,
                // ENStatus.Restricted =>
                // ENStatus.Paused =>
                _ => Chino.Status.Misc,
            };
        }
    }
}
