using System.Collections.Concurrent;


namespace StreamingRelayServer
{
    // represents a single client subscription
    internal class Subscription
    {
        private BlockingCollection<string> _messages = 
            new BlockingCollection<string>();

        // add a message to the _messages collection
        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        // remove the first message from the _messages collection
        // If there are no message in the collection, TakeMessage will wait
        // blocking the thread. 
        public string TakeMessage(CancellationToken cancellationToken)
        {
            return _messages.Take(cancellationToken);
        }
    }
}
