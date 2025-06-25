using Bezorg_App.Views;

namespace Bezorg_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(BezorgstatussenPage), typeof(BezorgstatussenPage));
            Routing.RegisterRoute(nameof(BevestigBezorging), typeof(BevestigBezorging));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
        }

        private async void BezorgstatussenPage_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(BezorgstatussenPage));
        }
    }
}
