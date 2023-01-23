import logging;

import grpc

import grpc_tools.protoc


logging.basicConfig()

grpc_tools.protoc.main([
    'grpc_tools.protoc',
    '-I{}'.format("../../Protos/."),
    '--python_out=.',
    '--grpc_python_out=.',
    '../../Protos/service.proto'
])

import service_pb2;
import service_pb2_grpc;


channel = grpc.insecure_channel('localhost:5555')

stub = service_pb2_grpc.RelayServiceStub(channel);

response = stub.Publish(service_pb2.Message(msg='Publish from Python Client'));