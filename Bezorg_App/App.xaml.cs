namespace Bezorg_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Start de app altijd met een NavigationPage
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
