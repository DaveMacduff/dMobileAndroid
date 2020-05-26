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
    using System.Data;
    using dMobile.Resources.Types;
    using dMobile.Controller;
    using Java.Util;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Collections;

    /// <summary>
    /// Defines the <see cref="TestPage" />
    /// </summary>
    public partial class TestPage : ContentPage
    {
        private ObservableCollection<object> itemList = new ObservableCollection<object>();
        private DataTemplate itemTemplate = new DataTemplate();

        public ObservableCollection<object> ItemList { get => itemList; set { itemList = value; OnPropertyChanged(); } }
        public DataTemplate ItemTemplate { get => itemTemplate; set { itemTemplate = value; OnPropertyChanged(); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPage"/> class.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.ItemList = this.DesignGrid();
            this.ItemTemplate = new DataTemplate(() =>
            {
                var tapGestureRecognizer = new TapGestureRecognizer();

                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };
                
                tapGestureRecognizer.Tapped += (s, e) => 
                {
                };
                
                grid.GestureRecognizers.Add(tapGestureRecognizer);

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                grid.Children.Add(labelQty);
                grid.Children.Add(labelWeight);
                grid.Children.Add(labelUnitCode);
                grid.Children.Add(labelSchemeID);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelSchemeID, 3);

                return grid;
            });
        }

        public ObservableCollection<object> DesignGrid()
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            for (int i = 0; i < 10; i++)
            {
                CountDetail countDetail = new CountDetail
                {
                    UnitCode = Convert.ToString("CS"),
                    ActivityDetailQuantity = Convert.ToDecimal(i != 9 ? i * 10 : 1000000),
                    ActivityDetailWeightQuantity = Convert.ToDecimal(i * 100 + 1000),
                    ActivityDetailSchemeID = Convert.ToString("thisisaverylongSTRINGthatisgoingheretoSEEIFWECANEXPANDasdfasdfasdfasdfasdfasdfa"),
                };

                observableCollection.Add(countDetail);
            }

            return observableCollection;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        //private async void Button_Clicked(object sender, EventArgs e)
        //{
        //    await DisplayAlert("", "dataGridMainActivity width: " + this.dataGridMainActivity.Width + Environment.NewLine + "dataGridMainActivityHeader width: " + this.dataGridMainActivityHeader.Width + Environment.NewLine + "scrollViewTest width: " + this.scrollViewTest.Width + Environment.NewLine, "OK");
        //}
    }
}
