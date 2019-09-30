// Reviewed on 2015/04/21 @ 10:21
namespace dMobile.View320x240
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Controller;

    public partial class ucActivityConvertUnit : UserControl, IAppArea, IAppPublisher
    {
        #region Fields

        private UserControl callingControl = new ucInventoryMenu();
        private bool isControlLoaded = false;
        private bool isChangesMade = false;
        private BarCodeScanner scanner = BarCodeSingleton.GetInstance().Scanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysEnvironment sysEnv = SysEnvironment.GetInstance();
        private LocationMap locMap = LocationMap.GetInstance();
        private Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private int fromProdUnitRefno = 0;
        private int toProdUnitRefno = 0;
        private bool isNumericTrapped = false;
        private bool isEscapeTrapped = false;
        private bool isEditTrapped = false;
        private SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.ConvertUnit;
        private ReqCloseHandler reqCloseEvent;
        private ReqSaveEnableHandler reqSaveEnableEvent;
        private ReqSaveDisableHandler reqSaveDisableEvent;
        private ReqCancelEnableHandler reqCancelEnableEvent;
        private ReqCancelDisableHandler reqCancelDisableEvent;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public ucActivityConvertUnit()
        {
            // This call is required by the Windows Form Designer.
            this.InitializeComponent();
#if MULTILANGUAGE
      Translator t = Translator.GetInstance();
      this.cmdLocation.Text = t.Translate(TranslatorVocabulary.SetLocation);
      this.cmdSelectToUnit.Text = t.Translate(TranslatorVocabulary.SelectToUnit);
      this.cmdSelectFromUnit.Text = t.Translate(TranslatorVocabulary.SelectFromUnit);
      this.lblConvertedUnitInfo.Text = t.Translate(TranslatorVocabulary.ConvertedQtyUOMKGLB);
      this.lblFullUnitInfo.Text = t.Translate(TranslatorVocabulary.QtyUOMKGLB);
#endif


            // Add any initialization after the InitializeComponent() call.
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
                this.updnBarCodeModifier1.Visible = false;
                this.updnBarCodeModifier1.Enabled = false;
            }

            this.updnBarCodeModifier1.Text = this.SysEnv.CurrentBarCodeModifier1;

            this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.ConvertUnit);
            this.Activity.SkidInfo = new SysSkidInfo();
            this.Activity.ActSubType = SysCommon.ActSubTypes.ConvertUnit;

            this.ActItem = ActItemFacade.GetActItem(
                this.Activity,
                SysCommon.ActSubTypes.ConvertUnit);
            this.ActItem.ActSubType = this.Activity.ActSubType;
            this.ActItem.GetDataView(false);

            this.ActDetail = ActDetailFacade.GetActDetail(
                this.ActItem,
                SysCommon.ActSubTypes.ConvertUnit);
            this.ActDetail.ActSubType = this.ActItem.ActSubType;
            this.ActDetail.DesignDataGrid(this.dgUnitWeight);

            this.txtGridEdit.Visible = false;
            this.dgUnitWeight.Controls.Add(this.txtGridEdit);
            this.ActDetail.GetDataView(false);
            this.dgUnitWeight.DataSource = this.ActDetail.DataViewDetails;
        }

        #endregion

        #region Delegates

        public delegate void ReqCloseHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        public delegate void ReqSaveEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        public delegate void ReqSaveDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        public delegate void ReqCancelEnableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

        public delegate void ReqCancelDisableHandler(IAppPublisher appEventPub, AppEventArgs appEventArgs);

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

        private BarCodeScanner Scanner
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

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
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

        private Activity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
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

        private int FromProdUnitRefno
        {
            get
            {
                return this.fromProdUnitRefno;
            }

            set
            {
                this.fromProdUnitRefno = value;
            }
        }

        private int ToProdUnitRefno
        {
            get
            {
                return this.toProdUnitRefno;
            }

            set
            {
                this.toProdUnitRefno = value;
            }
        }

        private UserControl CallingControl
        {
            get
            {
                return this.callingControl;
            }

            set
            {
                this.callingControl = value;
            }
        }

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

        private bool IsEscapeTrapped
        {
            get
            {
                return this.isEscapeTrapped;
            }

            set
            {
                this.isEscapeTrapped = value;
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

        #endregion

        #region Public Members

        #region IAppArea

        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public void ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
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
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used for post validation of all control combinations. Not an individual, use ValidateEntry.
        /// Must have a try catch block.</remarks>
        public bool IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                {
#if MULTILANGUAGE
          throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.LocationMustBeSet));
#else
                    throw new MessageOnlyException("Location must be set.");
#endif
                }

                if (0 >= this.FromProdUnitRefno)
                {
#if MULTILANGUAGE
          throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.ConvertFromProductMustBeSet));
#else
                    throw new MessageOnlyException("Convert from product must be set.");
#endif
                }

                if (0 >= this.ActItem.DataViewItems.Count)
                {
#if MULTILANGUAGE
          throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.NoConvertToItemsHaveBeDefined));
#else
                    throw new MessageOnlyException("No convert to items have be defined.");
#endif
                }

                if (0 >= this.ActDetail.DataViewDetails.Count)
                {
#if MULTILANGUAGE
          throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.NoConvertToItemDetailsHaveBeDefined));
