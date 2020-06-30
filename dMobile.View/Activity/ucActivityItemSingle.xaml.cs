using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using dMobile.BarCodeScanner;
using dMobile.Bedrock;
using dMobile.Controller;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.CustomPages.Modal;
using dMobile.View.Forms;
using static Android.Views.View;
using dMobile.Resources.Enums;
using Android.Views;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace dMobile.View.Activity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityItemSingle : DMobileModal, IAppArea, IAppPublisher, IDataLayerEventListener
    {
        #region Fields

        private LocationMap locMap;
        private Controller.Activity actHdr;
        private ActItem actItem;
        private SysCommon.ActSubTypes actSubType;
        private BarCodeScanner.BarCodeScanner scanner = null;
        private bool isRefreshed = false;
        private bool isControlLoaded = false;
        private int totalTicks = 0;
        //private UserControl callingControl = null;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;
        private string fullClassName = "dMobile.View240x320.ucActivityItemSingle";
        private bool isChangesMade = false;
        private bool isShowDataValidMsg = true;
        private ObservableCollection<string> itemsList = new ObservableCollection<string>();
        private List<string> optionsMenuItems = new List<string>();

        private string cmdLocationText;
        private bool cmdLocationIsEnabled;

        private string cmdAddSkidText;
        private bool cmdAddSkidIsEnabled;

        private string cmdSelectSkidText;
        private bool cmdSelectSkidIsEnabled;

        private string cmdFilterText;
        private bool cmdFilterIsEnabled;

        private string cmdAddProductText;
        private bool cmdAddProductIsEnabled;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_oActivity"></param>
        /// <param name="_ActivitySubTypes"></param>
        /// <remarks></remarks>
        public ucActivityItemSingle(Controller.Activity activity, SysCommon.ActSubTypes actSubTypes)
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.cmdLocationText = "Set Location";
            this.cmdDetails.Text = "Details";
            this.cmdNext.Text = "Next";
            this.lblOrderedQuantity.Text = "Ordered";
            this.lblShippedQuantity.Text = "Shipped";
            this.lblProductBCUnit.Text = "Product";
            this.cmdAddSkidText = "Add Skid";
            this.cmdSelectSkidText = "Select Skid";
            this.cmdFilterText = "Filter";
            this.cmdAddProductText = "Add Prod";
            this.cmdPrevious.Text = "Prev";
            this.lblIsItemComment.Text = "C";

            this.LocMap = LocationMap.GetInstance();
            this.SysEnv = SysEnvironment.GetInstance();

            this.ActSubType = actSubTypes;

            // Init activity header object
            this.ActHdr = activity;

            if (this.ActHdr.SkidInfo == null)
            {
                this.ActHdr.SkidInfo = new SysSkidInfo();
            }

            // Init activity item object
            this.ActItem = ActItemFacade.GetActItem(this.ActHdr, this.ActSubType);
            this.ActItem.ActHdr = this.ActHdr;

            // Init bar code modifier
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

            // Init scanner
            this.Scanner = BarCodeSingleton.GetInstance().Scanner;
            this.Title = this.GetControlDescription();
        }

        #endregion

        /// <summary>
        /// Check if options in displayActionSheet are enabled/disabled and rebuild list
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName == nameof(cmdLocationText) ||
                propertyName == nameof(cmdLocationIsEnabled) ||
                propertyName == nameof(cmdFilterText) ||
                propertyName == nameof(cmdFilterIsEnabled) ||
                propertyName == nameof(cmdSelectSkidText) ||
                propertyName == nameof(cmdSelectSkidIsEnabled) ||
                propertyName == nameof(cmdAddProductText) ||
                propertyName == nameof(cmdAddProductIsEnabled) ||
                propertyName == nameof(cmdAddSkidText) ||
                propertyName == nameof(cmdAddSkidIsEnabled)
               )
            {
                this.PopulateOptionsMenu();
            }
        }

        /// <summary>
        /// Populates a string array to be used in DisplayActionSheet
        /// </summary>
        private void PopulateOptionsMenu()
        {
            this.OptionsMenuItems.Clear();

            if (this.cmdLocationIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdLocationText);
            }

            if (this.cmdFilterIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdFilterText);
            }

            if (this.cmdSelectSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdSelectSkidText);
            }

            if (this.cmdAddProductIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdAddProductText);
            }

            if (this.cmdAddSkidIsEnabled)
            {
                this.OptionsMenuItems.Add(this.cmdAddSkidText);
            }
        }

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.lstDisplay.ItemsSource = this.ItemsList;

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
                    else if (keycode == Keycode.F2)
                    {
                        await this.KeyUpF2_ShowLineItemComment(this, null);
                    }
                    else if (keycode == Keycode.F3)
                    {
                        await this.KeyUpF3_ShowTotalQuantity(this, null);
                    }
                }
            });

            try
            {
                await this.ucActivityItem_GotFocus(this, new EventArgs());
                this.tmrActivityItemLoaded_Tick(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                await DisplayAlert("Error", "this.ucActivityItem_GotFocus threw an error " + ex, "OK");
            }
            finally
            {
                this.Loading = false;
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            this.ScannerDisable();

            base.OnDisappearing();
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

        private bool IsShowDataValidMsg
        {
            get
            {
                return this.isShowDataValidMsg;
            }

            set
            {
                this.isShowDataValidMsg = value;
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

        private string FullClassName
        {
            get
            {
                return this.fullClassName;
            }

            set
            {
                this.fullClassName = value;
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

        private Controller.Activity ActHdr
        {
            get
            {
                return this.actHdr;
            }

            set
            {
                this.actHdr = value;
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

        private ActItem ActItem
        {
            get
            {
                return this.actItem;
            }

            set
            {
                this.actItem = value;
            }
        }

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

        private bool IsRefreshed
        {
            get
            {
                return this.isRefreshed;
            }

            set
            {
                this.isRefreshed = value;
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

        public ObservableCollection<string> ItemsList { get => itemsList; set => itemsList = value; }
        public List<string> OptionsMenuItems { get => optionsMenuItems; set => optionsMenuItems = value; }

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
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
        /// Registers the bar code scan event handler and enables the scanner.
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

            if (isReadyToSave)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsItemReviewed();

                        // Added extra check for Lekker, check all items.
                        this.IsAllItemsReviewed();
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
                            FormErrorReport errRpt = new FormErrorReport(ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }
            }

            if (isReadyToSave)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (Exceptions.MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                // TODO: Resource strings
                                if (!await DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                                {
                                    isReadyToSave = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                //MessageBox.Show(ex.Message,"Quantity Validation",MessageBoxButtons.OK,MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                                // TOOD: Resource strings
                                await DisplayAlert("Quantity Validation", ex.Message, "OK");
                                isReadyToSave = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:
                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
                    }
                    catch (Exception ex)
                    {
                        isReadyToSave = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }
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

            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            try
            {
                this.ScannerDisable();
            }
            catch (Exception ex)
            {
                isReadyToClose = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();
            }

            // Lekker requirement to check reviewed even though we are cancelling / closing.
            if (isReadyToClose)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsItemReviewed();

                        // Added extra check for Lekker, check all items.
                        this.IsAllItemsReviewed();
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
                            FormErrorReport errRpt = new FormErrorReport(ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }
            }

            // Lekker requirement to check quantity under even though we are cancelling / closing.
            if (isReadyToClose)
            {
                if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    try
                    {
                        this.IsShowDataValidMsg = true;
                        this.ItemQuantityUnderCheck();

                        // Added extra check for Lekker, check all items.
                        this.AllItemsQuantityUnderCheck();
                    }
                    catch (Exceptions.MessageOnlyException ex)
                    {
                        switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                        {
                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                // TODO: Resource strings
                                if (!await DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                                {
                                    isReadyToClose = false;
                                    this.IsShowDataValidMsg = false;
                                }

                                break;

                            case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                // TODO: Resource strings
                                await DisplayAlert("Quantity Validation", ex.Message, "OK");
                                isReadyToClose = false;
                                this.IsShowDataValidMsg = false;

                                break;

                            default:
                                // TODO: Resource strings
                                throw new NotImplementedException("Unknown this.SysEnv.SysSettings.PickingQuantityUnderWarning type.");
                        }
                    }
                    catch (Exception ex)
                    {
                        isReadyToClose = false;

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(ex);
                            await errRpt.ShowDialog();
                        });
                    }
                }
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
        //    return this.CallingControl;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            string desc = string.Empty;

            switch (this.ActSubType)
            {
                case SysCommon.ActSubTypes.PhysicalCounts:
                    // TODO: Resource strings
                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    // TODO: Resource strings
                    desc = string.Format("Receive # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Picking:
                    // TODO: Resource strings
                    desc = string.Format("Picking Order # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    // TODO: Resource strings
                    desc = string.Format("Transfer from # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    // TODO: Resource strings
                    desc = string.Format("Transfer to # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Returns:
                    // TODO: Resource strings
                    desc = string.Format("Return # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    // TODO: Resource strings
                    desc = string.Format("Count # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    // TODO: Resource strings
                    desc = string.Format("Adjustment # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Finished # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Material # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    // TODO: Resource strings
                    desc = string.Format("Ticket Add-Product # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    // TODO: Resource strings
                    desc = string.Format("Picking Order # {0}", this.ActItem.ActHdr.FKRefno);
                    break;

                default:
                    // TODO: Resource strings
                    desc = "Activity Items";
                    break;
            }

            return desc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool isSaveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    if (this.ActItem.CurrRowIdx == this.ActItem.DataViewItems.Count - 1)
                    {
                        await this.FullQuantityShippingLabels();
                    }

                    if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    {
                        // update the header record as reviewed if all items were reviewed
                        bool isReviewed = true;

                        for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                        {
                            isReviewed = isReviewed && Convert.ToBoolean(this.ActItem.DataViewItems[i]["ActivityItemPickingIsReviewed"]);
                        }

                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].BeginEdit();
                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityPickingIsReviewed"] = isReviewed;
                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].EndEdit();
                    }

                    // If any ship quantity exists do not clear the picker. Keep it locked.
                    bool hasNotBeenPicked = true;

                    if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    {
                        for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                        {
                            if (Convert.ToDecimal(this.ActItem.DataViewItems[i]["ActivityItemPickingBaseQuantityShipped"]) != 0)
                            {
                                hasNotBeenPicked = false;
                                break;
                            }
                        }
                    }

                    if (hasNotBeenPicked)
                    {
                        if (!await ((PickingActivity)ActHdr).ClearSalesOrderBeingPicked(true))
                        {
                            // TODO: Resource strings
                            await DisplayAlert("Clear Picker", "Unable to clear the current picker.", "OK");
                        }
                    }

                    // Print all receiving skid labels.
                    await PrintSkidLabels();

                    ////ClearContents();
                    this.IsChangesMade = false;
                    isSaveComplete = true;
                }
                else
                {
                    if (this.IsShowDataValidMsg)
                    {
                        // IsReadyToSave will raise errors for invalid data. This is a catch all 
                        // in the event that IsReadyToSave just returns false.

                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Data failed validation. Check your data.");
                    }

                    // Reset for next validation condition which may be or not be a quantity under check.
                    this.IsShowDataValidMsg = true;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isSaveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isSaveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }

            return isSaveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = false;

            try
            {
                if (await this.IsReadyToClose())
                {
                    if (this.ActItem.CurrRowIdx == this.ActItem.DataViewItems.Count - 1)
                    {
                        await this.FullQuantityShippingLabels();
                    }

                    // Lekker requirement, cancel will update the reviewed header status.
                    if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    {
                        // update the header record as reviewed if all items were reviewed
                        bool isReviewed = true;

                        for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                        {
                            isReviewed = isReviewed && Convert.ToBoolean(this.ActItem.DataViewItems[i]["ActivityItemPickingIsReviewed"]);
                        }

                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].BeginEdit();
                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex]["ActivityPickingIsReviewed"] = isReviewed;
                        this.ActHdr.DataViewHdrs[this.ActHdr.SelectedIndex].EndEdit();
                    }

                    // Clear any properties.
                    cancelChanges = true;

                    ////ClearContents();

                    // If any ship quantity exists do not clear the picker. Keep it locked.
                    bool hasNotBeenPicked = true;

                    if (this.ActItem.CurrRowIdx >= 0 && this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                    {
                        for (int i = 0; i <= this.ActItem.DataViewItems.Count - 1; i++)
                        {
                            if (Convert.ToDecimal(this.ActItem.DataViewItems[i]["ActivityItemPickingBaseQuantityShipped"]) != 0)
                            {
                                hasNotBeenPicked = false;
                                break;
                            }
                        }
                    }

                    if (hasNotBeenPicked)
                    {
                        if (!await ((PickingActivity)ActHdr).ClearSalesOrderBeingPicked(true))
                        {
                            // TODO: Resource strings
                            await DisplayAlert("Clear Picker", "Unable to clear the current picker.", "OK");
                        }
                    }

                    // Print all receiving skid labels.
                    ////PrintSkidLabels();

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
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
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
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
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

        #region IDataLayerEventListener Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerStatOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            ////lstDisplay.Items.Clear();

            ////const int lineLength = 30;
            ////List<string> lines = SysCommon.WordWrap2List(dataLayerEventArgs.StatusOutMsg, lineLength);
            ////for (int i = 0; i < lines.Count; i++)
            ////{
            ////    lstDisplay.Items.Add(lines[i]);
            ////}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventPublisher"></param>
        /// <param name="dataLayerEventArgs"></param>
        public void OnLayerProgOut(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs)
        {
            //if (dataLayerEventArgs.SetMinMaxValue)
            //{
            //    this.pbarStatus.Maximum = dataLayerEventArgs.MaxValue;
            //    this.pbarStatus.Minimum = dataLayerEventArgs.MinValue;
            //}

            //this.pbarStatus.Value = dataLayerEventArgs.CurrentValue;
        }

        #endregion

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private async Task PrintSkidLabels()
        {
            // Print all receiving skid labels.
            if (this.ActSubType == SysCommon.ActSubTypes.Receiving && !this.SysEnv.SysSettings.IsDisconnectedMode && this.SysEnv.SysSettings.IsSkidTrackingModule && this.SysEnv.SysSettings.AskRecvPrintAllSkidLabelsUponSave)
            {
                // TODO: Resource strings
                if (await DisplayAlert("Skid Labels", "Do you wish to print all skid label(s)?", "Yes", "No"))
                {
                    StringBuilder xmlString = new StringBuilder();
                    XmlWriter xml = XmlWriter.Create(xmlString);
                    XDocument request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + SysCommon.GetRequestDefaults(false)
                        + "<command>"
                        + "<action>SUBMIT PRINT ALL SKID LABELS</action>"
                        + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                        + ("<fk_refno>" + this.ActHdr.FKRefno + "</fk_refno>")
                        + ("<fk_source>PO</fk_source>")
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
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                            }
                            else
                            {
                                // TODO: Resource strings
                                throw new Exception("Print all skid label(s) error." + Environment.NewLine + errorDetail);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            string msg = string.Empty;
            int prodUnitRefno = 0;
            int currRowIdx = this.ActItem.CurrRowIdx;
            int foundRowIdx = this.ActItem.CurrRowIdx;
            int actItemFoundRefno = 0;
            BarCodeParse barCodeParse = null;
            SActItemSchemeData tempItemSchemeData = new SActItemSchemeData();

            try
            {
                this.ScannerDisable();

                barCodeParse = scanArgs.ParseInfo;

                // Global location identifier.
                if (!string.IsNullOrEmpty(barCodeParse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(barCodeParse.LocationIdentifier));

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdLocationText = "Set Location";
                    }
                }
                else
                {
                    if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno) &&
                        (!this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc &&
                        !(this.ActSubType == SysCommon.ActSubTypes.PickingSingle ||
                        this.ActSubType == SysCommon.ActSubTypes.Receiving)))
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Location must be set first.");
                    }

                    // Product identifier.
                    if (!string.IsNullOrEmpty(barCodeParse.ProductIdentifier))
                    {
                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                        ////    this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////{
                        ////    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////    {
                        ////        this.ActHdr.SkidInfo = new SysSkidInfo();
                        ////        this.cmdSelectSkid.Text = string.Empty;
                        ////    }
                        ////}

                        prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, barCodeParse, prodUnitRefno);
                        ProductInfo tempProdInfo = new ProductInfo();
                        tempProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                        if (this.SysEnv.SysSettings.IsPickSingleScanProdMustMatchSelProd)
                        {
                            if (this.ActItem.ProdInfo.UnitRefno != prodUnitRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException(
                                    string.Format(
                                    "Scanned product code \'{0}\' does not match the currently selected one \'{1}\'",
                                     ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo),
                                     ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo)));
                            }
                        }

                        actItemFoundRefno = this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        if (actItemFoundRefno > 0)
                        {
                            foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                bool isAddNewItem = false;

                                if (this.ActItem.IsAskCreateNewItem)
                                {
                                    //if (MessageBox.Show(
                                    //    string.Format("A line item does not exist for this warehouse {0} and product \'{1}.\'"
                                    //    + Environment.NewLine + "Do you wish to create one?",
                                    //     this.LocMap.CurrLocMap.WhsCode,
                                    //    ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo)),
                                    //    this.GetControlDescription(),
                                    //    MessageBoxButtons.YesNo,
                                    //    MessageBoxIcon.Question,
                                    //    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                    if (await DisplayAlert(this.GetControlDescription(),
                                        string.Format("A line item does not exist for this warehouse {0} and product \'{1}.\'"
                                        + Environment.NewLine + "Do you wish to create one?", this.LocMap.CurrLocMap.WhsCode, ProductInfoTasks.GetCurrReadableProdBCUnit(tempProdInfo)),
                                        "Yes", "No"))
                                    {
                                        isAddNewItem = true;
                                    }
                                }
                                else
                                {
                                    isAddNewItem = true;
                                }

                                if (isAddNewItem)
                                {
                                    await this.ActItem.Insert(ProductInfoTasks.GetXML(tempProdInfo), false);

                                    // This is a little dangerous we assume that the new row idx will be the last one.
                                    foundRowIdx = this.ActItem.DataViewItems.Count - 1;

                                    actItemFoundRefno = this.ActItem.Refno;
                                }
                                else
                                {
                                    throw new GatherProductDataException(this.ActItem.GetItemNotFoundMessage(this.LocMap, tempProdInfo), true);
                                }
                            }
                            else
                            {
                                // TODO: Resource strings
                                msg = string.Format("Line item was not found for " + Environment.NewLine + "Location {0} " + Environment.NewLine + "Product {1} BC {2} UOM {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    tempProdInfo.Code,
                                    tempProdInfo.UnitBC,
                                    tempProdInfo.UnitCode);

                                throw new Exceptions.MessageOnlyException(msg);
                            }
                        }
                    }

                    // Activity item fk refno
                    if (!string.IsNullOrEmpty(barCodeParse.ActItemFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();

                        if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning an line item bar code is not supported here.");
                        }
                        else
                        {
                            foundRowIdx = this.ActItem.DataViewItems.Find(scanArgs.ParseInfo.ActItemFKRefno);

                            if (foundRowIdx >= 0)
                            {
                                this.ActItem.SelectItem(foundRowIdx);
                                actItemFoundRefno = this.ActItem.Refno;
                                this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            }
                            else
                            {
                                throw new Exceptions.MessageOnlyException(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                    }

                    // Activity detail fk refno
                    if (!string.IsNullOrEmpty(barCodeParse.ActDetailFKRefno) && !this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here.");
                        }
                        else
                        {
                            this.ActItem.DataViewItems.Sort = this.ActItem.GetDataViewSortFKRefnoIdx();
                            tempItemSchemeData = this.ActItem.GetProdSchemeDataFromDetailFKRefno(int.Parse(barCodeParse.ActDetailFKRefno));
                            foundRowIdx = this.ActItem.DataViewItems.Find(tempItemSchemeData.ActItemFKRefno.ToString());

                            if (foundRowIdx >= 0)
                            {
                                this.ActItem.SelectItem(foundRowIdx);
                                actItemFoundRefno = this.ActItem.Refno;
                                this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                                foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                            }
                            else
                            {
                                throw new Exceptions.MessageOnlyException(this.ActItem.GetUnableToFindItemFKRefnoMessage(scanArgs.ParseInfo.ActItemFKRefno));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(barCodeParse.ActDetailFKRefno) && this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Scanning an line item detail bar code is not supported here in disconnected mode.");
                    }

                    // Shipping containser scanned, skid
                    if (!string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        if (!this.SysEnv.SysSettings.IsSkidTrackingModule)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not enabled.");
                        }

                        if (this.SysEnv.SysSettings.IsDisconnectedMode)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Scanning a shipping container bar code is not supported in disconnected mode.");
                        }

                        this.ActHdr.SkidInfo = SysSkidInfo.GetSkidInfoFromBarCodeOrCode(barCodeParse.SSCC18);

                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Skid does not exist.");
                        }

                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                        if (actItemFoundRefno > 0)
                        {
                            foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx will be the last one
                                foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                                actItemFoundRefno = this.ActItem.Refno;
                            }
                            else
                            {
                                // TODO: Resource strings
                                msg = string.Format(
                                    "Item not found for " + Environment.NewLine
                                    + "Location {0} " + Environment.NewLine + "Product {1} BC {2} Unit {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Code,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitBC,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                                throw new Exceptions.MessageOnlyException(msg);
                            }
                        }
                    }

                    // If an activity line item was found
                    if (actItemFoundRefno == 0 || foundRowIdx < 0)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Line item was not found.");
                    }
                    else
                    {
                        this.ActItem.SelectItem(foundRowIdx);
                    }

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        if (this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno != this.ActItem.ProdInfo.UnitRefno)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Identified line item product does not match the selected skid product.");
                        }
                    }

                    if (string.IsNullOrEmpty(barCodeParse.SSCC18))
                    {
                        // Rule: check if the current location and product home location are the same
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                            (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                            this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                            this.ActSubType == SysCommon.ActSubTypes.TransferTo))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = this.ActItem.GetDefaultLocMapRefno(Convert.ToInt32(this.ActItem.DataViewItems[foundRowIdx]["LocationMapRefno"]), Convert.ToInt32(this.ActItem.DataViewItems[foundRowIdx]["ProductRefno"]));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("The current location does not match the product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts
                        ////                            || this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        bool isInsertReady = false;

                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);

                        // Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        // and the fact is isn't human readable friendly.
                        SProductSchemeData schemeData = actDetail.SchemeData;

                        if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                            schemeData.LogisticUnitBatchLotNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                            schemeData.LogisticUnitBestBeforeSellByDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                            schemeData.LogisticUnitProductionDate = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                            schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                            schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                            schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        }

                        tempItemSchemeData.ProdSchemeData = schemeData;
                        actDetail.SchemeData = schemeData;

                        isInsertReady = await SysShared.GatherProductData(barCodeParse, this.ActSubType, this.ActItem, tempItemSchemeData, isInsertReady, actDetail, false);
                        this.InsertActivityDetail(ref foundRowIdx, actDetail, barCodeParse, isInsertReady);
                    }
                    else
                    {
                        // Insert a new activity detail record
                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        string scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        string unqScheme = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        XDocument xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + ("<itm_refno>" + actItem.FKRefno + "</itm_refno>")
                            + ("<alt_qty>" + this.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                            + ("<wt_qty>" + this.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                            + ("<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                            + "</imclient>");

                        this.ActItem.IsReadyToApply(xmlParam);

                        actDetail.InsertSkid(this.ActHdr.SkidInfo);

                        // Upon a save of completed data update the data view to to show new item and quantity change
                        currRowIdx = this.ActItem.UpdateDataViewQuantity(
                            foundRowIdx,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        this.ActItem.UpdateQuantityFromDataView(currRowIdx, this.ActItem.Refno);
                    }

                    this.DisplayData();

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        this.cmdAddSkidIsEnabled = true;

                        // TODO: Resource strings
                        this.cmdSelectSkidText = Convert.ToString("Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode);
                    }
                    else
                    {
                        this.cmdAddSkidIsEnabled = false;

                        // TODO: Resource strings
                        this.cmdSelectSkidText = "Select Skid";
                    }
                }
            }
            catch (IsReadyToApplyException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }

                if (this.ActItem.DataViewItems.Count > 0 && foundRowIdx >= 0 && foundRowIdx < this.ActItem.DataViewItems.Count)
                {
                    this.ActItem.CurrRowIdx = foundRowIdx;
                }
            }
            catch (GatherProductDataException ex)
            {
                if (!ex.SuppressError)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }

                if (this.ActItem.DataViewItems.Count > 0 && foundRowIdx >= 0 && foundRowIdx < this.ActItem.DataViewItems.Count)
                {
                    this.ActItem.CurrRowIdx = foundRowIdx;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);

                if (this.ActItem.DataViewItems.Count > 0 && foundRowIdx >= 0 && foundRowIdx < this.ActItem.DataViewItems.Count)
                {
                    this.ActItem.CurrRowIdx = foundRowIdx;
                }
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });

                if (this.ActItem.DataViewItems.Count > 0 && foundRowIdx >= 0 && foundRowIdx < this.ActItem.DataViewItems.Count)
                {
                    this.ActItem.CurrRowIdx = foundRowIdx;
                }
            }
            finally
            {
                this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void DisplayData()
        {
            if (0 < this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno)
            {
                this.cmdAddSkidIsEnabled = true;
                this.cmdLocationIsEnabled = false;

                // TODO: Resource strings
                this.cmdSelectSkidText = "Skid# " + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
            }
            else
            {
                this.cmdLocationIsEnabled = true;
                this.cmdAddSkidIsEnabled = false;

                // TODO: Resource strings
                this.cmdSelectSkidText = "Select Skid";
            }

            if (0 < this.LocMap.CurrLocMap.WhsLocMapRefno)
            {
                string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                Device.BeginInvokeOnMainThread(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
            }
            else
            {
                // TODO: Resource strings
                this.cmdLocationText = "Set Location";
            }

            // TODO: Resource strings
            this.lblProductBCUnit.Text = "Product: " + ProductInfoTasks.GetCurrReadableProdBCUnit(this.ActItem.ProdInfo);

            ////lstDisplay.Items.Clear()

            int idx = this.ActItem.CurrRowIdx;
            DataView items = this.ActItem.DataViewItems;

            if (idx >= 0)
            {
                decimal ordQty = Convert.ToDecimal(items[idx]["ActivityItemPickingQuantityOrdered"]);
                decimal ordWtQty = Convert.ToDecimal(items[idx]["ActivityItemPickingWeightQuantityOrdered"]);

                // TODO: Resource strings
                this.lblOrderedQuantity.Text = "Ordered: " + SysCommon.NiceQuantityReadableString(this.ActItem.ProdInfo, ordQty, ordWtQty);

                decimal shpQty = Convert.ToDecimal(this.ActItem.DataViewItems[idx]["ActivityItemPickingQuantityShipped"]);
                decimal shpWtQty = Convert.ToDecimal(this.ActItem.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"]);

                // TODO: Resource strings
                this.lblShippedQuantity.Text = "Shipped: " + SysCommon.NiceQuantityReadableString(this.ActItem.ProdInfo, shpQty, shpWtQty);

                if (Convert.ToString(items[idx]["IsActivityItemPickingComment"]) == "*")
                {
                    this.lblIsItemComment.Text = "C";
                }
                else
                {
                    this.lblIsItemComment.Text = string.Empty;
                }

                items[idx].BeginEdit();

                if (!items.Table.Columns.Contains("ActivityItemPickingIsReviewed"))
                {
                    items.Table.Columns.Add("ActivityItemPickingIsReviewed");
                }

                items[idx]["ActivityItemPickingIsReviewed"] = true;
                items[idx].EndEdit();

                if (this.SysEnv.SysSettings.IsPickPopupCmnt)
                {
                    if (Convert.ToString(items[idx]["IsActivityItemPickingComment"]) == "*")
                    {
                        // TODO: Resource strings
                        DisplayAlert("Comment", this.ActItem.GetComment(this.ActItem.Refno), "OK");

                    }
                }

                string locCode = Convert.ToString(items[idx]["LocationCode"]);
                string locName = Convert.ToString(items[idx]["LocationName"]);
                string prodDesc = Convert.ToString(items[idx]["ProductDescription"]);
                string prodUnitSizeDesc = Convert.ToString(items[idx]["ProductUnitSizeDescription"]);
                string prodBrandDesc = Convert.ToString(items[idx]["ProductBrandDescription"]);

                // TODO: Resource strings
                this.ItemsList.Clear();
                this.ItemsList.Add("DESC: " + prodDesc.Trim());
                this.ItemsList.Add("SIZE: " + prodUnitSizeDesc.Trim() + ", " + prodBrandDesc.Trim());
                this.ItemsList.Add("LOCATION: " + locCode.Trim() + " " + locName.Trim());
            }

            if (idx == items.Count - 1)
            {
                this.cmdNext.IsEnabled = false;
            }
            else
            {
                this.cmdNext.IsEnabled = true;
            }

            if (idx == 0 || items.Count <= 0)
            {
                this.cmdPrevious.IsEnabled = false;
            }
            else
            {
                this.cmdPrevious.IsEnabled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="currRowIdx"></param>
        /// <param name="actDetail"></param>
        /// <param name="barCodeParse"></param>
        /// <remarks></remarks>
        private void InsertActivityDetail(ref int currRowIdx, ActDetail actDetail, BarCodeParse barCodeParse, bool isInsertDetailReady)
        {
            if (isInsertDetailReady)
            {
                XDocument xmlDoc = actDetail.GetXML(actDetail);
                actDetail.Insert(xmlDoc);

                // Upon a save of completed data update the data view to to show new item
                // and quantity change
                currRowIdx = this.ActItem.UpdateDataViewQuantity(
                    currRowIdx,
                    actDetail.Quantity,
                    actDetail.BaseQuantity,
                    actDetail.WeightQuantity);

                this.ActItem.UpdateQuantityFromDataView(currRowIdx, this.ActItem.Refno);
            }

            this.ActItem.CurrRowIdx = currRowIdx;

            this.DisplayData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void IsItemReviewed()
        {
            bool isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                if (0 <= this.ActItem.CurrRowIdx && this.ActItem.DataViewItems.Count > this.ActItem.CurrRowIdx)
                {
                    isReviewed = Convert.ToBoolean(this.ActItem.DataViewItems[this.ActItem.CurrRowIdx]["ActivityItemPickingIsReviewed"]);
                }

                if (!isReviewed)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void IsAllItemsReviewed()
        {
            bool isReviewed = true;

            if (this.SysEnv.SysSettings.IsPickMustReviewItemsBeforeRel)
            {
                for (int idx = 0; idx <= this.ActItem.DataViewItems.Count - 1; idx++)
                {
                    isReviewed = isReviewed && Convert.ToBoolean(this.ActItem.DataViewItems[idx]["ActivityItemPickingIsReviewed"]);
                }

                if (!isReviewed)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Items exist that have not been reviewed.");
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for ALL line items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private void AllItemsQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                DataView dvItems = this.ActItem.DataViewItems;
                decimal available = 0;
                decimal shipped = 0;
                decimal ordered = 0;

                for (int currIdx = 0; currIdx <= this.ActItem.DataViewItems.Count - 1; currIdx++)
                {
                    available = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingBaseQuantityAvailable"]);
                    shipped = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingQuantityShipped"]);
                    ordered = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingQuantityOrdered"]);

                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (0 < available && shipped < ordered)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Some line items exists where the shipped quantity is less that the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task FullQuantityShippingLabels()
        {
            if (this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity)
            {
                // Only print full cases not repacks
                if (2 > this.ActItem.ProdInfo.BoxCount)
                {
                    SysCommon.SLabelQuantity labelQuantity = new SysCommon.SLabelQuantity();
                    labelQuantity = this.ActItem.GetLabelQuantity(!this.SysEnv.SysSettings.IsPickSinglePrintShipLabelFullQuantity);

                    int totalQty = labelQuantity.FullQuantity
                        + labelQuantity.FreshQuantity
                        + labelQuantity.FrozenQuantity
                        + labelQuantity.DryQuantity
                        + labelQuantity.LiveQuantity;

                    // GetLabelQuantity will also tell if the line item has been printed already.
                    if (!labelQuantity.IsPrinted)
                    {
                        if (totalQty > this.SysEnv.SysSettings.PickSingleShipLabelStopThresholdQuantity && 0 < this.SysEnv.SysSettings.PickSingleShipLabelStopThresholdQuantity)
                        {
                            FormShipLabelQuantity formLblQty = new FormShipLabelQuantity();
                            {
                                if (await formLblQty.ShowDialog() == dMobile.Resources.Enums.DialogResult.OK)
                                {
                                    // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                    // If itm_refno is zero then no line is updated as printed.
                                    ////adjLabel.Element("params").Element("whs_id").AddAfterSelf(
                                    ////    new XElement("itm_refno", this.ActItem.FKRefno));

                                    // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                                    // If itm_refno is zero then no line is updated as printed.
                                    XDocument request = XDocument.Parse(
                                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                        + "<params>"
                                        + SysCommon.GetRequestDefaults(false)
                                        + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                                        + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                                        + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                                        + ("<full_alt_qty>" + labelQuantity.FullQuantity + "</full_alt_qty>")
                                        + ("<repack_alt_qty>0</repack_alt_qty>")
                                        + ("<fresh_alt_qty>" + labelQuantity.FreshQuantity + "</fresh_alt_qty>")
                                        + ("<frozen_alt_qty>" + labelQuantity.FrozenQuantity + "</frozen_alt_qty>")
                                        + ("<dry_alt_qty>" + labelQuantity.DryQuantity + "</dry_alt_qty>")
                                        + ("<live_alt_qty>" + labelQuantity.LiveQuantity + "</live_alt_qty>")
                                        + ("<pallet_alt_qty>" + labelQuantity.PalletQuantity + "</pallet_alt_qty>")
                                        + ("<repack_fresh_alt_qty>0</repack_fresh_alt_qty>")
                                        + ("<repack_frozen_alt_qty>0</repack_frozen_alt_qty>")
                                        + ("<repack_dry_alt_qty>0</repack_dry_alt_qty>")
                                        + ("<repack_live_alt_qty>0</repack_live_alt_qty>")
                                        + ("<bundle_alt_qty>" + labelQuantity.BundleQuantity + "</bundle_alt_qty>")
                                        + ("<total_alt_qty>" + formLblQty.Quantity + "</total_alt_qty>")
                                        + "</params>");

                                    this.ActHdr.PrintLabel(request);
                                }
                            }
                        }
                        else
                        {
                            // The itm_refno elements is used to mark the ar_oeitm.slbl_prtd as true.
                            // If itm_refno is zero then no line is updated as printed.
                            XDocument request = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<params>"
                                + SysCommon.GetRequestDefaults(false)
                                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                                + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                                + ("<full_alt_qty>" + labelQuantity.FullQuantity + "</full_alt_qty>")
                                + ("<repack_alt_qty>0</repack_alt_qty>")
                                + ("<fresh_alt_qty>" + labelQuantity.FreshQuantity + "</fresh_alt_qty>")
                                + ("<frozen_alt_qty>" + labelQuantity.FrozenQuantity + "</frozen_alt_qty>")
                                + ("<dry_alt_qty>" + labelQuantity.DryQuantity + "</dry_alt_qty>")
                                + ("<live_alt_qty>" + labelQuantity.LiveQuantity + "</live_alt_qty>")
                                + ("<pallet_alt_qty>" + labelQuantity.PalletQuantity + "</pallet_alt_qty>")
                                + ("<repack_fresh_alt_qty>0</repack_fresh_alt_qty>")
                                + ("<repack_frozen_alt_qty>0</repack_frozen_alt_qty>")
                                + ("<repack_dry_alt_qty>0</repack_dry_alt_qty>")
                                + ("<repack_live_alt_qty>0</repack_live_alt_qty>")
                                + ("<bundle_alt_qty>" + labelQuantity.BundleQuantity + "</bundle_alt_qty>")
                                + ("<total_alt_qty>" + totalQty + "</total_alt_qty>")
                                + "</params>");

                            this.ActHdr.PrintLabel(request);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform quantity under check for one line item.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Line items must exist, one needs to be selected.</remarks>
        private void ItemQuantityUnderCheck()
        {
            if (this.SysEnv.SysSettings.PickQtyUnderWarn != SysCommon.ActPickingQtyUnderValLevels.NoValidationOrWarning)
            {
                int currIdx = this.ActItem.CurrRowIdx;
                if (0 <= currIdx && this.ActItem.DataViewItems.Count > currIdx)
                {
                    DataView dvItems = this.ActItem.DataViewItems;
                    decimal available = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingBaseQuantityAvailable"]);
                    decimal shipped = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingQuantityShipped"]);
                    decimal ordered = Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingQuantityOrdered"]);

                    if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
                    {
                        // Do not warn if product is out of stock WO# 12054
                        if (0 < available && shipped < ordered)
                        {
                            string msg = string.Empty;

                            if (this.SysEnv.SysSettings.IsPickQtyUnderShowBaseQtyAvail)
                            {
                                // TODO: Resource strings
                                msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingBaseQuantityAvailable"]), Convert.ToString(dvItems[currIdx]["BaseUnitCode"]));
                            }
                            else
                            {
                                // TODO: Resource strings
                                msg = string.Format("The quantity shipped is less than the quantity ordered." + Environment.NewLine + "On Hand {0} {1}", Convert.ToDecimal(dvItems[currIdx]["ActivityItemPickingQuantityAvailable"]), Convert.ToString(dvItems[currIdx]["UnitCode"]));
                            }

                            throw new Exceptions.MessageOnlyException(msg);
                        }
                    }
                    else
                    {
                        if (shipped < ordered)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("The quantity shipped is less than the quantity ordered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void RefreshData()
        {
            this.ItemsList.Clear();

            try
            {
                this.ActItem.Refresh(this.ActHdr.XMLDataCriteria);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                DisplayAlert(this.GetControlDescription(), ex.Message, "OK");

            }

            ////If iCount > 50 And DateAndTime.Hour(DateTime.Now) Mod 2 = 0 And DateAndTime.Minute(DateTime.Now) Mod 2 = 0 Then
            ////  'use relection to get the current assembly path for the database
            ////  Dim strAppDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)
            ////  Dim strFullPathToMyFile As String = Path.Combine(strAppDir, "dMobileResource01.wav")
            ////  Dim oElevatorMusic As clsSound = New clsSound(strFullPathToMyFile)
            ////  oElevatorMusic.Play()
            ////End If

            this.ActItem.DataViewItems = this.ActItem.GetDataView(false);

            if (this.ActItem.DataViewItems.Count > 0)
            {
                this.ActItem.CurrRowIdx = 0;
                this.ActItem.SelectItem(this.ActItem.CurrRowIdx);

                string cmnt = this.ActItem.GetComment(Convert.ToInt32(this.ActItem.DataViewItems[this.ActItem.CurrRowIdx]["ActivityItemRefno"]));

                if (!string.IsNullOrEmpty(cmnt))
                {
                    if (cmnt.Length > 30)
                    {
                        for (int line = 1; line <= 3; line++)
                        {
                            this.ItemsList.Add(cmnt);
                        }
                    }
                    else
                    {
                        this.ItemsList.Add(cmnt);
                    }
                }

                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                    Device.BeginInvokeOnMainThread(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                }
                else
                {
                    // TODO: Resource strings
                    this.cmdLocationText = "Set Location";
                }

                this.cmdPrevious.IsEnabled = true;
                this.cmdNext.IsEnabled = true;
            }
            else
            {
                this.cmdPrevious.IsEnabled = false;
                this.cmdNext.IsEnabled = false;

                this.ItemsList.Clear();
            }

            this.DisplayData();
            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task ucActivityItem_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                if (this.IsControlLoaded == false)
                {
                    this.ActItem.CurrRowIdx = -1;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                        await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                    }
                    else
                    {
                        // TODO: Resource strings
                        this.cmdLocationText = "Set Location";
                    }

                    this.cmdAddSkidIsEnabled = !this.SysEnv.SysSettings.IsPickDisallowManualAddProd;
                    this.cmdAddProductIsEnabled = !this.SysEnv.SysSettings.IsPickDisallowManualAddProd;

                    this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                    this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchHdr_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsChangesMade = true;
            this.ActItem.Search(this.txtSearchItem.Text);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void tmrActivityItemLoaded_Tick(object sender, EventArgs e)
        {
            if (this.IsEnabled)
            {
                this.RefreshData();
                this.cmdLocationIsEnabled = true;
                this.cmdSelectSkidIsEnabled = this.SysEnv.SysSettings.IsSkidTrackingModule;
                this.cmdAddProductIsEnabled = this.ActItem.IsAllowAddItem;
                this.IsControlLoaded = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void txtSearchItem_GotFocus(object sender, EventArgs e)
        {
            //this.txtSearchItem.SelectAll();

            if (this.txtSearchItem.Text != null)
            {
                this.txtSearchItem.CursorPosition = 0;
                this.txtSearchItem.SelectionLength = this.txtSearchItem.Text.Length;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdLocation_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    this.ScannerDisable();

                    // if cycle by slot
                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

                    if (await form.DisplayControl.ShowDialog() == dMobile.Resources.Enums.DialogResult.OK)
                    {
                        if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                        {
                            string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                        }
                        else
                        {
                            // TODO: Resource strings
                            this.cmdLocationText = "Set Location";
                        }
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdSelectSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    this.ScannerDisable();

                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSkidInfo, this.ActItem, this.ActSubType);
                    {
                        await form.DisplayControl.ShowDialog();
                        await form.SaveChanges(sender, e);
                        XDocument xmlDoc = form.DisplayControlData;
                        this.ActHdr.SkidInfo = SysSkidInfo.GetSkidInfoFromXML(xmlDoc);

                        if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        {
                            this.cmdAddSkidIsEnabled = true;

                            // TODO: Resource strings
                            this.cmdSelectSkidText = Convert.ToString("Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode);

                            this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;

                            string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                            await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);

                            this.cmdLocationIsEnabled = false;
                        }
                        else
                        {
                            this.cmdAddSkidIsEnabled = false;

                            // TODO: Resource strings
                            this.cmdSelectSkidText = "Select Skid";

                            this.cmdLocationIsEnabled = true;
                        }
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdAddProduct_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    int itemFoundRefno = 0;
                    int prodUnitRefno = 0;
                    int foundRowIdx = 0;

                    this.ScannerDisable();

                    // Not allowed to count individual boxes on a skid. Skids must be
                    // scanned as a whole unit or built in skid maintenance.

                    // DEV-827 This change to allow counting partial skids is a bad idea.                        
                    ////if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                    ////            this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                    ////{
                    ////    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    ////    {
                    ////        this.ActHdr.SkidInfo = new SysSkidInfo();
                    ////        this.cmdSelectSkid.Text = string.Empty;
                    ////    }
                    ////}

                    XDocument xmlDoc = null;

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno == 0)
                    {
                        // activity item object needs to check if a new location and product can be added, if not add a detail
                        if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("The location must be set.");
                        }

                        FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProductUnit, this.ActSubType);

                        if (await form.DisplayControl.ShowDialog() == dMobile.Resources.Enums.DialogResult.OK)
                        {
                            await form.SaveChanges(sender, e);
                            xmlDoc = form.DisplayControlData;
                            prodUnitRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductUnitRefno").Value);
                        }
                    }
                    else
                    {
                        this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        ProductInfo info = this.ActHdr.SkidInfo.CurrSkid.ProdInfo;

                        xmlDoc = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<params>"
                            + ("<ProductUnitRefno>" + info.UnitRefno + "</ProductUnitRefno>")
                            + ("<ProductRefno>" + info.Refno + "</ProductRefno>")
                            + ("<ProductCode>" + info.Code + "</ProductCode>")
                            + ("<ProductDescription>" + SysCommon.AddXMLReservedChars(info.Desc) + "</ProductDescription>")
                            + ("<UnitCode>" + info.UnitCode + "</UnitCode>")
                            + ("<ProductUnitBC>" + info.UnitBC + "</ProductUnitBC>")
                            + ("<ProductUnitSizeDescription>" + SysCommon.AddXMLReservedChars(info.UnitSizeDesc)
                            + "</ProductUnitSizeDescription>")
                            + "</params>");
                    }

                    if (prodUnitRefno != 0)
                    {
                        itemFoundRefno = this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);

                        if (itemFoundRefno > 0)
                        {
                            string oldSort = this.ActItem.DataViewItems.Sort;
                            this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                            foundRowIdx = this.ActItem.DataViewItems.Find(itemFoundRefno);
                            this.ActItem.CurrRowIdx = foundRowIdx;
                            this.ActItem.DataViewItems.Sort = oldSort;
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(xmlDoc, false);

                                // This is a little dangerous we assume that the new row idx 
                                // will be the last one
                                foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                                this.ActItem.CurrRowIdx = foundRowIdx;
                                itemFoundRefno = this.ActItem.Refno;
                            }
                            else
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("Product rule says that it must exist on this document already.");
                            }
                        }

                        // Rule: Check if the current location and product home location are the same.
                        if (this.SysEnv.SysSettings.IsVerifyProductLocation &&
                                      (this.ActSubType == SysCommon.ActSubTypes.Picking ||
                                      this.ActSubType == SysCommon.ActSubTypes.TransferFrom ||
                                      this.ActSubType == SysCommon.ActSubTypes.TransferTo))
                        {
                            // LocationMapRefno is assumed to be the picking WarehouseLocationMapRefno
                            int locMapRefno = this.ActItem.GetDefaultLocMapRefno(
                                Convert.ToInt32(this.ActItem.DataViewItems[foundRowIdx]["LocationMapRefno"]),
                                Convert.ToInt32(this.ActItem.DataViewItems[foundRowIdx]["ProductRefno"]));

                            if (this.LocMap.CurrLocMapRefno != locMapRefno)
                            {
                                // TODO: Resource strings
                                throw new Exceptions.MessageOnlyException("The current location does not match the product home location.");
                            }
                        }

                        // Not allowed to count individual boxes on a skid. Skids must be
                        // scanned as a whole unit or built in skid maintenance.

                        // DEV-827 This change to allow counting partial skids is a bad idea.                        
                        ////                        if (this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts ||
                        ////                                      this.ActSubType == SysCommon.ActSubTypes.CycleCounts)
                        ////                        {
                        ////                            if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////                            {
                        ////                                this.ActItem.ActHdr.SkidInfo = new SysSkidInfo();
                        ////#if MULTILANGUAGE
                        ////                this.cmdSelectSkid.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectSkid);
                        ////#else
                        ////                                this.cmdSelectSkid.Text = "Select Skid";
                        ////#endif
                        ////                            }
                        ////                        }

                        ////BarCodeParse barCodeParse = BarCodeParseFacade.ParseData(string.Empty);
                        ////bool isInsertReady = false;
                        ////ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        ////actDetail.Quantity = 0;
                        ////actDetail.BaseQuantity = 0;
                        ////actDetail.WeightQuantity = 0;
                        ////actDetail.SchemeID = string.Empty;
                        ////actDetail.UniqueSchemeID = string.Empty;
                        ////actDetail.ActExtra.Text1 = string.Empty;
                        ////actDetail.ActExtra.Text2 = string.Empty;
                        ////actDetail.ActExtra.Text3 = string.Empty;
                        ////actDetail.ActExtra.Text4 = string.Empty;
                        ////actDetail.ActExtra.Text5 = string.Empty;

                        ////// Did not use this.ActItem.ActHdr.SkidInfo.CurrSkid.LogisticUnitSkidCode because of the len of the code
                        ////// and the fact is isn't human readable friendly.
                        ////SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();
                        ////SProductSchemeData schemeData = actDetail.SchemeData;

                        ////if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                        ////{
                        ////  schemeData.LogisticUnitSkidNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        ////  schemeData.LogisticUnitBatchLotNumber
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBatchLotNumber;
                        ////  schemeData.LogisticUnitBestBeforeSellByDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitBestBeforeSellByDate;
                        ////  schemeData.LogisticUnitProductionDate
                        ////      = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.LogisticUnitProductionDate;
                        ////  schemeData.VariantCode = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantCode;
                        ////  schemeData.VariantNumber = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantNumber;
                        ////  schemeData.VariantDescription = this.ActItem.ActHdr.SkidInfo.CurrSkid.SchemeData.VariantDescription;
                        ////}

                        ////tempItemProdSchemeData.ProdSchemeData = schemeData;
                        ////actDetail.SchemeData = schemeData;

                        ////SysShared.GatherProductData(
                        ////    barCodeParse,
                        ////    this.ActSubType,
                        ////    this.ActItem,
                        ////    tempItemProdSchemeData,
                        ////    this.CallingControl,
                        ////    ref isInsertReady,
                        ////    actDetail,
                        ////    true);

                        ////this.InsertActivityDetail(ref foundRowIdx, actDetail, barCodeParse, isInsertReady);
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (GatherProductDataException ex)
                {
                    if (!ex.SuppressError)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            FormErrorReport errRpt = new FormErrorReport(ex);
                            await errRpt.ShowDialog();
                        });
                    }

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            this.RefreshData();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });

                    // Do not refresh product list for counts. See WO# 12222.
                    // Reloading count items into the thousands is too slow.
                    // This could change depending on the Exception type such as Exceptions.MessageOnlyException vs Exception.
                    // Refreshing will have to be accomplished by exiting the items activity.
                    switch (this.ActSubType)
                    {
                        case SysCommon.ActSubTypes.PhysicalCounts:
                            break;

                        case SysCommon.ActSubTypes.CycleCounts:
                            break;

                        default:
                            // Performing a refresh data will remove and partially added new items or quantity.
                            this.RefreshData();
                            break;
                    }
                }
                finally
                {
                    DisplayData();
                    this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task cmdAddSkid_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    int actItemFoundRefno = 0;
                    int prodUnitRefno = 0;
                    int foundRowIdx = 0;
                    string message = string.Empty;

                    this.ScannerDisable();

                    // TODO: Resource strings
                    if (await DisplayAlert("Skid", "Add the ENTIRE skid to this activity?", "Yes", "No"))
                    {
                        prodUnitRefno = this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitRefno;
                        actItemFoundRefno = this.ActItem.GetRefnoFromProdUnitRefno(prodUnitRefno);
                        this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                        if (actItemFoundRefno > 0)
                        {
                            foundRowIdx = this.ActItem.DataViewItems.Find(actItemFoundRefno);
                        }
                        else
                        {
                            if (this.ActItem.IsAllowAddItem)
                            {
                                await this.ActItem.Insert(ProductInfoTasks.GetXML(this.ActHdr.SkidInfo.CurrSkid.ProdInfo), false);

                                // This is a little dangerous we assume that the new row idx 
                                // will be the last one
                                foundRowIdx = this.ActItem.DataViewItems.Count - 1;
                                actItemFoundRefno = this.ActItem.Refno;
                            }
                            else
                            {
                                // TODO: Resource strings
                                message = string.Format("Item not found for " + Environment.NewLine + "Location {0} " + Environment.NewLine + "Product {1} BC {2} Unit {3}",
                                    LocationMapTasks.GetCurrentReadableLocation(),
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Code,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitBC,
                                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.UnitCode);

                                throw new Exceptions.MessageOnlyException(message);
                            }
                        }

                        this.ActItem.CurrRowIdx = foundRowIdx;

                        // Insert a new activity detail record
                        ActDetail actDetail = ActDetailFacade.GetActDetail(this.ActItem, this.ActSubType);
                        actDetail.ActItem = this.ActItem;

                        string scheme = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        string unqScheme = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                            this.ActItem.ProdInfo.Code,
                            this.ActItem.ProdInfo.Scheme,
                            actDetail.SchemeData);

                        // IsReadyToApply will perform any validations, such as duplicate checks and quantity validations.
                        XDocument xmlParam = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + ("<itm_refno>" + actItem.FKRefno + "</itm_refno>")
                            + ("<alt_qty>" + this.ActHdr.SkidInfo.CurrSkid.Quantity + "</alt_qty>")
                            + ("<wt_qty>" + this.ActHdr.SkidInfo.CurrSkid.WeightQuantity + "</wt_qty>")
                            + ("<ActivityDetailUniqueSchemeID>" + actDetail.UniqueSchemeID + "</ActivityDetailUniqueSchemeID>")
                            + "</imclient>");

                        this.ActItem.IsReadyToApply(xmlParam);

                        actDetail.InsertSkid(this.ActHdr.SkidInfo);

                        // upon a save of completed data update the data view to to show new item 
                        // and quantity change
                        this.ActItem.CurrRowIdx = this.ActItem.UpdateDataViewQuantity(
                            foundRowIdx,
                            actDetail.Quantity,
                            actDetail.BaseQuantity,
                            actDetail.WeightQuantity);

                        this.ActItem.UpdateQuantityFromDataView(foundRowIdx, this.ActItem.Refno);
                        this.DisplayData();
                    }
                }
                catch (IsReadyToApplyException)
                {
                    // Do nothing.
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdDetails_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    this.ScannerDisable();
                    this.ActItem.SelectItem(this.ActItem.CurrRowIdx);

                    if (this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        SSkidInfo currSkid = this.ActItem.ActHdr.SkidInfo.CurrSkid;

                        if (currSkid.ProdInfo.Refno != this.ActItem.ProdInfo.Refno)
                        {
                            // TODO: Resource strings
                            throw new Exceptions.MessageOnlyException("Selected line item product must match the selected skid product.");
                        }
                    }

                    FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucActivityDetail, this.ActItem, this.ActSubType);
                    await form.DisplayControl.ShowDialog();
                    await form.SaveChanges(sender, e);
                    XDocument xmlDoc = form.DisplayControlData;

                    this.ActItem.RefreshQuantity(
                        this.ActItem.CurrRowIdx,
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("base_qty").Value), 3),
                        Math.Round(decimal.Parse(xmlDoc.Root.Element("wt_qty").Value), 3));

                    this.ActItem.UpdateQuantityFromDataView(this.ActItem.CurrRowIdx, this.ActItem.Refno);

                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno != 0)
                    {
                        this.cmdAddSkidIsEnabled = true;

                        // TODO: Resource strings
                        this.cmdSelectSkidText = "Skid# " + this.ActHdr.SkidInfo.CurrSkid.SkidCode;
                        this.cmdLocationIsEnabled = false;
                    }
                    else
                    {
                        this.cmdAddSkidIsEnabled = false;

                        // TODO: Resource strings
                        this.cmdSelectSkidText = "Select Skid";
                        this.cmdLocationIsEnabled = true;
                    }

                    this.DisplayData();
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.ScannerEnable();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdNext_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    bool isValid = true;

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        if (this.ActItem.CurrRowIdx < this.ActItem.DataViewItems.Count)
                        {
                            try
                            {
                                this.ItemQuantityUnderCheck();
                            }
                            catch (Exception ex)
                            {
                                switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                                {
                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                        // TODO: Resource strings
                                        if (!await DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                                        {
                                            isValid = false;
                                        }

                                        break;

                                    case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                        // TODO: Resource strings
                                        await DisplayAlert("Quantity Validation", ex.Message, "OK");
                                        isValid = false;

                                        break;
                                }
                            }
                        }
                        else
                        {
                            // TODO: Resource strings
                            await DisplayAlert(this.GetControlDescription(), "Last Item", "OK");
                        }
                    }

                    if (isValid)
                    {
                        await this.FullQuantityShippingLabels();

                        this.ActItem.CurrRowIdx++;
                        if (this.ActItem.CurrRowIdx >= this.ActItem.DataViewItems.Count)
                        {
                            this.ActItem.CurrRowIdx = this.ActItem.DataViewItems.Count - 1;
                        }

                        if (this.ActItem.CurrRowIdx >= 0)
                        {
                            this.ActItem.SelectItem(this.ActItem.CurrRowIdx);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            {
                                string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                                await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                            }
                            else
                            {
                                // TODO: Resource strings
                                this.cmdLocationText = "Set Location";
                            }
                        }
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.DisplayData();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdPrevious_Click(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                try
                {
                    bool isValid = true;

                    if (this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                    {
                        try
                        {
                            this.ItemQuantityUnderCheck();
                        }
                        catch (Exception ex)
                        {
                            switch (this.SysEnv.SysSettings.PickQtyUnderWarn)
                            {
                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndContinue:
                                    // TODO: Resource strings
                                    if (!await DisplayAlert("Quantity Validation", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                                    {
                                        isValid = false;
                                    }

                                    break;

                                case SysCommon.ActPickingQtyUnderValLevels.ValidateWarnAndStop:
                                    // TODO: Resource strings
                                    await DisplayAlert("Quantity Validation", ex.Message, "OK");

                                    isValid = false;
                                    break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        await this.FullQuantityShippingLabels();

                        this.ActItem.CurrRowIdx--;
                        if (this.ActItem.CurrRowIdx < 0)
                        {
                            this.ActItem.CurrRowIdx = 0;
                        }

                        if (this.ActItem.CurrRowIdx >= 0)
                        {
                            this.ActItem.SelectItem(this.ActItem.CurrRowIdx);
                        }

                        if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
                            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                            {
                                string currentReadableLocation = LocationMapTasks.GetCurrentReadableLocation();
                                await Device.InvokeOnMainThreadAsync(() => this.cmdLocationText = "Location: " + Environment.NewLine + currentReadableLocation);
                            }
                            else
                            {
                                // TODO: Resource strings
                                this.cmdLocationText = "Set Location";
                            }
                        }
                    }
                }
                catch (Exceptions.MessageOnlyException ex)
                {
                    ex.Show(this.ActSubType);
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    this.DisplayData();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF2_ShowLineItemComment(object sender, KeyEventArgs e)
        {
            try
            {
                this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.ScannerDisable();

                if (0 > this.ActItem.CurrRowIdx)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("Please select and line item to show the comments for.");
                }

                string comment = this.ActItem.GetComment(Convert.ToInt32(this.ActItem.DataViewItems[this.ActItem.CurrRowIdx]["ActivityItemRefno"]));

                if (!string.IsNullOrEmpty(comment))
                {
                    // TODO: Resource strings
                    await DisplayAlert("Item Comment", comment, "OK");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task KeyUpF3_ShowTotalQuantity(object sender, KeyEventArgs e)
        {
            try
            {
                this.ReqCancelDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveDisableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());

                this.ScannerDisable();

                string msg = this.ActItem.GetTotalQuantity();

                if (!string.IsNullOrEmpty(msg))
                {
                    // TODO: Resource strings
                    await DisplayAlert(this.GetControlDescription(), msg, "OK");
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }
            finally
            {
                this.ScannerEnable();

                this.ReqCancelEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
                this.ReqSaveEnableEvent?.Invoke((IAppPublisher)this, new AppEventArgs());
            }
        }

        #endregion

        private async void optionsMenu_Clicked(object sender, EventArgs e)
        {
            if (this.Loading)
            {
                return;
            }

            // TODO: Resource strings
            string result = await DisplayActionSheet("ActivityItemSingle Options", "Close", null, this.OptionsMenuItems.ToArray());

            if (result == this.cmdLocationText && this.cmdLocationIsEnabled)
            {
                await this.cmdLocation_Click(sender, e);
            }
            else if (result == this.cmdFilterText && this.cmdFilterIsEnabled)
            {
                //await this.cmdFilter_Clicked(sender, e);
            }
            else if (result == this.cmdSelectSkidText && this.cmdSelectSkidIsEnabled)
            {
                await this.cmdSelectSkid_Click(sender, e);
            }
            else if (result == this.cmdAddProductText && this.cmdAddProductIsEnabled)
            {
                await this.cmdAddProduct_Click(sender, e);
            }
            else if (result == this.cmdAddSkidText && this.cmdAddSkidIsEnabled)
            {
                await this.cmdAddSkid_Click(sender, e);
            }
        }

        private void updnBarCodeModifier1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsChangesMade = true;
            this.SysEnv.CurrentBarCodeModifier1 = Convert.ToString(this.updnBarCodeModifier1.SelectedItem);
        }
    }
}