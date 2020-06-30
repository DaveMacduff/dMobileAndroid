using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Sockets;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.Forms;
using static Android.Views.View;
using Android.Views;
using System.Collections.ObjectModel;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages.Modal;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityHdrSkidInfo : DMobileModal, IAppArea, IAppPublisher
    {
        //private UserControl callingControl = null;
        private bool isChangesMade = false;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private bool isNumericTrapped = false;
        private bool isEditTrapped = false;
        private int prodUnitRefno = 0;
        private SysSkidInfo skidInfo = new SysSkidInfo();
        private SysCommon.ActSubTypes callingActSubType = SysCommon.ActSubTypes.SkidInfo;
        private int savedLocMapRefno = 0;
        private SLocMap savedLocMap = new SLocMap();
        private SysSkidInfo savedSkidInfo = new SysSkidInfo();
        private bool isNewSkid = false;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private bool isControlLoaded = false;
        private int callingActFKRefno = 0;
        private int callingActItemFKRefno = 0;
        private bool isSkidCodeChanged = false;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private ObservableCollection<object> dataGridMainActivityItemsList = new ObservableCollection<object>();

        public ucActivityHdrSkidInfo()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SavedLocMap = this.LocMap.CurrLocMap;
            ////this.CallingActSubType = SysCommon.ActSubTypes.NotApplicable;
            this.CallingActSubType = SysCommon.ActSubTypes.SkidInfo;
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.IsNewSkid = true;

            this.updnBarCodeModifier1.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.updnBarCodeModifier1.Items.Add(" ");
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (string item in modifiers)
                {
                    this.updnBarCodeModifier1.Items.Add(item);
                }
            }
            else
            {
                this.updnBarCodeModifier1.IsVisible = false;
                this.updnBarCodeModifier1.IsEnabled = false;
            }

            this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            this.DisplayData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        /// <param name="callingActSubType"></param>
        /// <param name="activity"></param>
        public ucActivityHdrSkidInfo(SysCommon.ActSubTypes callingActSubType, Controller.Activity activity)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SavedLocMap = this.LocMap.CurrLocMap;
            //this.CallingControl = callingControl;
            this.CallingActSubType = callingActSubType;
            this.CallingActFKRefno = activity.FKRefno;
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;

            this.updnBarCodeModifier1.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.updnBarCodeModifier1.Items.Add(" ");
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (string item in modifiers)
                {
                    this.updnBarCodeModifier1.Items.Add(item);
                }
            }
            else
            {
                this.updnBarCodeModifier1.IsVisible = false;
                this.updnBarCodeModifier1.IsEnabled = false;
            }

            this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            this.IsNewSkid = true;

            if (activity.SkidInfo != null && activity.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                try
                {
                    this.IsNewSkid = false;
                    this.SavedSkidInfo = activity.SkidInfo;

                    // Update the skid info from dtec. e.g. Bucket totals may have been updated.
                    this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(activity.SkidInfo.CurrSkid.SkidCode.TrimEnd());

                    // Update the saved skid, to reflect dtec changes after the call GetSkidInfoFromBarCodeOrCode.
                    // The changes in dtec are valid.
                    this.SavedSkidInfo = activity.SkidInfo;
                    this.EnableControls();
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    this.SkidInfo = activity.SkidInfo;
                    ex.Show(SysCommon.ActSubTypes.SkidInfo);
                }
                catch (Exception ex)
                {
                    this.SkidInfo = activity.SkidInfo;
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    errRpt.ShowDialog();
                }

                if (activity.SkidInfo == null || activity.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    this.LocMap.CurrLocMap = this.SkidInfo.CurrSkid.LocMap;
                    this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                }
            }

            this.DisplayData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingControl"></param>
        /// <param name="callingActSubType"></param>
        /// <param name="actItem"></param>
        public ucActivityHdrSkidInfo(SysCommon.ActSubTypes callingActSubType, ActItem actItem)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdComplete.Text = "Build";
            this.cmdDetail.Text = "Detail";
            this.cmdSkidProduct.Text = "Select Product";
            this.cmdLocation.Text = "Set Location";
            this.lblCubeUOM.Text = "UOM";
            this.lblCube.Text = "Cube";
            this.lblHi.Text = "Hi";
            this.lblTi.Text = "Ti";
            this.cmdNewSkid.Text = "New";
            this.lblIsPutAway.Text = "Is Put Away";
            this.lblHeightUOM.Text = "UOM";
            this.lblHeight.Text = "Height";
            this.cmdSkidNo.Text = "Skid #";
            this.pictureBoxPrintLabel.Text = "Print";

            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.SavedLocMap = this.LocMap.CurrLocMap;
            //this.CallingControl = callingControl;
            this.CallingActSubType = callingActSubType;
            this.CallingActFKRefno = actItem.ActHdr.FKRefno;
            this.CallingActItemFKRefno = actItem.FKRefno;
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;

            this.updnBarCodeModifier1.Items.Clear();

            if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
            {
                this.updnBarCodeModifier1.Items.Add(" ");
                string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                foreach (string item in modifiers)
                {
                    this.updnBarCodeModifier1.Items.Add(item);
                }
            }
            else
            {
                this.updnBarCodeModifier1.IsVisible = false;
                this.updnBarCodeModifier1.IsEnabled = false;
            }

            this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

            this.IsNewSkid = true;

            // No active skid selected
            if (actItem.ActHdr.SkidInfo == null || actItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
            {
                // Default the location from the line item object.
                this.LocMap.CurrLocMap = actItem.LocMap.CurrLocMap;

                // Default the potential new skid product unit from the line item object.
                SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                tempSkidInfo.LocMap = this.LocMap.CurrLocMap;

                if (actItem.CurrRowIdx >= 0 && actItem.DataViewItems != null)
                {
                    this.ProdUnitRefno = Convert.ToInt32(actItem.DataViewItems[actItem.CurrRowIdx]["ProductUnitRefno"]);

                    tempSkidInfo.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                    tempSkidInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo.ProdInfo.Refno);
                }
                else
                {
                    tempSkidInfo.ProdInfo = new ProductInfo();
                }

                // Assign the current skid object with the defaults.
                this.SkidInfo.CurrSkid = tempSkidInfo;
            }
            else
            {
                try
                {
                    this.IsNewSkid = false;
                    this.SavedSkidInfo = actItem.ActHdr.SkidInfo;

                    // Update the skid info from dtec. e.g. Bucket totals may have been updated.
                    this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(actItem.ActHdr.SkidInfo.CurrSkid.SkidCode.TrimEnd());

                    // Update the saved skid, to reflect dtec changes after the call GetSkidInfoFromBarCodeOrCode.
                    // The changes in dtec are valid.
                    this.SavedSkidInfo = actItem.ActHdr.SkidInfo;
                    this.EnableControls();
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    this.SkidInfo = actItem.ActHdr.SkidInfo;
                    ex.Show(SysCommon.ActSubTypes.SkidInfo);
                }
                catch (Exception ex)
                {
                    this.SkidInfo = actItem.ActHdr.SkidInfo;
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    errRpt.ShowDialog();
                }

                if (actItem.ActHdr.SkidInfo == null || actItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    this.LocMap.CurrLocMap = this.SkidInfo.CurrSkid.LocMap;
                    this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                }
            }

            this.DisplayData();
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape)
                    {
                        if (await this.CancelChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.F1)
                    {
                        if (await this.SaveChanges())
                        {
                            this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                        }
                    }
                    else if (keycode == Keycode.F5)
                    {
                        this.ScannerDisable();
                        //DialogResult result = MessageBox.Show("Do you wish to break this skid?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        //if (result == DialogResult.Yes)
                        // TODO: Resource strings
                        if (await DisplayAlert(this.GetControlDescription(), "Do you wish to break this skid?", "Yes", "No"))
                        {
                            try
                            {
                                SysSkidInfo.Break(this.SkidInfo);

                                //MessageBox.Show("This skid is now broken.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                                // TODO: Resource strings
                                await DisplayAlert(this.GetControlDescription(), "This skid is now broken.", "OK");

                                this.SkidInfo = new SysSkidInfo();
                                this.ProdUnitRefno = 0;
                                this.LocMap.CurrLocMap = this.SavedLocMap;
                                this.IsNewSkid = true;
                            }
                            catch (Exception ex)
                            {
                                await Device.InvokeOnMainThreadAsync(async () =>
                                {
                                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                                    await errorReport.ShowDialog();
                                });
                            }
                        }

                        this.ScannerEnable();
                    }
                }
            });

            try
            {    
                this.ucActivityHdrSkidInfo_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert("Error", "this.ucActivityHdrSkidInfo_GotFocus threw an error " + ex, "OK");
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        public event ReqCloseHandler ReqClose
        {
            add
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Combine(this.ReqCloseEvent, value);
            }

            remove
            {
                this.ReqCloseEvent = (ReqCloseHandler)Delegate.Remove(this.ReqCloseEvent, value);
            }
        }

        public event ReqSaveEnableHandler ReqSaveEnable
        {
            add
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Combine(this.ReqSaveEnableEvent, value);
            }

            remove
            {
                this.ReqSaveEnableEvent = (ReqSaveEnableHandler)Delegate.Remove(this.ReqSaveEnableEvent, value);
            }
        }

        public event ReqSaveDisableHandler ReqSaveDisable
        {
            add
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Combine(this.ReqSaveDisableEvent, value);
            }

            remove
            {
                this.ReqSaveDisableEvent = (ReqSaveDisableHandler)Delegate.Remove(this.ReqSaveDisableEvent, value);
            }
        }

        public event ReqCancelEnableHandler ReqCancelEnable
        {
            add
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Combine(this.ReqCancelEnableEvent, value);
            }

            remove
            {
                this.ReqCancelEnableEvent = (ReqCancelEnableHandler)Delegate.Remove(this.ReqCancelEnableEvent, value);
            }
        }

        public event ReqCancelDisableHandler ReqCancelDisable
        {
            add
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Combine(this.ReqCancelDisableEvent, value);
            }

            remove
            {
                this.ReqCancelDisableEvent = (ReqCancelDisableHandler)Delegate.Remove(this.ReqCancelDisableEvent, value);
            }
        }

        public SysCommon.ActSubTypes CallingActSubType
        {
            get
            {
                return this.callingActSubType;
            }

            set
            {
                this.callingActSubType = value;
            }
        }

        private bool IsSkidCodeChanged
        {
            get
            {
                return this.isSkidCodeChanged;
            }

            set
            {
                this.isSkidCodeChanged = value;
            }
        }

        private int CallingActFKRefno
        {
            get
            {
                return this.callingActFKRefno;
            }

            set
            {
                this.callingActFKRefno = value;
            }
        }

        private int CallingActItemFKRefno
        {
            get
            {
                return this.callingActItemFKRefno;
            }

            set
            {
                this.callingActItemFKRefno = value;
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private BarCodeScanner.BarCodeScanner Scanner
        {
            get
            {
                return this.scanner;
            }

            set
            {
                this.scanner = value;
            }
        }

        private ReqCloseHandler ReqCloseEvent
        {
            get
            {
                return this.reqCloseEvent;
            }

            set
            {
                this.reqCloseEvent = value;
            }
        }

        private ReqSaveEnableHandler ReqSaveEnableEvent
        {
            get
            {
                return this.reqSaveEnableEvent;
            }

            set
            {
                this.reqSaveEnableEvent = value;
            }
        }

        private ReqSaveDisableHandler ReqSaveDisableEvent
        {
            get
            {
                return this.reqSaveDisableEvent;
            }

            set
            {
                this.reqSaveDisableEvent = value;
            }
        }

        private ReqCancelEnableHandler ReqCancelEnableEvent
        {
            get
            {
                return this.reqCancelEnableEvent;
            }

            set
            {
                this.reqCancelEnableEvent = value;
            }
        }

        private ReqCancelDisableHandler ReqCancelDisableEvent
        {
            get
            {
                return this.reqCancelDisableEvent;
            }

            set
            {
                this.reqCancelDisableEvent = value;
            }
        }

        private bool IsNewSkid
        {
            get
            {
                return this.isNewSkid;
            }

            set
            {
                this.isNewSkid = value;
            }
        }

        private SysSkidInfo SavedSkidInfo
        {
            get
            {
                return this.savedSkidInfo;
            }

            set
            {
                this.savedSkidInfo = value;
            }
        }

        private SLocMap SavedLocMap
        {
            get
            {
                return this.savedLocMap;
            }

            set
            {
                this.savedLocMap = value;
            }
        }

        private int SavedLocMapRefno
        {
            get
            {
                return this.savedLocMapRefno;
            }

            set
            {
                this.savedLocMapRefno = value;
            }
        }

        private int ProdUnitRefno
        {
            get
            {
                return this.prodUnitRefno;
            }

            set
            {
                this.prodUnitRefno = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        private bool IsBarCodeHandlerRegistered
        {
            get
            {
                return this.isBarCodeHandlerRegistered;
            }

            set
            {
                this.isBarCodeHandlerRegistered = value;
            }
        }

        private bool IsChangesMade
        {
            get
            {
                return this.isChangesMade;
            }

            set
            {
                this.isChangesMade = value;
            }
        }

        //private UserControl CallingControl
        //{
        //    get
        //    {
        //        return this.callingControl;
        //    }

        //    set
        //    {
        //        this.callingControl = value;
        //    }
        //}

        private bool IsNumericTrapped
        {
            get
            {
                return this.isNumericTrapped;
            }

            set
            {
                this.isNumericTrapped = value;
            }
        }

        private bool IsEditTrapped
        {
            get
            {
                return this.isEditTrapped;
            }

            set
            {
                this.isEditTrapped = value;
            }
        }

        private SysSkidInfo SkidInfo
        {
            get
            {
                return this.skidInfo;
            }

            set
            {
                this.skidInfo = value;
            }
        }

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public ObservableCollection<object> DataGridMainActivityItemsList { get => dataGridMainActivityItemsList; set => dataGridMainActivityItemsList = value; }

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                if (this.IsChangesMade)
                {
                    if (!LocationMapTasks.IsCompleteLocMapRefno(this.SkidInfo.CurrSkid.LocMap))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("The skid location must be set.");
                    }

                    if (0 >= this.ProdUnitRefno)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("A product must be assigned to a skid.");
                    }

                    ////if (this.SkidInfo.CurrSkid.SkidFKRefno == 0 || string.IsNullOrEmpty(this.txtSkidCode.Text)
                    ////    || this.SavedSkidInfo.CurrSkid.SkidCode != txtSkidCode.Text)
                    ////{
                    ////  this.IsNewSkid = true;
                    ////}
                    ////else
                    ////{
                    ////  this.IsNewSkid = false;
                    ////}

                    if (this.SysEnv.SysSettings.IsPickDisableMakeSkid && this.IsNewSkid && (this.CallingActSubType == SysCommon.ActSubTypes.Picking || this.CallingActSubType == SysCommon.ActSubTypes.PickingSingle))
                    {
                        ////throw new Exceptions.MessageOnlyException("Making a new skid has been restricted. Operation disallowed.");

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Making a new skid has been disabled.");
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.CallingActSubType, ex);
                    await errRpt.ShowDialog();
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
            bool isReadyToClose = true;

            try
            {
                if (this.IsChangesMade)
                {
                    // Perform any checks or changes needed to allow a rollback or form close.
                    // Set IsReadyToClose = false if checks fail.
                }

                if (isReadyToClose)
                {
                    if (this.SkidInfo.Equals(this.SavedSkidInfo))
                    {
                        this.IsChangesMade = false;
                    }

                    // NOTE: This code is needed to clean up the scanner driver.
                    // Symbol had threading issues without this call when dMobile was terminating.
                    // All user controls that initalize a bar code need this code.
                    // The parent form (not a user control) will call scanner.terminate and dispose
                    // of the scanner instance upon exiting dMobile.
                    this.ScannerDisable();
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.CallingActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo)
        //    {
        //        return new ucInventoryMenu();
        //    }
        //    else
        //    {
        //        return this.CallingControl;
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Skid Info";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Clear any properties.
                    this.LocMap.CurrLocMap = this.SavedLocMap;

                    // Rather than restore the selected skid to the saved original one clear the selected skid instead.
                    this.SkidInfo = new SysSkidInfo();

                    this.DisplayData();
                    this.IsChangesMade = false;

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
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errorReport = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    await errorReport.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public XDocument GetXML()
        {
            XDocument xmlDoc = null;
            SSkidInfo skidInfo = this.SkidInfo.CurrSkid;

            if (0 == skidInfo.SkidFKRefno)
            {
                DateTime dt = new DateTime(1900, 1, 1);
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + "<skid_fk_refno>0</skid_fk_refno>"
                    + "<chep>false</chep>"
                    + "<hold>false</hold>"
                    + "<base_quantity>0</base_quantity>"
                    + "<cube>0</cube>"
                    + "<height>0</height>"
                    + "<quantity>0</quantity>"
                    + "<weight_quantity>0</weight_quantity>"
                    + "<hi>0</hi>"
                    + "<ti>0</ti>"
                    + "<cube_unit></cube_unit>"
                    + "<height_unit></height_unit>"
                    + "<logistic_unit_skid_code></logistic_unit_skid_code>"
                    + "<skid_code></skid_code>"
                    + "<location_map_refno>0</location_map_refno>"
                    + "<product_unit_refno>0</product_unit_refno>"
                    + "<completed>false</completed>"
                    + "<consumed>false</consumed>"
                    + "<putaway>false</putaway>"
                    + "<logistic_unit_batch_lot_number></logistic_unit_batch_lot_number>"
                    + "<logistic_unit_best_before_sell_by_date>" + dt + "</logistic_unit_best_before_sell_by_date>"
                    + "<logistic_unit_production_date>" + dt + "</logistic_unit_production_date>"
                    + "<product_scheme_variant_no>0</product_scheme_variant_no>"
                    + "<product_scheme_variant_code></product_scheme_variant_code>"
                    + "<product_scheme_variant_description></product_scheme_variant_description>"
                    + "<act_sub_type>" + (int)this.CallingActSubType + "</act_sub_type>"
                    + "<act_fk_refno>0</act_fk_refno>"
                    + "<actitem_fk_refno>0</actitem_fk_refno>"
                    + "</params>");
            }
            else
            {
                xmlDoc = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<params>"
                    + ("<skid_fk_refno>" + skidInfo.SkidFKRefno + "</skid_fk_refno>")
                    + ("<chep>" + skidInfo.IsCHEP + "</chep>")
                    + ("<hold>" + skidInfo.IsHold + "</hold>")
                    + ("<base_quantity>" + skidInfo.BaseQuantity + "</base_quantity>")
                    + ("<cube>" + skidInfo.Cube + "</cube>")
                    + ("<height>" + skidInfo.Height + "</height>")
                    + ("<quantity>" + skidInfo.Quantity + "</quantity>")
                    + ("<weight_quantity>" + skidInfo.WeightQuantity + "</weight_quantity>")
                    + ("<hi>" + skidInfo.Hi + "</hi>")
                    + ("<ti>" + skidInfo.Ti + "</ti>")
                    + ("<cube_unit>" + skidInfo.CubeUnit + "</cube_unit>")
                    + ("<height_unit>" + skidInfo.HeightUnit + "</height_unit>")
                    + ("<logistic_unit_skid_code>" + skidInfo.LogisticUnitSkidCode + "</logistic_unit_skid_code>")
                    + ("<skid_code>" + skidInfo.SkidCode + "</skid_code>")
                    + ("<location_map_refno>" + skidInfo.LocMapRefno + "</location_map_refno>")
                    + ("<product_unit_refno>" + skidInfo.ProdInfo.UnitRefno + "</product_unit_refno>")
                    + ("<completed>" + skidInfo.IsCompleted + "</completed>")
                    + ("<consumed>" + skidInfo.IsConsumed + "</consumed>")
                    + ("<putaway>" + skidInfo.IsPutAway + "</putaway>")
                    + "<logistic_unit_batch_lot_number>" + skidInfo.SchemeData.LogisticUnitBatchLotNumber
                    + "</logistic_unit_batch_lot_number>"
                    + "<logistic_unit_best_before_sell_by_date>" + skidInfo.SchemeData.LogisticUnitBestBeforeSellByDate
                    + "</logistic_unit_best_before_sell_by_date>"
                    + "<logistic_unit_production_date>" + skidInfo.SchemeData.LogisticUnitProductionDate
                    + "</logistic_unit_production_date>"
                    + "<product_scheme_variant_no>" + skidInfo.SchemeData.VariantNumber + "</product_scheme_variant_no>"
                    + "<product_scheme_variant_code>" + skidInfo.SchemeData.VariantCode + "</product_scheme_variant_code>"
                    + "<product_scheme_variant_description>" + skidInfo.SchemeData.VariantDescription
                    + "</product_scheme_variant_description>"
                    + "</params>");
            }

            return xmlDoc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if ((this.callingActSubType == SysCommon.ActSubTypes.Picking || this.CallingActSubType == SysCommon.ActSubTypes.PickingSingle)
                    && !string.IsNullOrEmpty(this.txtSkidCode.Text)
                    && this.SkidInfo.CurrSkid.SkidCode != this.txtSkidCode.Text.PadRight(20))
                {
                    this.txtSkidCode_KeyUpEnter(this.txtSkidCode, new KeyEventArgs(false, Keycode.Enter, new KeyEvent(KeyEventActions.Up, Keycode.Enter)));
                }

                if (await this.IsReadyToSave())
                {
                    this.SaveSkid();

                    if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo)
                    {
                        //if (MessageBox.Show("Do you wish to exit?", "Skid Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        // TODO: Resource strings
                        if (await DisplayAlert("Skid Info", "Do you wish to exit?", "Yes", "No"))
                        {
                            saveComplete = true;
                        }
                        else
                        {
                            saveComplete = false;
                        }
                    }
                    else
                    {
                        saveComplete = true;
                    }

                    if (!saveComplete)
                    {
                        this.DisplayData();
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose += new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqClose(IAppListener appEventListener)
        {
            this.ReqClose -= new ReqCloseHandler(appEventListener.ReqClose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable += new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveEnable(IAppListener appEventListener)
        {
            this.ReqSaveEnable -= new ReqSaveEnableHandler(appEventListener.ReqSaveEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqSaveDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqSaveDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable += new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelEnable(IAppListener appEventListener)
        {
            this.ReqCancelEnable -= new ReqCancelEnableHandler(appEventListener.ReqCancelEnable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void RegisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable += new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appEventListener"></param>
        public void UnregisterReqCancelDisable(IAppListener appEventListener)
        {
            this.ReqSaveDisable -= new ReqSaveDisableHandler(appEventListener.ReqCancelDisable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="scanArgs"></param>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            BarCodeParse codeParse = null;

            try
            {
                this.ScannerDisable();

                codeParse = scanArgs.ParseInfo;

                // global location identifier scanned
                if (!string.IsNullOrEmpty(codeParse.LocationIdentifier))
                {
                    if (this.txtSkidCode.Text != this.SkidInfo.CurrSkid.SkidCode)
                    {
                        SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                        int locMapRefno = LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(codeParse.LocationIdentifier);

                        if (locMapRefno != 0)
                        {
                            tempSkidInfo.LocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                            this.SkidInfo.CurrSkid = tempSkidInfo;
                        }
                        else
                        {
                            tempSkidInfo.LocMapRefno = 0;
                        }

                        this.DisplayData();
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a location identifier can only be done with a new skid.");
                    }
                }

                // Activity item fk refno
                if (!string.IsNullOrEmpty(codeParse.ActItemFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning a line item bar code is not supported here!");
                }

                // Activity detail fk refno
                if (!string.IsNullOrEmpty(codeParse.ActDetailFKRefno))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here!");
                }

                // Shipping container scanned, skid
                if (!string.IsNullOrEmpty(codeParse.SSCC18))
                {
                    try
                    {
                        this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(codeParse.SSCC18.TrimEnd());
                        this.LocMap.CurrLocMap = this.SkidInfo.CurrSkid.LocMap;
                        this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        this.IsNewSkid = false;
                        this.EnableControls();
                    }
                    catch (Exception ex)
                    {
                        this.IsNewSkid = true;
                        this.ProdUnitRefno = 0;
                        this.LocMap.CurrLocMap = this.SavedLocMap;
                        //MessageBox.Show(ex.Message + Environment.NewLine + "A new skid will be created upon saving.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        // TODO: Resource strings
                        await DisplayAlert(this.GetControlDescription(), ex.Message + Environment.NewLine + "A new skid will be created upon saving.", "OK");
                        this.SkidInfo = new SysSkidInfo();
                        SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                        tempSkidInfo.LocMapRefno = this.SavedLocMapRefno;
                        tempSkidInfo.LocMap = this.SavedLocMap;
                        tempSkidInfo.ProdInfo = null;
                        this.SkidInfo.CurrSkid = tempSkidInfo;
                    }

                    this.DisplayData();
                }

                // Product identifier
                if (!string.IsNullOrEmpty(codeParse.ProductIdentifier))
                {
                    if (this.SkidInfo.CurrSkid.SkidFKRefno == 0 || this.SkidInfo.CurrSkid.SkidFKRefno != this.SavedSkidInfo.CurrSkid.SkidFKRefno)
                    {
                        SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                        int tempProdUnitRefno = this.ProdUnitRefno;
                        tempProdUnitRefno = await SysShared.IdentifyProductScanned(this.CallingActSubType, codeParse, tempProdUnitRefno);

                        this.ProdUnitRefno = tempProdUnitRefno;

                        if (this.ProdUnitRefno != 0)
                        {
                            tempSkidInfo.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                            tempSkidInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(
                                tempSkidInfo.ProdInfo.Refno);
                            this.SkidInfo.CurrSkid = tempSkidInfo;

                            //<issue="13609" author="RD" date="2016/02/22"/>
                            SysShared.AssertSuppressedErrorsAgainstScheme(this.SkidInfo.CurrSkid.ProdInfo, codeParse);
                        }
                        else
                        {
                            tempSkidInfo.ProdInfo = null;
                        }

                        this.DisplayData();
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning a product identifier can only be done with a new skid.");
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                await errRpt.ShowDialog();
            }
            finally
            {
                this.ScannerEnable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void pictureBoxPrintLabel_Click(object sender, EventArgs e)
        {
            try
            {
                if (await this.IsReadyToSave())
                {
                    this.SaveSkid();
                    this.DisplayData();

                    //if (MessageBox.Show("Print skid label?", this.GetControlDescription(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    // TODO: Resource strings
                    if (await DisplayAlert(this.GetControlDescription(), "Print skid label?", "Yes", "No"))
                    {
                        SysSkidInfo.PrintLabel(this.SkidInfo);
                    }
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                await errRpt.ShowDialog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveSkid()
        {
            if (this.IsChangesMade)
            {
                SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;

                // Important to not assign a current exiting SkidNo to a new skid.
                if (!this.IsNewSkid)
                {
                    tempSkidInfo.SkidCode = this.txtSkidCode.Text;
                }

                if (string.IsNullOrEmpty(this.txtHi.Text))
                {
                    tempSkidInfo.Hi = 0;
                }
                else
                {
                    tempSkidInfo.Hi = int.Parse(this.txtHi.Text);
                }

                if (string.IsNullOrEmpty(this.txtTi.Text))
                {
                    tempSkidInfo.Ti = 0;
                }
                else
                {
                    tempSkidInfo.Ti = int.Parse(this.txtTi.Text);
                }

                if (string.IsNullOrEmpty(this.txtCube.Text))
                {
                    tempSkidInfo.Cube = 0;
                }
                else
                {
                    tempSkidInfo.Cube = decimal.Parse(this.txtCube.Text);
                }

                if (string.IsNullOrEmpty(this.txtHeight.Text))
                {
                    tempSkidInfo.Height = 0;
                }
                else
                {
                    tempSkidInfo.Height = decimal.Parse(this.txtHeight.Text);
                }

                tempSkidInfo.IsPutAway = this.chkIsPutAway.IsToggled;
                tempSkidInfo.LocMap = this.LocMap.CurrLocMap;
                tempSkidInfo.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo.ProdInfo.Refno);

                if (this.IsNewSkid)
                {
                    tempSkidInfo.LocMapRefno = this.LocMap.CurrLocMapRefno;
                    tempSkidInfo.SkidCode = this.txtSkidCode.Text;
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.SkidInfo = SysSkidInfo.Create(SysSkidInfo.SkidInfoToXML(this.SkidInfo));
                }
                else
                {
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.SkidInfo = SysSkidInfo.SendUpdate(this.SkidInfo);
                }

                this.IsChangesMade = false;
                this.IsSkidCodeChanged = false;
            }
        }

        private void EnableControls()
        {
            if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo && this.SkidInfo.CurrSkid.Quantity == 0 && this.SkidInfo.CurrSkid.BaseQuantity == 0)
            {
                this.cmdSkidProduct.IsEnabled = true;
                this.updnBarCodeModifier1.IsEnabled = true;
            }

            if (this.IsNewSkid || (this.SkidInfo.CurrSkid.Quantity == 0 && this.SkidInfo.CurrSkid.BaseQuantity == 0))
            {
                this.cmdLocation.IsEnabled = true;
            }

            this.txtTi.IsEnabled = true;
            this.txtHi.IsEnabled = true;
            this.txtHeight.IsEnabled = true;
            this.txtCube.IsEnabled = true;
            this.cmdDetail.IsEnabled = true;
            this.cmdComplete.IsEnabled = true;
            this.pictureBoxPrintLabel.IsEnabled = true;
        }

        private void DisableControls()
        {
            if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo && this.SkidInfo.CurrSkid.Quantity == 0 && this.SkidInfo.CurrSkid.BaseQuantity == 0)
            {
            }
            else
            {
                this.cmdSkidProduct.IsEnabled = false;
                this.cmdLocation.IsEnabled = false;
                this.txtTi.IsEnabled = false;
                this.txtHi.IsEnabled = false;
                this.txtHeight.IsEnabled = false;
                this.txtCube.IsEnabled = false;
            }

            this.updnBarCodeModifier1.IsEnabled = false;
            this.cmdDetail.IsEnabled = false;
            this.cmdComplete.IsEnabled = false;
            this.pictureBoxPrintLabel.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayData()
        {
            this.txtSkidCode.Text = string.Empty;
            this.txtTi.Text = string.Empty;
            this.txtHi.Text = string.Empty;
            this.txtHeight.Text = string.Empty;
            this.txtCube.Text = string.Empty;
            this.chkIsPutAway.IsToggled = false;
            this.lblCubeUOM.Text = string.Empty;
            this.lblHeightUOM.Text = string.Empty;

            // TODO: Resource strings
            this.cmdLocation.Text = "Set Location";

            if (0 < this.SkidInfo.CurrSkid.LocMapRefno)
            {
                this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(this.SkidInfo.CurrSkid.LocMap);
            }

            // TODO: Resource strings
            this.cmdSkidProduct.Text = "Select Product";

            if (this.SkidInfo.CurrSkid.ProdInfo != null)
            {
                if (0 < this.SkidInfo.CurrSkid.ProdInfo.UnitRefno)
                {
                    this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(this.SkidInfo.CurrSkid.ProdInfo);
                }
            }

            this.cmdDetail.IsEnabled = false;
            this.cmdComplete.IsEnabled = false;

            this.ItemsList.Clear();

            if (this.SkidInfo != null)
            {
                if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
                {
                    this.txtSkidCode.Text = this.SkidInfo.CurrSkid.SkidCode;
                    this.txtTi.Text = this.SkidInfo.CurrSkid.Ti.ToString();
                    this.txtHi.Text = this.SkidInfo.CurrSkid.Hi.ToString();
                    this.txtHeight.Text = this.SkidInfo.CurrSkid.Height.ToString();
                    this.txtCube.Text = this.SkidInfo.CurrSkid.Cube.ToString();
                    this.chkIsPutAway.IsToggled = this.SkidInfo.CurrSkid.IsPutAway;
                    this.lblCubeUOM.Text = this.SkidInfo.CurrSkid.CubeUnit;
                    this.lblHeightUOM.Text = this.SkidInfo.CurrSkid.HeightUnit;
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(this.SkidInfo.CurrSkid.LocMap);
                    this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(this.SkidInfo.CurrSkid.ProdInfo);
                    this.cmdDetail.IsEnabled = true;

                    if (this.SkidInfo.CurrSkid.IsCompleted)
                    {
                        this.cmdComplete.IsEnabled = false;
                    }
                    else
                    {
                        this.cmdComplete.IsEnabled = true;
                    }

                    // TODO: Resource strings
                    this.ItemsList.Add(string.Format("Quantity {0} {1} @ {2} {3}", this.SkidInfo.CurrSkid.Quantity, this.SkidInfo.CurrSkid.ProdInfo.UnitCode, this.SkidInfo.CurrSkid.WeightQuantity, this.SkidInfo.CurrSkid.ProdInfo.WeightUnitCode));
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityHdrSkidInfo_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                this.ScannerEnable();

                this.txtSkidCode.Focus();

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                    tempSkidInfo.LocMap = this.LocMap.CurrLocMap;
                    this.SkidInfo.CurrSkid = tempSkidInfo;
                    this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdLocation.Text = "Set Location";
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDetail_Click(object sender, EventArgs e)
        {
            if (this.SkidInfo.CurrSkid.SkidFKRefno != 0)
            {
                this.ScannerDisable();
                Controller.Activity activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.SkidInfo);
                activity.FKRefno = this.SkidInfo.CurrSkid.SkidFKRefno;
                activity.Refno = this.SkidInfo.CurrSkid.SkidFKRefno;
                activity.SkidInfo = this.SkidInfo;
                ActItem actItem = ActItemFacade.GetActItem(activity, SysCommon.ActSubTypes.SkidInfo);
                actItem.FKRefno = this.SkidInfo.CurrSkid.SkidFKRefno;
                actItem.Refno = this.SkidInfo.CurrSkid.SkidFKRefno;
                actItem.LocMap = this.LocMap;
                actItem.ProdInfo = this.SkidInfo.CurrSkid.ProdInfo;
                actItem.ProdInfo.Unit2BaseUnitFactor = ProductInfoTasks.GetUnitRelationFactor(actItem.ProdInfo);

                FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetailSkid, actItem, SysCommon.ActSubTypes.SkidInfo);
                await form.DisplayControl.ShowDialog();
                await form.SaveChanges(sender, e);
                XDocument xmlDoc = form.DisplayControlData;

                if (this.SkidInfo.CurrSkid.IsConsumed)
                {
                    this.SkidInfo = new SysSkidInfo();
                    this.ProdUnitRefno = 0;
                    this.LocMap.CurrLocMap = this.SavedLocMap;
                    this.IsNewSkid = true;
                }

                this.ScannerEnable();
                this.DisplayData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdComplete_Click(object sender, EventArgs e)
        {
            try
            {
                this.Loading = true;

                await Task.Run(() =>
                {
                    this.SkidInfo = SysSkidInfo.Complete(this.SkidInfo);
                });

                this.Loading = false;
                //MessageBox.Sho;w("Skid completed.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                // TODO: Resource strings
                await DisplayAlert(this.GetControlDescription(), "Skid completed.", "OK");
            }
            catch (Exception ex)
            {
                FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                await errRpt.ShowDialog();
            }
            finally
            {
                this.Loading = false;
                this.DisplayData();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdSkidProduct_Click(object sender, EventArgs e)
        {
            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, SysCommon.ActSubTypes.SkidInfo);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                await form.SaveChanges(sender, e);
                XDocument xmlDoc = form.DisplayControlData;
                this.ProdUnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);
                SSkidInfo tempSkidInfo = this.SkidInfo.CurrSkid;
                tempSkidInfo.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo.ProdInfo.Refno);
                this.SkidInfo.CurrSkid = tempSkidInfo;
            }

            // TODO: Resource strings
            this.cmdSkidProduct.Text = "Select Product";

            if (this.SkidInfo.CurrSkid.ProdInfo != null)
            {
                if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != 0)
                {
                    this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(this.SkidInfo.CurrSkid.ProdInfo);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSkidCode_GotFocus(object sender, EventArgs e)
        {
            //this.txtSkidCode.SelectAll();

            if (this.txtSkidCode.Text != null)
            {
                this.txtSkidCode.CursorPosition = 0;
                this.txtSkidCode.SelectionLength = this.txtSkidCode.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtTi_GotFocus(object sender, EventArgs e)
        {
            //this.txtTi.SelectAll();

            if (this.txtTi.Text != null)
            {
                this.txtTi.CursorPosition = 0;
                this.txtTi.SelectionLength = this.txtTi.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtHi_GotFocus(object sender, EventArgs e)
        {
            //this.txtHi.SelectAll();

            if (this.txtHi.Text != null)
            {
                this.txtHi.CursorPosition = 0;
                this.txtHi.SelectionLength = this.txtHi.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtHeight_GotFocus(object sender, EventArgs e)
        {
            //this.txtHeight.SelectAll();

            if (this.txtHeight.Text != null)
            {
                this.txtHeight.CursorPosition = 0;
                this.txtHeight.SelectionLength = this.txtHeight.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtCube_GotFocus(object sender, EventArgs e)
        {
            //this.txtCube.SelectAll();

            if (this.txtCube.Text != null)
            {
                this.txtCube.CursorPosition = 0;
                this.txtCube.SelectionLength = this.txtCube.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    if (sender == this.txtSkidCode)
                    {
                        if (!this.IsSkidCodeChanged)
                        {
                            this.DisableControls();
                        }

                        this.IsSkidCodeChanged = true;
                    }

                    if (sender == this.txtCube)
                    {
                    }

                    if (sender == this.txtHeight)
                    {
                    }

                    if (sender == this.txtHi)
                    {
                    }

                    if (sender == this.txtTi)
                    {
                    }

                    if (sender == this.chkIsPutAway)
                    {
                    }

                    if (sender == this.cmdComplete)
                    {
                    }

                    if (sender == this.cmdDetail)
                    {
                    }

                    if (sender == this.cmdLocation)
                    {
                    }

                    if (sender == this.cmdSkidProduct)
                    {
                    }

                    if (sender == this.updnBarCodeModifier1)
                    {
                        this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
                    }

                    if (sender == this.cmdNewSkid)
                    {
                    }

                    this.ValidateEntry(sender, new CancelEventArgs());
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                // Do not set the IsChangesMade. Exceptions.MessageOnlyException is really not and error. 
                // Allow the validation to handle the setting for IsChangesMade.
                ////this.IsChangesMade = false;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                this.IsChangesMade = true;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void updnBarCodeModifier1_SelectedItemChanged(object sender, EventArgs e)
        {
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void updnBarCodeModifier1_GotFocus(object sender, EventArgs e)
        //{
        //    SysCommon.SelectValueInDomainUpDown((UpDownBase)sender);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            try
            {
                if (sender == this.txtSkidCode)
                {
                    // Keydown event fires before
                    if (this.SkidInfo.CurrSkid.SkidFKRefno == 0 || string.IsNullOrEmpty(this.txtSkidCode.Text) || this.SavedSkidInfo.CurrSkid.SkidCode != this.txtSkidCode.Text.TrimEnd())
                    {
                        this.IsNewSkid = true;
                    }
                    else
                    {
                        this.IsNewSkid = false;
                    }
                }

                if (sender == this.txtCube)
                {
                    if (true == (decimal.Parse(this.txtCube.Text) < 0))
                    {
                        e.Cancel = true;

                        // TODO: Resource strings 
                        DisplayAlert(this.GetControlDescription(), "Cube value must be greater than or equal to zero.", "OK");
                    }
                }

                if (sender == this.txtHeight)
                {
                }

                if (sender == this.txtHi)
                {
                }

                if (sender == this.txtTi)
                {
                }

                if (sender == this.chkIsPutAway)
                {
                }

                if (sender == this.cmdComplete)
                {
                }

                if (sender == this.cmdDetail)
                {
                }

                if (sender == this.cmdLocation)
                {
                }

                if (sender == this.cmdSkidProduct)
                {
                }

                if (sender == this.updnBarCodeModifier1)
                {
                }

                if (sender == this.cmdNewSkid)
                {
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                e.Cancel = false;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void DecimalKeyDownHandler(object sender, KeyEventArgs e)
        //{
        //    this.IsNumericTrapped = false;
        //    this.IsEditTrapped = false;

        //    const int INTERMEC_CK60_KEYCODE_DECIMAL = 190;
        //    const int LXE_MX7II_KEYCODE_SUBTRACT = 189;

        //    if (e.KeyCode == Keys.Insert)
        //    {
        //        this.IsEditTrapped = true;
        //    }
        //    else if (e.KeyCode == Keys.Back)
        //    {
        //        this.IsEditTrapped = true;
        //    }
        //    else if (e.KeyCode == Keys.Subtract || (int)e.KeyCode == LXE_MX7II_KEYCODE_SUBTRACT)
        //    {
        //        // Do nothing
        //    }
        //    else if (e.KeyCode == Keys.Decimal)
        //    {
        //        this.IsNumericTrapped = true;
        //    }
        //    else if ((int)e.KeyCode == INTERMEC_CK60_KEYCODE_DECIMAL)
        //    {
        //        this.IsNumericTrapped = true;
        //    }
        //    else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
        //    {
        //        this.IsNumericTrapped = true;
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void DecimalKeyPressHandler(object sender, KeyPressEventArgs e)
        //{
        //    if (this.IsEditTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsEditTrapped = false;
        //    }
        //    else if (this.IsNumericTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsNumericTrapped = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void IntegerKeyDownHandler(object sender, KeyEventArgs e)
        //{
        //    this.IsNumericTrapped = false;
        //    this.IsEditTrapped = false;

        //    const int INTERMEC_CK60_KEYCODE_DECIMAL = 190;
        //    const int LXE_MX7II_KEYCODE_SUBTRACT = 189;

        //    if (e.KeyCode == Keys.Insert)
        //    {
        //        this.IsEditTrapped = true;
        //    }
        //    else if (e.KeyCode == Keys.Back)
        //    {
        //        this.IsEditTrapped = true;
        //    }
        //    else if (e.KeyCode == Keys.Subtract || (int)e.KeyCode == LXE_MX7II_KEYCODE_SUBTRACT)
        //    {
        //        // Do nothing
        //    }
        //    else if (e.KeyCode == Keys.Decimal)
        //    {
        //        this.IsNumericTrapped = true;
        //    }
        //    else if ((int)e.KeyCode == INTERMEC_CK60_KEYCODE_DECIMAL)
        //    {
        //        // Do nothing
        //    }
        //    else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
        //    {
        //        this.IsNumericTrapped = true;
        //    }
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ///// <remarks></remarks>
        //private void IntegerKeyPressHandler(object sender, KeyPressEventArgs e)
        //{
        //    if (this.IsEditTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsEditTrapped = false;
        //    }
        //    else if (this.IsNumericTrapped)
        //    {
        //        e.Handled = false;
        //        this.IsNumericTrapped = false;
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}

        /// <summary>
        /// Setup skid, fill skid info if possible.
        /// </summary>
        private void SetupSkid()
        {
            if (string.IsNullOrEmpty(this.txtSkidCode.Text))
            {
                SetupNewSkid();
            }
            else
            {
                SetupNewSkidFromSkidCode();
            }
        }

        /// <summary>
        /// Setup a new skid from skid code or assigned from 
        /// </summary>
        private void SetupNewSkidFromSkidCode()
        {
            // SkidInfo may be a new skid, skid numbers can be created from the comm server.
            this.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(this.txtSkidCode.Text.TrimEnd());

            if (this.txtSkidCode.Text.TrimEnd() != this.SkidInfo.CurrSkid.SkidCode.TrimEnd())
            {
                this.IsNewSkid = true;
            }

            // Location may have been defaulted or already set.
            this.LocMap.CurrLocMap = this.SkidInfo.CurrSkid.LocMap;
            this.ProdUnitRefno = this.SkidInfo.CurrSkid.ProdInfo.UnitRefno;

            if (this.ProdUnitRefno == 0)
            {
                this.IsNewSkid = true;
            }
        }

        /// <summary>
        /// Setup a new skid
        /// </summary>
        private void SetupNewSkid()
        {
            this.IsNewSkid = true;
            this.SkidInfo = new SysSkidInfo();
            this.ProdUnitRefno = 0;
            this.LocMap.CurrLocMap = this.SavedLocMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSkidCode_KeyUpEnter(object sender, KeyEventArgs e)
        {
            SysSkidInfo tempSkidInfo = SysSkidInfo.DeepCopy(this.SkidInfo);

            this.IsNewSkid = false;

            try
            {
                this.SetupSkid();
                this.EnableControls();
                this.DisplayData();
            }
            catch (Exception ex)
            {
                this.IsNewSkid = true;
                this.LocMap.CurrLocMap = this.SavedLocMap;
                this.SkidInfo = tempSkidInfo;

                //MessageBox.Show(ex.Message + Environment.NewLine + "A new Skid will be created upon saving.", this.GetControlDescription(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                // TODO: Resource strings
                DisplayAlert(this.GetControlDescription(), ex.Message + Environment.NewLine + "A new Skid will be created upon saving.", "OK");

                this.EnableControls();
                this.SkidInfo = new SysSkidInfo();
                SSkidInfo tempSkidInfo2 = this.SkidInfo.CurrSkid;
                tempSkidInfo2.SkidCode = this.txtSkidCode.Text;
                tempSkidInfo2.LocMapRefno = this.SavedLocMapRefno;
                tempSkidInfo2.LocMap = this.SavedLocMap;
                tempSkidInfo2.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo2.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo2.ProdInfo.Refno);
                this.SkidInfo.CurrSkid = tempSkidInfo2;
            }

            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                SSkidInfo tempSkidInfo2 = this.SkidInfo.CurrSkid;
                tempSkidInfo2.LocMap = this.LocMap.CurrLocMap;
                this.SkidInfo.CurrSkid = tempSkidInfo2;
                this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
            }
            else
            {
                // TODO: Resource strings
                this.cmdLocation.Text = "Set Location";
            }

            // TODO: Resource strings
            this.cmdSkidProduct.Text = "Select Product";

            if (this.SkidInfo.CurrSkid.ProdInfo != null)
            {
                if (this.SkidInfo.CurrSkid.ProdInfo.UnitRefno != 0)
                {
                    this.cmdSkidProduct.Text = ProductInfoTasks.GetCurrReadableProdBCUnit(this.SkidInfo.CurrSkid.ProdInfo);
                }
            }

            this.IsSkidCodeChanged = false;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void txtSkidCode_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
        //    {
        //        this.txtSkidCode_KeyUpEnter(sender, e);
        //    }

        //    e.Handled = true;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdNewSkid_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsNewSkid = true;

                // Clearing the skid code text box is needed to handle a new skid in this.SaveChanges().
                this.txtSkidCode.Text = string.Empty;
                IsSkidCodeChanged = false;
                this.DisableControls();

                SSkidInfo tempSkidInfo = new SSkidInfo();
                tempSkidInfo.LocMap = this.LocMap.CurrLocMap;
                tempSkidInfo.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ProdUnitRefno);
                tempSkidInfo.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(tempSkidInfo.ProdInfo.Refno);

                this.SkidInfo.CurrSkid = tempSkidInfo;

                // this.IsChangesMade = true, Needed for SaveChanges to make the create new skid call.
                this.IsChangesMade = true;

                if (await this.IsReadyToSave())
                {
                    this.SaveSkid();
                }
                else
                {
                    // If save fails just display the new skid detail.
                }

                this.DisplayData();
                this.EnableControls();

                ////if (this.CallingActSubType == SysCommon.ActSubTypes.SkidInfo)
                ////{
                ////  if (this.IsReadyToSave())
                ////  {
                ////    this.SaveSkid();
                ////    this.DisplayData();
                ////  }
                ////}
                ////else
                ////{
                ////  if (this.SaveChanges())
                ////  {
                ////    if (this.ReqCloseEvent != null)
                ////    {
                ////      this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
                ////    }
                ////  }
                ////  else
                ////  {
                ////    // If save fails just display the new skid detail.
                ////    this.DisplayData();
                ////  }
                ////}
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(SysCommon.ActSubTypes.SkidInfo);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(SysCommon.ActSubTypes.SkidInfo, ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        private void txtSkidCode_Completed(object sender, EventArgs e)
        {
            this.txtSkidCode_KeyUpEnter(sender, new KeyEventArgs(false, Keycode.Enter, new KeyEvent(KeyEventActions.Up, Keycode.Enter)));
        }

        private void OptionsMenu_Clicked(object sender, EventArgs e)
        {
            // TODO: Add actions to displayactionsheet
        }

        private void updnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdentifyChangesMade(sender, new TextChangedEventArgs(string.Empty, string.Empty));
        }

        private void chkIsPutAway_Toggled(object sender, ToggledEventArgs e)
        {
            this.IdentifyChangesMade(sender, new TextChangedEventArgs(string.Empty, string.Empty));
        }
    }
}