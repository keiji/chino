using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chino.Tests
{
    public class GoogleDiagnosisKeysDataMappingConfigurationTest
    {
        private readonly string PATH_JSON = "./files/google_diagnosis_keys_data_mapping_configuration.json";

        [Test]
        public void TestSerializeToJson()
        {
            var googleDiagnosisKeysDataMappingConfiguration = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var jsonStr = JsonConvert.SerializeObject(googleDiagnosisKeysDataMappingConfiguration, Formatting.Indented);
            //Logger.D(jsonStr);

            using (var sr = new StreamReader(File.OpenRead(Utils.GetFullPath(PATH_JSON))))
            {
                var expected = sr.ReadToEnd();

                Assert.AreEqual(expected, jsonStr);
            }
        }

        [Test]
        public void TestDeserializeFromJson()
        {
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration();
            var googleDiagnosisKeysDataMappingConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);

            Assert.True(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }

        [Test]
        public void TestNotEquals()
        {
            var expected = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration
            {
                InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.None
            };

            var googleDiagnosisKeysDataMappingConfiguration = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);

            Assert.False(expected.Equals(googleDiagnosisKeysDataMappingConfiguration));
        }

        [Test]
        public void TestNotEquals2()
        {
            var googleDiagnosisKeysDataMappingConfiguration1 = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);
            var googleDiagnosisKeysDataMappingConfiguration2 = Utils
                .ReadObjectFromJsonPath<ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration>(PATH_JSON);

            googleDiagnosisKeysDataMappingConfiguration2.InfectiousnessForDaysSinceOnsetOfSymptoms[0] = Infectiousness.None;

            Assert.False(googleDiagnosisKeysDataMappingConfiguration1.Equals(googleDiagnosisKeysDataMappingConfiguration2));
        }
    }
}
