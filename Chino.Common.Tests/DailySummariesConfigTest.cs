using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{

    public class DailySummariesConfigTest
    {
        private readonly string PATH_JSON = "./files/daily_summaries_config.json";

        [Fact]
        public void TestSerializeToJson()
        {
            var dailySummariesConfig = new DailySummariesConfig();
            var jsonStr = JsonConvert.SerializeObject(dailySummariesConfig, Formatting.Indented);
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
            var expected = new DailySummariesConfig();
            var dailySummariesConfig = Utils.ReadObjectFromJsonPath<DailySummariesConfig>(PATH_JSON);

            Assert.True(expected.Equals(dailySummariesConfig));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new DailySummariesConfig();
            expected.DaysSinceExposureThreshold = 1;

            var dailySummariesConfig = Utils.ReadObjectFromJsonPath<DailySummariesConfig>(PATH_JSON);

            Assert.False(expected.Equals(dailySummariesConfig));
        }
    }
}
