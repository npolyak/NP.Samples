const { HelloRequest, HelloReply } = require('./Greeter_pb.js');
const { GreeterClient } = require('./Greeter_grpc_web_pb.js');

global.HelloRequest = HelloRequest;
global.HelloReply = HelloReply;
global.GreeterClient = GreeterClient;