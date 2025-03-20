using System.Collections.ObjectModel;

namespace Chatroom_MAUI;

public partial class MainPage : ContentPage
{
    // Observable collection binding for the user list
    public ObservableCollection<string> Users { get; set; }

    public MainPage()
    {
        InitializeComponent();

        // Example list of users, replace with dynamic data as needed
        Users = new ObservableCollection<string>
            {
                "Alice", "Bob", "Charlie"
            };

        // Set the BindingContext for data binding
        BindingContext = this;
    }

    private void SendButton_Clicked(object sender, EventArgs e)
    {
        SendMessage();
    }

    private void MessageEntry_Completed(object sender, EventArgs e)
    {
        SendMessage();
    }

    private void SendMessage()
    {
        var messageText = MessageEntry.Text?.Trim();

        if (!string.IsNullOrEmpty(messageText))
        {
            // Add the message to the ChatMessages StackLayout as a Label with white text color.
            ChatMessages.Children.Add(new Label
            {
                Text = messageText,
                FontSize = 16,
                TextColor = Colors.White // Set the text color to white
            });

            // Optionally clear the message entry.
            MessageEntry.Text = string.Empty;
        }
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///Login");
    }
}