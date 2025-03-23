using System.Net;
using System.Threading.Tasks;

namespace Chatroom_MAUI;

public partial class Login : ContentPage
{
    User user = new User("");

    public Login()
    {
        InitializeComponent();
    }

    private void ConnectButton_Clicked(object sender, EventArgs e)
    {
        Connect();
    }

    private void ServerIpEntry_Completed(object sender, EventArgs e)
    {
        Connect();
    }

    private async void Connect()
    {
        string username = UsernameEntry.Text;
        string serverIp = ServerIpEntry.Text;
        if (string.IsNullOrWhiteSpace(username))
        {
            await DisplayAlert("Error", "Please enter a username", "OK");
            return;
        }
        if (username.Length > 32)
        {
            await DisplayAlert("Error", "The username cannot be longer than 32 characters.", "OK");
            return;
        }

        user.Username = username.Trim();
        AppState.ServerIp = serverIp.Trim();
        await user.Connect();

        AppState.CurrentUser = user;

        await Shell.Current.GoToAsync("///MainPage");

        UsernameEntry.Text = "";
        ServerIpEntry.Text = "";
    }
}