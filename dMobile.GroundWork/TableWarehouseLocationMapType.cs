namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableWarehouseLocationMapType : TableBase
  {
    #region Constructors

    public TableWarehouseLocationMapType()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableWarehouseLocationMapType";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocations), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations.", true);
#endif

        GC.Collect();
        GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultPickingLocations), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default picking locations.", true);
#endif
        string reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Picking);
        if (0 < reqData.Length)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument reqErr = new XmlDocument();
            reqErr.LoadXml(reqData);
            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
          }

          this.Save(reqData);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DefaultReceivingLocations), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Default receiving locations.", true);
#endif
        reqData = this.SendRequest((int)SysCommon.ProdLocMapTypes.Receiving);
        if (0 < reqData.Length)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument reqErr = new XmlDocument();
            reqErr.LoadXml(reqData);
            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
          }

          this.Save(reqData);
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse locations failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouseLocationsFailed), ex);
#else
        throw new Exception("Seeding warehouse locations failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override int DataStatus()
    {
      int rowsAffected = 0;

      try
      {
        using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tWarehouseLocationMapType";
          cmndStatus.CommandType = CommandType.Text;
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.WarehouseLocations), rowsAffected),
#else
              string.Format("{0} warehouse locations.", rowsAffected),
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.WarehouseLocationsStatusFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "warehouse locations status failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredWarehouseLocationStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Warehouse location status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouseLocations), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tWarehouseLocationMapType";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouseLocations), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse locations.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingWarehouseLocations), ex);
#else
        throw new Exception("Database exception occurred. Purging warehouse locations.", ex);
