using DynamicData.Binding;
using System.Drawing;

namespace IntroToObservableCache;

public class Trade : 
    AbstractNotifyPropertyChanged
{
    // primary key (used for distinguishing 
    // between the new entries and the 
    // updates_)
    public int TradeId { get; }

    // Stock Symbol
    public Symbol TheSymbol { get; }

    // updatable total trade prices
    decimal _totalTradePrice;
    public decimal TotalTradePrice 
    {
        get => _totalTradePrice; 

        // SetAndRaise fires PropertyChanged event
        // when TotalTradePrice property changes
        set => SetAndRaise(ref _totalTradePrice, value);
    }

    public Trade
    (
        int tradeId,
        Symbol symbol,
        decimal totalTradeAmount)
    {
        this.TradeId = tradeId;
        this.TheSymbol = symbol;
        this.TotalTradePrice = totalTradeAmount;
    }

    public override int GetHashCode()
    {
        return TradeId.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Trade trade)
        {
            return this.TradeId.Equals(trade.TradeId) && 
                   (this.TheSymbol == trade.TheSymbol) && 
                   (this.TotalTradePrice == trade.TotalTradePrice);
        }

        return false;
    }
}