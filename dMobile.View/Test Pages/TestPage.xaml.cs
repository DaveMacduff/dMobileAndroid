namespace dMobile.View
{
    using dMobile.Resources.Enums;
    using dMobile.View.Forms;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using dMobile.GroundWork;

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
        /// The Button_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await new FormAssociateBarCodeOverride().ShowDialog();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                await new FormErrorReport(ex).ShowDialog();
            }


        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            await DisplayAlert("Test", dMobile.Resources.Properties.Resources.TestStringBelowNewLine, "Ok");
        }
    }
}
