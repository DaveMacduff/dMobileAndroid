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
    using Microsoft.VisualBasic;
    using Xamarin.Forms;

    public class PickSingleActItem : ActItem
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private double columnWidth5 = 0;
        private double columnWidth6 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_oActivity"></param>
        /// <remarks></remarks>
        public PickSingleActItem(Activity activity)
          : base(activity)
        {
            this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
            this.FullClassName = typeof(PickSingleActItem).FullName;
            this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;

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

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override SysCommon.SLabelQuantity GetLabelQuantity(bool isRepackQuantityOnly)
        {
            SysCommon.SLabelQuantity labelQty = new SysCommon.SLabelQuantity();

            try
            {
                string sendReq = string.Empty;
                string reqData = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                XDocument request = null;

                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SHIPPING LABEL ITEM QUANTITY</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<itm_refno>" + this.FKRefno + "</itm_refno>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                sendReq = xmlString.ToString();

                reqData = SysCommon.SubmitComm(sendReq);
                if (reqData.Length > 0)
                {
                    XmlDocument xmlResult = new XmlDocument();
                    xmlResult.LoadXml(reqData);

                    // Check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        string errorDetail = string.Format(

                // TODO: Resource strings
                "Order# {0} request shipping label(s) quantity failed." + Environment.NewLine + "{1}",

                this.FKRefno,
                            xmlResult.DocumentElement["detail"].InnerText);
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
                        if (!isRepackQuantityOnly)
                        {
                            labelQty.FullQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/FullCaseQuantity").FirstChild.Value);

                            labelQty.DryQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/DryQuantity").FirstChild.Value);

                            labelQty.FrozenQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/FrozenQuantity").FirstChild.Value);

                            labelQty.LiveQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/LiveQuantity").FirstChild.Value);

                            labelQty.FreshQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/FreshQuantity").FirstChild.Value);

                            labelQty.BundleQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/BundleQuantity").FirstChild.Value);

                            labelQty.PalletQuantity = int.Parse(
                                xmlResult.SelectSingleNode("/VFPData/PalletQuantity").FirstChild.Value);
                        }

                        labelQty.RepackQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/RepackQuantity").FirstChild.Value);

                        labelQty.RepackDryQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/RepackDryQuantity").FirstChild.Value);

                        labelQty.RepackFrozenQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/RepackFrozenQuantity").FirstChild.Value);

                        labelQty.RepackLiveQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/RepackLiveQuantity").FirstChild.Value);

                        labelQty.RepackFreshQuantity = int.Parse(
                            xmlResult.SelectSingleNode("/VFPData/RepackFreshQuantity").FirstChild.Value);

                        // Used to determine if there should be a repack quantity gathered.
                        labelQty.IsRepack = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackCase").FirstChild.Value);
                        labelQty.IsRepackLive = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackLive").FirstChild.Value);
                        labelQty.IsRepackFrozen = bool.Parse(
                              xmlResult.SelectSingleNode("/VFPData/IsRepackFrozen").FirstChild.Value);
                        labelQty.IsRepackFresh = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackFresh").FirstChild.Value);
                        labelQty.IsRepackDry = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackDry").FirstChild.Value);
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Order # {0} item request shipping label(s) quantity failed.", this.FKRefno));
                }
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(this.ActHdr.GetControlDescription(), string.Format("Get full unit label quantity failed." + Environment.NewLine + " {0}", ex.Message), "OK"));
            }

            return labelQty;
        }

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
                isCatchAsk = this.SysEnv.SysSettings.IsPickSingleCatchAsk;
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
                isCatchAskZero = !this.SysEnv.SysSettings.IsPickingSingleModuleNoCatchAskZeroEnabled;
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
                    // TODO: Resource strings
                    string msg = "Item is outside of expected weight.";
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("Weight Range Check", msg, "OK"));
                }
            }

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.PickQtyOverWarn != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning && 0 < this.FKRefno)
            {
                try
                {
                    this.PickingModuleQuantityOverCheck(int.Parse(data.Root.Element("itm_refno").Value), decimal.Parse(data.Root.Element("alt_qty").Value));
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
        /// <param name="actDetailFKRefno"></param>
        /// <returns></returns>
        public override SActItemSchemeData GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            SActItemSchemeData itemSchemeData = base.GetProdSchemeDataFromDetailFKRefno(actDetailFKRefno);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER ITEM FROM DETAIL</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<ser_refno>" + actDetailFKRefno + "</ser_refno>")
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
                    SProductSchemeData tempSchemeData = new SProductSchemeData();
                    itemSchemeData.ActItemFKRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activityitempickingfkrefno").InnerText);
                    itemSchemeData.ActDetailQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailquantity").InnerText);
                    itemSchemeData.UnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/unitcode").InnerText;
                    itemSchemeData.ActDetailBaseQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailbasequantity").InnerText);
                    itemSchemeData.BaseUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/baseunitcode").InnerText;
                    itemSchemeData.ActDetailWeightQuantity = decimal.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailweightquantity").InnerText);
                    itemSchemeData.WeightUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/weightunitcode").InnerText;
                    itemSchemeData.LocMapWhsFKRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapwarehousefkrefno").InnerText);
                    itemSchemeData.LocMapSectFKRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapsectionfkrefno").InnerText);
                    itemSchemeData.LocMapSlotFKRefno = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/locationmapslotfkrefno").InnerText);

                    tempSchemeData.SingleTradeItemSerialNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
                    tempSchemeData.LogisticUnitBatchLotNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
                    tempSchemeData.LogisticUnitSkidNumber = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
                    tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
                    tempSchemeData.VariantDescription = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantdescription").InnerText;
                    tempSchemeData.VariantNumber = int.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText
                        == string.Empty))
                    {
                        tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText
                        == string.Empty))
                    {
                        tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                            xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
                    {
                        tempSchemeData.LogisticUnitDueDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                            "/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
                    }

                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText
                        == string.Empty))
                    {
                        tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                            "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
                    }

                    itemSchemeData.ProdSchemeData = tempSchemeData;
                }
            }

            return itemSchemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override string GetComment(int idx)
        {
            string itemCmnt = base.GetComment(idx);

            using (SQLiteCommand cmndItemPickCmnt = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemPickCmnt.CommandType = CommandType.Text;
                cmndItemPickCmnt.CommandText = "SELECT * FROM tActivityItemPickingComment WHERE ActivityItemRefno = @activityItemRefno";
                //cmndItemPickCmnt.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";
                cmndItemPickCmnt.Parameters.Add(new SQLiteParameter("@activityItemRefno", idx));

                using (SQLiteDataReader drItemPickCmnt = cmndItemPickCmnt.ExecuteReader())
                {
                    //if (drItemPickCmnt.Seek(DbSeekOptions.FirstEqual, new object[] { idx }))
                    //{
                    if (drItemPickCmnt.Read())
                    {
                        itemCmnt = drItemPickCmnt["ActivityItemPickingCommentComment"].ToString();
                    }
                    //}
                }
            }

            return itemCmnt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse sales order item # {0}", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find sales order item # {0}", actItemStringFKRefno);
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
        /// <remarks></remarks>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pbProgressBar"></param>
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

                TableActivityPickingItem tableActPickItem = new TableActivityPickingItem();
                TableActivityPickingDetail tableActPickDetail = new TableActivityPickingDetail();
                tableActPickDetail.Purge();
                tableActPickItem.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPickItem.RegisterDataAccessListenerProgressOut((PickSingleActItem)this);
                tableActPickItem.RequestData(ActHdr.Refno, data);
                tableActPickItem.UnregisterDataAccessListenerProgressOut((PickSingleActItem)this);
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

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                PickingItem pickingItem = new PickingItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

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

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <param name="dataGridMain"></param>
        //    /// <remarks></remarks>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      int columnWidthFactor = 1;

        //      if (this.SysEnv.IsWindowsMobile6Device)
        //      {
        //        columnWidthFactor = 2;
        //      }

        //      if (dataGridMain.TableStyles.Count > 0)
        //      {
        //        dataGridMain.TableStyles.Clear();
        //      }

        //      DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();
        //      dataGridTableStyle.MappingName = this.MainGridMappingName;

        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemRefno";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = -1;
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "IsActivityItemPickingComment";
        //      dataGridCol.HeaderText = "C";
        //      dataGridCol.Width = 12 * columnWidthFactor;
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductCode";
        //      dataGridCol.HeaderText = "Product ID";
        //      dataGridCol.Width = 65 * columnWidthFactor;
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ProductUnitBC";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = 20 * columnWidthFactor;
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "UnitCode";
        //      dataGridCol.HeaderText = string.Empty;
        //      dataGridCol.Width = 22 * columnWidthFactor;
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPickingQuantityOrdered";
        //      dataGridCol.HeaderText = "Ordered";
        //      dataGridCol.Width = 52 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityItemPickingQuantityShipped";
        //      dataGridCol.HeaderText = "Shipped";
        //      dataGridCol.Width = 52 * columnWidthFactor;
        //      dataGridCol.Format = "0.000";
        //      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

        //      if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
        //      {
        //        dataGridCol = new DataGridTextBoxColumn();
        //        dataGridCol.MappingName = "ActivityItemPickingQuantityAvailable";
        //        dataGridCol.HeaderText = "Available";
        //        dataGridCol.Width = 52 * columnWidthFactor;
        //        dataGridTableStyle.GridColumnStyles.Add(dataGridCol);
        //      }

        //      ////dataGridCol.MappingName = string.Empty
        //      ////dataGridCol.HeaderText = "R"
        //      ////dataGridCol.Width = 12 * colWidthFactor
        //      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

        //      dataGridMain.TableStyles.Add(dataGridTableStyle);
        //      dataGridMain.RowHeadersVisible = false;
        //      dataGridMain.ColumnHeadersVisible = true;
        //    }

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
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno AS ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno AS ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
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
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
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
                sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
                sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed AS ActivityItemPickingIsReviewed ");
                sql.Append("FROM tActivityItemPicking ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
                sql.Append("tActivityItemPicking.ActivityItemRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
                sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
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
        /// <remarks></remarks>
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
                PickingItem pickingItem = new PickingItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(pickingItem);
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
            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemPickingFKRefno"]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            int foundActItemRefno = this.Refno;

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
                + "<action>SUBMIT PICKING ITEM</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + prodUnitIsBC + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
                + "<refno></refno>" + "</imclient>");

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
        public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] = quantity;
            this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] = baseQuantity;
            this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] = weightQuantity;
            this.DataViewItems[idx].EndEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            this.DataViewItems[idx].BeginEdit();
            this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"]) + quantity;
            this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"]) + baseQuantity;
            this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"]) + weightQuantity;
            this.DataViewItems[idx].EndEdit();

            return idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItem.CommandText = "tActivityItemPicking";
                cmndItem.CommandType = CommandType.Text;
                //cmndItem.IndexName = string.Empty;
                cmndItem.CommandText = "UPDATE tActivityItemPicking "
                    + "SET ActivityItemPickingQuantityShipped = "
                    + this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] + ","
                    + " ActivityItemPickingBaseQuantityShipped = "
                    + this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] + "," +
                    " ActivityItemPickingWeightQuantityShipped = "
                    + this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] +
                    " WHERE ActivityItemRefno = " + actItemRefno;
                cmndItem.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemPickFKRefno"></param>
        /// <param name="actItemPickQuantity"></param>
        private void PickingModuleQuantityOverCheck(int actItemPickFKRefno, decimal actItemPickQuantity)
        {
            ////if (0 < skidFKRefno)
            ////{
            ////  StringBuilder xmlString = new StringBuilder();
            ////  XmlWriter xml = XmlWriter.Create(xmlString);

            ////  // Sending the alt_qty is not used. Only the skid_no is used.
            ////  // Kept the alt_qty
            ////  XDocument request = XDocument.Parse(
            ////      "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
            ////      + "<imclient>"
            ////      + "<command>"
            ////      + "<action>REQUEST SALES ORDER ITEM QUANTITY OVER CHECK</action>"
            ////      + SysCommon.GetRequestDefaults(false)
            ////      + ("<itm_refno>" + actItemPickFKRefno + "</itm_refno>")
            ////      + ("<alt_qty>" + actItemPickQuantity + "</alt_qty>")
            ////      + "</command>"
            ////      + "</imclient>");

            ////  request.Save(xml);
            ////  xml.Flush();
            ////  xml.Close();

            ////  string reqData = SysCommon.SubmitComm(request.ToString());

            ////}

            if (this.CurrRowIdx >= 0 && this.CurrRowIdx < this.DataViewItems.Count)
            {
                decimal shipped = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityShipped"]);
                decimal ordered = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityOrdered"]);

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
