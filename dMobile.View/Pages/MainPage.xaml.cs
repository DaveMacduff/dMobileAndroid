// <copyright file="MainPage.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.BarCodeScanner.BarCodeScanners;
using dMobile.View.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Pages
{
    using System;
    using Helpers;

    /// <summary>
    /// Defines the <see cref="MainPage" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.masterPage.listView.ItemSelected += this.OnItemSelected;
        }

        /// <summary>
        /// The OnItemSelected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SelectedItemChangedEventArgs"/>.</param>
        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                this.masterPage.listView.SelectedItem = null;
                this.IsPresented = false;
                Application.Current.MainPage = new NavigationPage(new LoginStartup());
            }
        }
    }
}
