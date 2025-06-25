using Bezorg_App.Views;

namespace Bezorg_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Start de app altijd met de LoginPage
            MainPage = new LoginPage();
        }
    }
}
