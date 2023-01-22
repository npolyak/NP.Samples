using Grpc.Core;
using Service;
using static Service.RelayService;

namespace StreamingRelayServer
{
    internal class RelayServiceImplementations : RelayServiceBase
    {
        List<Subscription> _subscriptions = new List<Subscription>();

        public override async Task<PublishConfirmed> Publish
        (
            Message request, 
            ServerCallContext context)
        {
            foreach(Subscription subscription in _subscriptions)
            {
                subscription.AddMessage(request.Msg);
            }

            return new PublishConfirmed();
        }

        public override async Task Subscribe
        (
            SubscribeRequest request, 
            IServerStreamWriter<Message> responseStream, 
            ServerCallContext context)
        {
            Subscription subscription = new Subscription();

            _subscriptions.Add(subscription);
            
            while (true)
            {
                try
                {
                    string msg = subscription.TakeMessage(context.CancellationToken);

                    Message message = new Message { Msg = msg };

                    await responseStream.WriteAsync(message);
                }
                catch when(context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _subscriptions.Remove(subscription);
        }
    }
}
