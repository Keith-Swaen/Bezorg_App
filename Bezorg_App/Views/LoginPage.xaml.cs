using Microsoft.Maui.Controls;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Bezorg_App.Models;

namespace Bezorg_App.Views;

public static class UserStore
{
    public static User? CurrentUser { get; set; }
}

public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Functie { get; set; } = "Bezorger";
}

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();

        var user = DummyUsers.Users.FirstOrDefault(u => u.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null && string.Equals(password, DummyUsers.DefaultPassword, StringComparison.OrdinalIgnoreCase))
        {
            UserStore.CurrentUser = user;
            await DisplayAlert("Succes", $"Welkom {user.Name}!", "OK");
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            ErrorLabel.Text = "Gebruikersnaam of wachtwoord is incorrect.";
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
                // Log altijd in als Keith bij vingerprint
                UserStore.CurrentUser = DummyUsers.Users.FirstOrDefault(u => u.Name == "Keith");
                await DisplayAlert("Succes", "U bent ingelogd via vingerprint!", "OK");
                Application.Current.MainPage = new AppShell();
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
