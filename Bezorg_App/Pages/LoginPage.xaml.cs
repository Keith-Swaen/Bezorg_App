namespace Bezorg_App;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var password = PasswordEntry.Text;

        if (password == "admin")
        {
            // Login succesvol, navigeer naar de hoofd- of homepagina
            await DisplayAlert("Succes", "Inloggen gelukt!", "OK");

            // Bijvoorbeeld naar MainPage navigeren:
            Application.Current.MainPage = new MainPage();
        }
        else
        {
            ErrorLabel.Text = "Wachtwoord is incorrect.";
            ErrorLabel.IsVisible = true;
        }
    }
}
