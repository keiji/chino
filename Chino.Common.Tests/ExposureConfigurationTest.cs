using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class ExposureConfigurationTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/exposure_configuration.json");

        private static ExposureConfiguration ReadExposureConfiguration(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<ExposureConfiguration>(jsonStr);
            }
        }

        [Fact]
        public void TestSerializeToJson()
        {
            var exposureConfiguration = new ExposureConfiguration();
            var jsonStr = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
            //Logger.D(jsonStr);

            using (var sr = new StreamReader(File.OpenRead(PATH_JSON_SERIALIZED1)))
            {
                var expected = sr.ReadToEnd();

                Assert.Equal(expected, jsonStr);
            }
        }

        [Fact]
        public void TestDeserializeFromJson()
        {
            var expected = new ExposureConfiguration();
            var exposureConfiguration = ReadExposureConfiguration(PATH_JSON_SERIALIZED1);

            Assert.True(expected.Equals(exposureConfiguration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration();
            expected.GoogleExposureConfig.MinimumRiskScore = 1;

            var exposureConfiguration = ReadExposureConfiguration(PATH_JSON_SERIALIZED1);

            Assert.False(expected.Equals(exposureConfiguration));
        }
    }
}
