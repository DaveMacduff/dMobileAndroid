namespace dMobile.Controller
{
    using System;
    using System.Data;
    
    using System.Windows.Forms;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;

    public abstract class Activity : IDataLayerEventPublisher, IDataAccessEventListener
    {
        #region Fields

        private string mainGridMappingName = "tActivity";
        private DataView dataViewHdrs = null;
        private SysCommon.ActSubTypes actSubType;
        private int refno;
        private SysEnvironment sysEnv;
        private int fkRefno = 0;
        private string pono = string.Empty;
        private string actFilter = string.Empty;
        private XDocument xmlDataCriteria = null;
        private SysSkidInfo skidInfo = new SysSkidInfo();
        private int selectedIndex = 0;
        private DataLayerEventArgs dataEventArgs = new DataLayerEventArgs();
        private string fullClassName = typeof(Activity).FullName;

        #endregion

        #region Constructors

        public Activity()
        {
            this.SysEnv = SysEnvironment.GetInstance();
        }

        #endregion

        #region Delegates

        public delegate void OnDataLayerStatusOutEventHandler(
            IDataLayerEventPublisher DataLayerEventPublisher,
            DataLayerEventArgs DataLayerEventArgs);

        public delegate void OnDataLayerProgressOutEventHandler(
            IDataLayerEventPublisher DataLayerEventPublisher,
            DataLayerEventArgs DataLayerEventArgs);

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
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
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
        public DataView DataViewHdrs
        {
            get
            {
                return this.dataViewHdrs;
            }

            set
            {
                this.dataViewHdrs = value;
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

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbProgressBar"></param>
        /// <param name="xmlParam"></param>
        public abstract void Refresh(XDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public abstract DataView Search(string searchText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsSubmitBatch"></param>
        /// <returns></returns>
        public abstract DataView GetDataView(bool isSubmitBatch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dg"></param>
        public abstract void DesignDataGrid(DataGrid dg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lst"></param>
        public abstract void SelectHdr(int idx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public abstract int GetRefnoFromRowIdx(int idx);

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
        public abstract void Purge();

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
        public void Create(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                using (SQLiteCommand cmndAct = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndAct.CommandText = "tActivity";
                    cmndAct.CommandType = CommandType.TableDirect;

                    using (SqlCeResultSet rsAct = cmndAct.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        SqlCeUpdatableRecord recAct = rsAct.CreateRecord();

                        using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndID.CommandText = "SELECT @@IDENTITY";
                            cmndID.CommandType = CommandType.Text;

                            recAct.SetInt32((int)SysCommon.ActFields.SubType, (int)actSubType);
                            recAct.SetInt32((int)SysCommon.ActFields.Status, (int)SysCommon.ActStatus.Inserted);

                            rsAct.Insert(recAct);

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());
                        }
                    }
                }
            }
            catch (SqlCeException ex)
            {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredCreateActivityFailed), ex);
#else
                throw new Exception("Database exception occurred. Create activity failed.", ex);
#endif
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
        public virtual void Insert(XDocument xmlParams)
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
        /// <remarks></remarks>
        public virtual void PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
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
        /// Check if all items have been reviewed.
        /// Under quantity check for ALL items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool ChangeStatus(int selectedIndex)
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
        public void DataLayerProgressOutHandler(
            object sender,
            EventArgs e,
            int currentValue,
            int minValue,
            int maxValue)
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
