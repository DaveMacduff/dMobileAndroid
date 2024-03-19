// <copyright file="TableBase.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml;
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
        protected TableBase()
        {
            this.SysEnv = SysEnvironment.GetInstance();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBase"/> class.
        /// </summary>
        /// <param name="subType"> Current activity sub type.</param>
        protected TableBase(SysCommon.ActSubTypes subType)
        {
            this.SysEnv = SysEnvironment.GetInstance();
            this.CurrentSubType = subType;
        }

        /// <summary>
        /// On data access status out event handler.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void DataAccessStatusOutEventHandler(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// On data access progress out event handler.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void DataAccessProgressOutEventHandler(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs);

        /// <summary>
        /// Define the on access status out event.
        /// </summary>
        public event DataAccessStatusOutEventHandler OnDataAccessStatusOut;

        /// <summary>
        /// Define the on access progress out event.
        /// </summary>
        public event DataAccessProgressOutEventHandler OnDataAccessProgressOut;

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        protected string FullClassName { get; set; } = "TableBase";

        /// <summary>
        /// Gets or sets the message text activity lower case name.
        /// </summary>
        protected string LowerCaseName { get; set; } = "activity";

        /// <summary>
        /// Gets or sets the message text activity proper case name.
        /// </summary>
        protected string ProperCaseName { get; set; } = "Activity";

        /// <summary>
        /// Gets or sets the system environment.
        /// </summary>
        protected SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the table base current activity sub type.
        /// </summary>
        protected SysCommon.ActSubTypes CurrentSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets the data access event arguments.
        /// </summary>
        private DataAccessEventArgs DataEventArgs { get; set; } = new DataAccessEventArgs();

        /// <summary>
        /// The Request Header Data.
        /// </summary>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public virtual async Task RequestHeaderData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendComm(await this.CreateRequest());
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);

                string msg = $"Save {this.LowerCaseName} to database failed.{Environment.NewLine}Reason: {ex.Message}.";
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
        }

        /// <summary>
        /// The Request Header Data.
        /// </summary>
        /// <param name="criteria"> Filter criteria.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public virtual async Task RequestHeaderData(XDocument criteria)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendComm(await this.CreateRequest(criteria));
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);

                var msg = $"Save {this.LowerCaseName} to database failed.{Environment.NewLine}Reason: {ex.Message}.";
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
        }

        /// <summary>
        /// The Request Item Data.
        /// </summary>
        /// <param name="actHdrRefno"> Activity header refno.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public virtual async Task RequestItemData(int actHdrRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items.",
                    true);

                var responseData = await this.SendRequest(actHdrRefno);
                await this.Save(responseData, actHdrRefno);

                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items complete.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);

                var msg = $"Save {this.LowerCaseName} to database failed.{Environment.NewLine}Reason: {ex.Message}.";
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
        }

        /// <summary>
        /// The Request Item Data.
        /// </summary>
        /// <param name="actHdrRefno"> Activity header reference number.</param>
        /// <param name="criteria"> Item data filter.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public virtual async Task RequestItemData(int actHdrRefno, XDocument criteria)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items.",
                    true);

                string responseData;
                if (actHdrRefno == 0)
                {
                    var query = "SELECT * FROM tActivity WHERE ActivitySubType = ?";
                    var conn = DatabaseConnection.GetInstance().SqlConn;
                    var queryResult = await conn.QueryAsync<ActivityModel>(query, (int)this.CurrentSubType);
                    if (queryResult != null && queryResult.Count > 0)
                    {
                        foreach (ActivityModel model in queryResult)
                        {
                            responseData = await this.SendRequest(model.ActivityRefno, criteria);
                            await this.Save(responseData, model.ActivityRefno);
                        }
                    }
                }
                else
                {
                    responseData = await this.SendRequest(actHdrRefno, criteria);
                    await this.Save(responseData, actHdrRefno);
                }

                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items complete.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);

                var msg = $"Save {this.LowerCaseName} to database failed.{Environment.NewLine}" +
                    $"Reason: {ex.Message}.";
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
        }

        /// <summary>
        /// The Request Detail Data.
        /// </summary>
        /// <param name="actItemRefno"> Activity header refno.</param>
        /// <returns><see cref="Task"/> Task completed.</returns>
        public virtual async Task RequestDetailData(int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details.",
                    overrideDisconnectedMode: true);
                if (actItemRefno == 0)
                {
                    await RequestDataAll();
                }
                else
                {
                    await RequestDataSingeLineItem();
                }

                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details complete.",
                    overrideDisconnectedMode: true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    overrideDisconnectedMode: true);

                var msg = $"Save {this.LowerCaseName} to database failed.{Environment.NewLine}" +
                    $"Reason: {ex.Message}.";
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    overrideDisconnectedMode: true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    overrideDisconnectedMode: true);
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    overrideDisconnectedMode: true);
                throw;
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            async Task RequestDataAll()
            {
                // Get all activity detail for all activity items, used in disconnected mode.
                var query = "SELECT ActivityItemRefno FROM tActivityItem WHERE ActivityItemSubType = ?";
                var queryResult = await DatabaseConnection.GetInstance().SqlConn
                    .QueryAsync<tActivityItemModel>(
                        query,
                        Convert.ToInt32(this.CurrentSubType));
                if (queryResult?.Count > 0)
                {
                    foreach (var model in queryResult)
                    {
                        var response = await this.SendRequest(model.ActivityItemRefno);
                        await this.Save(response, model.ActivityItemRefno);
                    }
                }
                else
                {
                    var msg = $"No activity items could be found for the sub type" +
                        $" {Common.SubTypeEnum2String(this.CurrentSubType)}";
                    throw new Exception(msg);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            async Task RequestDataSingeLineItem()
            {
                var response = await this.SendRequest(actItemRefno);
                await this.Save(response, actItemRefno);
            }
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);

                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task RequestData(int actRefno);

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task PurgeData();

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <param name="sectLocMapRefno"><see cref="int"/> Section location map reference number.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task PurgeData(int sectLocMapRefno);

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <param name="actSubType"> .</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task PurgeData(SysCommon.ActSubTypes actSubType);

        /// <summary>
        /// Get the data status.
        /// </summary>
        /// <returns>.</returns>
        public abstract Task<int> DataStatus();

        /// <summary>
        /// Get the data header status.
        /// </summary>
        /// <returns><see cref="int"/> Row count.</returns>
        public virtual async Task<int> DataHeaderStatus()
        {
            int rowsAffected;
            try
            {
                var query = "SELECT COUNT(tActivity.ActivityRefno)"
                            + " FROM tActivity"
                            + " WHERE tActivity.ActivitySubType = ?";
                var conn = DatabaseConnection.GetInstance().SqlConn;
                rowsAffected = await conn.ExecuteScalarAsync<int>(query, (int)this.CurrentSubType);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} {this.LowerCaseName}(s)", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }
            catch
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Data status {this.LowerCaseName} failed.",
                    true);
                throw;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Get the data item status.
        /// </summary>
        /// <returns><see cref="int"/> Row count.</returns>
        public virtual async Task<int> DataItemStatus()
        {
            int rowsAffected;
            try
            {
                var query = "SELECT COUNT(tActivityItem.ActivityItemRefno) FROM tActivityItem WHERE" +
                    " tActivityItem.ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(
                    query,
                    (int)this.CurrentSubType);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} {this.LowerCaseName} items.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Data status {this.LowerCaseName} items failed.",
                    true);
                throw new Exception($"Database exception occurred. {this.ProperCaseName} items status failed.", ex);
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Data status {this.LowerCaseName} items failed.",
                    true);
                throw;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Get the data detail status.
        /// </summary>
        /// <returns><see cref="int"/> Row count.</returns>
        public virtual async Task<int> DataDetailStatus()
        {
            int rowsAffected;
            try
            {
                var query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno) FROM tActivityDetail WHERE" +
                    " tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(
                    query,
                    Convert.ToInt32(this.CurrentSubType));
                this.DataAccessStatusOutHandler(
                    sender: this,
                    EventArgs.Empty,
                    $"{rowsAffected} {this.LowerCaseName} details.",
                    overrideDisconnectedMode: true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} details status failed.", ex);
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Data status {this.LowerCaseName} detail failed.",
                    true);
                throw;
            }

            return rowsAffected;
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
            this.OnDataAccessStatusOut += new DataAccessStatusOutEventHandler(
                listener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// Unregister the data access status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerStatusOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessStatusOut -= new DataAccessStatusOutEventHandler(
                listener.OnDataAccessStatusOut);
        }

        /// <summary>
        /// Register the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void RegisterDataAccessListenerProgressOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessProgressOut += new DataAccessProgressOutEventHandler(
                listener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// Unregister the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerProgressOut(IDataAccessEventListener listener)
        {
            this.OnDataAccessProgressOut -= new DataAccessProgressOutEventHandler(
                listener.OnDataAccessProgressOut);
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code filter.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected async virtual Task<string> SendRequest(string familyCode)
        {
            return await this.SendComm(await this.CreateRequest(familyCode));
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="actItemRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual async Task<string> SendRequest(int actItemRefno)
        {
            return await this.SendComm(await this.CreateRequest(actItemRefno));
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="locMapFKRefno"><see cref="int"/> Location map foreign key reference.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual async Task<string> SendRequest(int prodLocMapType, int locMapFKRefno)
        {
            return await this.SendComm(await this.CreateRequest(prodLocMapType, locMapFKRefno));
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
        protected virtual async Task<string> SendRequest(XDocument criteria)
        {
            return await this.SendComm(await this.CreateRequest(criteria));
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="actItemRefno"><see cref="int"/> Activity reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual async Task<string> SendRequest(int actItemRefno, XDocument criteria)
        {
            return await this.SendComm(await this.CreateRequest(actItemRefno, criteria));
        }

        /// <summary>
        /// Save received data.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Received data to save.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        protected abstract Task Save(string responseData);

        /// <summary>
        /// Save received data.
        /// </summary>
        /// <param name="responseData"><see cref="string"/> Received data to save.</param>
        /// <param name="refno"><see cref="int"/> Refno.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        protected abstract Task Save(string responseData, int refno);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest();

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest(XDocument criteria);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest(string familyCode);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest(int activityItemFkRefno);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="whsLocMapFKRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno);

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected abstract Task<string> CreateRequestProdLoc(int prodLocMapType);

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
            int schemeVariantNo = useFNumberElements
                ? Convert.ToInt32(drow["f17"])
                : Convert.ToInt32(drow["ProductSchemeVariantNo"]);
            if (schemeVariantNo > 0)
            {
                var results = conn.Query<tProductSchemeVariantModel>(
                    "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                    Convert.ToInt32(schemeVariantNo));

                if (results != null && results.Count > 0)
                {
                    int rowsAffected = conn.Execute(
                        "INSERT INTO tActivityDetailProductSchemeVariantXRef (" +
                        "ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)",
                        Convert.ToInt32(actDetailRefno),
                        results[0].ProductSchemeVariantRefno);
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Insert into activity detail product scheme variant xref returned rows" +
                            " affected <= 0.");
                    }
                }
                else
                {
                    throw new Exception(
                        $"Unable to locate product scheme no {schemeVariantNo} in tProductSchemeVariant");
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

            int rowsAffected = conn.Execute(query, parameters.ToArray());
            if (rowsAffected <= 0)
            {
                throw new Exception("Insert into activity detail extra return rows affected <= 0.");
            }
        }

        /// <summary>
        /// Get the activity detail location information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
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
        protected virtual Task<string> SendComm(string sendData)
        {
            return Task.FromResult(SysCommon.SubmitComm(sendData));
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
        /// <param name="useFNumberElements"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
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
                    "drProduct.Read() failed. Unable to locate the product refno from the product foreign key" +
                    " reference number in tProduct.");
            }

            var unitResults = conn.Query<tUnitModel>("SELECT * FROM tUnit WHERE UnitCode = ?", unitCode);
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
                    " product foreign key reference number, broken case and unit code in tProductUnit.");
            }

            return prodUnitRefno;
        }
    }
}
