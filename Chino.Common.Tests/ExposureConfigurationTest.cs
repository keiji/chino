using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class ExposureConfigurationTest
    {
        private readonly string PATH_JSON = "./files/exposure_configuration.json";

        [Fact]
        public void TestSerializeToJson()
        {
            var exposureConfiguration = new ExposureConfiguration();
            var jsonStr = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
            //Logger.D(jsonStr);

            using (var sr = new StreamReader(File.OpenRead(Utils.GetFullPath(PATH_JSON))))
            {
                var expected = sr.ReadToEnd();

                Assert.Equal(expected, jsonStr);
            }
        }

        [Fact]
        public void TestDeserializeFromJson()
        {
            var expected = new ExposureConfiguration();
            var exposureConfiguration = Utils.ReadObjectFromJsonPath<ExposureConfiguration>(PATH_JSON);

            Assert.True(expected.Equals(exposureConfiguration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration();
            expected.GoogleExposureConfig.MinimumRiskScore = 1;

            var exposureConfiguration = Utils.ReadObjectFromJsonPath<ExposureConfiguration>(PATH_JSON);

            Assert.False(expected.Equals(exposureConfiguration));
        }
    }
}
