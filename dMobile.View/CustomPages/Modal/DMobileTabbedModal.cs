// <copyright file="DMobileTabbedModal.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomPages.Modal
{
    using System;
    using System.Threading.Tasks;
    using dMobile.Resources.Enums;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileTabbedModal" />.
    /// </summary>
    public class DMobileTabbedModal : TabbedPage, IDMobileModal
    {
        /// <summary>
        /// Defines the loading.
        /// </summary>
        private bool loading;

        /// <summary>
        /// Defines the optionsIconSource.
        /// </summary>
        private string optionsIconSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DMobileTabbedModal"/> class.
        /// </summary>
        public DMobileTabbedModal()
        {
            this.Tcs = new TaskCompletionSource<bool>();
            this.BindingContext = this;
            this.Loading = false;
            this.OptionsIconSource = Convert.ToString(Application.Current.Resources["OptionsIcon"]);
        }

        /// <summary>
        /// Gets or sets a value indicating whether Loading.
        /// </summary>
        public bool Loading
        {
            get
            {
                return this.loading;
            }

            set
            {
                this.loading = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the DialogResult
        /// Defines the DialogResult..
        /// </summary>
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// Gets or sets the OptionsIconSource.
        /// </summary>
        public string OptionsIconSource
        {
            get
            {
                return this.optionsIconSource;
            }

            set
            {
                this.optionsIconSource = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the Closed
        /// Checks if content page has been close..
        /// </summary>
        private Task Closed
        {
            get { return this.Tcs.Task; }
        }

        /// <summary>
        /// Gets or sets the task completion source.
        /// </summary>
        private TaskCompletionSource<bool> Tcs { get; set; }

        /// <summary>
        /// The ShowDialog.
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/>.</returns>
        public virtual async Task<DialogResult> ShowDialog()
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await mainPage.Detail.Navigation.PushModalAsync(new NavigationPage(this)));
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this)));
                }

                await this.Closed;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }

            return this.DialogResult;
        }

        /// <summary>
        /// The Close.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Close()
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    _ = await Device.InvokeOnMainThreadAsync(async () =>
                          await mainPage.Detail.Navigation.PopModalAsync());
                }
                else
                {
                    _ = await Device.InvokeOnMainThreadAsync(async () =>
                          await Application.Current.MainPage.Navigation.PopModalAsync());
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Application.Current.ModalPopping += this.Current_ModalPopping;
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Application.Current.ModalPopping -= this.Current_ModalPopping;
        }

        /// <summary>
        /// The Current_ModalPopped.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ModalPoppingEventArgs"/>.</param>
        private void Current_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            try
            {
                while (!this.Tcs.Task.IsCompleted)
                {
                    _ = this.Tcs.TrySetResult(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
