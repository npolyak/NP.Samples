using NP.DependencyInjection.Attributes;
using NP.Samples.Implementations;
using NP.Samples.Interfaces;

namespace NP.Samples.IoCyTests
{

    // using this attribute, an object of type MyOrg with OrgName set to "MyOrg1",
    // will be created and made part of the Multi-Cell
    // defined by CellType - IOrg and resolutionKey "TheOrgs"
    [RegisterMultiCellType(cellType: typeof(IOrg), "TheOrgs")]
    public class MyOrg : Org
    {
        public MyOrg()
        {
            OrgName = "MyOrg1";
        }
    }

    [HasRegisterMethods]
    public static class OrgFactory
    {
        // returns a single Org object with OrgName set to "MyOrg2"
        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IOrg CreateSingleOrg()
        {
            return new Org { OrgName = "MyOrg2" };
        }

        // returns an array of two objects with OrgNames 
        // "MyOrg3" and "MyOrg4" correspondingly
        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IEnumerable<IOrg> CreateOrgs()
        {
            return new IOrg[]
            {
                new Org { OrgName = "MyOrg3" },
                new Org { OrgName = "MyOrg4" }
            };
        }
    }

    public class OrgsContainer
    {
        public IEnumerable<IOrg> Orgs { get; }

        // injects the constructor with orgs argument of resolving type IEnumerable<IOrg>
        // and resolutionKey - "TheOrgs" that point us to the MultiCell created above. 
        [CompositeConstructor]
        public OrgsContainer([Inject(resolutionKey: "TheOrgs")] IEnumerable<IOrg> orgs)
        {
            Orgs = orgs;
        }
    }
}
