Command to generate grpc javascript client proxy code:

protoc -I=. Greeter.proto --js_out=import_style=commonjs:..\AspGrpcServerWithRazorClient\wwwroot\dist --grpc-web_out=import_style=commonjs,mode=grpcwebtext:..\AspGrpcServerWithRazorClient\wwwroot\dist