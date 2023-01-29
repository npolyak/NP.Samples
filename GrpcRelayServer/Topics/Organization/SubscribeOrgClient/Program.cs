using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.OrgClient;

IRelayClient relayClient = ClientBuilder.GetClient();

IDisposable disposable = 
    relayClient
        .ObserveTopicStream<Org>(Topic.OrgTopic)
        .Subscribe(OnOrgDataArrived);

void OnOrgDataArrived(Org org)
{
    Console.WriteLine(org.Name);
}

// prevent from exiting
Console.ReadLine();