#else
                    throw new MessageOnlyException("No convert to item details have be defined.");
#endif
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;
                using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errRpt.ShowDialog();
                }
            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsReadyToClose()
        {
            bool isReadyToClose = true;

            try
            {
                // NOTE: This code is needed to clean up the scanner driver.
                // Symbol had threading issues without this call when dMobile was terminating.
                // All user controls that initalize a bar code need this code.
                // The parent form (not a user control) will call scanner.terminate and dispose
                // of the scanner instance upon exiting dMobile.
                if (this.Scanner != null)
                {
                    this.ScannerDisable();
                }
            }
            catch (MessageOnlyException ex)
            {
                isReadyToClose = false;
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToClose = false;
                using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errRpt.ShowDialog();
                }
            }

            return isReadyToClose;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public UserControl GetCallingControl()
        {
            return new ucInventoryMenu();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
#if MULTILANGUAGE
        return Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnit);
#else
            return "Convert Unit";
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (this.IsReadyToSave())
                {
                    if (this.FromProdUnitRefno != 0 && this.ActItem.DataViewItems.Count != 0 &&
                        this.ActDetail.DataViewDetails.Count != 0)
                    {
                        string dataReq = SysCommon.SubmitComm(this.MakeConvertUnitSubmitString());
                        if (dataReq.Length > 0)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(dataReq);
                            if (dataReq.Contains("<message>ERROR</message>"))
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
                                if (this.SysEnv.SysSettings.CanPrintProductLabelConvertUnits)
                                {
                                    if (MessageBox.Show(
#if MULTILANGUAGE
                      Translator.GetInstance().Translate(TranslatorVocabulary.UnitConversionComplete) + Environment.NewLine +
                      Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToPrintProductLabels),
                      Translator.GetInstance().Translate(TranslatorVocabulary.UnitConversion),
#else
"Unit conversion complete." + Environment.NewLine +
                                        "Do you wish to print product labels?",
                                        "Unit Conversion",
#endif
 MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                    {
                                        for (int idx = 0; idx <= ActDetail.DataViewDetails.Count - 1; idx++)
                                        {
                                            dataReq = SysCommon.SubmitComm(this.MakeProductLabelsSubmitString(idx));
                                            if (dataReq.Contains("<message>ERROR</message>"))
                                            {
                                                xmlDoc.LoadXml(dataReq);
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
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(
#if MULTILANGUAGE
                      Translator.GetInstance().Translate(TranslatorVocabulary.UnitConversionComplete),
                      Translator.GetInstance().Translate(TranslatorVocabulary.UnitConversion),
#else
"Unit conversion complete.",
                                        "Unit Conversion",
#endif
 MessageBoxButtons.OK,
                                        MessageBoxIcon.None,
                                        MessageBoxDefaultButton.Button1);
                                }
                            }

                            this.ClearContents();
                        }

                        saveComplete = true;
                        this.IsChangesMade = false;

                        if (MessageBox.Show(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToConvertAnyMoreUnits),
                Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnits),
#else
"Do you wish to convert any more unit(s)?",
                            "Convert Unit(s)",
#endif
 MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            saveComplete = false;
                        }
                    }
                    else
                    {
                        if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                        {
                            MessageBox.Show(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.LocationMustBeSet),
                  Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnits),
#else
"Location must be set.",
                                "Convert Unit(s)",
#endif
 MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                        else if (this.ActItem.DataViewItems.Count == 0)
                        {
                            MessageBox.Show(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.ConvertFromUnitMustBeSet),
                  Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnits),
#else
"Convert from unit must be set.",
                                "Convert Unit(s)",
#endif
 MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                        else if (this.ActDetail.DataViewDetails.Count == 0)
                        {
                            MessageBox.Show(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.ConvertToUnitsMustBeSet),
                  Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnits),
#else
"Convert to unit(s) must be set.",
                                "Convert Unit(s)",
#endif
 MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errRpt.ShowDialog();
                }
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                // IsReadyToClose checks if any changes where made.
                if (this.IsReadyToClose())
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
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errorReport.ShowDialog();
                }
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
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
        /// <remarks></remarks>
        private void DisplayData()
        {
            if (this.ActItem.DataViewItems.Count > 0)
            {
                ProductInfo fromProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);

                this.lblFullUnitInfo.Text = string.Format(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.Qty),
#else
"Qty {0} {1} @ {2} {3}",
#endif
 this.ActItem.DataViewItems[0]["ActivityDetailQuantity"],
                    fromProdInfo.UnitCode,
                    Math.Round(Convert.ToDecimal(this.ActItem.DataViewItems[0]["ActivityDetailWeightQuantity"]), 3),
                    fromProdInfo.WeightUnitCode);
            }
            else
            {
#if MULTILANGUAGE
        this.lblFullUnitInfo.Text = Translator.GetInstance().Translate(TranslatorVocabulary.QtyUOMKGLB);
#else
                this.lblFullUnitInfo.Text = "Qty 0 UOM @ 0 KG/LB";
#endif
            }

            this.DisplayDataConvertedUnitInfo();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private void BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            BarCodeParse parse = null;
            SActItemSchemeData tempItemProdSchemeData = new SActItemSchemeData();

            try
            {
                this.ScannerDisable();

                parse = scanArgs.ParseInfo;

                // GLOBAL LOCATION IDENTIFIER SCANNED
                if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                {
                    this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(
                        LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier));

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
#if MULTILANGUAGE
            this.cmdLocation.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SetLocation);
