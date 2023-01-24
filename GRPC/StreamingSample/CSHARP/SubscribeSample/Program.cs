using Grpc.Core;
using static Service.RelayService;

// channel contains info for connecting to the server
Channel channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

// create RelayServiceClient
RelayServiceClient client = new RelayServiceClient(channel);

// replies is an async stream
using var replies = client.Subscribe(new Service.SubscribeRequest());

// move to the next message within the reply stream
while(await replies.ResponseStream.MoveNext())
{
    // get the current message within reply stream
    var message = replies.ResponseStream.Current;

    // print the current message
    Console.WriteLine(message.Msg);
}