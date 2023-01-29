using Google.Protobuf.WellKnownTypes;
using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.PersonClient;

IRelayClient relayClient = ClientBuilder.GetClient();

Person person = new Person { Age = 30, Name = "Joe Doe"};

await relayClient.Publish(Topic.PersonTopic, person);
