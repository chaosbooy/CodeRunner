namespace CodeRunner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OpenGame(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(GamePage)}");
        }

        private async void OpenSettings(object sender, EventArgs e)
        {
            SettingsPage.PreviousPage = nameof(MainPage);
            await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
        }

    }
}
