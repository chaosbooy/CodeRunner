namespace CodeRunner;

public partial class SettingsPage : ContentPage
{
    public static string PreviousPage = nameof(MainPage);

	public SettingsPage()
	{
		InitializeComponent();
    }

    private async void ReturnToPreviousPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{PreviousPage}");
    }

    private void SaveSettings(object sender, EventArgs e)
    {

    }
}