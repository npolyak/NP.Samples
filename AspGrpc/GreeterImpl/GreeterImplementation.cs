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

        public override async Task ServerStreamHelloReplies
        (
            HelloRequest request, 
            IServerStreamWriter<HelloReply> responseStream, 
            ServerCallContext context)
        {
            string name = request.Name;

            for(int i = 0; i < 10; i++)
            {
                await responseStream.WriteAsync(new HelloReply { Msg = $"Hello {name} {i + 1}" });
                await Task.Delay(500);
                context.CancellationToken.ThrowIfCancellationRequested();
            }
        }

        public override async Task<HelloReply> ClientStreamHelloRequests(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
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

        public override async Task ClientAndServerStreamingTest(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            //await foreach (var inputMessage in requestStream.ReadAllAsync())
            //{
            //    string name = inputMessage.Name;

            //    for (int i = 0; i < 4; i++)
            //    {
            //        string msg = $"Hello {name}_{i + 1}";
            //        Console.WriteLine(msg);
            //        await responseStream.WriteAsync(new HelloReply { Msg = msg });
            //        await Task.Delay(200);

            //        context.CancellationToken.ThrowIfCancellationRequested();
            //    }
            //}

            await Parallel.ForEachAsync(requestStream.ReadAllAsync(), async (inputMessage, ct) =>
            {
                string name = inputMessage.Name;

                for (int i = 0; i < 4; i++)
                {
                    string msg = $"Hello {name}_{i + 1}";
                    Console.WriteLine(msg);
                    await responseStream.WriteAsync(new HelloReply { Msg = msg });
                    await Task.Delay(200);

                    context.CancellationToken.ThrowIfCancellationRequested();
                }
            });
        }
    }
}
