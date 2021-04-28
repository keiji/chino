using System.IO;
using Newtonsoft.Json;
using Xunit;

namespace Chino.Common.Tests
{
    public class GoogleDiagnosisKeysDataMappingConfigurationTest
    {
        private readonly string PATH_JSON_SERIALIZED1 = Path.Combine(Utils.GetCurrentProjectPath(), "./files/google_diagnosis_keys_data_mapping_configuration.json");

        private static ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration ReadGoogleDiagnosisKeysDataMappingConfiguration(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(jsonStr);
            }
        }

        [Fact]
        public void TestSerializeToJson()
        {
            var googleDiagnosisKeysDataMappingConfiguration = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var jsonStr = JsonConvert.SerializeObject(googleDiagnosisKeysDataMappingConfiguration, Formatting.Indented);
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
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var googleDiagnosisKeysDataMappingConfiguration = ReadGoogleDiagnosisKeysDataMappingConfiguration(PATH_JSON_SERIALIZED1);

            Assert.True(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }

        [Fact]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            expected.InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.None;

            var googleDiagnosisKeysDataMappingConfiguration = ReadGoogleDiagnosisKeysDataMappingConfiguration(PATH_JSON_SERIALIZED1);

            Assert.False(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }
    }
}
