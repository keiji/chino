using System;
using ExposureNotifications;

namespace Chino.iOS
{
    // https://developer.apple.com/documentation/exposurenotification/entemporaryexposurekey
    public class PlatformTemporaryExposureKey : TemporaryExposureKey
    {
        public PlatformTemporaryExposureKey() { }

        public PlatformTemporaryExposureKey(ENTemporaryExposureKey source)
        {
            // DaysSinceOnsetOfSymptoms = source.DaysSinceOnsetOfSymptoms;
            KeyData = GetKeyData(source.KeyData);
            RollingPeriod = (int)source.RollingPeriod;
            RollingStartIntervalNumber = (int)source.RollingStartNumber;
            RiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
            ReportType = ReportType.ConfirmedTest;
        }

        private static byte[] GetKeyData(Foundation.NSData keyData)
        {
            byte[] dataBytes = new byte[keyData.Length];
            System.Runtime.InteropServices.Marshal.Copy(keyData.Bytes, dataBytes, 0, Convert.ToInt32(keyData.Length));
            return dataBytes;
        }
    }
}
