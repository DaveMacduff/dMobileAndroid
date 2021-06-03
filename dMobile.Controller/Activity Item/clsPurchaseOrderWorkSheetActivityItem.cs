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
    using System.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;
    using System.Collections.Generic;

    public class POWorkSheetActItem : ActItem
    {
        #region Fields

        private bool isUpdateView = true;
        private int newActItemFKRefno = 0;
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<PurchaseOrderWorkSheetItem> listPurchaseOrderWorkSheetItems = new List<PurchaseOrderWorkSheetItem>();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
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

        #region Properties

        public int NewActItemFKRefno
        {
            get
            {
                return this.newActItemFKRefno;
            }

            set
            {
                this.newActItemFKRefno = value;
            }
        }

        public bool IsUpdateView
        {
            get
            {
                return this.isUpdateView;
            }

            set
            {
                this.isUpdateView = value;
            }
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
                isCatchAsk = this.SysEnv.SysSettings.IsPOWorkSheetCatchAskEnabled;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsPOWorkSheetNoCatchAskZeroEnabled;
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
            return string.Format("Unable to parse purchase order work sheet item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find purchase order work sheet item# {0}", actItemStringFKRefno);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemPurchaseOrderWorkSheetStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            int actRefno = 0;

            this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);

            if (prodUnitRefno != 0)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(async (transaction) =>
                    {
                        try
                        {
                            transaction.Execute("INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)", Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet));

                            actRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivity"));
                            this.ActHdr.Refno = actRefno;
                            this.ActHdr.FKRefno = actRefno;

                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet), Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(this.ActHdr.Refno));

                            this.Refno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));

                            string queryActSheetItem = "INSERT INTO tActivityItemPurchaseOrderWorkSheet (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemPurchaseOrderWorkSheetFKRefno, " +
                                "ActivityItemPurchaseOrderWorkSheetStringFKRefno, " +
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
                                Convert.ToInt32(this.Refno),
                                Convert.ToInt32(SysCommon.ActSubTypes.PurchaseOrderWorkSheet),
                                Convert.ToInt32(this.Refno * -1),
                                Convert.ToString(this.Refno * -1),
                                Convert.ToInt32(prodUnitRefno),
                                Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno),
                                decimal.Parse(data.Root.Element("alt_qty").Value),
                                decimal.Parse(data.Root.Element("base_qty").Value),
                                decimal.Parse(data.Root.Element("wt_qty").Value)
                            };

                            transaction.Execute(queryActSheetItem, parametersActSheetItem);
                            transaction.Commit();

                            this.FKRefno = this.Refno * -1;

                            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                            this.IsUpdateView = true;
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            // TODO: Logging
                            Console.WriteLine(ex);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            // TODO: Logging
                            Console.WriteLine(ex);
                        }
                    });
                }
                else
                {
                    string xmlSend = this.CreateSubmitXML(data);
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
                            this.FKRefno = -Math.Abs(int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText));
                            this.ActHdr.FKRefno = this.FKRefno;
                            this.Refno = Math.Abs(this.FKRefno);
                        }
                    }
                }
            }

            if (this.IsUpdateView && this.ListItems != null)
            {
                // TODO: Fix sorting
                //string oldSort = DataViewItems.Sort;
                //int rowIdx = 0;
                //this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = null;
                bool toBeAdded = false;

                if (this.FKRefno > 0)
                {
                    purchaseOrderWorkSheetItem = this.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(worksheetItem => worksheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno == this.FKRefno).FirstOrDefault();
                }

                if (purchaseOrderWorkSheetItem == null)
                {
                    purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem();
                    purchaseOrderWorkSheetItem.ActivityItemRefno = this.Refno;
                    purchaseOrderWorkSheetItem.ProductUnitRefno = this.ProdInfo.UnitRefno;
                    purchaseOrderWorkSheetItem.ProductRefno = this.ProdInfo.Refno;
                    purchaseOrderWorkSheetItem.ProductCode = this.ProdInfo.Code;
                    purchaseOrderWorkSheetItem.ProductDescription = this.ProdInfo.Desc;
                    purchaseOrderWorkSheetItem.ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc;
                    purchaseOrderWorkSheetItem.UnitCode = this.ProdInfo.UnitCode;
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno = this.FKRefno;
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetStringFKRefno = this.FKRefno.ToString();

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
                //this.DataViewItems.Sort = oldSort;

                if (toBeAdded)
                {
                    this.ListItems.Add(purchaseOrderWorkSheetItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
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
                        transaction.Execute(query, decQty, decBaseQty, decWeightQty, this.Refno);
                        transaction.Execute("UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?", Convert.ToInt32(SysCommon.ActStatus.Updated), this.Refno);
                        transaction.Execute("UPDATE tActivity SET ActivityStatus = ? WHERE ActivityRefno = ?", Convert.ToInt32(SysCommon.ActStatus.Updated), this.ActHdr.Refno);
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        // TODO: Logging
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Logging
                        transaction.Rollback();
                        Console.WriteLine(ex);
                    }
                });
            }
            else
            {
                string xmlSend = this.CreateSubmitXML(data);
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
                        this.FKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                        this.ActHdr.FKRefno = this.FKRefno;
                    }
                }
            }

            if (this.IsUpdateView && this.ListItems != null)
            {
                // TODO: Fix sorting
                //DataRowView rowView = null;
                //string oldSort = this.DataViewItems.Sort;
                //int rowIdx = 0;
                //this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
                //rowIdx = DataViewItems.Find(new object[] { this.FKRefno });
                PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem = this.ListItems.OfType<PurchaseOrderWorkSheetItem>().Where(worksheetItem => worksheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno == this.FKRefno).FirstOrDefault();
                bool toBeUpdated = false;

                if (purchaseOrderWorkSheetItem == null)
                {
                    purchaseOrderWorkSheetItem = new PurchaseOrderWorkSheetItem();
                    purchaseOrderWorkSheetItem.ActivityItemRefno = this.Refno;
                    purchaseOrderWorkSheetItem.ProductUnitRefno = this.ProdInfo.UnitRefno;
                    purchaseOrderWorkSheetItem.ProductRefno = this.ProdInfo.Refno;
                    purchaseOrderWorkSheetItem.ProductCode = this.ProdInfo.Code;
                    purchaseOrderWorkSheetItem.ProductDescription = this.ProdInfo.Desc;
                    purchaseOrderWorkSheetItem.ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc;
                    purchaseOrderWorkSheetItem.UnitCode = this.ProdInfo.UnitCode;
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno = this.FKRefno;
                    purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetStringFKRefno = this.FKRefno.ToString();

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
                //this.DataViewItems.Sort = oldSort;

                if (toBeUpdated)
                {
                    this.ListItems.Add(purchaseOrderWorkSheetItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        public override async Task Submit(XDocument data, object selectedActivityItem)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(data);
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
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
                    {
                        purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno = this.FKRefno;
                        purchaseOrderWorkSheetItem.ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityItem SET tActivityItem.ActivityItemStatus = ? WHERE ActivityItemRefno = ?", (int)SysCommon.ActStatus.NoChange, this.Refno);
                }
            }
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

                TableActivityPurchaseOrderWorkSheetItem tableActPOWorkSheetItem = new TableActivityPurchaseOrderWorkSheetItem();
                await tableActPOWorkSheetItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPOWorkSheetItem.RegisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
                await tableActPOWorkSheetItem.Request(ActHdr.Refno);
                tableActPOWorkSheetItem.UnregisterDataAccessListenerProgressOut((POWorkSheetActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in this.ListItems)
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

            // TODO: Resource strings
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
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno AS ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno AS ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
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
                sql.Append("SELECT tActivityItemPurchaseOrderWorkSheet.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno AS ActivityItemPurchaseOrderWorkSheetFKRefno,");
                sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetStringFKRefno AS ActivityItemPurchaseOrderWorkSheetStringFKRefno,");
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

            //using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            //{
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewItems = ds.Tables[0].DefaultView;
            //        this.DataViewItems.Table.TableName = this.MainGridMappingName;
            //        this.DataViewItems.Sort = "ActivityItemPurchaseOrderWorkSheetFKRefno";
            //        this.DataViewItems.AllowNew = true;
            //        this.DataViewItems.AllowEdit = true;
            //        this.DataViewItems.AllowDelete = true;
            //    }
            //}

            this.listPurchaseOrderWorkSheetItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PurchaseOrderWorkSheetItem>(sql.ToString());
            this.ListItems = this.listPurchaseOrderWorkSheetItems.OrderBy(poItem => poItem.ActivityItemPurchaseOrderWorkSheetFKRefno).Cast<object>().ToList();
            return this.ListItems;
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

            //if (string.IsNullOrEmpty(this.ActItemFilter))
            //{
            this.ListItems = this.listPurchaseOrderWorkSheetItems.Where(purchaseOrderWorkSheetItem => purchaseOrderWorkSheetItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            //}
            //else
            //{
            // TODO: Fix filtering
            //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            //}

            foreach (PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem in this.ListItems)
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

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>

        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                await base.SelectActivityItem(purchaseOrderWorkSheetItem);
                this.FKRefno = Convert.ToInt32(purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetFKRefno);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productUnitRefno"></param>
        /// <returns></returns>
        public override async Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = 0;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemPurchaseOrderWorkSheet.ActivityItemPurchaseOrderWorkSheetFKRefno,");
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
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemPurchaseOrderWorkSheetFKRefno);
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
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + prodCode + "</prod_id>")
                + ("<is_bc>" + prodUnitIsBC.ToString() + "</is_bc>")
                + ("<symbology>" + symbologyPrefix + "</symbology>")
                + ("<gtin>" + barCodeCode + "</gtin>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<refno>" + this.FKRefno + "</refno>")
                + ("<itm_refno>" + this.FKRefno + "</itm_refno>")
                + ("<alt_qty>" + data.Root.Element("alt_qty").Value + "</alt_qty>")
                + ("<base_qty>" + data.Root.Element("base_qty").Value + "</base_qty>")
                + ("<wt_qty>" + data.Root.Element("wt_qty").Value + "</wt_qty>")
                + ("<alt_unit>" + this.ProdInfo.UnitCode + "</alt_unit>")
                + ("<base_unit>" + this.ProdInfo.BaseUnitCode + "</base_unit>")
                + ("<wt_unit>" + this.ProdInfo.WeightUnitCode + "</wt_unit>")
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
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered = quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered = baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered = weightQuantity;
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
            if (selectedActivityItem is PurchaseOrderWorkSheetItem purchaseOrderWorkSheetItem)
            {
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetQuantityOrdered += quantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered += baseQuantity;
                purchaseOrderWorkSheetItem.ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
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
                    actItemRefno
                };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        #endregion
    }
}
