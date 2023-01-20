import logging;

import grpc

import grpc_tools.protoc

build_exe_options = {'packages': ['pkg_resources']}

logging.basicConfig()

grpc_tools.protoc.main([
    'grpc_tools.protoc',
    '-I{}'.format("../Interfaces/."),
    '--python_out=.',
    '--grpc_python_out=.',
    '../Interfaces/service.proto'
])

import service_pb2;
import service_pb2_grpc;

channel = grpc.insecure_channel('localhost:5555')

stub = service_pb2_grpc.GreeterStub(channel);

response = stub.SayHello(service_pb2.HelloRequest(name='Joe Doe'))

print(response.msg);

print("END")
