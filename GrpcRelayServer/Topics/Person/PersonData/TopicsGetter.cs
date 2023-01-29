using NP.DependencyInjection.Attributes;
using NP.Protobuf;

namespace PersonData
{
    [HasRegisterMethods]
    public static class TopicsGetter
    {        
        /// <summary>
        /// Returns the PersonTopic value as part of the MultiCell Topics collection
        /// </summary>
        [RegisterMultiCellMethod(cellType: typeof(Enum), resolutionKey: IoCKeys.Topics)]
        public static Enum GetTopics()
        {
            return NP.PersonClient.Topic.PersonTopic;
        }
    }
}
