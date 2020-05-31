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
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class CycleActItem : ActItem
    {
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
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0)
                {
                    //result = MessageBox.Show("A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "New Item", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                    // TODO: Resource strings
                    result = await Application.Current.MainPage.DisplayAlert("New Item", "A line item already exists for this location and product.", "Yes", "No");
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    result = true;
                }

                if (result)
                {
                    SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                    using (SQLiteCommand cmndActCountItem = conn.CreateCommand())
                    using (SQLiteCommand cmndItem = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //using (SqlCeResultSet rs = cmndActCountItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //{
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", (int)SysCommon.ActSubTypes.CycleCounts));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", (int)SysCommon.ActStatus.NoChange));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                            cmndItem.ExecuteNonQuery();

                            //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.CycleCounts);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            //rsActItem.Insert(recActItem);

                            cmndID.CommandText = "SELECT last_insert_rowid()";
                            cmndID.CommandType = CommandType.Text;

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
                            this.FKRefno = this.Refno * -1;

                            cmndActCountItem.CommandText = "INSERT INTO tActivityItemCount (" +
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
                            cmndActCountItem.CommandType = CommandType.Text;
                            //cmndActCountItem.IndexName = "UQ_ActivityItemCountOnActivityItemCountFKRefno";
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountFKRefno", Convert.ToInt32(this.FKRefno)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountStringFKRefno", Convert.ToString(this.FKRefno)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@locationMapRefno", Convert.ToInt32(locMapRefno)));

                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantityOnHand", Convert.ToDecimal(0)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantityOnHand", Convert.ToDecimal(0)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantityOnHand", Convert.ToDecimal(0)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountQuantity", Convert.ToDecimal(0)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountBaseQuantity", Convert.ToDecimal(0)));
                            cmndActCountItem.Parameters.Add(new SQLiteParameter("@activityItemCountWeightQuantity", Convert.ToDecimal(0)));

                            cmndActCountItem.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            
                            // TOOD: Logging
                            Console.WriteLine(ex);
                        }

                        //rec.SetInt32((int)SysCommon.ActItemCountFields.ActItemRefno, this.Refno);
                        //rec.SetInt32((int)SysCommon.ActItemCountFields.ActItemSubType, (int)SysCommon.ActSubTypes.CycleCounts);
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

                        if (this.DataViewItems != null)
                        {
                            DataRowView rowView = this.DataViewItems.AddNew();
                            rowView["ActivityItemRefno"] = this.Refno;
                            rowView["ActivityItemCountFKRefno"] = this.FKRefno;
                            rowView["ActivityItemCountStringFKRefno"] = this.FKRefno.ToString();
                            rowView["ProductUnitRefno"] = prodUnitRefno;
                            rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
                            rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
                            rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
                            rowView["UnitCode"] = data.Root.Element("UnitCode").Value;
                            rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
                            rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
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

                TableActivityCycleCountItem tableActCountItem = new TableActivityCycleCountItem();
                TableActivityCycleCountDetail tableActCountDetail = new TableActivityCycleCountDetail();
                tableActCountDetail.Purge();
                tableActCountItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActCountItem.RegisterDataAccessListenerProgressOut((CycleActItem)this);
                tableActCountItem.Request(this.ActHdr.Refno);
                tableActCountItem.UnregisterDataAccessListenerProgressOut((CycleActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            // TODO: Calculate column widths
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Add all items in dataview to observable collection for display
            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                CountItem countItem = new CountItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"] != DBNull.Value ? row["ActivityItemCountFKRefno"] : null),
                    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"] != DBNull.Value ? row["ActivityItemCountStringFKRefno"] : null),
                    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountQuantityOnHand"] : null),
                    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountBaseQuantityOnHand"] : null),
                    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountWeightQuantityOnHand"] : null),
                    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"] != DBNull.Value ? row["ActivityItemCountQuantity"] : null),
                    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"] != DBNull.Value ? row["ActivityItemCountBaseQuantity"] : null),
                    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"] != DBNull.Value ? row["ActivityItemCountWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(countItem);
            }

            // TODO: Resource strings
            Label labelCountCode = new Label { Text = "Code", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountBC = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountUnit = new Label { Text = "", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountOnHand = new Label { Text = "On Hand", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountCounted = new Label { Text = "Counted", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelCountWtOn = new Label { Text = "Wt On", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountCode.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountBC.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountUnit.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountOnHand.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountCounted.Text);
            double columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelCountWtOn.Text);

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

            // Create item template
            DataTemplate dataTemplateCount = new DataTemplate(() =>
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

            dataGridMainActivity.ItemTemplate = dataTemplateCount;

            return observableCollection;
        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <param name="dataGridMain"></param>
//        /// <remarks></remarks>
//        public override void DesignDataGrid(DataGrid dataGridMain)
//        {
//            int colWidthFactor = 1;

//            if (this.SysEnv.IsWindowsMobile6Device)
//            {
//                colWidthFactor = 2;
//            }

//            if (dataGridMain.TableStyles.Count > 0)
//            {
//                dataGridMain.TableStyles.Clear();
//            }

//            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
//            dataGridStyle.MappingName = this.MainGridMappingName;

//            DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityItemRefno";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = -1;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ProductCode";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Codezz);
//#else
//            dataGridCol.HeaderText = "Code";
//#endif
//            dataGridCol.Width = 62 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ProductUnitBC";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
//#else
//            dataGridCol.HeaderText = "BC";
//#endif
//            dataGridCol.Width = 20 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "UnitCode";
//            dataGridCol.HeaderText = string.Empty;
//            dataGridCol.Width = 22 * colWidthFactor;
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityItemCountQuantityOnHand";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.OnHandz);
//#else
//            dataGridCol.HeaderText = "On Hand";
//#endif
//            dataGridCol.Width = 55 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridCol = new DataGridTextBoxColumn();
//            dataGridCol.MappingName = "ActivityItemCountQuantity";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Counted);
//#else
//            dataGridCol.HeaderText = "Counted";
//#endif
//            dataGridCol.Width = 45 * colWidthFactor;
//            dataGridCol.Format = "0.000";
//            dataGridStyle.GridColumnStyles.Add(dataGridCol);

//            dataGridMain.TableStyles.Add(dataGridStyle);
//            dataGridMain.RowHeadersVisible = false;
//            dataGridMain.ColumnHeadersVisible = true;
//        }

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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                sql.AppendFormat("AND tActivityItem.ActivityItemSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.CycleCounts);
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
                CountItem countItem = new CountItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemCountFKRefno = Convert.ToInt32(row["ActivityItemCountFKRefno"] != DBNull.Value ? row["ActivityItemCountFKRefno"] : null),
                    ActivityItemCountStringFKRefno = Convert.ToString(row["ActivityItemCountStringFKRefno"] != DBNull.Value ? row["ActivityItemCountStringFKRefno"] : null),
                    ActivityItemCountQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountQuantityOnHand"] : null),
                    ActivityItemCountBaseQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountBaseQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountBaseQuantityOnHand"] : null),
                    ActivityItemCountWeightQuantityOnHand = Convert.ToDecimal(row["ActivityItemCountWeightQuantityOnHand"] != DBNull.Value ? row["ActivityItemCountWeightQuantityOnHand"] : null),
                    ActivityItemCountQuantity = Convert.ToDecimal(row["ActivityItemCountQuantity"] != DBNull.Value ? row["ActivityItemCountQuantity"] : null),
                    ActivityItemCountBaseQuantity = Convert.ToDecimal(row["ActivityItemCountBaseQuantity"] != DBNull.Value ? row["ActivityItemCountBaseQuantity"] : null),
                    ActivityItemCountWeightQuantity = Convert.ToDecimal(row["ActivityItemCountWeightQuantity"] != DBNull.Value ? row["ActivityItemCountWeightQuantity"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(countItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectItem(int idx)
        {
            if (this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation)
            {
                this.CurrRowIdx = idx;
                this.Refno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemRefno"]);
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(this.DataViewItems[idx]["ProductUnitRefno"]));
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

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
                base.SelectItem(idx);
            }

            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemCountFKRefno"]);
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
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CYCLE COUNT ITEM</action>"
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
            this.DataViewItems[idx]["ActivityItemCountBaseQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemCountBaseQuantity"]) + baseQuantity;
            this.DataViewItems[idx]["ActivityItemCountWeightQuantity"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemCountWeightQuantity"]) + weightQuantity;
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
