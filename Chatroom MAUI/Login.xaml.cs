using System.Threading.Tasks;

namespace Chatroom_MAUI
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private void ConnectButton_Clicked(object sender, EventArgs e)
        {
            Connect();
        }

        private void UsernameEntry_Completed(object sender, EventArgs e)
        {
            Connect();
        }

        private async void Connect()
        {
            var username = UsernameEntry.Text?.Trim();
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "Please enter a username.", "OK");
                return;
            }

            // TODO: Username storing implementation

            await Shell.Current.GoToAsync("///MainPage");
        }
    }
}
