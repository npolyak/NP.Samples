using FluentAssertions;
using NP.Samples.Implementations;
using NP.Samples.Interfaces;
using NP.DependencyInjection.Interfaces;
using NP.IoCy;
using System.Reflection;
using NP.DependencyInjection.Attributes;

namespace NP.Samples.IoCyTests;

public static class Program
{
    public static bool IsSingleton<T>
    (
        this IDependencyInjectionContainer<string?> container,
        string? key = null
    )
    {
        T obj1 = container.Resolve<T>(key);

        obj1.Should().NotBeNull();

        T obj2 = container.Resolve<T>(key);
        obj2.Should().NotBeNull();

        return object.ReferenceEquals(obj1, obj2);
    }

    public static IOrg CreateOrg()
    {
        IOrg org = new Org();

        org.Manager = new Person();

        org.OrgName = "Other Department Store";
        org.Manager.PersonName = "Joe Doe";

        org.Manager.Address = new Address();
        org.Manager.Address.City = "Boston";
        org.Manager.Address.ZipCode = "12345";

        org.Log = new ConsoleLog();

        return org;
    }

    public static IOrg CreateOrgWithArgument([Inject(resolutionKey: "TheManager")] IPerson person)
    {
        return new Org { OrgName = "Other Department Store", Manager = person };
    }

    public static void TestOrg
    (
        this IDependencyInjectionContainer<string?> container, 
        bool isSingleton, 
        string? key = null)
    {
        container.IsSingleton<IOrg>(key).Should().Be(isSingleton);
        IOrg org = container.Resolve<IOrg>(key);
        org.OrgName.Should().Be("Other Department Store");
    }

