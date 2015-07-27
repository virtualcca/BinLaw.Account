using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BinLaw.Account.FE.Foundation.Helper
{
    public sealed class ResHelper
    {
        public static async Task<string> ReadResourceAsync(Assembly assembly, string resourceFileName)
        {
            string content;
            using (var stream = assembly.GetManifestResourceStream(resourceFileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    content = await reader.ReadToEndAsync();
                }
            }
            return content;
        }

        public static async Task<string> ReadResourceAsync(string path)
        {
            string[] names = path.Split(';');

            var assembly = Assembly.Load(new AssemblyName(names[0]));
            var fileName = names[1];

            return await ReadResourceAsync(assembly, fileName);
        }

        //public static IAsyncOperation GetConfiguration(string fileName)
        //{
        //    return (ReadResourceAsync(typeof(ResHelper).GetTypeInfo().Assembly, fileName)).AsAsyncOperation();
        //}

        public static Stream ReadResourceStreamAsync(string path)
        {
            string[] names = path.Split(';');

            var assembly = Assembly.Load(new AssemblyName(names[0]));
            var fileName = names[1];

            return assembly.GetManifestResourceStream(fileName);
        }
    }
}
