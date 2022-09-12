// <copyright file="clsReturnsActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
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
    using dMobile.GroundWork;
    using dMobile.Resources.Models;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ReturnsActItem" />.
    /// </summary>
    public class ReturnsActItem : ActItem
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listReturnItems.
        /// </summary>
        private List<ReturnItem> listReturnItems = new List<ReturnItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ReturnsActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(ReturnsActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Ret;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsRetCreateNewItemNoAsk;

            // Data grid
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
                    },
                };

                var labelIsActivityItemReturnsComment = new Label { VerticalOptions = LayoutOptions.Center };
                var labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelActivityItemReturnsQuantityReturned = new Label { VerticalOptions = LayoutOptions.Center };

                labelIsActivityItemReturnsComment.SetBinding(Label.TextProperty, new Binding("IsActivityItemReturnsComment"));
                labelProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemReturnsQuantityReturned.SetBinding(Label.TextProperty, new Binding("ActivityItemReturnsQuantityReturned", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelIsActivityItemReturnsComment);
                gridData.Children.Add(labelProductCode);
                gridData.Children.Add(labelProductUnitBC);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelActivityItemReturnsQuantityReturned);

                Grid.SetColumn(labelIsActivityItemReturnsComment, 0);
                Grid.SetColumn(labelProductCode, 1);
                Grid.SetColumn(labelProductUnitBC, 2);
                Grid.SetColumn(labelUnitCode, 3);
                Grid.SetColumn(labelActivityItemReturnsQuantityReturned, 4);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplateReturnsItem")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplateReturnsItem", dataTemplate);
            }
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItem(int actItemFoundRefno)
        {
            this.SelectedListItemIdx = -1;
            if (actItemFoundRefno >= 0)
            {
                var idx = 0;
                foreach (ReturnItem item in this.ListItems)
                {
                    if (item.ActivityItemRefno == this.ItemRefno)
                    {
                        this.SelectedActivityItem = item;
                        this.SelectedListItemIdx = idx;
                        break;
                    }

                    idx++;
                }
            }
            else
            {
                this.SelectedActivityItem = null;
            }
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
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;
            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsRetCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            var result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsReturnsNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            var comment = string.Empty;
            if (selectedActivityItem is ReturnItem returnItem)
            {
                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReturnsCommentModel>("SELECT * FROM tActivityItemReturnsComment WHERE ActivityItemRefno = ?", returnItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    comment = results[0].ActivityItemReturnsCommentComment;
                }
            }

            return comment;
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
            if (this.SelectedActivityItem is ReturnItem item)
            {
                if (Math.Sign(item.ActivityItemReturnsQuantityReturned) == 0 ||
                    Math.Sign(item.ActivityItemReturnsQuantityReturned) == -1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
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
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to parse returns item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to find returns item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemStringFKRefno";
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Stop and ask to insert an item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            throw new NotImplementedException("Add new return items is not supported.");
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="xmlParam">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument xmlParam)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                var tableActReturnsItem = new TableActivityReturnsItem();
                var tableActReturnsDetail = new TableActivityReturnsDetail();
                await tableActReturnsDetail.PurgeData();
                await tableActReturnsItem.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActReturnsItem.RegisterDataAccessListenerProgressOut((ReturnsActItem)this);
                await tableActReturnsItem.RequestData(this.ActHdr.HdrRefno);
                tableActReturnsItem.UnregisterDataAccessListenerProgressOut((ReturnsActItem)this);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelIsActivityItemReturnsCommentHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelProductUnitBCHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelActivityItemReturnsQuantityReturnedHeader = new Label { Text = "Returned", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemReturnsCommentHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBCHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReturnsQuantityReturnedHeader.Text);
            var gridHeaderReturns = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                },
            };

            gridHeaderReturns.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderReturns.Children.Add(labelIsActivityItemReturnsCommentHeader);
            gridHeaderReturns.Children.Add(labelProductCodeHeader);
            gridHeaderReturns.Children.Add(labelProductUnitBCHeader);
            gridHeaderReturns.Children.Add(labelUnitCodeHeader);
            gridHeaderReturns.Children.Add(labelActivityItemReturnsQuantityReturnedHeader);

            Grid.SetColumn(labelIsActivityItemReturnsCommentHeader, 0);
            Grid.SetColumn(labelProductCodeHeader, 1);
            Grid.SetColumn(labelProductUnitBCHeader, 2);
            Grid.SetColumn(labelUnitCodeHeader, 3);
            Grid.SetColumn(labelActivityItemReturnsQuantityReturnedHeader, 4);

            Grid.SetRow(gridHeaderReturns, 0);

            GridMainActivity.Children.Add(gridHeaderReturns);

            foreach (ReturnItem returnItem in this.ListItems)
            {
                // ReturnItem returnItem = new ReturnItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemReturnsQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsQuantityReturned"] : null),
                //    ActivityItemReturnsBaseQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsBaseQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsBaseQuantityReturned"] : null),
                //    ActivityItemReturnsWeightQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsWeightQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsWeightQuantityReturned"] : null),
                //    ActivityItemReturnsCustomerProductUnitCode = Convert.ToString(row["ActivityItemReturnsCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemReturnsCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    IsActivityItemReturnsComment = Convert.ToString(row["IsActivityItemReturnsComment"] != DBNull.Value ? row["IsActivityItemReturnsComment"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                // };
                observableCollection.Add(returnItem);

                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.IsActivityItemReturnsComment);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.ProductCode);
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.ProductUnitBC);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", returnItem.ActivityItemReturnsQuantityReturned));
                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeaderReturns.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeaderReturns.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeaderReturns.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeaderReturns.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeaderReturns.ColumnDefinitions[4].Width = this.columnWidth4;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplateReturnsItem"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var parameters = new List<object>();

            string query;
            if (isSubmitBatch)
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno AS ProductUnitRefno, " +
                "tProduct.ProductRefno AS ProductRefno, " +
                "tProduct.ProductCode AS ProductCode, " +
                "tProduct.ProductDescription AS ProductDescription, " +
                "tProduct.ProductBrandDescription AS ProductBrandDescription, " +
                "tUnit.UnitCode AS UnitCode, " +
                "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned AS ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned AS ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned AS ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode AS ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus AS ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "AND tActivityItem.ActivityItemStatus <> ? " +
                "UNION " +
                "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno, " +
                "tProduct.ProductRefno, " +
                "tProduct.ProductCode, " +
                "tProduct.ProductDescription, " +
                "tProduct.ProductBrandDescription, " +
                "tUnit.UnitCode, " +
                "tProductUnit.ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "AND tActivityDetail.ActivityDetailStatus <> ? " +
                "ORDER BY tActivityItemReturns.ActivityItemRefno";

                parameters.Add(this.ActHdr.HdrRefno);
                parameters.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemFKRefno, " +
                "tActivityItemReturns.ActivityItemStringFKRefno, " +
                "tProductUnit.ProductUnitRefno AS ProductUnitRefno, " +
                "tProduct.ProductRefno AS ProductRefno, " +
                "tProduct.ProductCode AS ProductCode, " +
                "tProduct.ProductDescription AS ProductDescription, " +
                "tProduct.ProductBrandDescription AS ProductBrandDescription, " +
                "tUnit.UnitCode AS UnitCode, " +
                "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                "tActivityItemReturns.ActivityItemReturnsQuantityReturned AS ActivityItemReturnsQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsBaseQuantityReturned AS ActivityItemReturnsBaseQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsWeightQuantityReturned AS ActivityItemReturnsWeightQuantityReturned, " +
                "tActivityItemReturns.ActivityItemReturnsCustomerProductUnitCode AS ActivityItemReturnsCustomerProductUnitCode, " +
                "tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno, " +
                "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                "CASE WHEN tActivityItemReturnsComment.ActivityItemReturnsCommentComment IS NULL " +
                "THEN \' \' ELSE \'*\' END AS IsActivityItemReturnsComment, " +
                "tActivityItem.ActivityItemStatus AS ActivityItemStatus " +
                "FROM tActivityItemReturns " +
                "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemReturns.ProductUnitRefno " +
                "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno " +
                "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno " +
                "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                "tActivityItemReturns.WarehouseLocationMapRefno " +
                "LEFT JOIN tActivityItemReturnsComment ON tActivityItemReturnsComment.ActivityItemRefno = " +
                "tActivityItemReturns.ActivityItemRefno " +
                "WHERE tActivityItem.ActivityRefno = ? " +
                "ORDER BY tActivityItemReturns.ActivityItemRefno";
                parameters.Add(this.ActHdr.HdrRefno);
            }

            // using (SQLiteDataAdapter dataAdapterReturnItems = new SQLiteDataAdapter(cmd))
            // {
            //    using (DataSet dataSetReturnItems = new DataSet())
            //    {
            //        dataAdapterReturnItems.Fill(dataSetReturnItems);
            //        this.DataViewItems = dataSetReturnItems.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            // }
            this.listReturnItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReturnItem>(query, parameters.ToArray());
            this.ListItems = this.listReturnItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActItemFilter))
            // {
            this.ListItems = this.listReturnItems.Where(returnItem => returnItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
            // }
            foreach (ReturnItem returnItem in this.ListItems)
            {
                // ReturnItem returnItem = new ReturnItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemReturnsQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsQuantityReturned"] : null),
                //    ActivityItemReturnsBaseQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsBaseQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsBaseQuantityReturned"] : null),
                //    ActivityItemReturnsWeightQuantityReturned = Convert.ToDecimal(row["ActivityItemReturnsWeightQuantityReturned"] != DBNull.Value ? row["ActivityItemReturnsWeightQuantityReturned"] : null),
                //    ActivityItemReturnsCustomerProductUnitCode = Convert.ToString(row["ActivityItemReturnsCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemReturnsCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    IsActivityItemReturnsComment = Convert.ToString(row["IsActivityItemReturnsComment"] != DBNull.Value ? row["IsActivityItemReturnsComment"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                // };
                observableCollection.Add(returnItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity object.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task AssignActivityItem(object activityItem)
        {
            if (activityItem is ReturnItem item)
            {
                this.SelectedActivityItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFKRefno = item.ActivityItemFKRefno;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                var locMap = await this.GetSelectedItemLocation(item.LocationMapRefno);

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Only assign the current location if the map is complete else leave it alone.
                    if (locMap != null && await LocationMapTasks.IsCompleteLocMap(locMap))
                    {
                        // Update the chosen location with the current when defaulting is turned on.
                        LocationMap.GetInstance().CurrLocMap = locMap.Clone();
                        LocationMap.GetInstance().ChosenLocMap = locMap.Clone();

                        // Assign the skid the location, because defaulting is enabled.
                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                        {
                            this.ActHdr.SkidInfo.CurrSkid.LocMap = locMap.Clone();
                        }

                        // Update the activity item location.
                        this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
                else
                {
                    // Only assign the current location if the map is complete else leave it alone.
                    if (locMap != null && await LocationMapTasks.IsCompleteLocMap(locMap))
                    {
                        // Do not update the current, chosen or skid location when defaulting is off.
                        this.LocMap = locMap.Clone();
                    }
                }

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            var foundActItemRefno = 0;
            var whsLocMapRefno = this.LocMap.WhsLocMapRefno;
            var query = "SELECT tActivityItem.ActivityItemRefno, " +
                        "tActivityItemReturns.ActivityItemFKRefno, " +
                        "tActivityItemReturns.ProductUnitRefno, " +
                        "tLocationMapWarehouse.LocationMapRefno " +
                        "FROM tActivityItemReturns " +
                        "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = " +
                        "tActivityItemReturns.ActivityItemRefno " +
                        "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = " +
                        "tActivityItemReturns.WarehouseLocationMapRefno " +
                        "WHERE tActivityItem.ActivityRefno = ? " +
                        "AND tActivityItemReturns.WarehouseLocationMapRefno = ? " +
                        "AND tActivityItemReturns.ProductUnitRefno = ?";

            var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReturnItem>(query, this.ActHdr.HdrRefno, whsLocMapRefno, prodUnitRefno);

            if (results != null && results.Count > 0)
            {
                // <WO ID="14777" comment="The last of the uncommented locmap code was removed here. We do not wish to set location map when we just want refnos." />
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                foundActItemRefno = this.ItemRefno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override Task<string> CreateSubmitXML(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var deviceID = this.SysEnv.DeviceID;
            var revision = this.SysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            var loginID = this.SysEnv.ContactLog.LoginID.ToString();
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            var current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT SALES RETURN ITEM</action>"
                + "</command>"
                + "<device_id>" + deviceID + "</device_id>"
                + "<login_id>" + loginID + "</login_id>"
                + "<revision_datetime>" + revision + "</revision_datetime>"
                + "<stamp>" + current + "</stamp>"
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>"
                + "<symbology>" + barCodeSymbologyPrefix + "</symbology>"
                + "<gtin>" + prodUnitBarCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.WhsFKRefno.ToString() + "</whs_id>"
                + "<sect_refno>" + this.LocMap.SectFKRefno.ToString() + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.SlotFKRefno.ToString() + "</slot_refno>"
                + "<refno></refno>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return Task.Run(() => xmlString.ToString());
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned = quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned = baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned = weightQuantity;
            }
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned += quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned += baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is ReturnItem returnItem)
            {
                var query = "UPDATE tActivityItemReturns " +
                            "SET ActivityItemReturnsQuantityReturned = ?, " +
                            "ActivityItemReturnsBaseQuantityReturned = ?, " +
                            "ActivityItemReturnsWeightQuantityReturned = ? " +
                            "WHERE ActivityItemRefno = ?";
                var parameters = new object[]
                {
                    returnItem.ActivityItemReturnsQuantityReturned,
                    returnItem.ActivityItemReturnsBaseQuantityReturned,
                    returnItem.ActivityItemReturnsWeightQuantityReturned,
                    returnItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key reference number, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKItRefno = ((ReceiveItem)resourceType).ActivityItemFKRefno;
            return tmpFKItRefno == null ? 0 : Convert.ToInt32(tmpFKItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Return # {0}", this.ActHdr.HdrFKRefno);
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFKRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<SActItemSchemeData> GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
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
    }
}
