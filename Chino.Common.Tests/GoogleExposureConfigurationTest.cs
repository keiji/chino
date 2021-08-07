using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Tests
{
    public class GoogleExposureConfigurationTest
    {
        private readonly string PATH_JSON = "./files/google_exposure_configuration.json";

        [Test]
        public void TestSerializeToJson()
        {
            var googleExposureConfiguration = new ExposureConfiguration.GoogleExposureConfiguration();
            var jsonStr = JsonConvert.SerializeObject(googleExposureConfiguration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.GoogleExposureConfiguration();
            var googleExposureConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleExposureConfiguration>(PATH_JSON);

            Assert.True(expected.Equals(googleExposureConfiguration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.GoogleExposureConfiguration
            {
                AttenuationWeight = 0
            };

            var googleExposureConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleExposureConfiguration>(PATH_JSON);

            Assert.False(expected.Equals(googleExposureConfiguration));
        }
    }
}
