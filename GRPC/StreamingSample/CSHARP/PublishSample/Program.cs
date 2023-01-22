using Grpc.Core;
using Service;
using static Service.RelayService;

Channel channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

RelayServiceClient client = new RelayServiceClient(channel);

await client.PublishAsync(new Message { Msg = "Hello World!" });