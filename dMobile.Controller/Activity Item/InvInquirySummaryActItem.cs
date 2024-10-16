// <copyright file="InvInquirySummaryActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Items;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="InvInquirySummaryActItem" />.
    /// </summary>
    public class InvInquirySummaryActItem : ActItem
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0 = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1 = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2 = 0;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3 = 0;

        /// <summary>
        /// Defines the columnWidth4.
        /// </summary>
        private double columnWidth4 = 0;

        /// <summary>
        /// Defines the columnWidth5.
        /// </summary>
        private double columnWidth5 = 0;

        /// <summary>
        /// Defines the columnWidth6.
        /// </summary>
        private double columnWidth6 = 0;

        /// <summary>
        /// Defines the columnWidth7.
        /// </summary>
        private double columnWidth7 = 0;

        /// <summary>
        /// Defines the columnWidth8.
        /// </summary>
        private double columnWidth8 = 0;

        /// <summary>
        /// Defines the columnWidth9.
        /// </summary>
        private double columnWidth9 = 0;

        /// <summary>
        /// Defines the columnWidth10.
        /// </summary>
        private double columnWidth10 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listInventoryInquiryItems.
        /// </summary>
        private List<InventoryInquiryItem> listInventoryInquiryItems = new List<InventoryInquiryItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InvInquirySummaryActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public InvInquirySummaryActItem(Activity.Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(CycleActItem).FullName;
            this.IsAskCreateNewItem = false;
            this.LocMap = LocationMap.GetInstance().CurrLocMap;

            // Create item template
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth6, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth7, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth8, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth9, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth10, GridUnitType.Absolute) },
                    },
                };

                var labelProductId = new Label { VerticalOptions = LayoutOptions.Center };
                var labelBc = new Label { VerticalOptions = LayoutOptions.Center };
                var labelQuantity = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelProdDate = new Label { VerticalOptions = LayoutOptions.Center };
                var labelBbDate = new Label { VerticalOptions = LayoutOptions.Center };
                var labelSection = new Label { VerticalOptions = LayoutOptions.Center };
                var labelSlot = new Label { VerticalOptions = LayoutOptions.Center };
                var labelSkid = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWeightUnitCode = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductId.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBc.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelQuantity.SetBinding(Label.TextProperty, new Binding("ActivityItemQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelProdDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitProductionDate"));
                labelBbDate.SetBinding(Label.TextProperty, new Binding("ActivityDetailLogisticUnitBestBeforeSellByDate"));
                labelSection.SetBinding(Label.TextProperty, new Binding("LocationMapSectionFKRefno"));
                labelSlot.SetBinding(Label.TextProperty, new Binding("LocationMapSlotFKRefno"));
                labelSkid.SetBinding(Label.TextProperty, new Binding("SkidCode"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityItemWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeightUnitCode.SetBinding(Label.TextProperty, new Binding("WeightUnitCode"));

                // Add labels to grid
                gridData.Children.Add(labelProductId);
                gridData.Children.Add(labelBc);
                gridData.Children.Add(labelQuantity);
                gridData.Children.Add(labelUnitCode);

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    gridData.Children.Add(labelProdDate);
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    gridData.Children.Add(labelBbDate);
                }

                if (this.LocMap.IsSectionTracked)
                {
                    gridData.Children.Add(labelSection);
                }

                if (this.LocMap.IsSectionTracked && this.LocMap.IsSlotTracked)
                {
                    gridData.Children.Add(labelSlot);
                }

                gridData.Children.Add(labelSkid);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelWeightUnitCode);

                // Set label columns
                Grid.SetColumn(labelProductId, 0);
                Grid.SetColumn(labelBc, 1);
                Grid.SetColumn(labelQuantity, 2);
                Grid.SetColumn(labelUnitCode, 3);

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    Grid.SetColumn(labelProdDate, 4);
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    Grid.SetColumn(labelBbDate, 5);
                }

                if (this.LocMap.IsSectionTracked)
                {
                    Grid.SetColumn(labelSection, 6);
                }

                if (this.LocMap.IsSectionTracked && this.LocMap.IsSlotTracked)
                {
                    Grid.SetColumn(labelSlot, 7);
                }

                Grid.SetColumn(labelSkid, 8);
                Grid.SetColumn(labelWeight, 9);
                Grid.SetColumn(labelWeightUnitCode, 10);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateInvInquirySummaryItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateInvInquirySummaryItem", dataTemplate);
            }
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <summary>
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>

        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data)
        {
            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            if (prodUnitRefno != 0)
            {
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            // Add all items in data view to observable collection for display
            foreach (var activityItem in this.ListItems.Cast<ActivityItem>())
            {
                observableCollection.Add(activityItem);
            }

            var labelProductIdHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBcHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelQuantityHeader = new Label { Text = "Quantity", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelProdDateHeader = new Label { Text = "Prod Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelBbDateHeader = new Label { Text = "BB/SB Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelSectionHeader = new Label { Text = "Section", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelSlotHeader = new Label { Text = "Slot", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelSkidHeader = new Label { Text = "Skid", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelWeightUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIdHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBcHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQuantityHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProdDateHeader.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBbDateHeader.Text);
            this.columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSectionHeader.Text);
            this.columnWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSlotHeader.Text);
            this.columnWidth8 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSkidHeader.Text);
            this.columnWidth9 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.columnWidth10 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightUnitCodeHeader.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth6, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth7, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth8, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth9, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth10, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelProductIdHeader);
            gridHeader.Children.Add(labelBcHeader);
            gridHeader.Children.Add(labelQuantityHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);

            if (this.ProdInfo.Scheme.IsProductionDateTracked)
            {
                gridHeader.Children.Add(labelProdDateHeader);
            }

            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                gridHeader.Children.Add(labelBbDateHeader);
            }

            if (this.LocMap.IsSectionTracked)
            {
                gridHeader.Children.Add(labelSectionHeader);
            }

            if (this.LocMap.IsSectionTracked && this.LocMap.IsSlotTracked)
            {
                gridHeader.Children.Add(labelSlotHeader);
            }

            gridHeader.Children.Add(labelSkidHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelWeightUnitCodeHeader);

            // Set label columns
            Grid.SetColumn(labelProductIdHeader, 0);
            Grid.SetColumn(labelBcHeader, 1);
            Grid.SetColumn(labelQuantityHeader, 2);
            Grid.SetColumn(labelUnitCodeHeader, 3);

            if (this.ProdInfo.Scheme.IsProductionDateTracked)
            {
                Grid.SetColumn(labelProdDateHeader, 4);
            }

            if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
            {
                Grid.SetColumn(labelBbDateHeader, 5);
            }

            if (this.LocMap.IsSectionTracked)
            {
                Grid.SetColumn(labelSectionHeader, 6);
            }

            if (this.LocMap.IsSectionTracked && this.LocMap.IsSlotTracked)
            {
                Grid.SetColumn(labelSlotHeader, 7);
            }

            Grid.SetColumn(labelSkidHeader, 8);
            Grid.SetColumn(labelWeightHeader, 9);
            Grid.SetColumn(labelWeightUnitCodeHeader, 10);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (var activityItem in observableCollection.Cast<ActivityItem>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ProductCode);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ProductUnitBC);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{activityItem.ActivityItemQuantity:F3}");
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ActivityDetailLogisticUnitProductionDate);
                var detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.ActivityDetailLogisticUnitBestBeforeSellByDate);
                var detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.LocationMapSectionFKRefno);
                var detailWidth7 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.LocationMapSlotFKRefno);
                var detailWidth8 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.SkidCode);
                var detailWidth9 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{activityItem.ActivityItemWeightQuantity:F3}");
                var detailWidth10 = DependencyService.Get<IGridService>().CalculateTextWidth(activityItem.WeightUnitCode);

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (this.ProdInfo.Scheme.IsProductionDateTracked)
                {
                    if (detailWidth4 > this.columnWidth4)
                    {
                        this.columnWidth4 = detailWidth4;
                        gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                    }
                }
                else
                {
                    this.columnWidth4 = 0;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (this.ProdInfo.Scheme.IsBestBeforeDateTracked)
                {
                    if (detailWidth5 > this.columnWidth5)
                    {
                        this.columnWidth5 = detailWidth5;
                        gridHeader.ColumnDefinitions[5].Width = this.columnWidth5;
                    }
                }
                else
                {
                    this.columnWidth5 = 0;
                    gridHeader.ColumnDefinitions[5].Width = this.columnWidth5;
                }

                if (this.LocMap.IsSectionTracked)
                {
                    if (detailWidth6 > this.columnWidth6)
                    {
                        this.columnWidth6 = detailWidth6;
                        gridHeader.ColumnDefinitions[6].Width = this.columnWidth6;
                    }
                }
                else
                {
                    this.columnWidth6 = 0;
                    gridHeader.ColumnDefinitions[6].Width = this.columnWidth6;
                }

                if (this.LocMap.IsSectionTracked && this.LocMap.IsSlotTracked)
                {
                    if (detailWidth7 > this.columnWidth7)
                    {
                        this.columnWidth7 = detailWidth7;
                        gridHeader.ColumnDefinitions[7].Width = this.columnWidth7;
                    }
                }
                else
                {
                    this.columnWidth7 = 0;
                    gridHeader.ColumnDefinitions[7].Width = this.columnWidth7;
                }

                if (detailWidth8 > this.columnWidth8)
                {
                    this.columnWidth8 = detailWidth8;
                    gridHeader.ColumnDefinitions[8].Width = this.columnWidth8;
                }

                if (detailWidth9 > this.columnWidth9)
                {
                    this.columnWidth9 = detailWidth9;
                    gridHeader.ColumnDefinitions[9].Width = this.columnWidth9;
                }

                if (detailWidth10 > this.columnWidth10)
                {
                    this.columnWidth10 = detailWidth10;
                    gridHeader.ColumnDefinitions[10].Width = this.columnWidth10;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateInvInquirySummaryItem"];

            return observableCollection;
        }

        public override Grid DesignGrid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Create and fill the data template.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        public override void FillDataTemplate(Grid gridMainActivity)
        {
            throw new NotImplementedException();
        }


        public override DataTemplate GetDataTemplate()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> GetActivityItemList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            // DataTable dataTableItems = new DataTable("tActivityItem");

            // dataTableItems.Columns.Add("productcode", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("productunitbc", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("unitcode", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("weightunitcode", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("activitydetaillogisticunitproductiondate", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("activitydetaillogisticunitbestbeforesellbydate", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("locationmapwarehousefkrefno", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("locationmapsectionfkrefno", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("locationmapslotfkrefno", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("skidcode", Type.GetType("System.String"));
            // dataTableItems.Columns.Add("activityitemquantity", Type.GetType("System.Decimal"));
            // dataTableItems.Columns.Add("activityitemweightquantity", Type.GetType("System.Decimal"));
            // this.listInventoryInquiryItems = dataTableItems.Rows.Cast<InventoryInquiryItem>().ToList();
            this.ListItems = new List<object>();
            return Task.FromResult(this.ListItems);
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item refno else 0.</returns>
        public override Task<int> GetActivityItemRefno(int productUnitRefno, SLocMap locMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object activityItem)
        {
            var itemFkRefno = Task.FromResult(0);
            this.SelectedItem = activityItem;
            return await itemFkRefno;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            try
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                var responseData = SysCommon.SubmitComm(await this.CreateSubmitXml(data));
                this.ListItems = await this.GetActivityItemList(false);
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 50);
                using (var ds = new DataSet())
                {
                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    if (ds.Tables.Count != 0)
                    {
                        foreach (DataRow row in ds.Tables["tActivityItem"].Rows)
                        {
                            var activityItem = new ActivityItem
                            {
                                ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                                ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                                UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                                WeightUnitCode = Convert.ToString(row["WeightUnitCode"] != DBNull.Value ? row["WeightUnitCode"] : null),
                                ActivityDetailLogisticUnitProductionDate = Convert.ToString(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                                ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToString(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                                LocationMapWarehouseFKRefno = Convert.ToString(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                                LocationMapSectionFKRefno = Convert.ToString(row["LocationMapSectionFKRefno"] != DBNull.Value ? row["LocationMapSectionFKRefno"] : null),
                                LocationMapSlotFKRefno = Convert.ToString(row["LocationMapSlotFKRefno"] != DBNull.Value ? row["LocationMapSlotFKRefno"] : null),
                                SkidCode = Convert.ToString(row["SkidCode"] != DBNull.Value ? row["SkidCode"] : null),
                                ActivityItemQuantity = Convert.ToDecimal(row["ActivityItemQuantity"] != DBNull.Value ? row["ActivityItemQuantity"] : null),
                                ActivityItemWeightQuantity = Convert.ToDecimal(row["ActivityItemWeightQuantity"] != DBNull.Value ? row["ActivityItemWeightQuantity"] : null),
                            };

                            this.ListItems.Add(activityItem);
                        }
                    }
                }

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 95);
                await this.FormatGrid();
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 100);
            }
            catch (Exception ex)
            {
                throw new Exception("Refresh inventory inquiry summary items failed.", ex);
            }
        }

        /// <summary>
        /// Hides the extra columns, changes the sectionFKRefno and slotFKRefno code to names.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task FormatGrid()
        {
            if (this.LocMap.IsSectionTracked)
            {
                // Format Section column
                // Use a dictionary to only hit the database once per unique section number this call.
                var names = new Dictionary<int, string>();

                foreach (var activityItem in this.ListItems.Cast<ActivityItem>())
                {
                    var sectionFkRefno = Convert.ToInt32(activityItem.LocationMapSectionFKRefno);
                    if (sectionFkRefno > 0)
                    {
                        string sectionName;

                        if (names.ContainsKey(sectionFkRefno))
                        {
                            sectionName = names[sectionFkRefno];
                        }
                        else
                        {
                            sectionName = await LocationMapTasks.GetSectionName(sectionFkRefno);
                            names.Add(sectionFkRefno, sectionName);
                        }

                        activityItem.LocationMapSectionFKRefno = sectionName;
                    }
                }

                names.Clear();
            }

            if (this.LocMap.IsSlotTracked)
            {
                // Format Slot column
                // Use a dictionary to only hit the database once per unique slot number this call.
                var names = new Dictionary<int, string>();

                foreach (var activityItem in this.ListItems.Cast<ActivityItem>())
                {
                    var slotFkRefno = Convert.ToInt32(activityItem.LocationMapSlotFKRefno);
                    if (slotFkRefno > 0)
                    {
                        string slotName;

                        if (names.ContainsKey(slotFkRefno))
                        {
                            slotName = names[slotFkRefno];
                        }
                        else
                        {
                            slotName = await LocationMapTasks.GetSlotName(slotFkRefno);
                            names.Add(slotFkRefno, slotName);
                        }

                        activityItem.LocationMapSlotFKRefno = slotName;
                    }
                }

                names.Clear();
            }
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            // WO 15547, WO 13770
            string sectionRefno;
            string slotRefno;

            if (SysEnvironment.GetInstance().SysSettings.InventoryInquirySearchEntireWarehouse)
            {
                sectionRefno = string.Empty;
                slotRefno = string.Empty;
            }
            else
            {
                sectionRefno = this.LocMap.SectFkRefno.ToString();
                slotRefno = this.LocMap.SlotFkRefno.ToString();
            }

            var sectionXml = "<sect_refno>" + sectionRefno + "</sect_refno>";
            var slotXml = "<slot_refno>" + slotRefno + "</slot_refno>";

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST INVENTORY INQUIRY SUMMARY SHORTCUT</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + this.ProdInfo.Code + "</prod_id>"
                + "<whs_id>" + this.LocMap.WhsFkRefno + "</whs_id>"
                + sectionXml
                + slotXml
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override Task UpdateItemTableQuantity(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return "Inventory inquiry summary.";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Submit(XDocument data, object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is InventoryInquiryItem item)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Picking);
            }

            if (locMapRefno == 0)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Floor);
            }

            if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
            {
                locMapRefno = 0;
            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAsk()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAskZero()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign key reference number.</param>
        /// <returns><see cref="int"/> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((InvInquirySummaryActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
