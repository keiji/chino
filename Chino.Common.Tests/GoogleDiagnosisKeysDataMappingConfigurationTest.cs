using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{
    public class GoogleDiagnosisKeysDataMappingConfigurationTest
    {
        private readonly string PATH_JSON = "./files/google_diagnosis_keys_data_mapping_configuration.json";

        [Fact]
        public void TestSerializeToJson()
        {
            var googleDiagnosisKeysDataMappingConfiguration = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var jsonStr = JsonConvert.SerializeObject(googleDiagnosisKeysDataMappingConfiguration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var googleDiagnosisKeysDataMappingConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);

            Assert.True(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            expected.InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.None;

            var googleDiagnosisKeysDataMappingConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);

            Assert.False(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }
    }
}
