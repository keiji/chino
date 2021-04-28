using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class ExposureConfigurationTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/serialized1.json");

        [Fact]
        public void TestSerializeToJson()
        {
            using (var sr = new StreamReader(File.OpenRead(PATH_JSON_SERIALIZED1)))
            {
                var expected = sr.ReadToEnd();

                var exposureConfiguration = new ExposureConfiguration();
                var jsonStr = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
                Logger.D(jsonStr);

                Assert.Equal(expected, jsonStr);
            }
        }

        [Fact]
        public void TestDeserializeFromJson()
        {
            using (var sr = new StreamReader(File.OpenRead(PATH_JSON_SERIALIZED1)))
            {
                var expected = new ExposureConfiguration();

                var jsonStr = sr.ReadToEnd();
                var exposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(jsonStr);

                Assert.True(expected.Equals(exposureConfiguration));
            }
        }
    }
}
