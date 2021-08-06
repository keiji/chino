using System.Collections.Generic;
using NUnit.Framework;

namespace Chino.Common.Tests
{
    public class ExposureWindowTest
    {
        private readonly string PATH_SINGLE_JSON = "./files/exposure_window.json";
        private readonly string PATH_LIST_JSON = "./files/exposure_windows.json";

        [Test]
        public void TestDeserializeFromJson1()
        {
            var exposureWindow = Utils.ReadObjectFromJsonPath<ExposureWindow>(PATH_SINGLE_JSON);
            Assert.IsNotNull(exposureWindow);

            Assert.AreEqual(CalibrationConfidence.High, exposureWindow.CalibrationConfidence);
            Assert.AreEqual(1626480000000, exposureWindow.DateMillisSinceEpoch);
            Assert.AreEqual(Infectiousness.Standard, exposureWindow.Infectiousness);
            Assert.AreEqual(ReportType.ConfirmedTest, exposureWindow.ReportType);

            var scanInstances = new List<ScanInstance>(exposureWindow.ScanInstances);

            Assert.AreEqual(4, scanInstances.Count);

            {
                ScanInstance si = scanInstances[0];
                Assert.AreEqual(38, si.MinAttenuationDb);
                Assert.AreEqual(300, si.SecondsSinceLastScan);
                Assert.AreEqual(39, si.TypicalAttenuationDb);
            }
            {
                ScanInstance si = scanInstances[1];
                Assert.AreEqual(39, si.MinAttenuationDb);
                Assert.AreEqual(300, si.SecondsSinceLastScan);
                Assert.AreEqual(39, si.TypicalAttenuationDb);
            }
            {
                ScanInstance si = scanInstances[2];
                Assert.AreEqual(39, si.MinAttenuationDb);
                Assert.AreEqual(300, si.SecondsSinceLastScan);
                Assert.AreEqual(40, si.TypicalAttenuationDb);
            }
            {
                ScanInstance si = scanInstances[3];
                Assert.AreEqual(38, si.MinAttenuationDb);
                Assert.AreEqual(300, si.SecondsSinceLastScan);
                Assert.AreEqual(40, si.TypicalAttenuationDb);
            }
        }

        [Test]
        public void TestDeserializeFromJson2()
        {
            var exposureWindows = Utils.ReadObjectFromJsonPath<List<ExposureWindow>>(PATH_LIST_JSON);
            Assert.AreEqual(345, exposureWindows.Count);
        }


    }
}
