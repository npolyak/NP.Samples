using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;
using Xunit.Sdk;

// for short
using DD = DynamicData;

namespace IntroToDynamicData;

public static class SimpleExamples
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

        // create the target collection
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


        // make sure the target collection is correct
        Assert.True(targetInts.SequenceEqual([6 * 6, 4 * 4, 2 * 2]));

        // remove 2 (even int) from source collection
        sourceInts.Remove(2);

        // make sure the target collection is correct
        Assert.True(targetInts.SequenceEqual([6 * 6, 4 * 4]));


        // insert 3 integers 20, 21 and 22 in the
        // source collection. Notice that only even numbers
        // 20 and 22 will influence the 
        // target collection.
        sourceInts.Insert(0, 20);
        sourceInts.Insert(1, 21);
        sourceInts.Insert(2, 22);

        // make sure the target collection is correct
        Assert.True(targetInts.SequenceEqual([22 * 22, 20 * 20, 6 * 6, 4 * 4]));

        // empty out the source collection
        sourceInts.Clear();

        // check that the target collection
        // has also been emptied out
        Assert.Empty(targetInts);
    }

    [Fact]
    public static void DynamicFilterTest()
    {
        // create source collection and populate
        // it with numbers 1 to 6.
        ObservableCollection<int> sourceInts =
            new ObservableCollection<int>(Enumerable.Range(1, 6));

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<int>> changeSetStream =
            sourceInts.ToObservableChangeSet();

        // create the target collection
        IObservableCollection<int> targetInts =
            new ObservableCollectionExtended<int>();

        Subject<Func<int, bool>> filter = 
            new Subject<Func<int, bool>>();

        IObservable<IChangeSet<int>> resultObservable =
            changeSetStream
                // filter only even ints
                .Filter(filter)
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

        // add filter only after the subscription
        // filter in only even numbers
        filter.OnNext(i => i % 2 == 0);

        // make sure the target collection is correct
        Assert.True(targetInts.SequenceEqual([6 * 6, 4 * 4, 2 * 2]));

        // filter in only number divisible by 3
        filter.OnNext(i => i % 3 == 0);

        // make sure the target collection is correct
        Assert.True(targetInts.SequenceEqual([6 * 6, 3 * 3]));

        // add a value 9 (divisible by 3) to the source sequence.
        sourceInts.Add(9);

        Assert.True(targetInts.SequenceEqual([9 * 9, 6 * 6, 3 * 3]));
    }

    [Fact]
    public static void TotalSumAggregationTest()
    {
        // create source collection and populate
        // it with numbers 1 to 3.
        ObservableCollection<int> sourceInts =
            new ObservableCollection<int>(Enumerable.Range(1, 3));

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<int>> changeSetStream =
            sourceInts.ToObservableChangeSet();

        // create the collection sum observable
        IObservable<int> sumObservable = changeSetStream.Sum(i => i);

        // create a variable to hold the sum
        int sumResult = 0;

        // subscribe to changes in total sum
        // first time it fires after the subscription.
        using IDisposable disposableSubscription =
            sumObservable.Subscribe(result => sumResult = result);

        // 1 + 2 + 3 = 6
        Assert.Equal(6, sumResult);

        // add some numbers
        sourceInts.AddRange([5, 10, 15]);

        // 1 + 2 + 3 + 5 + 10 + 15 = 36
        Assert.Equal(36, sumResult);

        // remove all numbers
        sourceInts.Clear();

        // test that we obtain 0 as a result
        // of removing everything
        Assert.Equal(0, sumResult);
    }

    [Fact]
    public static void GroupingTest()
    {
        // create source collection and populate
        // it with numbers 1, 3 and 5 (at this point only odd numbers).
        ObservableCollection<int> sourceInts =
            new ObservableCollection<int>([1, 3, 5]);

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<int>> changeSetStream =
            sourceInts.ToObservableChangeSet();

        // group the numbers using GroupWithImmutableState operator
        // by the remainder from division by 2 (odd vs even)
        IObservable<IChangeSet<DD.List.IGrouping<int, int>>> groupedObservable =
            changeSetStream.GroupWithImmutableState(i => i % 2);

        // create the grouped collection
        using IObservableList<DD.List.IGrouping<int, int>> groupedCollection =
            groupedObservable
                .AsObservableList();

        // grouped collection only has one group of odd numbers
        // since there are only odd number in the source collection:
        Assert.True(groupedCollection.Count == 1);

        // get odd group
        DD.List.IGrouping<int, int> oddGroup =
            groupedCollection.Items.Single(grouping => grouping.Key == 1);

        Assert.True(oddGroup.Items.SequenceEqual([1, 3, 5]));

        // add even numbers 2, 4, 6 to the source collection
        sourceInts.AddRange([2, 4, 6]);

        // now there should be two groups of numbers - odd and even
        Assert.True(groupedCollection.Count == 2);

        // get odd group
        oddGroup = 
            groupedCollection.Items.Single(grouping => grouping.Key == 1);

        // check that the numbers in the odd group are [1, 3, 5]
        Assert.True(oddGroup.Items.SequenceEqual([1, 3, 5]));

        // get event group
        DD.List.IGrouping<int, int> evenGroup =
            groupedCollection.Items.Single(grouping => grouping.Key == 0);

        // check that the numbers in the even group are [2, 4, 6]
        Assert.True(evenGroup.Items.SequenceEqual([2, 4, 6]));
    }
}
