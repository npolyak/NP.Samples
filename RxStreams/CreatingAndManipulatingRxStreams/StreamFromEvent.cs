using System.Reactive.Linq;
using Xunit;

namespace CreatingAndManipulatingRxStreams;

public static class StreamFromEvent
{
    class EventContainingClass
    {
        // boolean event
        public event Action<bool>? TestEvent;

        // fire event
        public void FireTestEvent(bool arg)
        {
            TestEvent?.Invoke(arg);
        }
    }

// extension method that 
// gets the observable for the TestEvent
// of the EventContainingClass passed as
// the argument.
private static IObservable<bool> 
    GetObservableFromEventObj(this EventContainingClass eventObj)
{
    // method Observable.FromEvent(...)
    // is used to create an observable 
    // from an event. 
    // The first two arguments to it
    // specify how to add and remove
    // an event handler. 
    IObservable<bool> observable =
        Observable
            .FromEvent<Action<bool>, bool>
            (
            // add handler
            eventHandler =>
                eventObj.TestEvent += eventHandler,

            // remove handler
            eventHandler =>
                eventObj.TestEvent -= eventHandler
            );

    return observable;
}


    [Fact]
    public static void TestStreamFromEvent()
    {
        // create an object containing an
        // Action<bool> event
        EventContainingClass eventContainingObj = 
            new EventContainingClass();

        // create an observable from 
        // eventContainingObj.TestEvent
        IObservable<bool> observable =
            eventContainingObj.GetObservableFromEventObj();

        // boolean to specify 
        // the last argument to the event
        bool lastFired = false;

        // subscribe to change the lastFired
        // to the last argument passed to 
        // fire the event
        using var subscriptionDisposable =
            observable.Subscribe(b => lastFired = b);

        // false originally
        Assert.False(lastFired);

        // after true is fired, lastFired 
        // should become true
        eventContainingObj.FireTestEvent(true);
        Assert.True(lastFired);

        // after another true is fired, lastFired 
        // should still be true
        eventContainingObj.FireTestEvent(true);
        Assert.True(lastFired);

        // after false is fired, lastFired 
        // should change to false
        eventContainingObj.FireTestEvent(false);
        Assert.False(lastFired);
    }

    [Fact]
    public static void TestStreamFromEventComposition()
    {
        // first object containing an event
        EventContainingClass eventContainingObj1 =
            new EventContainingClass();

        // observable obtained from the
        // first objects
        IObservable<bool> observable1 =
            eventContainingObj1.GetObservableFromEventObj();

        // second object containing the event
        EventContainingClass eventContainingObj2 =
            new EventContainingClass();

        // observable obtaine from the
        // second object
        IObservable<bool> observable2 =
            eventContainingObj2.GetObservableFromEventObj();

        // combine the two observables by using 
        // Observable.CombineLatest(...) extension 
        // method. This method fires every time any 
        // observable changes. 
        IObservable<bool> combinedLatestObservable =
            observable1
                .CombineLatest
                (
                    observable2,

                    // obtain the combined
                    // observable value by 
                    // doing boolean AND 
                    // operator on the two
                    // latest values from the
                    // two observables
                    resultSelector: (b1, b2) => b1 && b2
                );

        // get an observable to fire only if the
        // result changes from the previous value
        // (do not emit data if it stays the same)
        IObservable<bool> combinedLatestDistinctObservable =
            combinedLatestObservable.DistinctUntilChanged();

        // latest value from 
        // the resulting observable
        bool latestCompoundValue = false;

        // subscribe to set the
        // latestCompoundValue when 
        // the result changes
        using var subscriptionDisposable =
            combinedLatestDistinctObservable
                .Subscribe
                (
                    b => latestCompoundValue = b
                );

        Assert.False(latestCompoundValue);

        eventContainingObj1.FireTestEvent(true);
        eventContainingObj2.FireTestEvent(true);

        // since both observable fired true
        // the result should change to true
        Assert.True(latestCompoundValue);

        // one fire false - result
        // changes to false
        eventContainingObj1.FireTestEvent(false);
        Assert.False(latestCompoundValue);

        // both are false, result 
        // stays false
        eventContainingObj2.FireTestEvent(false);
        Assert.False(latestCompoundValue);

        // only one changed back to true
        // the result still stays false
        eventContainingObj1.FireTestEvent(true);
        Assert.False(latestCompoundValue);

        // both changed to true
        // the result changes to true
        eventContainingObj2.FireTestEvent(true);
        Assert.True(latestCompoundValue);
    }
}
