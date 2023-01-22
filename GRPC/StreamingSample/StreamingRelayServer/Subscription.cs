using System.Collections.Concurrent;


namespace StreamingRelayServer
{
    internal class Subscription
    {
        private BlockingCollection<string> _messages = new BlockingCollection<string>();

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public string TakeMessage(CancellationToken cancellationToken)
        {
            return _messages.Take(cancellationToken);
        }
    }
}
