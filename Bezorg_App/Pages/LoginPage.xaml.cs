using Microsoft.Maui.Controls;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;

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

    private async void OnFingerprintLoginClicked(object sender, EventArgs e)
    {
        var availability = await CrossFingerprint.Current.IsAvailableAsync();

        if (availability)
        {
            var authRequest = new AuthenticationRequestConfiguration("Bevestigen", "Gebruik uw vingerprint om in te loggen")
            {
                CancelTitle = "Annuleer"
            };

            var authResult = await CrossFingerprint.Current.AuthenticateAsync(authRequest);

            if (authResult.Authenticated)
            {
                await DisplayAlert("Succes", "U bent ingelogd via vingerprint!", "OK");

                // Navigeer naar MainPage (vervangt huidige pagina)
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                await DisplayAlert("Mislukt", "Vingerprint niet herkend of geannuleerd.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Niet beschikbaar", "Biometrische authenticatie is niet beschikbaar op dit apparaat.", "OK");
        }
    }
}
