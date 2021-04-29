using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Common.Tests
{
    public class AppleExposureV1ConfigurationTest
    {
        private readonly string PATH_JSON = "./files/apple_exposure_v1_configuration.json";

        [Test]
        public void TestSerializeToJson()
        {
            var appleExposureV1Configuration = new ExposureConfiguration.AppleExposureV1Configuration();
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
            var expected = new ExposureConfiguration.AppleExposureV1Configuration();
            var appleExposureV2Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureV1Configuration>(PATH_JSON);

            Assert.True(expected.Equals(appleExposureV2Configuration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.AppleExposureV1Configuration
            {
                MinimumRiskScore = 1
            };

            var appleExposureV2Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureV1Configuration>(PATH_JSON);

            Assert.False(expected.Equals(appleExposureV2Configuration));
        }
    }
}
