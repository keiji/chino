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

            Assert.True(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest2()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.MinAttenuationDb = 50;
            Assert.False(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest3()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.SecondsSinceLastScan = 250;
            Assert.False(scanInstance1.Equals(scanInstance2));
        }

        [Test]
        public void EaualsTest4()
        {
            var scanInstance1 = CreateScanInstance();
            var scanInstance2 = CreateScanInstance();

            scanInstance2.TypicalAttenuationDb = 50;
            Assert.False(scanInstance1.Equals(scanInstance2));
        }
    }
}
