using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;

namespace NP.Samples.Implementations
{

    [RegisterType(resolutionKey:"AnotherPerson")]
    public class AnotherPerson : IPersonGettersOnly
    {
        public string PersonName { get; set; }


        public IAddress Address { get; }

        [CompositeConstructor]
        public AnotherPerson([Inject(resolutionKey: "TheAddress")] IAddress address)
        {
            this.Address = address;
        }
    }
}
