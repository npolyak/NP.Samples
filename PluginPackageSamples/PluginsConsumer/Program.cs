using FluentAssertions;
using NP.DependencyInjection.Interfaces;
using NP.IoCy;
using NP.PackagePluginsTest.PluginInterfaces;

namespace PluginsConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // create container builder
            IContainerBuilder<string?> builder = new ContainerBuilder<string?>();

            // load plugins dynamically from sub-folders of Plugins folder
            // localted under the same folder that the executable
            builder.RegisterPluginsFromSubFolders("Plugins");

            // build the container
            IDependencyInjectionContainer<string?> container = builder.Build();

            // get the pluging for manipulating double numbers
            IDoubleManipulationsPlugin doubleManipulationsPlugin =
                container.Resolve<IDoubleManipulationsPlugin>();

            // get the result of 4 * 5
            double timesResult =
                doubleManipulationsPlugin.Times(4.0, 5.0);

            // check that 4 * 5 == 20
            timesResult.Should().Be(20.0);

            // get the result of 4 + 5
            double plusResult = doubleManipulationsPlugin.Plus(4.0, 5.0);

            // check that 4 + 5 is 9
            plusResult.Should().Be(9.0);

            // get string manipulations plugin
            IStringManipulationsPlugin stringManipulationsPlugin =
                container.Resolve<IStringManipulationsPlugin>();

            // concatinate two strings "Str1" and "Str2
            string concatResult = stringManipulationsPlugin.Concat("Str1", "Str2");

            // verify that the concatination result is "Str1Str2"
            concatResult.Should().Be("Str1Str2");

            // repeast "Str1" 3 times
            string repeatResult = stringManipulationsPlugin.Repeat("Str1", 3);

            // verify that the result is "Str1Str1Str1"
            repeatResult.Should().Be("Str1Str1Str1");

            var methodNames = container.Resolve<IEnumerable<string>>("MethodNames");

            methodNames.Count().Should().Be(4);

            methodNames.Should().Contain(nameof(IDoubleManipulationsPlugin.Plus));
            methodNames.Should().Contain(nameof(IDoubleManipulationsPlugin.Times));
            methodNames.Should().Contain(nameof(IStringManipulationsPlugin.Concat));
            methodNames.Should().Contain(nameof(IStringManipulationsPlugin.Repeat));

            Console.WriteLine("The END");
        }
    }
}