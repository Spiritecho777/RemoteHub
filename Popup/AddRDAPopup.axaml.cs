using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RemoteHub.Popup;

public partial class AddRDAPopup : Window
{
    public AddRDAPopup()
    {
        InitializeComponent();
    }

    private void Advanced_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //var advancedPopup = new AdvancedRDAPopup();
        //advancedPopup.ShowDialog(this);
    }

    private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //this.Close(true);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //this.Close(false);
    }
}