#endif
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="whsLocMapType"></param>
    /// <param name="whsLocMapFKRefno"></param>
    /// <returns></returns>
    protected override string SendRequest(int whsLocMapType)
    {
      return this.SendComm(this.CreateRequest(whsLocMapType));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="whsLocMapRefno"></param>
    protected override void Save(string reqData)
    {
      int locMapRefno = 0;
      int whsLocMapType = 0;
      int locMapFKRefno = 0;
      int locMapSubType = 0;
      int currWhsLocMapRefno = 0;
      int whsLocMapFKRefno = 0;
      int whsLocMapRefno = 0;
      string errMsg = string.Empty;
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;

      try
      {
        using (DataSet ds = new DataSet())
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
#endif
          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehouseLocations), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this,
                EventArgs.Empty,
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SQLiteCommand cmndLocMapType = conn.CreateCommand())
            {
              cmndLocMapType.CommandText = "tWarehouseLocationMapType";
              cmndLocMapType.CommandType = CommandType.TableDirect;
              cmndLocMapType.IndexName =
                    "UQ_WarehouseLocationMapTypeOnWarehouseLocationMapRefnoAndWarehouseLocationMapTypeEnum";

              using (SQLiteCommand cmndLocMapTypeUpdate = conn.CreateCommand())
              {
                cmndLocMapTypeUpdate.CommandText = string.Empty;
                cmndLocMapTypeUpdate.CommandType = CommandType.Text;
                cmndLocMapTypeUpdate.IndexName = string.Empty;

                using (SqliteDataReader drLocMapType = cmndLocMapType.ExecuteReader())
                {
                  using (SQLiteCommand cmndWhs = conn.CreateCommand())
                  {
                    cmndWhs.CommandText = "tLocationMapWarehouse";
                    cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                    cmndWhs.CommandType = CommandType.TableDirect;

                    using (SqliteDataReader drWhs = cmndWhs.ExecuteReader())
                    {
                      using (SQLiteCommand cmndSect = conn.CreateCommand())
                      {
                        cmndSect.CommandText = "tLocationMapSection";
                        cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";
                        cmndSect.CommandType = CommandType.TableDirect;

                        using (SqliteDataReader drSect = cmndSect.ExecuteReader())
                        {
                          using (SQLiteCommand cmndSlot = conn.CreateCommand())
                          {
                            cmndSlot.CommandText = "tLocationMapSlot";
                            cmndSlot.IndexName = "UQ_LocationMapSlotOnLocationMapSlotFKRefno";
                            cmndSlot.CommandType = CommandType.TableDirect;

                            using (SqliteDataReader drSlot = cmndSlot.ExecuteReader())
                            {
                              using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(
                                  ResultSetOptions.Updatable))
                              {
                                SqlCeUpdatableRecord rec = rs.CreateRecord();
                                int progValue = 0;
                                bool isLocMapTypeFound = false;

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                foreach (DataRow drow in ds.Tables["seg"].Rows)
                                {
                                  whsLocMapFKRefno = Convert.ToInt32(drow["WarehouseLocationMapFKRefno"]);
                                  errMsg += Environment.NewLine + "Warehouse fk refno " + whsLocMapFKRefno;
                                  if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapFKRefno }))
                                  {
                                    if (drWhs.Read())
                                    {
                                      whsLocMapRefno = Convert.ToInt32(drWhs.GetValue(
                                          (int)SysCommon.LocMapWhsFields.LocMapRefno));
                                    }
                                    else
                                    {
#if MULTILANGUAGE
                                      throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine
#else
                                      throw new Exception("drWhs.Read() failed." + Environment.NewLine
#endif
                                          + errMsg);
                                    }
                                  }
                                  else
                                  {
                                    throw new Exception(
#if MULTILANGUAGE
                                        Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse)
#else
                                        "Unable to locate the warehouse fk refno in tLocationMapWarehouse."
#endif
                                        + Environment.NewLine + errMsg);
                                  }

                                  locMapFKRefno = Convert.ToInt32(drow["LocationMapFKRefno"]);
                                  errMsg += Environment.NewLine + "Map fk refno " + locMapFKRefno;
                                  
                                  // locMapSubType = Warehouse, Section, Slot.
                                  // Create error message for locMapSubType.
                                  locMapSubType = Convert.ToInt32(drow["LocationMapSubType"]);
                                  switch (locMapSubType)
                                  {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                      errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                          + ", 'Warehouse'.";
                                      break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                      errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                          + ", 'Section'.";
                                      break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                      errMsg += Environment.NewLine + "Map sub type = " + locMapSubType
                                          + ", 'Slot'.";
                                      break;

                                    default:
                                      throw new Exception(
                                          string.Format(
#if MULTILANGUAGE
                                          Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
                                          "Undefined location map subtype {0}",
#endif
                                          locMapSubType));
                                  }

                                  // whsLocMapType = Picking, Receiving, Floor.
                                  whsLocMapType = Convert.ToInt32(drow["WarehouseLocationMapType"]);
                                  errMsg += Environment.NewLine + "Map type " + whsLocMapType;

                                  // Find the key for the locMapSubType.
                                  switch (locMapSubType)
                                  {
                                    case (int)SysCommon.LocMapSubTypes.Warehouse:
                                      if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                      {
                                        if (drWhs.Read())
                                        {
                                          locMapRefno = Convert.ToInt32(drWhs.GetValue(
                                              (int)SysCommon.LocMapWhsFields.LocMapRefno));
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine 
#else
                                          throw new Exception("drWhs.Read() failed." + Environment.NewLine 
#endif
                                              + errMsg);
                                        }
                                      }
                                      else
                                      {
                                        throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseFkRefnoInTlocationmapwarehouse)
#else
                                            "Unable to locate the warehouse fk refno in tLocationMapWarehouse."
#endif
                                            + Environment.NewLine + errMsg);
                                      }

                                      break;

                                    case (int)SysCommon.LocMapSubTypes.Section:
                                      if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                      {
                                        if (drSect.Read())
                                        {
                                          locMapRefno = Convert.ToInt32(
                                              drSect.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine
#else
                                          throw new Exception("drSect.Read() failed." + Environment.NewLine
#endif
                                              + errMsg);
                                        }
                                      }
                                      else
                                      {
                                        throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSectionFkRefnoInTlocationmapsection)
#else
                                            "Unable to locate section fk refno in tLocationMapSection."
#endif
                                            + Environment.NewLine + errMsg);
                                      }

                                      break;

                                    case (int)SysCommon.LocMapSubTypes.Slot:
                                      if (drSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                      {
                                        if (drSlot.Read())
                                        {
                                          locMapRefno = Convert.ToInt32(
                                              drSlot.GetValue((int)SysCommon.LocMapSlotFields.LocMapRefno));
                                        }
                                        else
                                        {
#if MULTILANGUAGE
                                          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrslotreadFailed) + Environment.NewLine 
#else
                                          throw new Exception("drSlot.Read() failed." + Environment.NewLine 
#endif
                                              + errMsg);
                                        }
                                      }
                                      else
                                      {
                                        throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateSlotFkRefnoInTlocationmapslot)
#else
                                            "Unable to locate slot fk refno in tLocationMapSlot."
#endif
                                            + Environment.NewLine + errMsg);
                                      }

                                      break;

                                    default:
                                      throw new Exception(
                                          string.Format(
#if MULTILANGUAGE
                                          Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtypez),
#else
                                          "Undefined location map subtype {0}",
#endif
                                          locMapSubType));
                                  }

                                  isLocMapTypeFound = false;

                                  currWhsLocMapRefno = 0;
                                  if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                  {
                                    if (drLocMapType.Seek(
                                        DbSeekOptions.FirstEqual,
                                        new object[] { whsLocMapRefno, whsLocMapType }))
                                    {
                                      if (drLocMapType.Read())
                                      {
                                        currWhsLocMapRefno = Convert.ToInt32(
                                            drLocMapType.GetValue((int)SysCommon.WhsLocMapTypeFields.LocMapRefno));
                                        isLocMapTypeFound = true;
                                      }
                                      else
                                      {
                                        throw new Exception(
#if MULTILANGUAGE
                                            Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmaptypereadFailed) + Environment.NewLine + errMsg);
#else
                                            "drLocMapType.Read() failed." + Environment.NewLine + errMsg);
#endif
                                      }
                                    }
                                  }

                                  if (isLocMapTypeFound)
                                  {
                                    if (currWhsLocMapRefno != locMapRefno)
                                    {
                                      // Update the LocMapRefno if different
                                      cmndLocMapTypeUpdate.CommandText = "UPDATE tWarehouseLocationMapType"
                                        + " SET LocationMapRefno = " + locMapRefno
                                        + " WHERE WarehouseLocationMapRefno = " + whsLocMapRefno
                                        + " AND WarehouseLocationMapTypeEnum = " + whsLocMapType;
                                      cmndLocMapTypeUpdate.ExecuteNonQuery();
                                    }
                                  }
                                  else
                                  {
                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.WhsLocMapTypeFields.WhsLocMapRefno),
                                        whsLocMapRefno);

                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.WhsLocMapTypeFields.EnumType),
                                        whsLocMapType);

                                    rec.SetInt32(
                                        Convert.ToInt32(SysCommon.WhsLocMapTypeFields.LocMapRefno),
                                        locMapRefno);

                                    rs.Insert(rec);
                                  }

                                  progValue++;
                                  if (progValue % 100 == 0)
                                  {
                                    this.DataAccessProgressOutHandler(
                                        this,
                                        EventArgs.Empty,
                                        progValue,
                                        0,
                                        progMax);
                                  }
                                }

                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                              }

                              drSlot.Close();
                            }
                          }

                          drSect.Close();
                        }
                      }

                      drWhs.Close();
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehouseLocationsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehouseLocationsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse locations failed.", false);
#endif

        // Duplicate key violations
        if (ex.NativeError == 25016)
        {
#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
#endif
        }

        throw new Exception(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingWarehouseLocationsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving warehouse locations failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string CreateRequest(int whsLocMapType)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST WAREHOUSE LOCATION MAP TYPES</action>"
          + SysCommon.GetRequestDefaults(true)
          + ("<warehouse_location_map_type>" + whsLocMapType + "</warehouse_location_map_type>") 
          + "</command>" 
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    #endregion
  }
}
