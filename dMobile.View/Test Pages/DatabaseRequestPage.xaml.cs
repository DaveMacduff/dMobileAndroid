namespace dMobile.View.Test_Pages
{
    using dMobile.GroundWork;
    using dMobile.View.CustomPages;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="DatabaseRequestPage" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatabaseRequestPage : DMobileContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRequestPage"/> class.
        /// </summary>
        public DatabaseRequestPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The BtnCreateDatabase_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private async void BtnCreateDatabase_Clicked(object sender, EventArgs e)
        {
            this.Loading = true;

            try
            {
                await Task.Run(() =>
                {
                    DatabaseNub database = new DatabaseNub();
                    database.CreateNewDatabase();
                    database.CreateTables();
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not create database: " + ex.Message, "OK");
            }

            this.Loading = false;
        }

        /// <summary>
        /// The BtnTableContactUser_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private async void BtnTableContactUser_Clicked(object sender, EventArgs e)
        {
            this.Loading = true;

            try
            {
                await Task.Run(() =>
                {
                    TableContactUsers tContactUsers = new TableContactUsers();
                    tContactUsers.Request();
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not create tContactUsers: " + ex.Message, "OK");
            }

            this.Loading = false;
        }

        /// <summary>
        /// The BtnTableLocationMapWarehouse_Clicked
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private async void BtnTableLocationMapWarehouse_Clicked(object sender, EventArgs e)
        {
            this.Loading = true;

            try
            {
                await Task.Run(() =>
                {
                    TableLocationMapWarehouse tLocationMapWarehouse = new TableLocationMapWarehouse();
                    tLocationMapWarehouse.Request();
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not create tLocationMapWarehouse: " + ex.Message, "OK");
            }

            this.Loading = false;
        }
    }
}
