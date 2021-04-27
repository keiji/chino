using System.IO;

namespace Chino.Common.Tests
{
    public static class Utils
    {
        // https://sau001.wordpress.com/2019/02/24/net-core-unit-tests-how-to-deploy-files-without-using-deploymentitem/
        public static string GetCurrentProjectPath()
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
