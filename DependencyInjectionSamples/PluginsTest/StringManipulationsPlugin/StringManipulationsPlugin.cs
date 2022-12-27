using NP.DependencyInjection.Attributes;
using PluginInterfaces;
using System.Text;

namespace StringManipulationsPlugin
{
    [RegisterType]
    public class StringManipulationsPlugin : IStringManipulationsPlugin
    {
        public string Concat(string str1, string str2)
        {
            return str1 + str2;
        }

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