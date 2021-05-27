namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
    using dMobile.Resources.QueryResults;
    using dMobile.Resources.Types;
    using SQLite;

    /// <summary>
    /// Defines the <see cref="LocationMapTasks" />
    /// </summary>
    public static class LocationMapTasks
    {
        /// <summary>
        /// GET THE LOCATION MAP REFNO FROM A GLOBAL LOCATION IDENTIFIER
        /// </summary>
        /// <param name="globalLocationIdentifier">The globalLocationIdentifier<see cref="string"/></param>
        /// <returns>Found LocationMapRefno</returns>
        public static async Task<int> GetLocMapRefnoFromBarCodeGlobalLocIdentifier(string globalLocationIdentifier)
        {
            int locMapRefno = 0;
            SysBarCodeSettings barCodeSettings = new SysBarCodeSettings().LoadData();
            bool secondAttempt = barCodeSettings.IsRuleEnabled_HeritageGlobalLocationIdentifierSecondPassIgnoreCheckDigit;

            // Because we each warehouse has it's own map structure now we can't assume the tracking map.
            // So this will be a little slower, plus the extra parsing rules to include.

            // first attempt to find the location as a complete SLOT bar code
            List<tLocationMapSlotModel> locationMapRefnoResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSlotModel>("SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapRefno = ?", globalLocationIdentifier);

            if (locationMapRefnoResults != null && locationMapRefnoResults.Count > 0)
            {
                locMapRefno = Convert.ToInt32(locationMapRefnoResults[0].LocationMapRefno);
            }
            else
            {
                if (!secondAttempt)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("Unable To Locate Global Location Identifier '{0}' In {1}", globalLocationIdentifier, "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapRefno = ? "));
                }
            }

            if (secondAttempt)
            {
                string barCode = globalLocationIdentifier.Substring(0, globalLocationIdentifier.TrimEnd().Length - 1);
                locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSlotModel>("SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotBarCode LIKE ?%", barCode));
            }


            // second attempt to find the location as a complete SECTION bar code
            if (locMapRefno == 0)
            {
                secondAttempt = false;

                List<tLocationMapSectionModel> tLocationMapSectionModelResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSectionModel>("SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = ?", globalLocationIdentifier);

                if (tLocationMapSectionModelResults != null && tLocationMapSectionModelResults.Count > 0)
                {
                    locMapRefno = Convert.ToInt32(tLocationMapSectionModelResults[0].LocationMapRefno);
                }
                else
                {
                    if (!secondAttempt)
                    {
                        // TODO: Resource strings
                        throw new Exception(string.Format("Unable To Locate Global Location Identifier '{0}' In {1}", globalLocationIdentifier, "SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = ?"));
                    }
                }

                if (secondAttempt)
                {
                    string barCode = globalLocationIdentifier.Substring(0, globalLocationIdentifier.TrimEnd().Length - 1);
                    locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSectionModel>("SELECT tLocationMapSection.LocationMapRefno FROM tLocationMapSection WHERE tLocationMapSection.LocationMapSectionBarCode LIKE ?%", barCode));
                }
            }

            // third attempt to find the location as a complete WAREHOUSE bar code
            if (locMapRefno == 0)
            {
                secondAttempt = false;

                List<tLocationMapWarehouseModel> tLocationMapWarehouseModelResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", globalLocationIdentifier);

                if (tLocationMapWarehouseModelResults != null && tLocationMapWarehouseModelResults.Count > 0)
                {
                    locMapRefno = Convert.ToInt32(tLocationMapWarehouseModelResults[0].LocationMapRefno);
                }
                else
                {
                    if (!secondAttempt)
                    {
                        // TODO: Resource strings
                        throw new Exception(string.Format("Unable To Locate Global Location Identifier '{0}' In {1}", globalLocationIdentifier, "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapRefno = ?"));
                    }
                }

                if (secondAttempt)
                {
                    string barCode = globalLocationIdentifier.Substring(0, globalLocationIdentifier.TrimEnd().Length - 1);
                    locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapWarehouseModel>("SELECT tLocationMapWarehouse.LocationMapRefno FROM tLocationMapWarehouse WHERE tLocationMapWarehouse.LocationMapWarehouseBarCode LIKE ?%", barCode));
                }

            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetWhsList
        /// </summary>
        /// <param name="dataTableWhsList">The dataTableWhsList<see cref="DataTable"/></param>
        public static async Task<List<Warehouse>> GetWhsList()
        {
            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Warehouse>("SELECT LocationMapWarehouseCode, LocationMapWarehouseName, LocationMapRefno FROM tLocationMapWarehouse ORDER BY LocationMapWarehouseCode");
        }

        /// <summary>
        /// The GetSectList
        /// </summary>
        /// <param name="dataTableSectionList">The dataTableSectionList<see cref="DataTable"/></param>
        public static async Task<List<Section>> GetSectList()
        {
            string sql = "SELECT LocationMapSectionCode,"
                + "LocationMapSectionName,"
                + "LocationMapRefno,"
                + "WarehouseLocationMapRefno "
                + "FROM tLocationMapSection "
                + "ORDER BY LocationMapSectionCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Section>(sql);
        }

        /// <summary>
        /// Gets the Section name from a LocationMapSectionFKRefno.
        /// </summary>
        /// <param name="locMapSectFKRefno">LocationMapSectionFKRefno</param>
        /// <returns>Name of the section.</returns>
        public static async Task<string> GetSectionName(int locMapSectFKRefno)
        {
            return Convert.ToString(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSectionModel>("SELECT LocationMapSectionName FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToInt32(locMapSectFKRefno)));
        }

        /// <summary>
        /// Gets the Slot name from a LocationMapSlotFKRefno.
        /// </summary>
        /// <param name="locMapSlotFKRefno">The locMapSlotFKRefno<see cref="int"/></param>
        /// <returns>Name of the slot.</returns>
        public static async Task<string> GetSlotName(int locMapSlotFKRefno)
        {
            return Convert.ToString(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSlotModel>("SELECT LocationMapSlotName FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", Convert.ToInt32(locMapSlotFKRefno)));
        }

        /// <summary>
        /// The GetSlotCoord1List
        /// </summary>
        /// <param name="dataTableSlotCoord1List">The dataTableSlotCoord1List<see cref="DataTable"/></param>
        //public static async Task<List<Slot>> GetSlotCoord1List(ref DataTable dataTableSlotCoord1List)
        public static async Task<List<Slot>> GetSlotCoord1List()
        {
            string sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                + "tLocationMapSlot.SectionLocationMapRefno "
                + "FROM tLocationMapSlot "
                + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                + "= tLocationMapSlot.SlotCoord1LocationMapRefno "
                + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
                + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(sql, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1));
        }

        /// <summary>
        /// The GetSlotCoord2List
        /// </summary>
        /// <param name="dataTableSlotCoord2List">The dataTableSlotCoord2List<see cref="DataTable"/></param>
        public static async Task<List<Slot>> GetSlotCoord2List()
        {
            string sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
            + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
            + "tLocationMapSlot.SectionLocationMapRefno "
            + "FROM tLocationMapSlot "
            + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
            + "= tLocationMapSlot.SlotCoord2LocationMapRefno "
            + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
            + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(sql, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2));

        }

        /// <summary>
        /// The GetSlotCoord3List
        /// </summary>
        /// <param name="dataTableSlotCoord3List">The dataTableSlotCoord3List<see cref="DataTable"/></param>
        public static async Task<List<Slot>> GetSlotCoord3List()
        {
            string sql = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                + "tLocationMapSlot.SectionLocationMapRefno "
                + "FROM tLocationMapSlot "
                + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                + "= tLocationMapSlot.SlotCoord3LocationMapRefno "
                + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = ?"
                + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";

            return await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Slot>(sql, Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3));
        }

        /// <summary>
        /// The GetSlotRefnoFromCoordinates
        /// </summary>
        /// <param name="sectionLocMapRefno"></param>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <param name="coordinate3"></param>
        /// <returns></returns>
        public static async Task<int> GetSlotRefnoFromCoordinates(int sectionLocMapRefno, int slotCoord1Refno, int slotCoord2Refno, int slotCoord3Refno)
        {
            int slotLocMapRefno = 0;
            LocationMap locMap = LocationMap.GetInstance();
            string query = string.Empty;
            List<object> parameters = new List<object>();


            if (locMap.CurrLocMap.IsSlotCoord1Tracked & locMap.CurrLocMap.IsSlotCoord2Tracked & locMap.CurrLocMap.IsSlotCoord3Tracked)
            {
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
            }
            else if (locMap.CurrLocMap.IsSlotCoord1Tracked & locMap.CurrLocMap.IsSlotCoord2Tracked)
            {
                query = "SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE tLocationMapSlot.SectionLocationMapRefno = ?"
                    + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = ?"
                    + " AND tLocationMapSlot.SlotCoord2LocationMapRefno = ?";
                parameters.Add(Convert.ToInt32(sectionLocMapRefno));
                parameters.Add(Convert.ToInt32(slotCoord1Refno));
                parameters.Add(Convert.ToInt32(slotCoord2Refno));
            }
            else if (locMap.CurrLocMap.IsSlotCoord1Tracked)
            {
                query = "SELECT tLocationMapSlot.LocationMapRefno "
                    + "FROM tLocationMapSlot "
                    + "WHERE tLocationMapSlot.SectionLocationMapRefno = ?"
                    + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = ?";
                parameters.Add(Convert.ToInt32(sectionLocMapRefno));
                parameters.Add(Convert.ToInt32(slotCoord1Refno));
            }
            else
            {
                // TODO: Resource strings
                throw new Exception("Location map tracking map settings are not set.");
            }

            List<tLocationMapSlotModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSlotModel>(query, parameters.ToArray());

            if (results != null && results.Count > 0)
            {
                slotLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
            }

            return slotLocMapRefno;
        }

        /// <summary>
        /// The IsCompleteLocMapRefno
        /// </summary>
        /// <param name="locMap"></param>
        /// <returns></returns>
        public static async Task<bool> IsCompleteLocMapRefno(SLocMap locMap)
        {
            bool isComplete = false;

            if (LocationMap.GetInstance().CurrLocMap.IsSlotTracked)
            {
                isComplete = await IsCompleteLocMapRefno(locMap.SlotLocMapRefno);
            }
            else if (LocationMap.GetInstance().CurrLocMap.IsSectionTracked)
            {
                isComplete = await IsCompleteLocMapRefno(locMap.SectLocMapRefno);
            }
            else
            {
                isComplete = await IsCompleteLocMapRefno(locMap.WhsLocMapRefno);
            }

            return isComplete;
        }

        /// <summary>
        /// The IsCompleteLocMapRefno
        /// </summary>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        public static async Task<bool> IsCompleteLocMapRefno(int locMapRefno)
        {
            bool isComplete = false;
            SysCommon.LocMapSubTypes locMapSubType = SysCommon.LocMapSubTypes.None;
            LocationMap locMap = LocationMap.GetInstance();

            //cmndLocMap.CommandType = CommandType.Text;
            //cmndLocMap.CommandText = "SELECT * FROM tLocationMap WHERE LocationMapRefno = @locMapRefno";
            //cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

            List<tLocationMapModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapModel>("SELECT * FROM tLocationMap WHERE LocationMapRefno = ?", Convert.ToInt32(locMapRefno));

            if (results != null && results.Count > 0)
            {
                //locMapSubType = (SysCommon.LocMapSubTypes)drLocMap.GetInt32(Convert.ToInt32(SysCommon.LocMapFields.SubType));
                locMapSubType = (SysCommon.LocMapSubTypes)results[0].LocationMapSubType;
            }

            if (locMap.CurrLocMap.IsSlotTracked)
            {
                if (locMapSubType == SysCommon.LocMapSubTypes.Slot)
                {
                    isComplete = true;
                }
            }
            else if (locMap.CurrLocMap.IsSectionTracked)
            {
                if (locMapSubType == SysCommon.LocMapSubTypes.Section)
                {
                    isComplete = true;
                }
            }
            else
            {
                if (locMapSubType == SysCommon.LocMapSubTypes.Warehouse)
                {
                    isComplete = true;
                }
            }

            return isComplete;
        }

        ///// <summary>
        ///// The GetWhsSectSlotLocMapRefnosFromLocMapRefno
        ///// </summary>
        ///// <param name="locMapRefno"></param>
        ///// <param name="warehouseLocMapRefno"></param>
        ///// <param name="sectionLocMapRefno">The sectionLocMapRefno<see cref="int"/></param>
        ///// <param name="slotLocMapRefno"></param>
        //public static void GetWhsSectSlotLocMapRefnosFromLocMapRefno(int locMapRefno, ref int warehouseLocMapRefno, ref int sectionLocMapRefno, ref int slotLocMapRefno)
        //{
        //    warehouseLocMapRefno = 0;
        //    sectionLocMapRefno = 0;
        //    slotLocMapRefno = 0;

        //    using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //    {
        //        cmndLocMap.CommandType = CommandType.Text;
        //        cmndLocMap.CommandText = "SELECT tWarehouseLocationMap.LocationMapRefno AS WarehouseLocationMapRefno,"
        //            + "CAST(0, Integer) AS SectionLocationMapRefno,"
        //            + "CAST(0, Integer) AS SlotLocationMapRefno "
        //            + "FROM tWarehouseLocationMap "
        //            + "WHERE tWarehouseLocationMap.LocationMapRefno = @warehouseLocMapRefno"
        //            + " UNION "
        //            + "SELECT tSectionLocationMap.WarehouseLocationMapRefno,"
        //            + "tSectionLocationMap.LocationMapRefno AS SectionLocationMapRefno,"
        //            + "CAST(0, Integer) AS SlotLocationMapRefno "
        //            + "FROM tSectionLocationMap "
        //            + "WHERE tSectionLocationMap.LocationMapRefno = @sectionLocMapRefno"
        //            + " UNION "
        //            + "SELECT tWarehouseLocationMap.LocationMapRefno AS WarehouseLocationMapRefno,"
        //            + "tSlotLocationMap.SectionLocationMapRefno,"
        //            + "tSlotLocationMap.LocationMapRefno AS SlotLocationMapRefno "
        //            + "FROM tSlotLocationMap "
        //            + "INNER JOIN tSectionLocationMap ON tSectionLocationMap.LocationMapRefno "
        //            + "= tSlotLocationMap.SectionLocationMapRefno "
        //            + "INNER JOIN tWarehouseLocationMap ON tWarehouseLocationMap.LocationMapRefno "
        //            + "= tSectionLocationMap.WarehouseLocationMapRefno "
        //            + "WHERE tSlotLocationMap.LocationMapRefno = @slotLocMapRefno";
        //        cmndLocMap.Parameters.Add(new SQLiteParameter("@warehouseLocMapRefno", Convert.ToInt32(warehouseLocMapRefno)));
        //        cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapRefno", Convert.ToInt32(sectionLocMapRefno)));
        //        cmndLocMap.Parameters.Add(new SQLiteParameter("@slotLocMapRefno", Convert.ToInt32(slotLocMapRefno)));

        //        using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
        //        {
        //            if (drLocMap.Read())
        //            {
        //                warehouseLocMapRefno = Convert.ToInt32(drLocMap["WarehouseLocationMapRefno"]);
        //                sectionLocMapRefno = Convert.ToInt32(drLocMap["SectionLocationMapRefno"]);
        //                slotLocMapRefno = Convert.ToInt32(drLocMap["SlotLocationMapRefno"]);
        //            }

        //            drLocMap.Close();
        //        }
        //    }
        //}

        public class SLocMapAndRefno
        {
            private SLocMap sLocMap;
            private int locMapRefno;

            public SLocMap SLocMap { get => this.sLocMap; set => this.sLocMap = value; }
            public int LocMapRefno { get => this.locMapRefno; set => this.locMapRefno = value; }
        }

        /// <summary>
        /// The GetSLocMapFromLocMapFKRefno
        /// </summary>
        /// <param name="warehouseLocMapFKRefno"></param>
        /// <param name="sectionLocMapFKRefno"></param>
        /// <param name="slotLocnMapFKRefno"></param>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        /// TODO: Check if locMapRefno is used anywhere
        //public static async Task<SLocMap> GetSLocMapFromLocMapFKRefno(int warehouseLocMapFKRefno, int sectionLocMapFKRefno, int slotLocnMapFKRefno, ref int locMapRefno)
        public static async Task<SLocMapAndRefno> GetSLocMapFromLocMapFKRefno(int warehouseLocMapFKRefno, int sectionLocMapFKRefno, int slotLocnMapFKRefno, int locMapRefno)
        {
            SLocMap retLocMap = new SLocMap();
            int tempLocMapRefno = 0;

            if (tempLocMapRefno == 0 & slotLocnMapFKRefno != 0)
            {
                //using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                //{
                //    cmndLocMap.CommandType = CommandType.Text;
                //    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @slotLocnMapFKRefno";
                //    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotLocnMapFKRefno", Convert.ToInt32(slotLocnMapFKRefno)));

                //    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                //    {
                //        if (drLocMap.Read())
                //        {
                //            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                //        }
                //    }
                //}

                List<tLocationMapModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapModel>("SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = ?", Convert.ToInt32(slotLocnMapFKRefno));

                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            if (tempLocMapRefno == 0 & sectionLocMapFKRefno != 0)
            {
                //using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                //{
                //    cmndLocMap.CommandType = CommandType.Text;
                //    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @sectionLocMapFKRefno";
                //    cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapFKRefno", Convert.ToInt32(sectionLocMapFKRefno)));

                //    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                //    {
                //        if (drLocMap.Read())
                //        {
                //            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                //        }
                //    }
                //}

                List<tLocationMapSectionModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSectionModel>("SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = ?", Convert.ToInt32(sectionLocMapFKRefno));

                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            if (tempLocMapRefno == 0 & warehouseLocMapFKRefno != 0)
            {
                //using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                //{
                //    cmndLocMap.CommandType = CommandType.Text;
                //    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @warehouseLocMapFKRefno";
                //    cmndLocMap.Parameters.Add(new SQLiteParameter("@warehouseLocMapFKRefno", Convert.ToInt32(warehouseLocMapFKRefno)));

                //    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                //    {
                //        if (drLocMap.Read())
                //        {
                //            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                //        }
                //    }
                //}

                List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = ?", Convert.ToInt32(warehouseLocMapFKRefno));

                if (results != null && results.Count > 0)
                {
                    tempLocMapRefno = Convert.ToInt32(results[0].LocationMapRefno);
                }
            }

            retLocMap = await GetSLocMapFromLocMapRefno(tempLocMapRefno);

            locMapRefno = tempLocMapRefno;

            return new SLocMapAndRefno { SLocMap = retLocMap, LocMapRefno = locMapRefno };
        }

        /// <summary>
        /// The GetSLocMapFromLocMapRefno
        /// </summary>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        public static async Task<SLocMap> GetSLocMapFromLocMapRefno(int locMapRefno)
        {
            SLocMap locMapInfo = new SLocMap();
            SysCommon.LocMapSubTypes locMapSubType = default(SysCommon.LocMapSubTypes);

            List<tLocationMapModel> tLocationMapResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapModel>("SELECT * FROM tLocationMap WHERE LocationMapRefno = ?", Convert.ToInt32(locMapRefno));

            if (tLocationMapResults != null && tLocationMapResults.Count > 0)
            {
                locMapSubType = (SysCommon.LocMapSubTypes)tLocationMapResults[0].LocationMapSubType;
            }

            if (locMapSubType == SysCommon.LocMapSubTypes.Slot)
            {
                string query = "SELECT tLocationMapSlot.LocationMapRefno, "
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
                    + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tLocationMapSlot.SectionLocationMapRefno "
                    + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tLocationMapSection.WarehouseLocationMapRefno "
                    + "WHERE tLocationMapSlot.LocationMapRefno = ?";

                List<tLocationMapSlotQueryResult> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSlotQueryResult>(query, Convert.ToInt32(locMapRefno));

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
            }
            else if (locMapSubType == SysCommon.LocMapSubTypes.Section)
            {
                string query = "SELECT tLocationMapSection.LocationMapRefno, "
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

                List<tLocationMapSectionQueryResult> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSectionQueryResult>(query, Convert.ToInt32(locMapRefno));

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
            }
            else if (locMapSubType == SysCommon.LocMapSubTypes.Warehouse)
            {
                string query = "SELECT tLocationMapWarehouse.LocationMapRefno, "
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

                List<tLocationMapWarehouseModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>(query, Convert.ToInt32(locMapRefno));

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
            }

            return locMapInfo;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'
        /// </summary>
        /// <returns>Location string</returns>
        public static string GetCurrentReadableLocation()
        {
            string location = string.Empty;
            LocationMap locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(3, Convert.ToChar("0")) + "M";
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
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'
        /// </summary>
        /// <param name="isSeparateSlotCoords"></param>
        /// <returns>Location string</returns>
        public static string GetCurrentReadableLocation(bool isSeparateSlotCoords)
        {
            string location = string.Empty;
            LocationMap locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(3, Convert.ToChar("0")) + "M";
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
                        location = location + "." + locMap.CurrLocMap.SlotName.Substring(0, locMap.CurrLocMap.SlotCoord1Mask.Length);
                    }

                    if (locMap.CurrLocMap.IsSlotCoord2Tracked)
                    {
                        location = location + "." + locMap.CurrLocMap.SlotName.Substring(locMap.CurrLocMap.SlotCoord1Mask.Length, locMap.CurrLocMap.SlotCoord2Mask.Length);
                    }

                    if (locMap.CurrLocMap.IsSlotCoord3Tracked)
                    {
                        location = location + "." + locMap.CurrLocMap.SlotName.Substring(locMap.CurrLocMap.SlotCoord1Mask.Length + locMap.CurrLocMap.SlotCoord2Mask.Length);
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
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'
        /// </summary>
        /// <param name="locMapInfo"></param>
        /// <returns>Location string</returns>
        public static string GetCurrentReadableLocation(SLocMap locMapInfo)
        {
            string location = string.Empty;

            if (locMapInfo.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMapInfo.WhsCode.ToString().TrimEnd().PadLeft(
                    3,
                    Convert.ToChar("0")) + "M";
            }

            if (locMapInfo.SectLocMapRefno != 0 && locMapInfo.SectName != null)
            {
                location = location + "." + locMapInfo.SectName.TrimEnd();
            }

            if (locMapInfo.SlotLocMapRefno != 0 && locMapInfo.SlotName != null)
            {
                location = location + "." + locMapInfo.SlotName.TrimEnd();
            }

            return location;
        }

        /// <summary>
        /// Returns a human readable (nice) formatted location string e.g. '001.FREEZER.123.45.6'
        /// </summary>
        /// <param name="locMapInfo"></param>
        /// <param name="isSepSlotCoords">The isSepSlotCoords<see cref="bool"/></param>
        /// <returns>Location string</returns>
        public static string GetCurrentReadableLocation(SLocMap locMapInfo, bool isSepSlotCoords)
        {
            string location = string.Empty;

            if (locMapInfo.WhsLocMapRefno != 0)
            {
                // Warehouse Example 001M
                location = location + locMapInfo.WhsCode.ToString().TrimEnd().PadLeft(3, Convert.ToChar("0")) + "M";
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
                        location = location + "." + locMapInfo.SlotName.Substring(0, locMapInfo.SlotCoord1Mask.Length);
                    }

                    if (locMapInfo.IsSlotCoord2Tracked)
                    {
                        location = location + "." + locMapInfo.SlotName.Substring(locMapInfo.SlotCoord1Mask.Length, locMapInfo.SlotCoord2Mask.Length);
                    }

                    if (locMapInfo.IsSlotCoord3Tracked)
                    {
                        location = location + "." + locMapInfo.SlotName.Substring(locMapInfo.SlotCoord1Mask.Length + locMapInfo.SlotCoord2Mask.Length);
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
        /// RETURNS A FORMATTED LOCATION STRING FOR UNIQUENESS CHECKING
        /// </summary>
        /// <returns>LOCATION STRING</returns>
        public static string GetCurrentLocationForUniquenessChecking()
        {
            string location = string.Empty;
            LocationMap locMap = LocationMap.GetInstance();

            if (locMap.CurrLocMap.WhsFKRefno != 0)
            {
                location += locMap.CurrLocMap.WhsCode.ToString().TrimEnd().PadLeft(locMap.CurrLocMap.WhsCodeMask.Length, Convert.ToChar("0"));
            }

            return location;
        }

        /// <summary>
        /// CALCULATE CHECK SUM MOD^10
        /// </summary>
        /// <param name="globalLocationIdentifier"></param>
        /// <returns></returns>
        public static int CalculateGlobalLocationIdentifierCheckSum(string globalLocationIdentifier)
        {
            int checkSum = 0;
            double odd = 0;
            double even = 0;
            double x = 0;
            int i = 0;

            // Padding left should not be needed but added anyway.
            // 7 digits company prefix padded left with zeros + 5 digits for the serial 
            // identifier padded left with zeros = 12
            if (globalLocationIdentifier.Length < 12)
            {
                globalLocationIdentifier = globalLocationIdentifier.PadLeft(12, Convert.ToChar("0"));
            }

            // Calculate check sum MOD^10
            for (i = 1; i <= globalLocationIdentifier.Length; i += 2)
            {
                odd += (int.Parse(globalLocationIdentifier.Substring(i - 1, 1)) * 3);
            }

            for (i = 2; i <= globalLocationIdentifier.Length; i += 2)
            {
                even += int.Parse(globalLocationIdentifier.Substring(i - 1, 1));
            }

            x = 10 - ((odd + even) / 10) - 0.5;

            checkSum = Convert.ToInt32(x);

            return checkSum;
        }

        /// <summary>
        /// The GetDefaultLocMapRefnoFromProdRefno
        /// </summary>
        /// <param name="whsLocMapRefno"></param>
        /// <param name="prodRefno">Argument may be zero, result is defaulting to the warehouse.</param>
        /// <param name="prodLocMapType"></param>
        /// <returns></returns>
        public static async Task<int> GetDefaultLocMapRefnoFromProdRefno(int whsLocMapRefno, int prodRefno, SysCommon.ProdLocMapTypes prodLocMapType)
        {
            int locMapRefno = 0;
            int defaultWhsLocMapRefno = 0;
            int defaultSectLocMapRefno = 0;
            int defaultSlotLocMapRefno = 0;
            LocationMap locMap = LocationMap.GetInstance();

            defaultWhsLocMapRefno = whsLocMapRefno;

            if (locMap.CurrLocMap.IsSlotTracked && defaultWhsLocMapRefno != 0)
            {
                if (0 < prodRefno)
                {
                    string query = "SELECT tProductLocationMapType.LocationMapRefno "
                        + "FROM tProductLocationMapType "
                        + "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno "
                        + "= tProductLocationMapType.LocationMapRefno "
                        + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                        + "= tLocationMapSlot.SectionLocationMapRefno "
                        + "WHERE tProductLocationMapType.ProductRefno = ? "
                        + "AND tProductLocationMapType.ProductLocationMapTypeEnum = ? "
                        + "AND tLocationMapSection.WarehouseLocationMapRefno = ?";
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodRefno", Convert.ToInt32(prodRefno)));
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                    //defaultSlotLocMapRefno = Convert.ToInt32(cmndProdLocMapType.ExecuteScalar());
                    defaultSlotLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<dynamic>(query, Convert.ToInt32(prodRefno), Convert.ToInt32(prodLocMapType), Convert.ToInt32(defaultWhsLocMapRefno)));
                }

                if (defaultSlotLocMapRefno == 0)
                {
                    string query = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                        + "FROM tWarehouseLocationMapType "
                        + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = ?"
                        + " AND tWarehouseLocationMapType.WarehouseLocationMapRefno = ?";
                    //cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                    //cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                    defaultSlotLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tWarehouseLocationMapTypeModel>(query, Convert.ToInt32(prodLocMapType), Convert.ToInt32(defaultWhsLocMapRefno)));
                }
            }

            if (defaultSlotLocMapRefno == 0 && locMap.CurrLocMap.IsSectionTracked && defaultWhsLocMapRefno != 0)
            {
                if (0 < prodRefno)
                {
                    string query = "SELECT tProductLocationMapType.LocationMapRefno "
                        + "FROM tProductLocationMapType "
                        + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                        + "= tProductLocationMapType.LocationMapRefno "
                        + "WHERE tProductLocationMapType.ProductRefno = ? "
                        + "AND tProductLocationMapType.ProductLocationMapTypeEnum = ? "
                        + "AND tLocationMapSection.WarehouseLocationMapRefno = ?";
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodRefno", Convert.ToInt32(prodRefno)));
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                    //cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                    defaultSectLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<dynamic>(query, Convert.ToInt32(prodRefno), Convert.ToInt32(prodLocMapType), Convert.ToInt32(defaultWhsLocMapRefno)));
                }

                if (defaultSectLocMapRefno == 0)
                {
                    string query = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                        + "FROM tWarehouseLocationMapType "
                        + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = ? "
                        + "AND tWarehouseLocationMapType.WarehouseLocationMapRefno = ?";
                    //cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                    //cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", defaultWhsLocMapRefno));

                    defaultSectLocMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tWarehouseLocationMapTypeModel>(query, Convert.ToInt32(prodLocMapType), defaultWhsLocMapRefno));
                }
            }

            if (defaultWhsLocMapRefno == 0)
            {
                defaultWhsLocMapRefno = locMap.LoginWhsLocMapRefno;
            }

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
        /// Gets the WarehouseLocationMapRefno from a LocationMapRefno.
        /// Checks the subtype in tLocationMap and then looks it up.
        /// </summary>
        /// <param name="locMapRefno">LocationMapRefno</param>
        /// <returns>WarehouseLocationMapRefno</returns>
        public static async Task<int> GetWhsLocMapRefnoFromLocMapRefno(int locMapRefno)
        {
            // First we check what the locationmaprefno subtype is
            // Perform SQL query to lookup subtype
            int locationSubType = 0;

            locationSubType = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapModel>("SELECT LocationMapSubType FROM tLocationMap WHERE LocationMapRefno = ?", locMapRefno));

            switch ((SysCommon.LocMapSubTypes)locationSubType)
            {
                default:
                case SysCommon.LocMapSubTypes.None:
                    return 0;

                case SysCommon.LocMapSubTypes.Section:
                    // Access Section table for WarehouseLocationMapRefno
                    return Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSectionModel>("SELECT WarehouseLocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = ?", Convert.ToInt32(locationSubType)));

                case SysCommon.LocMapSubTypes.Slot:
                    // Access Slot table for WarehouseLocationMapRefno
                    return Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<tLocationMapSlotModel>("SELECT WarehouseLocationMapRefno FROM tLocationMapSlot WHERE LocationMapRefno = ?", Convert.ToInt32(locationSubType)));

                case SysCommon.LocMapSubTypes.Warehouse:
                    // It already is a WarehouseLocationMapRefno
                    return locationSubType;
            }
        }

        /// <summary>
        /// The GetLocMapRefnoFromSLocMap
        /// </summary>
        /// <returns></returns>
        public static int GetLocMapRefnoFromSLocMap()
        {
            return GetLocMapRefnoFromSLocMap(LocationMap.GetInstance().CurrLocMap);
        }

        /// <summary>
        /// The GetLocMapRefnoFromSLocMap
        /// </summary>
        /// <param name="locMapInfo"></param>
        /// <returns></returns>
        public static int GetLocMapRefnoFromSLocMap(SLocMap locMapInfo)
        {
            int locMapRefno = 0;

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
