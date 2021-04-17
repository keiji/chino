namespace Chino
{
    public interface IExposureNotificationStatus
    {
        public abstract Status Status();
    }

    public enum Status
    {
        Active,
        BluetoothOff,
        NotActive,
        Unauthorized,
        Unknown,
        Misc
    }
}