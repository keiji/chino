using System;
using ExposureNotifications;

namespace Chino
{
    // https://developer.apple.com/documentation/exposurenotification/entemporaryexposurekey
    public class TemporaryExposureKey : ITemporaryExposureKey
    {
        public TemporaryExposureKey(ENTemporaryExposureKey source)
        {
            //DaysSinceOnsetOfSymptoms = source.DaysSinceOnsetOfSymptoms;
            KeyData = GetKeyData(source.KeyData);
            RollingPeriod = (int)source.RollingPeriod;
            RollingStartIntervalNumber = (int)source.RollingStartNumber;
            RiskLevel = (RiskLevel)Enum.ToObject(typeof(RiskLevel), source.TransmissionRiskLevel);
            ReportType = ReportType.ConfirmedTest;
        }

        public int DaysSinceOnsetOfSymptoms { get; set; }
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public ReportType ReportType { get; set; }

        private static byte[] GetKeyData(Foundation.NSData keyData)
        {
            byte[] dataBytes = new byte[keyData.Length];
            System.Runtime.InteropServices.Marshal.Copy(keyData.Bytes, dataBytes, 0, Convert.ToInt32(keyData.Length));
            return dataBytes;
        }
    }
}
