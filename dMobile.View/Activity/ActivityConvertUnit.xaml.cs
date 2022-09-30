// <copyright file="ActivityConvertUnit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;
using dMobile.Controller.Activity_Detail;
using dMobile.Controller.Activity_Item;

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Android.Views;
    using dMobile.BarCodeScanner.BarCodeParsing;
    using dMobile.BarCodeScanner.BarCodeScanners;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using dMobile.View.Views;
    using SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertUnit : DMobileContentPage, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityConvertUnit"/> class.
        /// </summary>
        public ActivityConvertUnit()
        {
            this.InitializeComponent();

            this.cmdLocation.Text = "Set Location";
            this.cmdSelectToUnit.Text = "Select To Unit";
            this.cmdSelectFromUnit.Text = "Select From Unit";
            this.lblConvertedUnitInfo.Text = "Converted Qty 0 UOM @ 0 KG/LB";
            this.lblFullUnitInfo.Text = "Qty 0 UOM @ 0 KG/LB";
            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";

            this.Scanner = ScannerEngine.GetInstance().Scanner;
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            ParseAndSeedBarCodeModifier();
            this.Title = this.GetControlDescription();

            //////////////////////////////////////////////////////////////////////////
            void ParseAndSeedBarCodeModifier()
            {
                this.BarCodeModifier1Picker.IsVisible = false;
                this.BarCodeModifier1Picker.IsEnabled = false;
                this.BarCodeModifier1Picker.Items.Clear();
                if (!this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    return;
                }

                this.BarCodeModifier1Picker.Items.Add(string.Empty);
                var modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');
                foreach (var item in modifiers)
                {
                    this.BarCodeModifier1Picker.Items.Add(item);
                }

                if (this.BarCodeModifier1Picker.Items.Count > 0)
                {
                    this.BarCodeModifier1Picker.IsVisible = true;
                    this.BarCodeModifier1Picker.IsEnabled = true;
                }

                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;
            }
        }

        /// <summary>
        /// The OnAppearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SubscribeKeyUpHandlers();

            try
            {
                this.Loading = true;
                if (this.FirstAppearance)
                {
                    this.FirstAppearance = false;

                    InitActivity();
                    await InitActivityItem();
                    InitActivityDetail();
                    if (this.ActDetail.ListDetails == null)
                    {
                        _ = await this.ActDetail.GetActivityDetailList(false);
                    }

                    await this.DesignObservableCollection();
                }

                await this.ucActivityConvertUnit_GotFocus(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                    "Error",
                    "this.ucActivityConvertUnit_GotFocus threw an error " + ex,
                    "OK"));
            }
            finally
            {
                this.Loading = false;
            }

            void InitActivityDetail()
            {
                this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.ConvertUnit);
                this.ActDetail.ActSubType = this.ActItem.ActSubType;
            }

            async Task InitActivityItem()
            {
                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.ConvertUnit);
                this.ActItem.ActSubType = this.Activity.ActSubType;
                _ = await this.ActItem.GetActivityItemList(false);
            }

            void InitActivity()
            {
                this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertUnit);
                this.Activity.SkidInfo = new SysSkidInfo();
                this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertUnit;
            }

            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    if (!this.Loading)
                    {
                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Enter || keycode == Keycode.F1)
                        {
                            if (await this.SaveChanges())
                            {
                                await this.Close();
                            }
                        }
                        else if (keycode == Keycode.F2)
                        {
                            if (this.dgUnitWeight.SelectedItem is ConvertUnitDetail convertUnitDetail)
                            {
                                var activityConvertUnitWeightEdit = new ucActivityConvertUnitWeightEdit(convertUnitDetail);

                                if (await activityConvertUnitWeightEdit.ShowDialog() == DialogResult.OK &&
                                    activityConvertUnitWeightEdit.WeightHasChanged)
                                {
                                    convertUnitDetail.ActivityDetailWeightQuantity = activityConvertUnitWeightEdit.EditedWeight;
                                    await this.DisplayData();
                                    if (this.ActDetail.ListDetails == null)
                                    {
                                        _ = await this.ActDetail.GetActivityDetailList(false);
                                    }

                                    await this.DesignObservableCollection();
                                }
                            }
                        }
                    }
                });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                _ = await this.CancelChanges();
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// The OnDisappearing.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        private BarCodeScanner Scanner { get; set; }

        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.ConvertUnit;

        private ActDetail ActDetail { get; set; } = null;

        private ActItem ActItem { get; set; } = null;

        private Controller.Activity.Activity Activity { get; set; } = null;

        private LocationMap LocMap { get; set; }

        private SysEnvironment SysEnv { get; set; }

        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        private bool IsControlLoaded { get; set; } = false;

        private bool IsChangesMade { get; set; } = false;

        private int FromProdUnitRefno { get; set; } = 0;

        private int ToProdUnitRefno { get; set; } = 0;

        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        private bool FirstAppearance { get; set; } = true;

        /// <summary>
        /// Un-register the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
            }

            this.IsBarCodeHandlerRegistered = false;
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        /// Must have a try catch block.</remarks>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = true;

            try
            {
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    throw new MessageOnlyException("Location must be set.");
                }

                if (this.FromProdUnitRefno <= 0)
                {
                    throw new MessageOnlyException("Convert from product must be set.");
                }

                if (this.ActItem.ListItems.Count <= 0)
                {
                    throw new MessageOnlyException("No convert to items have be defined.");
                }

                if (this.ActDetail.ListDetails.Count <= 0)
                {
                    throw new MessageOnlyException("No convert to item details have be defined.");
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = true;

            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initialize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
                {
                    await this.ScannerDisable();
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            return "Convert Unit";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.FromProdUnitRefno != 0 && this.ActItem.ListItems.Count != 0 && this.ActDetail.ListDetails.Count != 0)
                    {
                        var dataReq = SysCommon.SubmitComm(await this.MakeConvertUnitSubmitString());

                        if (dataReq.Length > 0)
                        {
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(dataReq);

                            if (dataReq.Contains("<message>ERROR</message>"))
                            {
                                var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

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
                                if (this.SysEnv.SysSettings.CanPrintProductLabelConvertUnits)
                                {
                                    if (await Device.InvokeOnMainThreadAsync(async () =>
                                            await this.DisplayAlert(
                                                "Unit Conversion",
                                                "Unit conversion complete." + Environment.NewLine + "Do you wish to print product labels?",
                                                "Yes",
                                                "No")))
                                    {
                                        for (var idx = 0; idx < this.ActDetail.ListDetails.Count; idx++)
                                        {
                                            dataReq = SysCommon.SubmitComm(await this.MakeProductLabelsSubmitString(idx));

                                            if (dataReq.Contains("<message>ERROR</message>"))
                                            {
                                                xmlDoc.LoadXml(dataReq);
                                                var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

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
                                else
                                {
                                    await Device.InvokeOnMainThreadAsync(async () =>
                                        await this.DisplayAlert("Unit Conversion", "Unit conversion complete.", "OK"));
                                }
                            }

                            await this.ClearContents();
                        }

                        saveComplete = true;
                        this.IsChangesMade = false;
                        if (await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    "Convert Unit(s)",
                                    "Do you wish to convert any more unit(s)?",
                                    "Yes", "No")))
                        {
                            saveComplete = false;
                        }
                    }
                    else
                    {
                        if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Convert Unit(s)",
                                "Location must be set.",
                                "OK"));
                        }
                        else if (this.ActItem.ListItems.Count == 0)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Convert Unit(s)",
                                "Convert from unit must be set.",
                                "OK"));
                        }
                        else if (this.ActDetail.ListDetails.Count == 0)
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Convert Unit(s)",
                                "Convert to unit(s) must be set.",
                                "OK"));
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    // this.ProdUnitRefno = 0;

                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    // ClearProperties();
                    // DisplayData();
                    // this.LoadData();
                    cancelChanges = false;
                    // this.IsChangesMade = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task<XDocument> GetXML()
        {
            var xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmlDoc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task DisplayData()
        {
            if (this.ActItem.ListItems.Count > 0)
            {
                var fromProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);

                // TODO: Test selected listItem value is correct
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.lblFullUnitInfo.Text = string.Format("Qty {0} {1} @ {2} {3}", (this.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity,
                                                fromProdInfo.UnitCode,
                                                Math.Round(Convert.ToDecimal((this.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity), 3),
                                                fromProdInfo.WeightUnitCode);
                });
            }
            else
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.lblFullUnitInfo.Text = "Qty 0 UOM @ 0 KG/LB");
            }

            await this.DisplayDataConvertedUnitInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task DesignObservableCollection()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.MainActivityItems = this.ActDetail.DesignGrid(ref this.GridMainActivity, ref this.dgUnitWeight);
                this.dgUnitWeight.ItemsSource = this.MainActivityItems;

                if (this.MainActivityItems.Count > 0)
                {
                    this.dgUnitWeight.SelectedItem = this.MainActivityItems[0];
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading || scanArgs == null)
            {
                return;
            }

            await Task.Run(async () =>
            {
                this.Loading = true;
                BarCodeParse parse = null;
                var tempItemProdSchemeData = new SActItemSchemeData();

                try
                {
                    await this.ScannerDisable();

                    parse = scanArgs.ParseInfo;

                    // GLOBAL LOCATION IDENTIFIER SCANNED
                    if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation());
                        }
                        else
                        {
                            _ = await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = "Set Location");
                        }
                    }
                    else
                    {
                        // Activity item fk refno
                        if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                        {
                            throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
                        }

                        // Activity detail fk refno
                        if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                        {
                            throw new MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                        }

                        // Shipping container scanned, skid
                        if (!string.IsNullOrEmpty(parse.SSCC18))
                        {
                            throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
                        }

                        // Product identifier
                        if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                        {
                            if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) && !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                            {
                                throw new MessageOnlyException("Location Must Be Set First");
                            }

                            await this.ClearContents();

                            var tempFromProdUnitRefno = this.FromProdUnitRefno;

                            tempFromProdUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);

                            this.FromProdUnitRefno = tempFromProdUnitRefno;
                            this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                            this.ActItem.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);

                            // Insert a new activity detail record
                            this.ActDetail.ActItem = this.ActItem;

                            var isInsertReady = false;

                            this.ActDetail.Quantity = 0;
                            this.ActDetail.BaseQuantity = 0;
                            this.ActDetail.WeightQuantity = 0;
                            this.ActDetail.SchemeID = string.Empty;
                            this.ActDetail.UniqueSchemeID = string.Empty;
                            this.ActDetail.ActExtra.Text1 = string.Empty;
                            this.ActDetail.ActExtra.Text2 = string.Empty;
                            this.ActDetail.ActExtra.Text3 = string.Empty;
                            this.ActDetail.ActExtra.Text4 = string.Empty;
                            this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                            isInsertReady = await SysShared.GatherProductData(parse, this.ActSubType, this.ActItem, tempItemProdSchemeData, isInsertReady, this.ActDetail, false);

                            var xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                            await this.ActItem.Insert(xmlDoc, false);

                            await this.DisplayData();

                            if (this.FromProdUnitRefno > 0)
                            {
                                var query = "SELECT tProduct.ProductCode," +
                                            " tProductUnit.ProductUnitBC," +
                                            " tUnit.UnitCode" +
                                            " FROM tProductUnit" +
                                            " INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno" +
                                            " INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno" +
                                            " WHERE tProductUnit.ProductUnitRefno = ?";

                                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ProductUnit>(query, Convert.ToString(this.FromProdUnitRefno));

                                if (results?.Count > 0)
                                {
                                    _ = await Device.InvokeOnMainThreadAsync(() => this.cmdSelectFromUnit.Text = results[0].ProductCode + " " + results[0].ProductUnitBC + " " + results[0].UnitCode);
                                }
                            }
                        }
                    }
                }
                catch (TaskCanceledException ex)
                {
                    // Do nothing, task cancelled by user
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errorReport = new FormErrorReport(this.ActSubType, ex);
                        _ = await errorReport.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                    this.Loading = false;
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task DisplayDataConvertedUnitInfo()
        {
            if (this.ActDetail.ListDetails.Count > 0)
            {
                decimal qty = 0;
                decimal wtQty = 0;

                for (var item = 0; item <= this.ActDetail.ListDetails.Count - 1; item++)
                {
                    // TODO: Test and determine type
                    qty += Convert.ToDecimal((this.ActDetail.ListDetails[item] as dynamic).ActivityDetailQuantity);
                    wtQty += Convert.ToDecimal((this.ActDetail.ListDetails[item] as dynamic).ActivityDetailWeightQuantity);
                }

                var toProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);
                _ = await Device.InvokeOnMainThreadAsync(() => this.lblConvertedUnitInfo.Text = string.Format("Qty {0} {1} @ {2} {3}", Math.Round(qty, 3), toProdInfo.UnitCode, Math.Round(wtQty, 3), toProdInfo.WeightUnitCode));
            }
            else
            {
                _ = await Device.InvokeOnMainThreadAsync(() => this.lblConvertedUnitInfo.Text = "Converted Qty 0 UOM @ 0 KG/LB");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                // buttons ignore cancel = true.
                if (sender == this.cmdLocation)
                {
                }

                if (sender == this.cmdSelectFromUnit)
                {
                }

                if (sender == this.cmdSelectToUnit)
                {
                }

                if (sender == this.BarCodeModifier1Picker)
                {
                }

                if (sender == this.dgUnitWeight)
                {
                }
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = true;
                _ = Device.InvokeOnMainThreadAsync(async () =>
                      await Application.Current.MainPage.DisplayAlert(
                          this.GetControlDescription(),
                          ex.Message,
                          Resource.OK));
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task ClearContents()
        {
            this.FromProdUnitRefno = 0;
            this.ToProdUnitRefno = 0;
            await this.ActItem.Delete();
            await this.ActDetail.Delete();

            await Device.InvokeOnMainThreadAsync(() =>
            {
                this.cmdSelectFromUnit.Text = "Select From Unit";
                this.cmdSelectToUnit.Text = "Select To Unit";
                this.lblFullUnitInfo.Text = "Qty 0 UOM @ KG/LB";
                this.lblConvertedUnitInfo.Text = "Qty 0 UOM @ KG/LB";
            });

            if (this.ActDetail.ListDetails == null)
            {
                _ = await this.ActDetail.GetActivityDetailList(false);
            }

            await this.DesignObservableCollection();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private async Task<string> MakeProductLabelsSubmitString(int idx)
        {
            var prnSettings = new PrinterSettings().LoadData();
            var production = string.Empty;
            var bestBefore = string.Empty;
            var prodInfo = new ProductInfo();

            prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(this.ActDetail.ListDetails[idx].GetType().GetProperty("GetType")?.GetValue(this.ActDetail.ListDetails[idx])));
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")?.GetValue(this.ActDetail.ListDetails[idx])).ToString("MM /dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")?.GetValue(this.ActDetail.ListDetails[idx])).ToString("MM/dd/yyyy");
            }

            var req = new StringBuilder();
            _ = req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            _ = req.Append("<imclient>");
            _ = req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            _ = req.Append(await SysCommon.GetRequestDefaults(false));
            _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            _ = req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.HdrFKRefno);
            _ = req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFKRefno);
            _ = req.AppendFormat("<prod_id>{0}</prod_id>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ProductCode")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<bc>{0}</bc>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ProductUnitBC")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<alt_unit>{0}</alt_unit>", this.ActDetail.ListDetails[idx].GetType().GetProperty("UnitCode")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            _ = req.AppendFormat("<serial>{0}</serial>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<pdate>{0}</pdate>", production);
            _ = req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            _ = req.AppendFormat("<lot_no>{0}</lot_no>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<variant_code>{0}</variant_code>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<variant_no>{0}</variant_no>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ProductSchemeVariantNumber")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<alt_qty>{0}</alt_qty>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<wt_qty>{0}</wt_qty>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<base_qty>{0}</base_qty>", this.ActDetail.ListDetails[idx].GetType().GetProperty("ActivityDetailBaseQuantity").GetValue(this.ActDetail.ListDetails[idx]));
            _ = req.AppendFormat("<{0}>{1}</{0}>", "mobile_printer_name", prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty);
            _ = req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<string> MakeConvertUnitSubmitString()
        {
            var production = string.Empty;
            var bestBefore = string.Empty;
            var prodInfo = new ProductInfo();
            var detail = this.ActDetail.ListDetails;
            var items = this.ActDetail.ActItem.ListItems;

            prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(items[0].GetType().GetProperty("ProductUnitRefno")?.GetValue(items[0])));
            prodInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(items[0].GetType().GetProperty("ActivityDetailLogisticUnitBestBeforeSellByDate")?.GetValue(items[0])).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(items[0].GetType().GetProperty("ActivityDetailLogisticUnitProductionDate")?.GetValue(items[0])).ToString("MM/dd/yyyy");
            }

            var req = new StringBuilder();
            _ = req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            _ = req.Append("<imclient>");
            _ = req.Append("<command><action>SUBMIT CONVERT UNIT</action></command>");
            _ = req.Append(await SysCommon.GetRequestDefaults(false));
            _ = req.Append("<from>");
            _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            _ = req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.HdrFKRefno);
            _ = req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFKRefno);
            _ = req.AppendFormat("<prod_id>{0}</prod_id>", items[0].GetType().GetProperty("ProductCode")?.GetValue(items[0]));
            _ = req.AppendFormat("<bc>{0}</bc>", items[0].GetType().GetProperty("ProductUnitBC")?.GetValue(items[0]));
            _ = req.AppendFormat("<alt_unit>{0}</alt_unit>", items[0].GetType().GetProperty("UnitCode")?.GetValue(items[0]));
            _ = req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            _ = req.AppendFormat("<serial>{0}</serial>", items[0].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(items[0]));
            _ = req.AppendFormat("<pdate>{0}</pdate>", production);
            _ = req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            _ = req.AppendFormat("<lot_no>{0}</lot_no>", items[0].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(items[0]));
            _ = req.AppendFormat("<variant_code>{0}</variant_code>", items[0].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(items[0]));
            _ = req.AppendFormat("<variant_no>{0}</variant_no>", items[0].GetType().GetProperty("ProductSchemeVariantNumber")?.GetValue(items[0]));
            _ = req.AppendFormat("<alt_qty>{0}</alt_qty>", items[0].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(items[0]));
            _ = req.AppendFormat("<wt_qty>{0}</wt_qty>", items[0].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(items[0]));
            _ = req.AppendFormat("<base_qty>{0}</base_qty>", items[0].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(items[0]));
            _ = req.AppendFormat("<base_unit>{0}</base_unit>", prodInfo.BaseUnitCode);
            _ = req.Append("</from>");

            for (var idx = 0; idx <= this.ActDetail.ListDetails.Count - 1; idx++)
            {
                var toProdInfo = new ProductInfo();
                toProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(detail[idx].GetType().GetProperty("ProductUnitRefno")?.GetValue(detail[idx])));

                _ = req.Append("<to>");
                _ = req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
                _ = req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
                _ = req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
                _ = req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.HdrFKRefno);
                _ = req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.ItemFKRefno);
                _ = req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx].GetType().GetProperty("ProductCode")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<bc>{0}</bc>", detail[idx].GetType().GetProperty("ProductUnitBC")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx].GetType().GetProperty("UnitCode")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
                _ = req.AppendFormat("<serial>{0}</serial>", detail[idx].GetType().GetProperty("ActivityDetailSingleTradeItemSerialNumber")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<pdate>{0}</pdate>", production);
                _ = req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                _ = req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx].GetType().GetProperty("ActivityDetailLogisticUnitBatchLotNumber")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx].GetType().GetProperty("ProductSchemeVariantCode")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx].GetType().GetProperty("ProductSchemeVariantNumber")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx].GetType().GetProperty("ActivityDetailQuantity")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx].GetType().GetProperty("ActivityDetailWeightQuantity")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx].GetType().GetProperty("ActivityDetailBaseQuantity")?.GetValue(detail[idx]));
                _ = req.AppendFormat("<base_unit>{0}</base_unit>", toProdInfo.BaseUnitCode);
                _ = req.Append("</to>");
            }

            _ = req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();
            var form = new FormDialogContainer(EDialogTypes.SelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = "Set Location";
                }
            }

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            try
            {
                this.cmdSelectFromUnit.IsEnabled = false;
                await this.ScannerDisable();
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    throw new MessageOnlyException("Your location must be set.");
                }

                var frmSelectProduct =
                    new FormDialogContainer(EDialogTypes.SelectProductUnit, SysCommon.ActSubTypes.ConvertUnit);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await frmSelectProduct.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    throw new CancelOperationException();
                }

                await this.ClearContents();
                var xmlDocProduct = await frmSelectProduct.GetXML();
                this.FromProdUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);
                if (this.FromProdUnitRefno == 0)
                {
                    throw new CancelOperationException();
                }

                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                this.ActItem.ProdInfo.Scheme =
                    await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);
                var detail = new dMobile.Resources.Types.ActivityDetail();
                await this.ActDetail.AssignThisFromDetailAsync(detail, true);

                this.ActDetail.Quantity = 0;
                this.ActDetail.BaseQuantity = 0;
                this.ActDetail.WeightQuantity = 0;
                this.ActDetail.SchemeID = string.Empty;
                this.ActDetail.UniqueSchemeID = string.Empty;
                this.ActDetail.ActExtra.Text1 = string.Empty;
                this.ActDetail.ActExtra.Text2 = string.Empty;
                this.ActDetail.ActExtra.Text3 = string.Empty;
                this.ActDetail.ActExtra.Text4 = string.Empty;
                this.ActDetail.ActExtra.DateTime1 = new DateTime(1980, 1, 1);

                var form = new FormDialogContainer(EDialogTypes.ActivityDetailEdit, this.ActDetail, SysCommon.ActSubTypes.ConvertUnit);
                if (await form.DisplayControl.ShowDialog() != DialogResult.OK)
                {
                    throw new CancelOperationException();
                }

                var xmlDoc = await form.GetXML();
                await this.ActItem.Insert(xmlDoc, false);
                var query = "SELECT tProduct.ProductCode,"
                            + "tProductUnit.ProductUnitBC,"
                            + "tUnit.UnitCode "
                            + "FROM tProductUnit "
                            + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                            + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                            + "WHERE tProductUnit.ProductUnitRefno = ?";

                var results = await DatabaseConnection.GetInstance().SQLConn
                    .QueryAsync<ProductUnit>(query, this.FromProdUnitRefno);
                if (results?.Count > 0)
                {
                    _ = await Device.InvokeOnMainThreadAsync(() =>
                        this.cmdSelectFromUnit.Text = Convert.ToString(results[0].ProductCode).TrimEnd() +
                                                      (" " + Convert.ToString(results[0].ProductUnitBC))
                                                      .TrimEnd() + " " + Convert.ToString(results[0].UnitCode));

                    await this.DisplayData();
                }
            }
            catch (CancelOperationException)
            {
            }
            catch (SQLiteException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.cmdSelectFromUnit.IsEnabled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSelectToUnit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    throw new MessageOnlyException("Your location must be set.");
                }

                if (this.FromProdUnitRefno == 0 || this.ActItem.ListItems.Count <= 0)
                {
                    throw new MessageOnlyException("Please set the from product unit first.");
                }

                await this.ScannerDisable();
                await SetFromProdInfo();
                await SelectToProductUnit();
                await SetDetailQuantity();
                SetDetailScheme();

                var form = new FormDialogContainer(
                    EDialogTypes.ActivityConvertUnitEdit,
                    this.ActDetail,
                    SysCommon.ActSubTypes.ConvertUnit);
                if (await form.DisplayControl.ShowDialog() != DialogResult.OK)
                {
                    throw new CancelOperationException();
                }

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    this.cmdSelectToUnit.Text = string.Format("{0} {1}\'S ({2} per {3})",
                        this.ActItem.ProdInfo.UnitBC,
                        this.ActItem.ProdInfo.UnitCode,
                        Math.Round(this.ActDetail.Quantity, 3),
                        Convert.ToString((this.ActItem.ListItems[0] as dynamic).UnitCode));
                });

                var xmlDoc = await form.GetXML();

                // Clear the detail list.
                await this.ActDetail.Delete();
                this.ActDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
                this.ActDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
                this.ActDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);
                if (bool.Parse(xmlDoc.Root.Element("split").Value))
                {
                    if (this.ActItem.ProdInfo.UnitCode == "LB" ||
                        this.ActItem.ProdInfo.UnitCode == "KG")
                    {
                        ////await this.ActDetail.Insert(xmlDoc);
                        var detail = this.ActDetail.InsertListDetail(
                            this.MainActivityItems);
                        ////this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFKRefno;
                    }
                    else
                    {
                        var altQty = this.ActDetail.Quantity;
                        var baseQty = this.ActDetail.BaseQuantity;
                        var wt = this.ActDetail.WeightQuantity;
                        var wtQty = altQty == 0 ? wt : wt / altQty;

                        for (var unitQty = 1; unitQty <= altQty; unitQty++)
                        {
                            this.ActDetail.Quantity = 1;
                            this.ActDetail.WeightQuantity = wtQty;

                            if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB" ||
                                this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
                            {
                                this.ActDetail.BaseQuantity = this.ActDetail.WeightQuantity;
                            }
                            else
                            {
                                if (this.ActDetail.Quantity != 0 &&
                                    this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                                {
                                    this.ActDetail.BaseQuantity = this.ActDetail.Quantity *
                                        this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                                }
                                else
                                {
                                    this.ActDetail.BaseQuantity = this.ActDetail.Quantity;
                                }
                            }

                            ////await this.ActDetail.Insert(xmlDoc);
                            var detail = this.ActDetail.InsertListDetail(this.MainActivityItems);
                            ////this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFKRefno;
                        }
                    }
                }
                else
                {
                    ////await this.ActDetail.Insert(xmlDoc);
                    var detail = this.ActDetail.InsertListDetail(
                        this.MainActivityItems);
                    ////this.SelectedActivityDetailFKRefno = this.ActDetail.DetailFKRefno;
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await this.DisplayData();
                    if (this.ActDetail.ListDetails == null)
                    {
                        _ = await this.ActDetail.GetActivityDetailList(false);
                    }

                    await this.DesignObservableCollection();
                });
            }
            catch (CancelOperationException)
            {
            }
            catch (SQLiteException sqlEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, sqlEx);
                    _ = await errorReport.ShowDialog();
                });
            }
            catch (MessageOnlyException msgEx)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.GetControlDescription(),
                        msgEx.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errorReport = new FormErrorReport(this.ActSubType, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }

            //////////////////////////////////////////////////////////////////////////
            async Task SetFromProdInfo()
            {
                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                this.ActItem.ProdInfo.Scheme =
                    await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);
            }

            //////////////////////////////////////////////////////////////////////////
            void SetDetailScheme()
            {
                this.ActDetail.SchemeID =
                    Convert.ToString((this.ActItem.ListItems[0] as dynamic).ActivityDetailSchemeID);
                this.ActDetail.UniqueSchemeID =
                    Convert.ToString((this.ActItem.ListItems[0] as dynamic).ActivityDetailUniqueSchemeID);

                var prodSchemeData = new SProductSchemeData
                {
                    LogisticUnitBestBeforeSellByDate = Convert.ToDateTime((this.ActItem.ListItems[0] as dynamic)
                        .ActivityDetailLogisticUnitBestBeforeSellByDate),
                    LogisticUnitProductionDate = Convert.ToDateTime((this.ActItem.ListItems[0] as dynamic)
                        .ActivityDetailLogisticUnitProductionDate),
                    VariantNumber =
                        Convert.ToInt32((this.ActItem.ListItems[0] as dynamic).ProductSchemeVariantNumber),
                    LogisticUnitBatchLotNumber = Convert.ToString((this.ActItem.ListItems[0] as dynamic)
                        .ActivityDetailLogisticUnitBatchLotNumber),
                    LogisticUnitSkidNumber = string.Empty,
                    SingleTradeItemSerialNumber = Convert.ToString((this.ActItem.ListItems[0] as dynamic)
                        .ActivityDetailSingleTradeItemSerialNumber),
                    VariantCode =
                        Convert.ToString((this.ActItem.ListItems[0] as dynamic).ProductSchemeVariantCode),
                };

                this.ActDetail.SchemeData = prodSchemeData;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task SetDetailQuantity()
            {
                SFillQuantity fillQty = await this.RequestConvertUnitResult(
                    this.FromProdUnitRefno,
                    this.ToProdUnitRefno,
                    Convert.ToDecimal((this.ActItem.ListItems[0] as dynamic).ActivityDetailQuantity),
                    Convert.ToDecimal((this.ActItem.ListItems[0] as dynamic).ActivityDetailWeightQuantity));

                this.ActDetail.Quantity = fillQty.Quantity;
                this.ActDetail.BaseQuantity = fillQty.BaseQuantity;
                this.ActDetail.WeightQuantity = fillQty.WeightQuantity;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task SelectToProductUnit()
            {
                var frmSelectProduct = new FormDialogContainer(
                    EDialogTypes.SelectProductUnitFilterToProduct,
                    this.ActItem,
                    SysCommon.ActSubTypes.ConvertUnit);
                if (await Device.InvokeOnMainThreadAsync(async () =>
                        await frmSelectProduct.DisplayControl.ShowDialog() != DialogResult.OK))
                {
                    throw new CancelOperationException();
                }

                var xmlDocProduct = await frmSelectProduct.GetXML();
                this.ToProdUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                if (this.ToProdUnitRefno == 0)
                {
                    throw new MessageOnlyException("A to product unit needs to be selected.");
                }

                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);
                this.ActItem.ProdInfo.Scheme =
                    await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ActItem.ProdInfo.Refno);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fromProdUnitRefno"></param>
        /// <param name="toProdUnitRefno"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private async Task<SFillQuantity> RequestConvertUnitResult(int fromProdUnitRefno, int toProdUnitRefno, decimal quantity, decimal weightQuantity)
        {
            var fillQty = new SFillQuantity
            {
                Quantity = quantity,
            };

            var request = await this.CreateConvertUnitResultXML(fromProdUnitRefno, toProdUnitRefno, quantity, weightQuantity);
            var dataReq = SysCommon.SubmitComm(request);

            if (dataReq.Length > 0)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(dataReq);

                if (dataReq.Contains("<message>ERROR</message>"))
                {
                    var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }

                if (xmlDoc.SelectSingleNode("/VFPData/Quantity").HasChildNodes)
                {
                    fillQty.Quantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/Quantity").FirstChild.Value);
                }

                if (xmlDoc.SelectSingleNode("/VFPData/BaseQuantity").HasChildNodes)
                {
                    fillQty.BaseQuantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/BaseQuantity").FirstChild.Value);
                }

                if (xmlDoc.SelectSingleNode("/VFPData/WeightQuantity").HasChildNodes)
                {
                    fillQty.WeightQuantity = Convert.ToDecimal(xmlDoc.SelectSingleNode("/VFPData/WeightQuantity").FirstChild.Value);
                }
            }

            return fillQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private async Task<string> CreateConvertUnitResultXML(int fromProdUnitRefno, int toProdUnitRefno, decimal quantity, decimal weightQuantity)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);
            var fromProd = await ProductInfoTasks.GetInfoFromUnitRefno(fromProdUnitRefno);
            var toProd = await ProductInfoTasks.GetInfoFromUnitRefno(toProdUnitRefno);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CONVERT UNIT RESULT</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + fromProd.Code + "</prod_id>"
                + "<from_bc>" + fromProd.UnitBC + "</from_bc>"
                + "<from_alt_qty>" + quantity + "</from_alt_qty>"
                + "<from_alt_unit>" + fromProd.UnitCode + "</from_alt_unit>"
                + "<from_wt_qty>" + weightQuantity + "</from_wt_qty>"
                + "<to_bc>" + toProd.UnitBC + "</to_bc>"
                + "<to_alt_unit>" + toProd.UnitCode + "</to_alt_unit>"
                + "<to_base_unit>" + toProd.BaseUnitCode + "</to_base_unit>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityConvertUnit_GotFocus(object sender, EventArgs e)
        {
            await this.ScannerEnable();

            if (!this.IsControlLoaded)
            {
                this.BarCodeModifier1Picker.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    this.cmdLocation.Text = "Set Location";
                }

                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.IsChangesMade = true;

                if (sender == this.BarCodeModifier1Picker)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.BarCodeModifier1Picker.SelectedItem);
                }
            }
        }

        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                await this.Close();
            }
        }

        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            _ = await this.CancelChanges();
            await this.Close();
        }

        private void dgUnitWeight_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _ = Device.InvokeOnMainThreadAsync(async () =>
                await Application.Current.MainPage.DisplayAlert(
                    this.GetControlDescription(),
                    "Item Tapped",
                    Resource.OK));
        }
    }
}