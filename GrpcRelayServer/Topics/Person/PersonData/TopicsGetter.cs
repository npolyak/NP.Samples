using NP.DependencyInjection.Attributes;
using NP.Protobuf;

namespace PersonData
{
    [HasRegisterMethods]
    public static class TopicsGetter
    {
        [RegisterMultiCellMethod(cellType: typeof(Enum), resolutionKey: IoCKeys.Topics)]
        public static Enum GetTopics()
        {
            return NP.PersonTest.Topic.PersonTopic;
        }
    }
}
