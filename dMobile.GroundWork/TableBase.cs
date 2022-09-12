// <copyright file="TableBase.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// <see cref="TableBase" /> Table base class.
    /// </summary>
    public abstract class TableBase : IDataAccessEventPublisher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableBase"/> class.
        /// </summary>
        public TableBase()
        {
            this.SysEnv = SysEnvironment.GetInstance();
        }

        /// <summary>
        /// On data access status out event handler.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void OnDataAccessStatusOutEventHandler(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// On data access progress out event handler.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void OnDataAccessProgressOutEventHandler(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// Define the on access status out event.
        /// </summary>
        public event OnDataAccessStatusOutEventHandler OnDataAccessStatusOut;

        /// <summary>
        /// Define the on access progress out event.
        /// </summary>
        public event OnDataAccessProgressOutEventHandler OnDataAccessProgressOut;

        /// <summary>
        /// Gets or sets the data access event arguments.
        /// </summary>
        public DataAccessEventArgs DataEventArgs { get; set; } = new DataAccessEventArgs();

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = "TableBase";

        /// <summary>
        /// Gets or sets the system environment.
        /// </summary>
        public SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task RequestData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <param name="criteria"><see cref="XDocument"/> Filter detail used in the comm server.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public virtual Task RequestData(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Filter detail used in the comm server.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public virtual Task RequestData(int actRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task PurgeData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <param name="sectLocMapRefno"><see cref="int"/> Section location map reference number.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <param name="actSubType"> .</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the data status.
        /// </summary>
        /// <returns>.</returns>
        public virtual Task<int> DataStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Data access status out event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="statMessage"><see cref="string"/> Status message.</param>
        /// <param name="overrideDisconnectedMode"><see cref="bool"/> Flag to override disconnected mode.</param>
        public void DataAccessStatusOutHandler(
            object sender,
            EventArgs e,
            string statMessage,
            bool overrideDisconnectedMode)
        {
            sender = sender ?? throw new ArgumentNullException(nameof(sender));
            e = e ?? throw new ArgumentNullException(nameof(e));

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
        /// Data access progress out event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress index value.</param>
        /// <param name="minValue"><see cref="int"/> Minimum / starting progress value.</param>
        /// <param name="maxValue"><see cref="int"/> Maximum / last progress value.</param>
        public void DataAccessProgressOutHandler(
            object sender,
            EventArgs e,
            int currentValue,
            int minValue,
            int maxValue)
        {
            sender = sender ?? throw new ArgumentNullException(nameof(sender));
            e = e ?? throw new ArgumentNullException(nameof(e));

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
        /// Data access progress out event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress index value.</param>
        public void DataAccessProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            sender = sender ?? throw new ArgumentNullException(nameof(sender));
            e = e ?? throw new ArgumentNullException(nameof(e));

            if (this.OnDataAccessProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataAccessProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Register the data access status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void RegisterDataAccessListenerStatusOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessStatusOut += new OnDataAccessStatusOutEventHandler(
                listener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// Unregister the data access status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerStatusOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessStatusOut -= new OnDataAccessStatusOutEventHandler(
                listener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// Register the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void RegisterDataAccessListenerProgressOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessProgressOut += new OnDataAccessProgressOutEventHandler(
                listener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// Unregister the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerProgressOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessProgressOut -= new OnDataAccessProgressOutEventHandler(
                listener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code filter.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequest(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual async Task<string> SendRequest()
        {
            return await this.SendComm(await this.CreateRequest());
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequest(int actRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="whsLocMapFKRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual Task<string> SendRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save received data.
        /// </summary>
        /// <param name="receivedData"><see cref="string"/> Received data to save.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        protected virtual Task Save(string receivedData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save received data.
        /// </summary>
        /// <param name="receivedData"><see cref="string"/> Received data to save.</param>
        /// <param name="refno"><see cref="int"/> Refno.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        protected virtual Task Save(string receivedData, int refno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="actFKRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(int actFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="actFKRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(int actFKRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="whsLocMapFKRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the common detail product scheme variant information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="actDetailRefno"><see cref="int"/> Activity detail reference number.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        protected virtual void SaveCommonDetailVariant(
            DataRow drow,
            SQLiteConnection conn,
            int actDetailRefno,
            bool useFNumberElements)
        {
            int schemeVariantNo;
            if (useFNumberElements)
            {
                schemeVariantNo = Convert.ToInt32(drow["f17"]);
            }
            else
            {
                schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
            }

            if (schemeVariantNo > 0)
            {
                var results = conn.Query<tProductSchemeVariantModel>(
                    "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                    Convert.ToInt32(schemeVariantNo));

                if (results != null && results.Count > 0)
                {
                    conn.Execute(
                        "INSERT INTO tActivityDetailProductSchemeVariantXRef (" +
                        "ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)",
                        Convert.ToInt32(actDetailRefno),
                        results[0].ProductSchemeVariantRefno);
                }
                else
                {
                    throw new Exception(string.Format(
                        "Unable to locate product scheme no {0} in tProductSchemeVariant",
                        schemeVariantNo));
                }
            }
        }

        /// <summary>
        /// Save activity detail extra information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="actDetailRefno"><see cref="int"/> Activity detail reference number.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        protected virtual void SaveDetailExtra(
            DataRow drow,
            SQLiteConnection conn,
            int actDetailRefno,
            bool useFNumberElements)
        {
            var emptyDate = "01-01-1900";
            var parameters = new List<object>();

            var query = "INSERT INTO tActivityDetailExtra (" +
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

            if (useFNumberElements)
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

            conn.Execute(query, parameters.ToArray());
        }

        /// <summary>
        /// get the activity detail location information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        /// <returns>The <see cref="int"/> Identified location map reference number.</returns>
        protected virtual int GetDetailLocation(
            DataRow drow,
            SQLiteConnection conn,
            bool useFNumberElements)
        {
            var locMapRefno = 0;
            int locMapWhsFKRefno;
            int locMapSectFKRefno;
            int locMapSlotFKRefno;
            if (useFNumberElements)
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
                var results = conn.Query<tLocationMapSlotModel>(
                    "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                    locMapSlotFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                var results = conn.Query<tLocationMapSectionModel>(
                    "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                    locMapSectFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                var results = conn.Query<tLocationMapWarehouseModel>(
                    "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                    locMapWhsFKRefno);

                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                throw new Exception(string.Format(
                    "Unable to identify the location map reference number. {0}Whs FK Refno {1}{2}Section" +
                        " FK Refno {3}{4} Slot FK Refno {5}{6}",
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
        /// Get the activity detail information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        /// <returns>The <see cref="List{object}"/> Activity detail information.</returns>
        protected virtual List<object> SaveDetail(
            DataRow drow,
            SQLiteConnection conn,
            bool useFNumberElements)
        {
            var emptyDate = "01-01-1900";
            var detailParameters = new List<object>();

            if (useFNumberElements)
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
        /// Send the communication request.
        /// </summary>
        /// <param name="sendData"><see cref="string"/> Communication request.</param>
        /// <returns><see cref="string"/> Communication result.</returns>
        protected virtual async Task<string> SendComm(string sendData)
        {
            return SysCommon.SubmitComm(sendData);
        }

        /// <summary>
        /// Get the warehouse location map refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Row of data.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQL connection.</param>
        /// <returns>.</returns>
        protected virtual int GetWhsLocMapRefnoFromDataRow(
            DataRow drow,
            SQLiteConnection conn)
        {
            var locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);

            var results = conn.Query<tLocationMapWarehouseModel>(
                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                Convert.ToInt32(locMapWhsFKRefno));

            int locMapRefno;
            if (results != null && results.Count > 0)
            {
                locMapRefno = results[0].LocationMapRefno;
            }
            else
            {
                throw new Exception("drWhs.Read() failed.");
            }

            if (locMapRefno == 0)
            {
                throw new Exception(
                    "Unable to locate the location map reference number.");
            }

            return locMapRefno;
        }

        /// <summary>
        /// Get the product unit refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row.</param>
        /// <param name="conn">The conn<see cref="SQLiteConnection"/> SQL connection.</param>
        /// <param name="errMsg">.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        /// <returns>.</returns>
        protected virtual int GetProductUnitRefnoFromDataRow(
            DataRow drow,
            SQLiteConnection conn,
            bool useFNumberElements)
        {
            int prodFKRefno;
            string unitCode;
            bool prodUnitIsBC;
            if (useFNumberElements)
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

            var productResults = conn.Query<tProductModel>(
                "SELECT * FROM tProduct WHERE ProductFKRefno = ?", Convert.ToInt32(prodFKRefno));

            int prodRefno;
            if (productResults != null && productResults.Count > 0)
            {
                prodRefno = Convert.ToInt32(productResults[0].ProductRefno);
            }
            else
            {
                throw new Exception(
                    "drProduct.Read() failed. Unable to locate the product refno" +
                    " from the product fk refno in tProduct.");
            }

            var unitResults = conn.Query<tUnitModel>(
                "SELECT * FROM tUnit WHERE UnitCode = ?", unitCode);
            int unitRefno;
            if (unitResults != null && unitResults.Count > 0)
            {
                unitRefno = Convert.ToInt32(unitResults[0].UnitRefno);
            }
            else
            {
                throw new Exception("drUnit.Read() failed.");
            }

            var productUnitResults = conn.Query<tProductUnitModel>(
                "SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND" +
                    " ProductUnitIsBC = ?", Convert.ToInt32(prodRefno),
                Convert.ToInt32(unitRefno),
                Convert.ToBoolean(prodUnitIsBC));
            int prodUnitRefno;
            if (productUnitResults != null && productUnitResults.Count > 0)
            {
                prodUnitRefno = Convert.ToInt32(productUnitResults[0].ProductUnitRefno);
            }
            else
            {
                throw new Exception(
                    "drProdUnit.Read() failed. Unable to locate the product unit refno from the" +
                    " product fk refno, bc and unit code in tProductUnit.");
            }

            return prodUnitRefno;
        }
    }
}
