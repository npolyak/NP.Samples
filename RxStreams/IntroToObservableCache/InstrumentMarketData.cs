namespace IntroToObservableCache;

public class InstrumentMarketData(Symbol symbol, string issuerName, decimal marketPrice)
{
    public Symbol TheSymbol { get; } = symbol;

    public string IssuerName { get; } = issuerName;

    public decimal MarketPrice { get; } = marketPrice;
}
