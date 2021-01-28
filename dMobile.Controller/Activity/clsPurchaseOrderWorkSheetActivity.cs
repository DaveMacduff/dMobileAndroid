namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public class POWorkSheetActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public POWorkSheetActivity()
          : base()
        {
            this.FullClassName = typeof(POWorkSheetActivity).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Work Sheet";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            
            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemPurchaseOrderWorkSheetFKRefno = Convert.ToInt32(row["ActivityItemPurchaseOrderWorkSheetFKRefno"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetFKRefno"] : null),
                    ActivityItemPurchaseOrderWorkSheetStringFKRefno = Convert.ToString(row["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityItemPurchaseOrderWorkSheetQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] : null),
                    ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] : null),
                    ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                };

                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            // TODO: Resource Strings
            Label labelProductID = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelOrdered = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeight = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductID.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrdered.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeight.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5, 5, 5, 5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelProductID);
            gridHeader.Children.Add(labelOrdered);
            gridHeader.Children.Add(labelWeight);

            Grid.SetColumn(labelProductID, 0);
            Grid.SetColumn(labelOrdered, 1);
            Grid.SetColumn(labelWeight, 2);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);
            
            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(purchaseOrderWorkSheetItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered));

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
            }

            // Data grid
            DataTemplate dataTemplateReceiving = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5, 5, 5, 5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) }
                    }
                };

                Label labelActivityPOProductID = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityPOOrdered = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityPOWeight = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityPOProductID.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelActivityPOOrdered.SetBinding(Label.TextProperty, new Binding("ActivityItemPurchaseOrderWorkSheetQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityPOWeight.SetBinding(Label.TextProperty, new Binding("ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelActivityPOProductID);
                gridData.Children.Add(labelActivityPOOrdered);
                gridData.Children.Add(labelActivityPOWeight);

                Grid.SetColumn(labelActivityPOProductID, 0);
                Grid.SetColumn(labelActivityPOOrdered, 1);
                Grid.SetColumn(labelActivityPOWeight, 2);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplateReceiving;

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        ///
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int colWidthFactor = 1;

        //      if (SysEnv.IsWindowsMobile6Device)
        //      {
        //        colWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //      dataGridStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductCode";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
        //#else
        //      dataGridCol.HeaderText = "Product ID";
        //#endif
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPurchaseOrderWorkSheetQuantityOrdered";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ordered);
        //#else
        //      dataGridCol.HeaderText = "Ordered";
        //#endif
        //      dataGridCol.Width = 52 * colWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered";
        //#if MULTILANGUAGE
        //      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
        //#else
        //      dataGridCol.HeaderText = "Weight";
        //#endif
        //      dataGridCol.Width = 52 * colWidthFactor;
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
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append(" tActivityItem.ActivityRefno AS ActivityRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno AS ActivityItemPurchaseOrderWorkSheetFKRefno, ");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno AS ActivityItemPurchaseOrderWorkSheetStringFKRefno, ");
                sql.Append(" tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append(" tProduct.ProductRefno AS ProductRefno,");
                sql.Append(" tProduct.ProductCode AS ProductCode,");
                sql.Append(" tProduct.ProductDescription AS ProductDescription,");
                sql.Append(" tUnit.UnitCode AS UnitCode,");
                sql.Append(" tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append(" tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append(" tActivityItem.ActivityItemStatus AS ActivityItemStatus");
                sql.Append(" FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat(" WHERE tActivityItem.ActivityItemStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append(" tActivityItem.ActivityRefno AS ActivityRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno AS ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno AS ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
                sql.Append(" tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append(" tProduct.ProductRefno AS ProductRefno,");
                sql.Append(" tProduct.ProductCode AS ProductCode,");
                sql.Append(" tProduct.ProductDescription AS ProductDescription,");
                sql.Append(" tUnit.UnitCode AS UnitCode,");
                sql.Append(" tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append(" tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append(" tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append(" tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append(" tActivityItem.ActivityItemStatus AS ActivityItemStatus");
                sql.Append(" FROM tActivityItemPurchaseOrderWorkSheet");
                sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
                sql.Append(" INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno");
                sql.Append(" INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append(" INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno");
                sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno");
                sql.Append(" ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            using (DataSet ds = new DataSet())
            {
                da.Fill(ds);
                this.DataViewHdrs = ds.Tables[0].DefaultView;
                this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                this.DataViewHdrs.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
            }

            return this.DataViewHdrs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]))
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);
            this.Refno = Convert.ToInt32(DataViewHdrs[idx]["ActivityRefno"]);

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                this.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityPurchaseOrderWorkSheetItem tableWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();

            tableWorkSheetItem.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
