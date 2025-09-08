// <copyright file="TableLocationMapSection.cs" company="InterSect Business Systems Inc">
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
    using Bedrock;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableLocationMapSection" />.
    /// </summary>
    public class TableLocationMapSection : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSection"/> class.
        /// </summary>
        public TableLocationMapSection()
      : base()
        {
            this.LowerCaseName = "sections";
            this.ProperCaseName = "Sections";
        }

        public override async Task RequestData(int whsLocMapRefno)
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName}.", true);
                var responseData = await this.SendRequest(whsLocMapRefno);
                await this.Save(responseData);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{this.LowerCaseName} seeded.", true);
            }
            catch (Exception ex)
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Seeding {this.LowerCaseName} failed.", true);
                throw new Exception($"Seeding {this.LowerCaseName} failed.", ex);
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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM tLocationMapSection"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} sections.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Sections status failed.", ex);
            }

            return rowsAffected;
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
        /// <exception cref="Exception">Thrown if no records are found for the specified <paramref name="activityFkRefno"/> in the database.</exception>
        protected override async Task<string> SendRequest(int activityFkRefno)
        {
            return await SendComm(await this.CreateRequest(activityFkRefno));
        }

        /// <inheritdoc cref="TableBase.Save(string)"/>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var rows = dataSet.Tables[0].Rows;
                if (rows.Count > 0)
                {
                    var sections = new List<Seg>();
                    foreach (DataRow row in rows)
                    {
                        var refno = await GetLocationMapRefno();
                        sections.Add(new Seg
                        {
                            LocationMapRefno = refno,
                            LocationMapSubType = Convert.ToInt32(row["LocationMapSubType"]),
                            LocationMapWarehouseFKRefno = Convert.ToInt32(row["LocationMapWarehouseFKRefno"]),
                            LocationMapSectionCode = Convert.ToInt32(row["LocationMapSectionCode"]),
                            LocationMapSectionName = Convert.ToString(row["LocationMapSectionName"]),
                            LocationMapSectionFKRefno = Convert.ToInt32(row["LocationMapSectionFKRefno"]),
                            LocationMapSectionBarCode = Convert.ToString(row["LocationMapSectionBarCode"]),
                        });
                    }

                    var query = "CREATE TEMP TABLE Seg"
                                + "("
                                + "LocationMapRefno INTEGER NOT NULL,"
                                + "LocationMapSubType INTEGER NOT NULL,"
                                + "LocationMapWarehouseFKRefno INTEGER NOT NULL,"
                                + "LocationMapSectionFKRefno INTEGER NOT NULL,"
                                + "LocationMapSectionCode INTEGER NOT NULL,"
                                + "LocationMapSectionName VARCHAR(10) NOT NULL,"
                                + "LocationMapSectionBarCode VARCHAR(13) NOT NULL"
                                + ")";
                    await conn.ExecuteAsync(query);
                    await conn.InsertAllAsync(sections);

                    query = "INSERT INTO tLocationMapSection (" +
                            "LocationMapRefno" +
                            ",LocationMapSubType" +
                            ",WarehouseLocationMapRefno" +
                            ",LocationMapSectionCode" +
                            ",LocationMapSectionName" +
                            ",LocationMapSectionFKRefno" +
                            ",LocationMapSectionBarCode" +
                            ") SELECT Seg.LocationMapRefno" +
                            ",Seg.LocationMapSubType" +
                            ",tLocationMapWarehouse.LocationMapRefno as WarehouseLocationMapRefno" +
                            ",Seg.LocationMapSectionCode" +
                            ",Seg.LocationMapSectionName" +
                            ",Seg.LocationMapSectionFKRefno" +
                            ",Seg.LocationMapSectionBarCode" +
                            " FROM Seg" +
                            " INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapWarehouseFKRefno = Seg.LocationMapWarehouseFKRefno";
                    await conn.ExecuteAsync(query);

                    // Drop the temporary table
                    const string dropTempTableQuery = "DROP TABLE IF EXISTS Seg";
                    await conn.ExecuteAsync(dropTempTableQuery);

                    this.DataAccessStatusOutHandler(
                        this,
                        EventArgs.Empty,
                        $"{sections.Count} sections.", true);
                }
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
                await conn.ExecuteAsync(query, Convert.ToInt32(SysCommon.LocMapSubTypes.Section));
                query = "SELECT last_insert_rowid()";
                ////query = "SELECT tLocationMap.LocationMapRefno FROM tLocationMap ORDER BY LocationMapRefno DESC LIMIT 1";
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
            if (activityFkRefno > 0)
            {
                locMapWhsFKRefno = await locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(activityFkRefno);
            }

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SECTIONS</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>"
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Class to hold the data for the temporary table.
        /// </summary>
        private class Seg
        {
            public int LocationMapRefno { get; set; }

            public int LocationMapSubType { get; set; }

            public int LocationMapWarehouseFKRefno { get; set; }

            public int LocationMapSectionFKRefno { get; set; }

            public int LocationMapSectionCode { get; set; }

            public string LocationMapSectionName { get; set; }

            public string LocationMapSectionBarCode { get; set; }
        }
    }
}
