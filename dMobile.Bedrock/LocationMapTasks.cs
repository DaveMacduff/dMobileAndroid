namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using dMobile.Resources.Models;
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
                locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.QueryScalarsAsync<tLocationMapSlotModel>("SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotBarCode LIKE ?%", barCode));
            }


            // second attempt to find the location as a complete SECTION bar code
            if (locMapRefno == 0)
            {
                secondAttempt = false;

                //cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = @globalLocationIdentifier";
                //cmndLocMap.Parameters.Add(new SQLiteParameter("@globalLocationIdentifier", globalLocationIdentifier));

                List<tLocationMapSectionModel> tLocationMapSectionModelResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapSectionModel>("SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapRefno = ?", globalLocationIdentifier);

                if (tLocationMapSectionModelResults != null && tLocationMapSectionModelResults.Count > 0)
                {
                    //if (drLocMap.Read())
                    //{
                    locMapRefno = Convert.ToInt32(tLocationMapSectionModelResults[0].LocationMapRefno);
                    //}
                    //else
                    //{
                    //    throw new Exception("drLocMap.Read() failed.");
                    //}
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
                    //cmndLocMap.CommandType = CommandType.Text;
                    //cmndLocMap.IndexName = string.Empty;
                    //cmndLocMap.CommandText = "SELECT tLocationMapSection.LocationMapRefno FROM tLocationMapSection WHERE tLocationMapSection.LocationMapSectionBarCode LIKE ?%";

                    locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.QueryScalarsAsync<tLocationMapSectionModel>("SELECT tLocationMapSection.LocationMapRefno FROM tLocationMapSection WHERE tLocationMapSection.LocationMapSectionBarCode LIKE ?%", barCode));
                }
            }

            // third attempt to find the location as a complete WAREHOUSE bar code
            if (locMapRefno == 0)
            {
                secondAttempt = false;

                //cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapRefno = @globalLocationIdentifier";
                //cmndLocMap.Parameters.Add(new SQLiteParameter("@globalLocationIdentifier", globalLocationIdentifier));

                List<tLocationMapWarehouseModel> tLocationMapWarehouseModelResults = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tLocationMapWarehouseModel>("SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapRefno = ?", globalLocationIdentifier);

                if (tLocationMapWarehouseModelResults != null && tLocationMapWarehouseModelResults.Count > 0)
                {
                    locMapRefno = Convert.ToInt32(tLocationMapWarehouseModelResults[0].LocationMapRefno);

                    //if (drLocMap.Read())
                    //{
                    //locMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                    //}
                    //else
                    //{
                    //    throw new Exception("drLocMap.Read() failed.");
                    //}
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

                    //cmndLocMap.IndexName = string.Empty;
                    //cmndLocMap.CommandText = "SELECT tLocationMapWarehouse.LocationMapRefno FROM tLocationMapWarehouse WHERE tLocationMapWarehouse.LocationMapWarehouseBarCode LIKE ?%";
                    locMapRefno = Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.QueryScalarsAsync<tLocationMapWarehouseModel>("SELECT tLocationMapWarehouse.LocationMapRefno FROM tLocationMapWarehouse WHERE tLocationMapWarehouse.LocationMapWarehouseBarCode LIKE ?%", barCode));
                }

            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetWhsList
        /// </summary>
        /// <param name="dataTableWhsList">The dataTableWhsList<see cref="DataTable"/></param>
        public static void GetWhsList(ref DataTable dataTableWhsList)
        {
            string sql = "SELECT tLocationMapWarehouse.LocationMapWarehouseCode,"
                + "tLocationMapWarehouse.LocationMapWarehouseName,"
                + "tLocationMapWarehouse.LocationMapRefno "
                + "FROM tLocationMapWarehouse "
                + "ORDER BY tLocationMapWarehouse.LocationMapWarehouseCode";

            using (SQLiteDataAdapter dataAdapterWhs = new SQLiteDataAdapter(sql, DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetWhs = new DataSet())
                {
                    dataAdapterWhs.Fill(dataSetWhs);
                    dataTableWhsList = dataSetWhs.Tables[0];
                }
            }
        }

        /// <summary>
        /// The GetSectList
        /// </summary>
        /// <param name="dataTableSectionList">The dataTableSectionList<see cref="DataTable"/></param>
        public static void GetSectList(ref DataTable dataTableSectionList)
        {
            string sql = "SELECT tLocationMapSection.LocationMapSectionCode,"
                + "tLocationMapSection.LocationMapSectionName,"
                + "tLocationMapSection.LocationMapRefno,"
                + "tLocationMapSection.WarehouseLocationMapRefno "
                + "FROM tLocationMapSection "
                + "ORDER BY tLocationMapSection.LocationMapSectionCode";

            using (SQLiteDataAdapter dataAdapterSect = new SQLiteDataAdapter(sql, DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetSect = new DataSet())
                {
                    dataAdapterSect.Fill(dataSetSect);
                    dataTableSectionList = dataSetSect.Tables[0];
                }
            }
        }

        /// <summary>
        /// Gets the Section name from a LocationMapSectionFKRefno.
        /// </summary>
        /// <param name="locMapSectFKRefno">LocationMapSectionFKRefno</param>
        /// <returns>Name of the section.</returns>
        public static string GetSectionName(int locMapSectFKRefno)
        {
            string result = string.Empty;
            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT tLocationMapSection.LocationMapSectionName FROM tLocationMapSection WHERE tLocationMapSection.LocationMapSectionFKRefno = @locMapSectFKRefno";
                cmd.Parameters.Add(new SQLiteParameter("@locMapSectFKRefno", Convert.ToInt32(locMapSectFKRefno)));

                result = Convert.ToString(cmd.ExecuteScalar());
            }

            return result;
        }

        /// <summary>
        /// Gets the Slot name from a LocationMapSlotFKRefno.
        /// </summary>
        /// <param name="locMapSlotFKRefno">The locMapSlotFKRefno<see cref="int"/></param>
        /// <returns>Name of the slot.</returns>
        public static string GetSlotName(int locMapSlotFKRefno)
        {
            string result = string.Empty;
            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT tLocationMapSlot.LocationMapSlotName FROM tLocationMapSlot WHERE tLocationMapSlot.LocationMapSlotFKRefno = @locMapSlotFKRefno";
                cmd.Parameters.Add(new SQLiteParameter("@locMapSlotFKRefno", Convert.ToInt32(locMapSlotFKRefno)));

                result = Convert.ToString(cmd.ExecuteScalar());
            }

            return result;
        }

        ///// <summary>
        ///// The GetSlotCoordList
        ///// </summary>
        ///// <param name="coordinateEnum">The coordinateEnum<see cref="int"/></param>
        ///// <param name="dataTableSlotList">The dataTableSlotList<see cref="DataTable"/></param>
        //public static void GetSlotCoordList(int coordinateEnum, ref DataTable dataTableSlotList)
        //{
        //    using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        //    {
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandText = "SELECT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode, tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno " +
        //        "FROM tLocationMapSlotCoordinate " +
        //        "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = @coordinateEnum " +
        //        "ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";
        //        cmd.Parameters.Add(new SQLiteParameter("@coordinateEnum", Convert.ToInt32(coordinateEnum)));

        //        using (SQLiteDataAdapter dataAdapterSlot = new SQLiteDataAdapter(cmd))
        //        {
        //            using (DataSet dataSetSlot = new DataSet())
        //            {
        //                dataAdapterSlot.Fill(dataSetSlot);
        //                dataTableSlotList = dataSetSlot.Tables[0];
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// The GetSlotCoord1List
        /// </summary>
        /// <param name="dataTableSlotCoord1List">The dataTableSlotCoord1List<see cref="DataTable"/></param>
        public static void GetSlotCoord1List(ref DataTable dataTableSlotCoord1List)
        {
            int coord = Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord1);

            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                + "tLocationMapSlot.SectionLocationMapRefno "
                + "FROM tLocationMapSlot "
                + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                + "= tLocationMapSlot.SlotCoord1LocationMapRefno "
                + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = @coord"
                + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";
                cmd.Parameters.Add(new SQLiteParameter("@coord", Convert.ToInt32(coord)));

                using (SQLiteDataAdapter dataAdapterCoord = new SQLiteDataAdapter(cmd))
                {
                    using (DataSet dataSetCoord = new DataSet())
                    {
                        dataAdapterCoord.Fill(dataSetCoord);
                        dataTableSlotCoord1List = dataSetCoord.Tables[0];
                    }
                }
            }
        }

        /// <summary>
        /// The GetSlotCoord2List
        /// </summary>
        /// <param name="dataTableSlotCoord2List">The dataTableSlotCoord2List<see cref="DataTable"/></param>
        public static void GetSlotCoord2List(ref DataTable dataTableSlotCoord2List)
        {
            int coord = Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord2);

            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                + "tLocationMapSlot.SectionLocationMapRefno "
                + "FROM tLocationMapSlot "
                + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                + "= tLocationMapSlot.SlotCoord2LocationMapRefno "
                + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = @coord"
                + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";
                cmd.Parameters.Add(new SQLiteParameter("@coord", Convert.ToInt32(coord)));

                using (SQLiteDataAdapter dataAdapterCoord = new SQLiteDataAdapter(cmd))
                {
                    using (DataSet dataSetCoord = new DataSet())
                    {
                        dataAdapterCoord.Fill(dataSetCoord);
                        dataTableSlotCoord2List = dataSetCoord.Tables[0];
                    }
                }

            }
        }

        /// <summary>
        /// The GetSlotCoord3List
        /// </summary>
        /// <param name="dataTableSlotCoord3List">The dataTableSlotCoord3List<see cref="DataTable"/></param>
        public static void GetSlotCoord3List(ref DataTable dataTableSlotCoord3List)
        {
            int coord = Convert.ToInt32(SysCommon.LocMapSlotCoordTypes.Coord3);


            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DISTINCT tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode,"
                + "tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno,"
                + "tLocationMapSlot.SectionLocationMapRefno "
                + "FROM tLocationMapSlot "
                + "INNER JOIN tLocationMapSlotCoordinate ON tLocationMapSlotCoordinate.LocationMapSlotCoordinateRefno "
                + "= tLocationMapSlot.SlotCoord3LocationMapRefno "
                + "WHERE tLocationMapSlotCoordinate.LocationMapSlotCoordinateEnum = @coord"
                + " ORDER BY tLocationMapSlotCoordinate.LocationMapSlotCoordinateCode";
                cmd.Parameters.Add(new SQLiteParameter("@coord", Convert.ToInt32(coord)));

                using (SQLiteDataAdapter dataAdapterCoord = new SQLiteDataAdapter(cmd))
                {
                    using (DataSet dataSetCoord = new DataSet())
                    {
                        dataAdapterCoord.Fill(dataSetCoord);
                        dataTableSlotCoord3List = dataSetCoord.Tables[0];
                    }
                }
            }
        }

        /// <summary>
        /// The GetSlotRefnoFromCoordinates
        /// </summary>
        /// <param name="sectionLocMapRefno"></param>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <param name="coordinate3"></param>
        /// <returns></returns>
        public static int GetSlotRefnoFromCoordinates(int sectionLocMapRefno, int slotCoord1Refno, int slotCoord2Refno, int slotCoord3Refno)
        {
            int slotLocMapRefno = 0;
            LocationMap locMap = LocationMap.GetInstance();

            using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndLocMap.CommandType = CommandType.Text;

                if (locMap.CurrLocMap.IsSlotCoord1Tracked & locMap.CurrLocMap.IsSlotCoord2Tracked & locMap.CurrLocMap.IsSlotCoord3Tracked)
                {
                    cmndLocMap.CommandText = "SELECT tLocationMapSlot.LocationMapRefno "
                        + "FROM tLocationMapSlot "
                        + "WHERE tLocationMapSlot.SectionLocationMapRefno = @sectionLocMapRefno"
                        + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = @slotCoord1Refno"
                        + " AND tLocationMapSlot.SlotCoord2LocationMapRefno = @slotCoord2Refno"
                        + " AND tLocationMapSlot.SlotCoord3LocationMapRefno = @slotCoord3Refno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapRefno", Convert.ToInt32(sectionLocMapRefno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord1Refno", Convert.ToInt32(slotCoord1Refno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord2Refno", Convert.ToInt32(slotCoord2Refno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord3Refno", Convert.ToInt32(slotCoord3Refno)));
                }
                else if (locMap.CurrLocMap.IsSlotCoord1Tracked & locMap.CurrLocMap.IsSlotCoord2Tracked)
                {
                    cmndLocMap.CommandText = "SELECT tLocationMapSlot.LocationMapRefno "
                        + "FROM tLocationMapSlot "
                        + "WHERE tLocationMapSlot.SectionLocationMapRefno = @sectionLocMapRefno"
                        + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = @slotCoord1Refno"
                        + " AND tLocationMapSlot.SlotCoord2LocationMapRefno = @slotCoord2Refno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapRefno", Convert.ToInt32(sectionLocMapRefno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord1Refno", Convert.ToInt32(slotCoord1Refno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord2Refno", Convert.ToInt32(slotCoord2Refno)));
                }
                else if (locMap.CurrLocMap.IsSlotCoord1Tracked)
                {
                    cmndLocMap.CommandText = "SELECT tLocationMapSlot.LocationMapRefno "
                        + "FROM tLocationMapSlot "
                        + "WHERE tLocationMapSlot.SectionLocationMapRefno = @sectionLocMapRefno"
                        + " AND tLocationMapSlot.SlotCoord1LocationMapRefno = @slotCoord1Refno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapRefno", Convert.ToInt32(sectionLocMapRefno)));
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotCoord1Refno", Convert.ToInt32(slotCoord1Refno)));
                }
                else
                {
                    // TODO: Resource strings
                    throw new Exception("Location map tracking map settings are not set.");
                }

                using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                {
                    if (drLocMap.Read())
                    {
                        slotLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                    }

                    drLocMap.Close();
                }
            }

            return slotLocMapRefno;
        }

        /// <summary>
        /// The IsCompleteLocMapRefno
        /// </summary>
        /// <param name="locMap"></param>
        /// <returns></returns>
        public static bool IsCompleteLocMapRefno(SLocMap locMap)
        {
            bool isComplete = false;

            if (LocationMap.GetInstance().CurrLocMap.IsSlotTracked)
            {
                isComplete = IsCompleteLocMapRefno(locMap.SlotLocMapRefno);
            }
            else if (LocationMap.GetInstance().CurrLocMap.IsSectionTracked)
            {
                isComplete = IsCompleteLocMapRefno(locMap.SectLocMapRefno);
            }
            else
            {
                isComplete = IsCompleteLocMapRefno(locMap.WhsLocMapRefno);
            }

            return isComplete;
        }

        /// <summary>
        /// The IsCompleteLocMapRefno
        /// </summary>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        public static bool IsCompleteLocMapRefno(int locMapRefno)
        {
            bool isComplete = false;
            SysCommon.LocMapSubTypes locMapSubType = SysCommon.LocMapSubTypes.None;
            LocationMap locMap = LocationMap.GetInstance();

            using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndLocMap.CommandType = CommandType.Text;
                cmndLocMap.CommandText = "SELECT * FROM tLocationMap WHERE LocationMapRefno = @locMapRefno";
                cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

                using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                {
                    if (drLocMap.Read())
                    {
                        locMapSubType = (SysCommon.LocMapSubTypes)drLocMap.GetInt32(Convert.ToInt32(SysCommon.LocMapFields.SubType));
                    }
                    else
                    {
                        ////throw new Exception(string.Format(
                        ////   "Unable to locate location map refno {0} in tLocationMap",
                        ////   locMapRefno));
                    }

                }
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

        /// <summary>
        /// The GetWhsSectSlotLocMapRefnosFromLocMapRefno
        /// </summary>
        /// <param name="locMapRefno"></param>
        /// <param name="warehouseLocMapRefno"></param>
        /// <param name="sectionLocMapRefno">The sectionLocMapRefno<see cref="int"/></param>
        /// <param name="slotLocMapRefno"></param>
        public static void GetWhsSectSlotLocMapRefnosFromLocMapRefno(int locMapRefno, ref int warehouseLocMapRefno, ref int sectionLocMapRefno, ref int slotLocMapRefno)
        {
            warehouseLocMapRefno = 0;
            sectionLocMapRefno = 0;
            slotLocMapRefno = 0;

            using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndLocMap.CommandType = CommandType.Text;
                cmndLocMap.CommandText = "SELECT tWarehouseLocationMap.LocationMapRefno AS WarehouseLocationMapRefno,"
                    + "CAST(0, Integer) AS SectionLocationMapRefno,"
                    + "CAST(0, Integer) AS SlotLocationMapRefno "
                    + "FROM tWarehouseLocationMap "
                    + "WHERE tWarehouseLocationMap.LocationMapRefno = @warehouseLocMapRefno"
                    + " UNION "
                    + "SELECT tSectionLocationMap.WarehouseLocationMapRefno,"
                    + "tSectionLocationMap.LocationMapRefno AS SectionLocationMapRefno,"
                    + "CAST(0, Integer) AS SlotLocationMapRefno "
                    + "FROM tSectionLocationMap "
                    + "WHERE tSectionLocationMap.LocationMapRefno = @sectionLocMapRefno"
                    + " UNION "
                    + "SELECT tWarehouseLocationMap.LocationMapRefno AS WarehouseLocationMapRefno,"
                    + "tSlotLocationMap.SectionLocationMapRefno,"
                    + "tSlotLocationMap.LocationMapRefno AS SlotLocationMapRefno "
                    + "FROM tSlotLocationMap "
                    + "INNER JOIN tSectionLocationMap ON tSectionLocationMap.LocationMapRefno "
                    + "= tSlotLocationMap.SectionLocationMapRefno "
                    + "INNER JOIN tWarehouseLocationMap ON tWarehouseLocationMap.LocationMapRefno "
                    + "= tSectionLocationMap.WarehouseLocationMapRefno "
                    + "WHERE tSlotLocationMap.LocationMapRefno = @slotLocMapRefno";
                cmndLocMap.Parameters.Add(new SQLiteParameter("@warehouseLocMapRefno", Convert.ToInt32(warehouseLocMapRefno)));
                cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapRefno", Convert.ToInt32(sectionLocMapRefno)));
                cmndLocMap.Parameters.Add(new SQLiteParameter("@slotLocMapRefno", Convert.ToInt32(slotLocMapRefno)));

                using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                {
                    if (drLocMap.Read())
                    {
                        warehouseLocMapRefno = Convert.ToInt32(drLocMap["WarehouseLocationMapRefno"]);
                        sectionLocMapRefno = Convert.ToInt32(drLocMap["SectionLocationMapRefno"]);
                        slotLocMapRefno = Convert.ToInt32(drLocMap["SlotLocationMapRefno"]);
                    }

                    drLocMap.Close();
                }
            }
        }

        /// <summary>
        /// The GetSLocMapFromLocMapFKRefno
        /// </summary>
        /// <param name="warehouseLocMapFKRefno"></param>
        /// <param name="sectionLocMapFKRefno"></param>
        /// <param name="slotLocnMapFKRefno"></param>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        public static SLocMap GetSLocMapFromLocMapFKRefno(int warehouseLocMapFKRefno, int sectionLocMapFKRefno, int slotLocnMapFKRefno, ref int locMapRefno)
        {
            SLocMap retLocMap = new SLocMap();
            int tempLocMapRefno = 0;

            if (tempLocMapRefno == 0 & slotLocnMapFKRefno != 0)
            {
                using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapSlot WHERE LocationMapSlotFKRefno = @slotLocnMapFKRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@slotLocnMapFKRefno", Convert.ToInt32(slotLocnMapFKRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                        }
                    }
                }
            }

            if (tempLocMapRefno == 0 & sectionLocMapFKRefno != 0)
            {
                using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapSection WHERE LocationMapSectionFKRefno = @sectionLocMapFKRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@sectionLocMapFKRefno", Convert.ToInt32(sectionLocMapFKRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                        }
                    }
                }
            }

            if (tempLocMapRefno == 0 & warehouseLocMapFKRefno != 0)
            {
                using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT LocationMapRefno FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @warehouseLocMapFKRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@warehouseLocMapFKRefno", Convert.ToInt32(warehouseLocMapFKRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            tempLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                        }
                    }
                }
            }

            retLocMap = GetSLocMapFromLocMapRefno(tempLocMapRefno);

            locMapRefno = tempLocMapRefno;

            return retLocMap;
        }

        /// <summary>
        /// The GetSLocMapFromLocMapRefno
        /// </summary>
        /// <param name="locMapRefno"></param>
        /// <returns></returns>
        public static SLocMap GetSLocMapFromLocMapRefno(int locMapRefno)
        {
            SLocMap locMapInfo = new SLocMap();
            SysCommon.LocMapSubTypes locMapSubType = default(SysCommon.LocMapSubTypes);

            using (SQLiteCommand cmndLocMap = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndLocMap.CommandType = CommandType.Text;
                cmndLocMap.CommandText = "SELECT * FROM tLocationMap WHERE LocationMapRefno = @locMapRefno";
                cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

                using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                {
                    if (drLocMap.Read())
                    {
                        locMapSubType = (SysCommon.LocMapSubTypes)Convert.ToInt32(drLocMap["LocationMapSubType"]);
                    }
                }

                if (locMapSubType == SysCommon.LocMapSubTypes.Slot)
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT tLocationMapSlot.LocationMapRefno, "
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
                        + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                        + "= tLocationMapSlot.SectionLocationMapRefno "
                        + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
                        + "= tLocationMapSection.WarehouseLocationMapRefno "
                        + "WHERE tLocationMapSlot.LocationMapRefno = @locMapRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            locMapInfo.SlotLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                            locMapInfo.SlotCode = Convert.ToInt32(drLocMap["LocationMapSlotCode"]);
                            locMapInfo.SlotName = Convert.ToString(drLocMap["LocationMapSlotName"]);
                            locMapInfo.SlotFKRefno = Convert.ToInt32(drLocMap["LocationMapSlotFKRefno"]);
                            locMapInfo.SlotCoord1LocMapRefno = Convert.ToInt32(drLocMap["SlotCoord1LocationMapRefno"]);
                            locMapInfo.SlotCoord2LocMapRefno = Convert.ToInt32(drLocMap["SlotCoord2LocationMapRefno"]);
                            locMapInfo.SlotCoord3LocMapRefno = Convert.ToInt32(drLocMap["SlotCoord3LocationMapRefno"]);

                            locMapInfo.SectLocMapRefno = Convert.ToInt32(drLocMap["SectionLocationMapRefno"]);
                            locMapInfo.SectCode = Convert.ToInt32(drLocMap["LocationMapSectionCode"]);
                            locMapInfo.SectName = Convert.ToString(drLocMap["LocationMapSectionName"]);
                            locMapInfo.SectFKRefno = Convert.ToInt32(drLocMap["LocationMapSectionFKRefno"]);

                            locMapInfo.WhsLocMapRefno = Convert.ToInt32(drLocMap["WarehouseLocationMapRefno"]);
                            locMapInfo.WhsName = Convert.ToString(drLocMap["LocationMapWarehouseName"]);
                            locMapInfo.WhsCode = Convert.ToInt32(drLocMap["LocationMapWarehouseCode"]);
                            locMapInfo.IsWhsWMS = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsWMS"]);
                            locMapInfo.WhsFKRefno = Convert.ToInt32(drLocMap["LocationMapWarehouseFKRefno"]);
                            locMapInfo.WhsCodeMask = Convert.ToString(drLocMap["LocationMapWarehouseMask"]);
                            locMapInfo.SectMask = Convert.ToString(drLocMap["LocationMapWarehouseSectionMask"]);
                            locMapInfo.IsSectionTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSectionTracked"]);
                            locMapInfo.IsSlotTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSlotTracked"]);
                            locMapInfo.SlotCoord1Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Name"]);
                            locMapInfo.SlotCoord2Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Name"]);
                            locMapInfo.SlotCoord3Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Name"]);
                            locMapInfo.SlotMask = Convert.ToString(drLocMap["LocationMapWarehouseSlotMask"]);
                            locMapInfo.SlotCoord1Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Mask"]);
                            locMapInfo.SlotCoord2Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Mask"]);
                            locMapInfo.SlotCoord3Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Mask"]);
                        }

                        drLocMap.Close();
                    }
                }
                else if (locMapSubType == SysCommon.LocMapSubTypes.Section)
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT tLocationMapSection.LocationMapRefno, "
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
                        + "WHERE tLocationMapSection.LocationMapRefno = @locMapRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            locMapInfo.SectLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                            locMapInfo.SectCode = Convert.ToInt32(drLocMap["LocationMapSectionCode"]);
                            locMapInfo.SectName = Convert.ToString(drLocMap["LocationMapSectionName"]);
                            locMapInfo.SectFKRefno = Convert.ToInt32(drLocMap["LocationMapSectionFKRefno"]);

                            locMapInfo.WhsLocMapRefno = Convert.ToInt32(drLocMap["WarehouseLocationMapRefno"]);
                            locMapInfo.WhsName = Convert.ToString(drLocMap["LocationMapWarehouseName"]);
                            locMapInfo.WhsCode = Convert.ToInt32(drLocMap["LocationMapWarehouseCode"]);
                            locMapInfo.IsWhsWMS = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsWMS"]);
                            locMapInfo.WhsFKRefno = Convert.ToInt32(drLocMap["LocationMapWarehouseFKRefno"]);
                            locMapInfo.WhsCodeMask = Convert.ToString(drLocMap["LocationMapWarehouseMask"]);
                            locMapInfo.SectMask = Convert.ToString(drLocMap["LocationMapWarehouseSectionMask"]);
                            locMapInfo.IsSectionTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSectionTracked"]);
                            locMapInfo.IsSlotTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSlotTracked"]);
                            locMapInfo.SlotCoord1Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Name"]);
                            locMapInfo.SlotCoord2Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Name"]);
                            locMapInfo.SlotCoord3Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Name"]);
                            locMapInfo.SlotMask = Convert.ToString(drLocMap["LocationMapWarehouseSlotMask"]);
                            locMapInfo.SlotCoord1Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Mask"]);
                            locMapInfo.SlotCoord2Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Mask"]);
                            locMapInfo.SlotCoord3Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Mask"]);
                        }

                        drLocMap.Close();
                    }
                }
                else if (locMapSubType == SysCommon.LocMapSubTypes.Warehouse)
                {
                    cmndLocMap.CommandType = CommandType.Text;
                    cmndLocMap.CommandText = "SELECT tLocationMapWarehouse.LocationMapRefno, "
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
                        + "WHERE tLocationMapWarehouse.LocationMapRefno = @locMapRefno";
                    cmndLocMap.Parameters.Add(new SQLiteParameter("@locMapRefno", Convert.ToInt32(locMapRefno)));

                    using (SQLiteDataReader drLocMap = cmndLocMap.ExecuteReader())
                    {
                        if (drLocMap.Read())
                        {
                            locMapInfo.WhsLocMapRefno = Convert.ToInt32(drLocMap["LocationMapRefno"]);
                            locMapInfo.WhsName = Convert.ToString(drLocMap["LocationMapWarehouseName"]);
                            locMapInfo.WhsCode = Convert.ToInt32(drLocMap["LocationMapWarehouseCode"]);
                            locMapInfo.IsWhsWMS = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsWMS"]);
                            locMapInfo.WhsFKRefno = Convert.ToInt32(drLocMap["LocationMapWarehouseFKRefno"]);
                            locMapInfo.WhsCodeMask = Convert.ToString(drLocMap["LocationMapWarehouseMask"]);
                            locMapInfo.SectMask = Convert.ToString(drLocMap["LocationMapWarehouseSectionMask"]);
                            locMapInfo.IsSectionTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSectionTracked"]);
                            locMapInfo.IsSlotTracked = Convert.ToBoolean(drLocMap["LocationMapWarehouseIsSlotTracked"]);
                            locMapInfo.SlotCoord1Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Name"]);
                            locMapInfo.SlotCoord2Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Name"]);
                            locMapInfo.SlotCoord3Name = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Name"]);
                            locMapInfo.SlotMask = Convert.ToString(drLocMap["LocationMapWarehouseSlotMask"]);
                            locMapInfo.SlotCoord1Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate1Mask"]);
                            locMapInfo.SlotCoord2Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate2Mask"]);
                            locMapInfo.SlotCoord3Mask = Convert.ToString(drLocMap["LocationMapWarehouseSlotCoordinate3Mask"]);
                        }

                        drLocMap.Close();
                    }
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
                odd = odd + (int.Parse(globalLocationIdentifier.Substring(i - 1, 1)) * 3);
            }

            for (i = 2; i <= globalLocationIdentifier.Length; i += 2)
            {
                even = even + int.Parse(globalLocationIdentifier.Substring(i - 1, 1));
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
        public static int GetDefaultLocMapRefnoFromProdRefno(int whsLocMapRefno, int prodRefno, SysCommon.ProdLocMapTypes prodLocMapType)
        {
            int locMapRefno = 0;
            int defaultWhsLocMapRefno = 0;
            int defaultSectLocMapRefno = 0;
            int defaultSlotLocMapRefno = 0;
            LocationMap locMap = LocationMap.GetInstance();

            using (SQLiteCommand cmndProdLocMapType = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdLocMapType.CommandType = CommandType.Text;
                //cmndProdLocMapType.IndexName = string.Empty;

                defaultWhsLocMapRefno = whsLocMapRefno;

                if (locMap.CurrLocMap.IsSlotTracked && defaultWhsLocMapRefno != 0)
                {
                    if (0 < prodRefno)
                    {
                        cmndProdLocMapType.CommandText = "SELECT tProductLocationMapType.LocationMapRefno "
                            + "FROM tProductLocationMapType "
                            + "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno "
                            + "= tProductLocationMapType.LocationMapRefno "
                            + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                            + "= tLocationMapSlot.SectionLocationMapRefno "
                            + "WHERE tProductLocationMapType.ProductRefno = @prodRefno "
                            + "AND tProductLocationMapType.ProductLocationMapTypeEnum = @prodLocMapType "
                            + "AND tLocationMapSection.WarehouseLocationMapRefno = @defaultWhsLocMapRefno";
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodRefno", Convert.ToInt32(prodRefno)));
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                        defaultSlotLocMapRefno = Convert.ToInt32(cmndProdLocMapType.ExecuteScalar());
                    }

                    if (defaultSlotLocMapRefno == 0)
                    {
                        using (SQLiteCommand cmndWhsLocMapType = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndWhsLocMapType.CommandType = CommandType.Text;

                            cmndWhsLocMapType.CommandText = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                                + "FROM tWarehouseLocationMapType "
                                + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = @prodLocMapType"
                                + " AND tWarehouseLocationMapType.WarehouseLocationMapRefno = @defaultWhsLocMapRefno";
                            cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                            cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                            defaultSlotLocMapRefno = Convert.ToInt32(cmndWhsLocMapType.ExecuteScalar());
                        }
                    }
                }

                if (defaultSlotLocMapRefno == 0 && locMap.CurrLocMap.IsSectionTracked && defaultWhsLocMapRefno != 0)
                {
                    if (0 < prodRefno)
                    {
                        cmndProdLocMapType.CommandText = "SELECT tProductLocationMapType.LocationMapRefno "
                            + "FROM tProductLocationMapType "
                            + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno "
                            + "= tProductLocationMapType.LocationMapRefno "
                            + "WHERE tProductLocationMapType.ProductRefno = @prodRefno "
                            + "AND tProductLocationMapType.ProductLocationMapTypeEnum = @prodLocMapType "
                            + "AND tLocationMapSection.WarehouseLocationMapRefno = @defaultWhsLocMapRefno";
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodRefno", Convert.ToInt32(prodRefno)));
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                        cmndProdLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", Convert.ToInt32(defaultWhsLocMapRefno)));

                        defaultSectLocMapRefno = Convert.ToInt32(cmndProdLocMapType.ExecuteScalar());
                    }

                    if (defaultSectLocMapRefno == 0)
                    {
                        using (SQLiteCommand cmndWhsLocMapType = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndWhsLocMapType.CommandType = CommandType.Text;

                            cmndWhsLocMapType.CommandText = "SELECT tWarehouseLocationMapType.LocationMapRefno "
                                + "FROM tWarehouseLocationMapType "
                                + "WHERE tWarehouseLocationMapType.WarehouseLocationMapTypeEnum = @prodLocMapType "
                                + "AND tWarehouseLocationMapType.WarehouseLocationMapRefno = @defaultWhsLocMapRefno";
                            cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@prodLocMapType", Convert.ToInt32(prodLocMapType)));
                            cmndWhsLocMapType.Parameters.Add(new SQLiteParameter("@defaultWhsLocMapRefno", defaultWhsLocMapRefno));

                            defaultSectLocMapRefno = Convert.ToInt32(cmndWhsLocMapType.ExecuteScalar());
                        }
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
            }

            return locMapRefno;
        }

        /// <summary>
        /// Gets the WarehouseLocationMapRefno from a LocationMapRefno.
        /// Checks the subtype in tLocationMap and then looks it up.
        /// </summary>
        /// <param name="locMapRefno">LocationMapRefno</param>
        /// <returns>WarehouseLocationMapRefno</returns>
        public static int GetWhsLocMapRefnoFromLocMapRefno(int locMapRefno)
        {
            // First we check what the locationmaprefno subtype is
            // Perform SQL query to lookup subtype
            int locationSubType = 0;

            using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "SELECT tLocationMap.LocationMapSubType "
                    + "FROM tLocationMap "
                    + "WHERE tLocationMap.LocationMapRefno = @locMapRefno";
                cmd.Parameters.Add(new SQLiteParameter("@locMapRefno", locMapRefno));

                locationSubType = Convert.ToInt32(cmd.ExecuteScalar());
            }

            switch ((SysCommon.LocMapSubTypes)locationSubType)
            {
                default:
                case SysCommon.LocMapSubTypes.None:
                    return 0;
                case SysCommon.LocMapSubTypes.Section:
                    // Access Section table for WarehouseLocationMapRefno
                    using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.CommandText = "SELECT tLocationMapSection.WarehouseLocationMapRefno "
                            + "FROM tLocationMapSection "
                            + "WHERE tLocationMapSection.LocationMapRefno = @locationSubType";
                        cmd.Parameters.Add(new SQLiteParameter("@locationSubType", Convert.ToInt32(locationSubType)));

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                case SysCommon.LocMapSubTypes.Slot:
                    // Access Slot table for WarehouseLocationMapRefno
                    using (SQLiteCommand cmd = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.CommandText = "SELECT tLocationMapSlot.WarehouseLocationMapRefno "
                            + "FROM tLocationMapSlot "
                            + "WHERE tLocationMapSlot.LocationMapRefno = @locationSubType";
                        cmd.Parameters.Add(new SQLiteParameter("@locationSubType", Convert.ToInt32(locationSubType)));

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
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
