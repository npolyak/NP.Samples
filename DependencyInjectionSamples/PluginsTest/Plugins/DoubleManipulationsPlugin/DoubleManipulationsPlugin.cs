using NP.DependencyInjection.Attributes;
using PluginInterfaces;

namespace DoubleManipulationsPlugin
{
    [RegisterType]
    public class DoubleManipulationsPlugin : IDoubleManipulationsPlugin
    {
        public double Plus(double number1, double number2)
        {
            return number1 + number2;
        }

        public double Times(double number1, double number2)
        {
            return number1 * number2;
        }
    }
}