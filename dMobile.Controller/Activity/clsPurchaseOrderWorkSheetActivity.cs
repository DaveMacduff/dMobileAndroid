namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class POWorkSheetActivity : Activity
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public POWorkSheetActivity()
          : base()
        {
            this.FullClassName = typeof(POWorkSheetActivity).FullName;

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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
            
            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in this.ListHdrs)
            {
                //PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemPurchaseOrderWorkSheetFKRefno = Convert.ToInt32(row["ActivityItemPurchaseOrderWorkSheetFKRefno"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetFKRefno"] : null),
                //    ActivityItemPurchaseOrderWorkSheetStringFKRefno = Convert.ToString(row["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemPurchaseOrderWorkSheetQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetQuantityOrdered"] : null),
                //    ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered"] : null),
                //    ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //};

                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            // TODO: Resource Strings
            Label labelProductID = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelOrdered = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeight = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductID.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrdered.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeight.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
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

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
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

            this.ListHdrs = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<object>(sql.ToString());
            return this.ListHdrs;
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
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                this.SelectedActivity = purchaseOrderWorkSheetItem;
                this.FKRefno = Convert.ToInt32(purchaseOrderWorkSheetItem.ActivityRefno);
                this.Refno = Convert.ToInt32(purchaseOrderWorkSheetItem.ActivityRefno);

                // Set the selected warehouse to the activity header warehouse.
                if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(purchaseOrderWorkSheetItem.LocationMapRefno))
                {
                    LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(purchaseOrderWorkSheetItem.LocationMapRefno));
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            dynamic poWorkSheet = selectedItem as dynamic;

            if (poWorkSheet != null)
            {
                this.FKRefno = Convert.ToInt32(poWorkSheet.ActivityRefno);
                this.Refno = Convert.ToInt32(poWorkSheet.ActivityRefno);
            }

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                await this.Purge();
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Purge()
        {
            TableActivityPurchaseOrderWorkSheetItem tableWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();

            await tableWorkSheetItem.Purge();
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
