// <copyright file="TableLocationMapSlot.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableLocationMapSlot" />.
    /// </summary>
    public class TableLocationMapSlot : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSlot"/> class.
        /// </summary>
        public TableLocationMapSlot()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlot";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task RequestData(int whsLocMapRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots.", true);

                var responseData = string.Empty;

                if (whsLocMapRefno > 0)
                {
                    var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", whsLocMapRefno);

                    if (results?.Count > 0)
                    {
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty,
                            $"For whs \'{results[0].LocationMapWarehouseName}\'.", true);
                    }
                    else
                    {
                        throw new Exception("No results in tLocationMapWarehouse for LocationMapRefno " + whsLocMapRefno + Environment.NewLine + this.FullClassName + " Request");
                    }
                }

                responseData = await this.SendRequest(whsLocMapRefno);
                await this.SaveOpt(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);

                throw new Exception("Database exception occurred. Seeding slots failed.", ex);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slots failed.", true);

                throw new Exception("Seeding slots failed.", ex);
            }
        }

        /// <summary>
        /// The PurgeData.
        /// </summary>
        /// <param name="sectRefno"></param>
        public override async Task PurgeData(int sectRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots.", true);

                if (sectRefno == 0)
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSlot");
                }
                else
                {
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("DELETE FROM tLocationMapSlot WHERE SectionLocationMapRefno = ?", sectRefno);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slots failed.", true);

                throw new Exception("Database exception occurred. Purging slots failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSlot"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} slots.", false);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Slots status failed.", ex);
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
        protected override Task Save(string responseData)
        {
            throw new Exception("TableLocationMapSlot Save is not implemented");
        }

        /// <summary>
        /// The SaveOpt.
        /// </summary>
        /// <param name="responseData"></param>
        protected async Task SaveOpt(string responseData)
        {
            var errMsg = string.Empty;
            var recordCount = 0;

            await DatabaseConnection.GetInstance().SQLConn.RunInTransactionAsync(transaction =>
            {
                try
                {
                    var startIndex = responseData.IndexOf("<VFPData", StringComparison.Ordinal);

                    using var stringReader = new StringReader(responseData.Substring(startIndex));
                    using var reader = XmlReader.Create(stringReader);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received responseData.", false);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots.", false);

                    var locMapRefno = 0;
                    var isLocMapSlotFound = false;
                    var sectLocMapRefno = 0;
                    var whsLocMapRefno = 0;
                    var coord1Refno = 0;
                    var coord2Refno = 0;
                    var coord3Refno = 0;

                    // f01
                    var slotName = string.Empty;

                    // f02
                    var slotBarCode = string.Empty;

                    // f03
                    var slotFKRefno = 0;

                    // f04
                    var sectFKRefno = 0;

                    // f05
                    var slotCoord1 = string.Empty;

                    // f06
                    var slotCoord2 = string.Empty;

                    // f07
                    var slotCoord3 = string.Empty;

                    var slot1Lookup = new Dictionary<string, int>();
                    var slot2Lookup = new Dictionary<string, int>();
                    var slot3Lookup = new Dictionary<string, int>();

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, 100);

                    while (!reader.EOF)
                    {
                        var recordFilled = false;
                        var elementName = string.Empty;

                        while (!recordFilled && reader.Read())
                        {
                            // Fill one record
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                elementName = reader.Name;
                            }
                            else if (reader.NodeType == XmlNodeType.Text)
                            {
                                switch (elementName)
                                {
                                    case "seg":
                                        // beginning a segment
                                        break;
                                    case "f01":
                                        slotName = reader.Value;
                                        break;
                                    case "f02":
                                        slotBarCode = reader.Value;
                                        break;
                                    case "f03":
                                        slotFKRefno = reader.ReadContentAsInt();
                                        break;
                                    case "f04":
                                        sectFKRefno = reader.ReadContentAsInt();
                                        break;
                                    case "f05":
                                        slotCoord1 = reader.Value;
                                        break;
                                    case "f06":
                                        slotCoord2 = reader.Value;
                                        break;
                                    case "f07":
                                        slotCoord3 = reader.Value;
                                        break;
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "seg")
                            {
                                recordFilled = true; // we are full
                            }
                        } // end of fill one record

                        if (!recordFilled)
                        {
                            continue;
                        }

                        errMsg = "Slot fk refno " + slotFKRefno;
                        errMsg += Environment.NewLine + "Slot name \'" + slotName + "\'";
                        errMsg += Environment.NewLine + "Slot bar code \'" + slotBarCode + "\'";
                        errMsg += Environment.NewLine + "Sect fk refno " + sectFKRefno;

                        var resultsLocationMapSection = transaction.Query<tLocationMapSectionModel>("SELECT * FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", sectFKRefno);

                        if (resultsLocationMapSection?.Count > 0)
                        {
                            sectLocMapRefno = resultsLocationMapSection[0].LocationMapRefno;
                            whsLocMapRefno = resultsLocationMapSection[0].WarehouseLocationMapRefno;
                        }
                        else
                        {
                            throw new Exception("No results in tLocationMapSection for LocationMapSectionFKRefno " + sectFKRefno + Environment.NewLine + errMsg);
                        }

                        errMsg += Environment.NewLine + "Slot coordinate 1 \'" + slotCoord1 + "\'";

                        if (!slot1Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord1, out coord1Refno))
                        {
                            var query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                            var resultsLocationMapSlotCoordinate1 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1), slotCoord1, whsLocMapRefno);

                            if (resultsLocationMapSlotCoordinate1?.Count > 0)
                            {
                                coord1Refno = resultsLocationMapSlotCoordinate1[0].LocationMapSlotCoordinateRefno;
                                slot1Lookup.Add(whsLocMapRefno.ToString() + slotCoord1, coord1Refno);
                            }
                            else
                            {
                                throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 1. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord1 + " " + slotCoord1 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                            }
                        }

                        errMsg += Environment.NewLine + "Slot coordinate 2 \'" + slotCoord2 + "\'";

                        if (!slot2Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord2, out coord2Refno))
                        {
                            var query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                            var resultsLocationMapSlotCoordinate2 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2), slotCoord2, whsLocMapRefno);

                            if (resultsLocationMapSlotCoordinate2?.Count > 0)
                            {
                                coord2Refno = resultsLocationMapSlotCoordinate2[0].LocationMapSlotCoordinateRefno;
                                slot2Lookup.Add(whsLocMapRefno.ToString() + slotCoord2, coord2Refno);
                            }
                            else
                            {
                                throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 2. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord2 + " " + slotCoord2 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                            }
                        }

                        errMsg += Environment.NewLine + "Slot coordinate 3 \'" + slotCoord3 + "\'";

                        if (!slot3Lookup.TryGetValue(whsLocMapRefno.ToString() + slotCoord3, out coord3Refno))
                        {
                            var query = "SELECT * FROM tLocationMapSlotCoordinate WHERE LocationMapSlotCoordinateEnum = ? AND LocationMapSlotCoordinateCode = ? AND WarehouseLocationMapRefno = ?";
                            var resultsLocationMapSlotCoordinate3 = transaction.Query<tLocationMapSlotCoordinateModel>(query, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3), slotCoord3, whsLocMapRefno);

                            if (resultsLocationMapSlotCoordinate3?.Count > 0)
                            {
                                coord3Refno = resultsLocationMapSlotCoordinate3[0].LocationMapSlotCoordinateRefno;
                                slot3Lookup.Add(whsLocMapRefno.ToString() + slotCoord3, coord3Refno);
                            }
                            else
                            {
                                throw new Exception("No results in tLocationMapSlotCoordinate. Slot coord 3. Parameters: " + SysCommon.LocMapSlotCoordTypes.Coord3 + " " + slotCoord3 + " " + whsLocMapRefno + Environment.NewLine + errMsg);
                            }
                        }

                        isLocMapSlotFound = false;

                        if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                        {
                            var results = transaction.Query<tLocationMapSlotModel>("SELECT LocationMapSlotFKRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", slotFKRefno);

                            if (results?.Count > 0)
                            {
                                isLocMapSlotFound = true;

                                var queryUpdate = "UPDATE tLocationMapSlot "
                                                  + "SET SectionLocationMapRefno = " + sectLocMapRefno + ","
                                                  + "LocationMapSlotCode = " + slotFKRefno.ToString()
                                                  + "LocationMapSlotName = \'" + slotName + "\',"
                                                  + "SlotCoord1LocationMapRefno = " + coord1Refno + ","
                                                  + "SlotCoord2LocationMapRefno = " + coord2Refno + ","
                                                  + "SlotCoord3LocationMapRefno = " + coord3Refno + ","
                                                  + "LocationMapSlotBarCode = \'" + slotBarCode + "\' "
                                                  + "WHERE LocationMapSlotFKRefno = " + slotFKRefno;

                                transaction.Execute(queryUpdate);
                            }
                        }

                        if (!isLocMapSlotFound)
                        {
                            transaction.Execute("INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)", Convert.ToInt32(SysCommon.LocMapSubTypes.Slot));

                            locMapRefno = Convert.ToInt32(transaction.ExecuteScalar<int>("SELECT last_insert_rowid() FROM tLocationMap"));

                            var queryInsert = "INSERT INTO tLocationMapSlot " +
                                              "(" +
                                              "LocationMapRefno, " +
                                              "LocationMapSubType, " +
                                              "SectionLocationMapRefno, " +
                                              "LocationMapSlotCode, " +
                                              "LocationMapSlotName, " +
                                              "SlotCoord1LocationMapRefno, " +
                                              "SlotCoord2LocationMapRefno, " +
                                              "SlotCoord3LocationMapRefno, " +
                                              "LocationMapSlotFKRefno, " +
                                              "LocationMapSlotBarCode, " +
                                              "WarehouseLocationMapRefno" +
                                              ") VALUES (" +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?, " +
                                              "?" +
                                              ")";
                            var parametersInsert = new object[]
                            {
                                    locMapRefno,
                                    Convert.ToInt32(SysCommon.LocMapSubTypes.Slot),
                                    sectLocMapRefno,
                                    slotFKRefno,
                                    slotName,
                                    coord1Refno,
                                    coord2Refno,
                                    coord3Refno,
                                    slotFKRefno,
                                    slotBarCode,
                                    whsLocMapRefno,
                            };

                            transaction.Execute(queryInsert, parametersInsert);
                        }

                        ++recordCount;
                        if (recordCount % 100 == 0)
                        {
                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, recordCount / 10 % 100, 0, 100);
                        }
                    } // end of record responseData

                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, 100, 0, 100);

                    transaction.Commit();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Records {recordCount}", false);

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slots failed.", false);

                    throw new Exception("Database exception occurred. Saving slots failed." + Environment.NewLine + errMsg, ex);
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
            var xml = XmlWriter.Create(xmlString);
            var locMapWhs = new TableLocationMapWarehouse();
            var locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (activityItemFkRefno != 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityItemFkRefno);
            }

            var request = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                                          + "<imclient>"
                                          + "<command>"
                                          + "<action>REQUEST SLOTS</action>"
                                          + await SysCommon.GetRequestDefaults(true)
                                          + "<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno.ToString() + "</LocationMapWarehouseFKRefno>"
                                          + "</command>"
                                          + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        public override Task PurgeData()
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
