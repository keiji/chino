using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class ExposureConfigurationTest
    {
        [Fact]
        public void TestSerializeToJson()
        {
            using (var sr = new StreamReader(File.OpenRead(Path.Combine(Utils.GetCurrentProjectPath(), "./files/serialized1.json"))))
            {
                var expected = sr.ReadToEnd();

                var exposureConfiguration = new ExposureConfiguration();
                var jsonStr = JsonConvert.SerializeObject(exposureConfiguration, Formatting.Indented);
                Logger.D(jsonStr);

                Assert.Equal(expected, jsonStr);
            }
        }
    }
}
