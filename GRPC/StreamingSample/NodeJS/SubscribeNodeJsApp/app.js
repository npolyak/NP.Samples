module;

let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');


const protobuf = require('protobufjs');
const { Service } = require('./node_modules/protobufjs/index');

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


const service = grpc.loadPackageDefinition(root).service;


const client = new service.RelayService("localhost:5555", grpc.credentials.createInsecure());

var call = client.Subscribe({});

call.on('data', function (response) {
    console.log(response.msg);
});
