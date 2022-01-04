using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Chino.Common.Tests
{
    public class ScanInstanceTest
    {
        private ScanInstance CreateScanInstance()
            => new ScanInstance() {
                MinAttenuationDb = 45,
                SecondsSinceLastScan = 300,
                TypicalAttenuationDb = 37,
            };

        [Test]
        public void EaualsTest1()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            Assert.AreEqual(scanInstance1.GetHashCode(), scanInstance2.GetHashCode());
            Assert.True(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest2()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.MinAttenuationDb = 50;
            Assert.AreNotEqual(scanInstance1.GetHashCode(), scanInstance2.GetHashCode());
            Assert.False(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest3()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.SecondsSinceLastScan = 250;
            Assert.AreNotEqual(scanInstance1.GetHashCode(), scanInstance2.GetHashCode());
            Assert.False(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest4()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.TypicalAttenuationDb = 50;
            Assert.AreNotEqual(scanInstance1.GetHashCode(), scanInstance2.GetHashCode());
            Assert.False(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void UnionTest1()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();
            scanInstance2.MinAttenuationDb = 40;

            var scanInstance3 = CreateScanInstance();
            var scanInstance4 = CreateScanInstance();
            scanInstance4.MinAttenuationDb = 10;

            var list1 = new List<ScanInstance>() {
                scanInstance1,
                scanInstance2
            };
            var list2 = new List<ScanInstance>() {
                scanInstance3,
                scanInstance4
            };

            var unionList = list1.Union(list2);
            Assert.AreEqual(3, unionList.Count());
            Assert.True(unionList.Contains(scanInstance2));
            Assert.True(unionList.Contains(scanInstance4));

            Assert.True(unionList.Contains(scanInstance1));
            Assert.True(unionList.Contains(scanInstance3));
        }

        [Test]
        public void SortTest1()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();
            scanInstance2.MinAttenuationDb = 10;
            var scanInstance3 = CreateScanInstance();
            scanInstance3.SecondsSinceLastScan = 40;
            var scanInstance4 = CreateScanInstance();
            scanInstance4.TypicalAttenuationDb = -5;
            scanInstance4.MinAttenuationDb = 5;

            var list = new List<ScanInstance>() {
                scanInstance1,
                scanInstance2,
                scanInstance3,
                scanInstance4
            };

            list.Sort(new ScanInstance.Comparer());
            Assert.AreEqual(4, list.Count());

            Assert.AreEqual(scanInstance1, list[0]);
            Assert.AreEqual(scanInstance3, list[1]);
            Assert.AreEqual(scanInstance2, list[2]);
            Assert.AreEqual(scanInstance4, list[3]);
        }
    }
}
