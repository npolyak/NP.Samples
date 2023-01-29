using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.PersonClient;

IRelayClient relayClient = ClientBuilder.GetClient();

IDisposable disposable = 
    relayClient
        .ObserveTopicStream<Person>(Topic.PersonTopic)
        .Subscribe(OnPersonDataArrived);

void OnPersonDataArrived(Person person)
{
    Console.WriteLine(person.Name);
}

// prevent from exiting
Console.ReadLine();