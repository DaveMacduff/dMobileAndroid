namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableLocationMapSection : TableBase
  {
    #region Constructors

    public TableLocationMapSection()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableLocationMapSection";
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    public override void Request(int whsLocMapRefno)
    {
//      try
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSections), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections.", true);
//#endif

//        string reqData = string.Empty;

//        GC.Collect();
//        GC.WaitForPendingFinalizers();

//        if (0 < whsLocMapRefno)
//        {
//          using (SQLiteCommand cmndWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//          {
//            cmndWhs.CommandText = "tLocationMapWarehouse";
//            cmndWhs.CommandType = CommandType.TableDirect;
//            cmndWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";

//            using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
//            {
//              if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { whsLocMapRefno }))
//              {
//                if (drWhs.Read())
//                {
//                  this.DataAccessStatusOutHandler(
//                      this,
//                      EventArgs.Empty,
//#if MULTILANGUAGE
//                      string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ForWhs), drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)),
//#else
//                      string.Format("For whs \'{0}\'.", drWhs.GetString((int)SysCommon.LocMapWhsFields.Name)),
//#endif
//                      true);
//                }
//                else
//                {
//#if MULTILANGUAGE
//                  throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
//#else
//                  throw new Exception("drWhs.Read() failed.");
//#endif
//                }
//              }
//              else
//              {
//                throw new Exception(
//                    string.Format(
//#if MULTILANGUAGE
//                    Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheWarehouseNameUsingTheWarehouseRefnoInTlocationmapwarehouse),
//#else
//                    "Unable to locate the warehouse name using the warehouse refno {0} in tLocationMapWarehouse.",
//#endif
//                    whsLocMapRefno));
//              }

//              drWhs.Close();
//            }
//          }
//        }

//        reqData = this.Send(whsLocMapRefno);
//        if (0 < reqData.Length)
//        {
//          if (reqData.Contains("<message>ERROR</message>"))
//          {
//            XmlDocument reqErr = new XmlDocument();
//            reqErr.LoadXml(reqData);
//            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
//          }

//          this.Save(reqData);
//        }

//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsComplete), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections complete.", true);
//#endif
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);
//#endif

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingSectionsFailed), ex);
//#else
//        throw new Exception("Database exception occurred. Seeding sections failed.", ex);
//#endif
//      }
//      catch (Exception ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), true);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding sections failed.", true);
//#endif

//#if MULTILANGUAGE
//        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSectionsFailed), ex);
//#else
//        throw new Exception("Seeding sections failed.", ex);
//#endif
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="whsLocMapRefno"></param>
    /// <remarks></remarks>
    public override void PurgeData(int whsLocMapRefno)
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSections), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;

          if (whsLocMapRefno == 0)
          {
            cmndPurge.CommandText = "DELETE FROM tLocationMapSection";
          }
          else
          {
            cmndPurge.CommandText = "DELETE FROM tLocationMapSection " 
                + "WHERE WarehouseLocationMapRefno = " + whsLocMapRefno.ToString();
          }
          
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSectionsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSectionsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sections failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSectionsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sections failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapSection";
          cmndStatus.CommandType = CommandType.Text;
          
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());
      
          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Sections), rowsAffected), 
#else
              string.Format("{0} sections.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSectionsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Sections status failed.", ex);
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
    protected override void Save(string reqData)
    {
//      string errMsg = string.Empty;
//      SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

//      try
//      {
//        using (DataSet ds = new DataSet())
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.ReadingReceivedData), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);
//#endif
//          ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSections), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSection"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndSect = conn.CreateCommand())
//            {
//              cmndSect.CommandText = "tLocationMapSection";
//              cmndSect.CommandType = CommandType.TableDirect;
//              cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

//              using (SQLiteDataReader drSect = cmndSect.ExecuteReader())
//              {
//                using (SQLiteCommand cmndLocMap = conn.CreateCommand())
//                {
//                  cmndLocMap.CommandText = "tLocationMap";
//                  cmndLocMap.CommandType = CommandType.TableDirect;

//                  using (SqlCeResultSet rsLocMap = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recLocMap = rsLocMap.CreateRecord();

//                    using (SQLiteCommand cmndID = conn.CreateCommand())
//                    {
//                      cmndID.CommandText = "SELECT @@IDENTITY";
//                      cmndID.CommandType = CommandType.Text;

//                      using (SQLiteCommand cmndWhs = conn.CreateCommand())
//                      {
//                        cmndWhs.CommandText = "tLocationMapWarehouse";
//                        cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
//                        cmndWhs.CommandType = CommandType.TableDirect;

