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
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    public class PickingActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public PickingActivity()
          : base()
        {
            this.FullClassName = typeof(PickingActivity).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void PrintLabel(XDocument data)
        {
            int locMapRefno = 0;
            SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value), int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value), 0, ref locMapRefno);
            string reqData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = null;
            PrinterSettings prnSettings = new PrinterSettings().LoadData();

            // Don't even bother sending in a print request if no quantity was entered.
            SysCommon.SLabelQuantity labelQty = new SysCommon.SLabelQuantity();
            labelQty.FullQuantity = int.Parse(data.Root.Element("full_alt_qty").Value);
            labelQty.RepackQuantity = int.Parse(data.Root.Element("repack_alt_qty").Value);
            labelQty.FreshQuantity = int.Parse(data.Root.Element("fresh_alt_qty").Value);
            labelQty.FrozenQuantity = int.Parse(data.Root.Element("frozen_alt_qty").Value);
            labelQty.LiveQuantity = int.Parse(data.Root.Element("live_alt_qty").Value);
            labelQty.DryQuantity = int.Parse(data.Root.Element("dry_alt_qty").Value);
            labelQty.RepackFreshQuantity = int.Parse(data.Root.Element("repack_fresh_alt_qty").Value);
            labelQty.RepackFrozenQuantity = int.Parse(data.Root.Element("repack_frozen_alt_qty").Value);
            labelQty.RepackLiveQuantity = int.Parse(data.Root.Element("repack_live_alt_qty").Value);
            labelQty.RepackDryQuantity = int.Parse(data.Root.Element("repack_dry_alt_qty").Value);
            labelQty.PalletQuantity = int.Parse(data.Root.Element("pallet_alt_qty").Value);
            labelQty.BundleQuantity = int.Parse(data.Root.Element("bundle_alt_qty").Value);

            // Used abs for handling the freaky event that a negative label quantity was requested.
            if (0 < labelQty.FullQuantity + labelQty.RepackQuantity + labelQty.FreshQuantity + labelQty.FrozenQuantity
                + labelQty.LiveQuantity + labelQty.DryQuantity + labelQty.RepackFreshQuantity + labelQty.RepackFrozenQuantity
                + labelQty.RepackLiveQuantity + labelQty.RepackDryQuantity + labelQty.PalletQuantity
                + labelQty.BundleQuantity)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT PRINT SHIPPING LABEL</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno + "</refno>")
                    + ("<itm_refno>" + data.Root.Element("itm_refno").Value + "</itm_refno>")
                    + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                    + ("<full_alt_qty>" + data.Root.Element("full_alt_qty").Value + "</full_alt_qty>")
                    + ("<repack_alt_qty>" + data.Root.Element("repack_alt_qty").Value + "</repack_alt_qty>")
                    + ("<fresh_alt_qty>" + data.Root.Element("fresh_alt_qty").Value + "</fresh_alt_qty>")
                    + ("<frozen_alt_qty>" + data.Root.Element("frozen_alt_qty").Value + "</frozen_alt_qty>")
                    + ("<live_alt_qty>" + data.Root.Element("live_alt_qty").Value + "</live_alt_qty>")
                    + ("<dry_alt_qty>" + data.Root.Element("dry_alt_qty").Value + "</dry_alt_qty>")
                    + ("<repack_fresh_alt_qty>" + data.Root.Element("repack_fresh_alt_qty").Value + "</repack_fresh_alt_qty>")
                    + ("<repack_frozen_alt_qty>" + data.Root.Element("repack_frozen_alt_qty").Value + "</repack_frozen_alt_qty>")
                    + ("<repack_live_alt_qty>" + data.Root.Element("repack_live_alt_qty").Value + "</repack_live_alt_qty>")
                    + ("<repack_dry_alt_qty>" + data.Root.Element("repack_dry_alt_qty").Value + "</repack_dry_alt_qty>")
                    + ("<pallet_alt_qty>" + data.Root.Element("pallet_alt_qty").Value + "</pallet_alt_qty>")
                    + ("<bundle_alt_qty>" + data.Root.Element("bundle_alt_qty").Value + "</bundle_alt_qty>")
                    + ("<total_alt_qty>" + data.Root.Element("total_alt_qty").Value + "</total_alt_qty>")
                    + ("<mobile_printer_name>"
                    + (prnSettings.IsEnabledShippingLabelPrinter ? prnSettings.ShippingLabelPrinterName : string.Empty)
                    + "</mobile_printer_name>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                reqData = SysCommon.SubmitComm(xmlString.ToString());
                
                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        // TODO: Resource Strings
                        throw new MessageOnlyException(string.Format("Order# {0} print shipping label(s) failed." + Environment.NewLine + "{1}", this.FKRefno, docError.DocumentElement["detail"].InnerText));
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Order# {0} Print shipping label(s) failed.", this.FKRefno));
                }
            }
        }

        /// <summary>
        /// Request from the server the number of suggested shipping labels to print.
        /// </summary>
        /// <returns>Structure containing of number of labels to print</returns>
        /// <remarks></remarks>
        public override SysCommon.SLabelQuantity GetLabelQuantity(bool isRepackQuantityOnly)
        {
            SysCommon.SLabelQuantity labelQty = new SysCommon.SLabelQuantity();

            try
            {
                int locMapRefno = 0;
                string sendReq = string.Empty;
                string reqData = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                XDocument request = null;
                SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value), int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value), 0, ref locMapRefno);

                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SHIPPING LABEL QUANTITY</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno + "</refno>")
                    + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
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
                        // TODO: Resource strings
                        throw new Exception(string.Format("Order# {0} request shipping label(s) quantity failed." + Environment.NewLine + "{1}", this.FKRefno, xmlResult.DocumentElement["detail"].InnerText));
                    }
                    else
                    {
                        if (!isRepackQuantityOnly)
                        {
                            labelQty.FullQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/FullCaseQuantity").FirstChild.Value);
                            labelQty.DryQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/DryQuantity").FirstChild.Value);
                            labelQty.FrozenQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/FrozenQuantity").FirstChild.Value);
                            labelQty.LiveQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/LiveQuantity").FirstChild.Value);
                            labelQty.FreshQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/FreshQuantity").FirstChild.Value);
                            labelQty.BundleQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/BundleQuantity").FirstChild.Value);
                            labelQty.PalletQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/PalletQuantity").FirstChild.Value);
                        }

                        labelQty.RepackQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/RepackQuantity").FirstChild.Value);
                        labelQty.RepackDryQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/RepackDryQuantity").FirstChild.Value);
                        labelQty.RepackFrozenQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/RepackFrozenQuantity").FirstChild.Value);
                        labelQty.RepackLiveQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/RepackLiveQuantity").FirstChild.Value);
                        labelQty.RepackFreshQuantity = int.Parse(xmlResult.SelectSingleNode("/VFPData/RepackFreshQuantity").FirstChild.Value);

                        // Used to determine if there should be a repack quantity gathered.
                        labelQty.IsRepack = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackCase").FirstChild.Value);
                        labelQty.IsRepackLive = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackLive").FirstChild.Value);
                        labelQty.IsRepackFrozen = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackFrozen").FirstChild.Value);
                        labelQty.IsRepackFresh = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackFresh").FirstChild.Value);
                        labelQty.IsRepackDry = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackDry").FirstChild.Value);
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Order# {0} request shipping label(s) quantity failed.", this.FKRefno));
                }
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine(ex);

                // TODO: Resource strings
                Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), "Get full unit label quantity failed." + Environment.NewLine + ex.Message, "OK");
            }

            return labelQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public async Task<bool> IsSalesOrderBeingPicked()
        {
            int locMapRefno = 0;
            SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value),
                int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value),
                0,
                ref locMapRefno);
            string reqData = string.Empty;
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            XDocument request = null;

            request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SALES ORDER IS BEING PICKED</action>"
                + SysCommon.GetRequestDefaults(false)
                + ("<refno>" + this.FKRefno.ToString() + "</refno>")
                + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            reqData = SysCommon.SubmitComm(xmlString.ToString());

            bool isBeingPicked = false;
            if (reqData.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);
                
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    isBeingPicked = true;
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                    
                    // TODO: Resource strings
                    await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), errorDetail, "OK");
                }
            }

            return isBeingPicked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearDeviceOnly"></param>
        /// <returns></returns>
        public async Task<bool> ClearSalesOrderBeingPicked(bool clearDeviceOnly)
        {
            bool isClearedBeingPicked = true;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int locMapRefno = 0;
                SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                    int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value),
                    int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value),
                    0,
                    ref locMapRefno);
                string reqData = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                XDocument request = null;

                isClearedBeingPicked = false;
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT SALES ORDER CLEAR BEING PICKED</action>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + FKRefno.ToString() + "</refno>")
                    + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                    + ("<device_only>" + clearDeviceOnly + "</device_only>")
                    + "</command>"
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                reqData = SysCommon.SubmitComm(xmlString.ToString());
                
                if (reqData.Length > 0)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reqData);
                    
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                        
                        // TODO: Resource strings
                        await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), errorDetail, "OK");
                    }
                    else
                    {
                        isClearedBeingPicked = true;
                    }
                }
            }

            return isClearedBeingPicked;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public async Task<bool> ReleaseByOrder()
        {
            bool isReleased = false;

            // TODO: Resource Strings
            if (await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), string.Format("Release order# {0} ?", this.FKRefno), "Yes", "No"))
            {
                string reqResult = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);

                XDocument request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>RELEASE SALES ORDER BY ORDER</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno.ToString() + "</refno>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                reqResult = SysCommon.SubmitComm(xmlString.ToString());
                
                if (reqResult.Length > 0)
                {
                    if (reqResult.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqResult);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;
                            
                        // TODO: Resource strings
                        throw new Exception(string.Format("Release order# {0} failed." + Environment.NewLine + errorDetail, this.FKRefno, null));
                    }

                    isReleased = true;
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Release order# {0} failed.", FKRefno));
                }
            }

            return isReleased;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public void PickingModuleQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int rowsAffected = 0;

                using (SQLiteCommand cmndItemPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndItemPicking.CommandType = CommandType.Text;
                    //cmndItemPicking.IndexName = string.Empty;
                    cmndItemPicking.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) "
                        + "FROM tActivityItem "
                        + "INNER JOIN tActivityItemPicking ON tActivityItemPicking.ActivityItemRefno "
                        + "= tActivityItem.ActivityItemRefno "
                        + "WHERE tActivityItem.ActivityRefno = " + this.Refno + " "
                        + "AND 0 < tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable "
                        + "AND tActivityItemPicking.ActivityItemPickingQuantityShipped "
                        + "< tActivityItemPicking.ActivityItemPickingQuantityOrdered";

                    rowsAffected = Convert.ToInt32(cmndItemPicking.ExecuteScalar());
                }

                // Do not warn if product is out of stock WO# 12054
                if (rowsAffected > 0)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Order# {0} quantity under check failed." + Environment.NewLine + "{1} Items exist where the quantity shipped is less than the quantity ordered.", this.FKRefno, rowsAffected));
                }
            }
            else
            {
                string reqResult = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                LocationMap locMap = LocationMap.GetInstance();

                XDocument request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>REQUEST SALES ORDER QUANTITY UNDER CHECK</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno + "</refno>")
                    + ("<whs_id>" + locMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + locMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                reqResult = SysCommon.SubmitComm(xmlString.ToString());
                
                if (reqResult.Length > 0)
                {
                    if (reqResult.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqResult);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;
                        
                        // TODO: Resource strings
                        throw new Exception(string.Format("Order# {0} quantity under check failed." + Environment.NewLine + "{1}", this.FKRefno, errorDetail));
                    }
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Order# {0} quantity under check failed.", FKRefno));
                }
            }
        }

        /// <summary>
        /// Check if all items reviewed.
        /// Perform under quantity check for ALL items.
        /// Releases by Section or Order.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override async Task<bool> ChangeStatus(int idx)
        {
            bool isResult = true;

            if (isResult)
            {
                if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel && idx >= 0)
                {
                    isResult = Convert.ToBoolean(this.DataViewHdrs[idx]["ActivityPickingIsReviewed"]);
                    if (this.SysEnv.SysSettings.IsPrePickCrit
                        && this.SysEnv.SysSettings.IsRelBySect
                        && !this.SysEnv.SysSettings.IsDisconnectedMode
                        && this.XMLDataCriteria != null
                        && int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value) != 0)
                    {
                        if (!isResult)
                        {
                            // TODO: Resource strings
                            throw new Exception("Items that have not been reviewed exist for this section.");
                        }
                    }
                    else if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        if (!isResult)
                        {
                            // TODO: Resource strings
                            throw new Exception("Items that have not been reviewed exist for this order.");
                        }
                    }
                }
            }

            if (isResult)
            {
                if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
                {
                    try
                    {
                        this.PickingModuleQuantityUnderCheck();
                    }
                    catch (Exception ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                // TODO: Resource strings
                                if (await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                                {
                                    isResult = true;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                // TODO: Resource strings
                                await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), ex.Message, "OK");
                                isResult = false;
                                break;
                        }
                    }
                }
            }

            if (isResult)
            {
                if (this.SysEnv.SysSettings.IsPrePickCrit
                    && this.SysEnv.SysSettings.IsRelBySect
                    && !this.SysEnv.SysSettings.IsDisconnectedMode
                    && this.XMLDataCriteria != null
                    && int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value) != 0)
                {
                    isResult = await this.ReleaseBySection();
                }
                else if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    isResult = await this.ReleaseByOrder();
                }
            }

            return isResult;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityPickingStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Picking";
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            
            // TODO: Resource strings            
            Label labelPickingOrderNumber = new Label { Text = "Order #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPickingDelDate = new Label { Text = "Del Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPickingRoute = new Label { Text = "Route", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPickingStop = new Label { Text = "Stop", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            Label labelPickingShipTo = new Label { Text = "Ship To", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            double columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingOrderNumber.Text);
            double columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDelDate.Text);
            double columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingRoute.Text);
            double columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingStop.Text);
            double columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingShipTo.Text);
            double detailWidth0 = 0;
            double detailWidth1 = 0;
            double detailWidth2 = 0;
            double detailWidth3 = 0;
            double detailWidth4 = 0;

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
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelPickingOrderNumber);
            gridHeader.Children.Add(labelPickingDelDate);

            if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
            {
                gridHeader.Children.Add(labelPickingRoute);
                gridHeader.Children.Add(labelPickingStop);
            }

            gridHeader.Children.Add(labelPickingShipTo);

            Grid.SetColumn(labelPickingOrderNumber, 0);
            Grid.SetColumn(labelPickingDelDate, 1);

            if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
            {
                Grid.SetColumn(labelPickingRoute, 2);
                Grid.SetColumn(labelPickingStop, 3);
            }

            Grid.SetColumn(labelPickingShipTo, 4);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Picking picking = new Picking
                {
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityPickingFKRefno = Convert.ToInt32(row["ActivityPickingFKRefno"] != DBNull.Value ? row["ActivityPickingFKRefno"] : null),
                    ActivityPickingStringFKRefno = Convert.ToString(row["ActivityPickingStringFKRefno"] != DBNull.Value ? row["ActivityPickingStringFKRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                    ActivityPickingOrderedDateTime = Convert.ToDateTime(row["ActivityPickingOrderedDateTime"] != DBNull.Value ? row["ActivityPickingOrderedDateTime"] : null),
                    ActivityPickingDeliveryDateTime = Convert.ToDateTime(row["ActivityPickingDeliveryDateTime"] != DBNull.Value ? row["ActivityPickingDeliveryDateTime"] : null),
                    ActivityPickingCustomerContactName = Convert.ToString(row["ActivityPickingCustomerContactName"] != DBNull.Value ? row["ActivityPickingCustomerContactName"] : null),
                    ActivityPickingCustomerShipToInfo = Convert.ToString(row["ActivityPickingCustomerShipToInfo"] != DBNull.Value ? row["ActivityPickingCustomerShipToInfo"] : null),
                    RouteCode = Convert.ToString(row["RouteCode"] != DBNull.Value ? row["RouteCode"] : null),
                    RouteDescription = Convert.ToString(row["RouteDescription"] != DBNull.Value ? row["RouteDescription"] : null),
                    RouteDayOfWeek = Convert.ToString(row["RouteDayOfWeek"] != DBNull.Value ? row["RouteDayOfWeek"] : null),
                    RouteStop = Convert.ToInt32(row["RouteStop"] != DBNull.Value ? row["RouteStop"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    ActivityPickingIsReviewed = Convert.ToBoolean(row["ActivityPickingIsReviewed"] != DBNull.Value ? row["ActivityPickingIsReviewed"] : null),
                };

                observableCollection.Add(picking);

                detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingStringFKRefno);
                detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingDeliveryDateTime.ToString("MM/dd/yyyy"));
                detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.RouteCode);
                detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(picking.RouteStop));
                detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingCustomerShipToInfo);

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

                if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                {
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
                }
                else
                {
                    columnWidth2 = 0;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                    columnWidth3 = 0;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }
            }

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
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label labelActivityPickingStringFKRefno = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityPickingDeliveryDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityPickingRouteCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityPickingRouteStop = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelActivityPickingCustomerShipToInfo = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityPickingStringFKRefno.SetBinding(Label.TextProperty, new Binding("ActivityPickingStringFKRefno"));
                labelActivityPickingDeliveryDateTime.SetBinding(Label.TextProperty, new Binding("ActivityPickingDeliveryDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityPickingRouteCode.SetBinding(Label.TextProperty, new Binding("RouteCode"));
                labelActivityPickingRouteStop.SetBinding(Label.TextProperty, new Binding("RouteStop"));
                labelActivityPickingCustomerShipToInfo.SetBinding(Label.TextProperty, new Binding("ActivityPickingCustomerShipToInfo"));

                gridData.Children.Add(labelActivityPickingStringFKRefno);
                gridData.Children.Add(labelActivityPickingDeliveryDateTime);

                if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                {
                    gridData.Children.Add(labelActivityPickingRouteCode);
                    gridData.Children.Add(labelActivityPickingRouteStop);
                }

                gridData.Children.Add(labelActivityPickingCustomerShipToInfo);

                Grid.SetColumn(labelActivityPickingStringFKRefno, 0);
                Grid.SetColumn(labelActivityPickingDeliveryDateTime, 1);

                if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                {
                    Grid.SetColumn(labelActivityPickingRouteCode, 2);
                    Grid.SetColumn(labelActivityPickingRouteStop, 3);
                }

                Grid.SetColumn(labelActivityPickingCustomerShipToInfo, 4);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplatePicking;

            return observableCollection;
        }


        //    /// <summary>
        //    ///
        //    /// </summary>
        //    /// <remarks></remarks>
        //    public override void DesignDataGrid(DataGrid dataGridMain)
        //    {
        //      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityPickingStringFKRefno";
        //      dataGridCol.HeaderText = "Order #";

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityPickingDeliveryDateTime";
        //      dataGridCol.HeaderText = "Del Date";
        //      dataGridCol.Width = 65 * colWidthFactor;
        //      dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //      if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
        //      {
        //        dataGridCol = new DataGridTextBoxColumn();
        //        dataGridCol.MappingName = "RouteCode";
        //        dataGridCol.HeaderText = "Route";

        //        dataGridCol = new DataGridTextBoxColumn();
        //        dataGridCol.MappingName = "RouteStop";
        //        dataGridCol.HeaderText = "Stop";
        //      }

        //      dataGridCol = new DataGridTextBoxColumn();
        //      dataGridCol.MappingName = "ActivityPickingCustomerShipToInfo";
        //      dataGridCol.HeaderText = "Ship To";

        //    }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            PickingSettings pickingSettings = new PickingSettings().LoadData();

            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityPicking.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityPicking.ActivityPickingFKRefno AS ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno AS ActivityPickingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode AS RouteCode,");
                sql.Append("tRoute.RouteDescription AS RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek AS RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop AS RouteStop,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus,");
                sql.Append("tActivityPicking.ActivityPickingIsReviewed AS ActivityPickingIsReviewed ");
                sql.Append("FROM tActivityPicking ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                    + "= tActivityPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno "
                    + "= tActivityPicking.ActivityRefno ");
                sql.Append("LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno ");
                sql.AppendFormat("WHERE tActivity.ActivityStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityPicking.ActivityRefno,");
                sql.Append("tActivityPicking.ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode,");
                sql.Append("tRoute.RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop,");
                sql.Append("tActivity.ActivityStatus,");
                sql.Append("tActivityPicking.ActivityPickingIsReviewed ");
                sql.Append("FROM tActivityPicking ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                    + "= tActivityPicking.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno "
                    + "= tActivityPicking.ActivityRefno ");
                sql.Append("LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno ");
                sql.AppendFormat("WHERE tActivityItem.ActivityItemStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityPicking.ActivityRefno,");
                sql.Append("tActivityPicking.ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode,");
                sql.Append("tRoute.RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop,");
                sql.Append("tActivity.ActivityStatus,");
                sql.Append("tActivityPicking.ActivityPickingIsReviewed ");
                sql.Append("FROM tActivityPicking ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                    + "= tActivityPicking.WarehouseLocationMapRefno ");
                sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno "
                    + "= tActivityItem.ActivityItemRefno ");
                sql.Append("LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno "
                    + "= tActivityPicking.ActivityRefno ");
                sql.Append("LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityDetailStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                sql.Append("SELECT tActivityPicking.ActivityRefno AS ActivityRefno,");
                sql.Append("tActivityPicking.ActivityPickingFKRefno AS ActivityPickingFKRefno,");
                sql.Append("tActivityPicking.ActivityPickingStringFKRefno AS ActivityPickingStringFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                sql.Append("tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,");
                sql.Append("tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,");
                sql.Append("tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,");
                sql.Append("tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,");
                sql.Append("tRoute.RouteCode AS RouteCode,");
                sql.Append("tRoute.RouteDescription AS RouteDescription,");
                sql.Append("tRoute.RouteDayOfWeek AS RouteDayOfWeek,");
                sql.Append("tActivityPickingRoute.RouteStop AS RouteStop,");
                sql.Append("tActivity.ActivityStatus AS ActivityStatus,");
                sql.Append("tActivityPicking.ActivityPickingIsReviewed AS ActivityPickingIsReviewed ");
                sql.Append("FROM tActivityPicking ");
                sql.Append("INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                    + "= tActivityPicking.WarehouseLocationMapRefno ");
                sql.Append("LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno "
                    + "= tActivityPicking.ActivityRefno ");
                sql.Append("LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno");
            }

            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);

                    this.DataViewHdrs = ds.Tables[0].DefaultView;
                    this.DataViewHdrs.Table.TableName = this.MainGridMappingName;
                    this.DataViewHdrs.Sort = pickingSettings.GetDataViewSortBy();
                    this.DataViewHdrs.RowFilter = this.GetFilter();
                }
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
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActFilter))
            {
                this.DataViewHdrs.RowFilter = "ActivityPickingStringFKRefno LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewHdrs.RowFilter = this.ActFilter + " AND ActivityPickingStringFKRefno LIKE \'%"
                    + searchText.Trim() + "%\'";
            }

            foreach (DataRow row in this.DataViewHdrs.ToTable().Rows)
            {
                Picking picking = new Picking
                {
                    ActivityRefno = Convert.ToInt32(row["ActivityRefno"] != DBNull.Value ? row["ActivityRefno"] : null),
                    ActivityPickingFKRefno = Convert.ToInt32(row["ActivityPickingFKRefno"] != DBNull.Value ? row["ActivityPickingFKRefno"] : null),
                    ActivityPickingStringFKRefno = Convert.ToString(row["ActivityPickingStringFKRefno"] != DBNull.Value ? row["ActivityPickingStringFKRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"] != DBNull.Value ? row["LocationMapWarehouseFKRefno"] : null),
                    LocationMapWarehouseName = Convert.ToString(row["LocationMapWarehouseName"] != DBNull.Value ? row["LocationMapWarehouseName"] : null),
                    ActivityPickingOrderedDateTime = Convert.ToDateTime(row["ActivityPickingOrderedDateTime"] != DBNull.Value ? row["ActivityPickingOrderedDateTime"] : null),
                    ActivityPickingDeliveryDateTime = Convert.ToDateTime(row["ActivityPickingDeliveryDateTime"] != DBNull.Value ? row["ActivityPickingDeliveryDateTime"] : null),
                    ActivityPickingCustomerContactName = Convert.ToString(row["ActivityPickingCustomerContactName"] != DBNull.Value ? row["ActivityPickingCustomerContactName"] : null),
                    ActivityPickingCustomerShipToInfo = Convert.ToString(row["ActivityPickingCustomerShipToInfo"] != DBNull.Value ? row["ActivityPickingCustomerShipToInfo"] : null),
                    RouteCode = Convert.ToString(row["RouteCode"] != DBNull.Value ? row["RouteCode"] : null),
                    RouteDescription = Convert.ToString(row["RouteDescription"] != DBNull.Value ? row["RouteDescription"] : null),
                    RouteDayOfWeek = Convert.ToString(row["RouteDayOfWeek"] != DBNull.Value ? row["RouteDayOfWeek"] : null),
                    RouteStop = Convert.ToInt32(row["RouteStop"] != DBNull.Value ? row["RouteStop"] : null),
                    ActivityStatus = Convert.ToInt32(row["ActivityStatus"] != DBNull.Value ? row["ActivityStatus"] : null),
                    ActivityPickingIsReviewed = Convert.ToBoolean(row["ActivityPickingIsReviewed"] != DBNull.Value ? row["ActivityPickingIsReviewed"] : null),
                };

                observableCollection.Add(picking);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityPickingFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);
            this.SelectedIndex = idx;

            // Set the selected warehouse to the activity header warehouse.
            if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]))
            {
                LocationMap.GetInstance().CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewHdrs[idx]["LocationMapRefno"]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityPickingFKRefno"]);
            this.Refno = Convert.ToInt32(this.DataViewHdrs[idx]["ActivityRefno"]);

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

                TableActivityPicking actPicking = new TableActivityPicking();
                try
                {
                    actPicking.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                    actPicking.RequestData(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    actPicking.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            TableActivityPicking actPicking = new TableActivityPicking();
            TableActivityPickingRoute actPickingRoute = new TableActivityPickingRoute();
            TableActivityPickingItem actPickingItem = new TableActivityPickingItem();
            TableActivityPickingDetail actPickingDetail = new TableActivityPickingDetail();

            actPickingDetail.Purge();
            actPickingItem.Purge();
            actPickingRoute.Purge();
            actPicking.Purge();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            string filter = string.Empty;
            PickingSettings pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (!string.IsNullOrEmpty(pickingSettings.strRouteCodeSelectedValue))
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter += string.Format("RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
                    }
                    else
                    {
                        filter += string.Format(" AND RouteCode = \'{0}\'", pickingSettings.strRouteCodeSelectedValue);
                    }
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.dtOrderedDateFilter))
                {
                    DateTime dt = pickingSettings.dtOrderedDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.dtDeliveryDateFilter))
                {
                    DateTime dt = pickingSettings.dtDeliveryDateFilter;

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            string filter = string.Empty;
            PickingSettings pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (string.Compare(data.Root.Element("route_code").Value, string.Empty) > 0)
                {
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = "RouteCode = \'" + data.Root.Element("route_code").Value + "\'";
                    }
                    else
                    {
                        filter = filter + " AND RouteCode = \'" + data.Root.Element("route_code").Value + "\'";
                    }
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (string.Compare(data.Root.Element("ordered_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("ordered_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingOrderedDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingOrderedDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (string.Compare(data.Root.Element("delivery_date").Value, string.Empty) > 0)
                {
                    DateTime dt = DateTime.Parse(data.Root.Element("delivery_date").Value);

                    // BETWEEN '2009-07-26 00:00:00.000' AND '2010-07-26 23:59:59.997'
                    if (string.IsNullOrEmpty(filter))
                    {
                        filter = string.Format(
                            "ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                    else
                    {
                        filter += string.Format(
                            " AND ActivityPickingDeliveryDateTime >= '{0}-{1}-{2} 00:00:00.000' "
                            + "AND ActivityPickingDeliveryDateTime <= '{0}-{1}-{2} 23:59:59.997'",
                            dt.Year,
                            dt.Month,
                            dt.Day);
                    }
                }
            }

            this.ActFilter = filter;

            return filter;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task<bool> ReleaseBySection()
        {
            bool isReleased = false;

            int locMapRefno = 0;
            SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapFKRefno(int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value), int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value), 0, ref locMapRefno);

            // TODO: Resource strings                
            string msg = string.Format("Release order# {0} for warehouse {1}, section {2}?", this.FKRefno, locMap.WhsName, locMap.SectName);

            // TODO: Resource strings
            //if (MessageBox.Show(
            //    msg,
            //    this.GetControlDescription(),
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question,
            //    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            if (await Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), msg, "Yes", "No"))
            {
                string reqData = string.Empty;
                StringBuilder xmlString = new StringBuilder();
                XmlWriter xml = XmlWriter.Create(xmlString);
                XDocument request = null;

                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>RELEASE SALES ORDER BY ORDER ITEM SECTION</action>"
                    + "</command>"
                    + SysCommon.GetRequestDefaults(false)
                    + ("<refno>" + this.FKRefno.ToString() + "</refno>")
                    + ("<whs_id>" + locMap.WhsFKRefno + "</whs_id>")
                    + ("<sect_refno>" + locMap.SectFKRefno + "</sect_refno>")
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                reqData = SysCommon.SubmitComm(xmlString.ToString());
                
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;
                        
                        // TODO: Resource strings
                        throw new Exception(string.Format("Release order# {0} by order item section {1} failed." + Environment.NewLine + errorDetail, this.FKRefno.ToString(), locMap.SectName));
                    }

                    isReleased = true;
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Release order# {0} by order item section {1} failed.", this.FKRefno, locMap.SectName));
                }
            }

            return isReleased;
        }

        #endregion
    }
}
