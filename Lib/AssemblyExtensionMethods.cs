using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lib
{
    public static class AssemblyExtensionMethods
    {
        public static IEnumerable<string> GetEmbeddedResourceLines(this Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}
