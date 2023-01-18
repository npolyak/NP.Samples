using Greet;
using Grpc.Core;

namespace SimpleGrpcServerTest
{
    internal class GreeterImplementation : Greeter.GreeterBase
    {
        public override async Task<HelloReply> SayHello
        (
            HelloRequest request, 
            ServerCallContext context)
        {
            return new HelloReply
            {
                Msg = $"Hello {request.Name}"
            };
        }
    }
}
