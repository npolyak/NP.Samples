import asyncio
import logging

import grpc

import grpc_tools.protoc

grpc_tools.protoc.main([
    'grpc_tools.protoc',
    '-I{}'.format("../../Protos/."),
    '--python_out=.',
    '--grpc_python_out=.',
    '../../Protos/service.proto'
])

import service_pb2;
import service_pb2_grpc;


async def run() -> None:
    async with grpc.aio.insecure_channel('localhost:5555') as channel:
        stub = service_pb2_grpc.RelayServiceStub(channel);

        async for response in stub.Subscribe(service_pb2.SubscribeRequest()):
            print(response.msg)

logging.basicConfig()
asyncio.run(run())