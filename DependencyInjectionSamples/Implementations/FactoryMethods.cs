using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;

namespace NP.Samples.Implementations
{
    [HasRegisterMethods]
    public static class FactoryMethods
    {
        [RegisterMethod(resolutionKey:"TheAddress", ResolvingType = typeof(IAddress))]
        public static IAddress CreateAddress()
        {
            return new Address();
        }
    }
}