#else
                        this.cmdLocation.Text = "Set Location";
#endif
                    }
                }
                else
                {
                    // Activity item fk refno
                    if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                    {
#if MULTILANGUAGE
            throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.ScanningALineItemBarCodeIsNotSupportedHere));
#else
                        throw new MessageOnlyException("Scanning a line item bar code is not supported here!");
#endif
                    }

                    // Activity detail fk refno
                    if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                    {
#if MULTILANGUAGE
            throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.ScanningAnLineItemDetailBarCodeIsNotSupportedHere));
#else
                        throw new MessageOnlyException("Scanning an line item detail bar code is not supported here!");
#endif
                    }

                    // Shipping container scanned, skid
                    if (!string.IsNullOrEmpty(parse.SSCC18))
                    {
#if MULTILANGUAGE
            throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.ScanningAShippingContainerBarCodeIsNotSupportedHere));
#else
                        throw new MessageOnlyException("Scanning a shipping container bar code is not supported here!");
#endif
                    }

                    // Product identifier
                    if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                    {
                        if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno) &&
                            !this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                        {
#if MULTILANGUAGE
              throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.LocationMustBeSetFirstz));
#else
                            throw new MessageOnlyException("Location Must Be Set First");
#endif
                        }

                        this.ClearContents();

                        int tempFromProdUnitRefno = this.FromProdUnitRefno;

                        SysShared.IdentifyProductScanned(
                            this,
                            this.ActSubType,
                            parse,
                            ref tempFromProdUnitRefno);

                        this.FromProdUnitRefno = tempFromProdUnitRefno;

                        this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(
                            this.FromProdUnitRefno);

                        this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(
                            this.ActItem.ProdInfo.Refno);

                        // Insert a new activity detail record
                        this.ActDetail.ActItem = this.ActItem;

                        bool isInsertReady = false;

                        this.ActDetail.Quantity = 0;
                        this.ActDetail.BaseQuantity = 0;
                        this.ActDetail.WeightQuantity = 0;
                        this.ActDetail.SchemeID = string.Empty;
                        this.ActDetail.UniqueSchemeID = string.Empty;
                        this.ActDetail.ActExtra.Text1 = string.Empty;
                        this.ActDetail.ActExtra.Text2 = string.Empty;
                        this.ActDetail.ActExtra.Text3 = string.Empty;
                        this.ActDetail.ActExtra.Text4 = string.Empty;
                        this.ActDetail.ActExtra.Text5 = string.Empty;

                        SysShared.GatherProductData(
                            parse,
                            this.ActSubType,
                            this.ActItem,
                            tempItemProdSchemeData,
                            this.CallingControl,
                            ref isInsertReady,
                            this.ActDetail,
                            false);

                        XDocument xmlDoc = this.ActDetail.GetXML(this.ActDetail);

                        this.ActItem.Insert(xmlDoc, false);

                        this.DisplayData();

                        if (this.FromProdUnitRefno > 0)
                        {
                            using (SqlCeCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                            {
                                cmndProdBCUnit.CommandType = CommandType.Text;
                                cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode," +
                                    " tProductUnit.ProductUnitBC," +
                                    " tUnit.UnitCode" +
                                    " FROM tProductUnit" +
                                    " INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno" +
                                    " INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno" +
                                    " WHERE tProductUnit.ProductUnitRefno = " + this.FromProdUnitRefno.ToString();

                                using (SqlCeDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                                {
                                    if (drProdBCUnit.Read())
                                    {
                                        cmdSelectFromUnit.Text = (string)drProdBCUnit["ProductCode"]
                                            + " " + (string)drProdBCUnit["ProductUnitBC"] + " "
                                            + (string)drProdBCUnit["UnitCode"];
                                    }

                                    drProdBCUnit.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errorReport.ShowDialog();
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
        private void DisplayDataConvertedUnitInfo()
        {
            if (this.ActDetail.DataViewDetails.Count > 0)
            {
                decimal qty = 0;
                decimal wtQty = 0;
                for (int item = 0; item <= this.ActDetail.DataViewDetails.Count - 1; item++)
                {
                    qty = qty + (decimal)this.ActDetail.DataViewDetails[item]["ActivityDetailQuantity"];

                    wtQty = wtQty + (decimal)this.ActDetail.DataViewDetails[item]["ActivityDetailWeightQuantity"];
                }

                ProductInfo toProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);
                this.lblConvertedUnitInfo.Text = string.Format(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.Qty),
#else
"Qty {0} {1} @ {2} {3}",
#endif
 Math.Round(qty, 3),
                    toProdInfo.UnitCode,
                    Math.Round(wtQty, 3),
                    toProdInfo.WeightUnitCode);
            }
            else
            {
#if MULTILANGUAGE
        this.lblConvertedUnitInfo.Text = Translator.GetInstance().Translate(TranslatorVocabulary.ConvertedQtyUOMKGLB);
#else
                this.lblConvertedUnitInfo.Text = "Converted Qty 0 UOM @ 0 KG/LB";
#endif
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

                if (sender == this.updnBarCodeModifier1)
                {
                }

                if (sender == this.dgUnitWeight)
                {
                }

                if (sender == this.txtGridEdit)
                {
                    DataGridCell cell = dgUnitWeight.CurrentCell;
                    this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber] = this.txtGridEdit.Text;
                    if (this.ActItem.ProdInfo.BaseUnitCode == "LB" || this.ActItem.ProdInfo.BaseUnitCode == "KG")
                    {
                        this.ActDetail.DataViewDetails[cell.RowNumber]["ActivityDetailBaseQuantity"]
                            = (decimal)this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber];
                    }

                    if (this.ActItem.ProdInfo.UnitCode == "LB" || this.ActItem.ProdInfo.UnitCode == "KG")
                    {
                        this.ActDetail.DataViewDetails[cell.RowNumber]["ActivityDetailQuantity"]
                            = (decimal)this.dgUnitWeight[cell.RowNumber, cell.ColumnNumber];
                    }

                    this.DisplayDataConvertedUnitInfo();
                    this.txtGridEdit.Visible = false;
                    this.txtGridEdit.Enabled = false;
                }
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = true;
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                e.Cancel = false;
                using (FormErrorReport errRpt = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errRpt.ShowDialog();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGridEdit_GotFocus(object sender, EventArgs e)
        {
            this.txtGridEdit.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            this.IsNumericTrapped = false;
            this.IsEscapeTrapped = false;
            this.IsEditTrapped = false;

            if (e.KeyCode == Keys.Enter || e.KeyValue == 134)
            {
                if (this.SaveChanges())
                {
                    if (this.ReqCloseEvent != null)
                    {
                        this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.CancelChanges();
            }
            else if (e.KeyCode == Keys.F1)
            {
                if (this.SaveChanges())
                {
                    if (this.ReqCloseEvent != null)
                    {
                        this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumericKeyUpHandler(object sender, KeyEventArgs e)
        {
            this.IsNumericTrapped = false;
            this.IsEscapeTrapped = false;
            this.IsEditTrapped = false;

            const int INTERMEC_CK60_KEYCODE_DECIMAL = 190;
            const int LXE_MX7II_KEYCODE_SUBTRACT = 189;

            if (e.KeyCode == Keys.Insert)
            {
                this.IsEditTrapped = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                this.IsEditTrapped = true;
            }
            else if (e.KeyCode == Keys.Subtract || Convert.ToInt32(e.KeyCode) == LXE_MX7II_KEYCODE_SUBTRACT)
            {
                this.IsNumericTrapped = true;
            }
            else if (e.KeyCode == Keys.Decimal)
            {
                this.IsNumericTrapped = true;
            }
            else if (Convert.ToInt32(e.KeyCode) == INTERMEC_CK60_KEYCODE_DECIMAL)
            {
                this.IsNumericTrapped = true;
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                this.IsNumericTrapped = true;
            }
            else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.F1)
            {
                this.IsEscapeTrapped = true;

                if (this.SaveChanges())
                {
                    if (this.ReqCloseEvent != null)
                    {
                        this.ReqCloseEvent((IAppPublisher)this, new AppEventArgs());
                    }
                }
                else
                {
                    this.CancelChanges();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void ClearContents()
        {
            this.FromProdUnitRefno = 0;
            this.ToProdUnitRefno = 0;
            this.ActItem.Delete();
            this.ActDetail.Delete();
#if MULTILANGUAGE
      this.cmdSelectFromUnit.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectFromUnit);
      this.cmdSelectToUnit.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SelectToUnit);
      this.lblFullUnitInfo.Text = Translator.GetInstance().Translate(TranslatorVocabulary.QtyUOMKGLBz);
      this.lblConvertedUnitInfo.Text = Translator.GetInstance().Translate(TranslatorVocabulary.QtyUOMKGLBz);
#else
            this.cmdSelectFromUnit.Text = "Select From Unit";
            this.cmdSelectToUnit.Text = "Select To Unit";
            this.lblFullUnitInfo.Text = "Qty 0 UOM @ KG/LB";
            this.lblConvertedUnitInfo.Text = "Qty 0 UOM @ KG/LB";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private string MakeProductLabelsSubmitString(int idx)
        {
            PrinterSettings prnSettings = new PrinterSettings().LoadData();
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)detail[idx]["ProductUnitRefno"]);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(
                    detail[idx]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT PRINT PRODUCT LABEL</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
            req.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
            req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
            req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNumber"]);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
            req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
            req.AppendFormat("<{0}>{1}</{0}>", "mobile_printer_name", (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty));
            req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string MakeConvertUnitSubmitString()
        {
            string production = string.Empty;
            string bestBefore = string.Empty;
            ProductInfo prodInfo = new ProductInfo();
            DataView detail = this.ActDetail.DataViewDetails;
            DataView items = this.ActDetail.ActItem.DataViewItems;

            prodInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)items[0]["ProductUnitRefno"]);
            prodInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(prodInfo.Refno);

            if (prodInfo.Scheme.IsBestBeforeDateTracked)
            {
                bestBefore = Convert.ToDateTime(
                    items[0]["ActivityDetailLogisticUnitBestBeforeSellByDate"]).ToString("MM/dd/yyyy");
            }

            if (prodInfo.Scheme.IsProductionDateTracked)
            {
                production = Convert.ToDateTime(
                    items[0]["ActivityDetailLogisticUnitProductionDate"]).ToString("MM/dd/yyyy");
            }

            StringBuilder req = new StringBuilder();
            req.Append("<?xml version=\'1.0\' encoding=\'utf-16\' standalone=\'yes\'?>");
            req.Append("<imclient>");
            req.Append("<command><action>SUBMIT CONVERT UNIT</action></command>");
            req.Append(SysCommon.GetRequestDefaults(false));
            req.Append("<from>");
            req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
            req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
            req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
            req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
            req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
            req.AppendFormat("<prod_id>{0}</prod_id>", items[0]["ProductCode"]);
            req.AppendFormat("<bc>{0}</bc>", items[0]["ProductUnitBC"]);
            req.AppendFormat("<alt_unit>{0}</alt_unit>", items[0]["UnitCode"]);
            req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
            req.AppendFormat("<serial>{0}</serial>", items[0]["ActivityDetailSingleTradeItemSerialNumber"]);
            req.AppendFormat("<pdate>{0}</pdate>", production);
            req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
            req.AppendFormat("<lot_no>{0}</lot_no>", items[0]["ActivityDetailLogisticUnitBatchLotNumber"]);
            req.AppendFormat("<variant_code>{0}</variant_code>", items[0]["ProductSchemeVariantCode"]);
            req.AppendFormat("<variant_no>{0}</variant_no>", items[0]["ProductSchemeVariantNumber"]);
            req.AppendFormat("<alt_qty>{0}</alt_qty>", items[0]["ActivityDetailQuantity"]);
            req.AppendFormat("<wt_qty>{0}</wt_qty>", items[0]["ActivityDetailWeightQuantity"]);
            req.AppendFormat("<base_qty>{0}</base_qty>", items[0]["ActivityDetailBaseQuantity"]);
            req.AppendFormat("<base_unit>{0}</base_unit>", prodInfo.BaseUnitCode);
            req.Append("</from>");

            for (int
                idx = 0; idx <= this.ActDetail.DataViewDetails.Count - 1; idx++)
            {
                ProductInfo toProdInfo = new ProductInfo();
                toProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)detail[idx]["ProductUnitRefno"]);

                req.Append("<to>");
                req.AppendFormat("<whs_id>{0}</whs_id>", this.LocMap.CurrLocMap.WhsFKRefno);
                req.AppendFormat("<sect_refno>{0}</sect_refno>", this.LocMap.CurrLocMap.SectFKRefno);
                req.AppendFormat("<slot_refno>{0}</slot_refno>", this.LocMap.CurrLocMap.SlotFKRefno);
                req.AppendFormat("<refno>{0}</refno>", this.ActItem.ActHdr.FKRefno);
                req.AppendFormat("<itm_refno>{0}</itm_refno>", this.ActItem.FKRefno);
                req.AppendFormat("<prod_id>{0}</prod_id>", detail[idx]["ProductCode"]);
                req.AppendFormat("<bc>{0}</bc>", detail[idx]["ProductUnitBC"]);
                req.AppendFormat("<alt_unit>{0}</alt_unit>", detail[idx]["UnitCode"]);
                req.AppendFormat("<wt_unit>{0}</wt_unit>", prodInfo.WeightUnitCode);
                req.AppendFormat("<serial>{0}</serial>", detail[idx]["ActivityDetailSingleTradeItemSerialNumber"]);
                req.AppendFormat("<pdate>{0}</pdate>", production);
                req.AppendFormat("<bdate>{0}</bdate>", bestBefore);
                req.AppendFormat("<lot_no>{0}</lot_no>", detail[idx]["ActivityDetailLogisticUnitBatchLotNumber"]);
                req.AppendFormat("<variant_code>{0}</variant_code>", detail[idx]["ProductSchemeVariantCode"]);
                req.AppendFormat("<variant_no>{0}</variant_no>", detail[idx]["ProductSchemeVariantNumber"]);
                req.AppendFormat("<alt_qty>{0}</alt_qty>", detail[idx]["ActivityDetailQuantity"]);
                req.AppendFormat("<wt_qty>{0}</wt_qty>", detail[idx]["ActivityDetailWeightQuantity"]);
                req.AppendFormat("<base_qty>{0}</base_qty>", detail[idx]["ActivityDetailBaseQuantity"]);
                req.AppendFormat("<base_unit>{0}</base_unit>", toProdInfo.BaseUnitCode);
                req.Append("</to>");
            }

            req.Append("</imclient>");

            return req.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void cmdLocation_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();
            using (FormDialogContainer form = new FormDialogContainer(
                FormDialogContainer.EDialogTypes.ucSelectLocationMap,
                SysCommon.ActSubTypes.NotApplicable,
                this))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
#if MULTILANGUAGE
            this.cmdLocation.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SetLocation);
#else
                        this.cmdLocation.Text = "Set Location";
#endif
                    }
                }
            }

            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void cmdSelectFromUnit_Click(object sender, EventArgs e)
        {
            this.ScannerDisable();

            if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
            {
                MessageBox.Show(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.YourLocationMustBeSet),
#else
"Your location must be set.",
#endif
 this.GetControlDescription(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                using (FormDialogContainer frmSelectProduct = new FormDialogContainer(
                    FormDialogContainer.EDialogTypes.ucSelectProductUnit,
                    SysCommon.ActSubTypes.ConvertUnit,
                    this))
                {
                    if (frmSelectProduct.ShowDialog() == DialogResult.OK)
                    {
                        this.ClearContents();
                        XDocument xmlDocProduct = frmSelectProduct.DisplayControlData;
                        this.FromProdUnitRefno = int.Parse(
                            xmlDocProduct.Root.Element("ProductUnitRefno").Value);
                        if (this.FromProdUnitRefno != 0)
                        {
                            this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);
                            this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(
                                this.ActItem.ProdInfo.Refno);
                            this.ActItem.LocMap = this.LocMap;

                            // Pass 0 as the current selected row on an add, true forces it as an add not an edit
                            this.ActDetail = this.ActDetail.GetInfo(0, true);

                            try
                            {
                                this.ActDetail.Quantity = 0;
                                this.ActDetail.BaseQuantity = 0;
                                this.ActDetail.WeightQuantity = 0;
                                this.ActDetail.SchemeID = string.Empty;
                                this.ActDetail.UniqueSchemeID = string.Empty;
                                this.ActDetail.ActExtra.Text1 = string.Empty;
                                this.ActDetail.ActExtra.Text2 = string.Empty;
                                this.ActDetail.ActExtra.Text3 = string.Empty;
                                this.ActDetail.ActExtra.Text4 = string.Empty;
                                this.ActDetail.ActExtra.Text5 = string.Empty;

                                using (FormDialogContainer form = new FormDialogContainer(
                                    FormDialogContainer.EDialogTypes.ucActivityDetailEdit,
                                    this.ActDetail,
                                    SysCommon.ActSubTypes.ConvertUnit,
                                    this))
                                {
                                    DialogResult dialogResult = form.ShowDialog();
                                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                                    {
                                        XDocument xmlDoc = form.DisplayControlData;
                                        this.ActItem.Insert(xmlDoc, false);

                                        using (SqlCeCommand cmndProdBCUnit = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                                        {
                                            cmndProdBCUnit.CommandType = CommandType.Text;
                                            cmndProdBCUnit.CommandText = "SELECT tProduct.ProductCode,"
                                                + "tProductUnit.ProductUnitBC,"
                                                + "tUnit.UnitCode "
                                                + "FROM tProductUnit "
                                                + "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno "
                                                + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno "
                                                + "WHERE tProductUnit.ProductUnitRefno = " + this.FromProdUnitRefno;

                                            using (SqlCeDataReader drProdBCUnit = cmndProdBCUnit.ExecuteReader())
                                            {
                                                if (drProdBCUnit.Read())
                                                {
                                                    this.cmdSelectFromUnit.Text =
                                                        drProdBCUnit["ProductCode"].ToString().TrimEnd()
                                                        + (" " + drProdBCUnit["ProductUnitBC"].ToString()).TrimEnd()
                                                        + " " + drProdBCUnit["UnitCode"].ToString();

                                                    this.DisplayData();
                                                }

                                                drProdBCUnit.Close();
                                            }
                                        }
                                    }
                                }
                            }
                            catch (SqlCeException ex)
                            {
                                using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                                {
                                    errorReport.ShowDialog();
                                }
                            }
                            catch (MessageOnlyException ex)
                            {
                                ex.Show(this, this.ActSubType);
                            }
                            catch (Exception ex)
                            {
                                using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                                {
                                    errorReport.ShowDialog();
                                }
                            }
                        }
                    }
                }

                this.cmdSelectToUnit.Refresh();
            }

            this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void cmdSelectToUnit_Click(object sender, EventArgs e)
        {
            try
            {
                // Needed to check if the enter key was pressed last, because it seems that 
                // SelectNextControl causes a click event to fire if it is the next control
                this.ScannerDisable();

                if (!LocationMapTasks.IsCompleteLocMapRefno(this.LocMap.CurrLocMapRefno))
                {
                    MessageBox.Show(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.YourLocationMustBeSet),
#else
"Your location must be set.",
#endif
 this.GetControlDescription(),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Hand,
                        MessageBoxDefaultButton.Button1);
                }
                else if (this.FromProdUnitRefno == 0 || this.ActItem.DataViewItems.Count <= 0)
                {
                    MessageBox.Show(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.PleaseSetTheFromProductUnitFirst),
#else
"Please set the from product unit first.",
#endif
 this.GetControlDescription(),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Hand,
                        MessageBoxDefaultButton.Button1);
                }
                else
                {
                    this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.FromProdUnitRefno);

                    this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(
                        this.ActItem.ProdInfo.Refno);

                    decimal decFromBaseFactor = ProductInfoTasks.GetUnitRelationFactor((ProductInfo)this.ActItem.ProdInfo);

                    using (FormDialogContainer frmSelectProduct = new FormDialogContainer(
                        FormDialogContainer.EDialogTypes.ucSelectProductUnitFilterToProduct,
                        this.ActItem,
                        SysCommon.ActSubTypes.ConvertUnit,
                        this))
                    {
                        if (frmSelectProduct.ShowDialog() == DialogResult.OK)
                        {
                            XDocument xmlDocProduct = frmSelectProduct.DisplayControlData;

                            this.ToProdUnitRefno = int.Parse(xmlDocProduct.Root.Element("ProductUnitRefno").Value);

                            if (this.ToProdUnitRefno != 0)
                            {
                                this.ActItem.LocMap = this.LocMap;

                                this.ActItem.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(this.ToProdUnitRefno);

                                this.ActItem.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(
                                    this.ActItem.ProdInfo.Refno);

                                SysCommon.SFillQuantity fillQty = this.RequestConvertUnitResult(
                                    this.FromProdUnitRefno,
                                    this.ToProdUnitRefno,
                                    (decimal)this.ActItem.DataViewItems[0]["ActivityDetailQuantity"],
                                    (decimal)this.ActItem.DataViewItems[0]["ActivityDetailWeightQuantity"]);

                                this.ActDetail.Quantity = fillQty.Quantity;
                                this.ActDetail.BaseQuantity = fillQty.BaseQuantity;
                                this.ActDetail.WeightQuantity = fillQty.WeightQuantity;

                                this.ActDetail.SchemeID = (string)this.ActItem.DataViewItems[0]["ActivityDetailSchemeID"];
                                this.ActDetail.UniqueSchemeID = (string)this.ActItem.DataViewItems[0]["ActivityDetailUniqueSchemeID"];

                                SProductSchemeData prodSchemeData = new SProductSchemeData();
                                prodSchemeData.LogisticUnitBestBeforeSellByDate
                                    = (DateTime)this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitBestBeforeSellByDate"];
                                prodSchemeData.LogisticUnitProductionDate
                                    = (DateTime)this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitProductionDate"];
                                prodSchemeData.VariantNumber = (int)this.ActItem.DataViewItems[0]["ProductSchemeVariantNumber"];
                                prodSchemeData.LogisticUnitBatchLotNumber
                                    = (string)this.ActItem.DataViewItems[0]["ActivityDetailLogisticUnitBatchLotNumber"];
                                prodSchemeData.LogisticUnitSkidNumber = string.Empty;
                                prodSchemeData.SingleTradeItemSerialNumber
                                    = (string)this.ActItem.DataViewItems[0]["ActivityDetailSingleTradeItemSerialNumber"];
                                prodSchemeData.VariantCode
                                    = (string)this.ActItem.DataViewItems[0]["ProductSchemeVariantCode"];

                                this.ActDetail.SchemeData = prodSchemeData;

                                try
                                {
                                    using (FormDialogContainer form = new FormDialogContainer(
                                        FormDialogContainer.EDialogTypes.ucActivityConvertUnitEdit,
                                        this.ActDetail,
                                        SysCommon.ActSubTypes.ConvertUnit,
                                        this))
                                    {
                                        DialogResult result = form.ShowDialog();
                                        if (result == System.Windows.Forms.DialogResult.OK)
                                        {
                                            XDocument xmlDoc = form.DisplayControlData;
                                            this.cmdSelectToUnit.Text = string.Format(
#if MULTILANGUAGE
                          Translator.GetInstance().Translate(TranslatorVocabulary.SPer),
#else
"{0} {1}\'S ({2} per {3})",
#endif
 this.ActItem.ProdInfo.UnitBC,
                                                this.ActItem.ProdInfo.UnitCode,
                                                Math.Round(this.ActDetail.Quantity, 3),
                                                (string)this.ActItem.DataViewItems[0]["UnitCode"]);

                                            this.ActDetail.Delete();
                                            this.ActDetail.Quantity = decimal.Parse(xmlDoc.Root.Element("alt_qty").Value);
                                            this.ActDetail.BaseQuantity = decimal.Parse(xmlDoc.Root.Element("base_qty").Value);
                                            this.ActDetail.WeightQuantity = decimal.Parse(xmlDoc.Root.Element("wt_qty").Value);
                                            if (bool.Parse(xmlDoc.Root.Element("split").Value))
                                            {
                                                if (this.ActItem.ProdInfo.UnitCode == "LB" ||
                                                    this.ActItem.ProdInfo.UnitCode == "KG")
                                                {
                                                    this.ActDetail.Insert(xmlDoc);
                                                }
                                                else
                                                {
                                                    decimal altQty = this.ActDetail.Quantity;
                                                    decimal baseQty = this.ActDetail.BaseQuantity;
                                                    decimal wt = this.ActDetail.WeightQuantity;
                                                    decimal wtQty = altQty == 0 ? wt : wt / altQty;
                                                    for (int unitQty = 1; unitQty <= altQty; unitQty++)
                                                    {
                                                        this.ActDetail.Quantity = 1;
                                                        this.ActDetail.WeightQuantity = wtQty;
                                                        if (this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "LB"
                                                            || this.ActDetail.ActItem.ProdInfo.BaseUnitCode == "KG")
                                                        {
                                                            this.ActDetail.BaseQuantity = this.ActDetail.WeightQuantity;
                                                        }
                                                        else
                                                        {
                                                            if (this.ActDetail.Quantity != 0
                                                                && this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor != 0)
                                                            {
                                                                this.ActDetail.BaseQuantity =
                                                                    this.ActDetail.Quantity * this.ActDetail.ActItem.ProdInfo.Unit2BaseUnitFactor;
                                                            }
                                                            else
                                                            {
                                                                this.ActDetail.BaseQuantity = this.ActDetail.Quantity;
                                                            }
                                                        }

                                                        this.ActDetail.Insert(xmlDoc);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                this.ActDetail.Insert(xmlDoc);
                                            }
                                        }
                                    }

                                    this.DisplayData();
                                }
                                catch (SqlCeException ex)
                                {
                                    using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                                    {
                                        errorReport.ShowDialog();
                                    }
                                }
                                catch (MessageOnlyException ex)
                                {
                                    ex.Show(this, this.ActSubType);
                                }
                                catch (Exception ex)
                                {
                                    using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                                    {
                                        errorReport.ShowDialog();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                ex.Show(this, this.ActSubType);
            }
            catch (Exception ex)
            {
                using (FormErrorReport errorReport = new FormErrorReport(this.ActSubType, this, ex))
                {
                    errorReport.ShowDialog();
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
        /// <param name="fromProdUnitRefno"></param>
        /// <param name="toProdUnitRefno"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private SysCommon.SFillQuantity RequestConvertUnitResult(
            int fromProdUnitRefno,
            int toProdUnitRefno,
            decimal quantity,
            decimal weightQuantity)
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();
            fillQty.Quantity = quantity;

            string request = this.CreateConvertUnitResultXML(
                fromProdUnitRefno,
                toProdUnitRefno,
                quantity,
                weightQuantity);
            string dataReq = SysCommon.SubmitComm(request);
            if (dataReq.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(dataReq);
                if (dataReq.Contains("<message>ERROR</message>"))
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
                    fillQty.WeightQuantity = Convert.ToDecimal(
                        xmlDoc.SelectSingleNode("/VFPData/WeightQuantity").FirstChild.Value);
                }
            }

            return fillQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string CreateConvertUnitResultXML(
            int fromProdUnitRefno,
            int toProdUnitRefno,
            decimal quantity,
            decimal weightQuantity)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            ProductInfo fromProd = ProductInfoTasks.GetInfoFromUnitRefno(fromProdUnitRefno);
            ProductInfo toProd = ProductInfoTasks.GetInfoFromUnitRefno(toProdUnitRefno);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST CONVERT UNIT RESULT</action>"
                + "</command>"
                + SysCommon.GetRequestDefaults(false)
                + ("<prod_id>" + fromProd.Code + "</prod_id>")
                + ("<from_bc>" + fromProd.UnitBC + "</from_bc>")
                + ("<from_alt_qty>" + quantity + "</from_alt_qty>")
                + ("<from_alt_unit>" + fromProd.UnitCode + "</from_alt_unit>")
                + ("<from_wt_qty>" + weightQuantity + "</from_wt_qty>")
                + ("<to_bc>" + toProd.UnitBC + "</to_bc>")
                + ("<to_alt_unit>" + toProd.UnitCode + "</to_alt_unit>")
                + ("<to_base_unit>" + toProd.BaseUnitCode + "</to_base_unit>")
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
        private void NumericKeyPressHandler(object sender, KeyPressEventArgs e)
        {
            if (this.IsEditTrapped)
            {
                e.Handled = false;
                this.IsEditTrapped = false;
            }
            else if (this.IsNumericTrapped)
            {
                e.Handled = false;
                this.IsNumericTrapped = false;
            }
            else if (this.IsEscapeTrapped)
            {
                e.Handled = false;
                this.IsEscapeTrapped = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void updnBarCodeModifier1_GotFocus(object sender, EventArgs e)
        {
            SysCommon.SelectValueInDomainUpDown((UpDownBase)sender);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ucActivityConvertUnit_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.ScannerEnable();

                if (!this.IsControlLoaded)
                {
                    this.updnBarCodeModifier1.Text = this.SysEnv.CurrentBarCodeModifier1;

                    if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                    {
                        this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation();
                    }
                    else
                    {
#if MULTILANGUAGE
            this.cmdLocation.Text = Translator.GetInstance().Translate(TranslatorVocabulary.SetLocation);
#else
                        this.cmdLocation.Text = "Set Location";
#endif
                    }

                    this.IsControlLoaded = true;
                }

                if (this.ReqCancelEnableEvent != null)
                {
                    this.ReqCancelEnableEvent((IAppPublisher)this, new AppEventArgs());
                }

                if (this.ReqSaveEnableEvent != null)
                {
                    this.ReqSaveEnableEvent((IAppPublisher)this, new AppEventArgs());
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void unitWeight_DoubleClick(object sender, EventArgs e)
        {
            DataGridCell currCell = dgUnitWeight.CurrentCell;

            if (this.dgUnitWeight.TableStyles["tActivityDetail"].GridColumnStyles[currCell.ColumnNumber].MappingName
                == "ActivityDetailWeightQuantity")
            {
                this.txtGridEdit.Bounds = this.dgUnitWeight.GetCellBounds(currCell.RowNumber, currCell.ColumnNumber);
                this.txtGridEdit.Text = this.dgUnitWeight[currCell.RowNumber, currCell.ColumnNumber].ToString();
                this.txtGridEdit.SelectAll();
                this.txtGridEdit.Visible = true;
                this.txtGridEdit.Enabled = true;
                this.txtGridEdit.Focus();
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

                if (sender == this.updnBarCodeModifier1)
                {
                    this.SysEnv.CurrentBarCodeModifier1 = this.updnBarCodeModifier1.Text;
                }

                if (sender == this.txtGridEdit)
                {
                }
            }
        }

        #endregion
    }
}
