// <copyright file="App.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View
{
    using System.Collections.Generic;
    using dMobile.Bedrock;
    using dMobile.View.Themes;
    using Xamarin.Forms;

    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            this.MainPage = new NavigationPage(new SplashStartup());

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SetTheme();
            }
        }

        public void SetTheme()
        {
            ICollection<ResourceDictionary> mergedDictionaries = Current.Resources.MergedDictionaries;
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