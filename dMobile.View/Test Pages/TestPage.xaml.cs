namespace dMobile.View
{
    using dMobile.Resources.Enums;
    using dMobile.View.Forms;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using dMobile.GroundWork;
    using Android.Views;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines the <see cref="TestPage" />
    /// </summary>
    public partial class TestPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPage"/> class.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();
        }

          /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter)
                {
                    await DisplayAlert("Key Pressed", "Key is: " + keycode.ToString(), "OK");
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

        private void BtnTest_Clicked(object sender, EventArgs e)
        {
            this.Test();
        }

        private async Task Test()
        {
            bool test = false;

            if (await DisplayAlert("TEST", "Click one", "Yes", "No"))
            {
                test = true;
            }

            if (test == true)
            {
                await DisplayAlert("Success", "This works!", "OK");
            }
        }

        private void BtnTest2_Clicked(object sender, EventArgs e)
        {

        }
    }
}
