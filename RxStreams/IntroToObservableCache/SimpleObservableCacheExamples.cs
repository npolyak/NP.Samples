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

namespace IntroToObservableCache;

public static class SimpleObservableCacheExamples
{
    private static void Verify
    (
        this IEnumerable<Trade> sourceTradeCollection,
        IEnumerable<Trade> targetCollection, 
        Func<Trade, bool> sourceFilter
    )
    {
        var filteredAndOrdered =
            sourceTradeCollection
            .Where(sourceFilter)
            .OrderBy(t => t.TotalTradePrice)
            .ToList();

        Assert.True(filteredAndOrdered.SequenceEqual(targetCollection));
    }

    [Fact]
    public static void FilterSortAndBindTest()
    {
        // create sourceCache
        ISourceCache<Trade, int>? sourceCache =
            new SourceCache<Trade, int>(t => t.TradeId);

        var metaTrade1 = Symbol.META.CreateTrade(2000);
        sourceCache.AddOrUpdate(metaTrade1);

        var oracleTrade1 = Symbol.ORCL.CreateTrade(1000);
        sourceCache.AddOrUpdate(oracleTrade1);

        var metaTrade2 = Symbol.META.CreateTrade(1900);
        sourceCache.AddOrUpdate(metaTrade2);

        var oracleTrade2 = Symbol.ORCL.CreateTrade(900);
        sourceCache.AddOrUpdate(oracleTrade2);

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<Trade, int>> changeSetStream =
            sourceCache!.Connect();

        // create the target collection
        IObservableCollection<Trade> targetCollection =
            new ObservableCollectionExtended<Trade>();

        IObservable<IChangeSet<Trade, int>> resultObservable =
            changeSetStream
                .Filter(t => t.TheSymbol == Symbol.ORCL)
                .SortAndBind
                (
                    targetCollection,
                    SortExpressionComparer<Trade>.Ascending(t => t.TotalTradePrice)
                );

        // now subscribe to start pulling data
        // using clause will dispose the subscription
        using var subscription = resultObservable.Subscribe();

        IEnumerable<Trade> sourceTradeCollection =
            [metaTrade1, oracleTrade1, metaTrade2, oracleTrade2];

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        // create another entry with the same TradeId key
        // as oracleTrade1 and add it to the sourceCache
        // to override the previous oracleTrade1 entry
        var oracleTrade1_modified =
            Symbol.ORCL.CreateTrade(100, oracleTrade1.TradeId);
        sourceCache.AddOrUpdate(oracleTrade1_modified);

        sourceTradeCollection =
            [metaTrade1, oracleTrade1_modified, metaTrade2, oracleTrade2];

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        var oracleTrade3 =
            Symbol.ORCL.CreateTrade(50);
        sourceCache.AddOrUpdate(oracleTrade3);

        sourceTradeCollection =
            [
                metaTrade1,
                oracleTrade1_modified,
                metaTrade2,
                oracleTrade2,
                oracleTrade3
            ];
        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);
    }

    [Fact]
    public static void 
        FilterSortAndBindFromObservableCollectionTest()
    {
        var metaTrade1 = Symbol.META.CreateTrade(2000);

        var oracleTrade1 = Symbol.ORCL.CreateTrade(1000);

        var metaTrade2 = Symbol.META.CreateTrade(1900);;

        var oracleTrade2 = Symbol.ORCL.CreateTrade(900);

        ObservableCollection<Trade> sourceTradeCollection =
            new ObservableCollection<Trade>
            {
                metaTrade1, oracleTrade1, metaTrade2, oracleTrade2
            };

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<Trade, int>> changeSetStream =
            sourceTradeCollection.ToObservableChangeSet(t => t.TradeId);

        // create the target collection
        IObservableCollection<Trade> targetCollection =
            new ObservableCollectionExtended<Trade>();

        IObservable<IChangeSet<Trade, int>> resultObservable =
            changeSetStream
                .Filter(t => t.TheSymbol == Symbol.ORCL)
                .AutoRefresh()
                .SortAndBind
                (
                    targetCollection,
                    SortExpressionComparer<Trade>.Ascending(t => t.TotalTradePrice)
                );

        // now subscribe to start pulling data
        // using clause will dispose the subscription
        using var subscription = resultObservable.Subscribe();

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        oracleTrade1.TotalTradePrice = 100;

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        // add another oracle trade
        var oracleTrade3 =
            Symbol.ORCL.CreateTrade(50);

        sourceTradeCollection.Add(oracleTrade3);

        sourceTradeCollection
           .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        // remove all oracle entries
        sourceTradeCollection.Remove(oracleTrade1);
        sourceTradeCollection.Remove(oracleTrade2);
        sourceTradeCollection.Remove(oracleTrade3);

        // the target collection should become empty
        Assert.True(targetCollection.Count == 0);
    }


    [Fact]
    public static void
        DynamicFilterTest()
    {
        var metaTrade1 = Symbol.META.CreateTrade(2000);

        var oracleTrade1 = Symbol.ORCL.CreateTrade(1000);

        var metaTrade2 = Symbol.META.CreateTrade(1900); ;

        var oracleTrade2 = Symbol.ORCL.CreateTrade(900);

        ObservableCollection<Trade> sourceTradeCollection =
            new ObservableCollection<Trade>
            {
                metaTrade1, oracleTrade1, metaTrade2, oracleTrade2
            };

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<Trade, int>> changeSetStream =
            sourceTradeCollection.ToObservableChangeSet(t => t.TradeId);

        Subject<Func<Trade, bool>> filterObservable = 
            new Subject<Func<Trade, bool>>();

        // create the target collection
        IObservableCollection<Trade> targetCollection =
            new ObservableCollectionExtended<Trade>();

        IObservable<IChangeSet<Trade, int>> resultObservable =
            changeSetStream
                .Filter(filterObservable)
                .AutoRefresh()
                .SortAndBind
                (
                    targetCollection,
                    SortExpressionComparer<Trade>.Ascending(t => t.TotalTradePrice)
                );

        // now subscribe to start pulling data
        // using clause will dispose the subscription
        using var subscription = resultObservable.Subscribe();

        filterObservable.OnNext(t => t.TheSymbol == Symbol.ORCL);

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        oracleTrade1.TotalTradePrice = 100;

        sourceTradeCollection
            .Verify(targetCollection, t => t.TheSymbol == Symbol.ORCL);

        // clear the source collection of oracle 
        // entries
        sourceTradeCollection.Remove(oracleTrade1);
        sourceTradeCollection.Remove(oracleTrade2);

        Assert.True(targetCollection.Count == 0);

        // change filter to meta
        filterObservable.OnNext(t => t.TheSymbol == Symbol.META);

        // we should get some META entries in the target Collection
        // right away
        Assert.True(targetCollection.Count > 0);
    }

    [Fact]
    public static void GroupingTest()
    {
        var metaTrade1 = Symbol.META.CreateTrade(2000);

        var oracleTrade1 = Symbol.ORCL.CreateTrade(1000);

        var metaTrade2 = Symbol.META.CreateTrade(1900); ;

        var oracleTrade2 = Symbol.ORCL.CreateTrade(900);

        ObservableCollection<Trade> sourceTradeCollection =
            new ObservableCollection<Trade>
            {
                metaTrade1, oracleTrade1, metaTrade2, oracleTrade2
            };

        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<Trade, int>> changeSetStream = 
            sourceTradeCollection.ToObservableChangeSet(t => t.TradeId);

        // do the grouping
        IObservable<IGroupChangeSet<Trade, int, Symbol>>
            groupedObservable =
                changeSetStream
                    .AutoRefresh()
                    .GroupOnProperty(t => t.TheSymbol);

        // transform the grouped entries into 
        // SymbolTradeGroup objects
        var transformedGroups =
            groupedObservable.Transform(g => new SymbolTradeGroup(g));

        ReadOnlyObservableCollection<SymbolTradeGroup>? symbolTradeGroups = null;

        // create and populate an observable collection
        // symbolTradeGroups that contains those SymbolTradeGroup
        // objects
        using IDisposable subscription =
            transformedGroups
                .Bind(out symbolTradeGroups) // create and populate
                .DisposeMany() // make sure that if an item is removed
                               // from the collection, it is disposed
                .Subscribe();  // start the subscription

        // Assert there are two groups
        // (one for ORCL and the other for META trades)
        Assert.True(symbolTradeGroups.Count == 2);

        // get the orclGroup and assert it exists and single
        SymbolTradeGroup orclGroup = 
            symbolTradeGroups.Single(g => g.TheSymbol == Symbol.ORCL);

        Assert.True(orclGroup.Trades.Count() == 2);
        Assert
            .True(orclGroup.TotalPrice == orclGroup.Trades.Sum(t => t.TotalTradePrice));


        // get the metaGroup and assert it exists and single
        SymbolTradeGroup metaGroup =
            symbolTradeGroups.Single(g => g.TheSymbol == Symbol.META);

        Assert.True(metaGroup.Trades.Count() == 2);
        Assert
            .True(metaGroup.TotalPrice == metaGroup.Trades.Sum(t => t.TotalTradePrice));

        // change the 
        oracleTrade1.TotalTradePrice = 5000;
        // assert that the aggregation's total price got updated
        Assert
            .True
            (orclGroup.TotalPrice == orclGroup.Trades.Sum(t => t.TotalTradePrice));

        // add another oracle trade to the source collection
        var oracleTrade3 = Symbol.ORCL.CreateTrade(7000);
        sourceTradeCollection.Add(oracleTrade3);

        // make sure that number of traded without orcl group
        // is now 3 and that the total price of the aggregation
        // got also updated
        Assert.True(orclGroup.Trades.Count() == 3);
        Assert
            .True(orclGroup.TotalPrice == orclGroup.Trades.Sum(t => t.TotalTradePrice));

        // add a trade for another instrument (e.g. Symbol.TSLA)
        // to the source collection
        var tslaTrade1 = Symbol.TSLA.CreateTrade(10);
        sourceTradeCollection.Add(tslaTrade1);

        // make sure that the number of groups is now 3:
        Assert.True(symbolTradeGroups.Count() == 3);

        // remove all Oracle trades from the source collection
        sourceTradeCollection.Remove(oracleTrade1);
        sourceTradeCollection.Remove(oracleTrade2);
        sourceTradeCollection.Remove(oracleTrade3);

        // make sure that the corresponding group is also removed
        Assert.True(symbolTradeGroups.Count() == 2);
        Assert.True
        (
            symbolTradeGroups
                .Where(group => group.TheSymbol == Symbol.ORCL)
                .Count() == 0);
    }
}

