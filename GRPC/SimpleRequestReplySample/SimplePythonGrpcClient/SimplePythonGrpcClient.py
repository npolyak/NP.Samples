# import require packages
import grpc
import grpc_tools.protoc

# generate service_pb2 (for proto messages) and 
# service_pb2_grpc (for RPCs) stubs
grpc_tools.protoc.main([
    'grpc_tools.protoc',
    '-I{}'.format("../Protos/."),
    '--python_out=.',
    '--grpc_python_out=.',
    '../Protos/service.proto'
])

# import the generated stubs
import service_pb2;
import service_pb2_grpc;

# create the channel connecting to the server at localhost:5555
channel = grpc.insecure_channel('localhost:5555')

# get the server gRCP stub
greeterStub = service_pb2_grpc.GreeterStub(channel)

# call SayHello RPC on the server passing HelloRequest message
# whose name is set to 'Python'
response = greeterStub.SayHello(service_pb2.HelloRequest(name='Python'))

# print the result
print(response.msg)

print("END")
