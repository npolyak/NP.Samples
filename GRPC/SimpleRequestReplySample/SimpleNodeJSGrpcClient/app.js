module;

// import grpc functionality 
let grpc = require('@grpc/grpc-js');

// import protoLoader functionality
let protoLoader = require('@grpc/proto-loader');

// load the services from service.proto file
const root =
    protoLoader.loadSync
        (
            '../Protos/service.proto', // path to service.proto file
            {
                keepCase: true, // service loading parameters
                longs: String,
                enums: String,
                defaults: true,
                oneofs: true
            });

// get the client package definitions for greet package
// defined within the services.proto file
const greet = grpc.loadPackageDefinition(root).greet;

// connect the client to the server
const client = new greet.Greeter("localhost:5555", grpc.credentials.createInsecure());

// call sayHello RPC passing "Java Script" as the name parameter
client.sayHello({ name: "Java Script" }, function (err, response) {
    // obtain the response and print its msg field
    console.log(response.msg);
});

// prevent the program from exiting
var done = (function wait() { if (!done) setTimeout(wait, 1000) })();