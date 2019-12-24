using dMobile.View.Test_Pages;
using Xamarin.Forms;

namespace dMobile.View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new MemoryTestPage());
            
            //MainPage = new NavigationPage(new ucLoginStartup());

            MainPage = new NavigationPage(new SplashStartup());
        }
    }
}