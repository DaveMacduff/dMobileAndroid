namespace dMobile.Controller
{
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using SQLite;
    using System;
    using System.Collections.ObjectModel;
    using SQLite;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using dMobile.Resources.Models;
    using System.Collections.Generic;

    public abstract class Activity : IDataLayerEventPublisher, IDataAccessEventListener
    {
        #region Fields

        private string mainGridMappingName = "tActivity";
        private List<object> listHdrs = null;
        private SysCommon.ActSubTypes actSubType;
        private int refno;
        private SysEnvironment sysEnv;
        private int fkRefno = 0;
        private string pono = string.Empty;
        private string actFilter = string.Empty;
        private XDocument xmlDataCriteria = null;
        private SysSkidInfo skidInfo = new SysSkidInfo();
        private DataLayerEventArgs dataEventArgs = new DataLayerEventArgs();
        private string fullClassName = typeof(Activity).FullName;
        private object selectedActivity;

        #endregion

        #region Constructors

        public Activity()
        {
            this.SysEnv = SysEnvironment.GetInstance();
        }

        #endregion

        #region Delegates

        public delegate void OnDataLayerStatusOutEventHandler(IDataLayerEventPublisher DataLayerEventPublisher, DataLayerEventArgs DataLayerEventArgs);
        public delegate void OnDataLayerProgressOutEventHandler(IDataLayerEventPublisher DataLayerEventPublisher, DataLayerEventArgs DataLayerEventArgs);

        #endregion

        #region Events

        public event OnDataLayerStatusOutEventHandler OnDataLayerStatusOut;
        public event OnDataLayerProgressOutEventHandler OnDataLayerProgressOut;

        #endregion

        #region Properties

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
        public SysSkidInfo SkidInfo
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

        /// <summary>
        /// 
        /// </summary>
        public XDocument XMLDataCriteria
        {
            get
            {
                return this.xmlDataCriteria;
            }

            set
            {
                this.xmlDataCriteria = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ActFilter
        {
            get
            {
                return this.actFilter;
            }

            set
            {
                this.actFilter = value;
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
        public List<object> ListHdrs
        {
            get
            {
                return this.listHdrs;
            }

            set
            {
                this.listHdrs = value;
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
        public string PONO
        {
            get
            {
                return this.pono;
            }

            set
            {
                this.pono = value;
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
        public object SelectedActivity
        {
            get
            {
                return this.selectedActivity;
            }

            set
            {
                this.selectedActivity = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbProgressBar"></param>
        /// <param name="xmlParam"></param>
        public abstract Task Refresh(XDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        public abstract Task<List<object>> GetActivityList(bool isSubmitBatch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dg"></param>
        public abstract ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lst"></param>
        public abstract Task SelectActivity(object selectedActivity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public abstract int GetRefnoFromRowIdx(object selectedActivity);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetFilter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <returns></returns>
        public abstract string GetFilter(XDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetControlDescription();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetFKRefnoIdx();

        /// <summary>
        /// 
        /// </summary>
        public abstract Task Purge();

        /// <summary>
        ///
        /// </summary>
        /// <param name="actSubType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsAllowedAdd(SysCommon.ActSubTypes actSubType)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actSubTypes"></param>
        /// <remarks></remarks>
        public async Task Create(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                //cmndAct.CommandType = CommandType.Text;
                //cmndAct.Parameters.Add(new SQLiteParameter("@activityStatus", Convert.ToInt32(SysCommon.ActStatus.Inserted)));
                //cmndAct.Parameters.Add(new SQLiteParameter("@activitySubType", Convert.ToInt32(actSubType)));
                //recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)actSubType);
                //recAct.SetInt32((int)SysCommon.ActFields.Status, (int)SysCommon.ActStatus.Inserted);
                //rsAct.Insert(recAct);
                
                string sql = "INSERT INTO tActivity (ActivityStatus, ActivitySubType) VALUES (?, ?)";
                await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tActivityModel>(sql, Convert.ToInt32(SysCommon.ActStatus.Inserted), Convert.ToInt32(actSubType));
                
                string queryID = "SELECT last_insert_rowid() FROM tActivity";
                this.Refno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.QueryScalarsAsync<int>(queryID));
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Create activity failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual XDocument CreateSubmitXML()
        {
            XDocument xmldocSubmission = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "</command>"
                + "</imclient>");

            return xmldocSubmission;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <remarks></remarks>
        public virtual Task Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public virtual Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public virtual Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Structure containing the label quantities.</returns>
        /// <remarks>Currently picking is the only module needing this. Used for Shipping Labels.</remarks>
        public virtual Task<SysCommon.SLabelQuantity> GetLabelQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isRepackQuantityOnly"></param>
        /// <returns>Structure containing the label quantities.</returns>
        /// <remarks>Currently picking is the only module needing this. Used for Shipping Labels.</remarks>
        public virtual Task<SysCommon.SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if all items have been reviewed.
        /// Under quantity check for ALL items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual async Task<bool> ChangeStatus(object selectedActivity)
        {
            return true;
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
    }
}
