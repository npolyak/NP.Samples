using Grpc.Core;
using Service;
using static Service.RelayService;

namespace StreamingRelayServer
{

    internal class RelayServiceImplementations : RelayServiceBase
    {
        // all client subscriptions
        List<Subscription> _subscriptions = new List<Subscription>();

        // Publish implementation
        public override async Task<PublishConfirmed> Publish
        (
            Message request, 
            ServerCallContext context)
        {
            // add a published message to every subscription
            foreach (Subscription subscription in _subscriptions)
            {
                subscription.AddMessage(request.Msg);
            }

            // return PublishConfirmed reply
            return new PublishConfirmed();
        }

        // Subscribe implementation
        public override async Task Subscribe
        (
            SubscribeRequest request, 
            IServerStreamWriter<Message> responseStream, 
            ServerCallContext context)
        {
            // create subscription object for a client subscription
            Subscription subscription = new Subscription();

            // add subscription to the list of subscriptions
            _subscriptions.Add(subscription);
            
            // subscription loop
            while (true)
            {
                try
                {
                    // take message one by one from subscription 
                    string msg = subscription.TakeMessage(context.CancellationToken);

                    // create Message reply
                    Message message = new Message { Msg = msg };

                    // write the message into the output stream. 
                    await responseStream.WriteAsync(message);
                }
                catch when(context.CancellationToken.IsCancellationRequested)
                {
                    // if subscription is cancelled, break the loop
                    break;
                }
            }

            // once the subscription is broken, remove it 
            // from the list of subscriptions
            _subscriptions.Remove(subscription);
        }
    }
}
