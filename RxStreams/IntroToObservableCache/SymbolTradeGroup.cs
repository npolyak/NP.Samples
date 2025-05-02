using System.Reactive.Linq;
using DynamicData;

namespace IntroToObservableCache;

public class SymbolTradeGroup : IDisposable
{
    IGroup<Trade, int, Symbol> _group;

    public IEnumerable<Trade> Trades => _group.Cache.Items;

    public Symbol TheSymbol => _group.Key;

    public decimal TotalPrice { get; private set; }


    private IDisposable _disposableSubscription;
    public void Dispose()
    {
        _group.Cache.Dispose();

        _disposableSubscription?.Dispose();
        _disposableSubscription = null;
    }

    public SymbolTradeGroup(IGroup<Trade, int, Symbol> group)
    {
        _group = group;

        _disposableSubscription =
            _group
                .Cache
                .Connect()
                .ToCollection()
                .Select(collection => collection.Sum(t => t.TotalTradePrice))
                .Subscribe(sum => this.TotalPrice = sum);

    }
}