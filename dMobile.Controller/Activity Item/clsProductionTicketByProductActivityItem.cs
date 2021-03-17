namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class TicketByProdActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public TicketByProdActItem(Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2TicketByProduct;
            this.FullClassName = typeof(TicketByProdActItem).FullName;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsTicketByProductCreateNewItemNoAsk;

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
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelExpQty.SetBinding(Label.TextProperty, new Binding("ActivityItemProductionWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));

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
        /// <remarks></remarks>
        public override bool IsCatchAsk()
        {
            bool isCatchAsk = base.IsCatchAsk();
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsTicketByProductCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsTicketByProductNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="prodRefno"></param>
        /// <returns></returns>
        public override int GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(whsLocMapRefno, prodRefno, SysCommon.ProdLocMapTypes.Receiving);

            return locMapRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse production item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
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
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemProductionByProductStringFKRefno";
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
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

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
                    SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                    using (SQLiteCommand cmndActItemByProduct = conn.CreateCommand())
                    using (SQLiteCommand cmndActProdItem = conn.CreateCommand())
                    using (SQLiteCommand cmndItem = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //using (SqlCeResultSet rsActItemByProduct = cmndActItemByProduct.ExecuteResultSet(ResultSetOptions.Updatable))
                            //SqlCeUpdatableRecord recActItemByProduct = rsActItemByProduct.CreateRecord();

                            //using (SqlCeResultSet rsActItemProd = cmndActProdItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //SqlCeUpdatableRecord recActItemProd = rsActItemProd.CreateRecord();

                            //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                            cmndItem.ExecuteNonQuery();

                            //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            //rsActItem.Insert(recActItem);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

                            cmndActProdItem.CommandText = "INSERT INTO tActivityItemProduction (" +
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
                                "@activityItemRefno, " +
                                "@activityItemSubType, " +
                                "@productUnitRefno, " +
                                "@locationMapRefno, " +
                                "@activityItemProductionQuantity, " +
                                "@activityItemProductionBaseQuantity, " +
                                "@activityItemProductionWeightQuantity, " +
                                "@activityItemProductionDescription, " +
                                "@activityItemProductionQuantityExpected, " +
                                "@activityItemProductionBaseQuantityExpected, " +
                                "@activityItemProductionWeightQuantityExpected" +
                                ")";
                            cmndActProdItem.CommandType = CommandType.Text;
                            //cmndActProdItem.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@locationMapRefno", Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantity", Convert.ToDecimal(0)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantity", Convert.ToDecimal(0)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionWeightQuantity", Convert.ToDecimal(0)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionDescription", Convert.ToString(data.Root.Element("ProductDescription").Value.ToString().TrimEnd() + " " + data.Root.Element("ProductUnitSizeDescription").Value)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionQuantityExpected", Convert.ToDecimal(0)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionBaseQuantityExpected", Convert.ToDecimal(0)));
                            cmndActProdItem.Parameters.Add(new SQLiteParameter("@activityItemProductionWeightQuantityExpected", Convert.ToDecimal(0)));

                            cmndActProdItem.ExecuteNonQuery();

                            //recActItemProd.SetInt32((int)SysCommon.ActItemProdFields.ActItemRefno, this.Refno);
                            //recActItemProd.SetInt32((int)SysCommon.ActItemProdFields.ActivityItemSubType, (int)SysCommon.ActSubTypes.TicketAddProduct);
                            //recActItemProd.SetInt32((int)SysCommon.ActItemProdFields.ProdUnitRefno, prodUnitRefno);
                            //recActItemProd.SetInt32((int)SysCommon.ActItemProdFields.LocMapRefno, this.LocMap.CurrLocMap.WhsLocMapRefno);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.Quantity, 0);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantity, 0);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantity, 0);
                            //string desc = data.Root.Element("ProductDescription").Value.ToString().TrimEnd() + " " + data.Root.Element("ProductUnitSizeDescription").Value;
                            //recActItemProd.SetString((int)SysCommon.ActItemProdFields.Description, desc);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.QuantityExpected, 0);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.BaseQuantityExpected, 0);
                            //recActItemProd.SetDecimal((int)SysCommon.ActItemProdFields.WeightQuantityExpected, 0);
                            //rsActItemProd.Insert(recActItemProd);

                            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                            this.FKRefno = this.Refno * -1;

                            cmndActItemByProduct.CommandText = "INSERT INTO tActivityItemProductionByProduct (ActivityItemRefno, ActivityItemProductionByProductFKRefno, ActivityItemProductionByProductStringFKRefno) VALUES (@activityItemRefno, @activityItemProductionByProductFKRefno, @activityItemProductionByProductStringFKRefno)";
                            cmndActItemByProduct.CommandType = CommandType.Text;
                            //cmndActItemByProduct.IndexName = "UQ_ActivityItemProductionByProductOnActivityItemProductionByProductFKRefno";
                            cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemProductionByProductFKRefno", Convert.ToInt32(this.FKRefno)));
                            cmndActItemByProduct.Parameters.Add(new SQLiteParameter("@activityItemProductionByProductStringFKRefno", Convert.ToString(this.FKRefno)));

                            cmndActItemByProduct.ExecuteNonQuery();

                            //recActItemByProduct.SetInt32((int)SysCommon.ActItemProdByProductFields.ActItemRefno, this.Refno);
                            //recActItemByProduct.SetInt32((int)SysCommon.ActItemProdByProductFields.FKRefno, this.FKRefno);
                            //recActItemByProduct.SetString((int)SysCommon.ActItemProdByProductFields.StringFKRefno, this.FKRefno.ToString());
                            //rsActItemByProduct.Insert(recActItemByProduct);

                            transaction.Commit();

                            if (this.DataViewItems != null)
                            {
                                DataRowView rowView = this.DataViewItems.AddNew();
                                rowView["ActivityItemRefno"] = this.Refno;
                                rowView["ActivityItemProductionByProductFKRefno"] = this.FKRefno;
                                rowView["ActivityItemProductionByProductStringFKRefno"] = this.FKRefno.ToString();
                                rowView["ProductUnitRefno"] = prodUnitRefno;
                                rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
                                rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
                                rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
                                rowView["BaseUnitCode"] = data.Root.Element("BaseUnitCode").Value;
                                rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
                                rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
                                rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
                                rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                                rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                                rowView["ActivityItemProductionQuantity"] = 0;
                                rowView["ActivityItemProductionBaseQuantity"] = 0;
                                rowView["ActivityItemProductionWeightQuantity"] = 0;
                                rowView["ActivityItemProductionQuantityExpected"] = 0;
                                rowView["ActivityItemProductionBaseQuantityExpected"] = 0;
                                rowView["ActivityItemProductionWeightQuantityExpected"] = 0;
                                rowView.EndEdit();
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            // TODO: Logging
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityProductionTicketByProductItem tableByProdItem = new TableActivityProductionTicketByProductItem();
                TableActivityProductionTicketByProductDetail tableByProdDetail = new TableActivityProductionTicketByProductDetail();
                tableByProdDetail.Purge();
                tableByProdItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableByProdItem.RegisterDataAccessListenerProgressOut((TicketByProdActItem)this);
                tableByProdItem.Request(ActHdr.Refno);
                tableByProdItem.UnregisterDataAccessListenerProgressOut((TicketByProdActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                ProductionTicketByProductItem productionTicketByProductItem = new ProductionTicketByProductItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                    ActivityItemProductionByProductFKRefno = Convert.ToInt32(row["ActivityItemProductionByProductFKRefno"] != DBNull.Value ? row["ActivityItemProductionByProductFKRefno"] : null),
                    ActivityItemProductionByProductStringFKRefno = Convert.ToString(row["ActivityItemProductionByProductStringFKRefno"] != DBNull.Value ? row["ActivityItemProductionByProductStringFKRefno"] : null),
                    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                };

                observableCollection.Add(productionTicketByProductItem);
            }

            // TODO: Resource strings
            Label labelCodeHeader = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelExpQtyHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

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

            foreach (ProductionTicketByProductItem productionTicketByProductItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(productionTicketByProductItem.ProductCode));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicketByProductItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionTicketByProductItem.BaseUnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", productionTicketByProductItem.ActivityItemProductionQuantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", productionTicketByProductItem.ActivityItemProductionWeightQuantity));

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

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int columnWidthFactor = 1;

        //      if (this.SysEnv.IsWindowsMobile6Device)
        //      {
        //        columnWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemRefno";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = -1;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductCode";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
        //#else
        //      dataGridCol.HeaderText = "Code";
        //#endif
        //      dataGridCol.Width = 62 * columnWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductUnitBC";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
        //#else
        //      dataGridCol.HeaderText = "BC";
        //#endif
        //      dataGridCol.Width = 20 * columnWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "UnitCode";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = 22 * columnWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemProductionQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //      dataGridCol.HeaderText = "Qty";
        //#endif
        //      dataGridCol.Width = 45 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemProductionWeightQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
        //#else
        //      dataGridCol.HeaderText = "Weight";
        //#endif
        //      dataGridCol.Width = 45 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductFKRefno AS ActivityItemProductionByProductFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductStringFKRefno AS ActivityItemProductionByProductStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.AppendFormat(
                    "AND tActivityDetail.ActivityDetailSubType = {0} ",
                    Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.AppendFormat("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemProduction.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductFKRefno AS ActivityItemProductionByProductFKRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductStringFKRefno AS ActivityItemProductionByProductStringFKRefno,");
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
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemProduction.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.TicketAddProduct));
                sql.Append("ORDER BY tActivityItemProduction.ActivityItemRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    this.DataViewItems = ds.Tables[0].DefaultView;
                    this.DataViewItems.Table.TableName = this.MainGridMappingName;
                    this.DataViewItems.AllowNew = true;
                    this.DataViewItems.AllowEdit = true;
                    this.DataViewItems.AllowDelete = true;
                    this.DataViewItems.Sort = "ActivityItemRefno";
                }
            }

            return this.DataViewItems;
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
                this.DataViewItems.RowFilter = "ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }

            // Add all items in dataview to observable collection for display
            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                ProductionTicketByProductItem productionTicketByProductItem = new ProductionTicketByProductItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemProductionQuantity = Convert.ToDecimal(row["ActivityItemProductionQuantity"] != DBNull.Value ? row["ActivityItemProductionQuantity"] : null),
                    ActivityItemProductionBaseQuantity = Convert.ToDecimal(row["ActivityItemProductionBaseQuantity"] != DBNull.Value ? row["ActivityItemProductionBaseQuantity"] : null),
                    ActivityItemProductionWeightQuantity = Convert.ToDecimal(row["ActivityItemProductionWeightQuantity"] != DBNull.Value ? row["ActivityItemProductionWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                    ActivityItemProductionBaseQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionBaseQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionBaseQuantityExpected"] : null),
                    ActivityItemProductionByProductFKRefno = Convert.ToInt32(row["ActivityItemProductionByProductFKRefno"] != DBNull.Value ? row["ActivityItemProductionByProductFKRefno"] : null),
                    ActivityItemProductionByProductStringFKRefno = Convert.ToString(row["ActivityItemProductionByProductStringFKRefno"] != DBNull.Value ? row["ActivityItemProductionByProductStringFKRefno"] : null),
                    ActivityItemProductionQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionQuantityExpected"] : null),
                    ActivityItemProductionWeightQuantityExpected = Convert.ToDecimal(row["ActivityItemProductionWeightQuantityExpected"] != DBNull.Value ? row["ActivityItemProductionWeightQuantityExpected"] : null),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null)
                };

                observableCollection.Add(productionTicketByProductItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectItem(int idx)
        {
            base.SelectItem(idx);
            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemProductionByProductFKRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;

            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandType = CommandType.Text;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemProductionByProduct.ActivityItemProductionByProductFKRefno,");
                sql.Append("tActivityItemProduction.ProductUnitRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("FROM tActivityItemProduction ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityItemProductionByProduct ");
                sql.Append("ON tActivityItemProductionByProduct.ActivityItemRefno ");
                sql.Append("= tActivityItemProduction.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemProduction.LocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItemProduction.LocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat("AND tActivityItemProduction.ProductUnitRefno = {0} ", prodUnitRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0}", (int)SysCommon.ActSubTypes.TicketAddProduct);

                cmndItem.CommandText = sql.ToString();

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemProductionByProductFKRefno"]);
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(ProdInfo.Refno);
                        foundActItemRefno = this.Refno;
                    }

                    drItem.Close();
                }
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
                + "<action>SUBMIT PRODUCTION TICKET BYPRODUCT ITEM</action>"
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
        public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemProductionQuantity"] = quantity;
            this.DataViewItems[idx]["ActivityItemProductionBaseQuantity"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemProductionWeightQuantity"] = weightQuantity;
            this.DataViewItems[idx].EndEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            DataRowView rowView = this.DataViewItems[idx];
            rowView.BeginEdit();
            rowView["ActivityItemProductionQuantity"] = Convert.ToDecimal(DataViewItems[idx]["ActivityItemProductionQuantity"]) + quantity;
            rowView["ActivityItemProductionBaseQuantity"] = Convert.ToDecimal(DataViewItems[idx]["ActivityItemProductionBaseQuantity"]) + baseQuantity;
            rowView["ActivityItemProductionWeightQuantity"] = Convert.ToDecimal(DataViewItems[idx]["ActivityItemProductionWeightQuantity"]) + weightQuantity;
            rowView.EndEdit();

            return idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "tActivityItemProduction";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemProduction "
                    + "SET ActivityItemProductionQuantity = " + this.DataViewItems[idx]["ActivityItemProductionQuantity"] + ","
                    + "ActivityItemProductionBaseQuantity = " + this.DataViewItems[idx]["ActivityItemProductionBaseQuantity"] + ","
                    + "ActivityItemProductionWeightQuantity = " + this.DataViewItems[idx]["ActivityItemProductionWeightQuantity"]
                    + " WHERE ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
