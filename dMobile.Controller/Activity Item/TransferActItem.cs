// <copyright file="clsTransferActivityItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="TransferActItem" />.
    /// </summary>
    public class TransferActItem : ActItem
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
        /// Defines the columnWidth5.
        /// </summary>
        private double columnWidth5 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listTransferItems.
        /// </summary>
        private List<TransferItem> listTransferItems = new List<TransferItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
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
                            new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                    },
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

            bool isAddKey = true;
            foreach (var item in this.resourceDictionary)
            {
                if (item.Key == "dataTemplate")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.resourceDictionary.Add("dataTemplate", dataTemplate);
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
                int idx = 0;
                foreach (TransferItem item in this.ListItems)
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
            bool isCatchAsk = this.ProdInfo.IsCatchAsk;

            if (isCatchAsk)
            {
                // modules are for exclusions
                isCatchAsk = this.SysEnv.SysSettings.IsTransCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            bool result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsTransferNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// The IsReadyToApply.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.TransQtyOverWarn != SysCommon.ActTransfersQtyOverValLevels.NoValidationOrWarning && this.ItemFKRefno > 0)
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

                            if (await Device.InvokeOnMainThreadAsync(async () => !await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }

                            break;

                        case SysCommon.ActTransfersQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to parse the transfer item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to find the transfer item # {0}", actItemStringFKRefno);
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
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
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
                parametersActivityItem.Add(Convert.ToInt32(this.ActHdr.HdrRefno));

                transaction.Execute(queryActivityItem, parametersActivityItem.ToArray());

                this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));
                this.ItemFKRefno = this.ItemRefno * -1;

                string queryTransferItem = "INSERT INTO tActivityItemTransfer (" +
                    "ActivityItemRefno, " +
                    "ActivityItemSubType, " +
                    "ActivityItemFKRefno, " +
                    "ActivityItemStringFKRefno, " +
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
                parametersTransferItem.Add(Convert.ToInt32(this.ItemRefno));

                if (this.ActSubType == SysCommon.ActSubTypes.TransferFrom)
                {
                    parametersTransferItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferFrom));
                }
                else
                {
                    parametersTransferItem.Add(Convert.ToInt32(SysCommon.ActSubTypes.TransferTo));
                }

                parametersTransferItem.Add(Convert.ToInt32(this.ItemFKRefno));
                parametersTransferItem.Add(Convert.ToString(this.ItemFKRefno));
                parametersTransferItem.Add(Convert.ToInt32(prodUnitRefno));
                parametersTransferItem.Add(Convert.ToInt32(this.LocMap.WhsLocMapRefno));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));
                parametersTransferItem.Add(Convert.ToInt32(0));

                transaction.Execute(queryTransferItem, parametersTransferItem.ToArray());
                transaction.Commit();
            });

            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

            if (this.ListItems != null)
            {
                TransferItem item = new TransferItem()
                {
                    ActivityItemRefno = this.ItemRefno,
                    ActivityItemFKRefno = this.ItemFKRefno,
                    ActivityItemStringFKRefno = this.ItemFKRefno.ToString(),
                    ProductUnitRefno = prodUnitRefno,
                    ProductRefno = int.Parse(data.Root.Element("ProductRefno").Value),
                    ProductCode = data.Root.Element("ProductCode").Value,
                    ProductDescription = data.Root.Element("ProductDescription").Value,
                    UnitCode = data.Root.Element("UnitCode").Value,
                    ProductUnitBC = data.Root.Element("ProductUnitBC").Value,
                    ProductUnitSizeDescription = data.Root.Element("ProductUnitSizeDescription").Value,
                    LocationMapRefno = this.LocMap.WhsLocMapRefno,
                    LocationCode = this.LocMap.WhsCode,
                    LocationName = this.LocMap.WhsName,
                    ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    ActivityItemTransferRequiredQuantity = 0,
                    ActivityItemTransferQuantity = 0,
                    ActivityItemTransferBaseQuantity = 0,
                    ActivityItemTransferWeightQuantity = 0,
                };
                this.ListItems.Add(item);
                this.SelectedListItemIdx = this.ListItems.Count - 1;
                _ = this.AssignActivityItem(item);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
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
                    await tableActTransferDetail.PurgeData();
                    await tableActTransferItem.PurgeData();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    await tableActTransferItem.RequestData(this.ActHdr.HdrRefno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
                }
                else
                {
                    TableActivityTransferToItem tableActTransferItem = new TableActivityTransferToItem();
                    TableActivityTransferToDetail tableActTransferDetail = new TableActivityTransferToDetail();
                    await tableActTransferDetail.PurgeData();
                    await tableActTransferItem.PurgeData();

                    this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                    tableActTransferItem.RegisterDataAccessListenerProgressOut((TransferActItem)this);
                    await tableActTransferItem.RequestData(this.ActHdr.HdrRefno);
                    tableActTransferItem.UnregisterDataAccessListenerProgressOut((TransferActItem)this);
                }
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
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            Label labelProductIDHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelBCHeader = new Label { Text = "BC", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelRequiredHeader = new Label { Text = "Required", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelWhsHeader = new Label { Text = "Whs", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductIDHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelBCHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelRequiredHeader.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWhsHeader.Text);

            // Create header data template
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth5, GridUnitType.Absolute) },
                },
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

            GridMainActivity.Children.Add(gridHeader);

            foreach (TransferItem transferItem in this.ListItems)
            {
                // TransferItem transferItem = new TransferItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                //    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
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
                // };
                observableCollection.Add(transferItem);

                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.ProductUnitBC);
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(transferItem.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", transferItem.ActivityItemTransferRequiredQuantity));
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", transferItem.ActivityItemTransferQuantity));
                double detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(transferItem.LocationCode));

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }

                if (detailWidth5 > this.columnWidth5)
                {
                    this.columnWidth5 = detailWidth5;
                    gridHeader.ColumnDefinitions[5].Width = this.columnWidth5;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno,");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno,");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.AppendFormat(" AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append(" ORDER BY tActivityItemTransfer.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemTransfer.ActivityItemRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemFKRefno, ");
                sql.Append("tActivityItemTransfer.ActivityItemStringFKRefno, ");
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
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.HdrRefno);
                sql.Append("ORDER BY tActivityItemTransfer.ActivityItemRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
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
            // }
            this.listTransferItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TransferItem>(sql.ToString());
            this.ListItems = this.listTransferItems.Cast<object>().ToList();
            return this.ListItems;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // if (string.IsNullOrEmpty(this.ActItemFilter))
            // {
            this.ListItems = this.listTransferItems.Where(transferItem => transferItem.ProductCode.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (TransferItem transferItem in this.ListItems)
            {
                // TransferItem transferItem = new TransferItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null),
                //    ActivityItemTransferBaseQuantity = Convert.ToDecimal(row["ActivityItemTransferBaseQuantity"] != DBNull.Value ? row["ActivityItemTransferBaseQuantity"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemTransferQuantity = Convert.ToDecimal(row["ActivityItemTransferQuantity"] != DBNull.Value ? row["ActivityItemTransferQuantity"] : null),
                //    ActivityItemTransferRequiredQuantity = Convert.ToDecimal(row["ActivityItemTransferRequiredQuantity"] != DBNull.Value ? row["ActivityItemTransferRequiredQuantity"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
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
                // };
                observableCollection.Add(transferItem);
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
            if (activityItem is TransferItem item)
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

                SLocMap locMap = await this.GetSelectedItemLocation(item.LocationMapRefno);

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
            int foundItemRefno = 0;

            string query = "SELECT tActivityItem.ActivityItemRefno,"
                + "tActivityItemTransfer.ActivityItemFKRefno,"
                + "tActivityItemTransfer.ProductUnitRefno,"
                + "tLocationMapWarehouse.LocationMapRefno "
                + "FROM tActivityItemTransfer "
                + "INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = tActivityItemTransfer.ActivityItemRefno "
                + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                + "= tActivityItemTransfer.TransferFromWarehouseLocationMapRefno "
                + "WHERE tActivityItem.ActivityRefno = ? "
                + "AND tActivityItemTransfer.TransferFromWarehouseLocationMapRefno = ? "
                + "AND tActivityItemTransfer.ProductUnitRefno = ?";

            List<TransferItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<TransferItem>(
                query, this.ActHdr.HdrRefno, this.LocMap.WhsLocMapRefno, prodUnitRefno);

            if (results?.Count > 0)
            {
                this.ItemRefno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.ItemFKRefno = Convert.ToInt32(results[0].ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                foundItemRefno = this.ItemRefno;
            }

            return foundItemRefno;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT TRANSFER ITEM</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + data.Root.Element("ProductCode").Value.PadRight(10) + "</prod_id>"
                + "<alt_unit>" + data.Root.Element("UnitCode").Value + "</alt_unit>"
                + "<is_bc>" + bool.Parse(data.Root.Element("ProductUnitIsBC").Value) + "</is_bc>"
                + "<symbology>" + data.Root.Element("ProductUnitBarCodeSymbologyPrefix") + "</symbology>"
                + "<gtin>" + data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20) + "</gtin>"
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
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity = quantity;
                transferItem.ActivityItemTransferBaseQuantity = baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity = weightQuantity;
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
            if (selectedActivityItem is TransferItem transferItem)
            {
                transferItem.ActivityItemTransferQuantity += quantity;
                transferItem.ActivityItemTransferBaseQuantity += baseQuantity;
                transferItem.ActivityItemTransferWeightQuantity += weightQuantity;
            }
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task UpdateItemTableQuantity(object activityItem)
        {
            if (activityItem is TransferItem transferItem)
            {
                string query = "UPDATE tActivityItemTransfer SET ActivityItemTransferQuantity = ?," +
                    " ActivityItemTransferBaseQuantity = ?, ActivityItemTransferWeightQuantity = ?" +
                    " WHERE ActivityItemRefno = ?";
                object[] parameters = new object[]
                {
                    transferItem.ActivityItemTransferQuantity,
                    transferItem.ActivityItemTransferBaseQuantity,
                    transferItem.ActivityItemTransferWeightQuantity,
                    transferItem.ActivityItemRefno,
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// The TransferModuleQuantityOverCheck.
        /// </summary>
        /// <param name="actItemTransFKRefno">.</param>
        /// <param name="actItemTransQuantity">.</param>
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

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKItRefno = ((TransferItem)resourceType).ActivityItemFKRefno;
            return tmpFKItRefno == null ? 0 : Convert.ToInt32(tmpFKItRefno);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Transfer # {0}", this.ActHdr.HdrFKRefno);
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
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
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
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = 0;
            if (this.SelectedActivityItem is TransferItem item)
            {
                if (Math.Sign(item.ActivityItemTransferQuantity) == 0 ||
                    Math.Sign(item.ActivityItemTransferQuantity) == 1)
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
