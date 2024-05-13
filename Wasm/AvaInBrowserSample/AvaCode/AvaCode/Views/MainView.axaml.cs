using Avalonia.Controls;

namespace AvaCode.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        ChangeTextButton.Click += ChangeTextButton_Click;
    }

    private void ChangeTextButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (GreetingTextBlock.Text?.StartsWith("Hello") == true)
        {
            GreetingTextBlock.Text = "Hi from Avalonia in-Browser!!!";
        }
        else
        {
            GreetingTextBlock.Text = "Hello from Avalonia in-Browser!!!";
        }
    }
}