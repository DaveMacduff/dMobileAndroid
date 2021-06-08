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
    public class DMobileModal : ContentPage, IDMobileModal
    {
        /// <summary>
        /// Defines the loading
        /// </summary>
        private bool loading;
        private string optionsIconSource;
        private DialogResult dialogResult = DialogResult.None;
        private bool isClosing = false;

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
        /// Gets the Closed
        /// Checks if content page has been close
        /// </summary>
        private Task Closed
        {
            get { return this.tcs.Task; }
        }

        /// <summary>
        /// Gets or sets the tcs
        /// </summary>
        private TaskCompletionSource<bool> tcs { get; set; }

        public DialogResult DialogResult { get => dialogResult; set => dialogResult = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DMobileModal"/> class.
        /// </summary>
        public DMobileModal()
        {
            this.tcs = new TaskCompletionSource<bool>();
            this.BindingContext = this;
            this.Loading = false;
            this.OptionsIconSource = Convert.ToString(Application.Current.Resources["OptionsIcon"]);
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
                    await Device.InvokeOnMainThreadAsync(async () => await mainPage.Detail.Navigation.PushAsync(this));
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.Navigation.PushAsync(this));
                }

                await this.Closed;
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
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
                Console.WriteLine(DateTime.Now.Ticks + " Enter modal closing");
                if (!isClosing)
                {
                    this.isClosing = true;

                    await Task.Run(() =>
                    {
                        while (this.Loading)
                        {
                            // Wait for loading to complete
                        }
                    });

                    while (!this.tcs.Task.IsCompleted)
                    {
                        this.tcs.TrySetResult(true);
                    }

                    if (Application.Current.MainPage is MasterDetailPage mainPage)
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await mainPage.Detail.Navigation.PopAsync());
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.Navigation.PopAsync());
                    }

                    this.isClosing = false;
                }

                Console.WriteLine(DateTime.Now.Ticks + " Exit modal closing");
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert(Resource.Error, ex.Message, Resource.OK));
            }
        }
    }
}
