using NP.DependencyInjection.Attributes;
using NP.PackagePluginsTest.PluginInterfaces;
using System.Text;

namespace NP.PackagePluginsTest.StringManipulationsPlugin
{
    [RegisterType]
    public class StringManipulationsPlugin : IStringManipulationsPlugin
    {
        // concatinates two strings
        public string Concat(string str1, string str2)
        {
            return str1 + str2;
        }

        // repeats string str numberTimesToRepeat times. 
        public string Repeat(string str, int numberTimesToRepeat)
        {
            StringBuilder sb = new StringBuilder(str.Length * numberTimesToRepeat);

            for(int i = 0; i < numberTimesToRepeat; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }
    }
}