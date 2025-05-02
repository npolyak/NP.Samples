using DynamicData.Binding;
using System.Drawing;

namespace IntroToObservableCache;

public class Trade : AbstractNotifyPropertyChanged
{
    private static int _globalTradeId = 0;

    public int GlobalTradeId { get; } = ++_globalTradeId;

    // primary key
    public int TradeId { get; set; }

    public Symbol TheSymbol { get; }

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