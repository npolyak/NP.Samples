# import python packages
import asyncio
import grpc
import grpc_tools.protoc

# generate service_pb2 (for proto messages) and 
# service_pb2_grpc (for RPCs) stubs
grpc_tools.protoc.main([
    'grpc_tools.protoc',
    '-I{}'.format("../../Protos/."),
    '--python_out=.',
    '--grpc_python_out=.',
    '../../Protos/service.proto'
])

# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import service_pb2;
import service_pb2_grpc;

# define async loop
async def run() -> None:
    #create the channel
    async with grpc.aio.insecure_channel('localhost:5555') as channel:
        # create the client stub object for RelayService
        stub = service_pb2_grpc.RelayServiceStub(channel);

        # call Subscribe gRCP and print the responses asyncronously
        async for response in stub.Subscribe(service_pb2.SubscribeRequest()):
            print(response.msg)

# run the async method calling that subscribes and 
# prints the messages coming from the server
asyncio.run(run())