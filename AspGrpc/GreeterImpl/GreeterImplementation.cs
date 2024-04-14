using Grpc.Core;
using simple;

namespace GrpcServerProcess
{
    public class GreeterImplementation : simple.Greeter.GreeterBase
    {
        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            string name = request.Name;

            return new HelloReply { Msg = $"Hello {name}!!!" };
        }

        public override async Task SayManyHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            string name = request.Name;

            for(int i = 0; i < 10; i++)
            {
                await responseStream.WriteAsync(new HelloReply { Msg = $"Hello {name} {i + 1}" });
                await Task.Delay(500);
            }
        }

        public override async Task<HelloReply> SayHelloToMany(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            string message = "Hello ";

            bool first = true;
            await foreach (var inputMessage in requestStream.ReadAllAsync())
            {
                if (!first)
                {
                    message += ", ";
                }

                message += inputMessage.Name;
                first = false;
            }

            return new HelloReply { Msg = message };
        }
    }
}
