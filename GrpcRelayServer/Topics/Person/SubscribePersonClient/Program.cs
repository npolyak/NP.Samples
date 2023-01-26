using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.PersonTest;

IRelayClient relayClient = ClientBuilder.GetClient();

Person person = new Person { Age = 30, Name = "Joe Doe"};

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