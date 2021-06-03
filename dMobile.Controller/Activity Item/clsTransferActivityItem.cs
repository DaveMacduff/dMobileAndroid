namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
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
    using System.Linq;

    public class TransferActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<TransferItem> listTransferItems = new List<TransferItem>();

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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
                            if (await Device.InvokeOnMainThreadAsync(async () => !await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
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

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(async (transaction) =>
            {
                string queryActivityItem = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)";
                List<object> parametersActivityItem = new List<object>();

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    parametersActivityItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));
                }
                else
                {
                    parametersActivityItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                }

                parametersActivityItem.Add(Convert.ToInt32(SysCommon.ActStatus.NoChange));
                parametersActivityItem.Add(Convert.ToInt32(this.ActHdr.Refno));

                transaction.Execute(queryActivityItem, parametersActivityItem.ToArray());

                this.Refno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                this.FKRefno = this.Refno * -1;

                string queryTransferItem = "INSERT INTO tActivityItemTransfer (" +
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
                List<object> parametersTransferItem = new List<object>();
                parametersTransferItem.Add(Convert.ToInt32(this.Refno));

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    parametersTransferItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));
                }
                else
                {
                    parametersTransferItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                }

                parametersTransferItem.Add(Convert.ToInt32(this.FKRefno));
                parametersTransferItem.Add(Convert.ToString(this.FKRefno));
                parametersTransferItem.Add(Convert.ToInt32(prodUnitRefno));
                parametersTransferItem.Add(Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));

                transaction.Execute(queryTransferItem, parametersTransferItem.ToArray());

                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                if (this.ListItems != null)
                {
                    this.ListItems.Add(new TransferItem()
                    {
                        ActivityItemRefno = this.Refno,
                        ActivityItemTransferFKRefno = this.FKRefno,
                        ActivityItemTransferStringFKRefno = this.FKRefno.ToString(),
                        ProductUnitRefno = prodUnitRefno,
                        ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                        ProductCode = data.Root.Element("ProductCode").Value,
                        ProductDescription = data.Root.Element("ProductDescription").Value,
                        UnitCode = data.Root.Element("UnitCode").Value,
                        ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                        ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                        LocationMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno,
                        LocationCode = this.LocMap.CurrLocMap.WhsCode,
                        LocationName = this.LocMap.CurrLocMap.WhsName,
                        ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                        ActivityItemTransferRequiredQuantity = 0,
                        ActivityItemTransferQuantity = 0,
                        ActivityItemTransferBaseQuantity = 0,
                        ActivityItemTransferWeightQuantity = 0
                    });
                }
            });
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

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    TableActivityTransferFromItem tableActTransferItem = new TableActivityTransferFromItem();
                    TableActivityTransferFromDetail tableActTransferDetail = new TableActivityTransferFromDetail();
                    await tableActTransferDetail.Purge();
                    await tableActTransferItem.Purge();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    await tableActTransferItem.Request(this.ActHdr.Refno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
                }
                else
                {
                    TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                    TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                    await tableActTransferDetail.Purge();
                    await tableActTransferItem.Purge();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    await tableActTransferItem.Request(this.ActHdr.Refno);
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

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIDHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRequiredHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);
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

            foreach (TransferItem transferItem in this.ListItems)
            {
                //TransferItem transferItem = new TransferItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                //    ActivityItemTransferFKRefno = Convert.ToInt32(row["ActivityItemTransferFKRefno"] != DBNull.Value ? row["ActivityItemTransferFKRefno"] : null),
                //    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                //    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                //    ActivityItemTransferStringFKRefno = Convert.ToString(row["ActivityItemTransferStringFKRefno"] != DBNull.Value ? row["ActivityItemTransferStringFKRefno"] : null),
                //    ActivityItemTransferWeightQuantity = Convert.ToDecimal(row["ActivityItemTransferWeightQuantity"] != DBNull.Value ? row["ActivityItemTransferWeightQuantity"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                //};

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

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

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

            this.listTransferItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TransferItem>(sql.ToString());
            this.ListItems = this.listTransferItems.Cast<object>().ToList();
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
            this.ListItems = this.listTransferItems.Where(transferItem => transferItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            //}
            //else
            //{
                // TODO: Fix filtering
                //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            //}

            foreach (TransferItem transferItem in this.ListItems)
            {
                //TransferItem transferItem = new TransferItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                //    ActivityItemTransferFKRefno = Convert.ToInt32(row["ActivityItemTransferFKRefno"] != DBNull.Value ? row["ActivityItemTransferFKRefno"] : null),
                //    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                //    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                //    ActivityItemTransferStringFKRefno = Convert.ToString(row["ActivityItemTransferStringFKRefno"] != DBNull.Value ? row["ActivityItemTransferStringFKRefno"] : null),
                //    ActivityItemTransferWeightQuantity = Convert.ToDecimal(row["ActivityItemTransferWeightQuantity"] != DBNull.Value ? row["ActivityItemTransferWeightQuantity"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null)
                //};

                observableCollection.Add(transferItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is TransferItem transferItem)
            {
                await base.SelectActivityItem(transferItem);
                this.FKRefno = Convert.ToInt32(transferItem.ActivityItemTransferFKRefno);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundItemRefno = 0;

            string query = "SELECT tActivityItem.ActivityItemRefno,"
                + "tActivityItemTransfer.ActivityItemTransferFKRefno,"
                + "tActivityItemTransfer.ProductUnitRefno,"
                + "tLocationMapWarehouse.LocationMapRefno "
                + "FROM tActivityItemTransfer "
                + "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemTransfer.ActivityItemRefno "
                + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                + "= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno "
                + "WHERE tActivityItem.ActivityRefno = ? "
                + "AND tActivityItemTransfer.TransferFromWarehouseLocationMapRefno = ? "
                + "AND tActivityItemTransfer.ProductUnitRefno = ?";

            List<TransferItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TransferItem>(query, this.ActHdr.Refno, this.LocMap.CurrLocMap.WhsLocMapRefno, prodUnitRefno);

            if (results?.Count > 0)
            {
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemTransferFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                foundItemRefno = this.Refno;
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
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity = quantity;
                transferItem.ActivityItemTransferBaseQuantity = baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity = weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity += quantity;
                transferItem.ActivityItemTransferBaseQuantity += baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="actItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is TransferItem transferItem)
            {
                string query = "UPDATE tActivityItemTransfer SET ActivityItemTransferQuantity = ?, ActivityItemTransferBaseQuantity = ?, ActivityItemTransferWeightQuantity = ? WHERE ActivityItemRefno = ?";
                object[] parameters = new object[] { transferItem.ActivityItemTransferQuantity, transferItem.ActivityItemTransferBaseQuantity, transferItem.ActivityItemTransferWeightQuantity, actItemRefno };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
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
            if (this.SelectedActivityItem is TransferItem transferItem)
            {
                decimal transferred = Convert.ToDecimal(transferItem.ActivityItemTransferQuantity);
                decimal required = Convert.ToDecimal(transferItem.ActivityItemTransferRequiredQuantity);

                if (transferred + actItemTransQuantity > required)
                {
                    throw new Exception(string.Format("The resulting quantity transferred {0} {1} will be greater than the quantity required {2} {3}.", transferred + actItemTransQuantity, this.ProdInfo.UnitCode, required, this.ProdInfo.UnitCode));
                }
            }
        }

        #endregion
    }
}