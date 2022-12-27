using NP.IoCy;
using FluentAssertions;
using NP.DependencyInjection.Interfaces;
using PluginInterfaces;
using System.Text;

namespace NP.Samples.IoCyDynamicLoadingTests;

public static class Program
{
    static void Main(string[] args)
    {
        // create container builder
        IContainerBuilder builder = new ContainerBuilder();

        builder.RegisterPluginsFromSubFolders("Plugins");

        IDependencyInjectionContainer container = builder.Build();

        IDoubleManipulationsPlugin doubleManipulationsPlugin = container.Resolve<IDoubleManipulationsPlugin>();

        double timesResult = 
            doubleManipulationsPlugin.Times(4d, 5d);

        timesResult.Should().BeApproximately(20.0, 0.000000001);

        double plusResult = doubleManipulationsPlugin.Plus(4.0, 5.0);

        plusResult.Should().BeApproximately(9, 0.000000001);

        IStringManipulationsPlugin stringManipulationsPlugin = container.Resolve<IStringManipulationsPlugin>();

        string concatResult = stringManipulationsPlugin.Concat("Str1", "Str2");

        concatResult.Should().Be("Str1Str2");

        string repeatResult = stringManipulationsPlugin.Repeat("Str1", 3);

        repeatResult.Should().Be("Str1Str1Str1");

        Console.WriteLine("The END");
    }
}