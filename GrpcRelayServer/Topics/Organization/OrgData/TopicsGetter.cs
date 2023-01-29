using NP.DependencyInjection.Attributes;
using NP.Protobuf;

namespace OrgData
{
    [HasRegisterMethods]
    public static class TopicsGetter
    {
        /// <summary>
        /// Returns the OrgTopic value as part of the MultiCell Topics collection
        /// </summary>
        [RegisterMultiCellMethod(cellType: typeof(Enum), resolutionKey: IoCKeys.Topics)]
        public static Enum GetTopics()
        {
            return NP.OrgClient.Topic.OrgTopic;
        }
    }
}