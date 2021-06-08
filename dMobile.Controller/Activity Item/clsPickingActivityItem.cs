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
    using dMobile.Resources.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class PickingActItem : ActItem
    {
        double columnWidth0 = 0;
        double columnWidth1 = 0;
        double columnWidth2 = 0;
        double columnWidth3 = 0;
        double columnWidth4 = 0;
        double columnWidth5 = 0;
        double columnWidth6 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();
        List<PickingItem> listPickingItems = new List<PickingItem>();

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public PickingActItem(Activity activity)
          : base(activity)
        {
            this.FullClassName = typeof(PickingActItem).FullName;
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;

            // Data grid
            DataTemplate dataTemplatePicking = new DataTemplate(() =>
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
                        new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) }
                    }
                };

                Label labelActivityItemPickingIsActivityItemPickingComment = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingProductCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingQuantityOrdered = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingQuantityReceived = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingQuantityAvailable = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityItemPickingIsActivityItemPickingComment.SetBinding(Label.TextProperty, new Binding("IsActivityItemPickingComment"));
                labelActivityItemPickingProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelActivityItemPickingProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelActivityItemPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemPickingQuantityOrdered.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemPickingQuantityReceived.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityShipped", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemPickingQuantityAvailable.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityAvailable", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelActivityItemPickingIsActivityItemPickingComment);
                gridData.Children.Add(labelActivityItemPickingProductCode);
                gridData.Children.Add(labelActivityItemPickingProductUnitBC);
                gridData.Children.Add(labelActivityItemPickingUnitCode);
                gridData.Children.Add(labelActivityItemPickingQuantityOrdered);
                gridData.Children.Add(labelActivityItemPickingQuantityReceived);

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    gridData.Children.Add(labelActivityItemPickingQuantityAvailable);
                }

                Grid.SetColumn(labelActivityItemPickingIsActivityItemPickingComment, 0);
                Grid.SetColumn(labelActivityItemPickingProductCode, 1);
                Grid.SetColumn(labelActivityItemPickingProductUnitBC, 2);
                Grid.SetColumn(labelActivityItemPickingUnitCode, 3);
                Grid.SetColumn(labelActivityItemPickingQuantityOrdered, 4);
                Grid.SetColumn(labelActivityItemPickingQuantityReceived, 5);

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    Grid.SetColumn(labelActivityItemPickingQuantityAvailable, 6);
                }

                return new ViewCell { View = gridData };
            });

            resourceDictionary.Add("dataTemplatePicking", dataTemplatePicking);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locMap"></param>
        /// <param name="prodInfo"></param>
        /// <returns></returns>
        public override string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
        {
            string msg = base.GetItemNotFoundMessage(locMap, prodInfo);

            if (this.SysEnv.SysSettings.IsPickOverrideMsg1)
            {
                // TODO: Resource strings
                msg = string.Format("Product scannned, code \'{0}\' bc \'{1}\' unit \'{2}\', is not found on this order for warehouse {3}.", prodInfo.Code, prodInfo.UnitBC, prodInfo.UnitCode, locMap.CurrLocMap.WhsFKRefno);
            }

            return msg;
        }

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
                isCatchAsk = this.SysEnv.SysSettings.IsPickCatchAsk;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsPickingNoCatchAskZeroEnabled;
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

            // Weight tolerance within percentage check.
            if (this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent != 0)
            {
                decimal weight = decimal.Parse(data.Root.Element("wt_qty").Value);
                decimal qty = decimal.Parse(data.Root.Element("alt_qty").Value);
                decimal per = this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent / 100;
                decimal weightUpper = (this.ProdInfo.UnitWeight + (this.ProdInfo.UnitWeight * per)) * qty;
                decimal weightLower = (this.ProdInfo.UnitWeight - (this.ProdInfo.UnitWeight * per)) * qty;

                if (weight < weightLower || weight > weightUpper)
                {
                    // clsPickingSingleAcivityitem is different, it only raises a messagebox as a warning, but continues.
                    // Lekker changed it's mind again and only wanting a warning rather than stop.
                    // TODO: Figure out why raising a MessageOnlyException is not being caught as such, but rather
                    // as a System.Exception.

                    // TODO: Resource strings
                    throw new MessageOnlyException("Item is outside of expected weight.");
                }
            }

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.PickQtyOverWarn != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning && 0 < this.FKRefno)
            {
                try
                {
                    this.PickingModuleQuantityOverCheck(decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyOverWarn)
                    {
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndContinue:
                            // TODO: Resource strings
                            if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No")))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndStop:
                            throw new IsReadyToApplyException(ex.Message, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityDetailFKRefno"></param>
        /// <returns></returns>
        public override SActItemSchemeData GetProdSchemeDataFromDetailFKRefno(int activityDetailFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            SActItemSchemeData itemSchemeData = base.GetProdSchemeDataFromDetailFKRefno(activityDetailFKRefno);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER ITEM FROM DETAIL</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<ser_refno>" + activityDetailFKRefno + "</ser_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            string reqData = SysCommon.SubmitComm(xmlString.ToString());

            if (reqData.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);

                if (reqData.Contains("<message>ERROR</message>"))
                {
                    // TODO: Resource strings
                    throw new Exception("Request picking item refno from detail refno error." + Environment.NewLine + xmlDoc.DocumentElement["detail"].InnerText);
                }
                else
                {
                    SProductSchemeData tempSchemeData = new SProductSchemeData();
                    itemSchemeData.ActItemFKRefno = int.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activityitempickingfkrefno").InnerText);
                    itemSchemeData.ActDetailQuantity = decimal.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailquantity").InnerText);
                    itemSchemeData.UnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/unitcode").InnerText;
                    itemSchemeData.ActDetailBaseQuantity = decimal.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailbasequantity").InnerText);
                    itemSchemeData.BaseUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/baseunitcode").InnerText;
                    itemSchemeData.ActDetailWeightQuantity = decimal.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailweightquantity").InnerText);
                    itemSchemeData.WeightUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/weightunitcode").InnerText;
                    itemSchemeData.LocMapWhsFKRefno = int.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/locationmapwarehousefkrefno").InnerText);
                    itemSchemeData.LocMapSectFKRefno = int.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/locationmapsectionfkrefno").InnerText);
                    itemSchemeData.LocMapSlotFKRefno = int.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/locationmapslotfkrefno").InnerText);
                    tempSchemeData.SingleTradeItemSerialNumber
                        = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
                    tempSchemeData.LogisticUnitBatchLotNumber
                        = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
                    tempSchemeData.LogisticUnitSkidNumber
                        = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
                    tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
                    tempSchemeData.VariantDescription
                        = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantdescription").InnerText;
                    tempSchemeData.VariantNumber = int.Parse(
                        xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);

                    if (!(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitDueDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode(
                        "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
                    }

                    itemSchemeData.ProdSchemeData = tempSchemeData;
                    itemSchemeData.ActDetailExtra.Text1 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra1").InnerText;
                    itemSchemeData.ActDetailExtra.Text2 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra2").InnerText;
                    itemSchemeData.ActDetailExtra.Text3 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra3").InnerText;
                    itemSchemeData.ActDetailExtra.Text4 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra4").InnerText;

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText == string.Empty))
                    {
                        itemSchemeData.ActDetailExtra.DateTime5 = DateTime.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText);
                    }
                }
            }

            return itemSchemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override async Task<string> GetComment(object selectedActivityItem)
        {
            string comment = await base.GetComment(selectedActivityItem);

            if (selectedActivityItem is PickingItem pickingItem)
            {
                List<tActivityItemPickingCommentModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityItemPickingCommentModel>("SELECT * FROM tActivityItemPickingComment WHERE ActivityItemRefno = ?", Convert.ToInt32(pickingItem.ActivityItemRefno));

                if (results != null && results.Count > 0)
                {
                    comment = Convert.ToString(results[0].ActivityItemPickingCommentComment);
                }
            }

            return comment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse sales order item # {0}.", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find sales order item # {0}.", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return "ActivityItemPickingStringFKRefno";
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

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = await this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0 && askToInsertAsNewItem)
                {
                    // TODO: Resource strings
                    if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("New Item", "A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "Yes", "No")))
                    {
                        result = true;
                    }
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    result = true;
                }

                if (result)
                {
                    await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
                    {
                        try
                        {
                            transaction.Execute("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(SysCommon.ActSubTypes.Picking), Convert.ToInt32(SysCommon.ActStatus.NoChange), Convert.ToInt32(this.ActHdr.Refno));

                            this.Refno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tActivityItem"));

                            string query = "INSERT INTO tActivityItemPicking (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemPickingFKRefno, " +
                                "ActivityItemPickingStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityItemPickingQuantityOrdered, " +
                                "ActivityItemPickingBaseQuantityOrdered, " +
                                "ActivityItemPickingWeightQuantityOrdered, " +
                                "ActivityItemPickingQuantityShipped, " +
                                "ActivityItemPickingBaseQuantityShipped, " +
                                "ActivityItemPickingWeightQuantityShipped, " +
                                "ActivityItemPickingCustomerProductUnitCode, " +
                                "ActivityItemPickingDescription, " +
                                "ActivityItemPickingQuantityAvailable, " +
                                "ActivityItemPickingBaseQuantityAvailable, " +
                                "ActivityItemPickingWeightQuantityAvailable, " +
                                "ActivityItemPickingIsReviewed" +
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
                            object[] parameters = new object[]
                            {
                                Convert.ToInt32(this.Refno),
                                Convert.ToInt32(SysCommon.ActSubTypes.Picking),
                                Convert.ToInt32(this.Refno * -1),
                                Convert.ToString(this.Refno * -1),
                                Convert.ToInt32(prodUnitRefno),
                                Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToString(string.Empty),
                                Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(0),
                                Convert.ToInt32(false)
                            };

                            transaction.Execute(query, parameters);
                            transaction.Commit();
                        }
                        catch (SQLiteException ex)
                        {
                            // TODO: Logging
                            Console.WriteLine(ex);
                            transaction.Rollback();
                        }
                    });

                    this.FKRefno = this.Refno * -1;
                    this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                    this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                    if (this.ListItems != null)
                    {
                        this.ListItems.Add(new PickingItem()
                        {
                            ActivityItemRefno = this.Refno,
                            ActivityItemPickingFKRefno = this.FKRefno,
                            ActivityItemPickingStringFKRefno = this.FKRefno.ToString(),
                            ProductUnitRefno = prodUnitRefno,
                            ProductRefno = this.ProdInfo.Refno,
                            ProductCode = this.ProdInfo.Code,
                            ProductDescription = this.ProdInfo.Desc,
                            UnitCode = this.ProdInfo.UnitCode,
                            ProductUnitBC = this.ProdInfo.UnitBC,
                            ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc,
                            LocationMapRefno = this.LocMap.CurrLocMap.WhsLocMapRefno,
                            LocationCode = this.LocMap.CurrLocMap.WhsCode,
                            LocationName = this.LocMap.CurrLocMap.WhsName,
                            ActivityItemPickingCustomerProductUnitCode = string.Empty,
                            ActivityItemStatus = Convert.ToInt32(SysCommon.ActStatus.Inserted),
                            IsActivityItemPickingComment = string.Empty,
                            ActivityItemPickingQuantityOrdered = 0,
                            ActivityItemPickingBaseQuantityOrdered = 0,
                            ActivityItemPickingWeightQuantityOrdered = 0,
                            ActivityItemPickingQuantityShipped = 0,
                            ActivityItemPickingBaseQuantityShipped = 0,
                            ActivityItemPickingWeightQuantityShipped = 0,
                            ActivityItemPickingQuantityAvailable = 0,
                            ActivityItemPickingBaseQuantityAvailable = 0,
                            ActivityItemPickingWeightQuantityAvailable = 0,
                        });
                    }
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

                TableActivityPickingItem tableActPickingItem = new TableActivityPickingItem();
                TableActivityPickingDetail tableActPickingDetail = new TableActivityPickingDetail();
                await tableActPickingDetail.Purge();
                await tableActPickingItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPickingItem.RegisterDataAccessListenerProgressOut((PickingActItem)this);
                await tableActPickingItem.RequestData(this.ActHdr.Refno, data);
                tableActPickingItem.UnregisterDataAccessListenerProgressOut((PickingActItem)this);
            }
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // TODO: Resource strings
            Label labelIsActivityItemPickingCommentHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingProductUnitBCHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingQuantityOrderedHeader = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingQuantityReceivedHeader = new Label { Text = "Shipped", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingQuantityAvailableHeader = new Label { Text = "Available", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelIsActivityItemPickingCommentHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductCodeHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingProductUnitBCHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingUnitCodeHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityOrderedHeader.Text);
            columnWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityReceivedHeader.Text);
            columnWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(labelActivityItemPickingQuantityAvailableHeader.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;
            double detailWidth5 = 0;
            double detailWidth6 = 0;

            // Header grid
            Grid gridHeaderPicking = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth5, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth6, GridUnitType.Absolute) }
                }
            };

            gridHeaderPicking.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeaderPicking.Children.Add(labelIsActivityItemPickingCommentHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductUnitBCHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingUnitCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityOrderedHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityReceivedHeader);

            if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
            {
                gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityAvailableHeader);
            }

            Grid.SetColumn(labelIsActivityItemPickingCommentHeader, 0);
            Grid.SetColumn(labelActivityItemPickingProductCodeHeader, 1);
            Grid.SetColumn(labelActivityItemPickingProductUnitBCHeader, 2);
            Grid.SetColumn(labelActivityItemPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelActivityItemPickingQuantityOrderedHeader, 4);
            Grid.SetColumn(labelActivityItemPickingQuantityReceivedHeader, 5);

            if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
            {
                Grid.SetColumn(labelActivityItemPickingQuantityAvailableHeader, 6);
            }

            Grid.SetRow(gridHeaderPicking, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderPicking);

            foreach (PickingItem pickingItem in this.ListItems)
            {
                //PickingItem pickingItem = new PickingItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                //    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                //    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                //    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                //    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                //    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                //    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                //    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                //    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                //    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                //};

                observableCollection.Add(pickingItem);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.IsActivityItemPickingComment);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.ProductCode);
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.ProductUnitBC);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingItem.UnitCode);
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", pickingItem.ActivityItemPickingQuantityOrdered));
                detailWidth5 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", pickingItem.ActivityItemPickingWeightQuantityShipped));
                detailWidth6 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", pickingItem.ActivityItemPickingQuantityAvailable));

                if (detailWidth0 > columnWidth0)
                {
                    columnWidth0 = detailWidth0;
                    gridHeaderPicking.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeaderPicking.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeaderPicking.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeaderPicking.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeaderPicking.ColumnDefinitions[4].Width = columnWidth4;
                }

                if (detailWidth5 > columnWidth5)
                {
                    columnWidth5 = detailWidth5;
                    gridHeaderPicking.ColumnDefinitions[5].Width = columnWidth5;
                }

                if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                {
                    if (detailWidth6 > columnWidth6)
                    {
                        columnWidth6 = detailWidth6;
                        gridHeaderPicking.ColumnDefinitions[6].Width = columnWidth6;
                    }
                }
                else
                {
                    columnWidth6 = 0;
                    gridHeaderPicking.ColumnDefinitions[6].Width = columnWidth6;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplatePicking"];

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
                // Gather any newly added line items.
                // Also perform union into tActivityDetail to check of detail exists.
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno AS ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno AS ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", ActHdr.Refno);
                sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno AS ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno AS ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
                sql.Append("= tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);
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
            //        TODO: Fix sorting
            //        this.DataViewItems.Sort = "ActivityItemRefno";
            //    }
            //}

            this.listPickingItems = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PickingItem>(sql.ToString());
            this.ListItems = this.listPickingItems.Cast<object>().ToList();
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
            this.ListItems = this.listPickingItems.Where(pickingItem => pickingItem.ProductCode.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();
            //}
            //else
            //{
            // TODO: Fix filtering
            //this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            //}

            foreach (PickingItem pickingItem in this.ListItems)
            {
                //PickingItem pickingItem = new PickingItem
                //{
                //    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                //    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                //    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                //    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                //    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                //    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                //    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                //    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                //    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                //    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                //    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                //    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                //    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                //};

                observableCollection.Add(pickingItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override async Task SelectActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                await base.SelectActivityItem(pickingItem);
                this.SelectedActivityItem = pickingItem;
                this.FKRefno = Convert.ToInt32(pickingItem.ActivityItemPickingFKRefno);
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

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tActivityItem.ActivityItemRefno,");
            sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
            sql.Append("tActivityItemPicking.ProductUnitRefno,");
            sql.Append("tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("FROM tActivityItemPicking ");
            sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
            sql.Append("= tActivityItemPicking.ActivityItemRefno ");
            sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
            sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
            sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
            sql.AppendFormat("AND tActivityItemPicking.WarehouseLocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
            sql.AppendFormat("AND tActivityItemPicking.ProductUnitRefno = {0}", prodUnitRefno);

            List<PickingItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PickingItem>(sql.ToString());

            if (results != null && results.Count > 0)
            {
                this.FKRefno = Convert.ToInt32(results[0].ActivityItemPickingFKRefno);
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(results[0].ProductUnitRefno));
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                this.Refno = Convert.ToInt32(results[0].ActivityItemRefno);
                foundActItemRefno = this.Refno;
            }

            return foundActItemRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string sendReq = this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_data>" + this.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_data>"
                    + "</params>"));

                string reqData = SysCommon.SubmitComm(sendReq);

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail);
                        }
                        else
                        {
                            throw new Exception(errorDetail);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = null;

            if (data.Document.Root.Element("skid_data") == null)
            {
                // TODO: Resource strings
                throw new NotImplementedException("Only skid deletion is supported.");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE SALES ORDER DETAIL SKID</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<refno>" + this.ActHdr.FKRefno + "</refno>"
                    + "<itm_refno>" + this.FKRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
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
                + "<action>SUBMIT PICKING ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
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
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                pickingItem.ActivityItemPickingQuantityShipped = quantity;
                pickingItem.ActivityItemPickingBaseQuantityShipped = baseQuantity;
                pickingItem.ActivityItemPickingWeightQuantityShipped = weightQuantity;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iRowIdx"></param>
        /// <param name="decQuantity"></param>
        /// <param name="decBaseQuantity"></param>
        /// <param name="decWeightQuantity"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                pickingItem.ActivityItemPickingQuantityShipped += quantity;
                pickingItem.ActivityItemPickingBaseQuantityShipped += baseQuantity;
                pickingItem.ActivityItemPickingWeightQuantityShipped += weightQuantity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="activityItemRefno"></param>
        public override async Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            if (selectedActivityItem is PickingItem pickingItem)
            {
                string query = "UPDATE tActivityItemPicking SET ActivityItemPickingQuantityShipped = ?, ActivityItemPickingBaseQuantityShipped = ?, ActivityItemPickingWeightQuantityShipped = ? WHERE tActivityItemPicking.ActivityItemRefno = ?";
                object[] parameters = new object[] { pickingItem.ActivityItemPickingQuantityShipped, pickingItem.ActivityItemPickingBaseQuantityShipped, pickingItem.ActivityItemPickingWeightQuantityShipped, actItemRefno };

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, parameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actFKRefno"></param>
        /// <returns></returns>
        public string CreateRequestTotalQuantityXML(int actFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST PICKING TOTAL QUANTITY</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + actFKRefno + "</refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task<string> GetTotalQuantity()
        {
            string msg = string.Empty;

            try
            {
                string query = "SELECT SUM(tActivityItemPicking.ActivityItemPickingQuantityOrdered) AS Ordered,"
                        + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered) AS WeightOrdered,"
                        + "SUM(tActivityItemPicking.ActivityItemPickingQuantityShipped) AS Shipped,"
                        + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityShipped) AS WeightShipped,"
                        + "tUnit.UnitCode AS WtUnitCode "
                        + "FROM tActivityItem "
                        + "INNER JOIN tActivityItemPicking ON tActivityItemPicking.ActivityItemRefno = "
                        + "tActivityItem.ActivityItemRefno "
                        + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno "
                        + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.WtUnitRefno "
                        + string.Format("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno)
                        + "GROUP BY tUnit.UnitCode";

                List<PickingItem> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<PickingItem>(query);

                if (results != null)
                {
                    foreach (dynamic pickingItem in results)
                    {
                        // Because an agregate sum function is used in a query. There will always be a result record.
                        // This causes problems detecting if there is nothing to sum, resulting in null.
                        // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
                        // TODO: Resource strings
                        msg += string.Format("Total ordered:\r{0} @ {1}{2}\r\rTotal shipped:\r{3} @ {4}{5}",
                            Convert.ToInt32(Convert.ToDecimal(pickingItem.Ordered)),
                            Convert.ToDecimal(pickingItem.WeightOrdered),
                            Convert.ToString(pickingItem.WtUnitCode),
                            Convert.ToInt32(Convert.ToDecimal(pickingItem.Shipped)),
                            Convert.ToDecimal(pickingItem.WeightShipped),
                            Convert.ToString(pickingItem.WtUnitCode));
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred.", ex);
            }

            return msg;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemPickQuantity"></param>
        private void PickingModuleQuantityOverCheck(decimal actItemPickQuantity)
        {
            if (this.SelectedActivityItem is PickingItem pickingItem)
            {
                decimal shipped = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityShipped);
                decimal ordered = Convert.ToDecimal(pickingItem.ActivityItemPickingQuantityOrdered);

                if (shipped + actItemPickQuantity > ordered)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("The resulting quantity shipped {0} {1} will be greater than the quantity ordered {2} {3}.", shipped + actItemPickQuantity, this.ProdInfo.UnitCode, ordered, this.ProdInfo.UnitCode));
                }
            }
        }

        #endregion
    }
}
