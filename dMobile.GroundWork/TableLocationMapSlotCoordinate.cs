// <copyright file="TableLocationMapSlotCoordinate.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models.LocationMap;
    using dMobile.Resources.Types;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="TableLocationMapSlotCoordinate" />.
    /// </summary>
    public class TableLocationMapSlotCoordinate : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableLocationMapSlotCoordinate"/> class.
        /// </summary>
        public TableLocationMapSlotCoordinate()
        {
            this.FullClassName = "dMobile.GroundWork.TableLocationMapSlotCoordinate";
            this.LowerCaseName = "slot coordinates";
            this.ProperCaseName = "Slot coordinates";
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"Purging {this.LowerCaseName}.", true);

                await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync("DELETE FROM tLocationMapSlotCoordinate");

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
                rowsAffected = Convert.ToInt32(await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tLocationMapSlotCoordinate"));

                this.DataAccessStatusOutHandler(this, EventArgs.Empty, $"{rowsAffected} slot coordinates.", true);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database exception occurred. {this.ProperCaseName} status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// The Save.
        /// </summary>
        /// <param name="responseData"></param>
        protected override async Task Save(string responseData)
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            using var dataSet = new DataSet();
            dataSet.ReadXml(SysCommon.GetXmlReader(ref responseData), XmlReadMode.Auto);
            if (dataSet.Tables.Count > 0)
            {
                var coords = new List<LocationMapSlotCoordinateModel>();
                var rowsToSave = dataSet.Tables["tLocationMapSlotCoordinate"].Rows.Count;
                for (var i = 0; i < rowsToSave; i++)
                {
                    var row = dataSet.Tables["tLocationMapSlotCoordinate"].Rows[i];
                    var coord = new LocationMapSlotCoordinateModel
                    {
                        LocationMapSlotCoordinateEnum = Convert.ToInt32(
                            row["LocationMapSlotCoordinateEnum"]),
                        WarehouseLocationMapRefno = await GetWarehouseLocMapRefno(row),
                        LocationMapSlotCoordinateCode = Convert.ToString(
                            row["LocationMapSlotCoordinateCode"]),
                    };
                    coords.Add(coord);
                }

                await conn.InsertAllAsync(coords);
                this.DataAccessStatusOutHandler(
                    this,
                EventArgs.Empty,
                    $"{coords.Count} slot coordinates.", true);
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
            async Task<int> GetWarehouseLocMapRefno(DataRow drow)
            {
                var whsFkRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
                const string query = "SELECT LocationMapRefno FROM tLocationMapWarehouse" +
                                     " WHERE LocationMapWarehouseFKRefno = ?";
                var whsLocMapRefno = Convert.ToInt32(
                    await conn.ExecuteScalarAsync<int>(query, whsFkRefno));
                return whsLocMapRefno;
            }
        }

        /// <summary>
        /// The Create.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> CreateRequest()
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST SLOT COORDINATES</action>"
                + await SysCommon.GetRequestDefaults(true)
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }
    }
}
