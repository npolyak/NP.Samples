using Greet;
using Grpc.Core;
using System.Threading.Tasks;

namespace SimpleGrpcServerTest
{
    internal class GreeterImplementation : Greeter.GreeterBase
    {
        // provides implementation for the abstract method SayHello(...)
        // from the generated server stub
        public override async Task<HelloReply> SayHello
        (
            HelloRequest request, 
            ServerCallContext context)
        {
            // return HelloReply with Msg consisting of the word Hello
            // and the name passed by the request
            return new HelloReply
            {
                Msg = $"Hello {request.Name}"
            };
        }
    }
}
