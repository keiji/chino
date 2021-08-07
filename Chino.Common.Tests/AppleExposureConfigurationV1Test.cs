using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Tests
{
    public class AppleExposureV1ConfigurationTest
    {
        private readonly string PATH_JSON = "./files/apple_exposure_configuration_v1.json";

        [Test]
        public void TestSerializeToJson()
        {
            var appleExposureV1Configuration = new ExposureConfiguration.AppleExposureConfigurationV1();
            var jsonStr = JsonConvert.SerializeObject(appleExposureV1Configuration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.AppleExposureConfigurationV1();
            var appleExposureV1Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureConfigurationV1>(PATH_JSON);

            Assert.True(expected.Equals(appleExposureV1Configuration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.AppleExposureConfigurationV1
            {
                MinimumRiskScore = 1
            };

            var appleExposureV1Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureConfigurationV1>(PATH_JSON);

            Assert.False(expected.Equals(appleExposureV1Configuration));
        }
    }
}
