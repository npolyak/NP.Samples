using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;

namespace NP.Samples.Implementations
{
    [RegisterType(resolutionKey:"TheOrg")]
    public class AnotherOrg : IOrgGettersOnly
    {
        public string OrgName { get; set; } 

        public IPersonGettersOnly Manager { get; }

        public ILog Log { get; }

        [CompositeConstructor]
        public AnotherOrg([Inject(resolutionKey:"AnotherPerson")] IPersonGettersOnly manager, [Inject]ILog log)
        {
            Manager = manager;
            Log = log;
        }

        public void LogOrgInfo()
        {
            Log?.WriteLog($"OrgName: {OrgName}");
            Log?.WriteLog($"Manager: {Manager!.PersonName}");
            Log?.WriteLog($"Manager's Address: {Manager!.Address.City}, {Manager.Address.ZipCode}");
        }
    }
}
