using Avalonia.Controls;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using simple;
using System;
using System.Net.Http;
using System.Threading;

namespace AvaGrpcClient.Views;

public partial class MainView : UserControl
{
    Greeter.GreeterClient _greeterGrpcClient;

    CancellationTokenSource? _serverStreamCancellationTokenSource;

    public MainView()
    {
        InitializeComponent();
        var channel =
            GrpcChannel.ForAddress
            (
                "https://localhost:55003",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                });

        // create the GreeterClient service
        _greeterGrpcClient = new Greeter.GreeterClient(channel);

        TestUnaryHelloButton.Click += TestUnaryHelloButton_Click;
        TestStreamingServerButton.Click += TestStreamingServerButton_Click;
        TestStreamingServerCancelButton.Click += TestStreamingServerCancelButton_Click;
        TestStreamingClientButton.Click += TestStreamingClientButton_Click;
    }


    private async void TestUnaryHelloButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var reply =
            await _greeterGrpcClient.SayHelloAsync(new HelloRequest { Name = "C# Client" });
        HelloResultText.Text = reply.Msg;
    }

    private async void TestStreamingServerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StreamingServerResultsText.Text = string.Empty;

        try
        {
            var serverStreamingCall = _greeterGrpcClient.ServerStreamHelloReplies(new HelloRequest { Name = "C# Client" });
            _serverStreamCancellationTokenSource = new CancellationTokenSource();
            await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync(_serverStreamCancellationTokenSource.Token))
            {
                StreamingServerResultsText.Text += response.Msg + "\n";
            }
        }
        catch(RpcException exception)
        {

        }
    }

    private void TestStreamingServerCancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _serverStreamCancellationTokenSource?.Cancel();
        _serverStreamCancellationTokenSource = null;
    }

    private async void TestStreamingClientButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var clientSreamingCall = _greeterGrpcClient.ClientStreamHelloRequests();

        for (int i = 0; i < 3; i++)
        {
            await clientSreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });
        }

        await clientSreamingCall.RequestStream.CompleteAsync();
        var clientStreamingResponse = await clientSreamingCall;

        StreamingClientResultsText.Text = clientStreamingResponse.Msg;
    }
}