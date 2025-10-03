// <copyright file="TableBase.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

#pragma warning disable SA1300
namespace dMobile.GroundWork
#pragma warning restore SA1300
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Bedrock;
    using Resources.Models.LocationMap;
    using Resources.Models.Products;
    using SQLite;

    /// <summary>
    /// <see cref="TableBase" /> Table base class.
    /// </summary>
    public abstract class TableBase : IDataAccessEventPublisher
    {
        /// <summary>
        /// On data access status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="args"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void DataAccessStatusOutEventHandler(
            IDataAccessEventPublisher publisher,
            DataAccessEventArgs args);

        /// <summary>
        /// On data access progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Data
        /// access event publisher.</param>
        /// <param name="args"><see cref="DataAccessEventArgs"/> Data access event
        /// arguments.</param>
        public delegate void DataAccessProgressOutEventHandler(
            IDataAccessEventPublisher publisher,
            DataAccessEventArgs args);

        /// <summary>
        /// Define the on access status out event.
        /// </summary>
        public event DataAccessStatusOutEventHandler OnDataAccessStatusOut;

        /// <summary>
        /// Define the on access progress out event.
        /// </summary>
        public event DataAccessProgressOutEventHandler OnDataAccessProgressOut;

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
        protected SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets or sets the table base current activity subtype.
        /// </summary>
        protected SysCommon.ActSubTypes CurrentSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets the data access event arguments.
        /// </summary>
        private DataAccessEventArgs DataEventArgs { get; set; } = new DataAccessEventArgs();

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <param name="actRefno"><see cref="int"/> Activity reference number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task PurgeData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform a data purge.
        /// </summary>
        /// <param name="actSubType"> .</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// Request header data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        ///     Send the comm request to the server and save the response to the local database.
        /// </remarks>
        public virtual async Task RequestHeaderData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName}.",
                    true);
                var responseData = await SendComm(await this.CreateRequest());
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.LowerCaseName} seeded.",
                    true);
            }
            catch (SQLiteException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// Request header detail data using a filter criteria.
        /// </summary>
        /// <param name="criteria"> Filter criteria.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Send the comm request to the server and save the response to the local database.
        /// </remarks>
        public async Task RequestHeaderData(XDocument criteria)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName}.",
                    true);
                var responseData = await SendComm(await this.CreateRequest(criteria));
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.LowerCaseName} seeded.",
                    true);
            }
            catch (SQLiteException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// The Request Item Data.
        /// </summary>
        /// <param name="actHdrFkRefno"> Activity header foreign key refno.</param>
        /// <param name="actHdrRefno"> Activity header refno.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RequestItemData(int actHdrFkRefno, int actHdrRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items.",
                    true);
                var responseData = await this.SendRequest(actHdrFkRefno);
                await this.Save(responseData, actHdrRefno);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items complete.",
                    true);
            }
            catch (SQLiteException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// The Request Item Data.
        /// </summary>
        /// <param name="actHdrFkRefno"> Activity header foreign key reference number.</param>
        /// <param name="criteria"> Item data filter.</param>
        /// <param name="actHdrRefno"> Activity header reference number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RequestItemData(int actHdrFkRefno, XDocument criteria, int actHdrRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items.",
                    true);
                var responseData = await this.SendRequest(actHdrFkRefno, criteria);
                await this.Save(responseData, actHdrRefno);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items complete.",
                    true);
            }
            catch (SQLiteException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} items failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// The Request Detail Data.
        /// </summary>
        /// <param name="actItemFkRefno">Activity item foreign key refno.</param>
        /// <param name="actItemRefno">Activity item refno.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RequestDetailData(int actItemFkRefno, int actItemRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details.",
                    true);
                var response = await this.SendRequest(actItemFkRefno);
                await this.Save(response, actItemRefno);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details complete.",
                    true);
            }
            catch (SQLiteException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    true);
                throw;
            }
            catch (CommunicationException)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Communication exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    true);
                throw;
            }
            catch (Exception)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Unhandled general exception caught.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} details failed.",
                    true);
                throw;
            }
        }

        /// <summary>
        /// Perform a request for data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName}.",
                    true);
                var reqData = await this.SendRequest();
                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var reqErr = new XmlDocument();
                        reqErr.LoadXml(reqData);
                        throw new BaseException(reqErr.DocumentElement?["detail"] !.InnerText);
                    }

                    await this.Save(reqData);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{this.LowerCaseName} seeded.",
                    true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"Seeding {this.LowerCaseName} failed.",
                    true);
                throw new BaseException($"Seeding {this.LowerCaseName} failed.", ex);
            }
        }

        /// <summary>
        /// Get the data header status.
        /// </summary>
        /// <returns><see cref="int"/> Row count.</returns>
        public async Task<int> DataHeaderStatus()
        {
            int rowsAffected;
            try
            {
                const string query = "SELECT COUNT(tActivity.ActivityRefno)" +
                                     " FROM tActivity" +
                                     " WHERE tActivity.ActivitySubType = ?";
                var conn = DatabaseConnection.GetInstance().SqlConn;
                rowsAffected = await conn.ExecuteScalarAsync<int>(query, (int)this.CurrentSubType);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} {this.LowerCaseName}(s)",
                    true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException(
                    $"Database exception occurred. {this.ProperCaseName} status failed.",
                    ex);
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
        public async Task<int> DataItemStatus()
        {
            int rowsAffected;
            try
            {
                const string query = "SELECT COUNT(tActivityItem.ActivityItemRefno)" +
                                     " FROM tActivityItem" +
                                     " WHERE tActivityItem.ActivityItemSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteScalarAsync<int>(
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
                throw new BaseException(
                    $"Database exception occurred. {this.ProperCaseName} items status failed.",
                    ex);
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
        public async Task<int> DataDetailStatus()
        {
            int rowsAffected;
            try
            {
                const string query = "SELECT COUNT(tActivityDetail.ActivityDetailRefno)" +
                                     " FROM tActivityDetail WHERE" +
                                     " tActivityDetail.ActivityDetailSubType = ?";
                rowsAffected = await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteScalarAsync<int>(
                        query,
                        Convert.ToInt32(this.CurrentSubType));
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rowsAffected} {this.LowerCaseName} details.",
                    true);
            }
            catch (SQLiteException ex)
            {
                throw new BaseException(
                    $"Database exception occurred. {this.ProperCaseName} details status failed.",
                    ex);
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
        /// Register the data access status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess"/> Data access event listener.</param>
        public void RegisterDataAccessListenerStatusOut(IDataAccess listener)
        {
            this.OnDataAccessStatusOut += listener.OnDataAccessStatusOut;
        }

        /// <summary>
        /// Unregister the data access status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerStatusOut(IDataAccess listener)
        {
            this.OnDataAccessStatusOut -= listener.OnDataAccessStatusOut;
        }

        /// <summary>
        /// Register the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess"/> Data access event listener.</param>
        public void RegisterDataAccessListenerProgressOut(IDataAccess listener)
        {
            this.OnDataAccessProgressOut += listener.OnDataAccessProgressOut;
        }

        /// <summary>
        /// Unregister the data access progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess"/> Data access event listener.</param>
        public void UnregisterDataAccessListenerProgressOut(IDataAccess listener)
        {
            this.OnDataAccessProgressOut -= listener.OnDataAccessProgressOut;
        }

        /// <summary>
        /// Save the common detail product scheme variant information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="actDetailRefno"><see cref="int"/> Activity detail reference number.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        protected static void SaveCommonDetailVariant(
            DataRow drow,
            SQLiteConnection conn,
            int actDetailRefno,
            bool useShortElementNames)
        {
            var schemeVariantNo = useShortElementNames
                ? Convert.ToInt32(drow["f17"])
                : Convert.ToInt32(drow["ProductSchemeVariantNo"]);
            if (schemeVariantNo > 0)
            {
                var results = conn.Query<ProductSchemeVariantModel>(
                    "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                    Convert.ToInt32(schemeVariantNo));

                if (results != null &&
                    results.Count > 0)
                {
                    var rowsAffected = conn.Execute(
                        "INSERT INTO tActivityDetailProductSchemeVariantXRef (" +
                        "ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)",
                        Convert.ToInt32(actDetailRefno),
                        results[0].ProductSchemeVariantRefno);
                    if (rowsAffected <= 0)
                    {
                        throw new BaseException(
                            "Insert into activity detail product scheme variant xref returned rows" +
                            " affected <= 0.");
                    }
                }
                else
                {
                    throw new BaseException(
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
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        protected static void SaveDetailExtra(
            DataRow drow,
            SQLiteConnection conn,
            int actDetailRefno,
            bool useShortElementNames)
        {
            const string emptyDate = "01-01-1900";
            var parameters = new List<object>();

            const string query = "INSERT INTO tActivityDetailExtra (" +
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

            if (useShortElementNames)
            {
                parameters.Add(Convert.ToString(drow["f19"]));
                parameters.Add(Convert.ToString(drow["f20"]));
                parameters.Add(Convert.ToString(drow["f21"]));
                parameters.Add(Convert.ToString(drow["f22"]));

                parameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f23"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f23"]));
            }
            else
            {
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra1"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra2"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra3"]));
                parameters.Add(Convert.ToString(drow["ActivityDetailExtra4"]));

                parameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["ActivityDetailExtra5"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["ActivityDetailExtra5"]));
            }

            var rowsAffected = conn.Execute(query, parameters.ToArray());
            if (rowsAffected <= 0)
            {
                throw new BaseException("Insert into activity detail extra return rows affected <= 0.");
            }
        }

        /// <summary>
        /// Get the activity detail location information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
        /// <param name="actSubType">Activity subtype to control the field names to parse.</param>
        /// <returns>The <see cref="int"/> Identified location map reference number.</returns>
        protected static int GetReceivedDataLocation(
            DataRow drow,
            SQLiteConnection conn,
            bool useShortElementNames,
            SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.NotApplicable)
        {
            var locMapRefno = 0;
            int locMapWhsFkRefno;
            int locMapSectFkRefno;
            int locMapSlotFkRefno;
            string query;
            if (useShortElementNames)
            {
                if (actSubType == SysCommon.ActSubTypes.PhysicalCounts || actSubType == SysCommon.ActSubTypes.CycleCounts)
                {
                    locMapWhsFkRefno = Convert.ToInt32(drow["f06"]);
                    locMapSectFkRefno = Convert.ToInt32(drow["f07"]);
                    locMapSlotFkRefno = Convert.ToInt32(drow["f08"]);
                }
                else
                {
                    locMapWhsFkRefno = Convert.ToInt32(drow["f07"]);
                    locMapSectFkRefno = Convert.ToInt32(drow["f08"]);
                    locMapSlotFkRefno = Convert.ToInt32(drow["f09"]);
                }
            }
            else
            {
                locMapWhsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                locMapSectFkRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                locMapSlotFkRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
            }

            if (locMapSlotFkRefno != 0)
            {
                query = "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?";
                var results = conn.Query<LocationMapSlotModel>(query, locMapSlotFkRefno);
                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapSectFkRefno != 0 && locMapRefno == 0)
            {
                query = "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?";
                var results = conn.Query<LocationMapSectionModel>(query, locMapSectFkRefno);
                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapWhsFkRefno != 0 && locMapRefno == 0)
            {
                query = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?";
                var results = conn.Query<LocationMapWarehouseModel>(query, locMapWhsFkRefno);
                if (results != null && results.Count > 0)
                {
                    locMapRefno = results[0].LocationMapRefno;
                }
            }

            if (locMapRefno == 0)
            {
                var msg = $"Unable to identify the location map reference number. {Environment.NewLine}" +
                          $"Whs FK Refno {locMapWhsFkRefno}{Environment.NewLine}Section" +
                          $" FK Refno {locMapSectFkRefno}{Environment.NewLine} Slot FK Refno {locMapSlotFkRefno}";
                throw new BaseException(msg);
            }

            return Convert.ToInt32(locMapRefno);
        }

        /// <summary>
        /// Get the activity detail location information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteAsyncConnection"/> SQLite async connection.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
        /// <param name="actSubType">Activity subtype to control the field names to parse.</param>
        /// <returns>The <see cref="int"/> Identified location map reference number.</returns>
        protected static async Task<int> GetReceivedDataLocationAsync(
            DataRow drow,
            SQLiteAsyncConnection conn,
            bool useShortElementNames,
            SysCommon.ActSubTypes actSubType = SysCommon.ActSubTypes.NotApplicable)
        {
            int locMapWhsFkRefno;
            int locMapSectFkRefno;
            int locMapSlotFkRefno;
            if (useShortElementNames)
            {
                if (actSubType == SysCommon.ActSubTypes.PhysicalCounts || actSubType == SysCommon.ActSubTypes.CycleCounts)
                {
                    locMapWhsFkRefno = Convert.ToInt32(drow["f06"]);
                    locMapSectFkRefno = Convert.ToInt32(drow["f07"]);
                    locMapSlotFkRefno = Convert.ToInt32(drow["f08"]);
                }
                else
                {
                    locMapWhsFkRefno = Convert.ToInt32(drow["f07"]);
                    locMapSectFkRefno = Convert.ToInt32(drow["f08"]);
                    locMapSlotFkRefno = Convert.ToInt32(drow["f09"]);
                }
            }
            else
            {
                locMapWhsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                locMapSectFkRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
                locMapSlotFkRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
            }

            var tasks = new List<Task<int>>
            {
                GetLocationMapRefnoAsync(
                    conn,
                    "SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                    locMapSlotFkRefno),
                GetLocationMapRefnoAsync(
                    conn,
                    "SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                    locMapSectFkRefno),
                GetLocationMapRefnoAsync(
                    conn,
                    "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                    locMapWhsFkRefno)
            };

            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);
                if (completedTask.Result != 0)
                {
                    return completedTask.Result;
                }

                tasks.Remove(completedTask);
            }

            var msg = $"Unable to identify the location map reference number. {Environment.NewLine}" +
                      $"Whs FK Refno {locMapWhsFkRefno}{Environment.NewLine}Section" +
                      $" FK Refno {locMapSectFkRefno}{Environment.NewLine} Slot FK Refno {locMapSlotFkRefno}";
            throw new BaseException(msg);

            static async Task<int> GetLocationMapRefnoAsync(SQLiteAsyncConnection conn, string query, int fkRefno)
            {
                if (fkRefno == 0)
                {
                    return 0;
                }

                var results = await conn.QueryAsync<LocationMapModel>(query, fkRefno);
                return results.Count > 0 ? results[0].LocationMapRefno : 0;
            }
        }

        /// <summary>
        /// Get the activity detail information.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row to get the scheme variant no from.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQLite connection.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read
        /// 'F' XML element or a more verbose field name element.</param>
        /// <returns>The activity detail information.</returns>
        protected static List<object> SaveDetail(
            DataRow drow,
            SQLiteConnection conn,
            bool useShortElementNames)
        {
            const string emptyDate = "01-01-1900";
            var detailParameters = new List<object>();
            if (useShortElementNames)
            {
                detailParameters.Add(Convert.ToString(drow["f05"]));
                detailParameters.Add(Convert.ToString(drow["f06"]));
                detailParameters.Add(Convert.ToDecimal(drow["f02"]));
                detailParameters.Add(Convert.ToDecimal(drow["f03"]));
                detailParameters.Add(Convert.ToDecimal(drow["f04"]));
                detailParameters.Add(Convert.ToString(drow["f10"]));
                detailParameters.Add(Convert.ToString(drow["f11"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f13"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f13"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f14"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f14"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f12"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f12"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f15"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f15"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(Convert.ToString(drow["f16"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["f16"]));

                detailParameters.Add(Convert.ToString(drow["f18"]));
            }
            else
            {
                detailParameters.Add(Convert.ToString(drow["ActivityDetailUniqueSchemeID"]));
                detailParameters.Add(Convert.ToString(drow["ActivityDetailSchemeID"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailQuantity"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));
                detailParameters.Add(Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));
                detailParameters.Add(
                    Convert.ToString(drow["ActivityDetailSingleTradeItemSerialNumber"]));
                detailParameters.Add(
                    Convert.ToString(drow["ActivityDetailLogisticUnitBatchLotNumber"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(
                        Convert.ToString(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(
                            drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(
                        Convert.ToString(drow["ActivityDetailLogisticUnitExpiryUseByDate"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(
                        Convert.ToString(drow["ActivityDetailLogisticUnitProductionDate"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(
                        Convert.ToString(drow["ActivityDetailLogisticUnitDueDate"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));

                detailParameters.Add(
                    string.IsNullOrEmpty(
                        Convert.ToString(drow["ActivityDetailLogisticUnitPackagingDate"]))
                        ? DateTime.Parse(emptyDate)
                        : Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));

                detailParameters.Add(Convert.ToString(drow["ActivityDetailLogisticUnitSkidCode"]));
            }

            return detailParameters;
        }

        /// <summary>
        /// Send the communication request.
        /// </summary>
        /// <param name="sendData"><see cref="string"/> Communication request.</param>
        /// <returns><see cref="string"/> Communication result.</returns>
        protected static Task<string> SendComm(string sendData)
        {
            return Task.FromResult(SysCommon.SubmitComm(sendData));
        }

        /// <summary>
        /// Get the warehouse location map refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Row of data.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQL connection.</param>
        /// <returns>Warehouse location map reference number.</returns>
        protected static int GetWhsLocMapRefnoFromDataRow(DataRow drow, SQLiteAsyncConnection conn)
        {
            var locMapWhsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
            var results = conn.QueryAsync<LocationMapWarehouseModel>(
                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                Convert.ToInt32(locMapWhsFkRefno));

            int locMapRefno;
            if (results != null && results.Result.Count > 0)
            {
                locMapRefno = results.Result[0].LocationMapRefno;
            }
            else
            {
                throw new BaseException("No query result was returned.");
            }

            if (locMapRefno == 0)
            {
                throw new BaseException(
                    "Unable to locate the location map reference number.");
            }

            return locMapRefno;
        }

        /// <summary>
        /// Get the warehouse location map refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Row of data.</param>
        /// <param name="conn"><see cref="SQLiteConnection"/> SQL connection.</param>
        /// <returns>Warehouse location map reference number.</returns>
        protected static int GetWhsLocMapRefnoFromDataRow(DataRow drow, SQLiteConnection conn)
        {
            var locMapWhsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
            var results = conn.Query<LocationMapWarehouseModel>(
                "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                Convert.ToInt32(locMapWhsFkRefno));

            int locMapRefno;
            if (results != null &&
                results.Count > 0)
            {
                locMapRefno = results[0].LocationMapRefno;
            }
            else
            {
                throw new BaseException("No query result was returned.");
            }

            if (locMapRefno == 0)
            {
                throw new BaseException(
                    "Unable to locate the location map reference number.");
            }

            return locMapRefno;
        }

        /// <summary>
        /// Get the product unit refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row.</param>
        /// <param name="conn">The conn<see cref="SQLiteConnection"/> SQL connection.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
        /// <returns>Product unit reference number.</returns>
        protected static int GetProductUnitRefnoFromDataRow(
            DataRow drow,
            SQLiteConnection conn,
            bool useShortElementNames)
        {
            int prodFkRefno;
            string unitCode;
            bool prodUnitIsBc;
            if (useShortElementNames)
            {
                prodFkRefno = Convert.ToInt32(drow["f03"]);
                unitCode = Convert.ToString(drow["f05"]);
                prodUnitIsBc = Convert.ToBoolean(drow["f04"]);
            }
            else
            {
                prodFkRefno = Convert.ToInt32(drow["ProductFKRefno"]);
                unitCode = Convert.ToString(drow["UnitCode"]);
                prodUnitIsBc = Convert.ToBoolean(drow["ProductUnitIsBC"]);
            }

            var productResults = conn.Query<ProductModel>(
                "SELECT * FROM tProduct WHERE ProductFKRefno = ?",
                Convert.ToInt32(prodFkRefno));
            int prodRefno;
            if (productResults != null &&
                productResults.Count > 0)
            {
                prodRefno = Convert.ToInt32(productResults[0].ProductRefno);
            }
            else
            {
                throw new BaseException(
                    "drProduct.Read() failed. Unable to locate the product refno from the product foreign key" +
                    " reference number in tProduct.");
            }

            var unitResults = conn.Query<UnitModel>(
                "SELECT * FROM tUnit WHERE UnitCode = ?",
                unitCode);
            int unitRefno;
            if (unitResults != null &&
                unitResults.Count > 0)
            {
                unitRefno = Convert.ToInt32(unitResults[0].UnitRefno);
            }
            else
            {
                throw new BaseException("drUnit.Read() failed.");
            }

            var productUnitResults = conn.Query<ProductUnitModel>(
                "SELECT * FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ? AND" +
                " ProductUnitIsBC = ?",
                Convert.ToInt32(prodRefno),
                Convert.ToInt32(unitRefno),
                Convert.ToBoolean(prodUnitIsBc));
            int prodUnitRefno;
            if (productUnitResults != null &&
                productUnitResults.Count > 0)
            {
                prodUnitRefno = Convert.ToInt32(productUnitResults[0].ProductUnitRefno);
            }
            else
            {
                throw new BaseException(
                    "drProdUnit.Read() failed. Unable to locate the product unit refno from the" +
                    " product foreign key reference number, broken case and unit code in tProductUnit.");
            }

            return prodUnitRefno;
        }

        /// <summary>
        /// Get the product unit refno from the data row.
        /// </summary>
        /// <param name="drow"><see cref="DataRow"/> Data row.</param>
        /// <param name="conn">The conn<see cref="SQLiteConnection"/> SQL connection.</param>
        /// <param name="useShortElementNames"><see cref="bool"/> Flag indicating whether to read 'F' XML element or a
        /// more verbose field name element.</param>
        /// <returns>Found product unit reference number.</returns>
        protected static async Task<int> GetProductUnitRefnoFromDataRowAsync(
            DataRow drow,
            SQLiteAsyncConnection conn,
            bool useShortElementNames)
        {
            int prodFkRefno;
            string unitCode;
            bool prodUnitIsBc;
            if (useShortElementNames)
            {
                // Physical and Cycle counts mapping fields.
                prodFkRefno = Convert.ToInt32(drow["f03"]);
                unitCode = Convert.ToString(drow["f05"]);
                prodUnitIsBc = Convert.ToBoolean(drow["f04"]);
            }
            else
            {
                prodFkRefno = Convert.ToInt32(drow["ProductFKRefno"]);
                unitCode = Convert.ToString(drow["UnitCode"]);
                prodUnitIsBc = Convert.ToBoolean(drow["ProductUnitIsBC"]);
            }

            var productTask = conn.QueryAsync<ProductModel>(
                "SELECT tProduct.ProductRefno FROM tProduct WHERE ProductFKRefno = ?",
                prodFkRefno);

            var unitTask = conn.QueryAsync<UnitModel>(
                "SELECT tUnit.UnitRefno FROM tUnit WHERE UnitCode = ?",
                unitCode);

            await Task.WhenAll(productTask, unitTask);

            var productResults = productTask.Result;
            var unitResults = unitTask.Result;

            if (productResults == null || productResults.Count == 0)
            {
                throw new BaseException(
                    "drProduct.Read() failed. Unable to locate the product refno from the product foreign key" +
                    " reference number in tProduct.");
            }

            if (unitResults == null || unitResults.Count == 0)
            {
                throw new BaseException("drUnit.Read() failed.");
            }

            var prodRefno = productResults[0].ProductRefno;
            var unitRefno = unitResults[0].UnitRefno;
            var productUnitResults = await conn.QueryAsync<ProductUnitModel>(
                "SELECT tProductUnit.ProductUnitRefno FROM tProductUnit WHERE ProductRefno = ? AND UnitRefno = ?" +
                " AND ProductUnitIsBC = ?",
                prodRefno,
                unitRefno,
                prodUnitIsBc);

            if (productUnitResults == null || productUnitResults.Count == 0)
            {
                throw new BaseException(
                    "Unable to locate the product unit refno from the product foreign key reference number, broken" +
                    " case and unit code in tProductUnit.");
            }

            return productUnitResults[0].ProductUnitRefno;
        }

        /// <summary>
        /// Sends a request based on the specified activity foreign key reference number.
        /// </summary>
        /// <param name="familyCode"><see cref="string"/> Family code filter.</param>
        /// <remarks>This method retrieves data from the database using the provided activity foreign key
        /// reference number. If a matching record is found, it creates and sends a communication request based on the
        /// retrieved data.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string from the
        /// communication request.</returns>
        protected virtual async Task<string> SendRequest(string familyCode)
        {
            return await SendComm(await this.CreateRequest(familyCode));
        }

        /// <summary>
        /// Sends a request based on the specified activity foreign key reference number.
        /// </summary>
        /// <remarks>This method retrieves data from the database using the provided activity foreign key
        /// reference number. If a matching record is found, it creates and sends a communication request based on the
        /// retrieved data.</remarks>
        /// <param name="activityFkRefno">The foreign key reference number of the activity item. Must be a valid reference number.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response string from the
        /// communication request.</returns>
        protected virtual async Task<string> SendRequest(int activityFkRefno)
        {
            return await SendComm(await this.CreateRequest(activityFkRefno));
        }

        /// <summary>
        /// Data access status out event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="statusMessage"><see cref="string"/> Status message.</param>
        /// <param name="suppressStatusOutMessage">Flag to suppress the status out message.</param>
        protected void DataAccessStatusOutHandler(
            object sender,
            EventArgs e,
            string statusMessage,
            bool suppressStatusOutMessage)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || suppressStatusOutMessage)
            {
                if (this.OnDataAccessStatusOut != null)
                {
                    this.DataEventArgs.StatusOutMsg = statusMessage;
                    this.DataEventArgs.OverrideDisconnectedMode = suppressStatusOutMessage;
                    this.OnDataAccessStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        ///     Data access status out event handler.
        /// </summary>
        /// <param name="statusMessage">Status message.</param>
        protected void StatusOut(string statusMessage)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode && this.OnDataAccessStatusOut != null)
            {
                this.DataEventArgs.StatusOutMsg = statusMessage;
                this.DataEventArgs.OverrideDisconnectedMode = false;
                this.OnDataAccessStatusOut(this, this.DataEventArgs);
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
        protected void DataAccessProgressOutHandler(
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
        /// Data access progress out event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress index value.</param>
        protected void DataAccessProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (this.OnDataAccessProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataAccessProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="locMapFkRefno"><see cref="int"/> Location map foreign key reference.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected async Task<string> SendRequest(int prodLocMapType, int locMapFkRefno)
        {
            return await SendComm(await this.CreateRequest(prodLocMapType, locMapFkRefno));
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <returns><see cref="string"/> Send result information.</returns>
        protected virtual async Task<string> SendRequest()
        {
            return await SendComm(await this.CreateRequest());
        }

        /// <summary>
        /// Send a data request.
        /// </summary>
        /// <param name="activityFkRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Send result information.</returns>
        /// <remarks>
        ///     The activity reference number may be an activity header, item or detail reference number.
        ///     The criteria are used to filter the data request.
        /// </remarks>
        protected virtual async Task<string> SendRequest(int activityFkRefno, XDocument criteria)
        {
            return await SendComm(await this.CreateRequest(activityFkRefno, criteria));
        }

        /// <summary>
        /// Performs the save operation for the received data.
        /// </summary>
        /// <param name="responseData">Received data from communication server.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task Save(string responseData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs the save operation for the received data.
        /// </summary>
        /// <param name="responseData">Received data from communication server.</param>
        /// <param name="refno">Activity or activity item or activity detail reference number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task Save(string responseData, int refno)
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
        /// <param name="criteriaFilter"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(XDocument criteriaFilter)
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
        /// <param name="activityFkRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        /// <remarks>
        ///     The activity reference number may be an activity header, item or detail reference number.
        /// </remarks>
        protected virtual Task<string> CreateRequest(int activityFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="activityFkRefno"><see cref="int"/> Activity foreign key reference number.</param>
        /// <param name="criteria"><see cref="XDocument"/> Data filter request.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        /// <remarks>
        ///     The activity reference number may be an activity header, item or detail reference number.
        /// </remarks>
        protected virtual Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the request.
        /// </summary>
        /// <param name="prodLocMapType"><see cref="int"/> Product location map type.</param>
        /// <param name="whsLocMapFkRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <returns><see cref="string"/> Created request.</returns>
        protected virtual Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
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
    }
}