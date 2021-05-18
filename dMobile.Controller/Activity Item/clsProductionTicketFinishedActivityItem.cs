namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Collections.Generic;
    using System.Linq;

    public class TicketFinishedActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        public TicketFinishedActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(TicketFinishedActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2TicketFinished;
            this.IsAskCreateNewItem = false;

            // Create item template
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label labelCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnit = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelExpQty = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnit.SetBinding(Label.TextProperty, new Binding("BaseUnitCode"));
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionBaseQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelExpQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionBaseQuantityExpected", BindingMode.Default, null, null, "{0:F3}"));

                // Add labels to grid
                gridData.Children.Add(labelCode);
                gridData.Children.Add(labelBC);
                gridData.Children.Add(labelUnit);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelExpQty);

                // Set label columns
                Grid.SetColumn(labelCode, 0);
                Grid.SetColumn(labelBC, 1);
                Grid.SetColumn(labelUnit, 2);
                Grid.SetColumn(labelQty, 3);
                Grid.SetColumn(labelExpQty, 4);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsTicketFinishedCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCatchAskZero()
        {
            bool isCatchAskZero = base.IsCatchAskZero();
            if (isCatchAskZero)
            {
                isCatchAskZero = !this.SysEnv.SysSettings.IsTicketFinishedNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(whsLocMapRefno, prodRefno, SysCommon.ProdLocMapTypes.Receiving);
            return locMapRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse production item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find production item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemProductionFinishedStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int foundActItemRefno = 0;
            bool dlgResult = false;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = await this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0)
                {
                    // TODO: Resource strings
                    dlgResult = await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("New Item", "A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "Yes", "No"));
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    dlgResult = true;
                }

                if (dlgResult)
                {
                    await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(async (transaction) =>
                    {
                        try
                        {
                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)",
                                Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
                                Convert.ToInt32(SysCommon.ActStatus.NoChange),
                                Convert.ToInt32(this.ActHdr.Refno));

                            this.Refno = Convert.ToInt32(transaction.QueryScalars<int>("SELECT last_insert_rowid() FROM tActivityItem"));

                            string querytItemProduction = "INSERT INTO tActivityItemProduction (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ProductUnitRefno, " +
                                "LocationMapRefno, " +
                                "ActivityItemProductionQuantity, " +
                                "ActivityItemProductionBaseQuantity, " +
                                "ActivityItemProductionWeightQuantity, " +
                                "ActivityItemProductionDescription, " +
                                "ActivityItemProductionQuantityExpected, " +
                                "ActivityItemProductionBaseQuantityExpected, " +
                                "ActivityItemProductionWeightQuantityExpected" +
                                ") VALUES (" +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?, " +
                                "?" +
                                ")";
                            List<object> parametersItemProduction = new List<object>()
                            {
                                Convert.ToInt32(this.Refno),
                                Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished),
                                Convert.ToInt32(prodUnitRefno),
                                Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToString(data.Root.Element("ProductDescription").Value.ToString().TrimEnd() + " " + data.Root.Element("ProductUnitSizeDescription").Value),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0)
                            };

                            transaction.Execute(querytItemProduction, parametersItemProduction);

                            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                            this.FKRefno = this.Refno * -1;

                            transaction.Execute("INSERT INTO tActivityItemProductionFinished (ActivityItemRefno, ActivityItemProductionFinishedFKRefno, ActivityItemProductionFinishedStringFKRefno) VALUES (?, ?, ?)",
                                Convert.ToInt32(this.Refno),
                                Convert.ToInt32(this.FKRefno),
                                Convert.ToString(this.FKRefno));
                            transaction.Commit();

                            if (this.ListItems != null)
                            {
                                this.ListItems.Add(new ProductionTicketFinishedItem()
                                {
                                    ActivityItemRefno = this.Refno,
                                    ActivityItemProductionFinishedFKRefno = this.FKRefno,
                                    ActivityItemProductionFinishedStringFKRefno = this.FKRefno.ToString(),
                                    ProductUnitRefno = prodUnitRefno,
                                    ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                                    ProductCode = data.Root.Element("ProductCode").Value,
                                    ProductDescription = data.Root.Element("ProductDescription").Value,
                                    BaseUnitCode = data.Root.Element("BaseUnitCode").Value,
                                    ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                                    ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                                    LocationMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno,
                                    LocationCode = this.LocMap.CurrLocMap.WhsCode,
                                    LocationName = this.LocMap.CurrLocMap.WhsName,
                                    ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                                    ActivityItemProductionQuantity = 0,
                                    ActivityItemProductionBaseQuantity = 0,
                                    ActivityItemProductionWeightQuantity = 0,
                                    ActivityItemProductionQuantityExpected = 0,
                                    ActivityItemProductionBaseQuantityExpected = 0,
                                    ActivityItemProductionWeightQuantityExpected = 0
                                });
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            // TODO: Logging
                            Console.WriteLine(ex);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityProductionTicketFinishedItem tableFinishedItem = new TableActivityProductionTicketFinishedItem();
                TableActivityProductionTicketFinishedDetail tableFinishedDetail = new TableActivityProductionTicketFinishedDetail();
                await tableFinishedDetail.Purge();
                await tableFinishedItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableFinishedItem.RegisterDataAccessListenerProgressOut((TicketFinishedActItem)this);
                await tableFinishedItem.Request(ActHdr.Refno);
                tableFinishedItem.UnregisterDataAccessListenerProgressOut((TicketFinishedActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Add all items in dataview to observable collection for display
            foreach (ProductionTicketFinishedItem productionTicketFinishedItem in this.ListItems)
            {
                //ProductionTicketFinishedItem productionTicketFinishedItem = new ProductionTicketFinishedItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                //    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                //    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                //    ActivityItemProductionFinishedFKRefno = Convert.ToInt32(row["ActivityItemProductionFinishedFKRefno"] != DBNull.Value ? row["ActivityItemProductionFinishedFKRefno"] : null),
                //    ActivityItemProductionFinishedStringFKRefno = Convert.ToString(row["ActivityItemProductionFinishedStringFKRefno"] != DBNull.Value ? row["ActivityItemProductionFinishedStringFKRefno"] : null),
                //    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                //    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                //};

                observableCollection.Add(productionTicketFinishedItem);
            }

            // TODO: Resource strings
            Label labelCodeHeader = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelExpQtyHeader = new Label { Text = "Exp Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCodeHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelExpQtyHeader.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelCodeHeader);
            gridHeader.Children.Add(labelBCHeader);
            gridHeader.Children.Add(labelUnitHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelExpQtyHeader);

            // Set label columns
            Grid.SetColumn(labelCodeHeader, 0);
            Grid.SetColumn(labelBCHeader, 1);
            Grid.SetColumn(labelUnitHeader, 2);
            Grid.SetColumn(labelQtyHeader, 3);
            Grid.SetColumn(labelExpQtyHeader, 4);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (ProductionTicketFinishedItem productionTickedFinishedItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTickedFinishedItem.ProductCode));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTickedFinishedItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTickedFinishedItem.BaseUnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", productionTickedFinishedItem.ActivityItemProductionBaseQuantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", productionTickedFinishedItem.ActivityItemProductionBaseQuantityExpected));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno AS ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno AS ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno AS ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno AS ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity AS ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity AS ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity AS ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected AS ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected AS ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected AS ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapSlot.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode + \'\\\' + tLocationMapSlot.LocationMapSlotCode ");
                sql.Append("AS LocationCode,");
                sql.Append(" \' \'  AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("tLocationMapSection.LocationMapSectionName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
                sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode AS BaseUnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantity,");
                sql.Append("tActivityItemProduction.ActivityItemProductionQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionBaseQuantityExpected,");
                sql.Append("tActivityItemProduction.ActivityItemProductionWeightQuantityExpected,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewItems = ds.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //        TODO: Fix sorting
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            //}

            this.ListItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(sql.ToString());
            return this.ListItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActItemFilter))
            {
                this.ListItems = (List<object>)this.ListItems.OfType<ProductionTicketFinishedItem>().Where(productionTicketFinishedItem => productionTicketFinishedItem.ProductCode.Contains(searchText.Trim()));
            }
            else
            {
                // TODO: Fix filtering
                //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }

            // Add all items in dataview to observable collection for display
            foreach (ProductionTicketFinishedItem productionTicketFinishedItem in this.ListItems)
            {
                //ProductionTicketFinishedItem productionTicketFinishedItem = new ProductionTicketFinishedItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                //    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                //    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                //    ActivityItemProductionFinishedFKRefno = Convert.ToInt32(row["ActivityItemProductionFinishedFKRefno"] != DBNull.Value ? row["ActivityItemProductionFinishedFKRefno"] : null),
                //    ActivityItemProductionFinishedStringFKRefno = Convert.ToString(row["ActivityItemProductionFinishedStringFKRefno"] != DBNull.Value ? row["ActivityItemProductionFinishedStringFKRefno"] : null),
                //    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                //    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                //};

                observableCollection.Add(productionTicketFinishedItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                await base.SelectActivityItem(productionTicketFinishedItem);
                this.FKRefno = Convert.ToInt32(productionTicketFinishedItem.ActivityItemProductionFinishedFKRefno);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemProductionFinished.ActivityItemProductionFinishedFKRefno,");
            sql.Append("tActivityItemProduction.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemProduction ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tActivityItemProductionFinished ON tActivityItemProductionFinished.ActivityItemRefno ");
            sql.Append("= tActivityItemProduction.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemProduction.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemProduction.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemProduction.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", Convert.ToInt32(SysCommon.ActSubTypes.TicketFinished));

            List<ProductionTicketFinishedItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductionTicketFinishedItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(results[0].LocationMapRefno));
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemProductionFinishedFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.Refno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string strBaseUnitCode = data.Root.Element("BaseUnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRODUCTION TICKET FINISHED ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + barCodeCode + "</gtin>")
                + "<whs_id></whs_id>"
                + "<sect_refno></sect_refno>"
                + "<slot_refno></slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                productionTicketFinishedItem.ActivityItemProductionQuantity = quantity;
                productionTicketFinishedItem.ActivityItemProductionBaseQuantity = baseQuantity;
                productionTicketFinishedItem.ActivityItemProductionWeightQuantity = weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                productionTicketFinishedItem.ActivityItemProductionQuantity += quantity;
                productionTicketFinishedItem.ActivityItemProductionBaseQuantity += baseQuantity;
                productionTicketFinishedItem.ActivityItemProductionWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="activityItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is ProductionTicketFinishedItem productionTicketFinishedItem)
            {
                string query = "UPDATE tActivityItemProduction SET ActivityItemProductionQuantity = ?, ActivityItemProductionBaseQuantity = ?, ActivityItemProductionWeightQuantity = ? WHERE ActivityItemRefno = ?";
                List<object> parameters = new List<object>() { productionTicketFinishedItem.ActivityItemProductionQuantity, productionTicketFinishedItem.ActivityItemProductionBaseQuantity, productionTicketFinishedItem.ActivityItemProductionWeightQuantity, actItemRefno };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        #endregion
    }
}
