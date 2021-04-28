using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class DailySummariesConfigTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/daily_summaries_config.json");

        private static DailySummariesConfig ReadDailySummariesConfig(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<DailySummariesConfig>(jsonStr);
            }
        }

        [Fact]
        public void TestSerializeToJson()
        {
            var dailySummariesConfig = new DailySummariesConfig();
            var jsonStr = JsonConvert.SerializeObject(dailySummariesConfig, Formatting.Indented);
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
            var expected = new DailySummariesConfig();
            var dailySummariesConfig = ReadDailySummariesConfig(PATH_JSON_SERIALIZED1);

            Assert.True(expected.Equals(dailySummariesConfig));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new DailySummariesConfig();
            expected.DaysSinceExposureThreshold = 1;

            var dailySummariesConfig = ReadDailySummariesConfig(PATH_JSON_SERIALIZED1);

            Assert.False(expected.Equals(dailySummariesConfig));
        }
    }
}
