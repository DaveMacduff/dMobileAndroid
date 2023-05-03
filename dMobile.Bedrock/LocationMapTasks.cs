// <copyright file="LocationMapTasks.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using dMobile.Resources.QueryResults;
    using dMobile.Resources.Types;

    /// <summary>
    /// Defines the <see cref="LocationMapTasks" /> Location map tasks.
    /// </summary>
    public static class LocationMapTasks
    {
        /// <summary>
        /// Get the location map reference number from a bar code global location identifier.
        /// </summary>
        /// <param name="globalLocationIdentifier"><see cref="string"/> Bar code global location identifier.</param>
        /// <returns><see cref="int"/> Found location map reference number or zero.</returns>
        public static async Task<int> GetLocMapRefnoFromBarCodeGlobalLocIdentifier(string globalLocationIdentifier)
        {
            var foundLocMapRefno = 0;
            var barCodeSettings = new SysBarCodeSettings().LoadData();

            // This option was needed because of a screw up calculating the location check digit.
            // Need to ignore the check digit when attempting to find the location.
            var ignoreCheckDigitAttempt =
                barCodeSettings.IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;
            var barCodeCheckDigitRemoved = globalLocationIdentifier.Substring(
                0,
                globalLocationIdentifier.TrimEnd().Length - 1);

            // Because we each warehouse has it's own map structure now we can't assume the tracking map.
            // So this will be a little slower, plus the extra parsing rules to include.

            // first attempt to find the location as a complete SLOT bar code
            var locationMapRefnoResults = await DatabaseConnection.GetInstance().
                SQLConn.QueryAsync<tLocationMapSlotModel>(
                    "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotBarCode = ?",
                    globalLocationIdentifier);

            if (locationMapRefnoResults != null && locationMapRefnoResults.Count > 0)
            {
                foundLocMapRefno = Convert.ToInt32(locationMapRefnoResults[0].LocationMapRefno);
            }

            if (foundLocMapRefno == 0 && ignoreCheckDigitAttempt)
            {
                foundLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                    ExecuteScalarAsync<int>(
                    "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotBarCode LIKE ?",
                    barCodeCheckDigitRemoved + "%"));
            }

            // second attempt to find the location as a complete SECTION bar code
            if (foundLocMapRefno == 0)
            {
                var tLocationMapSectionModelResults = await DatabaseConnection.
                    GetInstance().SQLConn.QueryAsync<tLocationMapSectionModel>(
                    "SELECT LocationMapRefno FROM tLocationMapSection" +
                    " WHERE LocationMapSectionBarCode = ?",
                    globalLocationIdentifier);

                if (tLocationMapSectionModelResults != null && tLocationMapSectionModelResults.Count > 0)
                {
                    foundLocMapRefno = Convert.ToInt32(tLocationMapSectionModelResults[0].LocationMapRefno);
                }
            }

            if (foundLocMapRefno == 0 && ignoreCheckDigitAttempt)
            {
                foundLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                    ExecuteScalarAsync<int>(
                    "SELECT LocationMapRefno FROM tLocationMapSection" +
                    " WHERE LocationMapSectionBarCode LIKE ?",
                    barCodeCheckDigitRemoved + "%"));
            }

            // third attempt to find the location as a complete WAREHOUSE bar code
            if (foundLocMapRefno == 0)
            {
                var tLocationMapWarehouseModelResults =
                    await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>(
                        "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseBarCode = ?",
                        globalLocationIdentifier);

                if (tLocationMapWarehouseModelResults != null && tLocationMapWarehouseModelResults.Count > 0)
                {
                    foundLocMapRefno = Convert.ToInt32(tLocationMapWarehouseModelResults[0].LocationMapRefno);
                }
            }

            if (foundLocMapRefno == 0 && ignoreCheckDigitAttempt)
            {
                foundLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                    ExecuteScalarAsync<int>(
                    "SELECT LocationMapRefno FROM tLocationMapWarehouse" +
                    " WHERE LocationMapWarehouseBarCode LIKE ?",
                    barCodeCheckDigitRemoved + "%"));
            }

            return foundLocMapRefno;
        }

        /// <summary>
        /// Assign location map reference number argument with the current warehouse or login warehouse if unassigned.
        /// </summary>
        /// <remarks>
        /// Make sure the location map reference number is at least a warehouse.
        /// This method is used mostly with activity line items or activity headers objects.
        /// Gets a default value for an argument value to the GetActivityItem method for example.
        /// </remarks>
        /// <param name="locationMapRefno"><see cref="int"/> Location map reference number to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"> Location map reference is less than or equal to zero.</exception>
        /// <returns><see cref="int"/> Assigned location map reference number.</returns>
        public static int DefaultLocMapRefnoToWhsIfUnassigned(int locationMapRefno)
        {
            locationMapRefno = locationMapRefno == 0 ? LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno : locationMapRefno;
            locationMapRefno = locationMapRefno == 0 ? LocationMap.GetInstance().LoginWhsLocMapRefno : locationMapRefno;

            if (locationMapRefno == 0)
            {
                throw new ArgumentOutOfRangeException("locationMapRefno == 0");
            }

            return locationMapRefno;
        }

        /// <summary>
        /// Get a list of warehouses.
        /// </summary>
        /// <returns><see cref="Task{T}"/> List of warehouses.</returns>
        public static async Task<List<Warehouse>> GetWhsList()
        {
            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Warehouse>(
                "SELECT LocationMapWarehouseCode, LocationMapWarehouseName, LocationMapRefno" +
                " FROM tLocationMapWarehouse ORDER BY LocationMapWarehouseCode");
        }

        /// <summary>
        /// Get a list of sections.
        /// </summary>
        /// <returns><see cref="Task{T}"/> List of sections.</returns>
        public static async Task<List<Section>> GetSectList()
        {
            var sql = "SELECT LocationMapSectionCode,"
                      + "LocationMapSectionName,"
                      + "LocationMapRefno,"
                      + "WarehouseLocationMapRefno "
                      + "FROM tLocationMapSection "
                      + "ORDER BY LocationMapSectionCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Section>(sql);
        }

        /// <summary>
        /// Get the section name from a section location map foreign key reference number.
        /// </summary>
        /// <param name="locMapSectFkRefno"><see cref="int"/> Section location map foreign key reference number.</param>
        /// <returns>Name of the section.</returns>
        public static async Task<string> GetSectionName(int locMapSectFkRefno)
        {
            var results = await DatabaseConnection.GetInstance().
                SQLConn.QueryAsync<tLocationMapSectionModel>(
                "SELECT LocationMapSectionName FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                Convert.ToInt32(locMapSectFkRefno));

            return Convert.ToString(results[0].LocationMapSectionName);
        }

        /// <summary>
        /// Get the slot name from a slot location map foreign key reference number.
        /// </summary>
        /// <param name="locMapSlotFkRefno"><see cref="int"/> Slot location map foreign key reference number.</param>
        /// <returns>Name of the slot.</returns>
        public static async Task<string> GetSlotName(int locMapSlotFkRefno)
        {
            var results = await DatabaseConnection.GetInstance().SQLConn.
                QueryAsync<tLocationMapSlotModel>(
                "SELECT LocationMapSlotName FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                Convert.ToInt32(locMapSlotFkRefno));

            return Convert.ToString(results[0].LocationMapSlotName);
        }

        /// <summary>
        /// Get a list of slot 1 coordinates.
        /// </summary>
        /// <returns><see cref="Task{T}"/> List of slot 1 coordinates.</returns>
        public static async Task<List<Slot>> GetSlotCoord1List()
        {
            var sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                      + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                      + "tLocationMapSlot.SectionLocationMapRefno "
                      + "FROM tLocationMapSlot "
                      + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                      + "= tLocationMapSlot.SlotCoord1LocationMapRefno "
                      + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
                      + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(
                sql,
                Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1));
        }

        /// <summary>
        /// Get a list of slot 2 coordinates.
        /// </summary>
        /// <returns><see cref="Task{List{Slot}}"/> List of slot 2 coordinates.</returns>
        public static async Task<List<Slot>> GetSlotCoord2List()
        {
            var sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                      + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                      + "tLocationMapSlot.SectionLocationMapRefno "
                      + "FROM tLocationMapSlot "
                      + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                      + "= tLocationMapSlot.SlotCoord2LocationMapRefno "
                      + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
                      + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(
                sql,
                Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2));
        }

        /// <summary>
        /// Get a list of slot 3 coordinates.
        /// </summary>
        /// <returns><see cref="Task{T}"/> List of slot 3 coordinates.</returns>
        public static async Task<List<Slot>> GetSlotCoord3List()
        {
            const string sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                               + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                               + "tLocationMapSlot.SectionLocationMapRefno "
                               + "FROM tLocationMapSlot "
                               + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                               + "= tLocationMapSlot.SlotCoord3LocationMapRefno "
                               + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
                               + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(
                sql,
                Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3));
        }

        /// <summary>
        /// Get the slot location map reference number from slot coordinate reference number's.
        /// </summary>
        /// <param name="sectionLocMapRefno"><see cref="int"/> Section location map reference number.</param>
        /// <param name="slotCoord1Refno"><see cref="int"/> Slot coordinate 1 reference number.</param>
        /// <param name="slotCoord2Refno"><see cref="int"/> Slot coordinate 2 reference number.</param>
        /// <param name="slotCoord3Refno"><see cref="int"/> Slot coordinate 3 reference number.</param>
        /// <returns>.</returns>
        public static async Task<int> GetSlotRefnoFromCoordinates(
            int sectionLocMapRefno,
            int slotCoord1Refno,
            int slotCoord2Refno,
            int slotCoord3Refno)
        {
            var slotLocMapRefno = 0;
            var locMap = LocationMap.GetInstance();
            var query = string.Empty;
            var parameters = new List<object>();

            switch (locMap.CurrLocMap.IsSlotCoord1Tracked)
            {
                case true when locMap.CurrLocMap.IsSlotCoord2Tracked &&
                               locMap.CurrLocMap.IsSlotCoord3Tracked:
                    query = "SELECT tLocationMapSlot.LocationMapRefno "
                            + "FROM tLocationMapSlot "
                            + "WHERE tLocationMapSlot.SectionLocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord2LocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord3LocationMapRefno = ?";
                    parameters.Add(Convert.ToInt32(sectionLocMapRefno));
                    parameters.Add(Convert.ToInt32(slotCoord1Refno));
                    parameters.Add(Convert.ToInt32(slotCoord2Refno));
                    parameters.Add(Convert.ToInt32(slotCoord3Refno));
                    break;

                case true when locMap.CurrLocMap.IsSlotCoord2Tracked:
                    query = "SELECT tLocationMapSlot.LocationMapRefno "
                            + "FROM tLocationMapSlot "
                            + "WHERE tLocationMapSlot.SectionLocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord2LocationMapRefno = ?";
                    parameters.Add(Convert.ToInt32(sectionLocMapRefno));
                    parameters.Add(Convert.ToInt32(slotCoord1Refno));
                    parameters.Add(Convert.ToInt32(slotCoord2Refno));
                    break;

                case true:
                    query = "SELECT tLocationMapSlot.LocationMapRefno "
                            + "FROM tLocationMapSlot "
                            + "WHERE tLocationMapSlot.SectionLocationMapRefno = ?"
                            + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = ?";
                    parameters.Add(Convert.ToInt32(sectionLocMapRefno));
                    parameters.Add(Convert.ToInt32(slotCoord1Refno));
                    break;

                default:
                    // throw new Exception("Location map tracking map settings are not set.");
                    break;
            }

            var results = await DatabaseConnection.GetInstance().SQLConn.
                QueryAsync<tLocationMapSlotModel>(query, parameters.ToArray());

            if (results != null && results.Count > 0)
            {
                slotLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
            }

            return slotLocMapRefno;
        }

        /// <summary>
        /// Check if we have a complete location map.
        /// </summary>
        /// <param name="locMap"><see cref="SLocMap"/> Location map.</param>
        /// <returns><see cref="bool"/> True if the location map is complete else false.</returns>
        public static async Task<bool> IsCompleteLocMap(SLocMap locMap)
        {
            bool isComplete;
            if (locMap.IsSlotTracked)
            {
                isComplete = await IsCompleteLocMap(locMap.SlotLocMapRefno);
            }
            else if (locMap.IsSectionTracked)
            {
                isComplete = await IsCompleteLocMap(locMap.SectLocMapRefno);
            }
            else
            {
                isComplete = await IsCompleteLocMap(locMap.WhsLocMapRefno);
            }

            return isComplete;
        }

        /// <summary>
        /// Check if we have a complete location map.
        /// </summary>
        /// <param name="locMapRefno"><see cref="int"/> Location map reference number.</param>
        /// <returns><see cref="bool"/> True if the location map is complete else false.</returns>
        public static async Task<bool> IsCompleteLocMap(int locMapRefno)
        {
            var isComplete = false;
            var results = await DatabaseConnection.GetInstance().SQLConn
                .QueryAsync<tLocationMapModel>(
                "SELECT * FROM tLocationMap WHERE LocationMapRefno = ?",
                Convert.ToInt32(locMapRefno));
            if (results != null && results.Count > 0)
            {
                var locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                var locMapSubType = (SysCommon.LocMapSubTypes)results[0].LocationMapSubType;
                if (locMap.IsSlotTracked && locMapSubType == SysCommon.LocMapSubTypes.Slot)
                {
                    isComplete = true;
                }
                else if (locMap.IsSectionTracked && locMapSubType == SysCommon.LocMapSubTypes.Section)
                {
                    isComplete = true;
                }
                else if (locMapSubType == SysCommon.LocMapSubTypes.Warehouse)
                {
                    isComplete = true;
                }
            }

            return isComplete;
        }

        /// <summary>
        /// Get the location map from a location map foreign key reference number's.
        /// </summary>
        /// <param name="warehouseLocMapFkRefno"><see cref="int"/> Warehouse location map foreign key reference number.</param>
        /// <param name="sectionLocMapFkRefno"><see cref="int"/> Section location map foreign key reference number.</param>
        /// <param name="slotLocMapFkRefno"><see cref="int"/> Slot location map foreign key reference number.</param>
        /// <returns><see cref="SLocMapAndRefno"/> Location map.</returns>
        public static async Task<SLocMapAndRefno> GetSLocMapFromLocMapFkRefno(
            int warehouseLocMapFkRefno,
            int sectionLocMapFkRefno,
            int slotLocMapFkRefno)
        {
            var tempLocMapRefno = 0;
            if (slotLocMapFkRefno != 0)
            {
                var results = await DatabaseConnection.GetInstance().SQLConn.
                    QueryAsync<tLocationMapModel>(
                    "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?",
                    Convert.ToInt32(slotLocMapFkRefno));
                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            if (tempLocMapRefno == 0 & sectionLocMapFkRefno != 0)
            {
                var results = await DatabaseConnection.GetInstance().
                    SQLConn.QueryAsync<tLocationMapSectionModel>(
                    "SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?",
                    Convert.ToInt32(sectionLocMapFkRefno));
                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            if (tempLocMapRefno == 0 & warehouseLocMapFkRefno != 0)
            {
                var results = await DatabaseConnection.GetInstance().
                    SQLConn.QueryAsync<tLocationMapWarehouseModel>(
                    "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?",
                    Convert.ToInt32(warehouseLocMapFkRefno));
                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            var retLocMap = await GetSLocMapFromLocMapRefno(tempLocMapRefno);
            return new SLocMapAndRefno { SLocMap = retLocMap, LocMapRefno = tempLocMapRefno };
        }

        /// <summary>
        /// Get the location map from a location map reference number.
        /// </summary>
        /// <param name="locMapRefno"><see cref="int"/> Location map reference number.</param>
        /// <returns><see cref="SLocMap"/> Location map.</returns>
        public static async Task<SLocMap> GetSLocMapFromLocMapRefno(int locMapRefno)
        {
            var locMapInfo = new SLocMap();
            var locMapSubType = SysCommon.LocMapSubTypes.None;
            var conn = DatabaseConnection.GetInstance().SQLConn;

            var tLocationMapResults = await DatabaseConnection.GetInstance().
                SQLConn.QueryAsync<tLocationMapModel>(
                "SELECT * FROM tLocationMap WHERE LocationMapRefno = ?",
                Convert.ToInt32(locMapRefno));

            if (tLocationMapResults != null && tLocationMapResults.Count > 0)
            {
                locMapSubType = (SysCommon.LocMapSubTypes)tLocationMapResults[0].LocationMapSubType;
            }

            switch (locMapSubType)
            {
                case SysCommon.LocMapSubTypes.Slot:
                    {
                        const string query = "SELECT tLocationMapSlot.LocationMapRefno, "
                                             + "tLocationMapSlot.LocationMapSlotCode, "
                                             + "tLocationMapSlot.LocationMapSlotName, "
                                             + "tLocationMapSlot.LocationMapSlotFKRefno, "
                                             + "tLocationMapSlot.SectionLocationMapRefno, "
                                             + "tLocationMapSlot.SlotCoord1LocationMapRefno, "
                                             + "tLocationMapSlot.SlotCoord2LocationMapRefno, "
                                             + "tLocationMapSlot.SlotCoord3LocationMapRefno, "
                                             + "tLocationMapSection.LocationMapSectionCode, "
                                             + "tLocationMapSection.LocationMapSectionName, "
                                             + "tLocationMapSection.LocationMapSectionFKRefno, "
                                             + "tLocationMapSection.WarehouseLocationMapRefno, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseName, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseCode, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsWMS,  "
                                             + "tLocationMapWarehouse.LocationMapWarehouseFKRefno, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSectionMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSectionTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSlotTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseAdhocEnabled "
                                             + "FROM tLocationMapSlot "
                                             + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno =" +
                                             " tLocationMapSlot.SectionLocationMapRefno "
                                             + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno =" +
                                             " tLocationMapSection.WarehouseLocationMapRefno "
                                             + "WHERE tLocationMapSlot.LocationMapRefno = ?";

                        var results = await conn.QueryAsync<tLocationMapSlotQueryResult>(
                            query,
                            Convert.ToInt32(locMapRefno));

                        if (results != null && results.Count > 0)
                        {
                            locMapInfo.SlotLocMapRefno = results[0].LocationMapRefno;
                            locMapInfo.SlotCode = results[0].LocationMapSlotCode;
                            locMapInfo.SlotName = results[0].LocationMapSlotName;
                            locMapInfo.SlotFKRefno = results[0].LocationMapSlotFKRefno;
                            locMapInfo.SlotCoord1LocMapRefno = results[0].SlotCoord1LocationMapRefno;
                            locMapInfo.SlotCoord2LocMapRefno = results[0].SlotCoord2LocationMapRefno;
                            locMapInfo.SlotCoord3LocMapRefno = results[0].SlotCoord3LocationMapRefno;
                            locMapInfo.SectLocMapRefno = results[0].SectionLocationMapRefno;
                            locMapInfo.SectCode = results[0].LocationMapSectionCode;
                            locMapInfo.SectName = results[0].LocationMapSectionName;
                            locMapInfo.SectFKRefno = results[0].LocationMapSectionFKRefno;
                            locMapInfo.WhsLocMapRefno = results[0].WarehouseLocationMapRefno;
                            locMapInfo.WhsName = results[0].LocationMapWarehouseName;
                            locMapInfo.WhsCode = results[0].LocationMapWarehouseCode;
                            locMapInfo.IsWhsWMS = results[0].LocationMapWarehouseIsWMS;
                            locMapInfo.WhsFKRefno = results[0].LocationMapWarehouseFKRefno;
                            locMapInfo.WhsCodeMask = results[0].LocationMapWarehouseMask;
                            locMapInfo.SectMask = results[0].LocationMapWarehouseSectionMask;
                            locMapInfo.IsSectionTracked = results[0].LocationMapWarehouseIsSectionTracked;
                            locMapInfo.IsSlotTracked = results[0].LocationMapWarehouseIsSlotTracked;
                            locMapInfo.SlotCoord1Name = results[0].LocationMapWarehouseSlotCoordinate1Name;
                            locMapInfo.SlotCoord2Name = results[0].LocationMapWarehouseSlotCoordinate2Name;
                            locMapInfo.SlotCoord3Name = results[0].LocationMapWarehouseSlotCoordinate3Name;
                            locMapInfo.SlotMask = results[0].LocationMapWarehouseSlotMask;
                            locMapInfo.SlotCoord1Mask = results[0].LocationMapWarehouseSlotCoordinate1Mask;
                            locMapInfo.SlotCoord2Mask = results[0].LocationMapWarehouseSlotCoordinate2Mask;
                            locMapInfo.SlotCoord3Mask = results[0].LocationMapWarehouseSlotCoordinate3Mask;
                        }

                        break;
                    }

                case SysCommon.LocMapSubTypes.Section:
                    {
                        const string query = "SELECT tLocationMapSection.LocationMapRefno, "
                                             + "tLocationMapSection.LocationMapSectionCode, "
                                             + "tLocationMapSection.LocationMapSectionName, "
                                             + "tLocationMapSection.LocationMapSectionFKRefno, "
                                             + "tLocationMapSection.WarehouseLocationMapRefno, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseName, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseCode, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsWMS, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseFKRefno, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSectionMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSectionTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSlotTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseAdhocEnabled "
                                             + "FROM tLocationMapSection "
                                             + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                                             + "= tLocationMapSection.WarehouseLocationMapRefno "
                                             + "WHERE tLocationMapSection.LocationMapRefno = ?";

                        var results = await conn.QueryAsync<tLocationMapSectionQueryResult>(
                        query,
                        Convert.ToInt32(locMapRefno));

                        if (results != null && results.Count > 0)
                        {
                            locMapInfo.SectLocMapRefno = results[0].LocationMapRefno;
                            locMapInfo.SectCode = results[0].LocationMapSectionCode;
                            locMapInfo.SectName = results[0].LocationMapSectionName;
                            locMapInfo.SectFKRefno = results[0].LocationMapSectionFKRefno;
                            locMapInfo.WhsLocMapRefno = results[0].WarehouseLocationMapRefno;
                            locMapInfo.WhsName = results[0].LocationMapWarehouseName;
                            locMapInfo.WhsCode = results[0].LocationMapWarehouseCode;
                            locMapInfo.IsWhsWMS = results[0].LocationMapWarehouseIsWMS;
                            locMapInfo.WhsFKRefno = results[0].LocationMapWarehouseFKRefno;
                            locMapInfo.WhsCodeMask = results[0].LocationMapWarehouseMask;
                            locMapInfo.SectMask = results[0].LocationMapWarehouseSectionMask;
                            locMapInfo.IsSectionTracked = results[0].LocationMapWarehouseIsSectionTracked;
                            locMapInfo.IsSlotTracked = results[0].LocationMapWarehouseIsSlotTracked;
                            locMapInfo.SlotCoord1Name = results[0].LocationMapWarehouseSlotCoordinate1Name;
                            locMapInfo.SlotCoord2Name = results[0].LocationMapWarehouseSlotCoordinate2Name;
                            locMapInfo.SlotCoord3Name = results[0].LocationMapWarehouseSlotCoordinate3Name;
                            locMapInfo.SlotMask = results[0].LocationMapWarehouseSlotMask;
                            locMapInfo.SlotCoord1Mask = results[0].LocationMapWarehouseSlotCoordinate1Mask;
                            locMapInfo.SlotCoord2Mask = results[0].LocationMapWarehouseSlotCoordinate2Mask;
                            locMapInfo.SlotCoord3Mask = results[0].LocationMapWarehouseSlotCoordinate3Mask;
                        }

                        break;
                    }

                case SysCommon.LocMapSubTypes.Warehouse:
                    {
                        const string query = "SELECT tLocationMapWarehouse.LocationMapRefno, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseName, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseCode, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsWMS, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseFKRefno,  "
                                             + "tLocationMapWarehouse.LocationMapWarehouseMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSectionMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotMask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Mask, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Name, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSectionTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseIsSlotTracked, "
                                             + "tLocationMapWarehouse.LocationMapWarehouseAdhocEnabled "
                                             + "FROM tLocationMapWarehouse "
                                             + "WHERE tLocationMapWarehouse.LocationMapRefno = ?";

                        var results = await conn.QueryAsync<tLocationMapWarehouseModel>(
                        query,
                        Convert.ToInt32(locMapRefno));

                        if (results != null && results.Count > 0)
                        {
                            locMapInfo.WhsLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                            locMapInfo.WhsName = Convert.ToString(results[0].LocationMapWarehouseName);
                            locMapInfo.WhsCode = Convert.ToInt32(results[0].LocationMapWarehouseCode);
                            locMapInfo.IsWhsWMS = Convert.ToBoolean(results[0].LocationMapWarehouseIsWMS);
                            locMapInfo.WhsFKRefno = Convert.ToInt32(results[0].LocationMapWarehouseFKRefno);
                            locMapInfo.WhsCodeMask = Convert.ToString(results[0].LocationMapWarehouseMask);
                            locMapInfo.SectMask = Convert.ToString(results[0].LocationMapWarehouseSectionMask);
                            locMapInfo.IsSectionTracked = Convert.ToBoolean(results[0].LocationMapWarehouseIsSectionTracked);
                            locMapInfo.IsSlotTracked = Convert.ToBoolean(results[0].LocationMapWarehouseIsSlotTracked);
                            locMapInfo.SlotCoord1Name = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate1Name);
                            locMapInfo.SlotCoord2Name = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate2Name);
                            locMapInfo.SlotCoord3Name = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate3Name);
                            locMapInfo.SlotMask = Convert.ToString(results[0].LocationMapWarehouseSlotMask);
                            locMapInfo.SlotCoord1Mask = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate1Mask);
                            locMapInfo.SlotCoord2Mask = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate2Mask);
                            locMapInfo.SlotCoord3Mask = Convert.ToString(results[0].LocationMapWarehouseSlotCoordinate3Mask);
                        }

                        break;
                    }

                case SysCommon.LocMapSubTypes.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return locMapInfo;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetCurrentReadableLocation()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.CurrLocMap.WhsCodeMask.Length, '0') + "M";
            }

            if (locMap.CurrLocMap.SectLocMapRefno != 0 && locMap.CurrLocMap.SectName != null)
            {
                location = location + "." + locMap.CurrLocMap.SectName.TrimEnd();
            }

            if (locMap.CurrLocMap.SlotLocMapRefno != 0 && locMap.CurrLocMap.SlotName != null)
            {
                location = location + "." + locMap.CurrLocMap.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetChosenReadableLocation()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.ChosenLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.ChosenLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.ChosenLocMap.WhsCodeMask.Length, '0') + "M";
            }

            if (locMap.ChosenLocMap.SectLocMapRefno != 0 && locMap.ChosenLocMap.SectName != null)
            {
                location = location + "." + locMap.ChosenLocMap.SectName.TrimEnd();
            }

            if (locMap.ChosenLocMap.SlotLocMapRefno != 0 && locMap.ChosenLocMap.SlotName != null)
            {
                location = location + "." + locMap.ChosenLocMap.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001145'.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetCurrentLocation()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location += locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.CurrLocMap.WhsCodeMask.Length, '0');
            }

            if (locMap.CurrLocMap.IsSectionTracked && locMap.CurrLocMap.SectLocMapRefno != 0)
            {
                location += locMap.CurrLocMap.SectCode.ToString().TrimEnd().PadLeft(
                    locMap.CurrLocMap.SectMask.Length, '0');
            }

            if (locMap.CurrLocMap.IsSlotTracked && locMap.CurrLocMap.SlotLocMapRefno != 0)
            {
                location += locMap.CurrLocMap.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001145'.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetChosenLocation()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.ChosenLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location += locMap.ChosenLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.ChosenLocMap.WhsCodeMask.Length,
                    Convert.ToChar("0"));
            }

            if (locMap.ChosenLocMap.IsSectionTracked && locMap.ChosenLocMap.SectLocMapRefno != 0)
            {
                location += locMap.ChosenLocMap.SectCode.ToString().TrimEnd().PadLeft(
                    locMap.ChosenLocMap.SectMask.Length, Convert.ToChar("0"));
            }

            if (locMap.ChosenLocMap.IsSlotTracked && locMap.ChosenLocMap.SlotLocMapRefno != 0)
            {
                location += locMap.ChosenLocMap.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'.
        /// </summary>
        /// <param name="isSeparateSlotCoords"><see cref="bool"/> Separate slot coordinates.</param>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetCurrentReadableLocation(bool isSeparateSlotCoords)
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.CurrLocMap.WhsCodeMask.Length,
                    Convert.ToChar("0")) + "M";
            }

            if (locMap.CurrLocMap.SectLocMapRefno != 0 && locMap.CurrLocMap.SectName != null)
            {
                location = location + "." + locMap.CurrLocMap.SectName.TrimEnd();
            }

            if (locMap.CurrLocMap.SlotLocMapRefno != 0 && locMap.CurrLocMap.SlotName != null)
            {
                if (isSeparateSlotCoords)
                {
                    if (locMap.CurrLocMap.IsSlotCoord1Tracked)
                    {
                        if (locMap.CurrLocMap.SlotName.Length > 0)
                        {
                            location = location + "." +
                                       locMap.CurrLocMap.SlotName[..locMap.CurrLocMap.SlotCoord1Mask.Length];
                        }
                    }

                    if (locMap.CurrLocMap.IsSlotCoord2Tracked)
                    {
                        location = location + "." + locMap.CurrLocMap.SlotName.Substring(
                            locMap.CurrLocMap.SlotCoord1Mask.Length,
                            locMap.CurrLocMap.SlotCoord2Mask.Length);
                    }

                    if (locMap.CurrLocMap.IsSlotCoord3Tracked)
                    {
                        location = location + "." + locMap.CurrLocMap.SlotName[
                            (locMap.CurrLocMap.SlotCoord1Mask.Length +
                            locMap.CurrLocMap.SlotCoord2Mask.Length)..];
                    }
                }
                else
                {
                    location = location + "." + locMap.CurrLocMap.SlotName.TrimEnd();
                }
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'.
        /// </summary>
        /// <param name="isSeparateSlotCoords"><see cref="bool"/> Separate slot coordinates.</param>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetChosenReadableLocation(bool isSeparateSlotCoords)
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.ChosenLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.ChosenLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.ChosenLocMap.WhsCodeMask.Length,
                    Convert.ToChar("0")) + "M";
            }

            if (locMap.ChosenLocMap.SectLocMapRefno != 0 && locMap.ChosenLocMap.SectName != null)
            {
                location = location + "." + locMap.ChosenLocMap.SectName.TrimEnd();
            }

            if (locMap.ChosenLocMap.SlotLocMapRefno != 0 && locMap.ChosenLocMap.SlotName != null)
            {
                if (isSeparateSlotCoords)
                {
                    if (locMap.ChosenLocMap.IsSlotCoord1Tracked)
                    {
                        location = location + "." + locMap.ChosenLocMap.SlotName.Substring(
                            0,
                            locMap.ChosenLocMap.SlotCoord1Mask.Length);
                    }

                    if (locMap.ChosenLocMap.IsSlotCoord2Tracked)
                    {
                        location = location + "." + locMap.ChosenLocMap.SlotName.Substring(
                            locMap.ChosenLocMap.SlotCoord1Mask.Length,
                            locMap.ChosenLocMap.SlotCoord2Mask.Length);
                    }

                    if (locMap.ChosenLocMap.IsSlotCoord3Tracked)
                    {
                        location = location + "." + locMap.ChosenLocMap.SlotName[
                            (locMap.ChosenLocMap.SlotCoord1Mask.Length +
                            locMap.ChosenLocMap.SlotCoord2Mask.Length)..];
                    }
                }
                else
                {
                    location = location + "." + locMap.ChosenLocMap.SlotName.TrimEnd();
                }
            }

            return location;
        }

        /// <summary>
        /// Returns a formatted location string e.g. '00112345'.
        /// </summary>
        /// <param name="locMapInfo"><see cref="SLocMap"/> Location map.</param>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetLocation(SLocMap locMapInfo)
        {
            var location = string.Empty;

            if (locMapInfo.WhsLocMapRefno != 0)
            {
                location += locMapInfo.WhsCode.ToString().TrimEnd().PadLeft(
                    locMapInfo.WhsCodeMask.Length,
                    Convert.ToChar("0"));
            }

            if (locMapInfo.IsSectionTracked && locMapInfo.SectLocMapRefno != 0)
            {
                location = location + "." + locMapInfo.SectCode.ToString().TrimEnd().PadLeft(
                    locMapInfo.SectMask.Length, Convert.ToChar("0"));
            }

            if (locMapInfo.IsSlotTracked && locMapInfo.SlotLocMapRefno != 0)
            {
                location = location + "." + locMapInfo.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'.
        /// </summary>
        /// <param name="locMapInfo"><see cref="SLocMap"/> Location map.</param>
        /// <param name="isSepSlotCoords"><see cref="bool"/> Separate slot coordinates.</param>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetReadableLocation(SLocMap locMapInfo, bool isSepSlotCoords = false)
        {
            var location = string.Empty;

            if (locMapInfo.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMapInfo.WhsCode.ToString().TrimEnd().PadLeft(
                    locMapInfo.WhsCodeMask.Length,
                    Convert.ToChar("0")) + "M";
            }

            if (locMapInfo.SectLocMapRefno != 0 && locMapInfo.SectName != null)
            {
                location = location + "." + locMapInfo.SectName.TrimEnd();
            }

            if (locMapInfo.SlotLocMapRefno != 0 && locMapInfo.SlotName != null)
            {
                if (isSepSlotCoords)
                {
                    if (locMapInfo.IsSlotCoord1Tracked)
                    {
                        location = location + "." + locMapInfo.SlotName.Substring(
                            0,
                            locMapInfo.SlotCoord1Mask.Length);
                    }

                    if (locMapInfo.IsSlotCoord2Tracked)
                    {
                        location = location + "." + locMapInfo.SlotName.Substring(
                            locMapInfo.SlotCoord1Mask.Length,
                            locMapInfo.SlotCoord2Mask.Length);
                    }

                    if (locMapInfo.IsSlotCoord3Tracked)
                    {
                        location = location + "." + locMapInfo.SlotName[
                            (locMapInfo.SlotCoord1Mask.Length + locMapInfo.SlotCoord2Mask.Length)..];
                    }
                }
                else
                {
                    location = location + "." + locMapInfo.SlotName.TrimEnd();
                }
            }

            return location;
        }

        /// <summary>
        /// Get a formatted location string for uniqueness checking.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetCurrentLocationForUniquenessChecking()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsFKRefno != 0)
            {
                location += locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.CurrLocMap.WhsCodeMask.Length, Convert.ToChar("0"));
            }

            return location;
        }

        /// <summary>
        /// Get a formatted location string for uniqueness checking.
        /// </summary>
        /// <returns><see cref="string"/> Location string.</returns>
        public static string GetChosenLocationForUniquenessChecking()
        {
            var location = string.Empty;
            var locMap = LocationMap.GetInstance();

            if (locMap.ChosenLocMap.WhsFKRefno != 0)
            {
                location += locMap.ChosenLocMap.WhsCode.ToString().TrimEnd().PadLeft(
                    locMap.ChosenLocMap.WhsCodeMask.Length, Convert.ToChar("0"));
            }

            return location;
        }

        /// <summary>
        /// Calculate the mod 10 Lund check digit.
        /// </summary>
        /// <param name="idWithoutCheckDigit"><see cref="string"/> Data to calculate the check digit for.</param>
        /// <returns><see cref="int"/> The calculated check digit number.</returns>
        public static int Mod10(string idWithoutCheckDigit)
        {
            idWithoutCheckDigit = idWithoutCheckDigit ?? throw new ArgumentNullException(nameof(idWithoutCheckDigit));
            const int numberBase = 10;
            const int validCodePointOffset = 48;    // '0'

            var index = idWithoutCheckDigit.Length;
            var parity = true;
            var sum = 0;

            while (index-- > 0)
            {
                var digit = idWithoutCheckDigit[index] - validCodePointOffset;
                if (parity)
                {
                    if ((digit <<= 1) > (numberBase - 1))
                    {
                        digit -= numberBase - 1;
                    }
                }

                parity = !parity;
                sum += digit;
            }

            return (numberBase - (sum %= numberBase)) % numberBase;
        }

        /// <summary>
        /// Get default location map reference number from a product reference number.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product refno, Argument may be zero, result
        /// is defaulting to the warehouse.</param>
        /// <param name="prodLocMapType"><see cref="SysCommon.ProdLocMapTypes"/> Product location map type.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public static async Task<int> GetDefaultLocMapRefnoFromWhsProdRefno(
            int whsLocMapRefno,
            int prodRefno,
            SysCommon.ProdLocMapTypes prodLocMapType)
        {
            var defaultWhsLocMapRefno = whsLocMapRefno;
            var defaultSectLocMapRefno = 0;
            var defaultSlotLocMapRefno = 0;
            var locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.IsSlotTracked && defaultWhsLocMapRefno != 0)
            {
                // Check for default slot for the warehouse and product.
                if (prodRefno > 0)
                {
                    var query = "SELECT tProductLocationMapType.LocationMapRefno "
                                + "FROM tProductLocationMapType "
                                + "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno "
                                + "= tProductLocationMapType.LocationMapRefno "
                                + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                                + "= tLocationMapSlot.SectionLocationMapRefno "
                                + "WHERE tProductLocationMapType.ProductRefno = ? "
                                + "AND tProductLocationMapType.ProductLocationMapTypeEnum = ? "
                                + "AND tLocationMapSection.WarehouseLocationMapRefno = ?";

                    var results = await DatabaseConnection.GetInstance().
                        SQLConn.QueryAsync<tProductLocationMapTypeModel>(
                        query,
                        Convert.ToInt32(prodRefno),
                        Convert.ToInt32(prodLocMapType),
                        Convert.ToInt32(defaultWhsLocMapRefno));

                    if (results?.Count > 0)
                    {
                        defaultSlotLocMapRefno = results[0].LocationMapRefno;
                    }
                }

                // Check for default slot for the warehouse if not found for product.
                if (defaultSlotLocMapRefno == 0)
                {
                    const string query = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                                         + "FROM tWarehouseLocationMapType "
                                         + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = ?"
                                         + " AND tWarehouseLocationMapType.WarehouseLocationMapRefno = ?";

                    var results = await DatabaseConnection.GetInstance().
                        SQLConn.QueryAsync<tWarehouseLocationMapTypeModel>(
                        query,
                        Convert.ToInt32(prodLocMapType),
                        Convert.ToInt32(defaultWhsLocMapRefno));

                    if (results?.Count > 0)
                    {
                        defaultSlotLocMapRefno = results[0].LocationMapRefno;
                    }
                }
            }

            if (defaultSlotLocMapRefno == 0 && locMap.CurrLocMap.IsSectionTracked && defaultWhsLocMapRefno != 0)
            {
                // Check for default section for the warehouse and product.
                if (prodRefno > 0)
                {
                    const string query = "SELECT tProductLocationMapType.LocationMapRefno "
                                         + "FROM tProductLocationMapType "
                                         + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                                         + "= tProductLocationMapType.LocationMapRefno "
                                         + "WHERE tProductLocationMapType.ProductRefno = ? "
                                         + "AND tProductLocationMapType.ProductLocationMapTypeEnum = ? "
                                         + "AND tLocationMapSection.WarehouseLocationMapRefno = ?";

                    var results = await DatabaseConnection.GetInstance().
                        SQLConn.QueryAsync<tProductLocationMapTypeModel>(
                        query,
                        Convert.ToInt32(prodRefno),
                        Convert.ToInt32(prodLocMapType),
                        Convert.ToInt32(defaultWhsLocMapRefno));

                    if (results?.Count > 0)
                    {
                        defaultSectLocMapRefno = results[0].LocationMapRefno;
                    }
                }

                // Check for default section for the warehouse if not found for product.
                if (defaultSectLocMapRefno == 0)
                {
                    const string query = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                                         + "FROM tWarehouseLocationMapType "
                                         + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = ? "
                                         + "AND tWarehouseLocationMapType.WarehouseLocationMapRefno = ?";

                    var results = await DatabaseConnection.GetInstance().
                        SQLConn.QueryAsync<tWarehouseLocationMapTypeModel>(
                        query,
                        Convert.ToInt32(prodLocMapType),
                        Convert.ToInt32(defaultWhsLocMapRefno));

                    if (results?.Count > 0)
                    {
                        defaultSectLocMapRefno = results[0].LocationMapRefno;
                    }
                }
            }

            if (defaultWhsLocMapRefno == 0)
            {
                defaultWhsLocMapRefno = locMap.LoginWhsLocMapRefno;
            }

            int locMapRefno;
            if (defaultSlotLocMapRefno != 0)
            {
                locMapRefno = defaultSlotLocMapRefno;
            }
            else if (defaultSectLocMapRefno != 0)
            {
                locMapRefno = defaultSectLocMapRefno;
            }
            else
            {
                locMapRefno = defaultWhsLocMapRefno;
            }

            return locMapRefno;
        }

        /// <summary>
        /// Get the warehouse location map reference number from a location map reference number.
        /// Checks the subtype in tLocationMap and then looks it up.
        /// </summary>
        /// <param name="locMapRefno"><see cref="int"/> Location map reference number.</param>
        /// <returns><see cref="int"/> Warehouse location map reference number.</returns>
        public static async Task<int> GetWhsLocMapRefnoFromLocMapRefno(int locMapRefno)
        {
            var locationSubType = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                ExecuteScalarAsync<int>(
                "SELECT LocationMapSubType FROM tLocationMap WHERE LocationMapRefno = ?",
                locMapRefno));

            return (SysCommon.LocMapSubTypes)locationSubType switch
            {
                SysCommon.LocMapSubTypes.Section => Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                    ExecuteScalarAsync<int>(
                    "SELECT WarehouseLocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = ?",
                    locMapRefno)), // Access Section table for WarehouseLocationMapRefno

                SysCommon.LocMapSubTypes.Slot => Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.
                    ExecuteScalarAsync<int>(
                    "SELECT WarehouseLocationMapRefno FROM tLocationMapSlot WHERE LocationMapRefno = ?",
                    locMapRefno)), // Access Slot table for WarehouseLocationMapRefno

                SysCommon.LocMapSubTypes.Warehouse => locationSubType, // It already is a WarehouseLocationMapRefno

                _ => 0,
            };
        }

        /// <summary>
        /// Get the location map reference number from the current location map.
        /// </summary>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public static int GetLocMapRefnoFromSLocMap()
        {
            return GetLocMapRefnoFromSLocMap(LocationMap.GetInstance().CurrLocMap);
        }

        /// <summary>
        /// Get the location map reference number from the location map.
        /// </summary>
        /// <param name="locMapInfo"><see cref="SLocMap"/> Location map.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public static int GetLocMapRefnoFromSLocMap(SLocMap locMapInfo)
        {
            var locMapRefno = 0;

            if (locMapInfo.SlotLocMapRefno > 0)
            {
                locMapRefno = locMapInfo.SlotLocMapRefno;
            }
            else if (locMapInfo.SectLocMapRefno > 0)
            {
                locMapRefno = locMapInfo.SectLocMapRefno;
            }
            else if (locMapInfo.WhsLocMapRefno > 0)
            {
                locMapRefno = locMapInfo.WhsLocMapRefno;
            }

            return locMapRefno;
        }
    }
}
