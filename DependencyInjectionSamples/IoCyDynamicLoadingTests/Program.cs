using NP.Samples.Interfaces;
using NP.DependencyInjection.Interfaces;
using NP.IoCy;

namespace NP.Samples.IoCyDynamicLoadingTests;

public static class Program
{
    static void Main(string[] args)
    {
        // create container builder
        IContainerBuilder builder1 = new ContainerBuilder();

        builder1.RegisterPluginsFromSubFolders("Plugins");


        // create container
        IDependencyInjectionContainer container1 = builder1.Build();

        // resolve and compose organization
        // all its injectable properties will be injected at
        // this stage. 
        IOrgGettersOnly org1 = container1.Resolve<IOrgGettersOnly>("MyOrg");

        // set values
        org1.OrgName = "Nicks Department Store";
        org1.Manager.PersonName = "Nick Polyak";
        org1.Manager.Address!.City = "Miami";
        org1.Manager.Address.ZipCode = "12345";

        // print to console.
        org1.LogOrgInfo();


        IContainerBuilder builder2 = new ContainerBuilder();
        builder2.RegisterPluginsFromSubFolders("Plugins");
        IDependencyInjectionContainer container2 = builder2.Build();

        // resolves from FactoryMethods.CreateOrg
        IOrg org2 = container2.Resolve<IOrg>();

        org2.Manager!.Address!.City = "Boston";
        org2.Manager.Address.ZipCode = "09875";

        org2.LogOrgInfo();
    }
}