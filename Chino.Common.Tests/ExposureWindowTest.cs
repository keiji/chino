using System.Collections.Generic;
using System.Linq;
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
            Assert.AreEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest2()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.CalibrationConfidence = CalibrationConfidence.High;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest3()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.DateMillisSinceEpoch = 167545000;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest4()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.Infectiousness = Infectiousness.High;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest5()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ReportType = ReportType.ConfirmedClinicalDiagnosis;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest6()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance() };
            Assert.AreEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest7()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance() };
            Assert.AreEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.True(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest8()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow2.ScanInstances = new List<ScanInstance>() { CreateScanInstance(), CreateScanInstance() };
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
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
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));
        }

        [Test]
        public void EaualsTest10()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();

            exposureWindow1.ScanInstances = null;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));

            exposureWindow1 = CreateExposureWindow();
            exposureWindow2 = CreateExposureWindow();
            exposureWindow2.ScanInstances = null;
            Assert.AreNotEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
            Assert.False(exposureWindow1.Equals(exposureWindow2));

            exposureWindow1 = CreateExposureWindow();
            exposureWindow2 = CreateExposureWindow();
            exposureWindow1.ScanInstances = null;
            exposureWindow2.ScanInstances = null;
            Assert.AreEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
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
            Assert.AreEqual(exposureWindow1.GetHashCode(), exposureWindow2.GetHashCode());
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

        [Test]
        public void UnionTest1()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();
            exposureWindow2.Infectiousness = Infectiousness.High;

            var exposureWindow3 = CreateExposureWindow();
            var exposureWindow4 = CreateExposureWindow();
            exposureWindow4.Infectiousness = Infectiousness.None;

            var list1 = new List<ExposureWindow>() {
                exposureWindow1,
                exposureWindow2
            };
            var list2 = new List<ExposureWindow>() {
                exposureWindow3,
                exposureWindow4
            };

            var unionList = list1.Union(list2);
            Assert.AreEqual(3, unionList.Count());
            Assert.True(unionList.Contains(exposureWindow2));
            Assert.True(unionList.Contains(exposureWindow4));

            Assert.True(unionList.Contains(exposureWindow1));
            Assert.True(unionList.Contains(exposureWindow3));
        }

        [Test]
        public void SortTest1()
        {
            var exposureWindow1 = CreateExposureWindow();
            var exposureWindow2 = CreateExposureWindow();
            exposureWindow2.Infectiousness = Infectiousness.High;
            var exposureWindow3 = CreateExposureWindow();
            exposureWindow3.ReportType= ReportType.ConfirmedTest;
            var exposureWindow4 = CreateExposureWindow();
            exposureWindow4.Infectiousness = Infectiousness.None;
            exposureWindow4.ReportType = ReportType.SelfReport;

            var list = new List<ExposureWindow>() {
                exposureWindow1,
                exposureWindow2,
                exposureWindow3,
                exposureWindow4
            };

            list.Sort(new ExposureWindow.Comparer());
            Assert.AreEqual(4, list.Count());

            Assert.AreEqual(exposureWindow2, list[0]);
            Assert.AreEqual(exposureWindow3, list[1]);
            Assert.AreEqual(exposureWindow1, list[2]);
            Assert.AreEqual(exposureWindow4, list[3]);
        }
    }
}
