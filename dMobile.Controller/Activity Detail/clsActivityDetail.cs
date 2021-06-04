namespace dMobile.Controller
{
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using SQLite;
    using System;
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using System.Linq;
    using System.Dynamic;

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public abstract class ActDetail : IDataLayerEventPublisher, IDataAccessEventListener
    {
        #region Fields

        private string mainGridMappingName = "tActivityDetail";
        private List<object> listDetails = null;
        private SysCommon.ActSubTypes actSubType;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private SProductSchemeData schemeData = new SProductSchemeData();
        private string uniqueSchemeID = string.Empty;
        private string schemeID = string.Empty;
        private decimal quantity = 0;
        private decimal baseQuantity = 0;
        private decimal weightQuantity = 0;
        private ActItem actItem = null;
        private int fkRefno = Convert.ToInt32(null);
        private int refno = Convert.ToInt32(null);
        private ActDetailExtra actExtra = new ActDetailExtra();
        private DataLayerEventArgs dataEventArgs = new DataLayerEventArgs();
        private SLocMap selectedLocMap = new SLocMap();
        private bool isUpdateView = false;
        private int newItemFKRefno = 0;
        private string fullClassName = typeof(ActDetail).FullName;
        private int newFKRefno = Convert.ToInt32(null);

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityItem"></param>
        public ActDetail(ActItem activityItem)
        {
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.ActItem = activityItem;
            this.ActSubType = this.ActItem.ActSubType;
        }

        #endregion

        #region Delegates

        public delegate void OnDataLayerStatusOutEventHandler(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs);
        public delegate void OnDataLayerProgressOutEventHandler(IDataLayerEventPublisher dataLayerEventPublisher, DataLayerEventArgs dataLayerEventArgs);

        #endregion

        #region Events

        public event OnDataLayerStatusOutEventHandler OnDataLayerStatusOut;
        public event OnDataLayerProgressOutEventHandler OnDataLayerProgressOut;

        #endregion

        #region Properties

        public int NewFKRefno
        {
            get
            {
                return this.newFKRefno;
            }

            set
            {
                this.newFKRefno = value;
            }
        }

        public string FullClassName
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

        public int NewItemFKRefno
        {
            get
            {
                return this.newItemFKRefno;
            }

            set
            {
                this.newItemFKRefno = value;
            }
        }

        public bool IsUpdateView
        {
            get
            {
                return this.isUpdateView;
            }

            set
            {
                this.isUpdateView = value;
            }
        }

        public SLocMap SelectedLocMap
        {
            get
            {
                return this.selectedLocMap;
            }

            set
            {
                this.selectedLocMap = value;
            }
        }

        public DataLayerEventArgs DataEventArgs
        {
            get
            {
                return this.dataEventArgs;
            }

            set
            {
                this.dataEventArgs = value;
            }
        }

        public ActDetailExtra ActExtra
        {
            get
            {
                return this.actExtra;
            }

            set
            {
                this.actExtra = value;
            }
        }

        public int Refno
        {
            get
            {
                return this.refno;
            }

            set
            {
                this.refno = value;
            }
        }

        public int FKRefno
        {
            get
            {
                return this.fkRefno;
            }

            set
            {
                this.fkRefno = value;
            }
        }

        public SProductSchemeData SchemeData
        {
            get
            {
                return this.schemeData;
            }

            set
            {
                this.schemeData = value;
            }
        }

        public string UniqueSchemeID
        {
            get
            {
                return this.uniqueSchemeID;
            }

            set
            {
                this.uniqueSchemeID = value;
            }
        }

        public string SchemeID
        {
            get
            {
                return this.schemeID;
            }

            set
            {
                this.schemeID = value;
            }
        }

        public decimal Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
            }
        }

        public decimal BaseQuantity
        {
            get
            {
                return this.baseQuantity;
            }

            set
            {
                this.baseQuantity = value;
            }
        }

        public decimal WeightQuantity
        {
            get
            {
                return this.weightQuantity;
            }

            set
            {
                this.weightQuantity = value;
            }
        }

        public string MainGridMappingName
        {
            get
            {
                return this.mainGridMappingName;
            }

            set
            {
                this.mainGridMappingName = value;
            }
        }

        public List<object> ListDetails
        {
            get
            {
                return this.listDetails;
            }

            set
            {
                this.listDetails = value;
            }
        }

        public SysCommon.ActSubTypes ActSubType
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

        public SysEnvironment SysEnv
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

        public LocationMap LocMap
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

        public ActItem ActItem
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

        #endregion

        #region Public Members

        public abstract Task Insert(XDocument data);
        public abstract Task Delete();
        public abstract Task Refresh();
        public abstract Task<string> CreateSubmitXML(XDocument data);
        public abstract Task<string> CreateDeleteXML(XDocument data);
        public abstract Task Submit(object selectedActivityDetail, XDocument data);
        public abstract ObservableCollection<object> Search(string searchText);
        public abstract Task<List<object>> GetActivityDetailList(bool isSubmitBatch);
        public abstract ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity);
        public abstract Task SelectActivityDetail(object selectedActivityDetail);
        public abstract int GetRefno(object selectedActivityDetail);
        public abstract int GetFKRefno(object selectedActivityDetail);
        public abstract Task InsertSkid(SysSkidInfo skidInfo);
        public abstract Task DeleteSkid(SysSkidInfo skidInfo);

        /// <summary>
        /// Total all activity detail quantity from the dataview
        /// </summary>
        /// <returns>SFillQuantity structure</returns>
        public virtual async Task<SysCommon.SFillQuantity> GetDataViewTotalQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();

            if (this.ListDetails != null && this.ListDetails.Count > 0)
            {
                foreach (dynamic detail in this.ListDetails)
                {
                    fillQty.Quantity += Convert.ToDecimal(detail.ActivityDetailQuantity);
                    fillQty.BaseQuantity += Convert.ToDecimal(detail.ActivityDetailBaseQuantity);
                    fillQty.WeightQuantity += Convert.ToDecimal(detail.ActivityDetailWeightQuantity);
                }
            }

            return fillQty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearProperties()
        {
            this.UniqueSchemeID = string.Empty;
            this.SchemeID = string.Empty;
            this.Quantity = 0;
            this.BaseQuantity = 0;
            this.WeightQuantity = 0;
            this.FKRefno = 0;
            this.Refno = 0;
            this.ActExtra.Text1 = string.Empty;
            this.ActExtra.Text2 = string.Empty;
            this.ActExtra.Text3 = string.Empty;
            this.ActExtra.Text4 = string.Empty;
            this.ActExtra.DateTime5 = new DateTime(1900, 1, 1);
            this.NewItemFKRefno = 0;
            this.NewFKRefno = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public virtual async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            SProductSchemeData tmpSchemeData = this.SchemeData;

            if (isAdd)
            {
                this.ClearProperties();

                tmpSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                tmpSchemeData.LogisticUnitProductionDate = DateTime.Now;
                tmpSchemeData.SingleTradeItemSerialNumber = string.Empty;
                tmpSchemeData.LogisticUnitBatchLotNumber = string.Empty;
                tmpSchemeData.VariantCode = string.Empty;
                tmpSchemeData.VariantNumber = 0;
            }
            else
            {
                if (selectedActivityDetail == null)
                {
                    throw new Exception("Detail is null!");
                }

                dynamic detail = selectedActivityDetail;

                this.Quantity = Convert.ToDecimal(detail.ActivityDetailQuantity);
                this.BaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity);
                this.WeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity);
                this.Refno = Convert.ToInt32(detail.ActivityDetailRefno);
                this.SchemeID = Convert.ToString(detail.ActivityDetailSchemeID);
                this.UniqueSchemeID = Convert.ToString(detail.ActivityDetailUniqueSchemeID);

                // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                if (detail.ActivityDetailExtra1Text != null)
                {
                    this.ActExtra.Text1 = Convert.ToString(detail.ActivityDetailExtra1Text);
                }

                // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                if (detail.ActivityDetailExtra2Text != null)
                {
                    this.ActExtra.Text2 = Convert.ToString(detail.ActivityDetailExtra2Text);
                }

                // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                if (detail.ActivityDetailExtra3Text != null)
                {
                    this.ActExtra.Text3 = Convert.ToString(detail.ActivityDetailExtra3Text);
                }

                // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                if (detail.ActivityDetailExtra4Text != null)
                {
                    this.ActExtra.Text4 = Convert.ToString(detail.ActivityDetailExtra4Text);
                }

                // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                if (detail.ActivityDetailExtra5Date != null && !string.IsNullOrEmpty(Convert.ToString(detail.ActivityDetailExtra5Date)))
                {
                    this.ActExtra.DateTime5 = Convert.ToDateTime(detail.ActivityDetailExtra5Date);
                }

                DateTime bestBefore = Convert.ToDateTime(detail.ActivityDetailLogisticUnitBestBeforeSellByDate);

                if (SysCommon.IsEmptyDateTime(bestBefore))
                {
                    if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTrackedDefault)
                    {
                        tmpSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                    }
                }
                else
                {
                    tmpSchemeData.LogisticUnitBestBeforeSellByDate = bestBefore;
                }

                DateTime production = Convert.ToDateTime(detail.ActivityDetailLogisticUnitProductionDate);

                if (SysCommon.IsEmptyDateTime(production))
                {
                    if (this.ActItem.ProdInfo.Scheme.IsProductionDateTrackedDefault)
                    {
                        tmpSchemeData.LogisticUnitProductionDate = DateTime.Now;
                    }
                }
                else
                {
                    tmpSchemeData.LogisticUnitProductionDate = production;
                }

                tmpSchemeData.SingleTradeItemSerialNumber = Convert.ToString(detail.ActivityDetailSingleTradeItemSerialNumber);
                tmpSchemeData.LogisticUnitBatchLotNumber = Convert.ToString(detail.ActivityDetailLogisticUnitBatchLotNumber);

                // Check for DBNull because variants are in a child table, left join from the dataview query.
                if (detail.ProductSchemeVariantCode != null)
                {
                    tmpSchemeData.VariantCode = Convert.ToString(detail.ProductSchemeVariantCode);
                }

                // Check for DBNull because variants are in a child table, left join from the dataview query.
                if (detail.ProductSchemeVariantNo != null)
                {
                    tmpSchemeData.VariantNumber = Convert.ToInt32(detail.ProductSchemeVariantNo);
                }
            }

            this.SchemeData = tmpSchemeData;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeID(this.UniqueSchemeID);

                    if (isDupSchemeId)
                    {
                        // TODO: Resource strings
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeID);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual Task<string> CreateSubmitProductLabelXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a string formatted xml of default detail values.
        /// </summary>
        /// <remarks>
        /// prod_id, is_bc
        /// whs_id, sect_refno, slot_refno
        /// refno, itm_refno, ser_refno
        /// alt_qty, alt_unit, base_qty, base_unit, wt_qty, wt_unit
        /// serial, lot_no, pdate, bdate, variant_no
        /// extra1, extra2, extra3, extra4, extra5
        /// is_disconnected
        /// </remarks>
        /// <returns></returns>
        public virtual string GetRequestDetailDefaults()
        {
            // Init the production and bestbefore date as empty intentional. 
            // We want to send a 'blank' date if it is not tracked rather than {01/01/1900} as the empty date.

            string bestBeforeDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked && !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                bestBeforeDate = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            string productionDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked && !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
            {
                productionDate = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
            }

            string def = ("<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>")
                + ("<is_bc>" + this.ActItem.ProdInfo.UnitIsBC + "</is_bc>")
                + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                + ("<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>")
                + ("<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>")
                + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                + ("<ser_refno>" + this.FKRefno + "</ser_refno>")
                + ("<alt_qty>" + this.Quantity + "</alt_qty>")
                + ("<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>")
                + ("<base_qty>" + this.BaseQuantity + "</base_qty>")
                + ("<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>")
                + ("<wt_qty>" + this.WeightQuantity + "</wt_qty>")
                + ("<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>")
                + ("<serial>" + this.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<lot_no>" + this.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<pdate>" + productionDate + "</pdate>")
                + ("<bdate>" + bestBeforeDate + "</bdate>")
                + ("<variant_no>" + this.SchemeData.VariantNumber + "</variant_no>")
                + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
                + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
                + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
                + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
                + ("<extra5>" + this.ActExtra.DateTime5 + "</extra5>")
                + ("<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>");

            return def;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetail"></param>
        /// <returns></returns>
        public XDocument GetXML(ActDetail actDetail)
        {
            return XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<refno></refno>"
                + "<ser_refno></ser_refno>"
                + ("<serial>" + actDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>")
                + ("<pdate>" + actDetail.SchemeData.LogisticUnitProductionDate + "</pdate>")
                + ("<lot_no>" + actDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>")
                + ("<bdate>" + actDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>")
                + ("<variant_code>" + actDetail.SchemeData.VariantCode + "</variant_code>")
                + ("<variant_no>" + actDetail.SchemeData.VariantNumber + "</variant_no>")
                + ("<alt_qty>" + actDetail.Quantity + "</alt_qty>")
                + ("<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>")
                + ("<base_qty>" + actDetail.BaseQuantity + "</base_qty>")
                + ("<skid_code>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_code>")
                + ("<is_batch_print_product_label>" + this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch
                + "</is_batch_print_product_label>")
                + ("<extra1>" + actDetail.ActExtra.Text1 + "</extra1>")
                + ("<extra2>" + actDetail.ActExtra.Text2 + "</extra2>")
                + ("<extra3>" + actDetail.ActExtra.Text3 + "</extra3>")
                + ("<extra4>" + actDetail.ActExtra.Text4 + "</extra4>")
                + ("<extra5>" + actDetail.ActExtra.DateTime5 + "</extra5>")
                + "</params>");
        }

        /// <summary>
        /// Check if the inventory tracking scheme is complete.
        /// </summary>
        /// <param name="actItem"></param>
        /// <returns></returns>
        public bool IsSchemeComplete(ActItem actItem)
        {
            bool isComplete = true;

            if (actItem.ProdInfo.Scheme.IsBestBeforeDateTracked && SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsLotTracked && string.IsNullOrEmpty(this.SchemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsProductionDateTracked && SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsSerialTracked && string.IsNullOrEmpty(this.SchemeData.SingleTradeItemSerialNumber))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsVariantTracked && this.SchemeData.VariantNumber == 0)
            {
                isComplete = false;
            }

            return isComplete;
        }

        /// <summary>
        /// Check if the product label scheme is complete.
        /// </summary>
        /// <returns></returns>
        public bool IsLabelSchemeComplete()
        {
            return this.IsLabelSchemeComplete(this.SchemeData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemeData"></param>
        /// <returns></returns>
        public bool IsLabelSchemeComplete(SProductSchemeData schemeData)
        {
            bool isComplete = true;

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked && SysCommon.IsEmptyDateTime(schemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsLotTracked && string.IsNullOrEmpty(schemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsProductionDateTracked && SysCommon.IsEmptyDateTime(schemeData.LogisticUnitProductionDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsSerialTracked && string.IsNullOrEmpty(schemeData.SingleTradeItemSerialNumber))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsVariantTracked && schemeData.VariantNumber == 0)
            {
                isComplete = false;
            }

            return isComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodSchemeData"></param>
        /// <returns></returns>
        public async Task<SProductSchemeData> FillSchemeDataVarInfoFromSchemeDataVarNo(SProductSchemeData prodSchemeData)
        {
            if (0 < prodSchemeData.VariantNumber)
            {

                List<tProductSchemeVariantModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", Convert.ToInt32(prodSchemeData.VariantNumber));

                if (results != null && results.Count > 0)
                {
                    prodSchemeData.VariantCode = results[0].ProductSchemeVariantCode;
                    prodSchemeData.VariantDescription = results[0].ProductSchemeVariantDescription;
                }
                else
                {
                    throw new Exception("FillSchemeDataVarInfoFromSchemeDataVarNo in clsActivityDetail returned no results");
                }
            }

            return prodSchemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public async Task FillActDetailObjFromXML(XDocument data)
        {
            if (!string.IsNullOrEmpty(data.Root.Element("ser_refno").Value))
            {
                this.FKRefno = int.Parse(data.Root.Element("ser_refno").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("refno").Value))
            {
                this.Refno = int.Parse(data.Root.Element("refno").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("alt_qty").Value))
            {
                this.Quantity = decimal.Parse(data.Root.Element("alt_qty").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("base_qty").Value))
            {
                this.BaseQuantity = decimal.Parse(data.Root.Element("base_qty").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("wt_qty").Value))
            {
                this.WeightQuantity = decimal.Parse(data.Root.Element("wt_qty").Value);
            }

            SProductSchemeData tmpSchemeData = this.SchemeData;

            if (!string.IsNullOrEmpty(data.Root.Element("bdate").Value))
            {
                tmpSchemeData.LogisticUnitBestBeforeSellByDate = Convert.ToDateTime(data.Root.Element("bdate").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("pdate").Value))
            {
                tmpSchemeData.LogisticUnitProductionDate = Convert.ToDateTime(data.Root.Element("pdate").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("serial").Value))
            {
                tmpSchemeData.SingleTradeItemSerialNumber = data.Root.Element("serial").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("lot_no").Value))
            {
                tmpSchemeData.LogisticUnitBatchLotNumber = data.Root.Element("lot_no").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_code").Value))
            {
                tmpSchemeData.VariantCode = data.Root.Element("variant_code").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_no").Value))
            {
                tmpSchemeData.VariantNumber = int.Parse(data.Root.Element("variant_no").Value);
            }

            tmpSchemeData = await this.FillSchemeDataVarInfoFromSchemeDataVarNo(tmpSchemeData);
            this.SchemeData = tmpSchemeData;

            this.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(actItem.ProdInfo.Code, actItem.ProdInfo.Scheme, this.SchemeData);
            this.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(actItem.ProdInfo.Code, actItem.ProdInfo.Scheme, this.SchemeData);

            if (!string.IsNullOrEmpty(data.Root.Element("extra1").Value))
            {
                this.ActExtra.Text1 = data.Root.Element("extra1").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra2").Value))
            {
                this.ActExtra.Text2 = data.Root.Element("extra2").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra3").Value))
            {
                this.ActExtra.Text3 = data.Root.Element("extra3").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra4").Value))
            {
                this.ActExtra.Text4 = data.Root.Element("extra4").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra5").Value))
            {
                this.ActExtra.DateTime5 = Convert.ToDateTime(data.Root.Element("extra5").Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataViewSortKey">Name of the fk refno index to locate the detail if it already exists.</param>
        /// <param name="actDetailFKRefnoFldName">Name of the fk refno field.</param>
        /// <example>InsertUpdateDataViewDetail("ActivityDetailPickingFKRefno", "ActivityDetailPickingFKRefno")</example>
        public void InsertUpdateDataViewDetail(string dataViewSortKey, string actDetailFKRefnoFldName)
        {
            if (this.IsUpdateView && this.ListDetails != null)
            {
                // TODO: Fix sorting
                // this is the object to be added/updated
                dynamic detail = null;
                bool detailToBeAdded = false;
                //string oldSort = this.DataViewDetails.Sort;
                //this.DataViewDetails.Sort = dataViewSortKey;

                if (this.FKRefno > 0)
                {
                    detail = this.ListDetails.Where(obj => Convert.ToInt32(obj.GetType().GetProperty(actDetailFKRefnoFldName).GetValue(obj, null)) == this.FKRefno).FirstOrDefault();
                }

                if (detail == null)
                {
                    detailToBeAdded = true;

                    detail = new ExpandoObject();
                    detail.ActivityDetailRefno = 0;
                    detail.ProductRefno = this.ActItem.ProdInfo.Refno;
                    detail.ProductCode = this.ActItem.ProdInfo.Code;
                    detail.ProductUnitBC = this.ActItem.ProdInfo.UnitBC;
                    detail.ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC;
                    detail.ProductDescription = this.ActItem.ProdInfo.Desc;
                    detail.ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc;
                    detail.UnitCode = this.ActItem.ProdInfo.UnitCode;
                    detail.BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode;
                    detail.WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode;
                    detail.ActivityDetailQuantity = 0;
                    detail.ActivityDetailBaseQuantity = 0;
                    detail.ActivityDetailWeightQuantity = 0;
                    detail.actDetailFKRefnoFldName = this.FKRefno;
                }

                detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;
                detail.ActivityDetailQuantity = Convert.ToDecimal(detail.ActivityDetailQuantity) + Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity) + Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity) + Math.Round(this.WeightQuantity, 3);
                detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
                detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
                detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
                detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
                detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
                detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
                detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
                detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
                detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
                detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
                detail.ActivityDetailExtra5Date = this.ActExtra.DateTime5;

                int locCode = 0;
                string locName = string.Empty;

                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode;
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode;
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode;
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                detail.LocationCode = locCode;
                detail.LocationName = locName;
                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;

                if (this.ActSubType == SysCommon.ActSubTypes.Returns || this.ActSubType == SysCommon.ActSubTypes.Picking || this.ActSubType == SysCommon.ActSubTypes.PickingSingle)
                {
                    detail.ActivityDetailIsConfirmed = true;
                    detail.ActivityDetailIsConfirmedString = "C";
                }

                if (this.ActSubType == SysCommon.ActSubTypes.Receiving)
                {
                    detail.IsBatchProductLabel = Convert.ToBoolean(this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch);
                }

                if (detailToBeAdded)
                {
                    this.ListDetails.Add(detail);
                }

                //TODO: Fix sorting
                //this.DataViewDetails.Sort = oldSort;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendReq"></param>
        /// <returns></returns>
        public bool InsertDetail(string sendReq)
        {
            bool isNewDetail = false;

            string reqData = SysCommon.SubmitComm(sendReq);

            if (reqData.Length > 0)
            {
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(reqData);

                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlData.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }

                this.IsUpdateView = true;
                this.NewItemFKRefno = int.Parse(xmlData.DocumentElement["itm_refno"].InnerText);
                this.FKRefno = int.Parse(xmlData.DocumentElement["ser_refno"].InnerText);

                if (this.NewItemFKRefno != this.ActItem.FKRefno)
                {
                    this.ActItem.FKRefno = NewItemFKRefno;
                    // not always new if scanned in and matche. Check for match
                    isNewDetail = true;
                }
            }
            else
            {
                // NOTE: do we need to raise an error here? SubmitComm will raise errors. When will the length be nothing?
                throw new NotImplementedException();
            }

            return isNewDetail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Inserted activity detail refno.</returns>
        /// <remarks>Assumes that 'this' is all filled in. Usually by calling this.FillActDetailObj</remarks>
        public async Task<int> InsertDetailDisconnected()
        {
            int actDetailRefno = 0;
            SQLiteAsyncConnection conn = DatabaseConnection.GetInstance().SQLConn;

            try
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeID(this.UniqueSchemeID);

                    if (isDupSchemeId)
                    {
                        // TODO: Resource strings
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeID);
                    }
                }

                string query = "INSERT INTO tActivityDetail (" +
                    "ActivityDetailSubType, " +
                    "ActivityDetailStatus, " +
                    "ActivityItemRefno, " +
                    "LocationMapRefno, " +
                    "ProductUnitRefno, " +
                    "ActivityDetailUniqueSchemeID, " +
                    "ActivityDetailSchemeID, " +
                    "ActivityDetailQuantity, " +
                    "ActivityDetailBaseQuantity, " +
                    "ActivityDetailWeightQuantity, " +
                    "ActivityDetailSingleTradeItemSerialNumber, " +
                    "ActivityDetailLogisticUnitBatchLotNumber, " +
                    "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                    "ActivityDetailLogisticUnitExpiryUseByDate, " +
                    "ActivityDetailLogisticUnitProductionDate, " +
                    "ActivityDetailLogisticUnitDueDate, " +
                    "ActivityDetailLogisticUnitPackagingDate, " +
                    "ActivityDetailLogisticUnitSkidNumber" +
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
                    Convert.ToInt32(this.ActSubType),
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(this.ActItem.Refno),
                    Convert.ToInt32(this.LocMap.CurrLocMapRefno),
                    Convert.ToInt32(this.ActItem.ProdInfo.UnitRefno),
                    Convert.ToString(this.UniqueSchemeID),
                    Convert.ToString(this.SchemeID),
                    Convert.ToDecimal(this.Quantity),
                    Convert.ToDecimal(this.BaseQuantity),
                    Convert.ToDecimal(this.WeightQuantity),
                    Convert.ToString(this.SchemeData.SingleTradeItemSerialNumber),
                    Convert.ToString(this.SchemeData.LogisticUnitBatchLotNumber),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitExpiryUseByDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitProductionDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitDueDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitPackagingDate),
                    Convert.ToString(this.SchemeData.LogisticUnitSkidNumber),
                };

                await conn.ExecuteAsync(query, parameters);

                actDetailRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>("SELECT last_insert_rowid() FROM tActivityDetail"));
                this.FKRefno = actDetailRefno * -1;

                if (0 < this.SchemeData.VariantNumber)
                {
                    int schemeVarRefno = 0;
                    List<tProductSchemeVariantModel> results = await conn.QueryAsync<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", Convert.ToInt32(this.SchemeData.VariantNumber));

                    if (results != null && results.Count > 0)
                    {
                        schemeVarRefno = Convert.ToInt32(results[0].ProductSchemeVariantRefno);
                        await conn.ExecuteAsync("INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)", Convert.ToInt32(actDetailRefno), Convert.ToInt32(schemeVarRefno));
                    }
                    else
                    {
                        throw new Exception("drSchemeVar.Read() failed.");
                    }
                }

                if (!string.IsNullOrEmpty(this.ActExtra.Text1) || string.IsNullOrEmpty(this.ActExtra.Text2) || string.IsNullOrEmpty(this.ActExtra.Text3) || string.IsNullOrEmpty(this.ActExtra.Text4) || SysCommon.IsEmptyDate(this.ActExtra.DateTime5))
                {
                    string queryExtra = "INSERT INTO tActivityDetailExtra (" +
                        "ActivityDetailRefno, " +
                        "ActivityDetailSubType, " +
                        "ActivityDetailExtra1Text, " +
                        "ActivityDetailExtra2Text, " +
                        "ActivityDetailExtra3Text, " +
                        "ActivityDetailExtra4Text, " +
                        "ActivityDetailExtra5Date" +
                        ") VALUES (" +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?" +
                        ")";

                    object[] parametersExtra = new object[]
                    {
                        Convert.ToInt32(actDetailRefno),
                        Convert.ToInt32(this.ActSubType),
                        Convert.ToString(this.ActExtra.Text1),
                        Convert.ToString(this.ActExtra.Text2),
                        Convert.ToString(this.ActExtra.Text3),
                        Convert.ToString(this.ActExtra.Text4),
                        Convert.ToDateTime(this.ActExtra.DateTime5)
                    };

                    await conn.ExecuteAsync(queryExtra, parametersExtra);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Inserting detail in disconnected mode failed.", ex);
            }

            return actDetailRefno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="outMessage"></param>
        /// <param name="overrideDisconnectedMode"></param>
        public void DataLayerStatusOutHandler(object sender, EventArgs e, string outMessage, bool overrideDisconnectedMode)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || overrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = overrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = outMessage;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="currentValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue, int minValue, int maxValue)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.MinValue = minValue;
                this.DataEventArgs.MaxValue = maxValue;
                this.DataEventArgs.SetMinMaxValue = true;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="currentValue"></param>
        public void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        #region IDataLayerEventPublisher Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventListener"></param>
        public void RegLayerListenerStatOut(IDataLayerEventListener dataLayerEventListener)
        {
            this.OnDataLayerStatusOut += new OnDataLayerStatusOutEventHandler(dataLayerEventListener.OnLayerStatOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventListener"></param>
        public void UnregLayerListenerStatOut(IDataLayerEventListener dataLayerEventListener)
        {
            this.OnDataLayerStatusOut -= new OnDataLayerStatusOutEventHandler(dataLayerEventListener.OnLayerStatOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventListener"></param>
        public void RegLayerListenerProgOut(IDataLayerEventListener dataLayerEventListener)
        {
            this.OnDataLayerProgressOut += new OnDataLayerProgressOutEventHandler(dataLayerEventListener.OnLayerProgOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerEventListener"></param>
        public void UnregLayerListenerProgOut(IDataLayerEventListener dataLayerEventListener)
        {
            this.OnDataLayerProgressOut -= new OnDataLayerProgressOutEventHandler(dataLayerEventListener.OnLayerProgOut);
        }

        #endregion

        #region IDataAccessEventListener Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        public void OnDataAccessStatusOut(IDataAccessEventPublisher dataAccessEventPublisher, DataAccessEventArgs dataAccessEventArgs)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || dataAccessEventArgs.OverrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = dataAccessEventArgs.OverrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = dataAccessEventArgs.StatusOutMsg;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessEventPublisher"></param>
        /// <param name="dataAccessEventArgs"></param>
        public void OnDataAccessProgressOut(IDataAccessEventPublisher dataAccessEventPublisher, DataAccessEventArgs dataAccessEventArgs)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.OverrideDisconnectedMode = dataAccessEventArgs.OverrideDisconnectedMode;
                this.DataEventArgs.CurrentValue = dataAccessEventArgs.CurrentValue;
                this.DataEventArgs.MinValue = dataAccessEventArgs.MinValue;
                this.DataEventArgs.MaxValue = dataAccessEventArgs.MaxValue;
                this.DataEventArgs.SetMinMaxValue = dataAccessEventArgs.SetMinMaxValue;

                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        #endregion

        #endregion
    }
}
