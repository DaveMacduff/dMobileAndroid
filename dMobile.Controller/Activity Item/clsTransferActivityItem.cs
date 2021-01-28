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

    public class TransferActItem : ActItem
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public TransferActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(TransferActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Trans;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsTransCreateNewItemNoAsk;
            this.ActSubType = activity.ActSubType;
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
                isCatchAsk = this.SysEnv.SysSettings.IsTransCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsTransferNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.TransQtyOverWarn != SysCommon.ActTransfersQtyOverValLevels.NoValidationOrWarning && 0 < this.FKRefno)
            {
                try
                {
                    this.TransferModuleQuantityOverCheck(int.Parse(data.Root.Element("itm_refno").Value), decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.TransQtyOverWarn)
                    {
                        case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndContinue:
                            //if (MessageBox.Show(ex.Message + Environment.NewLine + "Continue?", "Quantity Over", MessageBoxButtons.YesNo, MessageBoxIcon.Question, sMessageBoxDefaultButton.Button2) == DialogResult.No)
                            //{
                            // TODO: Resource strings
                            if (!await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);

                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }

                        case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse the transfer item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find the transfer item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemTransferStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            using (SQLiteCommand cmndActTransferItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                //using (SqlCeResultSet rs = cmndActTransferItem.ExecuteResultSet(ResultSetOptions.Updatable))
                //{
                //SqlCeUpdatableRecord rec = rs.CreateRecord();

                //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                //{
                //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                cmndItem.CommandType = CommandType.Text;

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                }
                else
                {
                    cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));
                }

                cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                cmndItem.ExecuteNonQuery();

                //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.TransferFrom);
                //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                //rsActItem.Insert(recActItem);

                cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                cmndID.CommandType = CommandType.Text;

                this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
                this.FKRefno = this.Refno * -1;

                cmndActTransferItem.CommandText = "INSERT INTO tActivityItemTransfer (" +
                    "ActivityItemRefno, " +
                    "ActivityItemSubType, " +
                    "ActivityItemTransferFKRefno, " +
                    "ActivityItemTransferStringFKRefno, " +
                    "ProductUnitRefno, " +
                    "TransferFromWarehouseLocationMapRefno, " +
                    "ActivityItemTransferRequiredQuantity, " +
                    "ActivityItemTransferQuantity, " +
                    "ActivityItemTransferBaseQuantity, " +
                    "ActivityItemTransferWeightQuantity" +
                    ") VALUES (" +
                    "@activityItemRefno, " +
                    "@activityItemSubType, " +
                    "@activityItemTransferFKRefno, " +
                    "@activityItemTransferStringFKRefno, " +
                    "@productUnitRefno, " +
                    "@transferFromWarehouseLocationMapRefno, " +
                    "@activityItemTransferRequiredQuantity, " +
                    "@activityItemTransferQuantity, " +
                    "@activityItemTransferBaseQuantity, " +
                    "@activityItemTransferWeightQuantity" +
                    ")";
                cmndActTransferItem.CommandType = CommandType.Text;
                //cmndActTransferItem.IndexName = "UQ_ActivityItemTransferOnActivityItemTransferFKRefno";
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom)));
                }
                else
                {
                    cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.TransferTo)));
                }

                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferFKRefno", Convert.ToInt32(this.FKRefno)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferStringFKRefno", Convert.ToString(this.FKRefno)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@transferFromWarehouseLocationMapRefno", Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferRequiredQuantity", Convert.ToInt32(0)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferQuantity", Convert.ToInt32(0)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferBaseQuantity", Convert.ToInt32(0)));
                cmndActTransferItem.Parameters.Add(new SQLiteParameter("@activityItemTransferWeightQuantity", Convert.ToInt32(0)));

                cmndActTransferItem.ExecuteNonQuery();

                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemRefno, this.Refno);
                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemSubType, (int)SysCommon.ActSubTypes.TransferFrom);
                //rec.SetInt32((int)SysCommon.ActItemTransferFields.FKRefno, this.FKRefno);
                //rec.SetString((int)SysCommon.ActItemTransferFields.StringFKRefno, this.FKRefno.ToString());
                //rec.SetInt32((int)SysCommon.ActItemTransferFields.ProductUnitRefno, prodUnitRefno);
                //rec.SetInt32((int)SysCommon.ActItemTransferFields.TransferFromWhsLocMapRefno, this.LocMap.CurrLocMap.WhsLocMapRefno);
                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.RequiredQuantity, 0);
                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.Quantity, 0);
                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.BaseQuantity, 0);
                //rec.SetDecimal((int)SysCommon.ActItemTransferFields.WeightQuantity, 0);
                //rs.Insert(rec);

                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                if (this.DataViewItems != null)
                {
                    DataRowView rowView = this.DataViewItems.AddNew();
                    rowView["ActivityItemRefno"] = this.Refno;
                    rowView["ActivityItemTransferFKRefno"] = this.FKRefno;
                    rowView["ActivityItemTransferStringFKRefno"] = this.FKRefno.ToString();
                    rowView["ProductUnitRefno"] = prodUnitRefno;
                    rowView["ProductRefno"] = int.Parse(data.Root.Element("ProductRefno").Value);
                    rowView["ProductCode"] = data.Root.Element("ProductCode").Value;
                    rowView["ProductDescription"] = data.Root.Element("ProductDescription").Value;
                    rowView["UnitCode"] = data.Root.Element("UnitCode").Value;
                    rowView["ProductUnitBC"] = data.Root.Element("ProductUnitBC").Value;
                    rowView["ProductUnitSizeDescription"] = data.Root.Element("ProductUnitSizeDescription").Value;
                    rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
                    rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                    rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                    rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                    rowView["ActivityItemTransferRequiredQuantity"] = 0;
                    rowView["ActivityItemTransferQuantity"] = 0;
                    rowView["ActivityItemTransferBaseQuantity"] = 0;
                    rowView["ActivityItemTransferWeightQuantity"] = 0;
                    rowView.EndEdit();
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

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    TableActivityTransferFromItem tableActTransferItem = new TableActivityTransferFromItem();
                    TableActivityTransferFromDetail tableActTransferDetail = new TableActivityTransferFromDetail();
                    tableActTransferDetail.Purge();
                    tableActTransferItem.Purge();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    tableActTransferItem.Request(this.ActHdr.Refno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
                }
                else
                {
                    TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                    TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                    tableActTransferDetail.Purge();
                    tableActTransferItem.Purge();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    tableActTransferItem.Request(this.ActHdr.Refno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
                }
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource strings
            Label labelProductIDHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelRequiredHeader = new Label { Text = "Required", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIDHeader.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRequiredHeader.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            double columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;
            double detailWidth5 = 0;

            // Create header data template
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

            gridHeader.Children.Add(labelProductIDHeader);
            gridHeader.Children.Add(labelBCHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelRequiredHeader);
            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWhsHeader);

            Grid.SetColumn(labelProductIDHeader, 0);
            Grid.SetColumn(labelBCHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelRequiredHeader, 3);
            Grid.SetColumn(labelQtyHeader, 4);
            Grid.SetColumn(labelWhsHeader, 5);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                TransferItem transferItem = new TransferItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                    ActivityItemTransferFKRefno = Convert.ToInt32(row["ActivityItemTransferFKRefno"] != DBNull.Value ? row["ActivityItemTransferFKRefno"] : null),
                    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                    ActivityItemTransferStringFKRefno = Convert.ToString(row["ActivityItemTransferStringFKRefno"] != DBNull.Value ? row["ActivityItemTransferStringFKRefno"] : null),
                    ActivityItemTransferWeightQuantity = Convert.ToDecimal(row["ActivityItemTransferWeightQuantity"] != DBNull.Value ? row["ActivityItemTransferWeightQuantity"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                };

                observableCollection.Add(transferItem);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.ProductCode);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.ProductUnitBC);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.UnitCode);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", transferItem.ActivityItemTransferRequiredQuantity));
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", transferItem.ActivityItemTransferQuantity));
                detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(transferItem.LocationCode));

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

            // Data grid
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
                            new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) }
                    }
                };

                Label labelProductID = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelRequired = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWhs = new Label { VerticalOptions = LayoutOptions.Center };

                labelProductID.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelRequired.SetBinding(Label.TextProperty, new Binding("ActivityItemTransferRequiredQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityItemTransferQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWhs.SetBinding(Label.TextProperty, new Binding("LocationCode"));

                gridData.Children.Add(labelProductID);
                gridData.Children.Add(labelBC);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelRequired);
                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWhs);

                Grid.SetColumn(labelProductID, 0);
                Grid.SetColumn(labelBC, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelRequired, 3);
                Grid.SetColumn(labelQty, 4);
                Grid.SetColumn(labelWhs, 5);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplate;

            return observableCollection;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dataGridMain"></param>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //    int colWidthFactor = 1;

        //    if (this.SysEnv.IsWindowsMobile6Device)
        //    {
        //        colWidthFactor = 2;
        //    }

        //    if (dataGridMain.TableStyles.Count > 0)
        //    {
        //        dataGridMain.TableStyles.Clear();
        //    }

        //    DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //    dataGridStyle.MappingName = this.MainGridMappingName;

        //    DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityItemRefno";
        //    dataGridCol.HeaderText = string.Empty;
        //    dataGridCol.Width = -1;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ProductCode";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
        //    #else
        //        dataGridCol.HeaderText = "Product ID";
        //    #endif

        //    dataGridCol.Width = 65 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ProductUnitBC";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Bc);
        //    #else
        //        dataGridCol.HeaderText = "BC";
        //    #endif

        //    dataGridCol.Width = 20 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "UnitCode";
        //    dataGridCol.HeaderText = string.Empty;
        //    dataGridCol.Width = 22 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityItemTransferRequiredQuantity";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Required);
        //    #else
        //        dataGridCol.HeaderText = "Required";
        //    #endif

        //    dataGridCol.Width = 55 * colWidthFactor;
        //    dataGridCol.Format = "0.000";
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "ActivityItemTransferQuantity";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //    #else
        //        dataGridCol.HeaderText = "Qty";
        //    #endif

        //    dataGridCol.Width = 55 * colWidthFactor;
        //    dataGridCol.Format = "0.000";
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridCol = new DataGridTextBoxColumn();
        //    dataGridCol.MappingName = "LocationCode";
        //    #if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Whs);
        //    #else
        //        dataGridCol.HeaderText = "Whs";
        //    #endif

        //    dataGridCol.Width = 58 * colWidthFactor;
        //    dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //    dataGridMain.TableStyles.Add(dataGridStyle);
        //    dataGridMain.RowHeadersVisible = false;
        //    dataGridMain.ColumnHeadersVisible = true;
        //}

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
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno AS ActivityItemTransferFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno AS ActivityItemTransferStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity AS ActivityItemTransferRequiredQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity AS ActivityItemTransferQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity AS ActivityItemTransferBaseQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity AS ActivityItemTransferWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity,");
                sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat(" AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" ORDER BY tActivityItemTransfer.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno AS ActivityItemRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferFKRefno AS ActivityItemTransferFKRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferStringFKRefno AS ActivityItemTransferStringFKRefno, ");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno, ");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode, ");
                sql.Append("tProduct.ProductDescription AS ProductDescription, ");
                sql.Append("tUnit.UnitCode AS UnitCode, ");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferRequiredQuantity AS ActivityItemTransferRequiredQuantity, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferQuantity AS ActivityItemTransferQuantity, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferBaseQuantity AS ActivityItemTransferBaseQuantity, ");
                sql.Append("tActivityItemTransfer.ActivityItemTransferWeightQuantity AS ActivityItemTransferWeightQuantity, ");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityitem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemTransfer ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemTransfer.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemTransfer.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemTransfer.ActivityItemRefno");
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

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                TransferItem transferItem = new TransferItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                    ActivityItemTransferFKRefno = Convert.ToInt32(row["ActivityItemTransferFKRefno"] != DBNull.Value ? row["ActivityItemTransferFKRefno"] : null),
                    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                    ActivityItemTransferStringFKRefno = Convert.ToString(row["ActivityItemTransferStringFKRefno"] != DBNull.Value ? row["ActivityItemTransferStringFKRefno"] : null),
                    ActivityItemTransferWeightQuantity = Convert.ToDecimal(row["ActivityItemTransferWeightQuantity"] != DBNull.Value ? row["ActivityItemTransferWeightQuantity"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                };

                observableCollection.Add(transferItem);
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
            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemTransferFKRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundItemRefno = 0;

            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = "SELECT tActivityItem.ActivityItemRefno,"
                    + "tActivityItemTransfer.ActivityItemTransferFKRefno,"
                    + "tActivityItemTransfer.ProductUnitRefno,"
                    + "tLocationMapWarehouse.LocationMapRefno "
                    + "FROM tActivityItemTransfer "
                    + "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemTransfer.ActivityItemRefno "
                    + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                    + "= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno "
                    + "WHERE tActivityItem.ActivityRefno = " + this.ActHdr.Refno
                    + " AND tActivityItemTransfer.TransferFromWarehouseLocationMapRefno = "
                    + this.LocMap.CurrLocMap.WhsLocMapRefno
                    + " AND tActivityItemTransfer.ProductUnitRefno = " + prodUnitRefno;

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        //// this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(drItem["LocationMapRefno"]));
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemTransferFKRefno"]);
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                        foundItemRefno = this.Refno;
                    }

                    drItem.Close();
                }
            }

            return foundItemRefno;
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

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT TRANSFER ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + data.Root.Element("ProductCode").Value.PadRight(10) + "</prod_id>")
                + ("<alt_unit>" + data.Root.Element("UnitCode").Value + "</alt_unit>")
                + ("<is_bc>" + bool.Parse(data.Root.Element("ProductUnitIsBC").Value) + "</is_bc>")
                + ("<symbology>" + data.Root.Element("ProductUnitBarCodeSymbologyPrefix") + "</symbology>")
                + ("<gtin>" + data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20) + "</gtin>")
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
        /// <param name="selectedIndex"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(int selectedIndex, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[selectedIndex].BeginEdit();
            this.DataViewItems[selectedIndex]["ActivityItemTransferQuantity"] = quantity;
            this.DataViewItems[selectedIndex]["ActivityItemTransferBaseQuantity"] = baseQuantity;
            this.DataViewItems[selectedIndex]["ActivityItemTransferWeightQuantity"] = weightQuantity;
            this.DataViewItems[selectedIndex].EndEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int rowIdx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[rowIdx].BeginEdit();
            this.DataViewItems[rowIdx]["ActivityItemTransferQuantity"] = Convert.ToDecimal(DataViewItems[rowIdx]["ActivityItemTransferQuantity"]) + quantity;
            this.DataViewItems[rowIdx]["ActivityItemTransferBaseQuantity"] = Convert.ToDecimal(DataViewItems[rowIdx]["ActivityItemTransferBaseQuantity"]) + baseQuantity;
            this.DataViewItems[rowIdx]["ActivityItemTransferWeightQuantity"] = Convert.ToDecimal(DataViewItems[rowIdx]["ActivityItemTransferWeightQuantity"]) + weightQuantity;
            this.DataViewItems[rowIdx].EndEdit();

            return rowIdx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "tActivityItemTransfer";
                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = "UPDATE tActivityItemTransfer "
                    + "SET ActivityItemTransferQuantity = " + this.DataViewItems[idx]["ActivityItemTransferQuantity"] + ","
                    + "ActivityItemTransferBaseQuantity = " + this.DataViewItems[idx]["ActivityItemTransferBaseQuantity"] + ","
                    + "ActivityItemTransferWeightQuantity = " + this.DataViewItems[idx]["ActivityItemTransferWeightQuantity"] +
                    " WHERE ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemTransFKRefno"></param>
        /// <param name="actItemTransQuantity"></param>
        private void TransferModuleQuantityOverCheck(int actItemTransFKRefno, decimal actItemTransQuantity)
        {
            if (this.CurrRowIdx >= 0 && this.CurrRowIdx < this.DataViewItems.Count)
            {
                decimal transferred = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemTransferQuantity"]);
                decimal required = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemTransferRequiredQuantity"]);

                if (transferred + actItemTransQuantity > required)
                {
                    throw new Exception(string.Format("The resulting quantity transferred {0} {1} will be greater than the quantity required {2} {3}.", transferred + actItemTransQuantity, this.ProdInfo.UnitCode, required, this.ProdInfo.UnitCode));
                }
            }
        }

        #endregion
    }
}