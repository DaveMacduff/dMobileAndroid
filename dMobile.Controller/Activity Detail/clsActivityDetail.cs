namespace dMobile.Controller
{
    using System;
    using System.Data;
    
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public abstract class ActDetail : IDataLayerEventPublisher, IDataAccessEventListener
    {
        #region Fields

        private string mainGridMappingName = "tActivityDetail";
        private DataView dataViewDetails = null;
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

        public delegate void OnDataLayerStatusOutEventHandler(
            IDataLayerEventPublisher dataLayerEventPublisher,
            DataLayerEventArgs dataLayerEventArgs);

        public delegate void OnDataLayerProgressOutEventHandler(
            IDataLayerEventPublisher dataLayerEventPublisher,
            DataLayerEventArgs dataLayerEventArgs);

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

        public DataView DataViewDetails
        {
            get
            {
                return this.dataViewDetails;
            }

            set
            {
                this.dataViewDetails = value;
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

        public abstract void Insert(XDocument data);

        public abstract void Delete();

        public abstract void Refresh();

        public abstract string CreateSubmitXML(XDocument data);

        public abstract string CreateDeleteXML(XDocument data);

        public abstract void Submit(int selectedRowIndex, XDocument data);

        public abstract DataView Search(string searchText);

        public abstract DataView GetDataView(bool isSubmitBatch);

        public abstract void DesignDataGrid(DataGrid dgGrid);

        public abstract void SelectDetail(int selectedIndex);

        public abstract int GetRefno(int selectedIndex);

        public abstract int GetFKRefno(int selectedIndex);

        public abstract void InsertSkid(SysSkidInfo skidInfo);

        public abstract void DeleteSkid(SysSkidInfo skidInfo);

        /// <summary>
        /// Total all activity detail quantity from the dataview
        /// </summary>
        /// <returns>SFillQuantity structure</returns>
        public virtual SysCommon.SFillQuantity GetDataViewTotalQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();

            if (this.DataViewDetails != null && this.DataViewDetails.Count > 0)
            {
                for (int i = 0; i <= this.DataViewDetails.Count - 1; i++)
                {
                    fillQty.Quantity = fillQty.Quantity + (decimal)this.DataViewDetails[i]["ActivityDetailQuantity"];
                    fillQty.BaseQuantity = fillQty.BaseQuantity + (decimal)this.DataViewDetails[i]["ActivityDetailBaseQuantity"];
                    fillQty.WeightQuantity = fillQty.WeightQuantity
                          + (decimal)this.DataViewDetails[i]["ActivityDetailWeightQuantity"];
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
            this.ActExtra.Text5 = string.Empty;
            this.NewItemFKRefno = 0;
            this.NewFKRefno = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public virtual ActDetail GetInfo(int idx, bool isAdd)
        {
            SProductSchemeData tmpSchemeData = this.SchemeData;
            DataView details = this.DataViewDetails;

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
                this.Quantity = (decimal)details[idx]["ActivityDetailQuantity"];
                this.BaseQuantity = (decimal)details[idx]["ActivityDetailBaseQuantity"];
                this.WeightQuantity = (decimal)details[idx]["ActivityDetailWeightQuantity"];
                this.Refno = (int)details[idx]["ActivityDetailRefno"];
                this.SchemeID = (string)details[idx]["ActivityDetailSchemeID"];
                this.UniqueSchemeID = (string)details[idx]["ActivityDetailUniqueSchemeID"];

                if (details.Table.Columns.Contains("ActivityDetailExtra1Text"))
                {
                    // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                    if (details[idx]["ActivityDetailExtra1Text"] != DBNull.Value)
                    {
                        this.ActExtra.Text1 = (string)details[idx]["ActivityDetailExtra1Text"];
                    }
                }

                if (details.Table.Columns.Contains("ActivityDetailExtra2Text"))
                {
                    // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                    if (details[idx]["ActivityDetailExtra2Text"] != DBNull.Value)
                    {
                        this.ActExtra.Text2 = (string)details[idx]["ActivityDetailExtra2Text"];
                    }
                }

                if (details.Table.Columns.Contains("ActivityDetailExtra3Text"))
                {
                    // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                    if (details[idx]["ActivityDetailExtra3Text"] != DBNull.Value)
                    {
                        this.ActExtra.Text3 = (string)details[idx]["ActivityDetailExtra3Text"];
                    }
                }

                if (details.Table.Columns.Contains("ActivityDetailExtra4Text"))
                {
                    // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                    if (details[idx]["ActivityDetailExtra4Text"] != DBNull.Value)
                    {
                        this.ActExtra.Text4 = (string)details[idx]["ActivityDetailExtra4Text"];
                    }
                }

                if (details.Table.Columns.Contains("ActivityDetailExtra5Text"))
                {
                    // Check for DBNull because extra detail is in a child table, left join from the dataview query.
                    if (details[idx]["ActivityDetailExtra5Text"] != DBNull.Value)
                    {
                        this.ActExtra.Text5 = (string)details[idx]["ActivityDetailExtra5Text"];
                    }
                }

                DateTime bestBefore = (DateTime)details[idx]["ActivityDetailLogisticUnitBestBeforeSellByDate"];
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

                DateTime production = (DateTime)details[idx]["ActivityDetailLogisticUnitProductionDate"];
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

                tmpSchemeData.SingleTradeItemSerialNumber = (string)details[idx]["ActivityDetailSingleTradeItemSerialNumber"];
                tmpSchemeData.LogisticUnitBatchLotNumber = (string)details[idx]["ActivityDetailLogisticUnitBatchLotNumber"];

                // Check for DBNull because variants are in a child table, left join from the dataview query.
                if (details[idx]["ProductSchemeVariantCode"] != DBNull.Value)
                {
                    tmpSchemeData.VariantCode = (string)details[idx]["ProductSchemeVariantCode"];
                }

                // Check for DBNull because variants are in a child table, left join from the dataview query.
                if (details[idx]["ProductSchemeVariantNo"] != DBNull.Value)
                {
                    tmpSchemeData.VariantNumber = (int)details[idx]["ProductSchemeVariantNo"];
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
        public virtual void IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = ProductInfoTasks.IsDuplicateUniqueSchemeID(this.UniqueSchemeID);
                    if (isDupSchemeId)
                    {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DuplicateSchemeId) + Environment.NewLine + this.UniqueSchemeID);
#else
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeID);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string CreateSubmitProductLabelXML(XDocument data)
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
            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked
                && !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                bestBeforeDate = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            string productionDate = string.Empty;
            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked
                && !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
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
                + ("<extra5>" + this.ActExtra.Text5 + "</extra5>")
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
                + ("<extra5>" + actDetail.ActExtra.Text5 + "</extra5>")
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

            if (actItem.ProdInfo.Scheme.IsBestBeforeDateTracked
                && SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsLotTracked && string.IsNullOrEmpty(this.SchemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsProductionDateTracked
                && SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
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

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked
                && SysCommon.IsEmptyDateTime(schemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsLotTracked
                && string.IsNullOrEmpty(schemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsProductionDateTracked
                && SysCommon.IsEmptyDateTime(schemeData.LogisticUnitProductionDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsSerialTracked
                && string.IsNullOrEmpty(schemeData.SingleTradeItemSerialNumber))
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
        public SProductSchemeData FillSchemeDataVarInfoFromSchemeDataVarNo(SProductSchemeData prodSchemeData)
        {
            if (0 < prodSchemeData.VariantNumber)
            {
                using (SQLiteCommand cmndSchemeVar = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndSchemeVar.CommandText = "tProductSchemeVariant";
                    cmndSchemeVar.CommandType = CommandType.TableDirect;
                    cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                    using (SqliteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                    {
                        if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { prodSchemeData.VariantNumber }))
                        {
                            if (drSchemeVar.Read())
                            {
                                prodSchemeData.VariantCode = drSchemeVar["ProductSchemeVariantCode"].ToString();
                                prodSchemeData.VariantDescription = drSchemeVar["ProductSchemeVariantDescription"].ToString();
                            }
                            else
                            {
                                throw new Exception("drSchemeVar.Read() failed.");
                            }
                        }

                        drSchemeVar.Close();
                    }
                }
            }

            return prodSchemeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void FillActDetailObjFromXML(XDocument data)
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

            tmpSchemeData = this.FillSchemeDataVarInfoFromSchemeDataVarNo(tmpSchemeData);
            this.SchemeData = tmpSchemeData;

            this.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                actItem.ProdInfo.Code,
                actItem.ProdInfo.Scheme,
                this.SchemeData);
            this.UniqueSchemeID = ProductInfoTasks.GetActUniqueSchemeIDFromSProdSchemeData(
                actItem.ProdInfo.Code,
                actItem.ProdInfo.Scheme,
                this.SchemeData);

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
                this.ActExtra.Text5 = data.Root.Element("extra5").Value;
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
            if (this.IsUpdateView && this.DataViewDetails != null)
            {
                DataRowView rowView = null;
                string oldSort = this.DataViewDetails.Sort;
                int rowIdx = 0;
                this.DataViewDetails.Sort = dataViewSortKey;
                if (this.FKRefno > 0)
                {
                    rowIdx = this.DataViewDetails.Find(new object[] { this.FKRefno });
                    if (rowIdx > -1)
                    {
                        rowView = this.DataViewDetails[rowIdx];
                    }
                }

                if (rowView == null)
                {
                    rowView = this.DataViewDetails.AddNew();
                    rowView["ActivityDetailRefno"] = 0;
                    rowView["ProductRefno"] = this.ActItem.ProdInfo.Refno;
                    rowView["ProductCode"] = this.ActItem.ProdInfo.Code;
                    rowView["ProductUnitBC"] = this.ActItem.ProdInfo.UnitBC;
                    rowView["ProductUnitIsBC"] = this.ActItem.ProdInfo.UnitIsBC;
                    rowView["ProductDescription"] = this.ActItem.ProdInfo.Desc;
                    rowView["ProductUnitSizeDescription"] = this.ActItem.ProdInfo.UnitSizeDesc;
                    rowView["UnitCode"] = this.ActItem.ProdInfo.UnitCode;
                    rowView["BaseUnitCode"] = this.ActItem.ProdInfo.BaseUnitCode;
                    rowView["WtUnitCode"] = this.ActItem.ProdInfo.WeightUnitCode;
                    rowView["ActivityDetailQuantity"] = 0;
                    rowView["ActivityDetailBaseQuantity"] = 0;
                    rowView["ActivityDetailWeightQuantity"] = 0;

                    rowView[actDetailFKRefnoFldName] = this.FKRefno;
                }

                rowView["LocationMapRefno"] = this.LocMap.CurrLocMapRefno;
                rowView["ActivityDetailQuantity"] =
                    Convert.ToDecimal(rowView["ActivityDetailQuantity"]) + Math.Round(this.Quantity, 3);
                rowView["ActivityDetailBaseQuantity"] =
                    Convert.ToDecimal(rowView["ActivityDetailBaseQuantity"]) + Math.Round(this.BaseQuantity, 3);
                rowView["ActivityDetailWeightQuantity"] =
                    Convert.ToDecimal(rowView["ActivityDetailWeightQuantity"]) + Math.Round(this.WeightQuantity, 3);
                rowView["ProductSchemeVariantCode"] = this.SchemeData.VariantCode;
                rowView["ProductSchemeVariantNo"] = this.SchemeData.VariantNumber;

                // " tProductSchemeVariant.ProductSchemeVariantDescription " & _
                rowView["ActivityDetailSingleTradeItemSerialNumber"] = this.SchemeData.SingleTradeItemSerialNumber;
                rowView["ActivityDetailLogisticUnitBestBeforeSellByDate"] = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                rowView["ActivityDetailLogisticUnitBatchLotNumber"] = this.SchemeData.LogisticUnitBatchLotNumber;
                rowView["ActivityDetailLogisticUnitProductionDate"] = this.SchemeData.LogisticUnitProductionDate;
                rowView["ActivityDetailLogisticUnitSkidNumber"] = this.SchemeData.LogisticUnitSkidNumber;

                rowView["ActivityDetailExtra1Text"] = this.ActExtra.Text1;
                rowView["ActivityDetailExtra2Text"] = this.ActExtra.Text2;
                rowView["ActivityDetailExtra3Text"] = this.ActExtra.Text3;
                rowView["ActivityDetailExtra4Text"] = this.ActExtra.Text4;
                rowView["ActivityDetailExtra5Text"] = this.ActExtra.Text5;

                string locCode = string.Empty;
                string locName = string.Empty;
                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode.ToString();
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode.ToString();
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode.ToString();
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                rowView["LocationCode"] = locCode;
                rowView["LocationName"] = locName;
                rowView["ActivityDetailSchemeID"] = this.SchemeID;
                rowView["ActivityDetailUniqueSchemeID"] = this.UniqueSchemeID;

                if (this.ActSubType == SysCommon.ActSubTypes.Returns
                    || this.ActSubType == SysCommon.ActSubTypes.Picking
                    || this.ActSubType == SysCommon.ActSubTypes.PickingSingle
                    || this.ActSubType == SysCommon.ActSubTypes.Transfers)
                {
                    rowView["ActivityDetailIsConfirmed"] = true;
                    rowView["ActivityDetailIsConfirmedString"] = "C";
                }

                if (this.ActSubType == SysCommon.ActSubTypes.Receiving)
                {
                    rowView["IsBatchProductLabel"] = Convert.ToBoolean(this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch);
                }

                rowView.EndEdit();

                this.DataViewDetails.Sort = oldSort;
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
        public int InsertDetailDisconnected()
        {
            int actDetailRefno = 0;
            SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

            try
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = ProductInfoTasks.IsDuplicateUniqueSchemeID(this.UniqueSchemeID);
                    if (isDupSchemeId)
                    {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DuplicateSchemeId) + Environment.NewLine + this.UniqueSchemeID);
#else
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeID);
#endif
                    }
                }

                using (SQLiteCommand cmndDetail = conn.CreateCommand())
                {
                    cmndDetail.CommandText = "tActivityDetail";
                    cmndDetail.CommandType = CommandType.TableDirect;
                    cmndDetail.IndexName = "PK_ActivityDetailOnActivityDetailRefno";

                    using (SqlCeResultSet rsDetail = cmndDetail.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        SqlCeUpdatableRecord recDetail = rsDetail.CreateRecord();

                        recDetail.SetInt32((int)SysCommon.ActDetailFields.SubType, (int)this.ActSubType);
                        recDetail.SetInt32((int)SysCommon.ActDetailFields.Status, (int)SysCommon.ActStatus.Inserted);
                        recDetail.SetInt32((int)SysCommon.ActDetailFields.ActivityItemRefno, this.ActItem.Refno);
                        recDetail.SetInt32((int)SysCommon.ActDetailFields.LocationMapRefno, this.LocMap.CurrLocMapRefno);
                        recDetail.SetInt32((int)SysCommon.ActDetailFields.ProductUnitRefno, this.ActItem.ProdInfo.UnitRefno);
                        recDetail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, this.Quantity);
                        recDetail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, this.BaseQuantity);
                        recDetail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, this.WeightQuantity);
                        recDetail.SetString(
                            (int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber,
                            this.SchemeData.SingleTradeItemSerialNumber);
                        recDetail.SetString(
                            (int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber,
                            this.SchemeData.LogisticUnitBatchLotNumber);
                        recDetail.SetDateTime(
                            (int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate,
                            this.SchemeData.LogisticUnitBestBeforeSellByDate);
                        recDetail.SetDateTime(
                            (int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate,
                            this.SchemeData.LogisticUnitExpiryUseByDate);
                        recDetail.SetDateTime(
                            (int)SysCommon.ActDetailFields.LogisticUnitProductionDate,
                            this.SchemeData.LogisticUnitProductionDate);
                        recDetail.SetDateTime(
                            (int)SysCommon.ActDetailFields.LogisticUnitDueDate,
                            this.SchemeData.LogisticUnitDueDate);
                        recDetail.SetDateTime(
                            (int)SysCommon.ActDetailFields.LogisticUnitPackagingDate,
                            this.SchemeData.LogisticUnitPackagingDate);
                        recDetail.SetString(
                            (int)SysCommon.ActDetailFields.LogisticUnitSkidNumber,
                            this.SchemeData.LogisticUnitSkidNumber);
                        recDetail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, this.UniqueSchemeID);
                        recDetail.SetString((int)SysCommon.ActDetailFields.SchemeID, this.SchemeID);

                        rsDetail.Insert(recDetail);
                    }
                }

                using (SQLiteCommand cmndID = conn.CreateCommand())
                {
                    cmndID.CommandText = "SELECT @@IDENTITY FROM tActivityDetail";
                    cmndID.CommandType = CommandType.Text;

                    actDetailRefno = Convert.ToInt32(cmndID.ExecuteScalar());
                }

                this.FKRefno = actDetailRefno * -1;

                if (0 < this.SchemeData.VariantNumber)
                {
                    using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                    {
                        cmndSchemeVar.CommandText = "tProductSchemeVariant";
                        cmndSchemeVar.CommandType = CommandType.TableDirect;
                        cmndSchemeVar.IndexName = "UQ_ProductSchemeVariantOnProductSchemeVariantNo";

                        using (SqliteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                        {
                            using (SQLiteCommand cmndSchemeVarXRef = conn.CreateCommand())
                            {
                                cmndSchemeVarXRef.CommandText = "tActivityDetailProductSchemeVariantXRef";
                                cmndSchemeVarXRef.CommandType = CommandType.TableDirect;
                                cmndSchemeVarXRef.IndexName =
                                    "PK_ActivityDetailProductSchemeVariantXRefOnActivityDetailRefnoAndProductSchemeVariantRefno";

                                using (SqlCeResultSet rsSchemeVarXRef = cmndSchemeVarXRef.ExecuteResultSet(ResultSetOptions.Updatable))
                                {
                                    SqlCeUpdatableRecord recSchemeVarXRef = rsSchemeVarXRef.CreateRecord();

                                    int schemeVarRefno = 0;
                                    if (drSchemeVar.Seek(DbSeekOptions.FirstEqual, new object[] { this.SchemeData.VariantNumber }))
                                    {
                                        if (drSchemeVar.Read())
                                        {
                                            schemeVarRefno = drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno);

                                            recSchemeVarXRef.SetInt32(
                                                (int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno,
                                                actDetailRefno);
                                            recSchemeVarXRef.SetInt32(
                                                (int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno,
                                                schemeVarRefno);

                                            rsSchemeVarXRef.Insert(recSchemeVarXRef);
                                        }
                                        else
                                        {
                                            throw new Exception("drSchemeVar.Read() failed.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            string.Format(
                                            "Unable to locate the product scheme variant no {0} in tProductSchemeVariant,",
                                            this.SchemeData.VariantNumber));
                                    }
                                }
                            }

                            drSchemeVar.Close();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.ActExtra.Text1)
                    || string.IsNullOrEmpty(this.ActExtra.Text2)
                    || string.IsNullOrEmpty(this.ActExtra.Text3)
                    || string.IsNullOrEmpty(this.ActExtra.Text4)
                    || string.IsNullOrEmpty(this.ActExtra.Text5))
                {
                    using (SQLiteCommand cmndExtra = conn.CreateCommand())
                    {
                        cmndExtra.CommandText = "tActivityDetailExtra";
                        cmndExtra.CommandType = CommandType.TableDirect;
                        cmndExtra.IndexName = "PK_ActivityDetailExtraOnActivityDetailRefno";

                        using (SqlCeResultSet rsExtra = cmndExtra.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            SqlCeUpdatableRecord recExtra = rsExtra.CreateRecord();

                            recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailRefno, actDetailRefno);
                            recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailSubType, (int)this.ActSubType);
                            recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, this.ActExtra.Text1);
                            recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, this.ActExtra.Text2);
                            recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, this.ActExtra.Text3);
                            recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, this.ActExtra.Text4);
                            recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, this.ActExtra.Text5);

                            rsExtra.Insert(recExtra);
                        }
                    }
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredInsertingDetailInDisconnectedModeFailed), ex);
#else
                throw new Exception("Database exception occurred. Inserting detail in disconnected mode failed.", ex);
#endif
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
        public void DataLayerStatusOutHandler(
            object sender,
            EventArgs e,
            string outMessage,
            bool overrideDisconnectedMode)
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
        public void OnDataAccessStatusOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
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
        public void OnDataAccessProgressOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
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
