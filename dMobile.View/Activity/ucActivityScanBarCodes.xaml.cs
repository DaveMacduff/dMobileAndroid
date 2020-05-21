using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using dMobile.GroundWork;
using dMobile.View.CustomPages;
using dMobile.View.Forms;
using dMobile.Resources.Enums;
using System.Data.SQLite;
using Color = Xamarin.Forms.Color;
using Android.Views;
using System.Collections.ObjectModel;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityScanBarCodes : DMobileContentPage, IAppArea, IAppPublisher
    {
        #region Fields

        //private UserControl callingControl = new ucUtilitiesMenu();
        private BarCodeParse parse = null;
        private Controller.Activity actBarCode = null;
        private ActItem actItemBarCode = null;
        private int unitRefno = 0;
        private int unitBarCodeRefno = 0;
        private bool isChangesMade = false;
        private bool isControlLoaded = false;
        private int totalTicks = 0;
        private bool isRegistered = false;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.BarCodeAssociation;
        private ObservableCollection<string> itemsList;
        private bool hasData = false;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public ucActivityScanBarCodes()
        {
            this.InitializeComponent();

            this.Title = this.GetControlDescription();

            this.cmdAssignProduct.Text = "Assign Scan To Product";
            this.lblProduct.Text = "Product:";
            this.lblProductData.Text = "Unidentified";

            if (!DesignMode.IsDesignModeEnabled)
            {
                this.SysEnv = SysEnvironment.GetInstance();
                this.Scanner = BarCodeSingleton.GetInstance().Scanner;

                this.itemsList = new ObservableCollection<string>();

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
            }
        }

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstScannedData.ItemsSource = this.ItemsList;

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else if (keycode == Keycode.Escape)
                {
                    if (await this.CancelChanges())
                    {
                        this.ReqCloseEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    }
                }
            });

            try
            {
                await this.ucActivityScanBarCodes_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine("this.ucActivityScanBarCodes_GotFocus threw error " + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.ScannerDisable();

            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveDisableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqCancelEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);
        public delegate void ReqSaveEnableHandler(IAppPublisher appPub, AppEventArgs appEventArgs);

        #endregion

        #region Events

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

        #endregion

        #region Properties

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubType;
            }

            set
            {
                this.actSubType = value;
            }
        }

        private BarCodeParse Parse
        {
            get
            {
                return this.parse;
            }

            set
            {
                this.parse = value;
            }
        }

        private ActItem ActItemBarCode
        {
            get
            {
                return this.actItemBarCode;
            }

            set
            {
                this.actItemBarCode = value;
            }
        }

        private Controller.Activity ActBarCode
        {
            get
            {
                return this.actBarCode;
            }

            set
            {
                this.actBarCode = value;
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

        public ObservableCollection<string> ItemsList
        {
            get
            {
                return this.itemsList;
            }

            set
            {
                this.itemsList = value;
            }
        }

        private bool IsRegistered
        {
            get
            {
                return this.isRegistered;
            }

            set
            {
                this.isRegistered = value;
            }
        }

        private int TotalTicks
        {
            get
            {
                return this.totalTicks;
            }

            set
            {
                this.totalTicks = value;
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

        private int UnitRefno
        {
            get
            {
                return this.unitRefno;
            }

            set
            {
                this.unitRefno = value;
            }
        }

        private int UnitBarCodeRefno
        {
            get
            {
                return this.unitBarCodeRefno;
            }

            set
            {
                this.unitBarCodeRefno = value;
            }
        }

        public bool HasData { get => hasData; set => hasData = value; }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Stop();
                this.IsRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerEnable()
        {
            if (!this.IsRegistered)
            {
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                this.Scanner.Start();
                this.IsRegistered = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = false;

            try
            {
                isReadyToSave = true;
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                this.ScannerDisable();
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
                });
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        //public UserControl GetCallingControl()
        //{
        //    return new ucUtilitiesMenu();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Bar Code Association";
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
                if (await this.IsReadyToSave())
                {
                    // ClearContents();
                    saveComplete = true;
                    this.isChangesMade = false;
                }
                else
                {
                    // IsReadyToSave will raise errors for invalid data. This is a catch all 
                    // in the event that IsReadyToSave just returns false.

                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
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
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (await this.IsReadyToClose())
                {
                    // Expect the control to close.
                }
                else
                {
                    // If changes where made.
                    // By reloading, restoring the last valid state we should be able to close the control
                    // on the second CancelChanges attempt.
                    // Methods that could be called to restore the control to a closable state.
                    ////ClearProperties();
                    ////DisplayData();
                    ////this.LoadData();
                    cancelChanges = false;
                    this.IsChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                cancelChanges = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                    await errRpt.ShowDialog();
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
            XDocument xmlDoc = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmlDoc;
        }

        #endregion

        #region IAppPublisher Members

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

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="msg"></param>
        private void AddScanMsg(object sender, EventArgs e, string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.ItemsList.Add(msg);
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="strMessage"></param>
        /// <remarks></remarks>
        private void StatusOut(object sender, EventArgs e, string msg)
        {
            this.AddScanMsg(sender, e, msg);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="barCodeScanArgs"></param>
        /// <remarks></remarks>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            await Task.Run(() =>
            {
                this.Loading = true;

                bool isSerialTracked = false;
                bool isProductionDateTracked = false;
                bool isLotTracked = false;
                bool isBestBeforeDateTracked = false;
                bool isVariantTracked = false;
                bool isLotUniqueComponent = false;
                bool isProductionDateUniqueComponent = false;
                bool isBestBeforeDateUniqueComponent = false;
                bool isVariantUniqueComponent = false;
                int prodSchemeRefno = 0;
                string prodCode = string.Empty;
                ProductInfo prodInfo = new ProductInfo();
                SLocMap currLocMap = LocationMap.GetInstance().CurrLocMap;

                try
                {
                    this.ScannerDisable();

                    this.Parse = scanArgs.ParseInfo;

                    if (this.Parse != null)
                    {
                        this.HasData = true;
                    }

                    //this.lstScannedData.Items.Clear();
                    Device.BeginInvokeOnMainThread(() => this.ItemsList.Clear());

                    // Lookup product
                    this.UnitRefno = 0;
                    this.UnitBarCodeRefno = 0;

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                    {
                        string prodIdentifier = scanArgs.ParseInfo.ProductIdentifier.PadRight(20);

                        using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndProdBCUnit.CommandType = CommandType.Text;
                            cmndProdBCUnit.CommandText = "SELECT tProductUnit.ProductUnitRefno,"
                                + "tProductUnitBarCode.ProductUnitBarCodeRefno,"
                                + "tLocationMapWarehouseProductScheme.ProductSchemeRefno,"
                                + "tProduct.ProductSchemeRefno,"
                                + "tProduct.ProductCode,"
                                + "tProduct.ProductDescription,"
                                + "tProductUnit.ProductUnitBC,"
                                + "tUnit.UnitCode "
                                + "FROM tProductUnitBarCode "
                                + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tProductUnitBarCode.ProductUnitRefno "
                                + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                + "INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno "
                                + "WHERE tProductUnitBarCode.ProductUnitBarCodeCode = @prodIdentifier "
                                + "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = @whsLocMapRefno "
                                + "AND tProductUnitBarCode.ProductUnitBarCodeModifier1 = @productUnitBarCodeModifier1";
                            cmndProdBCUnit.Parameters.Add(new SQLiteParameter("@prodIdentifier", prodIdentifier));
                            cmndProdBCUnit.Parameters.Add(new SQLiteParameter("@whsLocMapRefno", currLocMap.WhsLocMapRefno));
                            cmndProdBCUnit.Parameters.Add(new SQLiteParameter("@productUnitBarCodeModifier1", this.SysEnv.CurrentBarCodeModifier1));

                            using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                            {
                                if (drProdBCUnit.Read())
                                {
                                    this.UnitRefno = Convert.ToInt32(drProdBCUnit["ProductUnitRefno"]);
                                    this.UnitBarCodeRefno = Convert.ToInt32(drProdBCUnit["ProductUnitBarCodeRefno"]);
                                    prodSchemeRefno = Convert.ToInt32(drProdBCUnit["ProductSchemeRefno"]);
                                    prodCode = Convert.ToString(drProdBCUnit["ProductCode"]);

                                    // If product was found associated display it.  'ProductID+BC+UOM'

                                    // TODO: Resource strings
                                    this.AddScanMsg(sender, scanArgs, "Product associated to bar code");

                                    string prodBCUnit = drProdBCUnit["ProductCode"].ToString() + " " + drProdBCUnit["ProductUnitBC"].ToString() + " " + drProdBCUnit["UnitCode"].ToString();

                                    this.AddScanMsg(sender, scanArgs, prodBCUnit);

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        this.lblProductData.TextColor = Color.Black;
                                        this.lblProductData.Text = prodBCUnit;
                                    });

                                    this.AddScanMsg(sender, scanArgs, drProdBCUnit["ProductDescription"].ToString());
                                }
                            }

                            if (this.UnitRefno == 0)
                            {
                                // Do not look up gtin in barcode database for this rule, product will be identified by a range
                                // of fixed weights.
                                if (sysEnv.BarCodeSettings.IsRuleEnabled_ParseSumwaFarmFedWholeDuck_UseWeightRangeToIDProduct && this.Parse.ProductIdentifier == "90695955110022")
                                {
                                    SysShared.BarCodeRuleParseSumwaFarmFedWholeDuckUseWeightRangeToIDProduct(this.Parse);

                                    prodInfo = ProductInfoTasks.GetInfoFromCodeBCUnit(this.Parse.ProductIdentifier, " ", "CS");
                                    this.UnitRefno = prodInfo.UnitRefno;
                                }
                            }

                            if (this.UnitRefno == 0)
                            {
                                try
                                {
                                    decimal deliScaleWeight = 0;
                                    string deliScaleWeightUnit = string.Empty;
                                    this.UnitRefno = ProductInfoTasks.GetUnitRefnoFromBarCode(
                                        prodIdentifier,
                                        this.SysEnv.CurrentBarCodeModifier1,
                                        this.Parse.Symbology,
                                        this.Parse.DeliScaleAmount,
                                        ref deliScaleWeight,
                                        ref deliScaleWeightUnit,
                                        ref prodInfo);

                                    if (this.UnitRefno == 0)
                                    {
                                        // TODO: Resource strings
                                        this.AddScanMsg(sender, scanArgs, "Not associated to a product");

                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            this.lblProductData.TextColor = Color.Red;

                                            // TODO: Resource strings
                                            this.lblProductData.Text = "Unidentified";
                                        });
                                    }
                                    else
                                    {
                                        cmndProdBCUnit.CommandType = CommandType.Text;
                                        cmndProdBCUnit.CommandText = "SELECT tProductUnit.ProductUnitRefno,"
                                        + "tLocationMapWarehouseProductScheme.ProductSchemeRefno,"
                                        + "tProduct.ProductCode,"
                                        + "tProductUnit.ProductUnitBC,"
                                        + "tUnit.UnitCode "
                                        + "FROM tProductUnit "
                                        + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                        + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                        + "INNER JOIN tLocationMapWarehouseProductScheme ON tLocationMapWarehouseProductScheme.ProductRefno = tProduct.ProductRefno "
                                        + "WHERE tProductUnit.ProductUnitRefno = @productUnitRefno "
                                        + "AND tLocationMapWarehouseProductScheme.WarehouseLocationMapRefno = @whsLocMapRefno";
                                        cmndProdBCUnit.Parameters.Add(new SQLiteParameter("@productUnitRefno", this.UnitRefno));
                                        cmndProdBCUnit.Parameters.Add(new SQLiteParameter("@whsLocMapRefno", currLocMap.WhsLocMapRefno));

                                        using (SQLiteDataReader drProductBCUnit = cmndProdBCUnit.ExecuteReader())
                                        {
                                            if (drProductBCUnit.Read())
                                            {
                                                this.UnitRefno = Convert.ToInt32(drProductBCUnit["ProductUnitRefno"]);
                                                this.UnitBarCodeRefno = 0;
                                                prodSchemeRefno = Convert.ToInt32(drProductBCUnit["ProductSchemeRefno"]);
                                                prodCode = drProductBCUnit["ProductCode"].ToString();

                                                // If product was found associated display it.  'ProductID+BC+UOM'

                                                // TODO: Resource strings
                                                this.AddScanMsg(sender, scanArgs, "Product Associated To Bar Code");

                                                string prodbcuom = drProductBCUnit["ProductCode"].ToString() + " "
                                                    + drProductBCUnit["ProductUnitBC"].ToString() + " " + drProductBCUnit["UnitCode"].ToString();
                                                this.AddScanMsg(sender, scanArgs, prodbcuom);

                                                if (this.Parse.DeliScaleAmount != 0 && (this.Parse.Symbology == SysCommon.BarCodeSymbologies.UCC_12_UPC_E || this.Parse.Symbology == SysCommon.BarCodeSymbologies.EAN_UCC_13))
                                                {
                                                    this.AddScanMsg(sender, scanArgs, string.Format("Product Deli Scale Amount {0}", this.Parse.DeliScaleAmount));
                                                    this.AddScanMsg(sender, scanArgs, string.Format("Product Deli Scale Weight {0} {1}", deliScaleWeight, deliScaleWeightUnit));

                                                    if (this.SysEnv.BarCodeSettings.IsRuleEnabled_Parse_UCC_12_UPC_E_as_Deli_PLU_4_Digit_Amount)
                                                    {
                                                        this.AddScanMsg(sender, scanArgs, "Barcode Rule: 4 digit amount");
                                                    }
                                                }

                                                Device.BeginInvokeOnMainThread(() =>
                                                {
                                                    this.lblProductData.TextColor = Color.Black;
                                                    this.lblProductData.Text = prodbcuom;
                                                });
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        // TODO: Resource strings
                                        this.AddScanMsg(sender, scanArgs, "Product unidentified");

                                        this.lblProductData.TextColor = Color.Red;

                                        // TODO: Resource strings
                                        this.lblProductData.Text = "Unidentified";
                                    });
                                }
                            }

                            // get product tracking scheme
                            if (prodSchemeRefno != 0)
                            {
                                using (SQLiteCommand scheme = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                {
                                    scheme.CommandType = CommandType.Text;
                                    scheme.CommandText = "SELECT tProductScheme.ProductSchemeIsSerialTracked,"
                                        + "tProductScheme.ProductSchemeIsProductionDateTracked,"
                                        + "tProductScheme.ProductSchemeIsLotTracked,"
                                        + "tProductScheme.ProductSchemeIsBestBeforeDateTracked,"
                                        + "tProductScheme.ProductSchemeIsVariantTracked,"
                                        + "tProductScheme.ProductSchemeIsLotUniqueComponent,"
                                        + "tProductScheme.ProductSchemeIsProductionDateUniqueComponent,"
                                        + "tProductScheme.ProductSchemeIsBestBeforeDateUniqueComponent,"
                                        + "tProductScheme.ProductSchemeIsVariantUniqueComponent "
                                        + "FROM tProductScheme "
                                        + "WHERE tProductScheme.ProductSchemeRefno = " + prodSchemeRefno;

                                    using (SQLiteDataReader drScheme = scheme.ExecuteReader())
                                    {
                                        if (drScheme.Read())
                                        {
                                            isSerialTracked = Convert.ToBoolean(drScheme["ProductSchemeIsSerialTracked"]);
                                            isProductionDateTracked = Convert.ToBoolean(drScheme["ProductSchemeIsProductionDateTracked"]);
                                            isLotTracked = Convert.ToBoolean(drScheme["ProductSchemeIsLotTracked"]);
                                            isBestBeforeDateTracked = Convert.ToBoolean(drScheme["ProductSchemeIsBestBeforeDateTracked"]);
                                            isVariantTracked = Convert.ToBoolean(drScheme["ProductSchemeIsVariantTracked"]);
                                            isLotUniqueComponent = Convert.ToBoolean(drScheme["ProductSchemeIsLotUniqueComponent"]);
                                            isProductionDateUniqueComponent = Convert.ToBoolean(drScheme["ProductSchemeIsProductionDateUniqueComponent"]);
                                            isBestBeforeDateUniqueComponent = Convert.ToBoolean(drScheme["ProductSchemeIsBestBeforeDateUniqueComponent"]);
                                            isVariantUniqueComponent = Convert.ToBoolean(drScheme["ProductSchemeIsVariantUniqueComponent"]);
                                        }
                                        else
                                        {
                                            // TODO: Resource strings
                                            throw new Exception(string.Format("Unable to locate the product scheme for product \'{0}\'", prodCode));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // do not allow product bar code reassignment
                    if (this.UnitRefno == 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.cmdAssignProduct.IsEnabled = true;
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            this.cmdAssignProduct.IsEnabled = false;
                        });
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.SSCC18))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, (string)("SSCC-18: " + scanArgs.ParseInfo.SSCC18));

                        if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            try
                            {
                                SysSkidInfo skid = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(scanArgs.ParseInfo.SSCC18);

                                if (skid.CurrSkid.SkidFKRefno > 0)
                                {
                                    //// AddScanMsg(sender, BarCodeScanArgs, "Skid identified")

                                    // TODO: Resource strings
                                    this.AddScanMsg(sender, scanArgs, string.Format("Code: {0}", skid.CurrSkid.LogisticUnitSkidCode));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Refno: {0}", skid.CurrSkid.SkidFKRefno));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Number: {0}", skid.CurrSkid.SkidCode));

                                    string locReadable = LocationMapTasks.GetCurrentReadableLocation(skid.CurrSkid.LocMap);

                                    // TODO: Resource strings
                                    string skidQty = string.Format("Qty: {0} {1} @ {2} {3}", skid.CurrSkid.Quantity, skid.CurrSkid.ProdInfo.UnitCode, skid.CurrSkid.WeightQuantity, skid.CurrSkid.ProdInfo.WeightUnitCode);

                                    this.AddScanMsg(sender, scanArgs, string.Format("Location: {0}", locReadable));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Product: {0}", ProductInfoTasks.GetCurrReadableProdBCUnit(skid.CurrSkid.ProdInfo)));
                                    this.AddScanMsg(sender, scanArgs, skidQty);
                                    this.AddScanMsg(sender, scanArgs, string.Format("Put Away: {0}", skid.CurrSkid.IsPutAway));
                                    this.AddScanMsg(sender, scanArgs, string.Format("On Hold: {0}", skid.CurrSkid.IsHold));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Cube: {0} {1}", skid.CurrSkid.Cube, skid.CurrSkid.CubeUnit));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Height: {0} {1}", skid.CurrSkid.Height, skid.CurrSkid.HeightUnit));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Ti/Hi: {0}/{1}", skid.CurrSkid.Ti, skid.CurrSkid.Hi));
                                    this.AddScanMsg(sender, scanArgs, string.Format("CHEP: {0}", skid.CurrSkid.IsCHEP));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Completed: {0}", skid.CurrSkid.IsCompleted));
                                    this.AddScanMsg(sender, scanArgs, string.Format("Consumed: {0}", skid.CurrSkid.IsConsumed));
                                }
                                else
                                {
                                    // TODO: Resource strings
                                    throw new Exception("Not associated to a skid.");
                                }
                            }
                            catch (Exception)
                            {
                                // TODO: Resource strings
                                this.AddScanMsg(sender, scanArgs, "Skid unidentified.");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LocationIdentifier))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, (string)("Location identifier: " + scanArgs.ParseInfo.LocationIdentifier));

                        int locMapRefno = Convert.ToInt32(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(scanArgs.ParseInfo.LocationIdentifier));

                        if (locMapRefno > 0)
                        {
                            SLocMap locMap = LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, string.Format("Warehouse code: {0}", locMap.WhsCode));
                            this.AddScanMsg(sender, scanArgs, string.Format("Warehouse refno: {0}", locMap.WhsFKRefno));
                            this.AddScanMsg(sender, scanArgs, string.Format("Warehouse name: {0}", locMap.WhsName));
                            this.AddScanMsg(sender, scanArgs, string.Format("Section code: {0}", locMap.SectCode));
                            this.AddScanMsg(sender, scanArgs, string.Format("Section refno: {0}", locMap.SectFKRefno));
                            this.AddScanMsg(sender, scanArgs, string.Format("Section name: {0}", locMap.SectName));
                            this.AddScanMsg(sender, scanArgs, string.Format("Slot code: {0}", locMap.SlotCode));
                            this.AddScanMsg(sender, scanArgs, string.Format("Slot refno: {0}", locMap.SlotFKRefno));
                            this.AddScanMsg(sender, scanArgs, string.Format("Slot name: {0}", locMap.SlotName));
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Not associated to a location.");
                        }
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.GTIN))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "GTIN: " + scanArgs.ParseInfo.GTIN);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.GTINLogisticUnit))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "GTIN in a Logistic Unit: " + scanArgs.ParseInfo.GTINLogisticUnit);
                    }

                    if (scanArgs.ParseInfo.PackagingType != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Packaging Type: " + scanArgs.ParseInfo.PackagingType);
                    }

                    if (scanArgs.ParseInfo.NumberSystemCharacter != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Number System Character: " + scanArgs.ParseInfo.NumberSystemCharacter);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ProductIdentifier))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Product Identifier: " + scanArgs.ParseInfo.ProductIdentifier);
                    }

                    if (scanArgs.ParseInfo.ManufacturerSerialNumber != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Manufacturer Serial Number: " + scanArgs.ParseInfo.ManufacturerSerialNumber);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ItemReference))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Item Reference: " + scanArgs.ParseInfo.ItemReference);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.SingleTradeItemSerialNumber))
                    {
                        if (isSerialTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Serial #:" + scanArgs.ParseInfo.SingleTradeItemSerialNumber);
                        }
                    }
                    else
                    {
                        if (isSerialTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Serial # Missing");
                        }
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.LogisticUnitBatchLotNumber))
                    {
                        if (isLotTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Batch / Lot Number: " + scanArgs.ParseInfo.LogisticUnitBatchLotNumber);
                        }
                    }
                    else
                    {
                        if (isLotTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Batch / Lot Number Missing.");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitProductionDate))
                    {
                        if (isProductionDateTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Production Date: " + scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString());
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Production Date: " + scanArgs.ParseInfo.LogisticUnitProductionDate.ToShortDateString());
                        }
                    }
                    else
                    {
                        if (isProductionDateTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Production Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate))
                    {
                        if (isBestBeforeDateTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Best Before Date: " + scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString());
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Best Before Date: " + scanArgs.ParseInfo.LogisticUnitBestBeforeSellByDate.ToShortDateString());
                        }
                    }
                    else
                    {
                        if (isBestBeforeDateTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Best Before Date Missing");
                        }
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitExpiryUseByDate))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Expiry Date: " + scanArgs.ParseInfo.LogisticUnitExpiryUseByDate.ToShortDateString());
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitDueDate))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Due Date: " + scanArgs.ParseInfo.LogisticUnitDueDate.ToShortDateString());
                    }

                    if (!SysCommon.IsEmptyDateTime(scanArgs.ParseInfo.LogisticUnitPackagingDate))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Packaging Date: " + scanArgs.ParseInfo.LogisticUnitPackagingDate.ToShortDateString());
                    }

                    if (scanArgs.ParseInfo.LogisticUnitProductVariant != 0)
                    {
                        if (isVariantTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant);
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "Product Variant: " + scanArgs.ParseInfo.LogisticUnitProductVariant);
                        }
                    }
                    else
                    {
                        if (isVariantTracked)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "(Tracked) Product variant missing.");
                        }
                    }

                    if (scanArgs.ParseInfo.NetWeightMetricKGFoundAI)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Net weight metric KG: " + scanArgs.ParseInfo.NetWeightMetricKG);
                    }

                    if (scanArgs.ParseInfo.NetWeightNonMetricLBFoundAI)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Net weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLB);
                    }

                    if (scanArgs.ParseInfo.GrossWeightMetricKGFoundAI)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Gross weight metric KG: " + scanArgs.ParseInfo.GrossWeightMetricKG);
                    }

                    if (scanArgs.ParseInfo.GrossWeightNonMetricLBFoundAI)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Gross weight non-metric LB: " + scanArgs.ParseInfo.NetWeightNonMetricLB);
                    }

                    if (scanArgs.ParseInfo.LogisticUnitCountOfTradeItems != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Count of trade items: " + scanArgs.ParseInfo.LogisticUnitCountOfTradeItems);
                    }

                    if (scanArgs.ParseInfo.LogisticUnitVariableCount != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Variable Count: " + scanArgs.ParseInfo.LogisticUnitVariableCount);
                    }

                    if (scanArgs.ParseInfo.LengthOr1stDimensionMeters != 0)
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Length Meters: " + scanArgs.ParseInfo.LengthOr1stDimensionMeters);
                    }

                    // AI 91 Activity Item FK Refno
                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActItemFKRefno))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Activity Item FK Ref#: " + scanArgs.ParseInfo.ActItemFKRefno);
                    }

                    // AI 92 Activity FK Refno
                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActFKRefno))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Activity FK Ref#: " + scanArgs.ParseInfo.ActFKRefno);
                    }

                    // AI 93 Activity Detail FK Refno
                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.ActDetailFKRefno))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Activity Detail FK Ref#: " + scanArgs.ParseInfo.ActDetailFKRefno);
                    }

                    if (!string.IsNullOrEmpty(scanArgs.ParseInfo.Remainder))
                    {
                        // TODO: Resource strings
                        this.AddScanMsg(sender, scanArgs, "Unidentifiable Data");
                        this.AddScanMsg(sender, scanArgs, scanArgs.ParseInfo.Remainder);
                    }

                    // Symbology ID
                    // TODO: Resource strings
                    this.AddScanMsg(sender, scanArgs, "Symbology: " + scanArgs.ParseInfo.Symbology);

                    // Symbology Prefix
                    // TODO: Resource strings
                    this.AddScanMsg(sender, scanArgs, "Symbology Prefix: " + scanArgs.ParseInfo.SymbologyPrefix);

                    // Length
                    // TODO: Resource strings
                    this.AddScanMsg(sender, scanArgs, string.Format("Length: {0} prefix excluded", scanArgs.ParseInfo.Length));

                    // Raw Bar Code Data
                    // TODO: Resource strings
                    this.AddScanMsg(sender, scanArgs, "Bar Code Raw");

                    const int LineLength = 30;
                    List<string> rawDataLine = SysCommon.WordWrap2List(scanArgs.ParseInfo.Raw, LineLength);

                    for (int i = 0; i < rawDataLine.Count; i++)
                    {
                        this.AddScanMsg(sender, scanArgs, rawDataLine[i]);
                    }

                    //<issue="13609" author="RD" date="2016/02/22">
                    // All errors written to screen instead of throwing an error.
                    if (parse.HasErrors())
                    {
                        if (parse.Details.ErrorCount == 1)
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, "1 Error");
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.AddScanMsg(sender, scanArgs, string.Format("{0} Errors", parse.Details.ErrorCount));
                        }

                        int count = parse.Details.ErrorCount;

                        foreach (BarCodeParseErrorDetail errorDetail in parse.Details.ErrorDetails)
                        {
                            List<string> lines = SysCommon.WordWrap2List(errorDetail.Exception.Message, LineLength);

                            for (int i = 0; i < lines.Count; ++i)
                            {
                                this.AddScanMsg(sender, scanArgs, lines[i]);
                            }
                        }
                    }
                    //</issue>
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.Loading = false;
                    this.ScannerEnable();
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            try
            {
                if (this.IsControlLoaded)
                {
                    this.IsChangesMade = true;

                    //if (sender == this.updnBarCodeModifier1)
                    //{
                    this.SysEnv.CurrentBarCodeModifier1 = this.updnBarCodeModifier1.SelectedItem.ToString();
                    //}
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
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
        //private void ValidateEntry(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = false;

        //    try
        //    {
        //        if (sender == this.cmdAssignProduct)
        //        {
        //        }

        //        if (sender == this.lstScannedData)
        //        {
        //        }

        //        if (sender == this.updnBarCodeModifier1)
        //        {
        //        }
        //    }
        //    catch (Exceptions.MessageOnlyException ex)
        //    {
        //        e.Cancel = true;
        //        ex.Show(this.ActSubType);
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Cancel = false;
        //        using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
        //        {
        //            errRpt.ShowDialog();
        //        }
        //    }
        //}

        /// <summary>
        ///
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdAssignProduct_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            if (this.Parse != null)
            {
                await this.AssignProduct(sender);
            }

            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityScanBarCodes_GotFocus(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
                {
                    this.ScannerEnable();
                    this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                    if (this.IsEnabled)
                    {
                        this.IsControlLoaded = true;
                        TableActivityItemBarCodeAssociationXRef tableActItemBarCodeXRef = new TableActivityItemBarCodeAssociationXRef();
                        TableActivityItem tableActItem = new TableActivityItem();
                        TableActivity tableActivity = new TableActivity();

                        if (!this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            ////tableActivityItemBarCodeAssociationXRef.PurgeActivityItemBarCodeAssociationXRef()
                            ////tableActivityItem.PurgeActivityItem(ActivitySubTypes.BarCodeAssociation)
                            tableActivity.Purge(SysCommon.ActSubTypes.BarCodeAssociation);
                        }

                        this.ActBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
                        this.ActBarCode.Create(SysCommon.ActSubTypes.BarCodeAssociation);
                        this.ActItemBarCode = ActItemFacade.GetActItem(this.ActBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
                    }

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task AssignProduct(object sender)
        {
            string productDataText = string.Empty;

            // Changed functionality to disallow product scanning everywhere.
            // However, always prompt for password if not allowed to associate.
            bool isAllowedAssocProdBarCode = sysEnv.ContactLog.IsAllowedAssociateProductBarCode;

            if (!isAllowedAssocProdBarCode)
            {
                // Prompt for password
                FormAssociateBarCodeOverride formBarcodeOverride = new FormAssociateBarCodeOverride();

                if (await formBarcodeOverride.ShowDialog() == DialogResult.OK)
                {
                    isAllowedAssocProdBarCode = true;
                }
            }

            if (!isAllowedAssocProdBarCode)
            {
                // Just return without a message because the user will
                // already see a permission denied if they failed to enter override code.
                return;
            }

            string svdProductIdentifier = this.Parse.ProductIdentifier;
            SysCommon.BarCodeSymbologies svdSymbology = this.Parse.Symbology;
            string svdSymbologyPrefix = this.Parse.SymbologyPrefix;

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

            await form.DisplayControl.ShowDialog().ContinueWith(result =>
            {
                if (form.DisplayControl.DialogResult == DialogResult.OK)
                {
                    form.SaveChanges(this, new EventArgs());
                    XDocument xmlDoc = form.DisplayControlData;
                    this.UnitRefno = int.Parse(xmlDoc.Root.Element("ProductUnitRefno").Value);

                    if (this.UnitRefno != 0)
                    {
                        xmlDoc = XDocument.Parse(
                             "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                             + "<params>"
                             + ("<ProductUnitRefno>" + this.UnitRefno + "</ProductUnitRefno>")
                             + ("<ProductUnitBarCodeRefno>" + this.UnitBarCodeRefno + "</ProductUnitBarCodeRefno>")
                             + ("<ProductIdentifier>" + svdProductIdentifier + "</ProductIdentifier>")
                             + ("<ProductUnitBarCodeModifier1>" + this.SysEnv.CurrentBarCodeModifier1
                             + "</ProductUnitBarCodeModifier1>")
                             + "<RawData></RawData>"
                             + ("<enumBarCodeSymbology>" + (int)svdSymbology + "</enumBarCodeSymbology>")
                             + ("<BarCodeSymbologyPrefix>" + svdSymbologyPrefix + "</BarCodeSymbologyPrefix>")
                             + "</params>");

                        try
                        {
                            this.ActItemBarCode.Insert(xmlDoc, true);

                            //this.AddScanMsg(sender, null, "Product associated to bar code.");
                            Device.BeginInvokeOnMainThread(() => this.ItemsList[0] = "Product Associated To Bar Code");

                            using (SQLiteCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            {
                                cmndProdBCUnit.CommandType = CommandType.Text;
                                cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode,"
                                     + "tProductUnit.ProductUnitBC,"
                                     + "tUnit.UnitCode "
                                     + "FROM tProductUnit "
                                     + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                     + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                     + "WHERE tProductUnit.ProductUnitRefno = " + this.UnitRefno;

                                using (SQLiteDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                                {
                                    if (drProdBCUnit.Read())
                                    {
                                        productDataText = Convert.ToString(drProdBCUnit["ProductCode"]).TrimEnd()
                                                     + (" " + Convert.ToString(drProdBCUnit["ProductUnitBC"])).TrimEnd()
                                                     + " " + Convert.ToString(drProdBCUnit["UnitCode"]);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(productDataText))
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    this.lblProductData.TextColor = Color.Black;
                                    this.lblProductData.Text = productDataText;
                                });
                            }
                        }
                        catch (Exceptions.MessageOnlyException ex)
                        {
                            ex.Show(this.ActSubType);
                        }
                        catch (SQLiteException ex)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                             {
                                 FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                                 await errRpt.ShowDialog();
                             });
                        }
                        catch (Exception ex)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                             {
                                 FormErrorReport errRpt = new FormErrorReport(this.ActSubType, /*this,*/ ex);
                                 await errRpt.ShowDialog();
                             });
                        }
                        finally
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                this.Parse.ClearResetBarCodeProperties();
                                this.HasData = false;
                            });
                        }
                    }
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        //private void tmrActivityLoaded_Tick(object sender, EventArgs e)
        //{
        //    this.TotalTicks = this.TotalTicks + this.tmrActivityLoaded.Interval;

        //    if (this.IsEnabled && this.TotalTicks >= 100)
        //    {
        //        this.IsControlLoaded = true;
        //        this.tmrActivityLoaded.Enabled = false;
        //        TableActivityItemBarCodeAssociationXRef tableActItemBarCodeXRef = new TableActivityItemBarCodeAssociationXRef();
        //        TableActivityItem tableActItem = new TableActivityItem();
        //        TableActivity tableActivity = new TableActivity();

        //        if (!this.SysEnv.SysSettings.IsDisconnectedMode)
        //        {
        //            ////tableActivityItemBarCodeAssociationXRef.PurgeActivityItemBarCodeAssociationXRef()
        //            ////tableActivityItem.PurgeActivityItem(ActivitySubTypes.BarCodeAssociation)
        //            tableActivity.Purge(SysCommon.ActSubTypes.BarCodeAssociation);
        //        }

        //        this.ActBarCode = ActivityFacade.GetActivity(SysCommon.ActSubTypes.BarCodeAssociation);
        //        this.ActBarCode.Create(SysCommon.ActSubTypes.BarCodeAssociation);
        //        this.ActItemBarCode = ActItemFacade.GetActItem(this.ActBarCode, SysCommon.ActSubTypes.BarCodeAssociation);
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        //private void ucActivityScanBarCodes_LostFocus(object sender, EventArgs e)
        //{
        //    if (!this.tmrActivityLoaded.Enabled)
        //    {
        //        this.ScannerDisable();
        //    }
        //}

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

        #endregion
    }
}