using NP.Samples.Interfaces;
using NP.DependencyInjection.Interfaces;
using NP.IoCy;
using FluentAssertions;

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
        org1.Manager.Address.ZipCode = "12245";

        // print to console.
        org1.LogOrgInfo();


        IContainerBuilder builder2 = new ContainerBuilder();
        builder2.RegisterPluginsFromSubFolders("Plugins");

        IDependencyInjectionContainer container2 = builder2.Build();

        IOrg org2 = container2.Resolve<IOrg>("TheOrg");

        org2.OrgName.Should().Be("Other Department Store");
        org2.Manager.PersonName.Should().Be("Joe Doe");
        org2.Manager.Address.City.Should().Be("Providence");

        IOrg anotherOrg2 = container2.Resolve<IOrg>("TheOrg");

        org2.Should().NotBeSameAs(anotherOrg2);
        org2.Manager.Should().BeSameAs(anotherOrg2.Manager);

        IAddress address2 = container2.Resolve<IAddress>("TheAddress");

        address2.Should().NotBeSameAs(org2.Manager.Address);

        Console.WriteLine("The END");
    }
}