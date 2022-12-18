using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;
using System.Diagnostics;

namespace NP.Samples.Implementations
{
    [HasRegisterMethods]
    public static class FactoryMethods
    {
        [RegisterMethod(resolutionKey: "TheAddress")]
        public static IAddress CreateAddress()
        {
            return new Address { City = "Providence" };
        }

        [RegisterMethod(isSingleton: true, resolutionKey: "TheManager")]
        public static IPerson CreateManager([Inject(resolutionKey: "TheAddress")] IAddress address)
        {
            return new Person { PersonName = "Joe Doe", Address = address };
        }

        [RegisterMethod(resolutionKey: "TheOrg")]
        public static IOrg CreateOrg([Inject(resolutionKey: "TheManager")] IPerson person)
        {
            return new Org { OrgName = "Other Department Store", Manager = person };
        }
    }
}
