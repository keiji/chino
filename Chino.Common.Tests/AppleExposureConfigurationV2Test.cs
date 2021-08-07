using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Tests
{
    public class AppleExposureV2ConfigurationTest
    {
        private readonly string PATH_JSON = "./files/apple_exposure_configuration_v2.json";

        [Test]
        public void TestSerializeToJson()
        {
            var appleExposureConfigurationV2 = new ExposureConfiguration.AppleExposureConfigurationV2();
            var jsonStr = JsonConvert.SerializeObject(appleExposureConfigurationV2, Formatting.Indented);
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
            var expected = new ExposureConfiguration.AppleExposureConfigurationV2();
            var appleExposureV2Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureConfigurationV2>(PATH_JSON);

            Assert.True(expected.Equals(appleExposureV2Configuration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.AppleExposureConfigurationV2
            {
                ImmediateDurationWeight = 20
            };

            var appleExposureV2Configuration = Utils.ReadObjectFromJsonPath<ExposureConfiguration.AppleExposureConfigurationV2>(PATH_JSON);

            Assert.False(expected.Equals(appleExposureV2Configuration));
        }
    }
}
