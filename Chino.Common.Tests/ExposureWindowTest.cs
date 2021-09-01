using System.Collections.Generic;
using NUnit.Framework;

namespace Chino.Tests
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

        private ScanInstance CreateScanInstance()
            => new ScanInstance()
                {
                    MinAttenuationDb = 45,
                    SecondsSinceLastScan = 300,
                    TypicalAttenuationDb = 37,
                };

        private ExposureWindow CreateExposureWindow()
            => new ExposureWindow() {
                CalibrationConfidence = CalibrationConfidence.Medium,
                DateMillisSinceEpoch = 152345000,
                Infectiousness = Infectiousness.Standard,
                ReportType = ReportType.ConfirmedTest,
                ScanInstances = new List<ScanInstance>() { CreateScanInstance() },
            };

        [Test]
        public void EaualsTest1()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest2()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.CalibrationConfidence = CalibrationConfidence.High;
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest3()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.DateMillisSinceEpoch = 167545000;
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest4()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.Infectiousness = Infectiousness.High;
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest5()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ReportType = ReportType.ConfirmedClinicalDiagnosis;
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest6()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance() };
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest7()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance() };
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest8()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance(), CreateScanInstance() };
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest9()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            var scanInstance = CreateScanInstance();
            scanInstance.MinAttenuationDb = 22;
            exposureWindow2.ScanInstances = new List<ScanInstance>() { scanInstance };
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest10()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow1.ScanInstances = null;
            Assert.False(exposureWindow1.Equals(exposureWindow2));

            exposureWindow1 = CreateExposureWindow();
            exposureWindow2 = CreateExposureWindow();
            exposureWindow2.ScanInstances = null;
            Assert.False(exposureWindow1.Equals(exposureWindow2));

            exposureWindow1 = CreateExposureWindow();
            exposureWindow2 = CreateExposureWindow();
            exposureWindow1.ScanInstances = null;
            exposureWindow2.ScanInstances = null;
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest11()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = null;
            Assert.False(exposureWindow1.Equals(exposureWindow2));

            exposureWindow1 = CreateExposureWindow();
            exposureWindow2 = CreateExposureWindow();
            exposureWindow1.ScanInstances = null;
            exposureWindow2.ScanInstances = null;
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest12()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow1.ScanInstances = null;
            exposureWindow2.ScanInstances = null;
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }
    }
}
