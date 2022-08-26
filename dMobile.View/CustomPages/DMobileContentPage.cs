// <copyright file="DMobileContentPage.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomPages
{
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileContentPage" />.
    /// </summary>
    public class DMobileContentPage : ContentPage
    {
        /// <summary>
        /// Defines the loading.
        /// </summary>
        private bool loading;

        /// <summary>
        /// 
        /// </summary>
        private string optionsIconSource;

        /// <summary>
        /// 
        /// </summary>
        private bool isClosing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DMobileContentPage"/> class.
        /// </summary>
        public DMobileContentPage()
        {
            this.BindingContext = this;
            this.Loading = false;
            this.OptionsIconSource = Convert.ToString(Application.Current.Resources["OptionsIcon"]);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page content is loading.
        /// </summary>
        public bool Loading
        {
            get => this.loading;

            set
            {
                this.loading = value;
                this.OnPropertyChanged();
            }
        }

        public string OptionsIconSource
        {
            get => this.optionsIconSource;

            set
            {
                this.optionsIconSource = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The ShowDialog.
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/>.</returns>
        public async Task Show()
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await mainPage.Detail.Navigation.PushAsync(this);
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }

        /// <summary>
        /// The Close.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Close()
        {
            try
            {
                if (!this.isClosing)
                {
                    this.isClosing = true;

                    await Task.Run(() =>
                    {
                        while (this.Loading)
                        {
                            // Wait for loading to complete
                        }
                    });

                    if (Application.Current.MainPage is MasterDetailPage mainPage)
                    {
                        _ = await Device.InvokeOnMainThreadAsync(async () => await mainPage.Detail.Navigation.PopAsync());
                    }

                    this.isClosing = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }
    }
}
