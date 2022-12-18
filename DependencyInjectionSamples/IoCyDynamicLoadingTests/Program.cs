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
        IContainerBuilder builder = new ContainerBuilder();

        builder.RegisterPluginsFromSubFolders("Plugins");

        // create container
        IDependencyInjectionContainer container = builder.Build();

        // resolve and compose organization
        // all its injectable properties will be injected at
        // this stage. 
        IOrgGettersOnly orgWithGettersOnly = container.Resolve<IOrgGettersOnly>("MyOrg");

        // set values
        orgWithGettersOnly.OrgName = "Nicks Department Store";
        orgWithGettersOnly.Manager.PersonName = "Nick Polyak";
        orgWithGettersOnly.Manager.Address!.City = "Miami";
        orgWithGettersOnly.Manager.Address.ZipCode = "12245";

        // print to console.
        orgWithGettersOnly.LogOrgInfo();

        IOrg org = container.Resolve<IOrg>("TheOrg");

        // test that the properties values
        org.OrgName.Should().Be("Other Department Store");
        org.Manager.PersonName.Should().Be("Joe Doe");
        org.Manager.Address.City.Should().Be("Providence");

        IOrg anotherOrg = container.Resolve<IOrg>("TheOrg");

        // not a singleton
        org.Should().NotBeSameAs(anotherOrg);

        // singleton
        org.Manager.Should().BeSameAs(anotherOrg.Manager);


        IAddress address2 = container.Resolve<IAddress>("TheAddress");

        // not a singleton
        address2.Should().NotBeSameAs(org.Manager.Address);

        Console.WriteLine("The END");
    }
}