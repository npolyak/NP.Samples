using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using Xunit;
using Xunit.Sdk;

namespace IntroToDynamicData;

public static class SimpleExamplesFixture
{

    [Fact]
    public static void IntTransformFilteringAndSorting()
    {
        // create source collection and populate
        // it with numbers 1 to 6.
        ObservableCollection<int> sourceInts = 
            new ObservableCollection<int>(Enumerable.Range(1,6));

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<int>> changeSetStream = 
            sourceInts.ToObservableChangeSet();

        IObservableCollection<int> targetInts = 
            new ObservableCollectionExtended<int>();

        IObservable<IChangeSet<int>> resultObservable =
            changeSetStream
                // filter only even ints
                .Filter(i => i % 2 == 0)
                // sort descending
                .Sort(SortExpressionComparer<int>.Descending(i => i))
                // transform to squares
                .Transform(i => i * i)
                // bind the target collection to update
                .Bind(targetInts);

        // now subscribe to start pulling data
        // using clause will dispose the subscription
        using IDisposable subscribeDisposable =
            resultObservable.Subscribe();


        //using IDisposable subscriptionDisposable =
        //    changeSetStream
        //    .Filter(i => i % 2 == 0)
        //    .Sort(SortExpressionComparer<int>.Descending(i => i))
        //    .Transform(i => i * i)
        //    .Bind(targetInts)
        //    .Subscribe();

        Assert.True(targetInts.SequenceEqual([6 * 6, 4 * 4, 2 * 2]));

        // remove 2 (even int) from source collection
        sourceInts.Remove(2);

        Assert.True(targetInts.SequenceEqual([6 * 6, 4 * 4]));


        // insert 3 integers 20, 21 and 22 in the
        // source collection. Notice that only even numbers
        // 20 and 22 will influence the 
        // target collection.
        sourceInts.Insert(0, 20);
        sourceInts.Insert(1, 21);
        sourceInts.Insert(2, 22);

        Assert.True(targetInts.SequenceEqual([22 * 22, 20 * 20, 6 * 6, 4 * 4]));
    }
}
