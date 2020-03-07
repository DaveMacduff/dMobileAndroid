namespace dMobile.View.Options
{
    using Android.Views;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages.Modal;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ucCommunicationSetupStatus" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucCommunicationSetupStatus : DMobileModal
    {
        /// <summary>
        /// Defines the itemsList
        /// </summary>
        private ObservableCollection<string> itemsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucCommunicationSetupStatus"/> class.
        /// </summary>
        public ucCommunicationSetupStatus()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = "Communication Status";
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.lstStatus.ItemsSource = this.ItemsList;

            Device.BeginInvokeOnMainThread(() =>
            {
                this.lstStatus.ScrollTo(this.ItemsList[this.ItemsList.Count - 1], ScrollToPosition.MakeVisible, false);
            });

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Escape)
                {
                    Device.BeginInvokeOnMainThread(async () => await this.Close());
                }
            });
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        /// <summary>
        /// Gets or sets the ItemsList
        /// </summary>
        public ObservableCollection<string> ItemsList
        {
            get
            {
                if (this.itemsList != null)
                {
                    return this.itemsList;
                } else
                {
                    return new ObservableCollection<string>();
                }
            }

            set
            {
                this.itemsList = value;
            }
        }

        /// <summary>
        /// The BtnClose_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await this.Close());
        }
    }
}
