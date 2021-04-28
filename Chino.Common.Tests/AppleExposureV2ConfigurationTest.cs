using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{
    public class AppleExposureV1ConfigurationTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/apple_exposure_v1_configuration.json");

        private static ExposureConfiguration.AppleExposureV1Configuration ReadAppleExposureV1Configuration1(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<ExposureConfiguration.AppleExposureV1Configuration>(jsonStr);
            }
        }

        [Fact]
        public void TestSerializeToJson()
        {
            var appleExposureV1Configuration = new ExposureConfiguration.AppleExposureV1Configuration();
            var jsonStr = JsonConvert.SerializeObject(appleExposureV1Configuration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.AppleExposureV1Configuration();
            var appleExposureV1Configuration = ReadAppleExposureV1Configuration1(PATH_JSON_SERIALIZED1);

            Assert.True(expected.Equals(appleExposureV1Configuration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.AppleExposureV1Configuration();
            expected.MinimumRiskScore = 1;

            var appleExposureV1Configuration = ReadAppleExposureV1Configuration1(PATH_JSON_SERIALIZED1);

            Assert.False(expected.Equals(appleExposureV1Configuration));
        }
    }
}
