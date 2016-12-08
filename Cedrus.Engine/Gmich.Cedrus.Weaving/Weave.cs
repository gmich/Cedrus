using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gmich.Cedrus.Weaving
{
    internal class Weave
    {

        private static Configuration LoadConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileName = "weaving";
            var path = new Uri(assembly.CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(path);
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = directoryName + $"\\{fileName}.config";
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        static int Main(string[] args)
        {
            try
            {
                var assemblyPrefix = LoadConfiguration().AppSettings.Settings["AssemblyPrefix"].Value;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var weavers = Directory.GetFiles(path, "*.dll")
                    .Where(file =>
                        file.StartsWith(assemblyPrefix))
                    .Select(assemblyPath =>
                        new ILCodeWeaver(assemblyPath)).ToList();
            }
            catch (Exception ex)
            {
                File.WriteAllText("Gmich.Cedrus.Weaving.log", $"Weaving failure. {ex.ToString()}");
                return 1;
            }
            return 0;
        }
    }
}
