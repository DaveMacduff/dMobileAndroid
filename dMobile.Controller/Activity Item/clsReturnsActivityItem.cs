namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.IO;
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
    using dMobile.Resources.Models;
    using System.Linq;

    public class ReturnsActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        private List<ReturnItem> listReturnItems = new List<ReturnItem>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public ReturnsActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(ReturnsActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Ret;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsRetCreateNewItemNoAsk;

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
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label labelIsActivityItemReturnsComment = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityItemReturnsQuantityReturned = new Label { VerticalOptions = LayoutOptions.Center };

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
                isCatchAsk = this.SysEnv.SysSettings.IsRetCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsReturnsNoCatchAskZeroEnabled;
            }

            return isCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            string comment = await base.GetComment(selectedActivityItem);

            if (selectedActivityItem is ReturnItem returnItem)
            {
                List<tActivityItemReturnsCommentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemReturnsCommentModel>("SELECT * FROM tActivityItemReturnsComment WHERE ActivityItemRefno = ?", returnItem.ActivityItemRefno);

                if (results != null && results.Count > 0)
                {
                    comment = results[0].ActivityItemReturnsCommentComment;
                }
            }

            return comment;
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
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse returns item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find returns item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemReturnsStringFKRefno";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            // TODO: Resource strings
            throw new NotImplementedException("Add new return items is not supported.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlParam"></param>
        public override async Task Refresh(XDocument xmlParam)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityReturnsItem tableActReturnsItem = new TableActivityReturnsItem();
                TableActivityReturnsDetail tableActReturnsDetail = new TableActivityReturnsDetail();
                await tableActReturnsDetail.Purge();
                await tableActReturnsItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActReturnsItem.RegisterDataAccessListenerProgressOut((ReturnsActItem)this);
                await tableActReturnsItem.Request(ActHdr.Refno);
                tableActReturnsItem.UnregisterDataAccessListenerProgressOut((ReturnsActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource strings
            Label labelIsActivityItemReturnsCommentHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelProductUnitBCHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemReturnsQuantityReturnedHeader = new Label { Text = "Returned", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemReturnsCommentHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductCodeHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductUnitBCHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemReturnsQuantityReturnedHeader.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;

            Grid gridHeaderReturns = new Grid
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

            dataGridMainActivityHeader.Children.Add(gridHeaderReturns);

            foreach (ReturnItem returnItem in this.ListItems)
            {
                //ReturnItem returnItem = new ReturnItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemReturnsFKRefno = Convert.ToInt32(row["ActivityItemReturnsFKRefno"] != DBNull.Value ? row["ActivityItemReturnsFKRefno"] : null),
                //    ActivityItemReturnsStringFKRefno = Convert.ToString(row["ActivityItemReturnsStringFKRefno"] != DBNull.Value ? row["ActivityItemReturnsStringFKRefno"] : null),
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
                //};

                observableCollection.Add(returnItem);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.IsActivityItemReturnsComment);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.ProductCode);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.ProductUnitBC);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(returnItem.UnitCode);
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", returnItem.ActivityItemReturnsQuantityReturned));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeaderReturns.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeaderReturns.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeaderReturns.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeaderReturns.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeaderReturns.ColumnDefinitions[4].Width = columnWidth4;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            string query = string.Empty;
            List<object> parameters = new List<object>();

            if (isSubmitBatch)
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno AS ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemReturnsFKRefno AS ActivityItemReturnsFKRefno, " +
                "tActivityItemReturns.ActivityItemReturnsStringFKRefno AS ActivityItemReturnsStringFKRefno, " +
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
                "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
                "tActivityItemReturns.ActivityItemReturnsStringFKRefno, " +
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

                parameters.Add(this.ActHdr.Refno);
                parameters.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                query = "SELECT tActivityItemReturns.ActivityItemRefno AS ActivityItemRefno, " +
                "tActivityItemReturns.ActivityItemReturnsFKRefno AS ActivityItemReturnsFKRefno, " +
                "tActivityItemReturns.ActivityItemReturnsStringFKRefno AS ActivityItemReturnsStringFKRefno, " +
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
                parameters.Add(this.ActHdr.Refno);
            }

            //using (SQLiteDataAdapter dataAdapterReturnItems = new SQLiteDataAdapter(cmd))
            //{
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
            //}

            this.listReturnItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReturnItem>(query, parameters.ToArray());
            this.ListItems = this.listReturnItems.Cast<object>().ToList();
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

            //if (string.IsNullOrEmpty(this.ActItemFilter))
            //{
            this.ListItems = this.listReturnItems.Where(returnItem => returnItem.ProductCode.ToString().Contains(searchText.Trim())).Cast<object>().ToList();
            //}
            //else
            //{
            // TODO: Fix filtering
            //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
            //}

            foreach (ReturnItem returnItem in this.ListItems)
            {
                //ReturnItem returnItem = new ReturnItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemReturnsFKRefno = Convert.ToInt32(row["ActivityItemReturnsFKRefno"] != DBNull.Value ? row["ActivityItemReturnsFKRefno"] : null),
                //    ActivityItemReturnsStringFKRefno = Convert.ToString(row["ActivityItemReturnsStringFKRefno"] != DBNull.Value ? row["ActivityItemReturnsStringFKRefno"] : null),
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
                //};

                observableCollection.Add(returnItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is ReturnItem returnItem)
            {
                await base.SelectActivityItem(returnItem);
                this.FKRefno = Convert.ToInt32(returnItem.ActivityItemReturnsFKRefno);
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
            int whsLocMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno;
            string query = "SELECT tActivityItem.ActivityItemRefno, " +
            "tActivityItemReturns.ActivityItemReturnsFKRefno, " +
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

            List<ReturnItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReturnItem>(query, this.ActHdr.Refno, whsLocMapRefno, prodUnitRefno);

            if (results != null && results.Count > 0)
            {
                // <WO ID="14777" comment="The last of the uncommented locmap code was removed here. We do not wish to set location map when we just want refnos." />
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemReturnsFKRefno);
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
            string deviceID = this.SysEnv.DeviceID;
            string revision = this.SysEnv.RefreshOpt.RefreshedDataLast.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
            string loginID = this.SysEnv.ContactLog.LoginID.ToString();
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string prodUnitBarCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string barCodeSymbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;
            string current = DateTime.Now.ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT SALES RETURN ITEM</action>"
                + "</command>"
                + ("<device_id>" + deviceID + "</device_id>")
                + ("<login_id>" + loginID + "</login_id>")
                + ("<revision_datetime>" + revision + "</revision_datetime>")
                + ("<stamp>" + current + "</stamp>")
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
                + ("<symbology>" + barCodeSymbologyPrefix + "</symbology>")
                + ("<gtin>" + prodUnitBarCodeCode + "</gtin>")
                + ("<whs_id>" + LocMap.CurrLocMap.WhsFKRefno.ToString() + "</whs_id>")
                + ("<sect_refno>" + LocMap.CurrLocMap.SectFKRefno.ToString() + "</sect_refno>")
                + ("<slot_refno>" + LocMap.CurrLocMap.SlotFKRefno.ToString() + "</slot_refno>")
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned = quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned = baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned = weightQuantity;
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
            if (selectedActivityItem is ReturnItem returnItem)
            {
                returnItem.ActivityItemReturnsQuantityReturned += quantity;
                returnItem.ActivityItemReturnsBaseQuantityReturned += baseQuantity;
                returnItem.ActivityItemReturnsWeightQuantityReturned += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is ReturnItem returnItem)
            {
                string query = "UPDATE tActivityItemReturns " +
                    "SET ActivityItemReturnsQuantityReturned = ?, " +
                    "ActivityItemReturnsBaseQuantityReturned = ?, " +
                    "ActivityItemReturnsWeightQuantityReturned = ? " +
                    "WHERE ActivityItemRefno = ?";
                object[] parameters = new object[]
                {
                    returnItem.ActivityItemReturnsQuantityReturned,
                    returnItem.ActivityItemReturnsBaseQuantityReturned,
                    returnItem.ActivityItemReturnsWeightQuantityReturned,
                    actItemRefno
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        #endregion
    }
}