    static void Main(string[] args)
    {
        // create container builder
        var containerBuilder = new ContainerBuilder<string?>();

        // register Person object to be returned by IPerson resolving type
        containerBuilder.RegisterType<IPerson, Person>();
        containerBuilder.RegisterType<IAddress, Address>();
        containerBuilder.RegisterType<IOrg, Org>();

        // Register FilesLog as a singleton to be returned by ILog type
        containerBuilder.RegisterSingletonType<ILog, FileLog>();

        // register ConsoleLog to be returned by (ILog type, "MyLog" key) combination
        containerBuilder.RegisterType<ILog, ConsoleLog>("MyLog");

        // Create container
        var container1 = containerBuilder.Build();


        // resolve and compose organization
        // all its injectable properties will be populated
        // this stage. 
        IOrg org = container1.Resolve<IOrg>();

        // make sure ProjLead is null (it does not have
        // InjectAttribute)
        org.ProjLead.Should().BeNull();

        // make sure org.Manager is not null since it 
        // has InjectAttribute
        org.Manager.Should().NotBeNull();

        // get another IPerson object from the container
        IPerson person = container1.Resolve<IPerson>();

        // make sure that the new IPerson object is not the same 
        // as org.Manager (since IPerson - Person combination was not 
        // registered as a singleton: containerBuilder.RegisterType<IPerson, Person>();
        person.Should().NotBeSameAs(org.Manager);

        // Get another ILog (remember it was registered as a singleton:
        //  containerBuilder.RegisterSingletonType<ILog, FileLog>();)
        ILog log = container1.Resolve<ILog>();

        // Since it is a singleton, the new log should 
        // be the same as the org.Log
        log.Should().BeSameAs(org.Log);

        // Log2 is injected by (ILog, "MyLog") type-key combination.
        // This combination has been registered as a non-singletong:
        // containerBuilder.RegisterType<ILog, ConsoleLog>("MyLog");
        org.Log2.Should().NotBeNull();
        org.Log2.Should().BeOfType<ConsoleLog>();

        ILog log2 = container1.Resolve<ILog>("MyLog");

        log2.Should().NotBeNull();

        // the new log should not be the same as the old one
        // since it is not a singleton.
        log2.Should().NotBeSameAs(org.Log2);
        log2.Should().BeOfType<ConsoleLog>();

        // assign some values to the organization's properties
        org.OrgName = "Nicks Department Store";
        org.Manager.PersonName = "Nick Polyak";
        org.Manager.Address.City = "Miami";
        org.Manager.Address.ZipCode = "12345";

        // since org.Log is of FileLog type, these value
        // will be printed to MyLogFile.txt file within the same
        // folder as the executable
        org.LogOrgInfo();
        

        // replace ILog (formely resolved to FileLog) to be resolved to 
        // ConsoleLog
        ConsoleLog consoleLog = new ConsoleLog();
        containerBuilder.RegisterSingletonInstance<ILog>(consoleLog);

        // and create another container from containerBuilder. This new container
        // will reflect the change - instead of ILog resolving to FileLog
        // it will be resolving to ConsoleLog within the new container.
        var container2 = containerBuilder.Build();

        // resolve org from another Container.
        IOrg orgWithConsoleLog = container2.Resolve<IOrg>();

        orgWithConsoleLog.Log.Should().NotBeNull();
        // check that the resolved ILog is the same instance
        // as the consoleLog used for the singleton instance.
        orgWithConsoleLog.Log.Should().BeSameAs(consoleLog);

        // assing some data to the newly resolved IOrg object
        orgWithConsoleLog.OrgName = "Nicks Department Store";
        orgWithConsoleLog.Manager.PersonName = "Nick Polyak";
        orgWithConsoleLog.Manager.Address.City = "Miami";
        orgWithConsoleLog.Manager.Address.ZipCode = "12345";

        // send org data to console instead of a file.
        orgWithConsoleLog.LogOrgInfo();

        // now register a method Program.CreateOrg() with the container builder
        // this method will return IOrg object populated with some different data.
        // E.g. the owner will be "Joe Doe" and the org name will be "Other Department Store"
        containerBuilder.RegisterFactoryMethod(CreateOrg);
        // create a container with registered CreateOrg method
        var container3 = containerBuilder.Build();

        // test that organization is not a singleton and has its
        // properties correctly populated
        container3.TestOrg(false /* test for Transient */);

        // now register the same CreateOrg method by a pair (ILog, "TheOrg")
        containerBuilder.RegisterFactoryMethod(CreateOrg, "TheOrg");
        // create the container
        var container4 = containerBuilder.Build();
        // make sure the resulting org is not null, has correct data and is not a singleton
        container4.TestOrg(false, "TheOrg" /* The resolution key */);
        container4.TestOrg(false); // should still work because the old registration is also there

        // now replace the factory method mapped to ILog type, by a 
        // the same factory method, only as a singleton. 
        containerBuilder.RegisterSingletonFactoryMethod(CreateOrg);
        // build the container
        var container5 = containerBuilder.Build();
        // test that the resulting IOrg is a singleton
        container5.TestOrg(true /* test for a singleton */);

        // now replace the factory method cell pointed by (ILog, "TheOrg") pair
        // by a singleton. 
        containerBuilder.RegisterSingletonFactoryMethod(CreateOrg, "TheOrg");
        // create the container
        var container6 = containerBuilder.Build();
        // make sure the result is a singleton
        container6.TestOrg(true, "TheOrg" /* resolution key */);
        container6.TestOrg(true);

        // unregister FactoryMethod pointed to by  IOrg
        containerBuilder.UnRegister(typeof(IOrg));
        // unregister FactoryMethod pointed to by (IOrg, "TheOrg") pair
        containerBuilder.UnRegister(typeof(IOrg), "TheOrg");
        // build the container
        var container7 = containerBuilder.Build();

        // get the org by IOrg
        org = container7.Resolve<IOrg>();
        // make sure the resulting org is null (since the FactoryMethod cell has been unregistered)
        org.Should().BeNull();
        // get the cell by (IOrg, "TheOrg") pair
        org = container7.Resolve<IOrg>("TheOrg");
        // make sure it is null (since it has been unregistered)
        org.Should().BeNull();

        MethodInfo createOrgMethodInfo =
            typeof(Program).GetMethod(nameof(CreateOrgWithArgument));

        // register the singleton Person cell for (typeof(IPerson), "TheManager") pair
        // to be injected as the argument to CreateOrgWithArgument method
        containerBuilder.RegisterSingletonType<IPerson, Person>("TheManager");

        // register factory methods by their MethodInfo
        containerBuilder.RegisterSingletonFactoryMethodInfo<IOrg>(createOrgMethodInfo!, "TheOrg");
        var container8 = containerBuilder.Build();
        container8.TestOrg(true, "TheOrg"); // test the resulting org is a singleton


        containerBuilder.RegisterFactoryMethodInfo<IOrg>(createOrgMethodInfo, "TheOrg");
        var container9 = containerBuilder.Build();
        container9.TestOrg(false, "TheOrg"); // test the resulting org is not singleton

        // create a brand new container builder for building types with RegisterType attribute
        var attributedTypesContainerBuilder = new ContainerBuilder<string?>();

        // RegisterTypeAttribute will have parameters specifying the resolving type 
        // and resolution Key (if applicable). It will also specify whether the
        // cell should be singleton or not. 
        attributedTypesContainerBuilder.RegisterAttributedClass(typeof(AnotherOrg));
        attributedTypesContainerBuilder.RegisterAttributedClass(typeof(AnotherPerson));
        attributedTypesContainerBuilder.RegisterAttributedClass(typeof(ConsoleLog));
        attributedTypesContainerBuilder.RegisterType<IAddress, Address>("TheAddress");

        // create container
        var container10 = attributedTypesContainerBuilder.Build();

        // get the organization also testing the composing constructors
        IOrgGettersOnly orgGettersOnly =
            container10.Resolve<IOrgGettersOnly>("MyOrg");

        // make sure that Manager and Address are not null
        orgGettersOnly.Manager.Address.Should().NotBeNull();

        // make sure ILog is a singleton.
        container10.IsSingleton<ILog>().Should().BeTrue();

        IContainerBuilder containerBuilder11 = new ContainerBuilder();


        containerBuilder11.RegisterAttributedStaticFactoryMethodsFromClass(typeof(FactoryMethods));

        var container11 = containerBuilder11.Build();

        IOrg org11 = container11.Resolve<IOrg>("TheOrg");

        // check that the org11.OrgName was set by the factory method to "Other Department Store"
        org11.OrgName.Should().Be("Other Department Store");

        // check that the org11.Manager.PersonName was set by the factory method to "Joe Doe"
        org11.Manager.PersonName.Should().Be("Joe Doe");

        // Check that the org11.Manager.City is "Providence"
        org11.Manager.Address.City.Should().Be("Providence");

        // get another org
        IOrg anotherOrg11 = container11.Resolve<IOrg>("TheOrg");

        // test that it is not the same object as previous org
        // (since org is transient)
        org11.Should().NotBeSameAs(anotherOrg11);

        // test that the manager is the same between the two orgs
        // because CreateManager(...) creates a singleton
        org11.Manager.Should().BeSameAs(anotherOrg11.Manager);

        // get another address
        IAddress address11 = container11.Resolve<IAddress>("TheAddress");

        // test that the new address object is not the same
        // since CreateAddress(...) is not Singleton
        address11.Should().NotBeSameAs(org11.Manager.Address);

        Console.WriteLine("The END");
        Console.ReadKey();
    }
}