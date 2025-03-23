using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Chatroom_MAUI;

public partial class MainPage : ContentPage
{
    private User user;
    private HubConnection hubConnection;

    // Observable collection binding for the user list
    public ObservableCollection<string> Users { get; }
    public ObservableCollection<Message> Messages { get; }

    public MainPage()
    {
        InitializeComponent();

        user = AppState.CurrentUser;

        Users = new ObservableCollection<string>();
        Messages = new ObservableCollection<Message>();

        // Set the BindingContext for data binding
        BindingContext = this;

        InitializeSignalR();
    }

    private async void InitializeSignalR()
    {
        try
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://{AppState.ServerIp}/chathub")
                .Build();

            hubConnection.On<string, string, DateTime>("ReceiveMessage", (username, content, time) =>
            {
                Messages.Add(new Message(content) { Username = username, Time = time });
            });

            hubConnection.On<string>("UserConnected", (username) =>
            {
                if (!Users.Contains(username))
                {
                    Users.Add(username);
                }
            });

            hubConnection.On<string>("UserDisconnected", (username) =>
            {
                if (Users.Contains(username))
                {
                    Users.Remove(username);
                }
            });

            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("ConnectUser", user.Username);

            FetchUsers();
            FetchMessages();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing SignalR: {ex.Message}");
            // Optionally, display an alert to the user
            await DisplayAlert("Connection Error", "Unable to connect to the server. Please check the IP address and try again.", "OK");
        }
    }

    private async void FetchUsers()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://{AppState.ServerIp}/api/users");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user.Username);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching users: {ex.Message}");
            // Optionally, display an alert to the user
            await DisplayAlert("Connection Error", "Unable to fetch users. Please check the IP address and try again.", "OK");
        }
    }

    private async void FetchMessages()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://{AppState.ServerIp}/api/messages");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(json))
            {
                var messages = JsonSerializer.Deserialize<List<Message>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Messages.Clear();
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            }
            else
            {
                // Handle empty response scenario
                Console.WriteLine("No messages found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching messages: {ex.Message}");
            // Optionally, display an alert to the user
            await DisplayAlert("Connection Error", "Unable to fetch messages. Please check the IP address and try again.", "OK");
        }
    }

    private async void SendMessage()
    {
        var messageContent = MessageEntry.Text;
        if (string.IsNullOrWhiteSpace(messageContent))
        {
            return;
        }

        await user.SendMessage(messageContent.Trim());
        MessageEntry.Text = "";
    }

    private void SendButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
        {
            SendMessage();
        }
    }

    private void MessageEntry_Completed(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
        {
            SendMessage();
        }
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        if (hubConnection.State == HubConnectionState.Connected)
        {
            await hubConnection.InvokeAsync("DisconnectUser", user.Username);
            await user.Disconnect();
            await hubConnection.StopAsync();
        }
        await Shell.Current.GoToAsync("///Login");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (hubConnection.State == HubConnectionState.Disconnected)
        {
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("ConnectUser", user.Username);
            FetchUsers();
            FetchMessages();
        }
    }
}