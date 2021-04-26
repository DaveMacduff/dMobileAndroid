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

    public class AdjActItem : ActItem
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
        /// <remarks></remarks>
        public AdjActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(AdjActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Adjustments;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsAdjustmentCreateNewItemNoAsk;

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

                Label labelProductID = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWhs = new Label { VerticalOptions = LayoutOptions.Center };

                // Set label bindings
                labelProductID.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemAdjustmentQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWhs.SetBinding(Label.TextProperty, new Binding("LocationCode"));

                // Add labels to grid
                gridData.Children.Add(labelProductID);
                gridData.Children.Add(labelProductUnitBC);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWhs);

                // Set label columns
                Grid.SetColumn(labelProductID, 0);
                Grid.SetColumn(labelProductUnitBC, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelQty, 3);
                Grid.SetColumn(labelWhs, 4);

                return new ViewCell { View = gridData };
            });

            resourceDictionary.Add("dataTemplate", dataTemplate);
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
                isCatchAsk = this.SysEnv.SysSettings.IsAdjustmentCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsAdjustmentNoCatchAskZeroEnabled;
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
            return string.Format("Unable to parse adjustment item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find adjustment item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        ///
        /// </summary>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemAdjustmentStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int foundActItemRefno = 0;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            bool result = false;

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0 && askToInsertAsNewItem)
                {
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
                    SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                    using (SQLiteCommand cmndActAdjItem = conn.CreateCommand())
                    using (SQLiteCommand cmndItem = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //cmndActAdjItem.IndexName = "UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno";
                            //using (SqlCeResultSet rs = cmndActAdjItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();

                            //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                            cmndItem.ExecuteNonQuery();

                            //recActItem.SetInt32(Convert.ToInt32(SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Adjustment));
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            //rsActItem.Insert(recActItem);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
                            this.FKRefno = this.Refno * -1;

                            cmndActAdjItem.CommandText = "INSERT INTO tActivityItemAdjustment (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemAdjustmentFKRefno, " +
                                "ActivityItemAdjustmentStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityItemAdjustmentQuantity, " +
                                "ActivityItemAdjustmentBaseQuantity, " +
                                "ActivityItemAdjustmentWeightQuantity, " +
                                "ActivityItemAdjustmentDescription" +
                                ") VALUES (" +
                                "@activityItemRefno, " +
                                "@activityItemSubType, " +
                                "@activityItemAdjustmentFKRefno, " +
                                "@activityItemAdjustmentStringFKRefno, " +
                                "@productUnitRefno, " +
                                "@warehouseLocationMapRefno, " +
                                "@activityItemAdjustmentQuantity, " +
                                "@activityItemAdjustmentBaseQuantity, " +
                                "@activityItemAdjustmentWeightQuantity, " +
                                "@activityItemAdjustmentDescription" +
                                ")";
                            cmndActAdjItem.CommandType = CommandType.Text;
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Adjustment)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentFKRefno", Convert.ToInt32(this.FKRefno)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentStringFKRefno", Convert.ToString(this.FKRefno)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentQuantity", Convert.ToDecimal(0)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentBaseQuantity", Convert.ToDecimal(0)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentWeightQuantity", Convert.ToDecimal(0)));
                            cmndActAdjItem.Parameters.Add(new SQLiteParameter("@activityItemAdjustmentDescription", Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc)));

                            cmndActAdjItem.ExecuteNonQuery();

                            transaction.Commit();

                            //rec.SetInt32((int)SysCommon.ActItemAdjFields.ActItemRefno, this.Refno);
                            //rec.SetInt32((int)SysCommon.ActItemAdjFields.ActItemSubType, (int)SysCommon.ActSubTypes.Adjustment);
                            //rec.SetInt32((int)SysCommon.ActItemAdjFields.FKRefno, this.FKRefno);
                            //rec.SetString((int)SysCommon.ActItemAdjFields.StringFKRefno, this.FKRefno.ToString());
                            //rec.SetInt32((int)SysCommon.ActItemAdjFields.ProdUnitRefno, prodUnitRefno);
                            //rec.SetInt32((int)SysCommon.ActItemAdjFields.WhsLocMapRefno,this.LocMap.CurrLocMap.WhsLocMapRefno);
                            //rec.SetDecimal((int)SysCommon.ActItemAdjFields.Quantity, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemAdjFields.BaseQuantity, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemAdjFields.WeightQuantity, 0);
                            //rec.SetString((int)SysCommon.ActItemAdjFields.Description, this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc);
                            //rs.Insert(rec);

                            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                            if (this.DataViewItems != null)
                            {
                                DataRowView rowView = this.DataViewItems.AddNew();
                                rowView["ActivityItemRefno"] = this.Refno;
                                rowView["ActivityItemAdjustmentFKRefno"] = this.FKRefno;
                                rowView["ActivityItemAdjustmentStringFKRefno"] = this.FKRefno.ToString();
                                rowView["ProductUnitRefno"] = prodUnitRefno;
                                rowView["ProductRefno"] = this.ProdInfo.Refno;
                                rowView["ProductCode"] = this.ProdInfo.Code;
                                rowView["ProductDescription"] = this.ProdInfo.Desc;
                                rowView["UnitCode"] = this.ProdInfo.UnitCode;
                                rowView["ProductUnitBC"] = this.ProdInfo.UnitBC;
                                rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
                                rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
                                rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                                rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                                rowView["ActivityItemAdjustmentQuantity"] = 0;
                                rowView["ActivityItemAdjustmentBaseQuantity"] = 0;
                                rowView["ActivityItemAdjustmentWeightQuantity"] = 0;
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
        public override async Task Refresh(XDocument data)
        {
            if (SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityAdjustmentItem adjItem = new TableActivityAdjustmentItem();
                TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();
                await adjDetail.Purge();
                adjItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                adjItem.RegisterDataAccessListenerProgressOut(this);
                await adjItem.Request(ActHdr.Refno);
                adjItem.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Add all items in dataview to observable collection for display
            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                AdjustmentItem adjustmentItem = new AdjustmentItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemAdjustmentFKRefno = Convert.ToInt32(row["ActivityItemAdjustmentFKRefno"] != DBNull.Value ? row["ActivityItemAdjustmentFKRefno"] : null),
                    ActivityItemAdjustmentStringFKRefno = Convert.ToString(row["ActivityItemAdjustmentStringFKRefno"] != DBNull.Value ? row["ActivityItemAdjustmentStringFKRefno"] : null),
                    ActivityItemAdjustmentQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentQuantity"] : null),
                    ActivityItemAdjustmentBaseQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentBaseQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentBaseQuantity"] : null),
                    ActivityItemAdjustmentWeightQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentWeightQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(adjustmentItem);
            }

            // TODO: Resource strings
            Label labelProductIdHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIdHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);

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
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // Add labels to grid
            gridHeader.Children.Add(labelProductIdHeader);
            gridHeader.Children.Add(labelBCHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWhsHeader);

            // Set label columns
            Grid.SetColumn(labelProductIdHeader, 0);
            Grid.SetColumn(labelBCHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelQtyHeader, 3);
            Grid.SetColumn(labelWhsHeader, 4);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (AdjustmentItem adjustmentItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(adjustmentItem.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", adjustmentItem.ActivityItemAdjustmentQuantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(adjustmentItem.LocationCode));

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
        //    /// <remarks></remarks>
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
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
        //#else
        //      dataGridCol.HeaderText = "Product ID";
        //#endif
        //      dataGridCol.Width = 65 * columnWidthFactor;
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
        //      dataGridCol.MappingName = "ActivityItemAdjustmentQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //      dataGridCol.HeaderText = "Qty";
        //#endif
        //      dataGridCol.Width = 55 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "LocationCode";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
        //#else
        //      dataGridCol.HeaderText = "Whs";
        //#endif
        //      dataGridCol.Width = 30 * columnWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridMain.TableStyles.Add(dataGridStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno AS ActivityItemRefno, ");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno AS ActivityItemAdjustmentFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno AS ActivityItemAdjustmentStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity AS ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity AS ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity AS ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemAdjustment.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno AS ActivityItemAdjustmentFKRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentStringFKRefno AS ActivityItemAdjustmentStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentQuantity AS ActivityItemAdjustmentQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentBaseQuantity AS ActivityItemAdjustmentBaseQuantity,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentWeightQuantity AS ActivityItemAdjustmentWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemAdjustment.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemAdjustment.ActivityItemRefno");
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
        /// <remarks></remarks>
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

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                AdjustmentItem adjustmentItem = new AdjustmentItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemAdjustmentFKRefno = Convert.ToInt32(row["ActivityItemAdjustmentFKRefno"] != DBNull.Value ? row["ActivityItemAdjustmentFKRefno"] : null),
                    ActivityItemAdjustmentStringFKRefno = Convert.ToString(row["ActivityItemAdjustmentStringFKRefno"] != DBNull.Value ? row["ActivityItemAdjustmentStringFKRefno"] : null),
                    ActivityItemAdjustmentQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentQuantity"] : null),
                    ActivityItemAdjustmentBaseQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentBaseQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentBaseQuantity"] : null),
                    ActivityItemAdjustmentWeightQuantity = Convert.ToDecimal(row["ActivityItemAdjustmentWeightQuantity"] != DBNull.Value ? row["ActivityItemAdjustmentWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(adjustmentItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectItem(int idx)
        {
            // Did not call base.SelectItem() because of the default location rules for adjustments.

            this.CurrRowIdx = idx;
            this.Refno = Convert.ToInt32(DataViewItems[idx]["ActivityItemRefno"]);
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(DataViewItems[idx]["ProductUnitRefno"]));
            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemAdjustmentFKRefno"]);

            int actAdjType = Convert.ToInt32(this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityAdjustmentType"]);
            
            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc && !this.SysEnv.SysSettings.IsVerifyProductLocation && (SysCommon.AdjTypes)actAdjType == SysCommon.AdjTypes.Requisition)
            {
                // LocationMapRefno is assumed to be the adjustment WarehouseLocationMapRefno
                this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(this.GetDefaultLocMapRefno( Convert.ToInt32(this.DataViewItems[idx]["LocationMapRefno"]), Convert.ToInt32(this.DataViewItems[idx]["ProductRefno"])));
            }

            // If a skid is selected clear it if the select product does not match the selected skid product.
            if (0 < this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno)
            {
                if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno == this.ProdInfo.Refno)
                {
                    this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                }
                else if (SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid)
                {
                    this.ActHdr.SkidInfo = new SysSkidInfo();
                }
            }
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
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT tActivityItem.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemAdjustment.ActivityItemAdjustmentFKRefno AS ActivityItemAdjustmentFKRefno,");
                sql.Append("tActivityItemAdjustment.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno ");
                sql.Append("FROM tActivityItemAdjustment ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemAdjustment.ActivityItemRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemAdjustment.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);
                sql.AppendFormat(
                    " AND tActivityItemAdjustment.WarehouseLocationMapRefno = {0}",
                    this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat(" AND tActivityItemAdjustment.ProductUnitRefno = {0}", prodUnitRefno);

                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = sql.ToString();
                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemAdjustmentFKRefno"]);
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

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
        /// <returns></returns>
        /// <remarks></remarks>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT ADJUSTMENT ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + barCode + "</gtin>")
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
            this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"] = quantity;
            this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"] = weightQuantity;
            this.DataViewItems[idx].EndEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            DataRowView rowView = this.DataViewItems[idx];

            rowView.BeginEdit();
            rowView["ActivityItemAdjustmentQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"]) + quantity;
            rowView["ActivityItemAdjustmentBaseQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"]) + baseQuantity;
            rowView["ActivityItemAdjustmentWeightQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"]) + weightQuantity;
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
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = "UPDATE tActivityItemAdjustment"
                    + " SET ActivityItemAdjustmentQuantity = " + this.DataViewItems[idx]["ActivityItemAdjustmentQuantity"] + ","
                    + " ActivityItemAdjustmentBaseQuantity = " + this.DataViewItems[idx]["ActivityItemAdjustmentBaseQuantity"] + "," +
                    " ActivityItemAdjustmentWeightQuantity = " + this.DataViewItems[idx]["ActivityItemAdjustmentWeightQuantity"] + " WHERE ActivityItemRefno = " + actItemRefno;

                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
