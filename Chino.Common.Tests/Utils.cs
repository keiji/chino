using System.IO;
using Newtonsoft.Json;

namespace Chino.Tests
{
    public static class Utils
    {
        internal static T ReadObjectFromJsonPath<T>(string path)
        {
            string fullPath = GetFullPath(path);
            using (var sr = new StreamReader(fullPath))
            {
                var jsonStr = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
        }

        // https://sau001.wordpress.com/2019/02/24/net-core-unit-tests-how-to-deploy-files-without-using-deploymentitem/
        internal static string GetFullPath(string path) => Path.Combine(GetCurrentProjectPath(), path);

        private static string GetCurrentProjectPath()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);
            if (!folderAssembly.EndsWith("/"))
            {
                folderAssembly += "/";
            }
            return Path.GetFullPath(folderAssembly + "../../../");
        }
    }
}
