using Microsoft.AspNetCore.SignalR.Client;

namespace Chatroom_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            window.Destroying += OnWindowDestroying;
            return window;
        }

        private async void OnWindowDestroying(object sender, EventArgs e)
        {
            if (AppState.CurrentUser != null)
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5170/chathub")
                    .Build();

                await hubConnection.StartAsync();
                await hubConnection.InvokeAsync("DisconnectUser", AppState.CurrentUser.Username);
                await AppState.CurrentUser.Disconnect();
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
            }
        }
    }
}