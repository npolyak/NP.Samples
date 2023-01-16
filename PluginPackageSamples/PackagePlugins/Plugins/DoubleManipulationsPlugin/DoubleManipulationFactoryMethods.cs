using NP.DependencyInjection.Attributes;
using NP.PackagePluginsTest.PluginInterfaces;

namespace NP.PackagePluginsTest.DoubleManipulationsPlugin
{
    [HasRegisterMethods]
    public static class DoubleManipulationFactoryMethods
    {

        [RegisterMultiCellMethod(cellType:typeof(string), resolutionKey:"MethodNames")]
        public static IEnumerable<string> GetDoubleMethodNames()
        {
            return new[]
            {
                nameof(IDoubleManipulationsPlugin.Plus),
                nameof(IDoubleManipulationsPlugin.Times)
            };
        }
    }
}
