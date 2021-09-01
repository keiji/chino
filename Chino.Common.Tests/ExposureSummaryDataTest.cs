using NUnit.Framework;

namespace Chino.Tests
{
    public class ExposureSummaryDataTest
    {
        private ExposureSummaryData CreateExposureSummaryData()
            => new ExposureSummaryData()
            {
                MaximumScore = 50,
                ScoreSum = 200,
                WeightedDurationSum = 45.0,
            };

        [Test]
        public void EaualsTest1()
        {
            var exposureSummaryData1 = CreateExposureSummaryData();
            var exposureSummaryData2 = CreateExposureSummaryData();

            Assert.True(exposureSummaryData1.Equals(exposureSummaryData2));
        }

        [Test]
        public void EaualsTest2()
        {
            var exposureSummaryData1 = CreateExposureSummaryData();
            var exposureSummaryData2 = CreateExposureSummaryData();

            exposureSummaryData2.MaximumScore = 80;
            Assert.False(exposureSummaryData1.Equals(exposureSummaryData2));
        }

        [Test]
        public void EaualsTest3()
        {
            var exposureSummaryData1 = CreateExposureSummaryData();
            var exposureSummaryData2 = CreateExposureSummaryData();

            exposureSummaryData2.ScoreSum = 400;
            Assert.False(exposureSummaryData1.Equals(exposureSummaryData2));
        }

        [Test]
        public void EaualsTest4()
        {
            var exposureSummaryData1 = CreateExposureSummaryData();
            var exposureSummaryData2 = CreateExposureSummaryData();

            exposureSummaryData2.WeightedDurationSum = 48.0;
            Assert.False(exposureSummaryData1.Equals(exposureSummaryData2));
        }
    }
}
