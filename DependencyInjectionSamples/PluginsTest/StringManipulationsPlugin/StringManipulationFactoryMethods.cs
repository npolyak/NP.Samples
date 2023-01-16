using NP.DependencyInjection.Attributes;
using PluginInterfaces;

namespace DoubleManipulationsPlugin
{
    [HasRegisterMethods]
    public static class StringManipulationFactoryMethods
    {

        [RegisterMultiCellMethod(cellType:typeof(string), resolutionKey:"MethodNames")]
        public static IEnumerable<string> GetDoubleMethodNames()
        {
            return new[]
            {
                nameof(IStringManipulationsPlugin.Concat),
                nameof(IStringManipulationsPlugin.Repeat)
            };
        }
    }
}
