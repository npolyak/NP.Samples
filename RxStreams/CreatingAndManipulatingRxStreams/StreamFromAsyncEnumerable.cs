using Xunit;
using System.Linq;
using System.Reactive.Linq;

namespace CreatingAndManipulatingRxStreams;

public static class StreamFromAsyncEnumerable
{
    public static async IAsyncEnumerable<int> 
        GenerateAsyncEnumerable()
    {
        for (int i = 1; i <= 10; i++)
        {
            // one second delay
            await Task.Delay(TimeSpan.FromSeconds(1));

            yield return i;
        }
    }


    [Fact]
    public static async Task TestStreamFromAsyncEnumerable()
    {
        IAsyncEnumerable<int> asyncEnumerable = 
            GenerateAsyncEnumerable();

        IObservable<int> observable = 
            asyncEnumerable.ToObservable();

        var resultEvenSquaresCollection = new List<int>();
        bool completed = false;

        using var subscribeDisposable =
            observable
                .Where(i => i % 2 == 0)
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
    }
}
