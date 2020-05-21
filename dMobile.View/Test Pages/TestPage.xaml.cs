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

            this.dataGridMainActivity.ItemsSource = this.DesignGrid();
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

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                //ColumnSpacing = 15,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // TODO: Resource strings
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeader, 0);

            //dataGridMainActivityHeader.Children.Add(gridHeader);

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                FlexLayout flexLayout = new FlexLayout { AlignContent = FlexAlignContent.SpaceAround };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                flexLayout.Children.Add(labelQty);
                flexLayout.Children.Add(labelWeight);
                flexLayout.Children.Add(labelUnitCode);
                flexLayout.Children.Add(labelSchemeID);

                FlexLayout.SetGrow(labelQty, 1);
                FlexLayout.SetGrow(labelWeight, 1);
                FlexLayout.SetGrow(labelUnitCode, 1);
                FlexLayout.SetGrow(labelSchemeID, 10);

                FlexLayout.SetShrink(labelQty, 0);
                FlexLayout.SetShrink(labelWeight, 0);
                FlexLayout.SetShrink(labelUnitCode, 0);
                FlexLayout.SetShrink(labelSchemeID, 0);

                return new ViewCell { View = flexLayout };
            });

            dataGridMainActivity.ItemTemplate = dataTemplate;

            return observableCollection;
        }
    }
}
