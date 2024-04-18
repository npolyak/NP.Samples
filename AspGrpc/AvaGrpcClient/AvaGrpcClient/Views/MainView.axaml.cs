using Avalonia.Controls;
using Avalonia.Threading;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using simple;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AvaGrpcClient.Views;

public partial class MainView : UserControl
{
    Greeter.GreeterClient _greeterGrpcClient;

    CancellationTokenSource _serverStreamCancellationTokenSource;

    public MainView()
    {
        InitializeComponent();
        var channel =
            GrpcChannel.ForAddress
            (
                "https://localhost:1234",
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
        TestStreamingServerWithErrorButton.Click += TestStreamingServerWithErrorButton_Click;
        TestStreamingClientServerButton.Click += TestStreamingClientServerButton_Click;

        _serverStreamCancellationTokenSource = new CancellationTokenSource();
    }

    private string GreetingName => NameToEnter.Text ?? string.Empty;

    private async void TestUnaryHelloButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var reply =
            await _greeterGrpcClient.SayHelloAsync(new HelloRequest { Name = GreetingName });
        HelloResultText.Text = reply.Msg;
    }

    private async void TestStreamingServerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StreamingServerResultsText.Text = string.Empty;
        StreamingErrorText.Text = string.Empty;
        try
        {
            var serverStreamingCall = _greeterGrpcClient.ServerStreamHelloReplies(new HelloRequest { Name = GreetingName });
            await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync(_serverStreamCancellationTokenSource.Token))
            {
                StreamingServerResultsText.Text = response.Msg;
            }
        }
        catch(RpcException exception)
        {
            StreamingErrorText.Text = $"ERROR: {exception.Message}";
        }
    }

    private async void TestStreamingServerWithErrorButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StreamingServerResultsText.Text = string.Empty;
        StreamingErrorText.Text = string.Empty;

        try
        {
            var serverStreamingCall = _greeterGrpcClient.ServerStreamHelloRepliesWithError(new HelloRequest { Name = GreetingName });


            await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync(_serverStreamCancellationTokenSource.Token))
            {
                StreamingServerResultsText.Text = response.Msg;
            }
        }
        catch (RpcException exception)
        {
            StreamingErrorText.Text = $"ERROR: {exception.Message}";
        }
    }


    private void TestStreamingServerCancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _serverStreamCancellationTokenSource?.Cancel();
        _serverStreamCancellationTokenSource = new CancellationTokenSource();
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


    private async void TestStreamingClientServerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var clientServerStreamingCall = _greeterGrpcClient.ClientAndServerStreamingTest();

        StreamingClientServerResultsText.Text = string.Empty;

        var readTask = Task.Run(async () =>
        {
            await foreach (var reply in clientServerStreamingCall.ResponseStream.ReadAllAsync())
            {
                await Dispatcher.UIThread.InvokeAsync(() => { StreamingClientServerResultsText.Text += reply.Msg + "\n"; });
            }
        });

        for (int i = 0; i < 3; i++)
        {
            await clientServerStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });

            await Task.Delay(20);
        }

        await clientServerStreamingCall.RequestStream.CompleteAsync();
        await readTask;
    }

}