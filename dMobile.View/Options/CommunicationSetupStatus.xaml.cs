// <copyright file="CommunicationSetupStatus.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Android.Views;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="CommunicationSetupStatus" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommunicationSetupStatus : DMobileModal
    {
        /// <summary>
        /// Defines the itemsList.
        /// </summary>
        private ObservableCollection<string> itemsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSetupStatus"/> class.
        /// </summary>
        public CommunicationSetupStatus()
        {
            this.InitializeComponent();

            this.Title = "Communication Status";
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstStatus.ItemsSource = this.ExtraInfoItems;

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.lstStatus.ScrollTo(this.ExtraInfoItems[this.ExtraInfoItems.Count - 1], ScrollToPosition.MakeVisible, false);
            });

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                _ = Enum.TryParse(keyPressed, out Keycode keycode);
            });
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();

                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// Gets or sets the ExtraInfoItems.
        /// </summary>
        public ObservableCollection<string> ExtraInfoItems
        {
            get
            {
                if (this.itemsList != null)
                {
                    return this.itemsList;
                }
                else
                {
                    return new ObservableCollection<string>();
                }
            }

            set
            {
                this.itemsList = value;
            }
        }

        /// <summary>
        /// The BtnClose_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void BtnClose_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await Device.InvokeOnMainThreadAsync(async () => await this.Close());
        }
    }
}
