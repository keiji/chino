namespace Chino
{
    /// <summary>
    /// Detail status for exposure notification service.
    /// </summary>
    public interface IExposureNotificationStatus
    {
        public abstract Status Status();
    }

    public enum Status
    {
        /// <summary>
        /// Exposure notification is running.
        /// </summary>
        Active,

        /// <summary>
        /// Bluetooth is not enabled.
        /// </summary>
        BluetoothOff,

        /// <summary>
        /// Exposure notification is not running.
        /// </summary>
        NotActive,

        /// <summary>
        /// User is not consent for the client.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Current status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Other state.
        /// </summary>
        Misc
    }
}