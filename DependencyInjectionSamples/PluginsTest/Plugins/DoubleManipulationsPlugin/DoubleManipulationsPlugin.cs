using NP.DependencyInjection.Attributes;
using PluginInterfaces;

namespace DoubleManipulationsPlugin
{
    [RegisterType]
    public class DoubleManipulationsPlugin : IDoubleManipulationsPlugin
    {
        // sums two numbers
        public double Plus(double number1, double number2)
        {
            return number1 + number2;
        }

        // multiplies two numbers
        public double Times(double number1, double number2)
        {
            return number1 * number2;
        }
    }
}