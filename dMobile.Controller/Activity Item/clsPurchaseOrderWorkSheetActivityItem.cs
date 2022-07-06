// <copyright file="clsPurchaseOrderWorkSheetActivityItem.cs" company="InterSect Business Systems Inc">
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
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="POWorkSheetActItem" />.
    /// </summary>
    public class POWorkSheetActItem : ActItem
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
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listPurchaseOrderWorkSheetItems.
        /// </summary>
        private List<PurchaseOrderWorkSheetItem> listPurchaseOrderWorkSheetItems = new List<PurchaseOrderWorkSheetItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="POWorkSheetActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public POWorkSheetActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(POWorkSheetActItem).FullName;

            // Always allow adding product
            this.IsAllowAddItem = true;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPOWorkSheetCreateNewItemNoAsk;

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
                    },
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

        /// <summary>
        /// Gets or sets the NewActItemFKRefno.
        /// </summary>
        public int NewActItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether IsUpdateView.
        /// </summary>
        public bool IsUpdateView { get; set; } = true;

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
                isCatchAsk = this.SysEnv.SysSettings.IsPOWorkSheetCatchAskEnabled;
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
                result = !this.SysEnv.SysSettings.IsPOWorkSheetNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to parse purchase order work sheet item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Format("Unable to find purchase order work sheet item# {0}", actItemStringFKRefno);
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
        /// The Insert.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <param name="askToInsertAsNewItem">The askToInsertAsNewItem<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int actRefno = 0;

            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);

            if (prodUnitRefno != 0)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                    {
                        try
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            this.ActHdr.HdrRefno = actRefno;
                            this.ActHdr.HdrFKRefno = actRefno;

                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet), Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(this.ActHdr.HdrRefno));

                            this.ItemRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));

                            string queryActSheetItem = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemFKRefno, " +
                                "ActivityItemStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityItemPurchaseOrderWorkSheetQuantityOrdered, " +
                                "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered, " +
                                "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered" +
                                ") VALUES (" +
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
                            object[] parametersActSheetItem = new object[]
                            {
                                Convert.ToInt32(this.ItemRefno),
                                Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                                Convert.ToInt32(this.ItemRefno * -1),
                                Convert.ToString(this.ItemRefno * -1),
                                Convert.ToInt32(prodUnitRefno),
                                Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno),
                                decimal.Parse(data.Root.Element("alt_qty").Value),
                                decimal.Parse(data.Root.Element("base_qty").Value),
                                decimal.Parse(data.Root.Element("wt_qty").Value),
                            };

                            transaction.Execute(queryActSheetItem, parametersActSheetItem);
                            transaction.Commit();

                            this.ItemFKRefno = this.ItemRefno * -1;
                            this.IsUpdateView = true;
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();

                            Console.WriteLine(ex);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            Console.WriteLine(ex);
                        }
                    });
                }
                else
                {
                    string xmlSend = await this.CreateSubmitXML(data);
                    string reqData = SysCommon.SubmitComm(xmlSend);

                    if (reqData.Length > 0)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);

                        // check if result is an error message
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new MessageOnlyException(errorDetail);
                            }
                            else
                            {
                                throw new Exception(errorDetail);
                            }
                        }
                        else
                        {
                            this.IsUpdateView = true;
                            this.NewActItemFKRefno = -Math.Abs(int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText));
                            this.ItemFKRefno = -Math.Abs(int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText));
                            this.ActHdr.HdrFKRefno = this.ItemFKRefno;
                            this.ItemRefno = Math.Abs(this.ItemFKRefno);
                        }
                    }
                }
            }

            if (this.IsUpdateView && this.ListItems != null)
            {
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                // TODO: Fix sorting
                // string oldSort = DataViewItems.Sort;
                // int rowIdx = 0;
                // this.DataViewItems.Sort = "ActivityItemFKRefno";
                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = null;
                bool toBeAdded = false;

                if (this.ItemFKRefno > 0)
                {
                    purchaseOrderWorkSheetItem = this.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(worksheetItem => worksheetItem.ActivityItemFKRefno == this.ItemFKRefno).FirstOrDefault();
                }

                if (purchaseOrderWorkSheetItem == null)
                {
                    purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem();
                    purchaseOrderWorkSheetItem.ActivityItemRefno = this.ItemRefno;
                    purchaseOrderWorkSheetItem.ProductUnitRefno = this.ProdInfo.UnitRefno;
                    purchaseOrderWorkSheetItem.ProductRefno = this.ProdInfo.Refno;
                    purchaseOrderWorkSheetItem.ProductCode = this.ProdInfo.Code;
                    purchaseOrderWorkSheetItem.ProductDescription = this.ProdInfo.Desc;
                    purchaseOrderWorkSheetItem.ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc;
                    purchaseOrderWorkSheetItem.UnitCode = this.ProdInfo.UnitCode;
                    purchaseOrderWorkSheetItem.ActivityItemFKRefno = this.ItemFKRefno;
                    purchaseOrderWorkSheetItem.ActivityItemStringFKRefno = this.ItemFKRefno.ToString();

                    toBeAdded = true;
                }

                purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                purchaseOrderWorkSheetItem.LocationMapRefno = this.LocMap.LoginWhsLocMapRefno;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = Math.Round(decimal.Parse(data.Root.Element("alt_qty").Value), 3);
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = Math.Round(decimal.Parse(data.Root.Element("base_qty").Value), 3);
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = Math.Round(decimal.Parse(data.Root.Element("wt_qty").Value), 3);
                purchaseOrderWorkSheetItem.LocationCode = this.LocMap.LoginWhsLocMapRefno;
                purchaseOrderWorkSheetItem.LocationName = this.LocMap.CurrLocMap.WhsName;

                // TODO: Fix sorting
                // this.DataViewItems.Sort = oldSort;
                if (toBeAdded)
                {
                    this.ListItems.Add(purchaseOrderWorkSheetItem);
                }
            }
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Update(XDocument data)
        {
            decimal decQty = Math.Round(decimal.Parse(data.Root.Element("alt_qty").Value), 3);
            decimal decBaseQty = Math.Round(decimal.Parse(data.Root.Element("base_qty").Value), 3);
            decimal decWeightQty = Math.Round(decimal.Parse(data.Root.Element("wt_qty").Value), 3);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                {
                    try
                    {
                        // Update the Activity Item quantity shipped
                        string query = "UPDATE tActivityItemPurchaseOrderWorkSheet SET " +
                            "ActivityItemPurchaseOrderWorkSheetQuantityOrdered = ?, " +
                            "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = ?, " +
                            "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = ? " +
                            "WHERE ActivityItemRefno = ?";
                        transaction.Execute(query, decQty, decBaseQty, decWeightQty, this.ItemRefno);
                        transaction.Execute("UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?", Convert.ToInt32(SysCommon.ActStatus.Updated), this.ItemRefno);
                        transaction.Execute("UPDATE tActivity SET ActivityStatus = ? WHERE ActivityRefno = ?", Convert.ToInt32(SysCommon.ActStatus.Updated), this.ActHdr.HdrRefno);
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                });
            }
            else
            {
                string xmlSend = await this.CreateSubmitXML(data);
                string reqData = SysCommon.SubmitComm(xmlSend);

                if (reqData.Length > 0)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reqData);

                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                    else
                    {
                        this.IsUpdateView = true;
                        this.NewActItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                        this.ItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                        this.ActHdr.HdrFKRefno = this.ItemFKRefno;
                    }
                }
            }

            if (this.IsUpdateView && this.ListItems != null)
            {
                // TODO: Fix sorting
                // DataRowView rowView = null;
                // string oldSort = this.DataViewItems.Sort;
                // int rowIdx = 0;
                // this.DataViewItems.Sort = "ActivityItemFKRefno";
                // rowIdx = DataViewItems.Find(new object[] { this.FKRefno });
                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = this.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(worksheetItem => worksheetItem.ActivityItemFKRefno == this.ItemFKRefno).FirstOrDefault();
                bool toBeUpdated = false;

                if (purchaseOrderWorkSheetItem == null)
                {
                    purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem();
                    purchaseOrderWorkSheetItem.ActivityItemRefno = this.ItemRefno;
                    purchaseOrderWorkSheetItem.ProductUnitRefno = this.ProdInfo.UnitRefno;
                    purchaseOrderWorkSheetItem.ProductRefno = this.ProdInfo.Refno;
                    purchaseOrderWorkSheetItem.ProductCode = this.ProdInfo.Code;
                    purchaseOrderWorkSheetItem.ProductDescription = this.ProdInfo.Desc;
                    purchaseOrderWorkSheetItem.ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc;
                    purchaseOrderWorkSheetItem.UnitCode = this.ProdInfo.UnitCode;
                    purchaseOrderWorkSheetItem.ActivityItemFKRefno = this.ItemFKRefno;
                    purchaseOrderWorkSheetItem.ActivityItemStringFKRefno = this.ItemFKRefno.ToString();

                    toBeUpdated = true;
                }

                purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                purchaseOrderWorkSheetItem.LocationMapRefno = this.LocMap.LoginWhsLocMapRefno;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = decQty;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = decBaseQty;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = decWeightQty;
                purchaseOrderWorkSheetItem.LocationCode = this.LocMap.LoginWhsLocMapRefno;
                purchaseOrderWorkSheetItem.LocationName = this.LocMap.CurrLocMap.WhsName;

                // TODO: Fix sorting
                // this.DataViewItems.Sort = oldSort;
                if (toBeUpdated)
                {
                    this.ListItems.Add(purchaseOrderWorkSheetItem);
                }
            }
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(XDocument data, object selectedActivityItem)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = await this.CreateSubmitXML(data);
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }
                else
                {
                    this.ItemFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
                    {
                        purchaseOrderWorkSheetItem.ActivityItemFKRefno = this.ItemFKRefno;
                        purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityItem SET tActivityItem.ActivityItemStatus = ? WHERE ActivityItemRefno = ?", (int)SysCommon.ActStatus.NoChange, this.ItemRefno);
                }
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

                TableActivityPurchaseOrderWorkSheetItem tableActPOWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
                await tableActPOWorkSheetItem.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPOWorkSheetItem.RegisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
                await tableActPOWorkSheetItem.RequestData(this.ActHdr.HdrRefno);
                tableActPOWorkSheetItem.UnregisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
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

            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in this.ListItems)
            {
                // PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
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
                // };
                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            Label labelProductID = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelOrdered = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelWeight = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelProductID.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelOrdered.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeight.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelProductID);
            gridHeader.Children.Add(labelOrdered);
            gridHeader.Children.Add(labelWeight);

            Grid.SetColumn(labelProductID, 0);
            Grid.SetColumn(labelOrdered, 1);
            Grid.SetColumn(labelWeight, 2);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(purchaseOrderWorkSheetItem.ProductCode);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered));

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
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }
            else
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered AS ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
                sql.Append("ORDER BY tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewItems = ds.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.Sort = "ActivityItemFKRefno";
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //    }
            // }
            this.listPurchaseOrderWorkSheetItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PurchaseOrderWorkSheetItem>(sql.ToString());
            this.ListItems = this.listPurchaseOrderWorkSheetItems.OrderBy(poItem => poItem.ActivityItemFKRefno).Cast<object>().ToList();
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
            this.ListItems = this.listPurchaseOrderWorkSheetItems.Where(purchaseOrderWorkSheetItem => purchaseOrderWorkSheetItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            // }
            // else
            // {
            // TODO: Fix filtering
            // this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            // }
            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in this.ListItems)
            {
                // PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem
                // {
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemFKRefno = Convert.ToInt32(row["ActivityItemFKRefno"] != DBNull.Value ? row["ActivityItemFKRefno"] : null),
                //    ActivityItemStringFKRefno = Convert.ToString(row["ActivityItemStringFKRefno"] != DBNull.Value ? row["ActivityItemStringFKRefno"] : null),
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
                // };
                observableCollection.Add(purchaseOrderWorkSheetItem);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select an activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity object.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            int activityItemLocMapRefno = 0;
            if (selectedActivityItem is PurchaseOrderWorkSheetItem item)
            {
                this.SelectedActivityItem = item;
                this.ItemRefno = item.ActivityItemRefno;
                this.ItemFKRefno = Convert.ToInt32(item.ActivityItemFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                activityItemLocMapRefno = item.LocationMapRefno;
            }

            await this.GetSelectedItemLocation(activityItemLocMapRefno);

            // LocationMap.GetInstance().CurrLocMap needs to be set before call to GetSchemeComponentsFromRefno.
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(
                this.ProdInfo.Refno);
        }

        /// <summary>
        /// The GetActivityItemRefno.
        /// </summary>
        /// <param name="prodUnitRefno">The prodUnitRefno<see cref="int"/>.</param>
        /// <returns>.</returns>
        public override async Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemFKRefno,");
            sql.Append("tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemPurchaseOrderWorkSheet ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno ");
            sql.AppendFormat(
                "WHERE tActivityItemPurchaseOrderWorkSheet.WarehouseLocationMapRefno = {0} ",
                this.LocMap.CurrLocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemPurchaseOrderWorkSheet.ProductUnitRefno = {0} ", prodUnitRefno);

            List<PurchaseOrderWorkSheetItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PurchaseOrderWorkSheetItem>(sql.ToString().ToString());

            if (results != null && results.Count > 0)
            {
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(results[0].LocationMapRefno));
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
        public async override Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            string unitCode = data.Root.Element("UnitCode").Value;
            string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PURCHASE ORDER WORK SHEET DETAIL</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<refno>" + this.ItemFKRefno + "</refno>"
                + "<itm_refno>" + this.ItemFKRefno + "</itm_refno>"
                + "<alt_qty>" + data.Root.Element("alt_qty").Value + "</alt_qty>"
                + "<base_qty>" + data.Root.Element("base_qty").Value + "</base_qty>"
                + "<wt_qty>" + data.Root.Element("wt_qty").Value + "</wt_qty>"
                + "<alt_unit>" + this.ProdInfo.UnitCode + "</alt_unit>"
                + "<base_unit>" + this.ProdInfo.BaseUnitCode + "</base_unit>"
                + "<wt_unit>" + this.ProdInfo.WeightUnitCode + "</wt_unit>"
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
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = weightQuantity;
            }
        }

        /// <summary>
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered += quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered += baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered += weightQuantity;
            }
        }

        /// <summary>
        /// The UpdateQuantityFromActivityItemList.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="actItemRefno">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                string query = "UPDATE tActivityItemPurchaseOrderWorkSheet SET " +
                    "ActivityItemPurchaseOrderWorkSheetQuantityOrdered = ?, " +
                    "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = ?, " +
                    "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = ? " +
                    "WHERE ActivityItemRefno = ?";
                object[] parameters = new object[]
                {
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered,
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered,
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered,
                    actItemRefno,
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("PO worksheet # {0}", this.ActHdr.HdrFKRefno);
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
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
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
            if (this.SelectedActivityItem is PurchaseOrderWorkSheetItem item)
            {
                if (Math.Sign(item.ActivityItemPurchaseOrderWorkSheetQuantityOrdered) == 0 ||
                    Math.Sign(item.ActivityItemPurchaseOrderWorkSheetQuantityOrdered) == 1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
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
