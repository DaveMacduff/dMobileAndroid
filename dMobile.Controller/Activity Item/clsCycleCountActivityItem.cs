namespace dMobile.Controller
{
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using SQLite;
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using System.Collections.Generic;
    using System.Linq;

    public class CycleActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<CountItem> listCountItems = new List<CountItem>();

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="activity"></param>
        /// <remarks></remarks>
        public CycleActItem(Activity activity)
            : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Cycle;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsCycleCreateNewItemNoAsk;
            this.FullClassName = typeof(CycleActItem).FullName;

            DataTemplate cycleCountTemplate = new DataTemplate(() =>
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
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                    }
                };

                Label labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityItemCountQuantityOnHand = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityItemCountQuantity = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityItemCountWeightQuantityOnHand = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemCountQuantityOnHand.SetBinding(Label.TextProperty, new Binding("ActivityItemCountQuantityOnHand", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemCountQuantity.SetBinding(Label.TextProperty, new Binding("ActivityItemCountQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemCountWeightQuantityOnHand.SetBinding(Label.TextProperty, new Binding("ActivityItemCountWeightQuantityOnHand", BindingMode.Default, null, null, "{0:F3}"));

                // Add labels to grid
                gridData.Children.Add(labelProductCode);
                gridData.Children.Add(labelProductUnitBC);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelActivityItemCountQuantityOnHand);
                gridData.Children.Add(labelActivityItemCountQuantity);
                gridData.Children.Add(labelActivityItemCountWeightQuantityOnHand);

                // Set label columns
                Grid.SetColumn(labelProductCode, 0);
                Grid.SetColumn(labelProductUnitBC, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelActivityItemCountQuantityOnHand, 3);
                Grid.SetColumn(labelActivityItemCountQuantity, 4);
                Grid.SetColumn(labelActivityItemCountWeightQuantityOnHand, 5);

                return new ViewCell { View = gridData };
            });

            resourceDictionary.Add("cycleCountTemplate", cycleCountTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsCycleCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsCycleCountNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable To Parse Count Item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable To Find Count Item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemCountStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int foundActItemRefno = 0;
            bool result = false;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int locMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;

            if (this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
            {
                locMapRefno = this.LocMap.CurrLocMapRefno;
            }

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = await this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0)
                {
                    //result = MessageBox.Show("A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "New Item", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                    // TODO: Resource strings
                    result = await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("New Item", "A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "Yes", "No"));
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    result = true;
                }

                if (result)
                {
                    await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                    {
                        try
                        {
                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts), Convert.ToInt32(SysCommon.ActStatus.NoChange), Convert.ToInt32(this.ActHdr.Refno));
                            this.Refno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                            this.FKRefno = this.Refno * -1;

                            string query = "INSERT INTO tActivityItemCount (" +
                                    "ActivityItemRefno, " +
                                    "ActivityItemSubType, " +
                                    "ActivityItemCountFKRefno, " +
                                    "ActivityItemCountStringFKRefno, " +
                                    "ProductUnitRefno, " +
                                    "LocationMapRefno, " +
                                    "ActivityItemCountQuantityOnHand, " +
                                    "ActivityItemCountBaseQuantityOnHand, " +
                                    "ActivityItemCountWeightQuantityOnHand, " +
                                    "ActivityItemCountQuantity, " +
                                    "ActivityItemCountBaseQuantity, " +
                                    "ActivityItemCountWeightQuantity" +
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
                                    "?, " +
                                    "?" +
                                    ")";
                            object[] parameters = new object[]
                            {
                                Convert.ToInt32(this.Refno),
                                Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts),
                                Convert.ToInt32(this.FKRefno),
                                Convert.ToString(this.FKRefno),
                                Convert.ToInt32(prodUnitRefno),
                                Convert.ToInt32(locMapRefno),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0),
                                Convert.ToDecimal(0)
                            };

                            transaction.Execute(query, parameters);
                            transaction.Commit();
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            // TOOD: Logging
                            Console.WriteLine(ex);
                        }
                    });

                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                    this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                    if (this.ListItems != null)
                    {
                        this.ListItems.Add(new CountItem()
                        {
                            ActivityItemRefno = this.Refno,
                            ActivityItemCountFKRefno = this.FKRefno,
                            ActivityItemCountStringFKRefno = this.FKRefno.ToString(),
                            ProductUnitRefno = prodUnitRefno,
                            ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                            ProductCode = data.Root.Element("ProductCode").Value,
                            ProductDescription = data.Root.Element("ProductDescription").Value,
                            UnitCode = data.Root.Element("UnitCode").Value,
                            ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                            ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                            LocationMapRefno = locMapRefno,
                            LocationCode = Convert.ToString(this.LocMap.CurrLocMap.WhsCode),
                            LocationName = this.LocMap.CurrLocMap.WhsName,
                            ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            ActivityItemCountQuantityOnHand = 0,
                            ActivityItemCountBaseQuantityOnHand = 0,
                            ActivityItemCountWeightQuantityOnHand = 0,
                            ActivityItemCountQuantity = 0,
                            ActivityItemCountBaseQuantity = 0,
                            ActivityItemCountWeightQuantity = 0
                        });
                    }
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

                TableActivityCycleCountItem tableActCountItem = new TableActivityCycleCountItem();
                TableActivityCycleCountDetail tableActCountDetail = new TableActivityCycleCountDetail();
                await tableActCountDetail.Purge();
                await tableActCountItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActCountItem.RegisterDataAccessListenerProgressOut((CycleActItem)this);
                await tableActCountItem.Request(this.ActHdr.Refno);
                tableActCountItem.UnregisterDataAccessListenerProgressOut((CycleActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Add all items in dataview to observable collection for display
            foreach (CountItem countItem in this.ListItems)
            {
                //CountItem countItem = new CountItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"] != DBNull.Value ? row["ActivityItemCountFKRefno"] : null),
                //    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"] != DBNull.Value ? row["ActivityItemCountStringFKRefno"] : null),
                //    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountQuantityOnHand"] : null),
                //    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountBaseQuantityOnHand"] : null),
                //    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountWeightQuantityOnHand"] : null),
                //    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"] != DBNull.Value ? row["ActivityItemCountQuantity"] : null),
                //    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"] != DBNull.Value ? row["ActivityItemCountBaseQuantity"] : null),
                //    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"] != DBNull.Value ? row["ActivityItemCountWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                //};

                observableCollection.Add(countItem);
            }

            // TODO: Resource strings
            Label labelCountCode = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountBC = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountUnit = new Label { Text = "", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountOnHand = new Label { Text = "On Hand", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountCounted = new Label { Text = "Counted", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountWtOn = new Label { Text = "Wt On", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountCode.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountBC.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountUnit.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountOnHand.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountCounted.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountWtOn.Text);

            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelCountCode);
            gridHeader.Children.Add(labelCountBC);
            gridHeader.Children.Add(labelCountUnit);
            gridHeader.Children.Add(labelCountOnHand);
            gridHeader.Children.Add(labelCountCounted);
            gridHeader.Children.Add(labelCountWtOn);

            // Set label columns
            Grid.SetColumn(labelCountCode, 0);
            Grid.SetColumn(labelCountBC, 1);
            Grid.SetColumn(labelCountUnit, 2);
            Grid.SetColumn(labelCountOnHand, 3);
            Grid.SetColumn(labelCountCounted, 4);
            Grid.SetColumn(labelCountWtOn, 5);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (CountItem countItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(countItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(countItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(countItem.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", countItem.ActivityItemCountQuantityOnHand));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", countItem.ActivityItemCountQuantity));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", countItem.ActivityItemCountWeightQuantityOnHand));

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

                if (detailWidth5 > columnWidth5)
                {
                    columnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = columnWidth5;
                }
            }

            try
            {
                dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["cycleCountTemplate"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

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
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno AS ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno AS ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand AS ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand AS ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand AS ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity AS ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity AS ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity AS ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno AS LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("\' \' AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append(" SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(\'\' AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> (0) ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemCount.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno AS ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno AS ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand AS ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand AS ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand AS ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity AS ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity AS ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity AS ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSlot.LocationMapRefno AS LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName + \' \' + tLocationMapSlot.LocationMapSlotName ");
                sql.Append("AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("\' \' AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapSection.LocationMapRefno,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("UNION ");
                sql.Append(" SELECT tActivityItemCount.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantityOnHand,");
                sql.Append("tActivityItemCount.ActivityItemCountQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountBaseQuantity,");
                sql.Append("tActivityItemCount.ActivityItemCountWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseCode AS NVARCHAR(20)) AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemCount ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemCount.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemCount.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemCount.LocationMapRefno ");
                sql.AppendFormat(" WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemCount.ActivityItemRefno");
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
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            //}

            this.listCountItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountItem>(sql.ToString());
            this.ListItems = this.listCountItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            //if (string.IsNullOrEmpty(this.ActItemFilter))
            //{
            //this.DataViewItems.RowFilter = "ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            this.ListItems = this.listCountItems.Where(countItem => countItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            //}
            //else
            //{
            // TODO: Fix filtering
            //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            //}

            foreach (CountItem countItem in this.ListItems)
            {
                //CountItem countItem = new CountItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"] != DBNull.Value ? row["ActivityItemCountFKRefno"] : null),
                //    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"] != DBNull.Value ? row["ActivityItemCountStringFKRefno"] : null),
                //    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountQuantityOnHand"] : null),
                //    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountBaseQuantityOnHand"] : null),
                //    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountWeightQuantityOnHand"] : null),
                //    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"] != DBNull.Value ? row["ActivityItemCountQuantity"] : null),
                //    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"] != DBNull.Value ? row["ActivityItemCountBaseQuantity"] : null),
                //    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"] != DBNull.Value ? row["ActivityItemCountWeightQuantity"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                //};

                observableCollection.Add(countItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is CountItem countItem)
            {
                if (this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation)
                {
                    this.SelectedActivityItem = countItem;
                    this.Refno = Convert.ToInt32(countItem.ActivityItemRefno);
                    this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(countItem.ProductUnitRefno));
                    this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                    // Do not change the location to the skid location leave it alone for IsCycleDoNotDefaultProductLocation.
                    // Clear the skid if the product changes.
                    if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo != null && SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid)
                    {
                        if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ProdInfo.UnitRefno)
                        {
                            this.ActHdr.SkidInfo = new SysSkidInfo();
                        }
                    }
                }
                else
                {
                    await base.SelectActivityItem(countItem);
                }

                this.FKRefno = Convert.ToInt32(countItem.ActivityItemCountFKRefno);
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

            sql.Append("SELECT tActivityItem.ActivityItemRefno AS ActivityItemRefno,");
            sql.Append("tActivityItemCount.ActivityItemCountFKRefno AS ActivityItemCountFKRefno,");
            sql.Append("tActivityItemCount.ProductUnitRefno AS ProductUnitRefno,");
            sql.Append("tLocationMapSlot.LocationMapRefno AS LocationMapRefno ");
            sql.Append("FROM tActivityItemCount ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemCount.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
            sql.Append("= tActivityItemCount.LocationMapRefno ");
            sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
            sql.Append("= tLocationMapSlot.SectionLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.SlotLocMapRefno);
            sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.Append("UNION ");
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
            sql.Append("tActivityItemCount.ProductUnitRefno,");
            sql.Append("tLocationMapSection.LocationMapRefno ");
            sql.Append("FROM tActivityItemCount ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemCount.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
            sql.Append("= tActivityItemCount.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.SectLocMapRefno);
            sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0} ", prodUnitRefno);
            sql.Append("UNION ");
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
            sql.Append("tActivityItemCount.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemCount ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemCount.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemCount.LocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemCount.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemCount.ProductUnitRefno = {0}", prodUnitRefno);

            List<CountItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemCountFKRefno);
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
        public async override Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CYCLE COUNT ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
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
            if (selectedActivityItem is CountItem countItem)
            {
                countItem.ActivityItemCountQuantity = quantity;
                countItem.ActivityItemCountBaseQuantity = baseQuantity;
                countItem.ActivityItemCountWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is CountItem countItem)
            {
                countItem.ActivityItemCountQuantity += quantity;
                countItem.ActivityItemCountBaseQuantity += baseQuantity;
                countItem.ActivityItemCountWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is CountItem countItem)
            {
                string query = "UPDATE tActivityItemCount SET ActivityItemCountQuantity = ?, ActivityItemCountBaseQuantity = ?, ActivityItemCountWeightQuantity = ? WHERE ActivityItemRefno = ?";
                object[] parameters = new object[] { countItem.ActivityItemCountQuantity, countItem.ActivityItemCountBaseQuantity, countItem.ActivityItemCountWeightQuantity, actItemRefno };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }

            #endregion
        }
    }
}
