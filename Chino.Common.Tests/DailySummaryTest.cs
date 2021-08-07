using System.Collections.Generic;
using NUnit.Framework;

namespace Chino.Common.Tests
{
    public class DailySummaryTest
    {
        private readonly string PATH_SINGLE_JSON = "./files/daily_summary.json";
        private readonly string PATH_LIST_JSON = "./files/daily_summaries.json";

        [Test]
        public void TestDeserializeFromJson1()
        {
            var dailySummary = Utils.ReadObjectFromJsonPath<DailySummary>(PATH_SINGLE_JSON);
            Assert.IsNotNull(dailySummary);

            Assert.AreEqual(-1226205184, dailySummary.DateMillisSinceEpoch);

            {
                var ds = dailySummary.DaySummary;
                Assert.AreEqual(2040.0, ds.MaximumScore);
                Assert.AreEqual(121020.0, ds.ScoreSum);
                Assert.AreEqual(121020.0, ds.WeightedDurationSum);
            }
            {
                var ds = dailySummary.ConfirmedClinicalDiagnosisSummary;
                Assert.AreEqual(0.0, ds.MaximumScore);
                Assert.AreEqual(0.0, ds.ScoreSum);
                Assert.AreEqual(0.0, ds.WeightedDurationSum);
            }
            {
                var ds = dailySummary.ConfirmedTestSummary;
                Assert.AreEqual(2040.0, ds.MaximumScore);
                Assert.AreEqual(121020.0, ds.ScoreSum);
                Assert.AreEqual(121020.0, ds.WeightedDurationSum);
            }
            {
                var ds = dailySummary.RecursiveSummary;
                Assert.AreEqual(0.0, ds.MaximumScore);
                Assert.AreEqual(0.0, ds.ScoreSum);
                Assert.AreEqual(0.0, ds.WeightedDurationSum);
            }
            {
                var ds = dailySummary.SelfReportedSummary;
                Assert.AreEqual(0.0, ds.MaximumScore);
                Assert.AreEqual(0.0, ds.ScoreSum);
                Assert.AreEqual(0.0, ds.WeightedDurationSum);
            }
        }

        [Test]
        public void TestDeserializeFromJson2()
        {
            var dailySummaries = Utils.ReadObjectFromJsonPath<List<DailySummary>>(PATH_LIST_JSON);
            Assert.AreEqual(5, dailySummaries.Count);
        }
    }
}
