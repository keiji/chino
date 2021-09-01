using System.Collections.Generic;
using NUnit.Framework;

namespace Chino.Tests
{
    public class ExposureInformationTest
    {
        private readonly string PATH_SINGLE_JSON = "./files/exposure_information.json";
        private readonly string PATH_LIST_JSON = "./files/exposure_informations.json";

        [Test]
        public void TestDeserializeFromJson1()
        {
            var exposureInformation = Utils.ReadObjectFromJsonPath<ExposureInformation>(PATH_SINGLE_JSON);
            Assert.IsNotNull(exposureInformation);

            Assert.AreEqual(4, exposureInformation.AttenuationDurationsInMillis.Length);
            Assert.AreEqual(1800000, exposureInformation.AttenuationDurationsInMillis[0]);
            Assert.AreEqual(0, exposureInformation.AttenuationDurationsInMillis[1]);
            Assert.AreEqual(0, exposureInformation.AttenuationDurationsInMillis[2]);
            Assert.AreEqual(0, exposureInformation.AttenuationDurationsInMillis[3]);

            Assert.AreEqual(4, exposureInformation.AttenuationValue);
            Assert.AreEqual(1629331200000, exposureInformation.DateMillisSinceEpoch);
            Assert.AreEqual(1800000.0, exposureInformation.DurationInMillis);
            Assert.AreEqual(255, exposureInformation.TotalRiskScore);
            Assert.AreEqual(RiskLevel.Medium, exposureInformation.TransmissionRiskLevel);
        }

        [Test]
        public void TestDeserializeFromJson2()
        {
            var exposureWindows = Utils.ReadObjectFromJsonPath<List<ExposureInformation>>(PATH_LIST_JSON);
            Assert.AreEqual(11, exposureWindows.Count);
        }

        private ExposureInformation CreateExposureInformation()
            => new ExposureInformation()
            {
                AttenuationDurationsInMillis = new int[]
                {
                    1800000,
                    0,
                    0,
                    0,
                },
                AttenuationValue = 4,
                DateMillisSinceEpoch = 1629331200000,
                DurationInMillis = 1800000.0,
                TotalRiskScore = 255,
                TransmissionRiskLevel = RiskLevel.Medium,
            };

        [Test]
        public void EaualsTest1()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();
            Assert.True(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest2()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.AttenuationDurationsInMillis = new int[]
            {
                1800000,
                0,
                0,
                0,
            };
            Assert.True(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest3()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.AttenuationDurationsInMillis = new int[]
            {
                1800000,
                600000,
                0,
                0,
            };
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest4()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.AttenuationValue = 65;
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest5()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.DateMillisSinceEpoch = 6000000;
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest6()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.DurationInMillis = 60000;
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest7()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.TotalRiskScore = 80;
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }

        [Test]
        public void EaualsTest8()
        {
            var exposureInformation1 = CreateExposureInformation();
            var exposureInformation2 = CreateExposureInformation();

            exposureInformation2.TransmissionRiskLevel = RiskLevel.Lowest;
            Assert.False(exposureInformation1.Equals(exposureInformation2));
        }
    }
}
