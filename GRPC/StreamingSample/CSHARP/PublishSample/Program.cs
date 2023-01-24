using Grpc.Core;
using Service;
using static Service.RelayService;

// Channel contains information for establishing a connection to the server
Channel channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

// create the RelayServiceClient from the channel
RelayServiceClient client = new RelayServiceClient(channel);

// call PublishAsync and get the confirmation reply
PublishConfirmed confirmation =
    await client.PublishAsync(new Message { Msg = "Published from C# Client" });