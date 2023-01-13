using NP.DependencyInjection.Attributes;
using PluginInterfaces;

namespace DoubleManipulationsPlugin
{
    [HasRegisterMethods]
    public static class StringManipulationFactoryMethods
    {

        [RegisterMethod(isSingleton:true, isMultiCell:true, resolutionKey:"MethodNames")]
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
