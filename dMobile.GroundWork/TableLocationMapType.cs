// <copyright file="TableLocationMapType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableLocationMapType" />.
    /// </summary>
    public class TableLocationMapType : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapType"/> class.
        /// </summary>
        public TableLocationMapType()
      : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapType";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        public override async Task RequestData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types.", true);
                var responseData = string.Empty;
                var whsLocMapRefno = 0;
                responseData = await this.SendRequest(whsLocMapRefno);
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types failed.", true);

                throw new Exception("Seeding location map types failed.", ex);
            }
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types.", true);

                await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapType");

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types failed.", true);

                throw new Exception("Database exception occurred. Purging location map types failed.", ex);
            }
        }

        /// <summary>
        /// The DataStatus.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> DataStatus()
        {
            int rowsAffected;
            try
            {
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapType"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} location map types.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Location map types status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="actItemRefno"></param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int actItemRefno)
        {
            return await this.SendComm(await this.CreateRequest(actItemRefno));
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var errMsg = string.Empty;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    using var ds = new DataSet();
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    ds.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types.", false);

                    if (ds.Tables.Count != 0)
                    {
                        var progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {progMax}", false);

                        var locMapRefno = 0;
                        var locMapSubType = 0;
                        var locMapFKRefno = 0;
                        var progValue = 0;
                        var isLocMapTypeFound = false;
                        var locMapTypeEnum = 0;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["seg"].Rows)
                        {
                            locMapFKRefno = Convert.ToInt32(drow["f03"]);
                            errMsg = "Whs fk refno " + locMapFKRefno;

                            locMapTypeEnum = Convert.ToInt32(drow["f01"]);
                            switch (locMapTypeEnum)
                            {
                                case (int)SysCommon.LocMapTypes.IsFreezer:
                                    errMsg += Environment.NewLine + "Map type \'Freezer\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsGroundLevel:
                                    errMsg += Environment.NewLine + "Map type \'Ground Level\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsHandStack:
                                    errMsg += Environment.NewLine + "Map type \'Hand Stack\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsProcessing:
                                    errMsg += Environment.NewLine + "Map type \'Processing\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsReserved:
                                    errMsg += Environment.NewLine + "Map type \'Reserved\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsStaging:
                                    errMsg += Environment.NewLine + "Map type \'Staging\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsToBeOrdered:
                                    errMsg += Environment.NewLine + "Map type \'To Be Ordered\'";
                                    break;

                                case (int)SysCommon.LocMapTypes.IsTracked:
                                    errMsg += Environment.NewLine + "Map type \'Tracked\'";
                                    break;

                                default:
                                    throw new Exception($"Undefined location map type {locMapTypeEnum}");
                            }

                            locMapSubType = Convert.ToInt32(drow["f02"]);

                            switch (locMapSubType)
                            {
                                case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    errMsg += Environment.NewLine + "Map sub type \'Warehouse\'";
                                    var resultsLocationMapWarehouse = transaction.Query<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", locMapFKRefno);

                                    if (resultsLocationMapWarehouse?.Count > 0)
                                    {
                                        locMapRefno = resultsLocationMapWarehouse[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("No results in tLocationMapWarehouse for LocationMapWarehouseFKRefno " + locMapFKRefno + Environment.NewLine + this.FullClassName + " Save" + Environment.NewLine + errMsg);
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Section:
                                    errMsg += Environment.NewLine + "Map sub type \'Section\'";
                                    var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", locMapFKRefno);

                                    if (resultsLocationMapSection?.Count > 0)
                                    {
                                        locMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("No results in tLocationMapSection for LocationMapSectionFKRefno " + locMapFKRefno + Environment.NewLine + this.FullClassName + " Save" + Environment.NewLine + errMsg);
                                    }

                                    break;

                                case (int)SysCommon.LocMapSubTypes.Slot:
                                    errMsg += Environment.NewLine + "Map sub type \'Slot\'";
                                    var resultsLocationMapSlot = transaction.Query<tLocationMapSlotModel>("SELECT * FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", locMapFKRefno);

                                    if (resultsLocationMapSlot?.Count > 0)
                                    {
                                        locMapRefno = resultsLocationMapSlot[0].LocationMapRefno;
                                    }
                                    else
                                    {
                                        throw new Exception("No results in tLocationMapSlot for LocationMapSlotFKRefno " + locMapFKRefno + Environment.NewLine + this.FullClassName + " Save" + Environment.NewLine + errMsg);
                                    }

                                    break;

                                default:

                                    throw new Exception($"Undefined Location Map SubType {locMapSubType}");
                            }

                            isLocMapTypeFound = false;

                            if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                            {
                                var resultsLocationMapType = transaction.Query<tLocationMapTypeModel>("SELECT * FROM tLocationMapType WHERE LocationMapRefno = ? AND LocationMapTypeEnum = ?", locMapRefno, locMapTypeEnum);

                                if (resultsLocationMapType?.Count > 0)
                                {
                                    isLocMapTypeFound = true;
                                }
                            }

                            if (!isLocMapTypeFound)
                            {
                                transaction.Execute("INSERT INTO tLocationMapType (LocationMapRefno, LocationMapTypeEnum) VALUES (?, ?)", locMapRefno, locMapTypeEnum);
                            }

                            progValue++;
                            if (progValue % 100 == 0)
                            {
                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                            }
                        }

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                    }

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types failed.", false);

                    throw new Exception("Database exception occurred. Saving location map types failed." + Environment.NewLine + errMsg, ex);
                }
            });
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityItemFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityItemFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var locMapWhsFKRefno = 0;
            var locMapWhs = new TableLocationMapWarehouse();

            // lookup and verify warehouse
            if (activityItemFkRefno > 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityItemFkRefno);
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST LOCATION MAP TYPES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        public override Task RequestData(int actRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(int sectLocMapRefno)
        {
            throw new NotImplementedException();
        }

        public override Task PurgeData(SysCommon.ActSubTypes actSubType)
        {
            throw new NotImplementedException();
        }

        protected override Task Save(string responseData, int refno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityItemFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }
    }
}
