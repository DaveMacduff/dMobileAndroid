using dMobile.Bedrock;
using dMobile.View.Test_Pages;
using dMobile.View.Themes;
using System.Collections.Generic;
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

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SetTheme();
            }
        }

        public void SetTheme()
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            Theme theme = SysEnvironment.GetInstance().SysSettings.Theme;

            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case Theme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }
        }
    }
}