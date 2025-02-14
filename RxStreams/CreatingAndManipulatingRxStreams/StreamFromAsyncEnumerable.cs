using Xunit;
using System.Linq;
using System.Reactive.Linq;

namespace CreatingAndManipulatingRxStreams;

public static class StreamFromAsyncEnumerable
{
    // returns an IAsyncEnumerable stream
    // of numbers from 1 to 6; each number
    // takes one second to emit.
    public static async IAsyncEnumerable<int> 
        GenerateAsyncEnumerable()
    {
        for (int i = 1; i <= 6; i++)
        {
            // one second delay
            await Task.Delay(TimeSpan.FromSeconds(1));

            yield return i;
        }
    }


    [Fact]
    public static async Task TestStreamFromAsyncEnumerable()
    {
        // Get asyncEnumerable
        IAsyncEnumerable<int> asyncEnumerable = 
            GenerateAsyncEnumerable();

        // convert to an observable
        // stream
        IObservable<int> observable = 
            asyncEnumerable.ToObservable();

        // create a result list to be populated
        // within the observable's Subscription
        var resultEvenSquaresCollection = new List<int>();

        // define completed flag
        bool completed = false;

        // subscribe to receive
        // squares of even numbers
        using var subscribeDisposable =
            observable

                // filter in only even numbers
                .Where(i => i % 2 == 0) 

                // transform i to square(i)
                .Select(i => i * i)
                .Subscribe
                (
                    onNext:
                        i => resultEvenSquaresCollection.Add(i),

                    onCompleted:
                        () => completed = true
                );

        // delay the testing until completed flag is 
        // switched to true
        while (!completed)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        // assert that the result as expected
        Assert.True
        (
            resultEvenSquaresCollection
                .SequenceEqual([2 * 2, 4 * 4, 6 * 6])
        );
    }
}
