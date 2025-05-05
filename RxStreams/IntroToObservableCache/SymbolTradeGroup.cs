using System.Reactive.Linq;
using DynamicData;

namespace IntroToObservableCache;

// class representing trades grouping
// by Symbol
public class SymbolTradeGroup : IDisposable
{
    // Dynamic Data group
    IGroup<Trade, int, Symbol> _group;

    // trades within the group
    public IEnumerable<Trade> Trades => _group.Cache.Items;

    // group key
    public Symbol TheSymbol => _group.Key;

    // sum of TotalTradePrice across all
    // the trades within the group
    public decimal TotalPrice { get; private set; }


    private IDisposable? _disposableSubscription;
    public void Dispose()
    {
        // destroy the group cache
        _group.Cache.Dispose();

        // remove the aggregation subscription
        _disposableSubscription?.Dispose();
        _disposableSubscription = null;
    }


    public SymbolTradeGroup(IGroup<Trade, int, Symbol> group)
    {
        _group = group;

        // set up the TotalPrice to be 
        // dynamically calculated when the group
        // or individual trades are changed
        _disposableSubscription =
            _group
                .Cache
                .Connect()
                .ToCollection()
                .Select(collection => collection.Sum(t => t.TotalTradePrice))
                .Subscribe(sum => this.TotalPrice = sum);
    }
}