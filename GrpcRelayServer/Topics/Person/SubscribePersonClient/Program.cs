using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.PersonClient;

// create relay client
IRelayClient relayClient = ClientBuilder.GetClient();

// observe Topic PersonTopic and define the action on arrived Person object 
// by calling subscribe
IDisposable disposable = 
    relayClient
        .ObserveTopicStream<Person>(Topic.PersonTopic)
        .Subscribe(OnPersonDataArrived);

void OnPersonDataArrived(Person person)
{
    // print Person.Name for every new person
    // coming from the server
    Console.WriteLine(person.Name);
}

// prevent from exiting
Console.ReadLine();