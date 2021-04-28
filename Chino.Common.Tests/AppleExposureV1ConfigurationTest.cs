using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{
    public class AppleExposureV2ConfigurationTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/apple_exposure_v2_configuration.json");

        private static ExposureConfiguration.AppleExposureV2Configuration ReadAppleExposureV2Configuration(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<ExposureConfiguration.AppleExposureV2Configuration>(jsonStr);
            }
        }

        [Fact]
        public void TestSerializeToJson()
        {
            var appleExposureV2Configuration = new ExposureConfiguration.AppleExposureV2Configuration();
            var jsonStr = JsonConvert.SerializeObject(appleExposureV2Configuration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.AppleExposureV2Configuration();
            var appleExposureV2Configuration = ReadAppleExposureV2Configuration(PATH_JSON_SERIALIZED1);

            Assert.True(expected.Equals(appleExposureV2Configuration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.AppleExposureV2Configuration();
            expected.ImmediateDurationWeight = 250;

            var appleExposureV2Configuration = ReadAppleExposureV2Configuration(PATH_JSON_SERIALIZED1);

            Assert.False(expected.Equals(appleExposureV2Configuration));
        }
    }
}
