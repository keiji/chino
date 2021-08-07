using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Tests
{

    public class ExposureConfigurationTest
    {
        private readonly string PATH_JSON = "./files/exposure_configuration.json";

        [Test]
        public void TestSerializeToJson()
        {
            var exposureConfiguration = new ExposureConfiguration();
            var jsonStr = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
            //Logger.D(jsonStr);

            using (var sr = new StreamReader(File.OpenRead(Utils.GetFullPath(PATH_JSON))))
            {
                var expected = sr.ReadToEnd();

                Assert.AreEqual(expected, jsonStr);
            }
        }

        [Test]
        public void TestDeserializeFromJson()
        {
            var expected = new ExposureConfiguration();
            var exposureConfiguration = Utils.ReadObjectFromJsonPath<ExposureConfiguration>(PATH_JSON);

            Assert.True(expected.Equals(exposureConfiguration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration();
            expected.GoogleExposureConfig.MinimumRiskScore = 1;

            var exposureConfiguration = Utils.ReadObjectFromJsonPath<ExposureConfiguration>(PATH_JSON);

            Assert.False(expected.Equals(exposureConfiguration));
        }
    }
}
