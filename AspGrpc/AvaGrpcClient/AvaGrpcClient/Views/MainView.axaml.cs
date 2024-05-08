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
                CommonData.Url!, // server address
                new GrpcChannelOptions
                {
                    // indicates the browser grpc connection
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
        // call simple single request/single response SayHello service
        var reply =
            await _greeterGrpcClient.SayHelloAsync(new HelloRequest { Name = GreetingName });

        // display the result
        HelloResultText.Text = reply.Msg;
    }

    // test server streaming
    private async void TestStreamingServerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // set initial values to empty strings
        StreamingServerResultsText.Text = string.Empty;
        StreamingErrorText.Text = string.Empty;
        try
        {
            // get the server stream container
            var serverStreamingResponsesContainer = 
                _greeterGrpcClient.ServerStreamHelloReplies(new HelloRequest { Name = GreetingName });
            
            // foreach of the async responses from the server
            // passing the Cancellation Token allows cancelling the server
            // streaming from the client
            await foreach (var response in serverStreamingResponsesContainer.ResponseStream.ReadAllAsync(_serverStreamCancellationTokenSource.Token))
            {
                // change the text of the TextBox
                StreamingServerResultsText.Text = response.Msg;
            }
        }
        catch(RpcException exception)
        {
            // if an exception is throws, show the exception message
            StreamingErrorText.Text = $"ERROR: {exception.Message}";
        }
    }

    private async void TestStreamingServerWithErrorButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // set initial values to empty strings
        StreamingServerResultsText.Text = string.Empty;
        StreamingErrorText.Text = string.Empty;

        try
        {
            // foreach of the async responses from the server
            var serverStreamingResponsesContainer = _greeterGrpcClient.ServerStreamHelloRepliesWithError(new HelloRequest { Name = GreetingName });

            // foreach of the async responses from the server
            await foreach (var response in serverStreamingResponsesContainer.ResponseStream.ReadAllAsync(_serverStreamCancellationTokenSource.Token))
            {
                // change the text of the TextBox
                StreamingServerResultsText.Text = response.Msg;
            }
        }
        catch (RpcException exception)
        {
            // if an exception is throws, show the exception message
            StreamingErrorText.Text = $"ERROR: {exception.Message}";
        }
    }


    private void TestStreamingServerCancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // send signal to the server to cancel streaming 
        _serverStreamCancellationTokenSource?.Cancel();

        // change the streaming token
        _serverStreamCancellationTokenSource = new CancellationTokenSource();
    }

    // test streaming client to the server
    // unfortunately the client in a browser does not stream,
    // it accumulates all the messages on the browser, and
    // sends them together after the client indicates the end of streaming
    private async void TestStreamingClientButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // reset the UI text to emtpy
        StreamingClientResultsText.Text = string.Empty;

        // create stream container
        var clientStreamContainer = _greeterGrpcClient.ClientStreamHelloRequests();

        for (int i = 0; i < 5; i++)
        {
            // push the messages into the request streams
            await clientStreamContainer.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });

            await Task.Delay(1000);
        }

        // indicate the completion of the client streaming
        // Unfortunately it is only at this point that all the client messages
        // will be sent to the server. Essentially that means that there is 
        // no client streaming
        await clientStreamContainer.RequestStream.CompleteAsync();

        // get the server response
        var clientStreamingResponse = await clientStreamContainer;

        // set the visual text of the server response
        StreamingClientResultsText.Text = clientStreamingResponse.Msg;
    }

    // Bi-Directional (Client and Server) streaming test. 
    // Unfortunately the client messages are accumulated on the client side
    // and sent to the server together only after
    // the client indicates the end of streaming
    // Which essentially means that there is no client streaming
    private async void TestStreamingClientServerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // create server and client streamd container
        var clientServerStreamContainer = _greeterGrpcClient.ClientAndServerStreamingTest();

        // reset the server reply text to empty
        StreamingClientServerResultsText.Text = string.Empty;

        // create an async task to asynchronously process the server responses 
        // as they come
        var readTask = Task.Run(async () =>
        {
            await foreach (var reply in clientServerStreamContainer.ResponseStream.ReadAllAsync())
            {
                // for each server response we assing it to show in the client browser
                await Dispatcher.UIThread.InvokeAsync(() => { StreamingClientServerResultsText.Text += reply.Msg + "\n"; });
            }
        });

        // push 3 client requests into the stream
        // unfortunately they'll accumulated on the client and sent together 
        // only after the client call RequestStream.CompleteAsync() method
        for (int i = 0; i < 3; i++)
        {
            await clientServerStreamContainer.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });

            await Task.Delay(1000);
        }

        // wait for both the client and the server processing to finish.
        await Task.WhenAll(readTask, clientServerStreamContainer.RequestStream.CompleteAsync());
    }

}