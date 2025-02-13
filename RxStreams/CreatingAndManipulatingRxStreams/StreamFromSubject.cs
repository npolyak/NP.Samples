using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace CreatingAndManipulatingRxStreams;

public static class StreamFromSubject
{
    [Fact]
    public static void TestStreamFromSubject()
    {
        ISubject<int> subject = new Subject<int>();

        var resultEvenSquaresCollection = new List<int>();
        bool completed = false;
        string errorMessage = null;

        using var subscriptionDisposable =
            subject
                .Where(i => i % 2 == 0) // filter only even
                .Select(i => i * i) // get squares
                .Subscribe
                (
                    onNext: i => resultEvenSquaresCollection.Add(i), 
                    onCompleted: () => completed = true,
                    onError: exception => errorMessage = exception.Message
                );

        // push data into the stream
        // by using OnNext() method
        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnNext(3);
        subject.OnNext(4);

        // completion signal has not been sent yet
        Assert.False(completed);    

        // no exception occurred yet
        Assert.Null(errorMessage);

        Assert.True
        (
            resultEvenSquaresCollection
                .SequenceEqual([2 * 2, 4 * 4])
        );

        // add another even number
        subject.OnNext(10);

        // verify the output of the stream
        Assert.True
        (
            resultEvenSquaresCollection
                .SequenceEqual([2 * 2, 4 * 4, 10 * 10])
        );

        // no completion yet
        Assert.False(completed);

        // signal source stream completion
        subject.OnCompleted();

        // the completed flag should
        // be reset to True
        Assert.True(completed);
    }

    [Fact]
    public static void TestStreamFromSubjectWithException()
    {
        ISubject<int> subject = new Subject<int>();

        var resultEvenSquaresCollection = new List<int>();
        bool completed = false;
        string errorMessage = null;

        using var subscriptionDisposable =
            subject
                .Where(i => i % 2 == 0) // filter only even
                .Select(i => i * i) // get squares
                .Subscribe
                (
                    onNext: i => resultEvenSquaresCollection.Add(i),
                    onCompleted: () => completed = true,
                    onError: exception => 
                                errorMessage = exception.Message
                );

        // push data into the stream
        // by using OnNext() method
        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnNext(3);
        subject.OnNext(4);

        // completion signal has not been sent yet
        Assert.False(completed);

        // no exception occurred yet
        Assert.Null(errorMessage);

        Assert.True
        (
            resultEvenSquaresCollection
                .SequenceEqual([2 * 2, 4 * 4])
        );

        // add another even number
        subject.OnNext(10);

        // verify the output of the stream
        Assert.True
        (
            resultEvenSquaresCollection
                .SequenceEqual([2 * 2, 4 * 4, 10 * 10])
        );

        // no completion yet
        Assert.False(completed);
        
        // no exception occurred yet
        Assert.Null(errorMessage);

        // signal error
        subject.OnError(new Exception("Error Occurred"));

        // verify that our onError action worked
        Assert.True(errorMessage == "Error Occurred");
    }
}
