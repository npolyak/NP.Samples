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

// publish the Message object "Published from JS Client" 
// (as long as the Json structure matches the Message object structure it will be 
// converted to Message object)
client.Publish({ msg: "Published from JS Client" }, function (err, response) {

});