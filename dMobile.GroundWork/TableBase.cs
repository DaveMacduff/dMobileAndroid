namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Xml.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TableBase" />
    /// </summary>
    public abstract class TableBase : IDataAccessEventPublisher
    {
        /// <summary>
        /// Defines the fullClassName
        /// </summary>
        private string fullClassName = "TableBase";

        /// <summary>
        /// Defines the sysEnv
        /// </summary>
        private SysEnvironment sysEnv;

        /// <summary>
        /// Defines the dataEventArgs
        /// </summary>
        private DataAccessEventArgs dataEventArgs = new DataAccessEventArgs();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBase"/> class.
        /// </summary>
        public TableBase()
        {
            this.SysEnv = SysEnvironment.GetInstance();
        }

        /// <summary>
        /// The OnDataAccessStatusOutEventHandler
        /// </summary>
        /// <param name="dataAccessEventPublisher">The dataAccessEventPublisher<see cref="IDataAccessEventPublisher"/></param>
        /// <param name="dataAccessEventArgs">The dataAccessEventArgs<see cref="DataAccessEventArgs"/></param>
        public delegate void OnDataAccessStatusOutEventHandler(
        IDataAccessEventPublisher dataAccessEventPublisher,
        DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// The OnDataAccessProgressOutEventHandler
        /// </summary>
        /// <param name="dataAccessEventPublisher">The dataAccessEventPublisher<see cref="IDataAccessEventPublisher"/></param>
        /// <param name="dataAccessEventArgs">The dataAccessEventArgs<see cref="DataAccessEventArgs"/></param>
        public delegate void OnDataAccessProgressOutEventHandler(
        IDataAccessEventPublisher dataAccessEventPublisher,
        DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// Defines the OnDataAccessStatusOut
        /// </summary>
        public event OnDataAccessStatusOutEventHandler OnDataAccessStatusOut;

        /// <summary>
        /// Defines the OnDataAccessProgressOut
        /// </summary>
        public event OnDataAccessProgressOutEventHandler OnDataAccessProgressOut;

        /// <summary>
        /// Gets or sets the DataEventArgs
        /// </summary>
        public DataAccessEventArgs DataEventArgs
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
        /// Gets or sets the FullClassName
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
        /// Gets or sets the SysEnv
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
        /// The Request
        /// </summary>
        public virtual async Task Request()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno"></param>
        public virtual async Task Request(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="criteria"></param>
        public virtual void RequestData(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="actRefno"></param>
        /// <param name="criteria"></param>
        public virtual void RequestData(int actRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public virtual async Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="sectLocMapRefno"></param>
        public virtual void PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge
        /// </summary>
        /// <param name="actSubType"></param>
        public virtual void Purge(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DataAccessStatusOutHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="statMessage"></param>
        /// <param name="overrideDisconnectedMode"></param>
        public void DataAccessStatusOutHandler(
        object sender,
        EventArgs e,
        string statMessage,
        bool overrideDisconnectedMode)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || overrideDisconnectedMode)
            {
                if (this.OnDataAccessStatusOut != null)
                {
                    this.DataEventArgs.StatusOutMsg = statMessage;
                    this.DataEventArgs.OverrideDisconnectedMode = overrideDisconnectedMode;
                    this.OnDataAccessStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// The DataAccessProgressOutHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="currentValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public void DataAccessProgressOutHandler(
        object sender,
        EventArgs e,
        int currentValue,
        int minValue,
        int maxValue)
        {
            if (this.OnDataAccessProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.MinValue = minValue;
                this.DataEventArgs.MaxValue = maxValue;
                this.DataEventArgs.SetMinMaxValue = true;
                this.OnDataAccessProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// The DataAccessProgressOutHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="currentValue"></param>
        public void DataAccessProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (this.OnDataAccessProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataAccessProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// The RegisterDataAccessListenerStatusOut
        /// </summary>
        /// <param name="dataAccessEventListener"></param>
        public void RegisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener)
        {
            this.OnDataAccessStatusOut += new OnDataAccessStatusOutEventHandler(dataAccessEventListener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="dataAccessEventListener"></param>
        public void UnregisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener)
        {
            this.OnDataAccessStatusOut -= new OnDataAccessStatusOutEventHandler(dataAccessEventListener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// The RegisterDataAccessListenerProgressOut
        /// </summary>
        /// <param name="dataAccessEventListener"></param>
        public void RegisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener)
        {
            this.OnDataAccessProgressOut += new OnDataAccessProgressOutEventHandler(
                dataAccessEventListener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// The UnregisterDataAccessListenerProgressOut
        /// </summary>
        /// <param name="dataAccessEventListener"></param>
        public void UnregisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener)
        {
            this.OnDataAccessProgressOut -= new OnDataAccessProgressOutEventHandler(
                dataAccessEventListener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected virtual string SendRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno"></param>
        /// <returns></returns>
        protected virtual async Task<string> Send(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<string> Send()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual async Task<string> Send(string tableName)
        {
            return this.SendComm(this.Create());
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="criteria">The criteria<see cref="XDocument"/></param>
        /// <returns></returns>
        protected virtual async Task<string> SendRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="actRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual async Task<string> SendRequest(int actRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected virtual string SendRequest(ref string sendData, string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <param name="whsLocMapFKRefno">The whsLocMapFKRefno<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        protected virtual string SendRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <returns></returns>
        protected virtual string SendRequest(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="requestData"></param>
        protected virtual void Save(string requestData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="refno">The refno<see cref="int"/></param>
        protected virtual void Save(string requestData, int refno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SaveData
        /// </summary>
        /// <param name="requestData"></param>
        protected virtual void SaveData(ref string requestData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <returns></returns>
        protected virtual string Create()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="criteria">The criteria<see cref="XDocument"/></param>
        /// <returns></returns>
        protected virtual string CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected virtual string CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="actFKRefno"></param>
        /// <returns></returns>
        protected virtual string Create(int actFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="actFKRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual string CreateRequest(int actFKRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <param name="whsLocMapFKRefno"></param>
        /// <returns></returns>
        protected virtual string CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateRequest
        /// </summary>
        /// <param name="prodLocMapType"></param>
        /// <returns></returns>
        protected virtual string CreateRequest(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SaveCommonDetailVariant
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="drSchemeVar"></param>
        /// <param name="recSchemeVarXRef"></param>
        /// <param name="actDetailRefno"></param>
        /// <param name="useFNo"></param>
        /// <returns></returns>
        protected virtual bool SaveCommonDetailVariant(DataRow drow, ref SQLiteConnection conn, ref SQLiteCommand cmndSchemeVarXRef, int actDetailRefno, bool useFNo)
        {
            int schemeVariantNo = 0;
            bool isNeedToSave = false;

            if (useFNo)
            {
                schemeVariantNo = Convert.ToInt32(drow["f17"]);
            }
            else
            {
                schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
            }

            if (0 < schemeVariantNo)
            {
                using (SQLiteCommand cmndSchemeVar = conn.CreateCommand())
                {
                    cmndSchemeVar.CommandText = "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = @productSchemeVariantNo";
                    cmndSchemeVar.CommandType = CommandType.Text;
                    cmndSchemeVar.Parameters.Add(new SQLiteParameter("@productSchemeVariantNo", Convert.ToInt32(schemeVariantNo)));

                    using (SQLiteDataReader drSchemeVar = cmndSchemeVar.ExecuteReader())
                    {
                        if (drSchemeVar.Read())
                        {
                            cmndSchemeVarXRef.CommandText = "INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (@activityDetailRefno, @productSchemeVariantRefno)";
                            cmndSchemeVarXRef.CommandType = CommandType.Text;
                            cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@activityDetailRefno", Convert.ToInt32(actDetailRefno)));
                            cmndSchemeVarXRef.Parameters.Add(new SQLiteParameter("@productSchemeVariantRefno", drSchemeVar.GetInt32(Convert.ToInt32(SysCommon.ProdSchemeVariantFields.Refno))));

                            //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno, actDetailRefno);
                            //recSchemeVarXRef.SetInt32((int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno, drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno));

                            isNeedToSave = true;
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("drSchemeVar.Read() failed. " + string.Format("Unable to locate product scheme no {0} in tProductSchemeVariant", schemeVariantNo));
                        }
                    }
                }
            }

            return isNeedToSave;
        }

        /// <summary>
        /// The SaveDetailExtra
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="recExtra"></param>
        /// <param name="actDetailRefno"></param>
        /// <param name="useFNo"></param>
        protected virtual void SaveDetailExtra(
        DataRow drow,
        ref SQLiteCommand cmndExtra,
        int actDetailRefno,
        bool useFNo)
        {
            string emptyDate = "01-01-1900";

            cmndExtra.CommandText = "INSERT INTO tActivityDetailExtra (" +
            "ActivityDetailRefno, " +
            "ActivityDetailSubType, " +
            "ActivityDetailExtra1Text, " +
            "ActivityDetailExtra2Text, " +
            "ActivityDetailExtra3Text, " +
            "ActivityDetailExtra4Text, " +
            "ActivityDetailExtra5Date" +
            ") VALUES (" +
            "@activityDetailRefno, " +
            "@activityDetailSubType, " +
            "@activityDetailExtra1Text, " +
            "@activityDetailExtra2Text, " +
            "@activityDetailExtra3Text, " +
            "@activityDetailExtra4Text, " +
            "@activityDetailExtra5Date" +
            ")";
            cmndExtra.CommandType = CommandType.Text;
            cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailRefno", actDetailRefno));
            cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailSubType", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts)));

            //cmndExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailRefno, actDetailRefno);
            //cmndExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailSubType, (int)SysCommon.ActSubTypes.CycleCounts);

            if (useFNo)
            {
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra1Text", drow["f19"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra2Text", drow["f20"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra3Text", drow["f21"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra4Text", drow["f22"].ToString()));
                
                if (string.IsNullOrEmpty(drow["f23"].ToString()))
                {
                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", DateTime.Parse(emptyDate)));
                }
                else
                {
                    //recExtra.SetDateTime((int)SysCommon.ActDetailExtraFields.DateTime5, Convert.ToDateTime(drow["f23"]));
                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", Convert.ToDateTime(drow["f23"])));
                }

                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["f19"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["f20"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["f21"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["f22"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["f23"].ToString());
            }
            else
            {
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra1Text", drow["ActivityDetailExtra1"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra2Text", drow["ActivityDetailExtra2"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra3Text", drow["ActivityDetailExtra3"].ToString()));
                cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra4Text", drow["ActivityDetailExtra4"].ToString()));
                
                if (string.IsNullOrEmpty(drow["ActivityDetailExtra5"].ToString()))
                {
                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", DateTime.Parse(emptyDate)));
                }
                else
                {
                    cmndExtra.Parameters.Add(new SQLiteParameter("@activityDetailExtra5Date", Convert.ToDateTime(drow["ActivityDetailExtra5"])));
                }

                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["ActivityDetailExtra1"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["ActivityDetailExtra2"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["ActivityDetailExtra3"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["ActivityDetailExtra4"].ToString());
                //cmndExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["ActivityDetailExtra5"].ToString());
            }

            cmndExtra.ExecuteNonQuery();
        }

        /// <summary>
        /// The SaveDetailLocation
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="drLocMapSlot"></param>
        /// <param name="drLocMapSection"></param>
        /// <param name="drLocMapWhs"></param>
        /// <param name="recDetail"></param>
        /// <param name="useFNo"></param>
        protected virtual void SaveDetailLocation(DataRow drow, ref SQLiteConnection conn, ref SQLiteCommand cmndDetail, bool useFNo)
        {
            int locMapRefno = 0;
            int locMapWhsFKRefno = 0;
            int locMapSectFKRefno = 0;
            int locMapSlotFKRefno = 0;

            if (useFNo)
            {
                locMapWhsFKRefno = Convert.ToInt32(drow["f07"]);
                locMapSectFKRefno = Convert.ToInt32(drow["f08"]);
                locMapSlotFKRefno = Convert.ToInt32(drow["f09"]);
            }
            else
            {
                locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
            }

            if (locMapRefno == 0)
            {
                using (SQLiteCommand cmndLocMapSlot = conn.CreateCommand())
                {
                    cmndLocMapSlot.CommandText = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @locationMapSlotFKRefno";
                    cmndLocMapSlot.CommandType = CommandType.Text;
                    cmndLocMapSlot.Parameters.Add(new SQLiteParameter("@locationMapSlotFKRefno", locMapSlotFKRefno));

                    using (SQLiteDataReader drLocMapSlot = cmndLocMapSlot.ExecuteReader())
                    {
                        if (drLocMapSlot.Read())
                        {
                            locMapRefno = Convert.ToInt32(drLocMapSlot.GetValue(Convert.ToInt32(SysCommon.LocMapSlotFields.LocMapRefno)));
                        }
                    }
                }
            }

            if (locMapRefno == 0)
            {
                using (SQLiteCommand cmndLocMapSect = conn.CreateCommand())
                {
                    cmndLocMapSect.CommandText = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @locationMapSectionFKRefno";
                    cmndLocMapSect.CommandType = CommandType.Text;
                    cmndLocMapSect.Parameters.Add(new SQLiteParameter("@locationMapSectionFKRefno", locMapSlotFKRefno));

                    using (SQLiteDataReader drLocMapSection = cmndLocMapSect.ExecuteReader())
                    {
                        if (drLocMapSection.Read())
                        {
                            locMapRefno = Convert.ToInt32(drLocMapSection.GetValue(Convert.ToInt32(SysCommon.LocMapSectFields.LocMapRefno)));
                        }
                    }
                }
            }

            if (locMapRefno == 0)
            {
                using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
                {
                    cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                    cmndLocMapWhs.CommandType = CommandType.Text;
                    cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", locMapWhsFKRefno));

                    using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                    {
                        if (drLocMapWhs.Read())
                        {
                            locMapRefno = Convert.ToInt32(drLocMapWhs.GetValue(Convert.ToInt32(SysCommon.LocMapWhsFields.LocMapRefno)));
                        }
                    }
                }
            }

            if (locMapRefno == 0)
            {
                throw new Exception(string.Format("Unable to identify the location map refno. {0}Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
                    Environment.NewLine,
                    locMapWhsFKRefno,
                    Environment.NewLine,
                    locMapSectFKRefno,
                    Environment.NewLine,
                    locMapSlotFKRefno));
            }

            cmndDetail.Parameters.Add(new SQLiteParameter("@locationMapRefno", Convert.ToInt32(locMapRefno)));
        }

        /// <summary>
        /// The SaveDetail
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="detail"></param>
        /// <param name="useFNo"></param>
        protected virtual void SaveDetail(DataRow drow, ref SQLiteCommand detail, bool useFNo)
        {
            string emptyDate = "01-01-1900";

            detail.Parameters.Add(new SQLiteParameter("@activityDetailStatus", Convert.ToInt32(0)));
            //detail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);

            if (useFNo)
            {
                detail.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", Convert.ToString(drow["f05"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailSchemeID", Convert.ToString(drow["f06"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailQuantity", Convert.ToDecimal(drow["f02"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailBaseQuantity", Convert.ToDecimal(drow["f03"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailWeightQuantity", Convert.ToDecimal(drow["f04"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailSingleTradeItemSerialNumber", Convert.ToString(drow["f10"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBatchLotNumber", Convert.ToString(drow["f11"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitSkidNumber", Convert.ToString(drow["f18"])));

                //detail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, drow["f05"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.SchemeID, drow["f06"].ToString());
                //detail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, Convert.ToDecimal(drow["f02"]));
                //detail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, Convert.ToDecimal(drow["f03"]));
                //detail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, Convert.ToDecimal(drow["f04"]));
                //detail.SetString((int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber, drow["f10"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber, drow["f11"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitSkidNumber, drow["f18"].ToString());

                if (string.IsNullOrEmpty(Convert.ToString(drow["f13"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", Convert.ToDateTime(drow["f13"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, Convert.ToDateTime(drow["f13"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f14"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", Convert.ToDateTime(drow["f14"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, Convert.ToDateTime(drow["f14"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f12"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", Convert.ToDateTime(drow["f12"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, Convert.ToDateTime(drow["f12"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f15"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", Convert.ToDateTime(drow["f15"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, Convert.ToDateTime(drow["f15"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f16"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", Convert.ToDateTime(drow["f16"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, Convert.ToDateTime(drow["f16"]));
                }
            }
            else
            {
                detail.Parameters.Add(new SQLiteParameter("@activityDetailUniqueSchemeID", Convert.ToString(drow["ActivityDetailUniqueSchemeID"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailSchemeID", Convert.ToString(drow["ActivityDetailSchemeID"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailQuantity", Convert.ToDecimal(drow["ActivityDetailQuantity"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailBaseQuantity", Convert.ToDecimal(drow["ActivityDetailBaseQuantity"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailWeightQuantity", Convert.ToDecimal(drow["ActivityDetailWeightQuantity"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailSingleTradeItemSerialNumber", Convert.ToString(drow["ActivityDetailSingleTradeItemSerialNumber"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBatchLotNumber", Convert.ToString(drow["ActivityDetailLogisticUnitBatchLotNumber"])));
                detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitSkidNumber", Convert.ToString(drow["ActivityDetailLogisticUnitSkidCode"])));


                //detail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, drow["ActivityDetailUniqueSchemeID"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.SchemeID, drow["ActivityDetailSchemeID"].ToString());
                //detail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, Convert.ToDecimal(drow["ActivityDetailQuantity"]));
                //detail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));
                //detail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));
                //detail.SetString((int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber, drow["ActivityDetailSingleTradeItemSerialNumber"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber, drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString());
                //detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitSkidNumber, drow["ActivityDetailLogisticUnitSkidCode"].ToString());

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitBestBeforeSellByDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitExpiryUseByDate"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitExpiryUseByDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitProductionDate"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitProductionDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitProductionDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitDueDate"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitDueDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitDueDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitPackagingDate"])))
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", DateTime.Parse(emptyDate)));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, DateTime.Parse(emptyDate));
                }
                else
                {
                    detail.Parameters.Add(new SQLiteParameter("@activityDetailLogisticUnitPackagingDate", Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"])));
                    //detail.SetDateTime((int)SysCommon.ActDetailFields.LogisticUnitPackagingDate, Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                }
            }
        }

        /// <summary>
        /// The SendComm
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        protected virtual string SendComm(string sendData)
        {
            return SysCommon.SubmitComm(sendData);
        }

        /// <summary>
        /// The GetWhsLocMapRefnoFromDataRow
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="errMsg"></param>
        /// <param name="drWhs"></param>
        /// <returns></returns>
        protected virtual int GetWhsLocMapRefnoFromDataRow(DataRow drow, ref string errMsg, SQLiteCommand cmndLocMapWhs)
        {
            int locMapRefno = 0;
            int locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
            errMsg += Environment.NewLine + "Whs " + locMapWhsFKRefno;

            cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
            cmndLocMapWhs.CommandType = CommandType.Text;
            //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
            cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(locMapWhsFKRefno)));

            using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
            {
                //if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                //{
                if (drLocMapWhs.Read())
                {
                    locMapRefno = drLocMapWhs.GetInt32(Convert.ToInt32(SysCommon.LocMapWhsFields.LocMapRefno));
                }
                else
                {
                    // TODO: Resource String
                    throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
                }
                //}

                if (locMapRefno == 0)
                {
                    // TODO: Resource String
                    throw new Exception("Unable to locate the location map refno." + Environment.NewLine + errMsg);
                }
            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetProductUnitRefnoFromDataRow
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="errMsg"></param>
        /// <param name="drProduct"></param>
        /// <param name="drUnit"></param>
        /// <param name="drProdUnit"></param>
        /// <param name="useFNo">The useFNo<see cref="bool"/></param>
        /// <returns></returns>
        protected virtual int GetProductUnitRefnoFromDataRow(DataRow drow, ref SQLiteConnection conn, ref string errMsg, bool useFNo)
        {
            using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
            using (SQLiteCommand cmndProd = conn.CreateCommand())
            using (SQLiteCommand cmndUnit = conn.CreateCommand())
            {
                int prodRefno = 0;
                int prodFKRefno = 0;
                string unitCode = string.Empty;
                int unitRefno = 0;
                int prodUnitRefno = 0;
                bool prodUnitIsBC = false;

                if (useFNo)
                {
                    prodFKRefno = Convert.ToInt32(drow["f03"]);
                    unitCode = Convert.ToString(drow["f05"]);
                    prodUnitIsBC = Convert.ToBoolean(drow["f04"]);
                }
                else
                {
                    prodFKRefno = Convert.ToInt32(drow["ProductFKRefno"]);
                    unitCode = Convert.ToString(drow["UnitCode"]);
                    prodUnitIsBC = Convert.ToBoolean(drow["ProductUnitIsBC"]);
                }

                errMsg += Environment.NewLine + "Prod fk refno " + prodFKRefno;
                errMsg += Environment.NewLine + "Unit \'" + unitCode + "\'";
                errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

                cmndProd.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                cmndProd.CommandType = CommandType.Text;
                cmndProd.Parameters.Add(new SQLiteParameter("@productFKRefno", Convert.ToInt32(prodFKRefno)));

                using (SQLiteDataReader drProduct = cmndProd.ExecuteReader())
                {
                    if (drProduct.Read())
                    {
                        prodRefno = drProduct.GetInt32(Convert.ToInt32(SysCommon.ProductFields.Refno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drProduct.Read() failed. Unable to locate the product refno from the product fk refno in tProduct." + Environment.NewLine + errMsg);
                    }
                }

                cmndUnit.CommandText = "SELECT * FROM tUnit WHERE UnitCode = @unitCode";
                cmndUnit.CommandType = CommandType.Text;
                cmndUnit.Parameters.Add(new SQLiteParameter("@unitCode", unitCode));

                using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
                {
                    if (drUnit.Read())
                    {
                        unitRefno = drUnit.GetInt32(Convert.ToInt32(SysCommon.UnitFields.UnitRefno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
                    }
                }

                cmndProdUnit.CommandText = "SELECT * FROM tProductUnit WHERE ProductRefno = @productRefno AND UnitRefno = @unitRefno AND ProductUnitIsBC = @productUnitIsBC";
                cmndProdUnit.CommandType = CommandType.Text;
                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(prodRefno)));
                cmndProdUnit.Parameters.Add(new SQLiteParameter("@unitRefno", Convert.ToInt32(unitRefno)));
                cmndProdUnit.Parameters.Add(new SQLiteParameter("@productUnitIsBC", Convert.ToBoolean(prodUnitIsBC)));

                using (SQLiteDataReader drProductUnit = cmndProdUnit.ExecuteReader())
                {
                    if (drProductUnit.Read())
                    {
                        prodUnitRefno = drProductUnit.GetInt32(Convert.ToInt32(SysCommon.ProdUnitFields.Refno));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("drProdUnit.Read() failed. Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit." + Environment.NewLine + errMsg);
                    }
                }

                return prodUnitRefno;
            }
        }
    }
}
