// import grpc packages
let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');

// load and parse the service.proto file
const root = protoLoader.loadSync
(
    '../../Protos/service.proto',
    {
        keepCase: true,
        longs: String,
        enums: String,
        defaults: true,
        oneofs: true
    });

// get the service package containing RelayService object
const service = grpc.loadPackageDefinition(root).service;

// create the RelayService client connected to localhost:5555 port
const client = new service.RelayService("localhost:5555", grpc.credentials.createInsecure());

// create the client subcription by passing
// an empty Json message (matching empty SubscribeRequest from service.proto)
var call = client.Subscribe({});

// process data stream combing from the server in 
// response to calling Subscribe(...) gRPC
call.on('data', function (response) {
    console.log(response.msg);
});
