using NP.DependencyInjection.Attributes;
using PluginInterfaces;

namespace DoubleManipulationsPlugin
{
    [HasRegisterMethods]
    public static class DoubleManipulationFactoryMethods
    {

        [RegisterMethod(isSingleton:true, isMultiCell:true, resolutionKey:"MethodNames")]
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
