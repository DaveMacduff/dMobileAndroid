namespace dMobile.View.CustomPages.Modal
{
    using dMobile.Resources.Enums;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DialogModal" />
    /// </summary>
    public class DialogModal : ContentPage
    {
        /// <summary>
        /// Gets the Closed
        /// Checks if content page has been close
        /// </summary>
        public Task Closed
        {
            get { return Tcs.Task; }
        }

        /// <summary>
        /// Defines the DialogResult
        /// </summary>
        public DialogResult DialogResult = DialogResult.None;

        /// <summary>
        /// Gets or sets the Tcs
        /// </summary>
        private TaskCompletionSource<bool> Tcs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogModal"/> class.
        /// </summary>
        public DialogModal()
        {
            Tcs = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Tcs.SetResult(true);
        }

        /// <summary>
        /// The ShowDialog
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/></returns>
        public async Task<DialogResult> ShowDialog()
        {
            try
            {
                await App.Current.MainPage.Navigation.PushModalAsync(this);
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
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }
        }
    }
}
