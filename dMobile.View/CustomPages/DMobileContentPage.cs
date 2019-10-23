namespace dMobile.View.CustomPages
{
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileContentPage" />
    /// </summary>
    public class DMobileContentPage : ContentPage
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
            get { return loading; }
            set
            {
                loading = value;
                this.OnPropertyChanged();
            }
        }

        public DMobileContentPage()
        {
            this.BindingContext = this;
            this.Loading = false;
        }

        /// <summary>
        /// The ShowDialog
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/></returns>
        public async Task Show()
        {
            try
            {
                await App.Current.MainPage.Navigation.PushAsync(this);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }
        }

        /// <summary>
        /// The Close
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task Close()
        {
            try
            {
                await App.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert(Resource.Error, ex.Message, Resource.OK);
            }
        }
    }
}
