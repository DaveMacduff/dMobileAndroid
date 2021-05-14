namespace dMobile.GroundWork
{
    using dMobile.Bedrock;
    using System;
    using System.Data;
    using SQLite;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

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
        public virtual Task Request()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Request
        /// </summary>
        /// <param name="actRefno"></param>
        public virtual Task Request(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="criteria"></param>
        public virtual Task RequestData(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The RequestData
        /// </summary>
        /// <param name="actRefno"></param>
        /// <param name="criteria"></param>
        public virtual Task RequestData(int actRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge
        /// </summary>
        public virtual Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PurgeData
        /// </summary>
        /// <param name="sectLocMapRefno"></param>
        public virtual Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Purge
        /// </summary>
        /// <param name="actSubType"></param>
        public virtual Task Purge(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DataStatus
        /// </summary>
        /// <returns></returns>
        public virtual Task<int> DataStatus()
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
        protected virtual Task<string> SendRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="actRefno"></param>
        /// <returns></returns>
        protected virtual Task<string> Send(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <returns></returns>
        protected virtual Task<string> Send()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Send
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual Task<string> Send(string tableName)
        {
            return this.SendComm(this.Create());
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="criteria">The criteria<see cref="XDocument"/></param>
        /// <returns></returns>
        protected virtual Task<string> SendRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SendRequest
        /// </summary>
        /// <param name="actRefno"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual Task<string> SendRequest(int actRefno, XDocument criteria)
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
        protected virtual Task Save(string requestData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="refno">The refno<see cref="int"/></param>
        protected virtual Task Save(string requestData, int refno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SaveData
        /// </summary>
        /// <param name="requestData"></param>
        protected virtual Task SaveData(ref string requestData)
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
        protected virtual void SaveCommonDetailVariant(DataRow drow, SQLiteConnection conn, int actDetailRefno, bool useFNo)
        {
            int schemeVariantNo = 0;

            if (useFNo)
            {
                schemeVariantNo = Convert.ToInt32(drow["f17"]);
            }
            else
            {
                schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
            }

            if (schemeVariantNo > 0)
            {
                List<tProductSchemeVariantModel> results = conn.Query<tProductSchemeVariantModel>("SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?", Convert.ToInt32(schemeVariantNo));

                if (results != null && results.Count > 0)
                {
                    conn.Execute("INSERT INTO tActivityDetailProductSchemeVariantXRef (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)", Convert.ToInt32(actDetailRefno), results[0].ProductSchemeVariantRefno);
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception("drSchemeVar.Read() failed. " + string.Format("Unable to locate product scheme no {0} in tProductSchemeVariant", schemeVariantNo));
                }
            }
        }

        /// <summary>
        /// The SaveDetailExtra
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="recExtra"></param>
        /// <param name="actDetailRefno"></param>
        /// <param name="useFNo"></param>
        protected virtual void SaveDetailExtra(DataRow drow, SQLiteConnection conn, int actDetailRefno, bool useFNo)
        {
            string emptyDate = "01-01-1900";
            List<object> parameters = new List<object>();

            string query = "INSERT INTO tActivityDetailExtra (" +
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

            parameters.Add(actDetailRefno);
            parameters.Add(Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));

            if (useFNo)
            {
                parameters.Add(Convert.ToString(drow["f19"]));
                parameters.Add(Convert.ToString(drow["f20"]));
                parameters.Add(Convert.ToString(drow["f21"]));
                parameters.Add(Convert.ToString(drow["f22"]));

                if (string.IsNullOrEmpty(Convert.ToString(drow["f23"])))
                {
                    parameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    parameters.Add(Convert.ToDateTime(drow["f23"]));
                }
            }
            else
            {
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra1"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra2"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra3"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra4"]));

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailExtra5"])))
                {
                    parameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    parameters.Add(Convert.ToDateTime(drow["ActivityDetailExtra5"]));
                }
            }

            conn.Execute(query, parameters);
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
        protected virtual int SaveDetailLocation(DataRow drow, SQLiteConnection conn, bool useFNo)
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
                List<tLocationMapSlotModel> results = conn.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapSlotFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                List<tLocationMapSectionModel> results = conn.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapSectFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                List<tLocationMapWarehouseModel> results = conn.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapWhsFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
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

            return Convert.ToInt32(locMapRefno);
        }

        /// <summary>
        /// The SaveDetail
        /// </summary>
        /// <param name="drow"></param>
        /// <param name="detail"></param>
        /// <param name="useFNo"></param>
        protected virtual List<object> SaveDetail(DataRow drow, SQLiteConnection detail, bool useFNo)
        {
            string emptyDate = "01-01-1900";
            List<object> detailParameters = new List<object>();

            detailParameters.Add(Convert.ToInt32(0));

            if (useFNo)
            {
                detailParameters.Add(Convert.ToString(drow["f05"]));
                detailParameters.Add(Convert.ToString(drow["f06"]));
                detailParameters.Add(Convert.ToDecimal(drow["f02"]));
                detailParameters.Add(Convert.ToDecimal(drow["f03"]));
                detailParameters.Add(Convert.ToDecimal(drow["f04"]));
                detailParameters.Add(Convert.ToString(drow["f10"]));
                detailParameters.Add(Convert.ToString(drow["f11"]));

                if (string.IsNullOrEmpty(Convert.ToString(drow["f13"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["f13"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f14"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["f14"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f12"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["f12"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f15"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["f15"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["f16"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["f16"]));
                }

                detailParameters.Add(Convert.ToString(drow["f18"]));
            }
            else
            {
                detailParameters.Add(Convert.ToString(drow["ActivityDetailUniqueSchemeID"]));
                detailParameters.Add(Convert.ToString(drow["ActivityDetailSchemeID"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailQuantity"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));
                detailParameters.Add(Convert.ToString(drow["ActivityDetailSingleTradeItemSerialNumber"]));
                detailParameters.Add(Convert.ToString(drow["ActivityDetailLogisticUnitBatchLotNumber"]));

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitExpiryUseByDate"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitProductionDate"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitDueDate"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
                }

                if (string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailLogisticUnitPackagingDate"])))
                {
                    detailParameters.Add(DateTime.Parse(emptyDate));
                }
                else
                {
                    detailParameters.Add(Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
                }

                detailParameters.Add(Convert.ToString(drow["ActivityDetailLogisticUnitSkidCode"]));
            }

            return detailParameters;
        }

        /// <summary>
        /// The SendComm
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        protected virtual async Task<string> SendComm(string sendData)
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
        protected virtual int GetWhsLocMapRefnoFromDataRow(DataRow drow, ref string errMsg, SQLiteConnection conn)
        {
            int locMapRefno = 0;
            int locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
            errMsg += Environment.NewLine + "Whs " + locMapWhsFKRefno;

            List<tLocationMapWarehouseModel> results = conn.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(locMapWhsFKRefno));

            if (results != null && results.Count > 0)
            {
                locMapRefno = results[0].LocationMapRefno;
            }
            else
            {
                // TODO: Resource String
                throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
            }

            if (locMapRefno == 0)
            {
                // TODO: Resource String
                throw new Exception("Unable to locate the location map refno." + Environment.NewLine + errMsg);
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
        protected virtual int GetProductUnitRefnoFromDataRow(DataRow drow, SQLiteConnection conn, ref string errMsg, bool useFNo)
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

            List<tProductModel> productResults = conn.Query<tProductModel>("SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

            if (productResults != null && productResults.Count > 0)
            {
                prodRefno = Convert.ToInt32(productResults[0].ProductRefno);
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drProduct.Read() failed. Unable to locate the product refno from the product fk refno in tProduct." + Environment.NewLine + errMsg);
            }

            List<tUnitModel> unitResults = conn.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode);

            if (unitResults != null && unitResults.Count > 0)
            {
                unitRefno = Convert.ToInt32(unitResults[0].UnitRefno);
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
            }

            List<tProductUnitModel> productUnitResults = conn.Query<tProductUnitModel>("SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND ProductUnitIsBC = ?", Convert.ToInt32(prodRefno), Convert.ToInt32(unitRefno), Convert.ToBoolean(prodUnitIsBC));

            if (productUnitResults != null && productUnitResults.Count > 0)
            {
                prodUnitRefno = Convert.ToInt32(productUnitResults[0].ProductUnitRefno);
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("drProdUnit.Read() failed. Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit." + Environment.NewLine + errMsg);
            }

            return prodUnitRefno;
        }
    }
}
