using System;
using Microsoft.Maui.Controls;

namespace Bezorg_App;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var password = PasswordEntry.Text?.Trim();  

        if (password == "admin")
        {
            await DisplayAlert("Succes", "Inloggen gelukt!", "OK");

            // Navigeer naar MainPage (vervangt huidige pagina)
            Application.Current.MainPage = new MainPage();
        }
        else
        {
            ErrorLabel.Text = "Wachtwoord is incorrect.";
            ErrorLabel.IsVisible = true;
        }
    }
}
