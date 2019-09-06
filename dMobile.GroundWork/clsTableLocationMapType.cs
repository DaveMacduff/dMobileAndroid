namespace dMobile.GroundWork
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  
  using System.Text;
  using System.Windows.Forms;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableLocationMapType : TableBase
  {
    #region Constructors

    public TableLocationMapType()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableLocationMapType";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingLocationMapTypes), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types.", true);
#endif

        string reqData = string.Empty;
        int whsLocMapRefno = 0;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        reqData = this.Send(whsLocMapRefno);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingLocationMapTypesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingLocationMapTypesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding location map types failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingLocationMapTypesFailed), ex);
#else
        throw new Exception("Seeding location map types failed.", ex);
#endif
      }
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingLocationMapTypes), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tLocationMapType";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingLocationMapTypesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingLocationMapTypesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging location map types failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingLocationMapTypesFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging location map types failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapType";
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.LocationMapTypes), rowsAffected), 
#else
              string.Format("{0} location map types.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredLocationMapTypesStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Location map types status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int whsLocMapRefno)
    {
      return this.SendComm(this.Create(whsLocMapRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData)
    {
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingLocationMapTypes), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["seg"].Rows.Count);
#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif

            using (SQLiteCommand cmndLocMapType = conn.CreateCommand())
            {
              cmndLocMapType.CommandText = "tLocationMapType";
              cmndLocMapType.CommandType = CommandType.TableDirect;
              cmndLocMapType.IndexName = "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";

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
                            using (SqlCeResultSet rs = cmndLocMapType.ExecuteResultSet(ResultSetOptions.Updatable))
                            {
                              SqlCeUpdatableRecord rec = rs.CreateRecord();
                              int locMapRefno = 0;
                              int locMapSubType = 0;
                              int locMapFKRefno = 0;
                              int progValue = 0;
                              bool isLocMapTypeFound = false;
                              int locMapTypeEnum = 0;

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
                                    throw new Exception(
                                      string.Format(
#if MULTILANGUAGE
                                      Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapType), 
#else
                                      "Undefined location map type {0}", 
#endif
                                      locMapTypeEnum));
                                }

                                locMapSubType = Convert.ToInt32(drow["f02"]);
                                switch (locMapSubType)
                                {
                                  case (int)SysCommon.LocMapSubTypes.Warehouse:
                                    errMsg += Environment.NewLine + "Map sub type \'Warehouse\'";

                                    if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapFKRefno }))
                                    {
                                      if (drWhs.Read())
                                      {
                                        locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                      }
                                      else
                                      {
#if MULTILANGUAGE
                                        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
#endif
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
                                    errMsg += Environment.NewLine + "Map sub type \'Section\'";

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
                                        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrsectreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drSect.Read() failed." + Environment.NewLine + errMsg);
#endif
                                      }
                                    }
                                    else
                                    {
                                      throw new Exception(
#if MULTILANGUAGE
                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSectionFkRefnoInTlocationmapsection)
#else
                                          "Unable to locate the section fk refno in tLocationMapSection."
#endif
                                          + Environment.NewLine + errMsg);
                                    }

                                    break;

                                  case (int)SysCommon.LocMapSubTypes.Slot:
                                    errMsg += Environment.NewLine + "Map sub type \'Slot\'";

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
                                        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrslotreadFailed) + Environment.NewLine + errMsg);
#else
                                        throw new Exception("drSlot.Read() failed." + Environment.NewLine + errMsg);
#endif
                                      }
                                    }
                                    else
                                    {
                                      throw new Exception(
#if MULTILANGUAGE
                                          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheSlotFkRefnoInTlocationmapslot)
#else
                                          "Unable to locate the slot fk refno in tLocationMapSlot."
#endif
                                          + Environment.NewLine + errMsg);
                                    }

                                    break;

                                  default:
                                    throw new Exception(
#if MULTILANGUAGE
                                        string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UndefinedLocationMapSubtype), locMapSubType));
#else
                                        string.Format("Undefined Location Map SubType {0}", locMapSubType));
#endif
                                }

                                isLocMapTypeFound = false;
                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                  cmndLocMapType.CommandText = "tLocationMapType";
                                  cmndLocMapType.CommandType = CommandType.TableDirect;
                                  cmndLocMapType.IndexName =
                                      "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";

                                  if (drLocMapType.Seek(
                                      DbSeekOptions.FirstEqual, 
                                      new object[] { locMapRefno, locMapTypeEnum }))
                                  {
                                    isLocMapTypeFound = true;
                                  }
                                }

                                if (!isLocMapTypeFound)
                                {
                                  cmndLocMapType.CommandText = "tLocationMapType";
                                  cmndLocMapType.CommandType = CommandType.TableDirect;
                                  cmndLocMapType.IndexName = 
                                      "PK_LocationMapTypeOnLocationMapRefnoAndLocationMapTypeEnum";
                                  rec.SetInt32((int)SysCommon.LocMapTypeFields.LocMapRefno, locMapRefno);
                                  rec.SetInt32((int)SysCommon.LocMapTypeFields.EnumType, locMapTypeEnum);

                                  rs.Insert(rec);
                                }

                                progValue++;
                                if (progValue % 100 == 0)
                                {
                                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
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

                drLocMapType.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingLocationMapTypesComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingLocationMapTypesFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving location map types failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingLocationMapTypesFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving location map types failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int whsLocMapRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      int locMapWhsFKRefno = 0;
      TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();

      // lookup and verify warehouse
      if (0 < whsLocMapRefno)
      {
        locMapWhsFKRefno = locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
      }

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST LOCATION MAP TYPES</action>"
          + SysCommon.GetRequestDefaults(true)
          + ("<LocationMapWarehouseFKRefno>" + locMapWhsFKRefno + "</LocationMapWarehouseFKRefno>") 
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
