// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.DependencyInjection.Attributes;
using NP.Samples.Interfaces;

namespace NP.Samples.Implementations
{
    public class Org : IOrg
    {
        public string? OrgName { get; set; }

        [Inject]
        public IPerson? Manager { get; set; }

        public IPerson? ProjLead { get; set; }

        [Inject]
        public ILog? Log { get; set; }

        [Inject(resolutionKey:"MyLog")]
        public ILog? Log2 { get; set; }

        public void LogOrgInfo()
        {
            Log?.WriteLog($"OrgName: {OrgName}");
            Log?.WriteLog($"Manager: {Manager!.PersonName}");
            Log?.WriteLog($"Manager's Address: {Manager!.Address.City}, {Manager.Address.ZipCode}");
        }
    }
}
