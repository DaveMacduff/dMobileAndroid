// <copyright file="TableLocationMapSlot.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Types;
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
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlot";
            this.LowerCaseName = "slots";
            this.ProperCaseName = "Slots";
        }

        /// <summary>
        /// The Request.
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        public override async Task RequestData(int whsLocMapRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                if (whsLocMapRefno > 0)
                {
                    var results = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<LocationMapWarehouseModel>(
                        "SELECT * FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", whsLocMapRefno);

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

                var responseData = await this.SendRequest(whsLocMapRefno);
                await this.SaveOpt(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} complete.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);

                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                if (sectRefno == 0)
                {
                    await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapSlot");
                }
                else
                {
                    await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapSlot WHERE SectionLocationMapRefno = ?", sectRefno);
                }

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} complete.", true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName} failed.", true);

                throw new Exception($"Database exception occurred. Purging {this.LowerCaseName} failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSlot"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} slots.", false);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Send.
        /// </summary>
        /// <param name="activityFkRefno</param>
        /// <returns></returns>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            return await this.SendComm(await this.CreateRequest(activityFkRefno));
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
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var slots = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        var refno = await GetLocationMapRefno();
                        slots.Add(new Seg
                        {
                            LocationMapRefno = refno,
                            LocationMapSlotCode = Convert.ToInt32(row["LocationMapSlotCode"]),
                            LocationMapSlotName = row["LocationMapSlotName"].ToString(),
                            LocationMapSlotBarCode = row["LocationMapSlotBarCode"].ToString(),
                            LocationMapSlotFKRefno = Convert.ToInt32(row["LocationMapSlotFKRefno"]),
                            LocationMapSectionFKRefno = Convert.ToInt32(row["LocationMapSectionFKRefno"]),
                            LocationMapSlotCoordinate1Code = row["LocationMapSlotCoordinate1Code"].ToString(),
                            LocationMapSlotCoordinate2Code = row["LocationMapSlotCoordinate2Code"].ToString(),
                            LocationMapSlotCoordinate3Code = row["LocationMapSlotCoordinate3Code"].ToString(),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg "
                                + "("
                                + "LocationMapRefno INTEGER NOT NULL,"
                                + "LocationMapSlotCode INTEGER NOT NULL,"
                                + "LocationMapSlotName VARCHAR(10) NOT NULL,"
                                + "LocationMapSlotBarCode VARCHAR(13) NOT NULL,"
                                + "LocationMapSlotFKRefno INTEGER NOT NULL,"
                                + "LocationMapSectionFKRefno INTEGER NOT NULL,"
                                + "LocationMapSlotCoordinate1Code VARCHAR(10) NOT NULL,"
                                + "LocationMapSlotCoordinate2Code VARCHAR(10) NOT NULL,"
                                + "LocationMapSlotCoordinate3Code VARCHAR(10) NOT NULL"
                                + ")";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(slots);

                    query = "INSERT INTO tLocationMapSlot (" +
                        "LocationMapRefno" +
                        ",LocationMapSubType" +
                        ",SectionLocationMapRefno" +
                        ",LocationMapSlotCode" +
                        ",LocationMapSlotName" +
                        ",SlotCoord1LocationMapRefno" +
                        ",SlotCoord2LocationMapRefno" +
                        ",SlotCoord3LocationMapRefno" +
                        ",LocationMapSlotFKRefno" +
                        ",LocationMapSlotBarCode" +
                        ",WarehouseLocationMapRefno" +
                        ") SELECT Seg.LocationMapRefno" +
                        $",{Convert.ToInt32(SysCommon.LocMapSubTypes.Slot)} as LocationMapSubType" +
                        ",Sect.LocationMapRefno as SectionLocationMapRefno" +
                        ",Seg.LocationMapSlotCode" +
                        ",Seg.LocationMapSlotName" +
                        ",Coord1.LocationMapSlotCoordinateRefno as SlotCoord1LocationMapRefno" +
                        ",Coord2.LocationMapSlotCoordinateRefno as SlotCoord2LocationMapRefno" +
                        ",Coord3.LocationMapSlotCoordinateRefno as SlotCoord3LocationMapRefno" +
                        ",Seg.LocationMapSlotFKRefno" +
                        ",Seg.LocationMapSlotBarCode" +
                        ",Whs.LocationMapRefno as WarehouseLocationMapRefno" +
                        " FROM Seg" +
                        " INNER JOIN tLocationMapSection Sect ON Sect.LocationMapSectionFKRefno = Seg.LocationMapSectionFKRefno" +
                        " INNER JOIN tLocationMapWarehouse Whs ON Whs.LocationMapRefno = Sect.WarehouseLocationMapRefno" +
                        $" INNER JOIN tLocationMapSlotCoordinate Coord1 ON Coord1.LocationMapSlotCoordinateEnum = {Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1)}" +
                        " AND Coord1.WarehouseLocationMapRefno = Sect.WarehouseLocationMapRefno" +
                        " AND Coord1.LocationMapSlotCoordinateCode = Seg.LocationMapSlotCoordinate1Code" +
                        $" INNER JOIN tLocationMapSlotCoordinate Coord2 ON Coord2.LocationMapSlotCoordinateEnum = {Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2)}" +
                        " AND Coord2.WarehouseLocationMapRefno = Sect.WarehouseLocationMapRefno" +
                        " AND Coord2.LocationMapSlotCoordinateCode = Seg.LocationMapSlotCoordinate2Code" +
                        $" INNER JOIN tLocationMapSlotCoordinate Coord3 ON Coord3.LocationMapSlotCoordinateEnum = {Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3)}" +
                        " AND Coord3.WarehouseLocationMapRefno = Sect.WarehouseLocationMapRefno" +
                        " AND Coord3.LocationMapSlotCoordinateCode = Seg.LocationMapSlotCoordinate3Code";
                    var rowsAffected = await conn.ExecuteAsync(query);
                    if (slots.Count != rowsAffected)
                    {
                        throw new Exception("Failed to insert all rows into tLocationMapSlot");
                    }

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);
                }

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"{rows.Count} {this.LowerCaseName}.", true);
            }
            else
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    $"No {this.LowerCaseName} to save.",
                    true);
            }

            return;

            ///////////////////////////////////////////////////////////
            async Task<int> GetLocationMapRefno()
            {
                var query = "INSERT INTO tLocationMap (LocationMapSubType) VALUES (?)";
                await conn.ExecuteAsync(query, Convert.ToInt32(SysCommon.LocMapSubTypes.Slot));
                query = "SELECT tLocationMap.LocationMapRefno FROM tLocationMap ORDER BY LocationMapRefno DESC LIMIT 1";
                var locMapRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(query));
                return locMapRefno;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <param name="activityFkRefno"></param>
        /// <returns></returns>
        protected override async Task<string> CreateRequest(int activityFkRefno)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var locMapWhs = new TableLocationMapWarehouse();
            var locMapWhsFKRefno = 0;

            // lookup and verify warehouse
            if (activityFkRefno != 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityFkRefno);
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
            await xml.FlushAsync();
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

        protected override Task<string> CreateRequest(XDocument criteriaFilter)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(string familyCode)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int activityFkRefno, XDocument criteria)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequest(int prodLocMapType, int whsLocMapFkRefno)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> CreateRequestProdLoc(int prodLocMapType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            public int LocationMapRefno { get; set; }

            public int LocationMapSlotCode { get; set; }

            public string LocationMapSlotName { get; set; }

            public string LocationMapSlotBarCode { get; set; }

            public int LocationMapSlotFKRefno { get; set; }

            public int LocationMapSectionFKRefno { get; set; }

            public string LocationMapSlotCoordinate1Code { get; set; }

            public string LocationMapSlotCoordinate2Code { get; set; }

            public string LocationMapSlotCoordinate3Code { get; set; }
        }
    }
}
