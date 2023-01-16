using NP.DependencyInjection.Attributes;
using NP.PackagePluginsTest.PluginInterfaces;

namespace NP.PackagePluginsTest.StringManipulationsPlugin
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
