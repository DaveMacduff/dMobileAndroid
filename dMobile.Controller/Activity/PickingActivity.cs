// <copyright file="PickingActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Bedrock.Settings;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Headers;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="PickingActivity" />.
    /// </summary>
    public class PickingActivity : Activity
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
        /// Defines the listPicking.
        /// </summary>
        private List<Picking> listPicking = new List<Picking>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PickingActivity"/> class.
        /// </summary>
        public PickingActivity()
          : base()
        {
            DefineInitialGridDataTemplate();

            void DefineInitialGridDataTemplate()
            {
                var dataTemplatePicking = new DataTemplate(() =>
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

                    var labelActivityStringFkRefno = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityPickingDeliveryDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityPickingRouteCode = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityPickingRouteStop = new Label { VerticalOptions = LayoutOptions.Center };
                    var labelActivityPickingCustomerShipToInfo = new Label { VerticalOptions = LayoutOptions.Center };

                    labelActivityStringFkRefno.SetBinding(Label.TextProperty, new Binding("ActivityStringFKRefno"));
                    labelActivityPickingDeliveryDateTime.SetBinding(Label.TextProperty, new Binding("ActivityPickingDeliveryDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                    labelActivityPickingRouteCode.SetBinding(Label.TextProperty, new Binding("RouteCode"));
                    labelActivityPickingRouteStop.SetBinding(Label.TextProperty, new Binding("RouteStop"));
                    labelActivityPickingCustomerShipToInfo.SetBinding(Label.TextProperty, new Binding("ActivityPickingCustomerShipToInfo"));

                    gridData.Children.Add(labelActivityStringFkRefno);
                    gridData.Children.Add(labelActivityPickingDeliveryDateTime);

                    if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                    {
                        gridData.Children.Add(labelActivityPickingRouteCode);
                        gridData.Children.Add(labelActivityPickingRouteStop);
                    }

                    gridData.Children.Add(labelActivityPickingCustomerShipToInfo);

                    Grid.SetColumn(labelActivityStringFkRefno, 0);
                    Grid.SetColumn(labelActivityPickingDeliveryDateTime, 1);

                    if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                    {
                        Grid.SetColumn(labelActivityPickingRouteCode, 2);
                        Grid.SetColumn(labelActivityPickingRouteStop, 3);
                    }

                    Grid.SetColumn(labelActivityPickingCustomerShipToInfo, 4);

                    return new ViewCell { View = gridData };
                });

                var isAddKey = true;
                foreach (var item in this.resourceDictionary)
                {
                    if (item.Key == "dataTemplatePicking")
                    {
                        isAddKey = false;
                    }
                }

                if (isAddKey)
                {
                    this.resourceDictionary.Add("dataTemplatePicking", dataTemplatePicking);
                }
            }
        }

        /// <summary>
        /// Gets or sets the PickingLocationMapFilter.
        /// </summary>
        public SLocMap PickingLocationMapFilter { get; set; } = new SLocMap();

        /// <summary>
        /// Display the extra information for the selected activity.
        /// </summary>
        /// <param name="extraInfo">Extra information collection.</param>
        public override void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            base.DisplayExtraInfo(extraInfo);
            if (this.SelectedActivity is Picking activity)
            {
                extraInfo.Add("Customer: " + activity.ActivityPickingCustomerContactName);
                extraInfo.Add("Ship to:   " + activity.ActivityPickingCustomerShipToInfo);
                extraInfo.Add(
                    $"Order warehouse:   {activity.LocationMapWarehouseFKRefno} - {activity.LocationMapWarehouseName}");
            }
        }

        /// <summary>
        /// The PrintLabel.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task PrintLabel(XDocument data)
        {
            var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                int.Parse(this.XmlDataCriteria.Root.Element("whs_id").Value),
                int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value),
                0);
            var locMap = sLocMapAndRefno.SLocMap;
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var prnSettings = new PrinterSettings().LoadData();

            // Don't even bother sending in a print request if no quantity was entered.
            var labelQty = new SLabelQuantity
            {
                FullQuantity = int.Parse(data.Root.Element("full_alt_qty").Value),
                RepackQuantity = int.Parse(data.Root.Element("repack_alt_qty").Value),
                FreshQuantity = int.Parse(data.Root.Element("fresh_alt_qty").Value),
                FrozenQuantity = int.Parse(data.Root.Element("frozen_alt_qty").Value),
                LiveQuantity = int.Parse(data.Root.Element("live_alt_qty").Value),
                DryQuantity = int.Parse(data.Root.Element("dry_alt_qty").Value),
                RepackFreshQuantity = int.Parse(data.Root.Element("repack_fresh_alt_qty").Value),
                RepackFrozenQuantity = int.Parse(data.Root.Element("repack_frozen_alt_qty").Value),
                RepackLiveQuantity = int.Parse(data.Root.Element("repack_live_alt_qty").Value),
                RepackDryQuantity = int.Parse(data.Root.Element("repack_dry_alt_qty").Value),
                PalletQuantity = int.Parse(data.Root.Element("pallet_alt_qty").Value),
                BundleQuantity = int.Parse(data.Root.Element("bundle_alt_qty").Value),
            };

            // Used abs for handling the freaky event that a negative label quantity was requested.
            if (labelQty.FullQuantity + labelQty.RepackQuantity + labelQty.FreshQuantity + labelQty.FrozenQuantity
                + labelQty.LiveQuantity + labelQty.DryQuantity + labelQty.RepackFreshQuantity + labelQty.RepackFrozenQuantity
                + labelQty.RepackLiveQuantity + labelQty.RepackDryQuantity + labelQty.PalletQuantity
                + labelQty.BundleQuantity > 0)
            {
                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                    <command>
                        <action>SUBMIT PRINT SHIPPING LABEL</action>
                    </command>
                    {await SysCommon.GetRequestDefaults(false)}
                    <refno>{this.HdrFkRefno}</refno>
                    <itm_refno>{data.Root.Element("itm_refno").Value}</itm_refno>
                    <whs_id>{locMap.WhsFkRefno}</whs_id>
                    <sect_refno>{locMap.SectFkRefno}</sect_refno>
                    <full_alt_qty>{data.Root.Element("full_alt_qty").Value}</full_alt_qty>
                    <repack_alt_qty>{data.Root.Element("repack_alt_qty").Value}</repack_alt_qty>
                    <fresh_alt_qty>{data.Root.Element("fresh_alt_qty").Value}</fresh_alt_qty>
                    <frozen_alt_qty>{data.Root.Element("frozen_alt_qty").Value}</frozen_alt_qty>
                    <live_alt_qty>{data.Root.Element("live_alt_qty").Value}</live_alt_qty>
                    <dry_alt_qty>{data.Root.Element("dry_alt_qty").Value}</dry_alt_qty>
                    <repack_fresh_alt_qty>{data.Root.Element("repack_fresh_alt_qty").Value}</repack_fresh_alt_qty>
                    <repack_frozen_alt_qty>{data.Root.Element("repack_frozen_alt_qty").Value}</repack_frozen_alt_qty>
                    <repack_live_alt_qty>{data.Root.Element("repack_live_alt_qty").Value}</repack_live_alt_qty>
                    <repack_dry_alt_qty>{data.Root.Element("repack_dry_alt_qty").Value}</repack_dry_alt_qty>
                    <pallet_alt_qty>{data.Root.Element("pallet_alt_qty").Value}</pallet_alt_qty>
                    <bundle_alt_qty>{data.Root.Element("bundle_alt_qty").Value}</bundle_alt_qty>
                    <total_alt_qty>{data.Root.Element("total_alt_qty").Value}</total_alt_qty>
                    <mobile_printer_name>
                        {(prnSettings.IsEnabledShippingLabelPrinter ? prnSettings.ShippingLabelPrinterName : string.Empty)}
                    </mobile_printer_name>
                    </imclient>";

                var request = XDocument.Parse(xmlRequest);

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();
                _ = SysCommon.SubmitComm(xmlString.ToString());
            }
        }

        /// <summary>
        /// Request from the server the number of suggested shipping labels to print.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>Structure containing of number of labels to print.</returns>
        public override async Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            var labelQty = new SLabelQuantity();

            try
            {
                var sendReq = string.Empty;
                var responseData = string.Empty;
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);

                XDocument request = null;
                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                    int.Parse(this.XmlDataCriteria.Root.Element("whs_id").Value),
                    int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value),
                    slotLocMapFkRefno: 0);
                var locMap = sLocMapAndRefno.SLocMap;

                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                    <command>
                        <action>REQUEST SHIPPING LABEL QUANTITY</action>
                    </command>
                    {await SysCommon.GetRequestDefaults(false)}
                    <refno>{this.HdrFkRefno}</refno>
                    <sect_refno>{locMap.SectFkRefno}</sect_refno>
                    </imclient>";

                request = XDocument.Parse(xmlRequest);

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                sendReq = xmlString.ToString();
                responseData = SysCommon.SubmitComm(sendReq);
                var xmlResult = new XmlDocument();
                xmlResult.LoadXml(responseData);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(this.GetControlDescription(), "Get full unit label quantity failed." + Environment.NewLine + ex.Message, "OK"));
            }

            return labelQty;
        }

        /// <summary>
        /// /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> IsSalesOrderBeingPicked()
        {
            var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                int.Parse(this.XmlDataCriteria.Root.Element("whs_id").Value),
                int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value),
                slotLocMapFkRefno: 0);
            var locMap = sLocMapAndRefno.SLocMap;
            var responseData = string.Empty;
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request = null;

            var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                <imclient>
                <command>
                    <action>REQUEST SALES ORDER IS BEING PICKED</action>
                    {await SysCommon.GetRequestDefaults(false)}
                    <refno>{this.HdrFkRefno.ToString()}</refno>
                    <whs_id>{locMap.WhsFkRefno}</whs_id>
                    <sect_refno>{locMap.SectFkRefno}</sect_refno>
                </command>
                </imclient>";

            request = XDocument.Parse(xmlRequest);
            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            var isBeingPicked = false;
            responseData = SysCommon.SubmitComm(xmlString.ToString());
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);
            if (responseData.Contains("<message>ERROR</message>"))
            {
                isBeingPicked = true;
                var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    errorDetail,
                    "OK"));
            }

            return isBeingPicked;
        }

        /// <summary>
        /// Clear the sales order being picked flag.
        /// </summary>
        /// <param name="clearDeviceOnly"><see cref="bool"/> </param>
        /// <returns></returns>
        public async Task<bool> ClearSalesOrderBeingPicked(bool clearDeviceOnly)
        {
            var isClearedBeingPicked = true;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                    int.Parse(this.XmlDataCriteria.Root.Element("whs_id").Value),
                    int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value),
                    slotLocMapFkRefno: 0);
                var locMap = sLocMapAndRefno.SLocMap;
                var responseData = string.Empty;
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);
                XDocument request = null;
                isClearedBeingPicked = false;

                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                        <command>
                            <action>SUBMIT SALES ORDER CLEAR BEING PICKED</action>
                            {await SysCommon.GetRequestDefaults(false)}
                            <refno>{this.HdrFkRefno.ToString()}</refno>
                            <whs_id>{locMap.WhsFkRefno}</whs_id>
                            <sect_refno>{locMap.SectFkRefno}</sect_refno>
                            <device_only>{clearDeviceOnly}</device_only>
                        </command>
                    </imclient>";

                request = XDocument.Parse(xmlRequest);
                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                try
                {
                    responseData = SysCommon.SubmitComm(xmlString.ToString());
                    isClearedBeingPicked = true;
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        "OK"));
                }
            }

            return isClearedBeingPicked;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> ReleaseByOrder()
        {
            var isReleased = false;

            if (await Device.InvokeOnMainThreadAsync(async () =>
                await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    $"Release order# {this.HdrFkRefno} ?",
                    "Yes",
                    "No")))
            {
                var reqResult = string.Empty;
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);

                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                        <command>
                            <action>RELEASE SALES ORDER BY ORDER</action>
                        </command>
                        {await SysCommon.GetRequestDefaults(false)}
                        <refno>{this.HdrFkRefno}</refno>
                    </imclient>";

                var request = XDocument.Parse(xmlRequest);
                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                reqResult = SysCommon.SubmitComm(xmlString.ToString());
                if (reqResult.Length > 0)
                {
                    isReleased = true;
                }
                else
                {
                    throw new Exception($"Release order# {this.HdrFkRefno} failed.");
                }
            }

            return isReleased;
        }

        /// <summary>
        /// /// </summary>
        /// <remarks></remarks>
        /// <summary>
        ///.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task PickingModuleQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var query = $@"
                    SELECT COUNT(tActivityItem.ActivityItemRefno) 
                        FROM tActivityItem 
                        INNER JOIN tActivityItemPicking ON tActivityItemPicking.ActivityItemRefno = tActivityItem.ActivityItemRefno 
                        WHERE tActivityItem.ActivityRefno = {this.HdrRefno}
                            AND 0 < tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable 
                            AND tActivityItemPicking.ActivityItemPickingQuantityShipped < tActivityItemPicking.ActivityItemPickingQuantityOrdered";

                var rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<object>(query));

                // Do not warn if product is out of stock WO# 12054
                if (rowsAffected > 0)
                {
                    throw new Exception(string.Format("Order# {0} quantity under check failed." + Environment.NewLine + "{1} Items exist where the quantity shipped is less than the quantity ordered.", this.HdrFkRefno, rowsAffected));
                }
            }
            else
            {
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);

                var locMap = LocationMap.GetInstance();

                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                        <command>
                            <action>REQUEST SALES ORDER QUANTITY UNDER CHECK</action>
                        </command>
                        {await SysCommon.GetRequestDefaults(false)}
                        <refno>{this.HdrFkRefno}</refno>
                        <whs_id>{locMap.CurrLocMap.WhsFkRefno}</whs_id>
                        <sect_refno>{locMap.CurrLocMap.SectFkRefno}</sect_refno>
                    </imclient>";
                var request = XDocument.Parse(xmlRequest);

                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                var responseData = string.Empty;
                try
                {
                    responseData = SysCommon.SubmitComm(xmlString.ToString());
                }
                catch (MessageOnlyException msgEx)
                {
                    var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                    var docError = new XmlDocument();
                    docError.LoadXml(responseData);
                    var errorDetail = docError.DocumentElement["detail"].InnerText;
                    throw new Exception(string.Format(
                        "Order# {0} quantity under check failed." +
                        Environment.NewLine + "{1}",
                        this.HdrFkRefno,
                        errorDetail));
                }
                catch (Exception ex)
                {
                    var msg = string.Format(
                    "Order# {0} quantity under check failed." +
                    Environment.NewLine + "{1}",
                    this.HdrFkRefno,
                    ex.Message);
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Check if all items reviewed.
        /// Perform under quantity check for ALL items.
        /// Releases by Section or Order.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override async Task<bool> ChangeStatus(object selectedItem)
        {
            var isResult = true;

            if (isResult)
            {
                if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel && selectedItem is Picking picking)
                {
                    isResult = Convert.ToBoolean(picking.ActivityPickingIsReviewed);
                    if (this.SysEnv.SysSettings.IsPrePickCrit
                        && this.SysEnv.SysSettings.IsRelBySect
                        && !this.SysEnv.SysSettings.IsDisconnectedMode
                        && this.XmlDataCriteria != null
                        && int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value) != 0)
                    {
                        if (!isResult)
                        {
                            throw new Exception("Items that have not been reviewed exist for this section.");
                        }
                    }
                    else if (this.SysEnv.SysSettings.IsRelByOrder && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        if (!isResult)
                        {
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
                        await this.PickingModuleQuantityUnderCheck();
                    }
                    catch (MessageOnlyException msgEx)
                    {
                        var msg = (msgEx.Message.IndexOf("MSGBOX-") == -1) ? msgEx.Message : msgEx.Message["MSGBOX-".Length..];
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                                if (await Device.InvokeOnMainThreadAsync(async () =>
                                        await Application.Current.MainPage.DisplayAlert(
                                            this.GetControlDescription(),
                                            msgEx.Message + Environment.NewLine + "Continue?",
                                            "Yes",
                                            "No")))
                                {
                                    isResult = true;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                                await Device.InvokeOnMainThreadAsync(async () =>
                                    await Application.Current.MainPage.DisplayAlert(
                                        this.GetControlDescription(), msgEx.Message, "OK"));
                                isResult = false;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Items that have not been reviewed exist for this order.");
                    }
                }
            }

            if (isResult)
            {
                if (this.SysEnv.SysSettings.IsPrePickCrit
                    && this.SysEnv.SysSettings.IsRelBySect
                    && !this.SysEnv.SysSettings.IsDisconnectedMode
                    && this.XmlDataCriteria != null
                    && int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value) != 0)
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
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Picking";
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The gridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The listViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            var observableCollection = new ObservableCollection<object>();

            var labelPickingOrderNumber = new Label { Text = "Order #", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPickingDelDate = new Label { Text = "Del Date", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPickingRoute = new Label { Text = "Route", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPickingStop = new Label { Text = "Stop", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelPickingShipTo = new Label { Text = "Ship To", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingOrderNumber.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDelDate.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingRoute.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingStop.Text);
            this.columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingShipTo.Text);

            // Create header data template
            var gridHeader = new Grid
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

            gridMainActivity.Children.Add(gridHeader);

            foreach (var picking in this.ListHeaders.Cast<Picking>())
            {
                observableCollection.Add(picking);

                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityStringFKRefno);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingDeliveryDateTime.ToString("MM/dd/yyyy"));
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.RouteCode ?? string.Empty);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(Convert.ToString(picking.RouteStop) ?? string.Empty);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(picking.ActivityPickingCustomerShipToInfo);

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

                if (!this.SysEnv.SysSettings.IsPickHideRouteCol)
                {
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
                }
                else
                {
                    this.columnWidth2 = 0;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                    this.columnWidth3 = 0;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }

                if (detailWidth4 > this.columnWidth4)
                {
                    this.columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.columnWidth4;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplatePicking"];

            return observableCollection;
        }

        /// <summary>
        ///.
        /// </summary>
        /// <param name="isSubmitBatch">The isSubmitBatch<see cref="bool"/>.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var pickingSettings = new PickingSettings().LoadData();
            var sql = string.Empty;
            if (isSubmitBatch)
            {
                sql = $@"
                    SELECT tActivityPicking.ActivityRefno,
                        tActivityPicking.ActivityFKRefno,
                        tActivityPicking.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,
                        tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,
                        tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,
                        tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,
                        tRoute.RouteCode AS RouteCode,
                        tRoute.RouteDescription AS RouteDescription,
                        tRoute.RouteDayOfWeek AS RouteDayOfWeek,
                        tActivityPickingRoute.RouteStop AS RouteStop,
                        tActivity.ActivityStatus AS ActivityStatus,
                        tActivityPicking.ActivityPickingIsReviewed AS ActivityPickingIsReviewed 
                    FROM tActivityPicking 
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityPicking.WarehouseLocationMapRefno 
                    LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno = tActivityPicking.ActivityRefno 
                    LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno 
                    WHERE tActivity.ActivityStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)} 
                    UNION 
                    SELECT tActivityPicking.ActivityRefno,
                        tActivityPicking.ActivityFKRefno,
                        tActivityPicking.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName,
                        tActivityPicking.ActivityPickingOrderedDateTime,
                        tActivityPicking.ActivityPickingDeliveryDateTime,
                        tActivityPicking.ActivityPickingCustomerContactName,
                        tActivityPicking.ActivityPickingCustomerShipToInfo,
                        tRoute.RouteCode,
                        tRoute.RouteDescription,
                        tRoute.RouteDayOfWeek,
                        tActivityPickingRoute.RouteStop,
                        tActivity.ActivityStatus,
                        tActivityPicking.ActivityPickingIsReviewed 
                    FROM tActivityPicking 
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityPicking.WarehouseLocationMapRefno 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno 
                    LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno = tActivityPicking.ActivityRefno 
                    LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno 
                    WHERE tActivityItem.ActivityItemStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)} 
                    UNION 
                    SELECT tActivityPicking.ActivityRefno,
                        tActivityPicking.ActivityFKRefno,
                        tActivityPicking.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName,
                        tActivityPicking.ActivityPickingOrderedDateTime,
                        tActivityPicking.ActivityPickingDeliveryDateTime,
                        tActivityPicking.ActivityPickingCustomerContactName,
                        tActivityPicking.ActivityPickingCustomerShipToInfo,
                        tRoute.RouteCode,
                        tRoute.RouteDescription,
                        tRoute.RouteDayOfWeek,
                        tActivityPickingRoute.RouteStop,
                        tActivity.ActivityStatus,
                        tActivityPicking.ActivityPickingIsReviewed 
                    FROM tActivityPicking 
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityPicking.WarehouseLocationMapRefno 
                    INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno 
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno 
                    LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno = tActivityPicking.ActivityRefno 
                    LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno 
                    WHERE tActivityDetail.ActivityDetailStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}";
            }
            else
            {
                sql = @"
                    SELECT tActivityPicking.ActivityRefno,
                        tActivityPicking.ActivityFKRefno,
                        tActivityPicking.ActivityStringFKRefno,
                        tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,
                        tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,
                        tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,
                        tActivityPicking.ActivityPickingOrderedDateTime AS ActivityPickingOrderedDateTime,
                        tActivityPicking.ActivityPickingDeliveryDateTime AS ActivityPickingDeliveryDateTime,
                        tActivityPicking.ActivityPickingCustomerContactName AS ActivityPickingCustomerContactName,
                        tActivityPicking.ActivityPickingCustomerShipToInfo AS ActivityPickingCustomerShipToInfo,
                        tRoute.RouteCode AS RouteCode,
                        tRoute.RouteDescription AS RouteDescription,
                        tRoute.RouteDayOfWeek AS RouteDayOfWeek,
                        tActivityPickingRoute.RouteStop AS RouteStop,
                        tActivity.ActivityStatus AS ActivityStatus,
                        tActivityPicking.ActivityPickingIsReviewed AS ActivityPickingIsReviewed 
                    FROM tActivityPicking 
                    INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityPicking.ActivityRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityPicking.WarehouseLocationMapRefno 
                    LEFT JOIN tActivityPickingRoute ON tActivityPickingRoute.PickingActivityRefno = tActivityPicking.ActivityRefno 
                    LEFT JOIN tRoute ON tRoute.RouteRefno = tActivityPickingRoute.RouteRefno";
            }

            this.listPicking = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<Picking>(sql);
            foreach (var sort in pickingSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.listPicking = this.listPicking.OrderBy(picking => picking.GetType().GetProperty(sort.SortValue).GetValue(picking)).ToList();
                }
                else
                {
                    this.listPicking = this.listPicking.OrderByDescending(picking => picking.GetType().GetProperty(sort.SortValue).GetValue(picking)).ToList();
                }
            }

            this.GetFilter();

            this.ListHeaders = this.listPicking.Cast<object>().ToList();
            return this.ListHeaders;
        }

        /// <inheritdoc cref="Activity.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();

            this.ListHeaders = this.listPicking.Where(picking => picking.ActivityStringFKRefno.ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (var picking in this.ListHeaders.Cast<Picking>())
            {
                observableCollection.Add(picking);
            }

            return observableCollection;
        }

        /// <summary>
        /// The SelectActivity.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Picking header)
            {
                this.SelectedActivity = header;
                this.HdrFkRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;
                this.LocationMapRefno = header.LocationMapRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    // Set the current location to the header.
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    // Override and set the chosen location to the current location.
                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    // Override and set any selected skid location to the current location.
                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFkRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is Picking picking)
            {
                this.HdrFkRefno = Convert.ToInt32(picking.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(picking.ActivityRefno);
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var actPicking = new TableActivityPicking();
                await this.Purge();
                await actPicking.RequestHeaderData(data);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Purge()
        {
            var actPicking = new TableActivityPicking();
            var actPickingRoute = new TableActivityPickingRoute();
            var actPickingItem = new TableActivityPickingItem();
            var actPickingDetail = new TableActivityPickingDetail();

            await actPickingDetail.PurgeData();
            await actPickingItem.PurgeData();
            await actPickingRoute.PurgeData();
            await actPicking.PurgeData();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
            var filter = string.Empty;
            var pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (!string.IsNullOrEmpty(pickingSettings.RouteCodeSelectedValue))
                {
                    this.listPicking = this.listPicking.Where(pick => pick.RouteCode == pickingSettings.RouteCodeSelectedValue).ToList();
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.OrderedDateFilter))
                {
                    var dt = pickingSettings.OrderedDateFilter;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listPicking = this.listPicking.Where(pick => pick.ActivityPickingOrderedDateTime >= minDate
                    && pick.ActivityPickingOrderedDateTime <= maxDate).ToList();
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (!SysCommon.IsEmptyDate(pickingSettings.DeliveryDateFilter))
                {
                    var dt = pickingSettings.DeliveryDateFilter;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listPicking = this.listPicking.Where(pick => pick.ActivityPickingDeliveryDateTime >= minDate
                    && pick.ActivityPickingDeliveryDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        public override void GetFilter(XDocument data)
        {
            var filter = string.Empty;
            var pickingSettings = new PickingSettings().LoadData();

            if (pickingSettings.IsApplyRouteCodeFilter)
            {
                if (string.Compare(data.Root.Element("route_code").Value, string.Empty) > 0)
                {
                    this.listPicking = this.listPicking.Where(pick => pick.RouteCode == data.Root.Element("route_code").Value).ToList();
                }
            }

            if (pickingSettings.IsApplyOrderedDateFilter)
            {
                if (string.Compare(data.Root.Element("ordered_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("ordered_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listPicking = this.listPicking.Where(pick => pick.ActivityPickingOrderedDateTime >= minDate
                    && pick.ActivityPickingOrderedDateTime <= maxDate).ToList();
                }
            }

            if (pickingSettings.IsApplyDeliveryDateFilter)
            {
                if (string.Compare(data.Root.Element("delivery_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(data.Root.Element("delivery_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

                    this.listPicking = this.listPicking.Where(pick => pick.ActivityPickingDeliveryDateTime >= minDate
                    && pick.ActivityPickingDeliveryDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// The ReleaseBySection.
        /// </summary>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        private async Task<bool> ReleaseBySection()
        {
            var isReleased = false;
            var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFkRefno(
                int.Parse(this.XmlDataCriteria.Root.Element("whs_id").Value),
                int.Parse(this.XmlDataCriteria.Root.Element("sect_refno").Value),
                slotLocMapFkRefno: 0);
            var locMap = sLocMapAndRefno.SLocMap;
            var msg = $"Release order# {this.HdrFkRefno} for warehouse {locMap.WhsName}, section {locMap.SectName}?";
            if (await Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    msg,
                    "Yes",
                    "No")))
            {
                var responseData = string.Empty;
                var xmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = false,
                };
                var xml = XmlWriter.Create(xmlString, settings);
                XDocument request = null;
                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                    <imclient>
                        <command>
                            <action>RELEASE SALES ORDER BY ORDER ITEM SECTION</action>
                        </command>
                        {await SysCommon.GetRequestDefaults(false)}
                        <refno>{this.HdrFkRefno.ToString()}</refno>
                        <whs_id>{locMap.WhsFkRefno}</whs_id>
                        <sect_refno>{locMap.SectFkRefno}</sect_refno>
                    </imclient>";
                request = XDocument.Parse(xmlRequest);
                request.Save(xml);
                await xml.FlushAsync();
                xml.Close();

                responseData = SysCommon.SubmitComm(xmlString.ToString());
                if (responseData.Contains("<message>ERROR</message>"))
                {
                    var docError = new XmlDocument();
                    docError.LoadXml(responseData);
                    var errorDetail = docError.DocumentElement["detail"].InnerText;

                    throw new Exception(string.Format("Release order# {0} by order item section {1} failed." +
                        Environment.NewLine + errorDetail, this.HdrFkRefno.ToString(), locMap.SectName));
                }

                isReleased = true;
            }

            return isReleased;
        }
    }
}
