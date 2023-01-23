module;


let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');


const protobuf = require('protobufjs');

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

client.Publish({ msg: "Published from JS Client" }, function (err, response) {

});