//                        using (SQLiteDataReader drWhs = cmndWhs.ExecuteReader())
//                        {
//                          using (SqlCeResultSet rs = cmndSect.ExecuteResultSet(ResultSetOptions.Updatable))
//                          {
//                            SqlCeUpdatableRecord rec = rs.CreateRecord();
//                            int locMapRefno = 0;
//                            int whsLocMapRefno = 0;
//                            int progValue = 0;
//                            bool isSectFound = false;
//                            int sectFKRefno = 0;
//                            int sectCode = 0;
//                            string sectName = string.Empty;
//                            string sectBarCode = string.Empty;

//                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                            foreach (DataRow drowSection in ds.Tables["tLocationMapSection"].Rows)
//                            {
//                              sectFKRefno = Convert.ToInt32(drowSection["LocationMapSectionFKRefno"]);
//                              errMsg = "Section fk refno " + sectFKRefno;
//                              sectCode = Convert.ToInt32(drowSection["LocationMapSectionCode"]);
//                              errMsg += Environment.NewLine + "Section code " + sectCode;
//                              sectName = Convert.ToString(drowSection["LocationMapSectionName"]);
//                              errMsg += Environment.NewLine + "Section name \'" + sectName + "\'";
//                              sectBarCode = Convert.ToString(drowSection["LocationMapSectionBarCode"]);
//                              errMsg += Environment.NewLine + "Section bar code \'" + sectBarCode + "\'";

//                              whsLocMapRefno = this.GetWhsLocMapRefnoFromDataRow(drowSection, ref errMsg, drWhs);

//                              isSectFound = false;
//                              if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
//                              {
//                                cmndSect.CommandText = "tLocationMapSection";
//                                cmndSect.CommandType = CommandType.Text;
//                                cmndSect.IndexName = "UQ_LocationMapSectionOnLocationMapSectionFKRefno";

//                                if (drSect.Seek(DbSeekOptions.FirstEqual, new object[] { sectFKRefno }))
//                                {
//                                  isSectFound = true;
//                                  cmndSect.CommandType = CommandType.Text;
//                                  cmndSect.IndexName = string.Empty;
//                                  cmndSect.CommandText = "UPDATE tLocationMapSection "
//                                      + "SET WarehouseLocationMapRefno = " + whsLocMapRefno + ","
//                                      + "LocationMapSectionCode = " + sectCode + ","
//                                      + "LocationMapSectionName = \'" + sectName + "\'," 
//                                      + "LocationMapSectionBarCode = \'" + sectBarCode + "\' " 
//                                      + "WHERE LocationMapSectionFKRefno = " + sectFKRefno;

//                                  cmndSect.ExecuteNonQuery();
//                                }
//                              }

//                              if (!isSectFound)
//                              {
//                                cmndSect.CommandText = "tLocationMapSection";
//                                cmndSect.CommandType = CommandType.TableDirect;
//                                cmndSect.IndexName = string.Empty;
                                
//                                recLocMap.SetInt32(
//                                    (int)SysCommon.LocMapFields.SubType, 
//                                    (int)SysCommon.LocMapSubTypes.Section);
                                
//                                rsLocMap.Insert(recLocMap);

//                                locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                rec.SetInt32((int)SysCommon.LocMapSectFields.LocMapRefno, locMapRefno);
//                                rec.SetInt32(
//                                    (int)SysCommon.LocMapSectFields.LocMapSubType, 
//                                    (int)SysCommon.LocMapSubTypes.Section);
//                                rec.SetInt32((int)SysCommon.LocMapSectFields.WhsLocMapRefno, whsLocMapRefno);
//                                rec.SetInt32((int)SysCommon.LocMapSectFields.Code, sectCode);
//                                rec.SetString((int)SysCommon.LocMapSectFields.Name, sectName);
//                                rec.SetInt32((int)SysCommon.LocMapSectFields.FKRefno, sectFKRefno);
//                                rec.SetString((int)SysCommon.LocMapSectFields.BarCode, sectBarCode);

//                                rs.Insert(rec);
//                              }

//                              progValue++;
//                              if (progValue % 100 == 0)
//                              {
//                                this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
//                              }
//                            }

//                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                          }

//                          drWhs.Close();
//                        }
//                      }
//                    }
//                  }
//                }

//                drSect.Close();
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSectionsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSectionsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving sections failed.", false);
//#endif

//        // Duplicate key violations
//        if (ex.NativeError == 25016)
//        {
//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation), false);
//          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UniquenessViolation) + Environment.NewLine + errMsg, ex);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
//          throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
//#endif
//        }

//        throw new Exception(
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSectionsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving sections failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
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
      TableLocationMapWarehouse locMapWhs = new TableLocationMapWarehouse();
      int locMapWhsFKRefno = 0;

      // lookup and verify warehouse
      if (0 < whsLocMapRefno)
      {
        locMapWhsFKRefno = locMapWhs.GetLocMapWhsFKRefnoFromLocMapWhsRefno(whsLocMapRefno);
      }

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST SECTIONS</action>"
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
