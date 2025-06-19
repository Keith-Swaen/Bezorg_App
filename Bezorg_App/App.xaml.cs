namespace Bezorg_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new LoginPage();
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
