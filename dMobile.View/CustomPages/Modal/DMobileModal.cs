// <copyright file="DMobileModal.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="DMobileModal" /> dMobile model base content page class.
    /// </summary>
    public class DMobileModal : ContentPage, IDMobileModal
    {
        /// <summary>
        /// Defines the is loading.
        /// </summary>
        private bool loading = false;

        /// <summary>
        /// Defines the optionsIconSource.
        /// </summary>
        private string optionsIconSource = string.Empty;

        /// <summary>
        /// Defines the isClosing.
        /// </summary>
        private bool isClosing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DMobileModal"/> class.
        /// </summary>
        public DMobileModal()
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
            get => this.loading;

            set
            {
                this.loading = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the OptionsIconSource.
        /// </summary>
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
        /// Gets or sets the DialogResult.
        /// </summary>
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// Gets or sets the task completion source.
        /// </summary>
        private TaskCompletionSource<bool> Tcs { get; set; }

        /// <summary>
        /// Gets the task result indicating if content page has been close.
        /// </summary>
        private Task Closed => this.Tcs.Task;

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
                        await mainPage.Detail.Navigation.PushAsync(this));
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.Navigation.PushAsync(this));
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
        /// Close the content page.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public async Task Close()
        {
            try
            {
                if (!this.isClosing)
                {
                    this.isClosing = true;

                    await Task.Run((Action)(() =>
                    {
                        while (this.Loading)
                        {
                            // Wait for loading to complete
                        }
                    }));

                    while (!this.Tcs.Task.IsCompleted)
                    {
                        _ = this.Tcs.TrySetResult(true);
                    }

                    _ = Application.Current.MainPage is MasterDetailPage mainPage
                        ? await Device.InvokeOnMainThreadAsync(async () =>
                            await mainPage.Detail.Navigation.PopAsync())
                        : await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.Navigation.PopAsync());

                    this.isClosing = false;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await this.DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }
    }
}
