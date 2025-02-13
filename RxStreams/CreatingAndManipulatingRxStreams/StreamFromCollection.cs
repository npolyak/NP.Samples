using System.Reactive.Linq;
using Xunit;

namespace CreatingAndManipulatingRxStreams;

public class StreamFromCollection
{
    [Fact]
    public static void TestStreamFromCollection()
    {
        // source collection consists of int numbers 1 to 10
        int[] sourceCollection1_10 = Enumerable.Range(1, 10).ToArray();

        IObservable<int> sourceObservable = sourceCollection1_10.ToObservable();

        // observable emitting squares for only even numbers
        // out of the source
        IObservable<int> evenSquaresObservable =
               sourceObservable
                .Where(i => i % 2 == 0) // filter only even numbers
                .Select(i => i * i);    // transform i to square(i)


        // create result collection
        var resultEvenSquaresCollection = new List<int>();

        // populate result collection by subscribing
        // to it.
        // Note that before subscription, the observables
        // however complex they are do not consume any
        // heap resources and cannot result in a memory leak.
        // Only subscription needs to be dispose of 
        // (which we do automatically by providing the 'using' clause).
        using var subscriptionDisposable =
            evenSquaresObservable
                .Subscribe(i => resultEvenSquaresCollection.Add(i));

        // create collection of squares of even positive ints
        // smaller or equal to 10 to compare the
        // result with
        int[] expectedResultsCollection =
            [2 * 2, 4 * 4, 6 * 6, 8 * 8, 10 * 10];

        // assert that the result and expectedResults collections
        // are the same.
        Assert.True(resultEvenSquaresCollection.SequenceEqual(expectedResultsCollection));
    }
}
