using Microsoft.Maui.Controls;

namespace Bezorg_App.Views;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
        var user = UserStore.CurrentUser;
        if (user != null)
        {
            NameLabel.Text = user.Name;
            EmailLabel.Text = user.Email;
            FunctieLabel.Text = user.Functie;
        }
        else
        {
            NameLabel.Text = "-";
            EmailLabel.Text = "-";
            FunctieLabel.Text = "-";
        }
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        UserStore.CurrentUser = null;
        Application.Current.MainPage = new LoginPage();
    }
} 