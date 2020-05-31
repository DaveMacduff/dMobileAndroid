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
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class PhysicalActItem : ActItem
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public PhysicalActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(PhysicalActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProduct2Physical;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPhysicalCreateNewItemNoAsk;
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
                isCatchAsk = this.SysEnv.SysSettings.IsPhysicalCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsPhysicalCountNoCatchAskZeroEnabled;
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
            return string.Format("Unable to parse count item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find count item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
            DialogResult result = DialogResult.None;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int locMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;

            if (this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
            {
                locMapRefno = this.LocMap.CurrLocMapRefno;
            }

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0)
                {
                    // TODO: Resource strings
                    string msg = "A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?";
                    //                    result = MessageBox.Show(msg,
                    //#if MULTILANGUAGE
                    //                          Translator.GetInstance().Translate(TranslatorVocabulary.NewItem), 
                    //#else
                    //                          "New Item",
                    //#endif
                    //                          MessageBoxButtons.YesNoCancel,
                    //                        MessageBoxIcon.Question,
                    //                        MessageBoxDefaultButton.Button3);

                    // TODO: Resource strings
                    if (await Application.Current.MainPage.DisplayAlert("New Item", msg, "Yes", "No"))
                    {
                        result = DialogResult.Yes;
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("A line item already exists for this location and product.");
                    }

                }
                else
                {
                    //if the selected product to insert was not found, force the dialog result to insert it.
                    result = DialogResult.Yes;
                }

                if (result == DialogResult.Yes)
                {
                    SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                    using (SQLiteCommand cmndCountItem = conn.CreateCommand())
                    using (SQLiteCommand cmndItem = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //using (SqlCeResultSet rs = cmndCountItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();

                            //using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

                            cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                            cmndItem.ExecuteNonQuery();

                            //recItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.PhysicalCounts);
                            //recItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            //recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            //rsItem.Insert(recItem);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
                            this.FKRefno = this.Refno * -1;

                            cmndCountItem.CommandText = "INSERT INTO tActivityItemCount (" +
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
                                "@activityItemRefno, " +
                                "@activityItemSubType, " +
                                "@activityItemCountFKRefno, " +
                                "@activityItemCountStringFKRefno, " +
                                "@productUnitRefno, " +
                                "@locationMapRefno, " +
                                "@activityItemCountQuantityOnHand, " +
                                "@activityItemCountBaseQuantityOnHand, " +
                                "@activityItemCountWeightQuantityOnHand, " +
                                "@activityItemCountQuantity, " +
                                "@activityItemCountBaseQuantity, " +
                                "@activityItemCountWeightQuantity" +
                                ")";
                            cmndCountItem.CommandType = CommandType.Text;
                            //cmndCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.PhysicalCounts)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountFKRefno", Convert.ToInt32(this.FKRefno)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountStringFKRefno", Convert.ToString(this.FKRefno)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@locationMapRefno", Convert.ToInt32(locMapRefno)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantityOnHand", Convert.ToInt32(0)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantityOnHand", Convert.ToInt32(0)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantityOnHand", Convert.ToInt32(0)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantity", Convert.ToInt32(0)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantity", Convert.ToInt32(0)));
                            cmndCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantity", Convert.ToInt32(0)));

                            cmndCountItem.ExecuteNonQuery();

                            transaction.Commit();

                            //rec.SetInt32((int)SysCommon.ActItemCountFields.ActItemRefno, this.Refno);
                            //rec.SetInt32((int)SysCommon.ActItemCountFields.ActItemSubType, (int)SysCommon.ActSubTypes.PhysicalCounts);
                            //rec.SetInt32((int)SysCommon.ActItemCountFields.FKRefno, this.FKRefno);
                            //rec.SetString((int)SysCommon.ActItemCountFields.StringFKRefno, this.FKRefno.ToString());
                            //rec.SetInt32((int)SysCommon.ActItemCountFields.ProdUnitRefno, prodUnitRefno);
                            //rec.SetInt32((int)SysCommon.ActItemCountFields.LocMapRefno, locMapRefno);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.QuantityOnHand, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantityOnHand, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantityOnHand, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.Quantity, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.BaseQuantity, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemCountFields.WeightQuantity, 0);
                            //rs.Insert(rec);

                            this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                            if (DataViewItems != null)
                            {
                                DataRowView rowView = this.DataViewItems.AddNew();
                                rowView["ActivityItemRefno"] = this.Refno;
                                rowView["ActivityItemCountFKRefno"] = this.FKRefno;
                                rowView["ActivityItemCountStringFKRefno"] = this.FKRefno.ToString();
                                rowView["ProductUnitRefno"] = prodUnitRefno;
                                rowView["ProductRefno"] = this.ProdInfo.Refno;
                                rowView["ProductCode"] = this.ProdInfo.Code;
                                rowView["ProductDescription"] = this.ProdInfo.Desc;
                                rowView["UnitCode"] = this.ProdInfo.UnitCode;
                                rowView["ProductUnitBC"] = this.ProdInfo.UnitBC;
                                rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
                                rowView["LocationMapRefno"] = locMapRefno;
                                rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                                rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                                rowView["ActivityItemCountQuantityOnHand"] = 0;
                                rowView["ActivityItemCountBaseQuantityOnHand"] = 0;
                                rowView["ActivityItemCountWeightQuantityOnHand"] = 0;
                                rowView["ActivityItemCountQuantity"] = 0;
                                rowView["ActivityItemCountBaseQuantity"] = 0;
                                rowView["ActivityItemCountWeightQuantity"] = 0;
                                rowView.EndEdit();
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            // TODO: Logging
                            Console.WriteLine(ex);

                            transaction.Rollback();
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

                TableActivityPhysicalCountItem tableItem = new TableActivityPhysicalCountItem();
                TableActivityPhysicalCountDetail tableDetail = new TableActivityPhysicalCountDetail();
                tableDetail.Purge();
                tableItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableItem.RegisterDataAccessListenerProgressOut((PhysicalActItem)this);
                tableItem.Request(this.ActHdr.Refno);
                tableItem.UnregisterDataAccessListenerProgressOut((PhysicalActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            // TODO: Calculate column widths
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            Grid gridHeaderPhysicalCount = new Grid
            {
                Padding = new Thickness(5),
                ColumnSpacing = 15,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) }
                }
            };

            gridHeaderPhysicalCount.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            // TODO: Resource strings
            Label labelPhysicalCountCode = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountBC = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountUnit = new Label { Text = "", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountOnHand = new Label { Text = "On Hand", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountCounted = new Label { Text = "Counted", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPhysicalCountWtOn = new Label { Text = "Wt On", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            // Add labels to grid
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountCode);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountBC);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountUnit);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountOnHand);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountCounted);
            gridHeaderPhysicalCount.Children.Add(labelPhysicalCountWtOn);

            // Set label columns
            Grid.SetColumn(labelPhysicalCountCode, 0);
            Grid.SetColumn(labelPhysicalCountBC, 1);
            Grid.SetColumn(labelPhysicalCountUnit, 2);
            Grid.SetColumn(labelPhysicalCountOnHand, 3);
            Grid.SetColumn(labelPhysicalCountCounted, 4);
            Grid.SetColumn(labelPhysicalCountWtOn, 5);

            Grid.SetRow(gridHeaderPhysicalCount, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderPhysicalCount);

            // Create item template
            DataTemplate dataTemplatePhysicalCount = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnSpacing = 15,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) }
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

            dataGridMainActivity.ItemTemplate = dataTemplatePhysicalCount;
            observableCollection.Clear();

            // Add all items in dataview to observable collection for display
            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                CountItem countItem = new CountItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"]),
                    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"]),
                    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"]),
                    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"]),
                    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"]),
                    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"]),
                    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"]),
                    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"]),
                    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"]),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                    ProductCode = Convert.ToString(row["ProductCode"]),
                    ProductDescription = Convert.ToString(row["ProductDescription"]),
                    UnitCode = Convert.ToString(row["UnitCode"]),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                    LocationCode = Convert.ToString(row["LocationCode"]),
                    LocationName = Convert.ToString(row["LocationName"]),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"])
                };

                observableCollection.Add(countItem);
            }

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
        //      dataGridCol.MappingName = "ActivityItemCountQuantityOnHand";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.OnHandz);
        //#else
        //      dataGridCol.HeaderText = "On Hand";
        //#endif
        //      dataGridCol.Width = 55 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemCountQuantity";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Counted);
        //#else
        //      dataGridCol.HeaderText = "Counted";
        //#endif
        //      dataGridCol.Format = "0.000";
        //      dataGridCol.Width = 45 * columnWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemCountWeightQuantityOnHand";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.WtOnHand);
        //#else
        //      dataGridCol.HeaderText = "Wt On Hand";
        //#endif
        //      dataGridCol.Width = 55 * columnWidthFactor;
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> (0) ", (int)SysCommon.ActStatus.NoChange);
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
                CountItem countItem = new CountItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"]),
                    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"]),
                    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"]),
                    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"]),
                    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"]),
                    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"]),
                    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"]),
                    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"]),
                    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"]),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"]),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"]),
                    ProductCode = Convert.ToString(row["ProductCode"]),
                    ProductDescription = Convert.ToString(row["ProductDescription"]),
                    UnitCode = Convert.ToString(row["UnitCode"]),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"]),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"]),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"]),
                    LocationCode = Convert.ToString(row["LocationCode"]),
                    LocationName = Convert.ToString(row["LocationName"]),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"])
                };

                observableCollection.Add(countItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectItem(int idx)
        {
            if (this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
            {
                this.CurrRowIdx = idx;
                this.Refno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemRefno"]);
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(this.DataViewItems[idx]["ProductUnitRefno"]));
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                // Do not change the location to the skid location leave it alone for IsPhysicalDoNotDefaultProductLocation.
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
                base.SelectItem(idx);
            }

            this.FKRefno = Convert.ToInt32(DataViewItems[idx]["ActivityItemCountFKRefno"]);
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

                sql.Append("SELECT tActivityItem.ActivityItemRefno,");
                sql.Append("tActivityItemCount.ActivityItemCountFKRefno,");
                sql.Append("tActivityItemCount.ProductUnitRefno,");
                sql.Append("tLocationMapSlot.LocationMapRefno ");
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

                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = sql.ToString();

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemCountFKRefno"]);
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
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PHYSICAL COUNT ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
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
            this.DataViewItems[idx]["ActivityItemCountQuantity"] = quantity;
            this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemCountWeightQuantity"] = weightQuantity;
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
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemCountQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemCountQuantity"]) + quantity;
            this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] = Convert.ToDecimal(DataViewItems[idx]["ActivityItemCountBaseQuantity"]) + baseQuantity;
            this.DataViewItems[idx]["ActivityItemCountWeightQuantity"] = Convert.ToDecimal(DataViewItems[idx]["ActivityItemCountWeightQuantity"]) + weightQuantity;
            this.DataViewItems[idx].EndEdit();

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
                cmndItem.CommandText = "tActivityItemCount";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemCount "
                    + "SET ActivityItemCountQuantity = " + this.DataViewItems[idx]["ActivityItemCountQuantity"] + ","
                    + "ActivityItemCountBaseQuantity = " + this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] + ","
                    + "ActivityItemCountWeightQuantity = " + this.DataViewItems[idx]["ActivityItemCountWeightQuantity"]
                    + " WHERE tActivityItemCount.ActivityItemRefno = " + actItemRefno;

                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
