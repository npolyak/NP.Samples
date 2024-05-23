using Grpc.Core;
using simple;

namespace GrpcServerProcess;

public class GreeterImplementation : simple.Greeter.GreeterBase
{
    public GreeterImplementation()
    {
        
    }

    public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        string name = request.Name;

        // return HelloReply with its message set to the greeting string
        return new HelloReply { Msg = $"Hello {name}!!!" };
    }


    private async Task ServerStreamHelloRepliesImpl
    (
        HelloRequest request,
        bool throwException,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        // get the name from the client request
        string name = request.Name;

        for (int i = 0; i < 20; i++)
        {
            await Task.Delay(200); // delay by 0.2 of a second

            // write the reply asynchronously
            await responseStream.WriteAsync(new HelloReply { Msg = $"Hello {name} {i + 1}" });

            // cancel stream if cancellation is requested
            context.CancellationToken.ThrowIfCancellationRequested();

            // throw the RpcException (propagated to the client) 
            // after the 11th iteration if throwException argument is passed as true
            if (i == 10 && throwException)
            {
                // sets the status code and Error messages for the client and the server. 
                // the status code and the error message for the client will be sent over to the client,
                // while the error message for the server can be logged or acted upon in varous ways 
                // on the server.
                throw new RpcException(new Status(StatusCode.Internal, "Error Status Detail (for the client)"), "ERROR: Cannot Continue with streaming (server error)!");
            }
        }
    }


    public override async Task ServerStreamHelloReplies
    (
        HelloRequest request,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        await ServerStreamHelloRepliesImpl(request, false, responseStream, context);
    }


    public override async Task ServerStreamHelloRepliesWithError
    (
        HelloRequest request,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        await ServerStreamHelloRepliesImpl(request, true, responseStream, context);
    }


    public override async Task<HelloReply> ClientStreamHelloRequests(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        string message = "Hello ";

        bool first = true;

        // for each message from the client (read asynchronously)
        await foreach (var inputMessage in requestStream.ReadAllAsync())
        {
            if (!first)
            {
                // if not the first message prepend it with ", " string
                message += ", ";
            }

            // add the Name from the message
            message += inputMessage.Name;
            first = false;
        }

        // after streaming ended return the HelloReply the corresponding Msg property
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
