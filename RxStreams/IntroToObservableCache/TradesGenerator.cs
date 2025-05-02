namespace IntroToObservableCache;

public static class TradesGenerator
{
    static int _tradeId = 0;

    public static int CurrentTradeId => _tradeId;

    internal static Trade CreateTrade
    (
        this Symbol symbol,
        decimal totalTradeAmount,
        int tradeId = -1
    )
    {
        int intDelta = Random.Shared.Next(-5, 5);

        if (tradeId < 0)
        {
            tradeId = ++_tradeId;
        }
        Trade newTrade =
            new Trade
            (
                tradeId,
                symbol,
                totalTradeAmount
            );

        return newTrade;
    }

    public static void ResetId()
    {
        _tradeId = 0;
    }
}
