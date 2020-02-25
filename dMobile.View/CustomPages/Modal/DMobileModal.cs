namespace dMobile.View.CustomPages.Modal
{
    using dMobile.Resources.Enums;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileModal" />
    /// </summary>
    public class DMobileModal : ContentPage
    {
        /// <summary>
        /// Defines the loading
        /// </summary>
        private bool loading;

        /// <summary>
        /// Gets or sets a value indicating whether Loading
        /// </summary>
        public bool Loading
        {
            get { return this.loading; }
            set
            {
                this.loading = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the Closed
        /// Checks if content page has been close
        /// </summary>
        private Task Closed
        {
            get { return this.tcs.Task; }
        }

        /// <summary>
        /// Defines the DialogResult
        /// </summary>
        public DialogResult DialogResult = DialogResult.None;

        /// <summary>
        /// Gets or sets the tcs
        /// </summary>
        private TaskCompletionSource<bool> tcs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DMobileModal"/> class.
        /// </summary>
        public DMobileModal()
        {
            this.tcs = new TaskCompletionSource<bool>();
            this.BindingContext = this;
            this.Loading = false;
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                while (!this.tcs.Task.IsCompleted)
                {
                    this.tcs.TrySetResult(true);
                }
            }
            catch (Exception ex)
            {
                // TODO: logging
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// The ShowDialog
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/></returns> 
        public virtual async Task<DialogResult> ShowDialog()
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await mainPage.Detail.Navigation.PushModalAsync(this);
                }
                else
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(this);
                }

                await this.Closed;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }

            return this.DialogResult;
        }

        /// <summary>
        /// The Close
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task Close()
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await mainPage.Detail.Navigation.PopModalAsync();
                }
                else
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }
        }
    }
}
