using NP.Samples.Interfaces;
using NP.DependencyInjection.Interfaces;
using NP.DependencyInjection.AutofacAdapter;

namespace NP.Samples.IoCyDynamicLoadingTests;

public static class Program
{
    static void Main(string[] args)
    {
        // create container builder
        IContainerBuilder builder = new AutofacContainerBuilder();

        builder.RegisterPluginsFromSubFolders("Plugins");


        // create container
        IDependencyInjectionContainer container = builder.Build();

        // resolve and compose organization
        // all its injectable properties will be injected at
        // this stage. 
        IOrgGettersOnly org = container.Resolve<IOrgGettersOnly>("TheOrg");

        // set values
        org.OrgName = "Nicks Department Store";
        org.Manager.PersonName = "Nick Polyak";
        org.Manager.Address!.City = "Miami";
        org.Manager.Address.ZipCode = "12345";

        // print to console.
        org.LogOrgInfo();
    }
}