namespace dMobile.View.CustomPages
{
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Resource = Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="DMobileContentPage" />
    /// </summary>
    public class DMobileContentPage : ContentPage
    {
        /// <summary>
        /// Defines the loading
        /// </summary>
        private bool loading;
        private string optionsIconSource;

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

        public string OptionsIconSource
        {
            get
            {
                return optionsIconSource;
            }

            set
            {
                this.optionsIconSource = value;
                this.OnPropertyChanged();
            }
        }

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
        /// The ShowDialog
        /// </summary>
        /// <returns>The <see cref="Task{DialogResult}"/></returns>
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
                await Device.InvokeOnMainThreadAsync(async() => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
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
                while (this.Loading)
                {
                    // Wait for loading to complete
                }

                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await mainPage.Detail.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async() => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }
    }
}
