using DynamicData.Binding;
using System.ComponentModel;
using Xunit;

namespace CreatingAndManipulatingRxStreams;

public static class StreamFromNotifiableProperty
{
    // class with single notifiable property 
    class PropContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        bool _notifiableProperty = false;
        public bool NotifiableProperty
        {
            get => _notifiableProperty;

            set
            {
                if (_notifiableProperty == value)
                {
                    return;
                }

                _notifiableProperty = value;

                // fire PropertyChanged event 
                // if the property changes.
                PropertyChanged?.Invoke
                (
                    this, 
                    new PropertyChangedEventArgs(nameof(NotifiableProperty))
                );
            }
        }
    }

    [Fact]
    public static void TestNotifyablePropertyValueChange ()
    {
        // create an object containing a notifiable property
        var propContainer = new PropContainer ();

        // use DynamicData's 
        // NotifyPropertyChangedEx.WhenValueChanged(...)
        // method to convert the property to IObservable<bool>
        IObservable<bool> whenPropChangedObservable = 
            propContainer.WhenValueChanged
            (
                pContainer => pContainer.NotifiableProperty
            );

        var currentPropValue = propContainer.NotifiableProperty;

        // subscribe to change currentPropValue
        // every time the property changes via the observable
        using var subscriptionDisposable = 
            whenPropChangedObservable
                .Subscribe(newValue => currentPropValue = newValue);

        // test that the currentPropValue (obtained through 
        // the observable) really matches the current property.
        Assert.Equal
        (
            currentPropValue, 
            propContainer.NotifiableProperty
        );

        propContainer.NotifiableProperty = true;
        Assert.Equal
        (
            currentPropValue, 
            propContainer.NotifiableProperty
        );

        propContainer.NotifiableProperty = false;
        Assert.Equal
        (
            currentPropValue, 
            propContainer.NotifiableProperty
        );
    }
}
