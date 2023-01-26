using NP.Grpc.ClientBuilder;
using NP.Grpc.CommonRelayInterfaces;
using NP.OrgTest;

IRelayClient relayClient = ClientBuilder.GetClient();

Org person = new Org{ Name = "Google, Inc", NumberPeople = 100000};

await relayClient.Publish(Topic.OrgTopic, person);
