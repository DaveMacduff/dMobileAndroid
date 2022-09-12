// <copyright file="ActivityItem.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;
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
    using dMobile.View.CustomPages.Modal;
    using dMobile.View.Forms;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Activity items control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityItem : DMobileModal, IAppArea, IDataLayerEventListener
    {
        /// <summary>
        /// Define the warehouse map location text information.
        /// </summary>
        private string cmdLocationText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the location selection is enabled.
        /// </summary>
        private bool cmdLocationIsEnabled = false;

        /// <summary>
        /// Define the activity filter text information.
        /// </summary>
        private string cmdFilterText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the filter selection is enabled.
        /// </summary>
        private bool cmdFilterIsEnabled = false;

        /// <summary>
        /// Define the select skid text information.
        /// </summary>
        private string cmdSelectSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether the skid selection is enabled.
        /// </summary>
        private bool cmdSelectSkidIsEnabled = false;

        /// <summary>
        /// Define the add a product text information.
        /// </summary>
        private string cmdAddProductText = string.Empty;

        /// <summary>
        /// Define a value indicating whether adding a product is enabled.
        /// </summary>
        private bool cmdAddProductIsEnabled = false;

        /// <summary>
        /// Define the add skid text information.
        /// </summary>
        private string cmdAddSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether adding a skid is enabled.
        /// </summary>
        private bool cmdAddSkidIsEnabled = false;

        /// <summary>
        /// Define the clear selected skid text information.
        /// </summary>
        private string cmdClearSelectedSkidText = string.Empty;

        /// <summary>
        /// Define a value indicating whether clear selected a skid is enabled.
        /// </summary>
        private bool cmdClearSelectedSkidIsEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityItem"/> class.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> Activity header instance.</param>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubType"/> Activity sub type.</param>
        public ActivityItem(Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            this.CmdLocationText = "Set Location";
            this.CmdFilterText = "Filter";
            this.CmdSelectSkidText = "Select Skid";
            this.CmdClearSelectedSkidText = "Clear Skid";
            this.CmdAddProductText = "Add Prod";
            this.CmdAddSkidText = "Add Skid";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.LocMap = LocationMap.GetInstance();
                this.SysEnv = SysEnvironment.GetInstance();
                this.ActSubType = actSubTypes;
                this.ActItem = ActItemFacade.GetActItem(activity, this.ActSubType);
                this.ActItem.ActHdr = activity;

                this.CmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;

                // If the skid was not set on the header activity define an empty one.
                activity.SkidInfo ??= new SysSkidInfo();

                ParseAndSeedBarCodeModifier();

                // Page title.
                this.Title = this.ActItem.GetControlDescription();
            }

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
        /// Gets or sets the selected activity item foreign key reference number.
        /// </summary>
        private int SelectedActivityItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether to show a data valid message or raise an error.
        /// </summary>
        private bool IsShowDataValidMsg { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether data changes have been made.
        /// </summary>
        private bool IsChangesMade { get; set; }

        /// <summary>
        /// Gets or sets the warehouse location map instance.
        /// </summary>
        private LocationMap LocMap { get; set; }

        /// <summary>
        /// Gets or sets the activity item instance.
        /// </summary>
        private ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        private SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets a value indicating whether the bar code handler has been registered.
        /// </summary>
        private bool IsBarCodeHandlerRegistered { get; set; } = false;

        /// <summary>
        /// Gets or sets the system environment instance.
        /// </summary>
        private SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets or sets the bar code scanner instance.
        /// </summary>
        private BarCodeScanner Scanner { get; set; } = ScannerEngine.GetInstance().Scanner;

        /// <summary>
        /// Gets or sets a list of extra information items.
        /// </summary>
        private ObservableCollection<string> ExtraInfoItems { get; set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets a list of list view activity items.
        /// </summary>
        private ObservableCollection<object> MainActivityItems { get; set; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the options menu items list.
        /// </summary>
        private List<string> OptionsMenuItems { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to performing loading data upon got focus.
        /// </summary>
        private bool IsFirstAppearance { get; set; } = true;

        /// <summary>
        /// Gets or sets a counter that tracks the number of screen taps, for double click.
        /// </summary>
        private int NumTaps { get; set; } = 0;

        /// <summary>
        /// Gets or sets the sender for tracking the number of screen taps, for double click.
        /// </summary>
        private object PreviousSender { get; set; }

        /// <summary>
        /// Gets or sets the command location text.
        /// </summary>
        private string CmdLocationText
        {
            get => this.cmdLocationText;
            set
            {
                this.cmdLocationText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether command location is enabled.
        /// </summary>
        private bool CmdLocationIsEnabled
        {
            get => this.cmdLocationIsEnabled;
            set
            {
                this.cmdLocationIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the command filter text.
        /// </summary>
        private string CmdFilterText
        {
            get => this.cmdFilterText;
            set
            {
                this.cmdFilterText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command filter is enabled.
        /// </summary>
        private bool CmdFilterIsEnabled
        {
            get => this.cmdFilterIsEnabled;
            set
            {
                this.cmdFilterIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the command select skid text.
        /// </summary>
        private string CmdSelectSkidText
        {
            get => this.cmdSelectSkidText;
            set
            {
                this.cmdSelectSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command select skid is enabled.
        /// </summary>
        private bool CmdSelectSkidIsEnabled
        {
            get => this.cmdSelectSkidIsEnabled;
            set
            {
                this.cmdSelectSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the command add product text.
        /// </summary>
        private string CmdAddProductText
        {
            get => this.cmdAddProductText;
            set
            {
                this.cmdAddProductText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command add product is enabled.
        /// </summary>
        private bool CmdAddProductIsEnabled
        {
            get => this.cmdAddProductIsEnabled;
            set
            {
                this.cmdAddProductIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the command add skid text.
        /// </summary>
        private string CmdAddSkidText
        {
            get => this.cmdAddSkidText;
            set
            {
                this.cmdAddSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command add skid is enabled.
        /// </summary>
        private bool CmdAddSkidIsEnabled
        {
            get => this.cmdAddSkidIsEnabled;
            set
            {
                this.cmdAddSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the command clear selected skid text.
        /// </summary>
        private string CmdClearSelectedSkidText
        {
            get => this.cmdClearSelectedSkidText;
            set
            {
                this.cmdClearSelectedSkidText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command clear selected skid is enabled.
        /// </summary>
        private bool CmdClearSelectedSkidIsEnabled
        {
            get => this.cmdClearSelectedSkidIsEnabled;
            set
            {
                this.cmdClearSelectedSkidIsEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get the control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public string GetControlDescription()
        {
            return "Activity Item";
        }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerDisable()
        {
            if (this.Scanner != null)
            {
                if (this.IsBarCodeHandlerRegistered)
                {
                    this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    await this.Scanner.Stop();
                    this.IsBarCodeHandlerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerEnable()
        {
            if (this.Scanner != null)
            {
                if (!this.IsBarCodeHandlerRegistered)
                {
                    await this.Scanner.Start();
                    this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                    this.IsBarCodeHandlerRegistered = true;
                }
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task ScannerStart()
        {
            if (this.Scanner != null)
            {
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.IsBarCodeHandlerRegistered = true;
            }
        }

        /// <summary>
        /// Check if the activity item is ready to save.
        /// </summary>
        /// <returns><see cref="bool"/> True if it is ready to save, else false.</returns>
        public async Task<bool> IsReadyToSave()
        {
            var isReadyToSave = !this.IsChangesMade;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                    this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    isReadyToSave = await IsReadyToSavePicking();
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToSave;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToSavePicking()
            {
                try
                {
                    this.ItemQuantityUnderCheck();

                    // Added extra check for Lekker, check all items.
                    this.AllItemsQuantityUnderCheck();
                }
                catch (MessageOnlyException ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                    {
                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                            if (await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    "Quantity Validation",
                                    ex.Message + Environment.NewLine + "Continue?",
                                    "Yes",
                                    "No") == false))
                            {
                                isReadyToSave = false;
                                this.IsShowDataValidMsg = false;
                            }

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:

                            await Device.InvokeOnMainThreadAsync(
                                async () => await this.DisplayAlert(
                                    "Quantity Validation",
                                    ex.Message,
                                    "OK"));
                            isReadyToSave = false;
                            this.IsShowDataValidMsg = false;

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                            break;

                        default:

                            throw new NotImplementedException(
                                "Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                    }
                }
                catch
                {
                    throw;
                }

                return isReadyToSave;
            }
        }

        /// <summary>
        /// Check if the activity item is ready to close.
        /// </summary>
        /// <returns><see cref="bool"/> True if the activity item is ready to close, else false.</returns>
        public async Task<bool> IsReadyToClose()
        {
            var isReadyToClose = !this.IsChangesMade;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                // Prairie meats like Lekker requirement to check quantity under even though we are
                // canceling / closing.
                // See WO# 12080
                if (isReadyToClose)
                {
                    if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                        this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        isReadyToClose = await IsReadyToClosePicking(isReadyToClose);
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> IsReadyToClosePicking(bool isReadyToClose)
            {
                try
                {
                    this.IsShowDataValidMsg = true;
                    this.ItemQuantityUnderCheck();

                    // Added extra check for Lekker, check all items.
                    this.AllItemsQuantityUnderCheck();
                }
                catch (MessageOnlyException ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                    {
                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:

                            if (await Device.InvokeOnMainThreadAsync(
                                async () => await this.DisplayAlert(
                                    "Quantity Validation",
                                    ex.Message + Environment.NewLine + "Continue?",
                                    "Yes",
                                    "No") == false))
                            {
                                isReadyToClose = false;
                                this.IsShowDataValidMsg = false;
                            }

                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert("Quantity Validation", ex.Message, "OK"));
                            isReadyToClose = false;
                            this.IsShowDataValidMsg = false;
                            break;

                        case SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning:
                            break;

                        default:
                            throw new NotImplementedException(
                                "Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                    }
                }
                catch
                {
                    throw;
                }

                return isReadyToClose;
            }
        }

        /// <summary>
        /// Save any activity item data changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the save was successful, else false.</returns>
        public async Task<bool> SaveChanges()
        {
            var isSaveComplete = false;

            // NOTE: Do not need to set scanner off or on. Caller will handle.
            try
            {
                if (await this.IsReadyToSave())
                {
                    ////ClearContents();

                    // Print all receiving skid labels.
                    if (this.ActSubType == SysCommon.ActSubTypes.Receiving &&
                        !this.SysEnv.SysSettings.IsDisconnectedMode &&
                        this.SysEnv.SysSettings.IsSkidTrackingModule &&
                        this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
                    {
                        await PrintSkidLabels();
                    }

                    this.IsChangesMade = false;
                    isSaveComplete = true;
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all
                        // in the event that IsReadyToSave just returns false.
                        throw new MessageOnlyException(
                            "Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (MessageOnlyException ex)
            {
                isSaveComplete = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                isSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return isSaveComplete;

            //////////////////////////////////////////////////////////////////////////
            async Task PrintSkidLabels()
            {
                if (!await CheckIfSkids2Print())
                {
                    return;
                }

                if (!await this.DisplayAlert(
                    "Skid Labels",
                    "Do you wish to print all skid label(s)?",
                    "Yes",
                    "No"))
                {
                    return;
                }

                var xmlString = new StringBuilder();
                var xml = XmlWriter.Create(xmlString);
                var request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<command>"
                    + "<action>SUBMIT PRINT ALL SKID LABELS</action>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<fk_refno>" + this.ActItem.ActHdr.HdrFKRefno + "</fk_refno>"
                    + "<fk_source>PO</fk_source>"
                    + "</command>"
                    + "</imclient>");

                request.Save(xml);
                xml.Flush();
                xml.Close();

                var reqData = SysCommon.SubmitComm(xmlString.ToString());

                if (reqData.Length > 0)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(reqData);

                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(
                                "Print all skid label(s) error." + Environment.NewLine +
                                errorDetail);
                        }
                        else
                        {
                            throw new Exception("Print all skid label(s) error." +
                                Environment.NewLine + errorDetail);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task<bool> CheckIfSkids2Print()
                {
                    var isExist2Print = false;
                    var xmlString = new StringBuilder();
                    var xml = XmlWriter.Create(xmlString);
                    var request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<command>"
                        + "<action>REQUEST CHECK IF SKIDS 2 PRINT</action>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                        + "<fk_refno>" + this.ActItem.ActHdr.HdrFKRefno + "</fk_refno>"
                        + "<fk_source>PO</fk_source>"
                        + "</command>"
                        + "</imclient>");

                    request.Save(xml);
                    xml.Flush();
                    xml.Close();

                    var reqData = SysCommon.SubmitComm(xmlString.ToString());

                    if (reqData.Length > 0)
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(reqData);

                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new MessageOnlyException(
                                    "Check skids to print error." + Environment.NewLine + errorDetail);
                            }
                            else
                            {
                                throw new Exception(
                                    "Check skids to print error." + Environment.NewLine + errorDetail);
                            }
                        }
                        else if (reqData.Contains("<detail>True</detail>"))
                        {
                            isExist2Print = true;
                        }
                    }
                    else
                    {
                        throw new Exception("Request to check if there are skids to print failed.");
                    }

                    return isExist2Print;
                }
            }
        }

        /// <summary>
        /// Cancel any pending activity item changes.
        /// </summary>
        /// <returns><see cref="bool"/> True if the pending changes where canceled, else false.</returns>
        public async Task<bool> CancelChanges()
        {
            var cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    cancelChanges = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                cancelChanges = false;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        /// Get the activity item return XDocument information.
        /// </summary>
        /// <returns><see cref="XDocument"/> Activity item return information.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<XDocument> GetXML()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>" + "</params>");

            return xmldocParams;
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle status information.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// event publisher.</param>
        /// <param name="dataLayerEventArgs"><see cref="DataLayerEventArgs"/> Data layer event
        /// arguments.</param>
        public void OnLayerStatOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            ////ListViewDisplayExtraInfo.Items.Clear();
            ////const int lineLength = 30;
            ////List<string> lines = SysCommon.WordWrap2List(dataLayerEventArgs.StatusOutMsg, lineLength);
            ////for (int i = 0; i < lines.Count; i++)
            ////{
            ////    ListViewDisplayExtraInfo.Items.Add(lines[i]);
            ////}
        }

        /// <summary>
        /// Data layer controller event handler. Used to handle progress information.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer
        /// event publisher.</param>
        /// <param name="dataLayerEventArgs">><see cref="DataLayerEventArgs"/> Data layer event
        /// arguments.</param>
        public void OnLayerProgOut(IDataLayerEventPublisher publisher, DataLayerEventArgs dataLayerEventArgs)
        {
            if (dataLayerEventArgs.CurrentValue > 0 && dataLayerEventArgs.MaxValue > 0)
            {
                double percent = 100 * dataLayerEventArgs.CurrentValue / dataLayerEventArgs.MaxValue;
                ////this.progBarMain.Progress = percent / 100;
            }
        }

        /// <summary>
        /// On back button pressed handler.
        /// </summary>
        /// <returns><see cref="bool"/> True upon completion.</returns>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await this.CancelChanges())
                {
                    await this.ActDisappearing();
                    _ = base.OnBackButtonPressed();
                    await this.Close();
                }
            });

            return true;
        }

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list.
        /// </summary>
        /// <param name="propertyName"><see cref="string"/> Property name to check if changed.</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName == nameof(this.CmdLocationText) ||
                propertyName == nameof(this.CmdLocationIsEnabled) ||
                propertyName == nameof(this.CmdFilterText) ||
                propertyName == nameof(this.CmdFilterIsEnabled) ||
                propertyName == nameof(this.CmdSelectSkidText) ||
                propertyName == nameof(this.CmdSelectSkidIsEnabled) ||
                propertyName == nameof(this.CmdAddProductText) ||
                propertyName == nameof(this.CmdAddProductIsEnabled) ||
                propertyName == nameof(this.CmdAddSkidText) ||
                propertyName == nameof(this.CmdAddSkidIsEnabled) ||
                propertyName == nameof(this.CmdClearSelectedSkidText) ||
                propertyName == nameof(this.CmdClearSelectedSkidIsEnabled))
            {
                PopulateOptionsMenu();
            }

            if (propertyName == nameof(this.CmdSelectSkidText))
            {
                this.lblSkidNumber.IsVisible = true;
                if (this.CmdSelectSkidText != "Select Skid")
                {
                    this.lblSkidNumber.Text = this.CmdSelectSkidText;
                }
                else
                {
                    this.lblSkidNumber.Text = "No skid selected";
                }
            }

            void PopulateOptionsMenu()
            {
                this.OptionsMenuItems.Clear();

                if (this.CmdLocationIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdLocationText);
                }

                if (this.CmdFilterIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdFilterText);
                }

                if (this.CmdSelectSkidIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdSelectSkidText);
                }

                if (this.CmdClearSelectedSkidIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdClearSelectedSkidText);
                }

                if (this.CmdAddProductIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdAddProductText);
                }

                if (this.CmdAddSkidIsEnabled)
                {
                    this.OptionsMenuItems.Add(this.CmdAddSkidText);
                }
            }
        }

        /// <summary>
        /// Fires just before the page is displayed.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                this.Loading = true;
                if (this.IsFirstAppearance)
                {
                    this.IsFirstAppearance = false;
                    await this.ScannerStart();
                    this.CmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                    this.CmdClearSelectedSkidIsEnabled = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0
                        && this.CmdAddSkidIsEnabled && this.CmdSelectSkidIsEnabled;

                    this.ActItem.RegLayerListenerProgOut(this);
                    await this.RefreshBySubType();
                    this.ActItem.UnregLayerListenerProgOut(this);
                    await this.DesignGrid();

                    if (this.MainActivityItems == null || this.MainActivityItems.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(() => this.ExtraInfoItems.Clear());
                    }
                    else
                    {
                        await this.ActAppearing();
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                this.IsFirstAppearance = true;
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var msg = "This activity unexpectedly failed to initialize correctly." +
                              " Please exit and try again.";
                    var errorReport = new FormErrorReport(msg, ex);
                    _ = await errorReport.ShowDialog();
                });
            }
            finally
            {
                this.Loading = false;
                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }
        }

        /// <summary>
        /// Fires just before the page disappears.
        /// </summary>
        protected override async void OnDisappearing()
        {
            await this.ActDisappearing();
            base.OnDisappearing();
        }

        /// <summary>
        /// Subscribe key up handlers, other events and find the selected item using this.SelectedActivityItemFKRefno.
        /// </summary>
        private async Task ActAppearing()
        {
            await this.ScannerEnable();
            this.ListViewMainActivity.ItemTapped += this.ListViewMainActivity_ItemTapped;
            this.ListViewDisplayExtraInfo.ItemsSource = this.ExtraInfoItems;
            SubscribeKeyUpHandlers();
            await this.FindSelectedItemAsync();

            //////////////////////////////////////////////////////////////////////////
            void SubscribeKeyUpHandlers()
            {
                MessagingCenter.Subscribe<string>(
                    this,
                    EventTypes.OnKeyUp.ToString(),
                    async (keyPressed) =>
                    {
                        if (this.Loading)
                        {
                            return;
                        }

                        var svdIsScanReg = this.IsBarCodeHandlerRegistered;
                        await this.ScannerDisable();

                        _ = Enum.TryParse(keyPressed, out Keycode keycode);

                        if (keycode == Keycode.Enter)
                        {
                            await OnAppearingEnterKey();
                        }
                        else if (keycode == Keycode.Clear)
                        {
                            await OnAppearingClearKey();
                        }
                        else if (keycode == Keycode.Del)
                        {
                            await OnAppearingDelKey();
                        }
                        else if (keycode == Keycode.F1)
                        {
                            await OnAppearingF1Key();
                        }
                        else if (keycode == Keycode.F2)
                        {
                            await OnAppearingF2Key();
                        }
                        else if (keycode == Keycode.F3)
                        {
                            await OnAppearingF3Key();
                        }
                        else if (keycode == Keycode.F4)
                        {
                            await OnAppearingF4Key();
                        }
                        else if (keycode == Keycode.DpadDown)
                        {
                            OnAppearingDpadDown();
                        }
                        else if (keycode == Keycode.DpadUp)
                        {
                            OnAppearingDpadUp();
                        }

                        if (svdIsScanReg)
                        {
                            await this.ScannerEnable();
                        }
                    });

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingEnterKey()
                {
                    if (this.Loading)
                    {
                        return;
                    }

                    if (this.ListViewMainActivity.SelectedItem == null)
                    {
                        return;
                    }

                    if (Convert.ToInt32(this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem)) < 0)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                "Please select an item.",
                                Resource.OK));
                        return;
                    }

                    await this.ListViewMainActivity_KeyUpEnter();
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingClearKey()
                {
                    if (await this.CancelChanges())
                    {
                        this.DialogResult = DialogResult.None;

                        // TODO: DEV-5619
                        // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                        await this.ActDisappearing();
                        await this.Close();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF1Key()
                {
                    if (await this.SaveChanges())
                    {
                        this.DialogResult = DialogResult.OK;

                        // TODO: DEV-5619
                        // Android dMobile F1 & F3 Button Not Working Same As Windows dMobile.
                        await this.ActDisappearing();
                        await this.Close();
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadDown()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) <
                            this.MainActivityItems.Count - 1)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) + 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                void OnAppearingDpadUp()
                {
                    if (this.ListViewMainActivity.SelectedItem != null &&
                        Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > 0)
                    {
                        this.ListViewMainActivity.SelectedItem =
                            this.MainActivityItems[this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) - 1];
                        this.ListViewMainActivity.ScrollTo(
                            this.ListViewMainActivity.SelectedItem,
                            ScrollToPosition.MakeVisible,
                            true);
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingDelKey()
                {
                    if (this.Loading)
                    {
                        return;
                    }

                    var isItemSelected = true;
                    if (this.ListViewMainActivity.SelectedItem == null)
                    {
                        isItemSelected = false;
                    }

                    if (Convert.ToInt32(this.MainActivityItems.IndexOf(
                        this.ListViewMainActivity.SelectedItem)) < 0)
                    {
                        isItemSelected = false;
                    }
                    else
                    {
                        isItemSelected = true;
                    }

                    if (!isItemSelected)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.GetControlDescription(),
                                "Nothing selected to delete.",
                                Resource.OK));
                        return;
                    }

                    // If there is a skid selected
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                    {
                        // Ask to delete the skid
                        var msg = $"Do you wish to delete this skid" +
                                  $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' detail?";
                        if (await this.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            msg,
                            "Yes",
                            "No"))
                        {
                            // Make sure that the warehouse, product, bc, and unit match.
                            var skidInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                            if (this.ActItem.ProdInfo.FKRefno != skidInfo.ProdInfo.FKRefno ||
                                this.ActItem.ProdInfo.UnitIsBC != skidInfo.ProdInfo.UnitIsBC ||
                                this.ActItem.ProdInfo.UnitRefno != skidInfo.ProdInfo.UnitRefno ||
                                this.ActItem.LocMap.WhsFKRefno != skidInfo.LocMap.WhsFKRefno)
                            {
                                throw new MessageOnlyException("The selected skid warehouse and" +
                                    " product must be the same as the line item warehouse and product" +
                                    " before the skid can be deleted.");
                            }

                            var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                            await actDetail.DeleteSkid(this.ActItem.ActHdr.SkidInfo);

                            var fillQty = await actDetail.GetTotalQuantityFromListDetail();
                            actDetail.ActItem.ReplaceQuantityInActivityItem(
                                actDetail.ActItem.SelectedActivityItem,
                                fillQty.Quantity,
                                fillQty.BaseQuantity,
                                fillQty.WeightQuantity);
                            await this.ActItem.UpdateItemTableQuantity(actDetail.ActItem.SelectedActivityItem);
                            this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;
                            this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    "Item deletion needs to be performed on the desktop.",
                                    Resource.OK));
                        }
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                "Item deletion needs to be performed on the desktop.",
                                Resource.OK));
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF2Key()
                {
                    try
                    {
                        this.ListViewMainActivity.IsEnabled = false;
                        if (this.ListViewMainActivity.SelectedItem != null &&
                            this.MainActivityItems.IndexOf(
                                this.ListViewMainActivity.SelectedItem) < 0)
                        {
                            throw new MessageOnlyException(
                                "Please select a line item to show the comments for.");
                        }

                        string comment = await this.ActItem.GetComment(
                            this.ListViewMainActivity.SelectedItem as dynamic);
                        if (!string.IsNullOrEmpty(comment))
                        {
                            await this.DisplayAlert("Item Comment", comment, "OK");
                        }
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                    finally
                    {
                        this.ListViewMainActivity.IsEnabled = true;
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF3Key()
                {
                    try
                    {
                        var msg = await this.ActItem.GetTotalQuantity();

                        if (!string.IsNullOrEmpty(msg))
                        {
                            await Device.InvokeOnMainThreadAsync(async () =>
                                await this.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    msg,
                                    "OK"));
                        }
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                async Task OnAppearingF4Key()
                {
                    try
                    {
                        this.ListViewMainActivity.IsEnabled = false;
                        await this.ActDisappearing();

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException(
                                "Inventory Inquiry Summary is unavailable in disconnected mode.");
                        }

                        var form = new FormDialogContainer(
                            EDialogTypes.ActivityInventoryInquirySummary,
                            this.ActItem,
                            this.ActSubType);
                        _ = await Device.InvokeOnMainThreadAsync(async () => await form.DisplayControl.ShowDialog());

                        // No need to refresh the data here after call to inquiry form.
                    }
                    catch (MessageOnlyException ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                ex.Message,
                                Resource.OK));
                    }
                    catch (Exception ex)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }
                    finally
                    {
                        await this.ActAppearing();
                        this.ListViewMainActivity.IsEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe from KeyUpHandler, other event hooks and turn off the scanner.
        /// </summary>
        private async Task ActDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ListViewMainActivity.ItemTapped -= this.ListViewMainActivity_ItemTapped;
            await this.ScannerDisable();
        }

        /// <summary>
        /// Double click on list view item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="ItemTappedEventArgs"/> Item tapped event arguments.</param>
        private async void ListViewMainActivity_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            this.NumTaps++;

            if (this.PreviousSender == e.Item)
            {
                if (this.NumTaps == 2)
                {
                    this.NumTaps = 0;

                    if (this.ListViewMainActivity.SelectedItem != null &&
                        (Convert.ToInt32(this.MainActivityItems.IndexOf(
                            this.ListViewMainActivity.SelectedItem)) > -1))
                    {
                        await this.ListViewMainActivity_KeyUpEnter();
                    }
                }
            }
            else
            {
                this.PreviousSender = e.Item;
                this.NumTaps = 1;
            }

            await Task.Delay(500);
            this.NumTaps = 0;
        }

        /// <summary>
        /// Check if the activity item has been reviewed.
        /// </summary>
        private void IsItemReviewed()
        {
            var isReviewed = true;

            // TODO: Looks like confirmations have not been implemented. Currently picking and
            // warehouse transfers can use confirmations.
            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                if (this.ActItem.SelectedActivityItem != null)
                {
                    // TODO: test
                    isReviewed = Convert.ToBoolean(
                        (this.ActItem.SelectedActivityItem as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// Check if ALL the activity items has been reviewed.
        /// </summary>
        private void IsAllItemsReviewed()
        {
            var isReviewed = true;

            // TODO: Looks like confirmations have not been implemented. Currently picking and
            // warehouse transfers can use confirmations.
            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                for (var idx = 0; idx <= this.ActItem.ListItems.Count - 1; idx++)
                {
                    // TODO: test
                    isReviewed = isReviewed && Convert.ToBoolean(
                        (this.ActItem.ListItems[idx] as dynamic).ActivityItemPickingIsReviewed);
                }

                if (!isReviewed)
                {
                    throw new MessageOnlyException("Items exist that have not been reviewed.");
                }
            }

            ////if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel &&
            ////    (this.ActSubType == SysCommon.ActSubTypes.Picking ||
            ////    this.ActSubType == SysCommon.ActSubTypes.PickingSingle))
            ////{
            ////    bool isReviewed = true;
            ////    foreach (object item in this.ActItem.ListItems)
            ////    {
            ////        isReviewed = isReviewed && ((PickingItem)item).ActivityItemPickingIsReviewed;
            ////    }

            ////    if (isReviewed)
            ////    {
            ////        ((Picking)this.ActItem.ActHdr.SelectedActivity).ActivityPickingIsReviewed = true;
            ////    }

            ////    if (!await ((PickingActivity)this.ActItem.ActHdr).ClearSalesOrderBeingPicked(true))
            ////    {
            ////        await Device.InvokeOnMainThreadAsync(async () =>
            ////            await Application.Current.MainPage.DisplayAlert(
            ////                this.ActItem.GetControlDescription(),
            ////                "Unable to clear the current picker.",
            ////                Resource.OK));
            ////    }
            ////}
        }

        /// <summary>
        /// Perform quantity under check for ALL line items.
        /// </summary>
        private void AllItemsQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn
                != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                for (var currIdx = 0; currIdx < this.ActItem.ListItems.Count; currIdx++)
                {
                    // TODO: Test
                    dynamic selectedItem = this.ActItem.ListItems[currIdx];

                    decimal available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                    decimal shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                    decimal ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);
                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (available > 0 && shipped < ordered)
                        {
                            throw new MessageOnlyException(
                                "Some line items exists where the shipped quantity is less that" +
                                " the quantity ordered.");
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            throw new MessageOnlyException(
                                "Some line items exists where the shipped quantity is less that" +
                                " the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for one line item.
        /// </summary>
        /// <remarks>Line items must exist, one needs to be selected.</remarks>
        private void ItemQuantityUnderCheck()
        {
            if (this.ActItem.ListItems != null)
            {
                if (this.SysEnv.SysSettings.PickQtyUnderWarn
                    != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
                {
                    if (this.ActItem.SelectedActivityItem != null)
                    {
                        dynamic selectedItem = this.ActItem.SelectedActivityItem;
                        ////decimal available = Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable);
                        decimal shipped = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityShipped);
                        decimal ordered = Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityOrdered);

                        if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                        {
                            // Do not warn if product is out of stock WO# 12054 was only applied
                            // to ucActivityItemSingle if (0 < available && shipped < ordered)
                            if (shipped < ordered)
                            {
                                string msg;
                                if (this.SysEnv.SysSettings.IsPickQtyUnderShowBaseQtyAvail)
                                {
                                    msg = string.Format(
                                        "The quantity shipped is less than the quantity ordered." +
                                        Environment.NewLine + "On Hand {0} {1}",
                                        Convert.ToDecimal(selectedItem.ActivityItemPickingBaseQuantityAvailable),
                                        Convert.ToString(selectedItem.BaseUnitCode));
                                }
                                else
                                {
                                    msg = string.Format(
                                        "The quantity shipped is less than the quantity ordered." +
                                        Environment.NewLine + "On Hand {0} {1}",
                                        Convert.ToDecimal(selectedItem.ActivityItemPickingQuantityAvailable),
                                        Convert.ToString(selectedItem.UnitCode));
                                }

                                throw new MessageOnlyException(msg);
                            }
                        }
                        else
                        {
                            if (shipped < ordered)
                            {
                                throw new MessageOnlyException(
                                    "The quantity shipped is less than the quantity ordered.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Main data grid key up enter event handler.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        private async Task ListViewMainActivity_KeyUpEnter()
        {
            if (this.Loading)
            {
                return;
            }

            if (this.ListViewMainActivity.SelectedItem == null)
            {
                return;
            }

            if (Convert.ToInt32(this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem)) < 0)
            {
                return;
            }

            await Task.Run(async () =>
            {
                try
                {
                    await this.ActDisappearing();
                    await this.ActItem.AssignActivityItem(this.ListViewMainActivity.SelectedItem);
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                        if (currSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                        {
                            throw new MessageOnlyException(
                                "Selected line item product must match the selected skid product.");
                        }
                    }

                    var currSelectedIdx = this.MainActivityItems.IndexOf(this.ListViewMainActivity.SelectedItem);
                    var form = new FormDialogContainer(
                        EDialogTypes.ActivityDetail,
                        this.ActItem,
                        this.ActSubType);
                    this.IsChangesMade = false;

                    // There currently no way to cancel from the detail screen. Always returns DialogResult.OK.
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        var xmlDoc = await form.GetXML();
                        this.ActItem.ReplaceQuantityInActivityItem(
                            this.ActItem.SelectedActivityItem,
                            Math.Round(decimal.Parse(xmlDoc.Root.Element("qty").Value), 3),
                            Math.Round(decimal.Parse(xmlDoc.Root.Element("base_qty").Value), 3),
                            Math.Round(decimal.Parse(xmlDoc.Root.Element("wt_qty").Value), 3));
                        await this.ActItem.UpdateItemTableQuantity(this.ListViewMainActivity.SelectedItem);
                        this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                        // This line is needed to display the new updated quantity visually.
                        this.MainActivityItems[this.ActItem.SelectedListItemIdx] = this.ActItem.SelectedActivityItem;
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ActAppearing();
                }
            });
        }

        /// <summary>
        /// Bar code scan event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="scanArgs"><see cref="BarCodeScannerEventArgs"/> Sender bar code scanner
        /// event arguments.</param>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            if (this.Loading)
            {
                return;
            }

            scanArgs = scanArgs ?? throw new ArgumentNullException(nameof(scanArgs));

            this.Loading = true;

            await this.ActDisappearing();

            var prodUnitRefno = 0;
            object selectedItem = null;
            var actItemFoundRefno = 0;
            var tempItemProdSchemeData = new SActItemSchemeData();

            try
            {
                var parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    await BarCodeHandlerLocationIdentifier(parse);
                }
                else
                {
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        // Identify the product from the parse info.
                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse);
                        var scannedProdInfo = new ProductInfo();
                        scannedProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        await VerifyAndDefaultLocation(scannedProdInfo);

                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        if (actItemFoundRefno > 0)
                        {
                            this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                            selectedItem = this.ActItem.SelectedActivityItem;
                            this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                if (await AskToAddNewItem(scannedProdInfo))
                                {
                                    // Additional found matching line items are not allowed to be created during
                                    // scanning and inserting. I am not expecting matches to be found because
                                    // that check has already been performed at this point.
                                    await this.ActItem.Insert(ProductInfoTasks.GetXML(scannedProdInfo), false);
                                    selectedItem = this.ActItem.SelectedActivityItem;
                                    this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                                    actItemFoundRefno = this.ActItem.ItemRefno;

                                    // Reload all the line items from the ActItem.ListItem collection and populate the
                                    // List view.
                                    // NOTE: Change this to add one only the ListView / Grid at a time on the end.
                                    // For example a physical / cycle count could have 1000 plus line on a document.
                                    // Plus add new line items consistently with already many existing will get
                                    // progressively slower.
                                    await this.DesignGrid();
                                }
                                else
                                {
                                    throw new GatherProductDataException(
                                        this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo), true);
                                }
                            }
                            else
                            {
                                throw new MessageOnlyException(this.ActItem.GetItemNotFoundMessage(this.LocMap, scannedProdInfo));
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
                        selectedItem = this.ActItem.ListItems.OfType<object>().FirstOrDefault(activityItem =>
                            Convert.ToString(activityItem.GetType().GetProperty(
                                this.ActItem.GetDataViewSortFKRefnoIdx())?.GetValue(activityItem)) ==
                                Convert.ToString(scanArgs.ParseInfo.ActItemFKRefno));
                        if (selectedItem == null)
                        {
                            throw new Exception(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                        }
                        else
                        {
                            await this.ActItem.AssignActivityItem(selectedItem);
                            this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                            actItemFoundRefno = this.ActItem.ItemRefno;
                        }
                    }
                    else if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
                        throw new MessageOnlyException(
                            "Scanning an line item detail bar code is not supported here.");
                    }
                    else if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            throw new MessageOnlyException(
                                "Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromBarCodeOrCode(
                            parse.SSCC18);

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            throw new MessageOnlyException("Skid does not exist.");
                        }

                        prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        if (actItemFoundRefno > 0)
                        {
                            this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                            this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                            selectedItem = this.ActItem.SelectedActivityItem;
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                var data = ProductInfoTasks.GetXML(this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);

                                // Additional found matching line items are not allowed to be created during
                                // scanning and inserting. I am not expecting matches to be found because
                                // that check has already been performed at this point.
                                await this.ActItem.Insert(data, askToInsertAsNewItem: false);
                                selectedItem = this.ActItem.SelectedActivityItem;
                                this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                throw new MessageOnlyException(
                                    this.ActItem.GetItemNotFoundMessage(
                                        this.LocMap,
                                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }
                        }
                    }

                    // Check the location again because of product location defaulting.
                    await VerifyLocationIsValid();

                    // If NOT a container, evaluate
                    if (string.IsNullOrEmpty(parse.SSCC18))
                    {
                        await CheckCurrentLocationMatchProductLocation(selectedItem);

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.
                        //// if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////    || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        //// {
                        ////    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////    {
                        ////        this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////        this.cmdSelectSkid.Text = Translator.GetInstance().Translate(
                        ////        TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////        this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////    }
                        //// }

                        var isInsertReady = false;
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        var schemeData = actDetail.SchemeData;
                        schemeData = DefaultSchemeDataFromCurrentSkidSchemeDetail(actDetail);
                        tempItemProdSchemeData.ProdSchemeData = schemeData;
                        actDetail.SchemeData = schemeData;

                        isInsertReady = await SysShared.GatherProductData(
                            parse,
                            this.ActSubType,
                            this.ActItem,
                            tempItemProdSchemeData,
                            isInsertReady,
                            actDetail,
                            false);

                        if (isInsertReady)
                        {
                            var xmlDoc = actDetail.GetXML(actDetail);
                            await actDetail.Insert(xmlDoc);

                            // TODO check if actually new, if not delete and change counter
                            // Does the row exist? If not, then it has been removed due to not needing it
                            // Skip update qty.
                            if (selectedItem != null)
                            {
                                // Upon a save of completed data update the data view to to show new item and
                                // quantity change.
                                this.ActItem.AddToQuantityInActivityItem(
                                    selectedItem,
                                    actDetail.Quantity,
                                    actDetail.BaseQuantity,
                                    actDetail.WeightQuantity);
                                await this.ActItem.UpdateItemTableQuantity(selectedItem);

                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    this.ListViewMainActivity.SelectedItem = selectedItem;

                                    // This line is needed to display the new updated quantity visually.
                                    this.MainActivityItems[this.ActItem.SelectedListItemIdx] = selectedItem;
                                });
                            }
                        }
                    }
                    else
                    {
                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        var actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);
                        actDetail.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks
                        // and quantity validations.
                        var xmlParam = XDocument.Parse(
                                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                    + "<imclient>"
                                    + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                                    + "<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                                    + "<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                                    + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                                    + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        if (actDetail.Quantity == 0)
                        {
                            throw new MessageOnlyException($"No quantity exists on skid" +
                                $" '{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}' in warehouse" +
                                $" {LocationMap.GetInstance().CurrLocMap.WhsCode}.");
                        }

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            selectedItem,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);
                        await this.ActItem.UpdateItemTableQuantity(selectedItem);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.ListViewMainActivity.SelectedItem = selectedItem;

                            // This line is needed to display the new updated quantity visually.
                            this.MainActivityItems[this.ActItem.SelectedListItemIdx] = selectedItem;
                        });
                    }

                    // IF AN ACTIVITY LINE ITEM WAS FOUND
                    if (actItemFoundRefno == 0 || selectedItem == null)
                    {
                        throw new MessageOnlyException("Line item not found.");
                    }
                    else
                    {
                        this.ActItem.SelectedActivityItem = selectedItem;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing as task is being canceled from IdentifyProductScanned when user
                // declines to assign bar code to product.
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (GatherProductDataException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (IsReadyToApplyException ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = true;

                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                    });
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.CmdAddSkidIsEnabled = false;

                        this.CmdSelectSkidText = "Select Skid";
                    });
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ActAppearing();
            }

            this.Loading = false;

            //////////////////////////////////////////////////////////////////////////
            async Task BarCodeHandlerLocationIdentifier(BarCodeParse parse)
            {
                // If section filtering was set in the activity header. Do we want to enforce
                // the scanned section location.
                var tmpLoc = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(
                        parse.LocationIdentifier));
                this.LocMap.CurrLocMap = tmpLoc;
                this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();

                // Special validation for picking and filtering by section.
                // Check if the scanned section matched the filtered one.
                if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                    this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                    if (sectRefno != 0 && tmpLoc.SectLocMapRefno != sectRefno)
                    {
                        var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                        var msg = $"Warning scanned location section '{tmpLoc.SectName}' does" +
                                  $" not match the filtered picking section '{sectName}'.";

                        await Device.InvokeOnMainThreadAsync(async () =>
                                await Application.Current.MainPage.DisplayAlert(
                                    this.ActItem.GetControlDescription(),
                                    msg,
                                    Resource.OK));
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task<bool> AskToAddNewItem(ProductInfo scannedProdInfo)
            {
                var isAddNewItem = false;
                if (this.ActItem.IsAskCreateNewItem)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        if (await this.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            string.Format(
                                "A line item does not exist for this warehouse {0}" +
                                    " and product \'{1}.\'" + Environment.NewLine +
                                    "Do you wish to create one?",
                                this.LocMap.CurrLocMap.WhsCode,
                                ProductInfoTasks.GetCurrReadableProdBCUnit(scannedProdInfo)),
                            "Yes",
                            "No"))
                        {
                            isAddNewItem = true;
                        }
                    });
                }
                else
                {
                    isAddNewItem = true;
                }

                return isAddNewItem;
            }

            //////////////////////////////////////////////////////////////////////////
            async Task VerifyAndDefaultLocation(ProductInfo scannedProdInfo)
            {
                var dspOverrideMsg = false;
                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                            this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                        || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                            this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                    {
                        dspOverrideMsg = false;
                    }
                    else
                    {
                        dspOverrideMsg = true;
                    }
                }

                if (dspOverrideMsg)
                {
                    var whsLocMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(locationMapRefno: 0);

                    // We can't determine the product location map base on the sub type and quantity.
                    // We don't have the quantity yet, so default to the floor.
                    var tmpLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        scannedProdInfo.Refno,
                        SysCommon.ProdLocMapTypes.Floor);
                    if (await LocationMapTasks.IsCompleteLocMap(tmpLocMapRefno))
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(tmpLocMapRefno);
                        this.LocMap.ChosenLocMap = this.LocMap.CurrLocMap.Clone();
                    }
                }
                else
                {
                    // This should not be needed, current location should have been set using the location picker.
                    if (this.LocMap.CurrLocMapRefno != this.LocMap.ChosenLocMapRefno)
                    {
                        this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();
                    }
                }

                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno))
                {
                    throw new MessageOnlyException("Location must be set first!");
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task VerifyLocationIsValid()
            {
                // Check the location again because of product location defaulting.
                if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMapRefno) &&
                    this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    throw new MessageOnlyException("Location must be set first.");
                }

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno !=
                        this.ActItem.ProdInfo.UnitRefno)
                    {
                        throw new MessageOnlyException(
                            "Identified line item product does not match the selected skid product.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CheckCurrentLocationMatchProductLocation(object selectedItem)
            {
                // RULE: CHECK IF THE CURRENT LOCATION AND PRODUCT HOME LOCATION ARE THE SAME
                if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                    (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                    this.ActSubType == SysCommon.ActSubTypes.TransferFrom))
                {
                    // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                    var locMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                        Convert.ToInt32(selectedItem.GetType().GetProperty(
                            "LocationMapRefno")?.GetValue(selectedItem)),
                        Convert.ToInt32(selectedItem.GetType().GetProperty(
                            "ProductRefno")?.GetValue(selectedItem)));

                    if (this.LocMap.CurrLocMapRefno != locMapRefno)
                    {
                        throw new MessageOnlyException(
                            "The current location does not match the product home location.");
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            SProductSchemeData DefaultSchemeDataFromCurrentSkidSchemeDetail(ActDetail actDetail)
            {
                // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                // because of the length of the code and the fact is isn't human readable friendly.
                var schemeData = actDetail.SchemeData;

                if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    var curSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;
                    schemeData.LogisticUnitSkidNumber = curSkid.SkidCode;
                    schemeData.LogisticUnitBatchLotNumber = curSkid.SchemeData.LogisticUnitBatchLotNumber;
                    schemeData.LogisticUnitBestBeforeSellByDate = curSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                    schemeData.LogisticUnitProductionDate = curSkid.SchemeData.LogisticUnitProductionDate;
                    schemeData.VariantCode = curSkid.SchemeData.VariantCode;
                    schemeData.VariantNumber = curSkid.SchemeData.VariantNumber;
                    schemeData.VariantDescription = curSkid.SchemeData.VariantDescription;
                }

                return schemeData;
            }
        }

        /// <summary>
        /// Call refresh data, unique to the activity sub type.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task RefreshBySubType()
        {
            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.PhysicalCounts:
                    // Do not refresh product list for counts. Except on first load. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as
                    // MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    if (this.IsFirstAppearance)
                    {
                        await this.RefreshData();
                    }

                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    // Do not refresh product list for counts. Except on first load. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as
                    // MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    if (this.IsFirstAppearance)
                    {
                        await this.RefreshData();
                    }

                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.BarCodeAssociation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.RefreshData:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.DataStatus:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Returns:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SelectLocation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurgeActivityData:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.BreakSkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.LoginStartup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SplashStartup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PrintProductLabel:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingSelectCriteria:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.PickingFilter:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SubmitBatch:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.CommunicationSetup:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.SetupDefaults:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.InventoryInquirySummary:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    await this.RefreshData();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    await this.RefreshData();
                    break;

                default:
                    // Performing a refresh data will remove and partially added new items or quantity.
                    await this.RefreshData();
                    break;
            }
        }

        /// <summary>
        /// Refresh the activity item data.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        private async Task RefreshData()
        {
            try
            {
                await this.ScannerDisable();
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ActItem.RegLayerListenerProgOut(this);
                    await this.ActItem.Refresh(this.ActItem.ActHdr.XMLDataCriteria);
                    await Device.InvokeOnMainThreadAsync(() => this.ActItem.ListItems.Clear());
                    this.ActItem.ListItems = await this.ActItem.GetActivityItemList(false);
                });
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(
                        this.ActSubType,
                        ex,
                        this.ActItem.GetControlDescription(),
                        "Refresh");
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
                this.ActItem.UnregLayerListenerProgOut(this);
                ////await Task.Run(() =>
                ////{
                ////    // Explicit GC, Xamarin forms not disposing of objects properly
                ////    GC.Collect();
                ////    GC.WaitForPendingFinalizers();
                ////});
            }
        }

        /// <summary>
        /// Design the main activity grid / list.
        /// </summary>
        /// <returns>The <see cref="Task"/>. Task operation.</returns>
        private async Task DesignGrid()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                // Redesign the grid / list and rebind it to the refreshed list items.
                this.MainActivityItems = this.ActItem.DesignGrid(
                    ref this.GridMainActivity,
                    ref this.ListViewMainActivity);

                if (!string.IsNullOrEmpty(this.txtSearchItem.Text))
                {
                    this.MainActivityItems = this.ActItem.Search(this.txtSearchItem.Text);
                }

                this.ListViewMainActivity.ItemsSource = this.MainActivityItems;
            });
        }

        /// <summary>
        /// Find the selected item in the data list.
        /// </summary>
        /// <returns><see cref="Task"/> Task operation.</returns>
        private async Task FindSelectedItemAsync()
        {
            if (this.ActItem.ListItems.Count > 0)
            {
                this.ActItem.SelectedListItemIdx = -1;
                object selectedItem = null;
                foreach (var item in this.ActItem.ListItems)
                {
                    if (this.SelectedActivityItemFKRefno == this.ActItem.GetFKRefno(item))
                    {
                        selectedItem = item;
                        this.ActItem.SelectedListItemIdx++;
                        break;
                    }
                }

                if (selectedItem == null)
                {
                    // Goto the first one in the list.
                    this.ActItem.SelectedListItemIdx = 0;
                    selectedItem = this.ActItem.ListItems[this.ActItem.SelectedListItemIdx];
                    await this.ActItem.AssignActivityItem(selectedItem);
                    this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                }
                else
                {
                    await this.ActItem.AssignActivityItem(selectedItem);
                    this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = selectedItem;
                    this.ListViewMainActivity.ScrollTo(selectedItem, ScrollToPosition.MakeVisible, false);
                });
            }
            else
            {
                // Clear the activity nothing can be selected.
                this.ActItem.SelectedListItemIdx = -1;
                this.ActItem.SelectedActivityItem = null;
                this.ActItem.ItemFKRefno = 0;
                this.ActItem.ItemRefno = 0;
                this.SelectedActivityItemFKRefno = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ListViewMainActivity.SelectedItem = null;
                });
            }
        }

        /// <summary>
        /// Text search activity items text changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Sender object text changed event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void TxtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                await this.ScannerDisable();
                await this.DesignGrid();
            }
            catch (MessageOnlyException ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert(
                        this.ActItem.GetControlDescription(),
                        ex.Message,
                        Resource.OK));
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var errRpt = new FormErrorReport(this.ActSubType, ex);
                    _ = await errRpt.ShowDialog();
                });
            }
            finally
            {
                await this.ScannerEnable();
            }
        }

        /// <summary>
        /// Bar code modifier selected index changed event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        private void UpdnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = this.BarCodeModifier1Picker.SelectedItem.ToString();
        }

        /// <summary>
        /// Data grid activity item selected item event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="SelectedItemChangedEventArgs"/> Sender selected item changed
        /// event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void ListViewMainActivity_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await this.ActItem.AssignActivityItem(e.SelectedItem);
            this.ActItem.SelectedListItemIdx = e.SelectedItemIndex;
            this.SelectedActivityItemFKRefno = this.ActItem.ItemFKRefno;
            await DisplayDataItem();

            /// <summary>
            /// Activity item display data.
            /// </summary>
            async Task DisplayDataItem()
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    this.ExtraInfoItems.Clear();

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                    {
                        this.CmdAddSkidIsEnabled = true;
                        this.CmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        ////this.ExtraInfoItems.Add("Selected Skid #: " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode);
                    }
                    else
                    {
                        this.CmdAddSkidIsEnabled = false;
                        this.CmdSelectSkidText = "Select Skid";
                    }

                    this.ExtraInfoItems.Add("Selected Location: " + LocationMapTasks.GetCurrentReadableLocation());

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno > 0)
                    {
                        this.CmdLocationText = "Location: " + Environment.NewLine +
                            LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
                        this.CmdLocationText = "Set Location";
                    }

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                    {
                        this.CmdLocationIsEnabled = false;
                    }
                    else
                    {
                        this.CmdLocationIsEnabled = true;
                    }

                    var prodLocCode = string.Empty;
                    var prodLocName = string.Empty;
                    var prodDesc = string.Empty;
                    var prodUnitSizeDesc = string.Empty;
                    var defLoc = string.Empty;
                    var defProdLoc = string.Empty;
                    var prodBrandDesc = string.Empty;
                    var prodCode = string.Empty;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno <= 0)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            this.LocMap.LoginWhsLocMapRefno);
                    }

                    switch (this.ActItem.ActSubType)
                    {
                        case SysCommon.ActSubTypes.Adjustment:
                            if (this.ListViewMainActivity.SelectedItem is AdjustmentItem adjustmentItem)
                            {
                                prodDesc = adjustmentItem.ProductDescription;
                                prodUnitSizeDesc = adjustmentItem.ProductUnitSizeDescription;
                                this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    adjustmentItem.LocationCode,
                                    adjustmentItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);

                                this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            }

                            break;

                        case SysCommon.ActSubTypes.AssignDefaultLocation:
                            break;

                        case SysCommon.ActSubTypes.BarCodeAssociation:
                            break;

                        case SysCommon.ActSubTypes.BreakSkid:
                            break;

                        case SysCommon.ActSubTypes.ConvertUnit:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            var countLoc = string.Empty;

                            if (this.ListViewMainActivity.SelectedItem is CountItem cycleCountItem)
                            {
                                // Get the cycle count line item location.
                                var countItemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    Convert.ToInt32(cycleCountItem.LocationMapRefno));
                                countLoc = LocationMapTasks.GetReadableLocation(countItemLocMap);

                                // Using the cycle count line item warehouse, get the product default
                                // picking location.
                                var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                    countItemLocMap.WhsLocMapRefno,
                                    Convert.ToInt32(cycleCountItem.ProductRefno),
                                    SysCommon.ProdLocMapTypes.Picking);
                                defProdLoc = LocationMapTasks.GetReadableLocation(
                                    await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                                prodDesc = cycleCountItem.ProductDescription.ToString();
                                prodUnitSizeDesc = cycleCountItem.ProductUnitSizeDescription.ToString();
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            this.ExtraInfoItems.Add("Count Location: " + countLoc.Trim());
                            break;

                        case SysCommon.ActSubTypes.DataStatus:
                            break;

                        case SysCommon.ActSubTypes.InventoryInquiry:
                            if (this.ActItem.ProdInfo.Refno <= 0)
                            {
                                this.ExtraInfoItems.Add(string.Format("PRODUCT: {0}", "All"));
                            }
                            else
                            {
                                this.ExtraInfoItems.Add(string.Format(
                                    "PRODUCT: {0}",
                                    ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo)));
                            }

                            this.ExtraInfoItems.Add(string.Format(
                                "LOCATION: {0}",
                                LocationMapTasks.GetCurrentReadableLocation()));
                            break;

                        case SysCommon.ActSubTypes.InventoryReplenishment:
                            break;

                        case SysCommon.ActSubTypes.MoveInventory:
                            break;

                        case SysCommon.ActSubTypes.MoveSkid:
                            break;

                        case SysCommon.ActSubTypes.NotApplicable:
                            break;

                        case SysCommon.ActSubTypes.PhysicalCounts:
                            if (this.ListViewMainActivity.SelectedItem is CountItem physicalCountItem)
                            {
                                // Do not check if default location is enabled. Always show the default location.
                                var tempLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    Convert.ToInt32(physicalCountItem.LocationMapRefno));
                                var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                                    tempLocMap.WhsLocMapRefno,
                                    Convert.ToInt32(physicalCountItem.ProductRefno),
                                    SysCommon.ProdLocMapTypes.Picking);
                                defProdLoc = LocationMapTasks.GetReadableLocation(
                                    await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                                prodDesc = physicalCountItem.ProductDescription.ToString();
                                prodUnitSizeDesc = physicalCountItem.ProductUnitSizeDescription.ToString();
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defProdLoc.Trim());
                            break;

                        case SysCommon.ActSubTypes.Picking:
                            if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItem)
                            {
                                prodLocCode = Convert.ToString(pickingItem.LocationCode);
                                prodLocName = pickingItem.LocationName;
                                prodCode = pickingItem.ProductCode;
                                prodDesc = pickingItem.ProductDescription;
                                prodUnitSizeDesc = pickingItem.ProductUnitSizeDescription;
                                prodBrandDesc = pickingItem.ProductBrandDescription;
                                switch (this.SysEnv.SysSettings.PickItemDescOpt)
                                {
                                    case 1:

                                        this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " +
                                            prodBrandDesc.Trim());
                                        break;

                                    default:

                                        this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " +
                                            prodUnitSizeDesc.Trim());
                                        break;
                                }

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    pickingItem.LocationCode,
                                    pickingItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);

                                this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            }

                            break;

                        case SysCommon.ActSubTypes.PickingSingle:
                            if (this.ListViewMainActivity.SelectedItem is PickingItem pickingItemSingle)
                            {
                                prodLocCode = Convert.ToString(pickingItemSingle.LocationCode);
                                prodLocName = pickingItemSingle.LocationName;
                                prodDesc = pickingItemSingle.ProductDescription;
                                prodUnitSizeDesc = pickingItemSingle.ProductUnitSizeDescription;
                                prodBrandDesc = pickingItemSingle.ProductBrandDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    pickingItemSingle.LocationCode,
                                    pickingItemSingle.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            switch (this.SysEnv.SysSettings.PickItemDescOpt)
                            {
                                case 1:

                                    this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " +
                                        prodBrandDesc.Trim());
                                    break;

                                default:

                                    this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " +
                                        prodUnitSizeDesc.Trim());
                                    break;
                            }

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.TicketAddProduct:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketByProductItem
                                productionTicketByProductItem)
                            {
                                prodLocCode = Convert.ToString(productionTicketByProductItem.LocationCode);
                                prodLocName = productionTicketByProductItem.LocationName;
                                prodDesc = productionTicketByProductItem.ProductDescription;
                                prodUnitSizeDesc = productionTicketByProductItem.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    productionTicketByProductItem.LocationCode,
                                    productionTicketByProductItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.TicketFinished:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketFinishedItem
                                productionTicketFinishedItem)
                            {
                                prodLocCode = Convert.ToString(productionTicketFinishedItem.LocationCode);
                                prodLocName = productionTicketFinishedItem.LocationName;
                                prodDesc = productionTicketFinishedItem.ProductDescription;
                                prodUnitSizeDesc = productionTicketFinishedItem.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    productionTicketFinishedItem.LocationCode,
                                    productionTicketFinishedItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.TicketMaterial:
                            if (this.ListViewMainActivity.SelectedItem is ProductionTicketMaterialItem
                                productionTicketMaterialItem)
                            {
                                prodLocCode = Convert.ToString(productionTicketMaterialItem.LocationCode);
                                prodLocName = productionTicketMaterialItem.LocationName;
                                prodDesc = productionTicketMaterialItem.ProductDescription;
                                prodUnitSizeDesc = productionTicketMaterialItem.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    productionTicketMaterialItem.LocationCode,
                                    productionTicketMaterialItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                            if (this.ListViewMainActivity.SelectedItem is PurchaseOrderWorkSheetItem
                                purchaseOrderWorkSheetItem)
                            {
                                prodLocCode = Convert.ToString(purchaseOrderWorkSheetItem.LocationCode);
                                prodLocName = purchaseOrderWorkSheetItem.LocationName;
                                prodDesc = purchaseOrderWorkSheetItem.ProductDescription;
                                prodUnitSizeDesc = purchaseOrderWorkSheetItem.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    purchaseOrderWorkSheetItem.LocationCode,
                                    purchaseOrderWorkSheetItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.PurgeActivityData:
                            break;

                        case SysCommon.ActSubTypes.PutAwaySkid:
                            break;

                        case SysCommon.ActSubTypes.Receiving:
                            if (this.ListViewMainActivity.SelectedItem is ReceiveItem receiveItem)
                            {
                                prodLocCode = Convert.ToString(receiveItem.LocationCode);
                                prodLocName = receiveItem.LocationName;
                                prodDesc = receiveItem.ProductDescription;
                                prodUnitSizeDesc = receiveItem.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    receiveItem.LocationCode,
                                    receiveItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.RefreshData:
                            break;

                        case SysCommon.ActSubTypes.ReleaseByItemSection:
                            break;

                        case SysCommon.ActSubTypes.ReleaseByOrder:
                            break;

                        case SysCommon.ActSubTypes.Returns:
                            if (this.ListViewMainActivity.SelectedItem is ReturnItem returnItem)
                            {
                                prodLocCode = Convert.ToString(returnItem.LocationCode);
                                prodLocName = returnItem.LocationName;
                                prodCode = returnItem.ProductCode;
                                prodDesc = returnItem.ProductDescription;
                                prodUnitSizeDesc = returnItem.ProductUnitSizeDescription;
                                prodBrandDesc = returnItem.ProductBrandDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    returnItem.LocationCode,
                                    returnItem.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            switch (this.SysEnv.SysSettings.PickItemDescOpt)
                            {
                                case 1:

                                    this.ExtraInfoItems.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " +
                                        prodBrandDesc.Trim());
                                    break;

                                default:

                                    this.ExtraInfoItems.Add("DESC: " + prodDesc.Trim() + ", " +
                                        prodUnitSizeDesc.Trim());
                                    break;
                            }

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.SelectLocation:
                            break;

                        case SysCommon.ActSubTypes.SelectProduct:
                            break;

                        case SysCommon.ActSubTypes.SkidInfo:
                            break;

                        case SysCommon.ActSubTypes.TransferFrom:
                            if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemFrom)
                            {
                                prodLocCode = Convert.ToString(transferItemFrom.LocationCode);
                                prodLocName = transferItemFrom.LocationName;
                                prodDesc = transferItemFrom.ProductDescription;
                                prodUnitSizeDesc = transferItemFrom.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    transferItemFrom.LocationCode,
                                    transferItemFrom.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());

                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.TransferTo:
                            if (this.ListViewMainActivity.SelectedItem is TransferItem transferItemTo)
                            {
                                prodLocCode = Convert.ToString(transferItemTo.LocationCode);
                                prodLocName = transferItemTo.LocationName;
                                prodDesc = transferItemTo.ProductDescription;
                                prodUnitSizeDesc = transferItemTo.ProductUnitSizeDescription;

                                // Get product default location info
                                var defLocMapRefno = await this.ActItem.GetDefaultLocMapRefno(
                                    transferItemTo.LocationCode,
                                    transferItemTo.ProductRefno);
                                var sLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                                    defLocMapRefno);
                                defLoc = LocationMapTasks.GetReadableLocation(sLocMap);
                            }

                            this.ExtraInfoItems.Add(prodDesc.Trim() + " " + prodUnitSizeDesc.Trim());
                            this.ExtraInfoItems.Add("Default Location: " + defLoc);
                            break;

                        case SysCommon.ActSubTypes.LoginStartup:
                            break;

                        case SysCommon.ActSubTypes.SplashStartup:
                            break;

                        case SysCommon.ActSubTypes.PrintProductLabel:
                            break;

                        case SysCommon.ActSubTypes.PickingSelectCriteria:
                            break;

                        case SysCommon.ActSubTypes.PickingFilter:
                            break;

                        case SysCommon.ActSubTypes.SubmitBatch:
                            break;

                        case SysCommon.ActSubTypes.CommunicationSetup:
                            break;

                        case SysCommon.ActSubTypes.SetupDefaults:
                            break;

                        case SysCommon.ActSubTypes.InventoryInquirySummary:
                            break;

                        case SysCommon.ActSubTypes.ConvertProduct:
                            break;

                        default:
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Option menu click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <remarks>Code lens reports no references yet this event does fire.</remarks>
        private async void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                _ = await Device.InvokeOnMainThreadAsync(() => this.CmdLocationText =
                    "Location: " + Environment.NewLine + currentReadableLocation);
            });

            var result = await this.DisplayActionSheet(
                "ActivityItem Options",
                "Close",
                null,
                this.OptionsMenuItems.ToArray());

            if (result == this.CmdLocationText && this.CmdLocationIsEnabled)
            {
                await CmdLocationMenuClicked();
            }
            else if (result == this.CmdFilterText && this.CmdFilterIsEnabled)
            {
            }
            else if (result == this.CmdSelectSkidText && this.CmdSelectSkidIsEnabled)
            {
                await CmdSelectSkidClicked();
            }
            else if (result == this.CmdAddProductText && this.CmdAddProductIsEnabled)
            {
                await CmdAddProductClicked();
            }
            else if (result == this.CmdAddSkidText && this.CmdAddSkidIsEnabled)
            {
                await CmdAddSkidClicked();
            }
            else if (result == this.CmdClearSelectedSkidText && this.CmdClearSelectedSkidIsEnabled)
            {
                await CmdClearSelectedSkidClicked();
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdLocationMenuClicked()
            {
                try
                {
                    var dspOverrideMsg = false;
                    await this.ActDisappearing();
                    if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                    {
                        if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts &&
                                this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                            || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts &&
                                this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                        {
                            dspOverrideMsg = false;
                        }
                        else
                        {
                            dspOverrideMsg = true;
                        }
                    }

                    if (dspOverrideMsg)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                            await Application.Current.MainPage.DisplayAlert(
                                this.ActItem.GetControlDescription(),
                                "Default locations are enabled, location changes may be overridden.",
                                Resource.OK));
                    }

                    var form = new FormDialogContainer(
                        EDialogTypes.SelectLocationMap,
                        SysCommon.ActSubTypes.NotApplicable);
                    if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                    {
                        this.LocMap.CurrLocMap = this.LocMap.ChosenLocMap.Clone();

                        // Special validation for picking and filtering by section.
                        // Check if the scanned section matched the filtered one.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                            if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                            {
                                var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                                var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                          $" not match the filtered picking section '{sectName}'.";

                                await Device.InvokeOnMainThreadAsync(async () =>
                                        await Application.Current.MainPage.DisplayAlert(
                                            this.ActItem.GetControlDescription(),
                                            msg,
                                            Resource.OK));
                            }
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdSelectSkidClicked()
            {
                try
                {
                    // Needed to check if the enter key was pressed last, because it seems that
                    // SelectNextControl in the key press event causes a click event to fire
                    // if a command button is the next control rather than just giving the button focus.
                    await this.ActDisappearing();
                    var form = new FormDialogContainer(
                        EDialogTypes.SkidInfo,
                        this.ActItem,
                        this.ActSubType);
                    _ = await form.DisplayControl.ShowDialog();

                    // Current location was assigned in the form.
                    var xmlDoc = await form.GetXML();
                    this.ActItem.ActHdr.SkidInfo = await SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        if (this.ActItem.SelectedActivityItem != null)
                        {
                            // Needed to refresh the selected item location, called SelectItem.
                            await this.ActItem.AssignActivityItem(this.ActItem.SelectedActivityItem);
                            this.CmdClearSelectedSkidIsEnabled = false;
                        }
                    }
                    else
                    {
                        var currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                        if (this.ActItem.ProdInfo.Code == currSkid.ProdInfo.Code &&
                            this.ActItem.ProdInfo.UnitBC == currSkid.ProdInfo.UnitBC &&
                            this.ActItem.ProdInfo.UnitCode == currSkid.ProdInfo.UnitCode)
                        {
                            this.CmdAddSkidIsEnabled = true;

                            this.CmdSelectSkidText = "Skid# " + currSkid.SkidCode;
                            this.CmdLocationIsEnabled = false;
                        }
                        else
                        {
                            await Device.InvokeOnMainThreadAsync(async () => await this.DisplayAlert(
                                "Skid",
                                "Skid product must match the selected line item product.",
                                "OK"));

                            this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                            this.CmdSelectSkidText = "Select Skid";
                            this.CmdAddSkidIsEnabled = false;
                        }

                        // Special validation for picking and filtering by section.
                        // Check if the scanned section matched the filtered one.
                        if (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            var sectRefno = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectLocMapRefno;
                            if (sectRefno != 0 && this.LocMap.ChosenLocMap.SectLocMapRefno != sectRefno)
                            {
                                var sectName = ((PickingActivity)this.ActItem.ActHdr).PickingLocationMapFilter.SectName;
                                var msg = $"Warning selected location section '{this.LocMap.ChosenLocMap.SectName}' does" +
                                          $" not match the filtered picking section '{sectName}'.";

                                await Device.InvokeOnMainThreadAsync(async () =>
                                        await Application.Current.MainPage.DisplayAlert(
                                            this.ActItem.GetControlDescription(),
                                            msg,
                                            Resource.OK));
                            }
                        }

                        this.CmdClearSelectedSkidIsEnabled = true;
                        var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(
                            this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno);

                        // TODO: Sorting example
                        this.ActItem.ListItems = this.ActItem.ListItems.OrderBy(item =>
                            item.GetType().GetProperty("ActivityItemRefno")?.GetValue(item)).ToList();

                        if (actItemFoundRefno > 0)
                        {
                            ////int foundRowIdx = this.ActItem.ListItems.Find(actItemFoundRefno);
                            var selectedItem = this.ActItem.ListItems.FirstOrDefault(act =>
                                Convert.ToString(act.GetType().GetProperty("ActivityItemRefno")?.GetValue(act)) ==
                                Convert.ToString(actItemFoundRefno));

                            if (selectedItem != null)
                            {
                                await this.ActItem.AssignActivityItem(selectedItem);
                            }
                        }
                    }
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdClearSelectedSkidClicked()
            {
                try
                {
                    await this.ActDisappearing();
                    this.ActItem.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                    this.CmdSelectSkidIsEnabled = true;
                    this.CmdClearSelectedSkidIsEnabled = false;
                    this.CmdAddSkidIsEnabled = false;

                    // Expected menu to be recreated.
                    this.CmdSelectSkidText = "Select Skid";
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdAddProductClicked()
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                try
                {
                    await this.ActDisappearing();

                    XDocument xmlDoc = null;
                    ProductInfo prodInfo = null;
                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        var form = new FormDialogContainer(
                            EDialogTypes.SelectProductUnit,
                            this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
                        {
                            xmlDoc = await form.GetXML();
                            var prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                            if (prodUnitRefno == 0)
                            {
                                throw new MessageOnlyException("A product unit must be chosen.");
                            }

                            prodInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                        }
                        else
                        {
                            throw new CancelOperationException();
                        }
                    }
                    else
                    {
                        prodInfo = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo;
                        var unitSizeDesc = SysCommon.AddXMLReservedChars(prodInfo.UnitSizeDesc);

                        xmlDoc = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + "<ProductUnitRefno>" + prodInfo.UnitRefno + "</ProductUnitRefno>"
                            + "<ProductRefno>" + prodInfo.Refno + "</ProductRefno>"
                            + "<ProductCode>" + prodInfo.Code + "</ProductCode>"
                            + "<ProductDescription>" + SysCommon.AddXMLReservedChars(prodInfo.Desc) + "</ProductDescription>"
                            + "<UnitCode>" + prodInfo.UnitCode + "</UnitCode>"
                            + "<BaseUnitCode>" + prodInfo.BaseUnitCode + "</BaseUnitCode>"
                            + "<ProductUnitBC>" + prodInfo.UnitBC + "</ProductUnitBC>"
                            + "<ProductUnitSizeDescription>" + unitSizeDesc + "</ProductUnitSizeDescription>"
                            + "</params>");
                    }

                    if (!await LocationMapTasks.IsCompleteLocMap(this.LocMap.CurrLocMap))
                    {
                        throw new MessageOnlyException("Location must be set.");
                    }

                    var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodInfo.UnitRefno);
                    if (actItemFoundRefno > 0)
                    {
                        this.ActItem.SetSelectedActivityItem(actItemFoundRefno);
                    }
                    else
                    {
                        if (!this.ActItem.IsAllowAddItem)
                        {
                            throw new MessageOnlyException(
                                "Adding new line items for this activity is currently disabled.");
                        }

                        // After insert this.ActItem.ItemFKRefno may be a negative value if a new line item was created.
                        await this.ActItem.Insert(xmlDoc, false);
                        this.ActItem.SetSelectedActivityItem(this.ActItem.ItemRefno);
                        this.ListViewMainActivity.SelectedItem = this.ActItem.SelectedActivityItem;

                        // This is a little dangerous we assume that the new row idx will be the last one
                        // selectedItem = this.ActItem.ListItems.Last();
                        // Try not to refresh all the items.
                        ////this.ActItem.RegLayerListenerProgOut(this);
                        ////await this.RefreshBySubType();
                        ////this.ActItem.UnregLayerListenerProgOut(this);
                        await this.DesignGrid();

                        await this.ActAppearing();

                    }
                }
                catch (CancelOperationException)
                {
                }
                catch (GatherProductDataException ex)
                {
                    if (!ex.SuppressError)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var errRpt = new FormErrorReport(this.ActSubType, ex);
                            _ = await errRpt.ShowDialog();
                        });
                    }

                    await this.RefreshBySubType();
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });

                    await this.RefreshBySubType();
                }
                finally
                {
                    if (this.ActItem.ListItems.Count > 0)
                    {
                        this.ActItem.SelectedActivityItem = this.ActItem.ListItems.LastOrDefault();
                    }

                    await this.ActAppearing();
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task CmdAddSkidClicked()
            {
                // Needed to check if the enter key was pressed last, because it seems that
                // SelectNextControl in the key press event causes a click event to fire
                // if a command button is the next control rather than just giving the button focus.
                try
                {
                    await this.ActDisappearing();
                    if (await this.DisplayAlert(
                        "Add Skid",
                        "Add the ENTIRE skid to this activity?",
                        "Yes",
                        "No"))
                    {
                        var prodUnitRefno = this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        var actItemFoundRefno = await this.ActItem.GetActivityItemRefno(prodUnitRefno);
                        object selectedItem = null;
                        if (actItemFoundRefno > 0)
                        {
                            ////foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            selectedItem = this.ActItem.ListItems.FirstOrDefault(actItem =>
                                Convert.ToString(actItem.GetType().GetProperty(
                                    "ActivityItemRefno")?.GetValue(actItem)) ==
                                Convert.ToString(actItemFoundRefno));
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                var xml = ProductInfoTasks.GetXML(
                                    this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo);
                                await this.ActItem.Insert(xml, false);

                                // This is a little dangerous we assume that the new row idx
                                // will be the last one
                                ////foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                                selectedItem = this.ActItem.ListItems.LastOrDefault();
                                actItemFoundRefno = this.ActItem.ItemRefno;
                            }
                            else
                            {
                                throw new MessageOnlyException(
                                    this.ActItem.GetItemNotFoundMessage(
                                        this.LocMap,
                                        this.ActItem.ActHdr.SkidInfo.CurrSkid.ProdInfo));
                            }
                        }

                        this.ActItem.SelectedActivityItem = selectedItem;

                        // INSERT A NEW ACTIVITY DETAIL RECORD
                        var actDetail = ActDetailFacade.GetActDetail(
                            this.ActItem,
                            this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode
                        // because of the length of the code and the fact is isn't human readable friendly.
                        var schemeData = actDetail.SchemeData;
                        schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        actDetail.SchemeData = schemeData;
                        actDetail.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);
                        actDetail.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate
                        // checks and quantity validations.
                        var xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                            + "<alt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>"
                            + "<wt_qty>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>"
                            + "<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>"
                            + "</imclient>");

                        await this.ActItem.IsReadyToApply(xmlParam);
                        await actDetail.InsertSkid(this.ActItem.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to to show new
                        // item and quantity change
                        this.ActItem.AddToQuantityInActivityItem(
                            this.ActItem.SelectedActivityItem,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        this.ListViewMainActivity.SelectedItem = selectedItem;
                        this.MainActivityItems[this.ActItem.SelectedListItemIdx] = selectedItem;
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
                }
                catch (MessageOnlyException ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                        await Application.Current.MainPage.DisplayAlert(
                            this.ActItem.GetControlDescription(),
                            ex.Message,
                            Resource.OK));
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var errRpt = new FormErrorReport(this.ActSubType, ex);
                        _ = await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ActAppearing();
                }
            }
        }
    }
}
