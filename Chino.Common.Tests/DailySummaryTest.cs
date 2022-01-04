using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Chino.Tests
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

        private DailySummary CreateDailySummary()
            => new DailySummary()
            {
                DateMillisSinceEpoch = 123456,
            };

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
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            Assert.True(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest2()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries2.DateMillisSinceEpoch = 500;
            Assert.False(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest3()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.ConfirmedClinicalDiagnosisSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedClinicalDiagnosisSummary = CreateExposureSummaryData();
            Assert.True(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest4()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.ConfirmedClinicalDiagnosisSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedClinicalDiagnosisSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedClinicalDiagnosisSummary.MaximumScore = 60;
            Assert.False(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest5()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.ConfirmedTestSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedTestSummary = CreateExposureSummaryData();
            Assert.True(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest6()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.ConfirmedTestSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedTestSummary = CreateExposureSummaryData();
            dailySummaries2.ConfirmedTestSummary.MaximumScore = 60;
            Assert.False(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest7()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.RecursiveSummary = CreateExposureSummaryData();
            dailySummaries2.RecursiveSummary = CreateExposureSummaryData();
            Assert.True(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest8()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.RecursiveSummary = CreateExposureSummaryData();
            dailySummaries2.RecursiveSummary = CreateExposureSummaryData();
            dailySummaries2.RecursiveSummary.MaximumScore = 60;
            Assert.False(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest9()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.SelfReportedSummary = CreateExposureSummaryData();
            dailySummaries2.SelfReportedSummary = CreateExposureSummaryData();
            Assert.True(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void EaualsTest10()
        {
            var dailySummaries1 = CreateDailySummary();
            var dailySummaries2 = CreateDailySummary();

            dailySummaries1.SelfReportedSummary = CreateExposureSummaryData();
            dailySummaries2.SelfReportedSummary = CreateExposureSummaryData();
            dailySummaries2.SelfReportedSummary.MaximumScore = 60;
            Assert.False(dailySummaries1.Equals(dailySummaries2));
        }

        [Test]
        public void UnionTest1()
        {
            var dailySummary1 = CreateDailySummary();
            var dailySummary2 = CreateDailySummary();
            dailySummary2.DateMillisSinceEpoch = 40;

            var dailySummary3 = CreateDailySummary();
            var dailySummary4 = CreateDailySummary();
            dailySummary4.DateMillisSinceEpoch = 100;

            var list1 = new List<DailySummary>() {
                dailySummary1,
                dailySummary2
            };
            var list2 = new List<DailySummary>() {
                dailySummary3,
                dailySummary4
            };

            var unionList = list1.Union(list2);
            Assert.AreEqual(3, unionList.Count());
            Assert.True(unionList.Contains(dailySummary2));
            Assert.True(unionList.Contains(dailySummary4));

            Assert.True(unionList.Contains(dailySummary1));
            Assert.True(unionList.Contains(dailySummary3));
        }

        [Test]
        public void SortTest1()
        {
            var dailySummary1 = CreateDailySummary();
            dailySummary1.DateMillisSinceEpoch = 10;
            var dailySummary2 = CreateDailySummary();
            dailySummary2.DateMillisSinceEpoch = 5;
            var dailySummary3 = CreateDailySummary();
            dailySummary3.DateMillisSinceEpoch = 40;
            var dailySummary4 = CreateDailySummary();
            dailySummary4.DateMillisSinceEpoch = 100;

            var list = new List<DailySummary>() {
                dailySummary1,
                dailySummary2,
                dailySummary3,
                dailySummary4
            };

            list.Sort(new DailySummary.Comparer());
            Assert.AreEqual(4, list.Count());

            Assert.AreEqual(dailySummary2, list[0]);
            Assert.AreEqual(dailySummary1, list[1]);
            Assert.AreEqual(dailySummary3, list[2]);
            Assert.AreEqual(dailySummary4, list[3]);
        }

        [Test]
        public void SortTest2()
        {
            var dailySummary1 = CreateDailySummary();
            dailySummary1.DaySummary = CreateExposureSummaryData();
            dailySummary1.DaySummary.MaximumScore = 100;

            var dailySummary2 = CreateDailySummary();
            dailySummary1.DaySummary = CreateExposureSummaryData();
            dailySummary1.DaySummary.MaximumScore = 110;

            var list = new List<DailySummary>() {
                dailySummary1,
                dailySummary2,
            };

            list.Sort(new DailySummary.Comparer());
            Assert.AreEqual(2, list.Count());

            Assert.AreEqual(dailySummary1, list[0]);
            Assert.AreEqual(dailySummary2, list[1]);
        }

        [Test]
        public void SortTest3()
        {
            var dailySummary1 = CreateDailySummary();
            dailySummary1.DaySummary = CreateExposureSummaryData();
            dailySummary1.DaySummary.MaximumScore = 100;

            var dailySummary2 = CreateDailySummary();
            dailySummary1.ConfirmedClinicalDiagnosisSummary = CreateExposureSummaryData();
            dailySummary1.ConfirmedClinicalDiagnosisSummary.MaximumScore = 100;

            var list = new List<DailySummary>() {
                dailySummary1,
                dailySummary2,
            };

            list.Sort(new DailySummary.Comparer());
            Assert.AreEqual(2, list.Count());

            Assert.AreEqual(dailySummary1, list[0]);
            Assert.AreEqual(dailySummary2, list[1]);
        }
    }
}
