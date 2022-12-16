using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;

namespace NP.Samples.Implementations
{
    [HasRegisterMethods]
    public static class FactoryMethods
    {
        public static IAddress CreateAddress()
        {
            return new Address();
        }
    }
}
