# import python packages
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

# create the channel
channel = grpc.insecure_channel('localhost:5555')

# create the client stub object for RelayService
stub = service_pb2_grpc.RelayServiceStub(channel);

# create and publish the message
response = stub.Publish(service_pb2.Message(msg='Publish from Python Client'));