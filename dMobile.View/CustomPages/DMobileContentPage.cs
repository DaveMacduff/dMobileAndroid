// <copyright file="DMobileContentPage.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomPages
{
    using dMobile.GroundWork;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using static dMobile.GroundWork.TableBase;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileContentPage" />.
    /// </summary>
    public class DMobileContentPage : ContentPage, IPageShownPublisher
    {
        /// <summary>
        /// Defines the loading.
        /// </summary>
        private bool isPageLoading = false;

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
            this.IsIndicatorRunning = false;
            this.OptionsIconSource = Convert.ToString(Application.Current.Resources["OptionsIcon"]);

            // Subscribe to the LayoutChanged event
            this.LayoutChanged += OnLayoutChanged;
        }

        public delegate void PageShownEventHandler(IPageShownPublisher publisher, PageShownEventArgs args);

        public event PageShownEventHandler OnPageShown;

        /// <summary>
        /// Gets or sets a value indicating whether the activity indicator should run.
        /// </summary>
        public bool IsIndicatorRunning
        {
            get => this.isPageLoading;

            set
            {
                this.isPageLoading = value;
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLayoutChanged(object sender, EventArgs e)
        {
            // This event is called whenever the layout of the page changes
            // You can put logic here that runs when the layout is updated
            // Typically, this would indicate that the drawing is complete
            this.LayoutChanged -= OnLayoutChanged; // Unsubscribe if you only need it once
            if (OnPageShown != null)
            {
                OnPageShown(this, new PageShownEventArgs());
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
                        while (this.IsIndicatorRunning)
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

        public void RegisterPageShownListener(IPageShownListener pageShownListener)
        {
            this.OnPageShown += new PageShownEventHandler(
                pageShownListener.OnPageShown);
        }

        public void UnregisterPageShownListener(IPageShownListener pageShownListener)
        {
            this.OnPageShown -= new PageShownEventHandler(
                pageShownListener.OnPageShown);
        }
    }
}
