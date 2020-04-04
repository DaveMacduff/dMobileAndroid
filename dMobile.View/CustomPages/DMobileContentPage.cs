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
        private bool actionsVisible;
        private string iconSource;

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

        public bool ActionsVisible
        {
            get { return this.actionsVisible; }
            set
            {
                this.actionsVisible = value;
                this.IconSource = this.actionsVisible ? Convert.ToString(Application.Current.Resources["ShowLessIcon"]) : Convert.ToString(Application.Current.Resources["ShowMoreIcon"]);
                this.OnPropertyChanged();
            }
        }

        public string IconSource
        {
            get
            {
                return iconSource;
            }

            set
            {
                this.iconSource = value;
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
                Device.BeginInvokeOnMainThread(async() => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
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
                if (Application.Current.MainPage is MasterDetailPage mainPage)
                {
                    await mainPage.Detail.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async() => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }
    }
}
