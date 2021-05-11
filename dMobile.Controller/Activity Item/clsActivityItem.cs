namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using Xamarin.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Used as return value for GetActivityItemProductSchemeDataFromActivityDetailRefno
    /// </summary>
    /// <remarks></remarks>
    public struct SActItemSchemeData
    {
        #region Fields

        private int actItemFKRefno;
        private decimal actDetailQuantity;
        private string unitCode;
        private decimal actDetailBaseQuantity;
        private string baseUnitCode;
        private decimal actDetailWeightQuantity;
        private string weightUnitCode;
        private SProductSchemeData prodSchemeData;
        private int locMapWhsFKRefno;
        private int locMapSectFKRefno;
        private int locMapSlotFKRefno;
        private ActDetailExtra actDetailExtra;

        #endregion

        #region Properties

        public ActDetailExtra ActDetailExtra
        {
            get
            {
                return this.actDetailExtra;
            }

            set
            {
                this.actDetailExtra = value;
            }
        }

        public int ActItemFKRefno
        {
            get
            {
                return this.actItemFKRefno;
            }

            set
            {
                this.actItemFKRefno = value;
            }
        }

        public decimal ActDetailQuantity
        {
            get
            {
                return this.actDetailQuantity;
            }

            set
            {
                this.actDetailQuantity = value;
            }
        }

        public string UnitCode
        {
            get
            {
                if (this.unitCode == null)
                {
                    this.unitCode = string.Empty;
                }

                return this.unitCode;
            }

            set
            {
                this.unitCode = value;
            }
        }

        public decimal ActDetailBaseQuantity
        {
            get
            {
                return this.actDetailBaseQuantity;
            }

            set
            {
                this.actDetailBaseQuantity = value;
            }
        }

        public string BaseUnitCode
        {
            get
            {
                if (this.baseUnitCode == null)
                {
                    this.baseUnitCode = string.Empty;
                }

                return this.baseUnitCode;
            }

            set
            {
                this.baseUnitCode = value;
            }
        }

        public decimal ActDetailWeightQuantity
        {
            get
            {
                return this.actDetailWeightQuantity;
            }

            set
            {
                this.actDetailWeightQuantity = value;
            }
        }

        public string WeightUnitCode
        {
            get
            {
                if (this.weightUnitCode == null)
                {
                    this.weightUnitCode = string.Empty;
                }

                return this.weightUnitCode;
            }

            set
            {
                this.weightUnitCode = value;
            }
        }

        public SProductSchemeData ProdSchemeData
        {
            get
            {
                return this.prodSchemeData;
            }

            set
            {
                this.prodSchemeData = value;
            }
        }

        public int LocMapWhsFKRefno
        {
            get
            {
                return this.locMapWhsFKRefno;
            }

            set
            {
                this.locMapWhsFKRefno = value;
            }
        }

        public int LocMapSectFKRefno
        {
            get
            {
                return this.locMapSectFKRefno;
            }

            set
            {
                this.locMapSectFKRefno = value;
            }
        }

        public int LocMapSlotFKRefno
        {
            get
            {
                return this.locMapSlotFKRefno;
            }

            set
            {
                this.locMapSlotFKRefno = value;
            }
        }

        #endregion
    }

    public abstract class ActItem : IDataLayerEventPublisher, IDataAccessEventListener
    {
        #region Fields

        private string mainGridMappingName = "tActivityItem";
        private List<object> listItems = null;
        private SysCommon.ActSubTypes actSubType;
        private int refno = 0;
        private int fkRefno = 0;
        private SysEnvironment sysEnv;
        private ProductInfo prodInfo = new ProductInfo();
        private object selectedActivityItem;

        /// <summary>
        /// Used to store the currently selected line item location map.
        /// </summary>
        private LocationMap locMap;
        private Activity actHdr = null;
        private bool isAllowAddItem = false;
        private string actItemFilter = string.Empty;
        private DataLayerEventArgs dataEventArgs = new DataLayerEventArgs();
        private string fullClassName = typeof(ActItem).FullName;
        private bool isAskCreateNewItem = true;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity"></param>
        public ActItem(Activity activity)
        {
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.ActHdr = activity;
            this.ActSubType = this.ActHdr.ActSubType;
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsAskCreateNewItem
        {
            get
            {
                return this.isAskCreateNewItem;
            }

            set
            {
                this.isAskCreateNewItem = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public string ActItemFilter
        {
            get
            {
                return this.actItemFilter;
            }

            set
            {
                this.actItemFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowAddItem
        {
            get
            {
                return this.isAllowAddItem;
            }

            set
            {
                this.isAllowAddItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the current selected line item location map.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public ProductInfo ProdInfo
        {
            get
            {
                return this.prodInfo;
            }

            set
            {
                this.prodInfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public List<object> ListItems
        {
            get
            {
                return this.listItems;
            }

            set
            {
                this.listItems = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public Activity ActHdr
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

        /// <summary>
        /// 
        /// </summary>
        public object SelectedActivityItem
        {
            get
            {
                return this.selectedActivityItem;
            }

            set
            {
                this.selectedActivityItem = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public abstract Task Insert(XDocument data, bool askToInsertAsNewItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public abstract Task Refresh(XDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract string CreateSubmitXML(XDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public abstract Task<List<object>> GetActivityItemList(bool isSubmitBatch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataview"></param>
        public abstract ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        public abstract Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetDataViewSortFKRefnoIdx();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public abstract string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public abstract string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno);

        /// <summary>
        /// Update the item dataview quantity. Adds to the existing values.
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        /// <remarks>Scanning a barcode that as all the required scheme data should just add the new quantity to 
        /// the existing.</remarks>
        public abstract void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity);

        /// <summary>
        /// Replaces the item dataview quantity. Does not add to the existing values.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <remarks>Returning from the activity detail screen the xml will have the total quantity for 
        /// all details.</remarks>
        public abstract void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity);

        /// <summary>
        /// Update the activity line item quantity in the database from the line item dataview.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        /// <remarks></remarks>
        public abstract Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public virtual async Task SelectActivityItem(object selectedActivityItem)
        {
            dynamic item = selectedActivityItem;

            if (selectedActivityItem == null || item == null)
            {
                return;
            }

            this.SelectedActivityItem = selectedActivityItem;
            this.Refno = Convert.ToInt32(item.ActivityItemRefno);
            this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(item.ProductUnitRefno));

            bool clearSkidFlag = SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid;
            bool hasSkidData = this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0;
            bool allowAddToSkid = hasSkidData;

            if (allowAddToSkid && clearSkidFlag)
            {
                // With this flag turned on, they should only be allowed to add to skid with the same product
                allowAddToSkid &= this.ProdInfo.Refno == this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno;
            }

            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc && !this.SysEnv.SysSettings.IsVerifyProductLocation)
            {
                if (0 < this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno && this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno == this.ProdInfo.Refno)
                {
                    this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                }
                else
                {
                    // Use a tmpLocMap to get the WarehouseLocationMapRefno. 
                    // The count LocationMapRefno could be a warehouse, section or slot location map refno.
                    SLocMap tmpLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(item.LocationMapRefno));
                    int tmpDefaultLocMap = this.GetDefaultLocMapRefno(tmpLocMap.WhsLocMapRefno,Convert.ToInt32(item.ProductRefno));
                    this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(tmpDefaultLocMap);

                    // clear the skid
                    if (clearSkidFlag)
                    {
                        this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                    }
                }
            }
            else
            {
                // Default the location to the skid location even if defaulting location is turned off.
                if (0 < this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno && this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno == this.ProdInfo.Refno)
                {
                    this.LocMap.CurrLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                }
                else if (clearSkidFlag)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }
            }

            // LocationMap.GetInstance().CurrLocMap needs to be set before call to GetSchemeComponentsFromRefno.
            this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
        }

        /// <summary>
        /// Get the default location map refno for a warehouse, product.
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="productRefno"></param>
        /// <returns></returns>
        /// Location map refno.
        /// <remarks>Use the Current Warehouse Location Map Refno which should have be set from the selected 
        /// Activity Item</remarks>
        public virtual async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(whsLocMapRefno, prodRefno, SysCommon.ProdLocMapTypes.Picking);
            return locMapRefno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Structure containing the label quantities.</returns>
        /// <remarks>Currently picking is the only module needing this. Used for Shipping Labels.</remarks>
        public virtual SysCommon.SLabelQuantity GetLabelQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isRepackQuantityOnly"></param>
        /// <returns>Structure containing the label quantities.</returns>
        /// <remarks>Currently picking is the only module needing this. Used for Shipping Labels.</remarks>
        public virtual SysCommon.SLabelQuantity GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locMap"></param>
        /// <param name="prodInfo"></param>
        /// <returns></returns>
        public virtual string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
        {
            // TODO: Resource strings
            string message = string.Format("Item not found for "
                + Environment.NewLine + "Location {0} "
                + Environment.NewLine + "Product {1} BC {2} Unit {3}",
                LocationMapTasks.GetCurrentReadableLocation(),
                prodInfo.Code,
                prodInfo.UnitBC,
                prodInfo.UnitCode);

            return message;
        }

        /// <summary>
        /// IsReadyToApply will perform validations, such as duplicate checks and quantity over / under validations.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>clsPickingActivityItem for example performs extra quantity validation.</remarks>
        public virtual async Task IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string actDetailUniqSchemeID = data.Root.Element("ActivityDetailUniqueSchemeID").Value;

                if (string.Compare(actDetailUniqSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeID = await ProductInfoTasks.IsDuplicateUniqueSchemeID(actDetailUniqSchemeID);

                    if (isDupSchemeID)
                    {
                        // TODO: Resource strings
                        throw new IsReadyToApplyException("Duplicate scheme id!" + Environment.NewLine + actDetailUniqSchemeID, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDetailFKRefno"></param>
        /// <returns></returns>
        public virtual SActItemSchemeData GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
        {
            return new SActItemSchemeData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual bool ChangeStatus(int idx)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual async Task<string> GetComment(object selectedActivityItem)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        public virtual Task Submit(XDocument data, object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public virtual void Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string CreateDeleteXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool IsCatchAsk()
        {
            return this.ProdInfo.IsCatchAsk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCatchAskZero()
        {
            return this.ProdInfo.IsCatchAskZero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual Task Update(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the total quantity as a message string for displaying in a messagebox.
        /// </summary>
        /// <remarks>Used for WO# 11445 Trimpac wanted to see the total cases received.
        /// Trimpac's assumption is that all the UOM are the same to have any value being summed.</remarks>
        /// Weight makes sense to sum.
        /// Decided to add this to the base class in the event other modules would need something similar.
        /// Lets hope common sense will prevail. :-)
        /// <returns></returns>
        public virtual async Task<string> GetTotalQuantity()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actSubTypes"></param>
        public async Task Create(SysCommon.ActSubTypes actSubTypes)
        {
            try
            {
                SQLiteAsyncConnection conn = DatabaseConnection.GetInstance().SQLConn;

                await conn.ExecuteAsync("INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (?, ?, ?)", Convert.ToInt32(actSubTypes), Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(this.ActHdr.Refno));
                this.Refno = Convert.ToInt32(conn.QueryScalarsAsync<int>("SELECT last_insert_rowid() FROM tActivityItem"));
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred.", ex);
            }
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

        #endregion
    }
}