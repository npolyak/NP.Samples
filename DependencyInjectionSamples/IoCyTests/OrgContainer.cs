using NP.DependencyInjection.Attributes;
using NP.Samples.Implementations;
using NP.Samples.Interfaces;

namespace NP.Samples.IoCyTests
{

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
        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IOrg CreateSingleOrg()
        {
            return new MyOrg { OrgName = "MyOrg2" };
        }

        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IEnumerable<IOrg> CreateOrgs()
        {
            return new IOrg[]
            {
                new MyOrg { OrgName = "MyOrg3" },
                new MyOrg { OrgName = "MyOrg4" }
            };
        }
    }

    public class OrgsContainer
    {
        public IEnumerable<IOrg> Orgs { get; }

        [CompositeConstructor]
        public OrgsContainer([Inject(resolutionKey: "TheOrgs")] IEnumerable<IOrg> orgs)
        {
            Orgs = orgs;
        }
    }
}
