using NUnit.Framework;

namespace Chino.Tests
{
    public class ExposureSummaryTest
    {
        private readonly string PATH_JSON = "./files/exposure_summary.json";

        [Test]
        public void TestDeserializeFromJson1()
        {
            var exposureSummary = Utils.ReadObjectFromJsonPath<ExposureSummary>(PATH_JSON);
            Assert.IsNotNull(exposureSummary);

            Assert.AreEqual(3, exposureSummary.AttenuationDurationsInMillis.Length);
            Assert.AreEqual(1800000, exposureSummary.AttenuationDurationsInMillis[0]);
            Assert.AreEqual(1560000, exposureSummary.AttenuationDurationsInMillis[1]);
            Assert.AreEqual(0, exposureSummary.AttenuationDurationsInMillis[2]);
            Assert.AreEqual(0, exposureSummary.DaysSinceLastExposure);
            Assert.AreEqual(11, exposureSummary.MatchedKeyCount);
            Assert.AreEqual(255, exposureSummary.MaximumRiskScore);
            Assert.AreEqual(10280, exposureSummary.SummationRiskScore);
        }

        private ExposureSummary CreateExposureSummary()
            => new ExposureSummary() {
                AttenuationDurationsInMillis = new int[] {
                    1800000,
                    1560000,
                    0
                },
                DaysSinceLastExposure = 0,
                MatchedKeyCount = 11,
                MaximumRiskScore = 255,
                SummationRiskScore = 10280,
            };

        [Test]
        public void EaualsTest1()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            Assert.True(exposureSummary1.Equals(exposureSummary2));
        }

        [Test]
        public void EaualsTest2()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            exposureSummary2.AttenuationDurationsInMillis = new int[]
            {
                1800000,
                1560000,
                0,
            };
            Assert.True(exposureSummary1.Equals(exposureSummary2));
        }

        [Test]
        public void EaualsTest3()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            exposureSummary2.AttenuationDurationsInMillis = new int[]
            {
                1800000,
                0,
                0,
            };
            Assert.False(exposureSummary1.Equals(exposureSummary2));
        }

        [Test]
        public void EaualsTest4()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            exposureSummary2.DaysSinceLastExposure = 1;
            Assert.False(exposureSummary1.Equals(exposureSummary2));
        }

        [Test]
        public void EaualsTest5()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            exposureSummary2.MaximumRiskScore = 250;
            Assert.False(exposureSummary1.Equals(exposureSummary2));
        }

        [Test]
        public void EaualsTest6()
        {
            var exposureSummary1 = CreateExposureSummary();
            var exposureSummary2 = CreateExposureSummary();

            exposureSummary2.SummationRiskScore = 10000;
            Assert.False(exposureSummary1.Equals(exposureSummary2));
        }
    }
}
