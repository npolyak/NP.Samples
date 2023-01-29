using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.PersonClient;

// get the client from ClientBuilder
IRelayClient relayClient = ClientBuilder.GetClient();

// create person 30 years old, named Joe Doe
Person person = new Person { Age = 30, Name = "Joe Doe"};

// publish the person to Topic.PersonTopic
await relayClient.Publish(Topic.PersonTopic, person);
