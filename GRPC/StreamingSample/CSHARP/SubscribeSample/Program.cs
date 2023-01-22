using Grpc.Core;
using Microsoft.VisualBasic;
using static Service.RelayService;

Channel channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

RelayServiceClient client = new RelayServiceClient(channel);

using var replies = client.Subscribe(new Service.SubscribeRequest());

while(await replies.ResponseStream.MoveNext())
{
    var msg = replies.ResponseStream.Current;

    Console.WriteLine(msg);
}