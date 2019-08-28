namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableLocationMapWarehouse : TableBase
  {
    #region Constructors

    public TableLocationMapWarehouse()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableLocationMapWarehouse";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehouses), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses.", true);
#endif

        string reqData = string.Empty;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        reqData = this.Send();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouses failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingWarehousesFailed), ex);
#else
        throw new Exception("Seeding warehouses failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehouses), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tLocationMap WHERE LocationMapSubType = " 
              + SysCommon.LocMapSubTypes.Warehouse.ToString();
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehousesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingWarehousesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouses failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingWarehousesFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging warehouses failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="locMapWhsFKRefno"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public int GetLocMapWhsRefnoFromLocWhsFKRefno(int locMapWhsFKRefno)
    {
      int whsLocMapRefno = 0;

      try
      {
        using (SqlCeCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndGetWhs.CommandText = "tLocationMapWarehouse";
          cmndGetWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
          cmndGetWhs.CommandType = CommandType.TableDirect;

          using (SqlCeDataReader drWhs = cmndGetWhs.ExecuteReader())
          {
            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
            {
              if (drWhs.Read())
              {
                whsLocMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
              }
              else
              {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
#else
                throw new Exception("drWhs.Read() failed.");
#endif
              }
            }
            else
            {
              throw new Exception(
                string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateWarehouseRefnoUsingTheWarehouseFkRefnoInTlocationmapwarehouse),
#else
                  "Unable to locate warehouse refno using the warehouse fk refno {0} in tLocationMapWarehouse.",
#endif
                  locMapWhsFKRefno));
            }

            drWhs.Close();
          }
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetlocmapwhsrefnofromlocwhsfkrefnoFailed), ex);
#else
        throw new Exception("Database exception occurred. GetLocMapWhsRefnoFromLocWhsFKRefno failed.", ex);
#endif
      }

      return whsLocMapRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="locMapWhsRefno"></param>
    /// <returns></returns>
    public int GetLocMapWhsFKRefnoFromLocMapWhsRefno(int locMapWhsRefno)
    {
      int locMapWhsFKRefno = 0;

      try
      {
        using (SqlCeCommand cmndGetWhs = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndGetWhs.CommandText = "tLocationMapWarehouse";
          cmndGetWhs.IndexName = "PK_LocationMapWarehouseOnLocationMapRefno";
          cmndGetWhs.CommandType = CommandType.TableDirect;

          using (SqlCeDataReader drWhs = cmndGetWhs.ExecuteReader())
          {
            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsRefno }))
            {
              if (drWhs.Read())
              {
                locMapWhsFKRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.FKRefno);
              }
              else
              {
#if MULTILANGUAGE
                throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed));
#else
                throw new Exception("drWhs.Read() failed.");
#endif
              }
            }
            else
            {
              throw new Exception(
                string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateWarehouseFkRefnoUsingTheWarehouseRefnoInTlocationmapwarehouse),
#else
                  "Unable to locate warehouse fk refno using the warehouse refno {0} in tLocationMapWarehouse.",
#endif
                  locMapWhsRefno));
            }

            drWhs.Close();
          }
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredGetlocmapwhsfkrefnofromlocmapwhsrefnoFailed), ex);
#else
        throw new Exception("Database exception occurred. GetLocMapWhsFKRefnoFromLocMapWhsRefno failed.", ex);
#endif
      }

      return locMapWhsFKRefno;
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
        using (SqlCeCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapWarehouse";
          cmndStatus.CommandType = CommandType.Text;
      
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Warehouses), rowsAffected), 
#else
              string.Format("{0} warehouses.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredWarehousesStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Warehouses status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send()
    {
      return this.SendComm(this.Create());
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehouses), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouse"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax),
#else
                string.Format("Records {0}", progMax),
#endif
                false);

            using (SqlCeCommand cmndWhs = conn.CreateCommand())
            {
              cmndWhs.CommandText = "tLocationMapWarehouse";
              cmndWhs.CommandType = CommandType.TableDirect;
              cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

              using (SqlCeDataReader drWhs = cmndWhs.ExecuteReader())
              {
                using (SqlCeCommand cmndLocMap = conn.CreateCommand())
                {
                  cmndLocMap.CommandText = "tLocationMap";
                  cmndLocMap.CommandType = CommandType.TableDirect;

                  using (SqlCeResultSet rsLocMap = cmndLocMap.ExecuteResultSet(ResultSetOptions.Updatable))
                  {
                    SqlCeUpdatableRecord recLocMap = rsLocMap.CreateRecord();

                    using (SqlCeCommand cmndID = conn.CreateCommand())
                    {
                      cmndID.CommandText = "SELECT @@IDENTITY";
                      cmndID.CommandType = CommandType.Text;

                      using (SqlCeResultSet rs = cmndWhs.ExecuteResultSet(ResultSetOptions.Updatable))
                      {
                        SqlCeUpdatableRecord rec = rs.CreateRecord();
                        int locMapRefno = 0;
                        int progValue = 0;
                        bool isLocMapWhsFound = false;
                        int locMapWhsFKRefno = 0;
                        int locMapWhsCode = 0;
                        string locMapWhsName = string.Empty;
                        string locMapWhsBarCode = string.Empty;

                        this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                        foreach (DataRow drow in ds.Tables["tLocationMapWarehouse"].Rows)
                        {
                          locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);

                          errMsg = "Whs fk refno " + locMapWhsFKRefno;
                          locMapWhsCode = Convert.ToInt32(drow["LocationMapWarehouseCode"]);
                          errMsg += Environment.NewLine + "Whs code " + locMapWhsCode;
                          locMapWhsName = drow["LocationMapWarehouseName"].ToString();
                          errMsg += Environment.NewLine + "Whs name \'" + locMapWhsCode + "\'";
                          locMapWhsBarCode = drow["LocationMapWarehouseBarCode"].ToString();
                          errMsg += Environment.NewLine + "Whs bar code \'" + locMapWhsBarCode + "\'";

                          isLocMapWhsFound = false;
                          if (this.SysEnv.RefreshOpt.RefreshDataChangesOnly)
                          {
                            cmndWhs.CommandText = "tLocationMapWarehouse";
                            cmndWhs.CommandType = CommandType.TableDirect;
                            cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                            {
                              isLocMapWhsFound = true;
                              cmndWhs.CommandType = CommandType.Text;
                              cmndWhs.IndexName = string.Empty;
                              cmndWhs.CommandText = "UPDATE tLocationMapWarehouse " +
                                  "SET LocationMapWarehouseCode = " + locMapWhsCode.ToString() + ", " +
                                  "LocationMapWarehouseName = \'" + locMapWhsName + "\', " +
                                  "LocationMapWarehouseBarCode = \'" + locMapWhsBarCode + "\' " +
                                  "WHERE LocationMapWarehouseFKRefno = " + locMapWhsFKRefno.ToString();
                              cmndWhs.ExecuteScalar();
                            }
                          }

                          if (!isLocMapWhsFound)
                          {
                            cmndWhs.CommandText = "tLocationMapWarehouse";
                            cmndWhs.CommandType = CommandType.TableDirect;
                            cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                            recLocMap.SetInt32(
                              Convert.ToInt32(SysCommon.LocMapFields.SubType), 
                              Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));
                            rsLocMap.Insert(recLocMap);
                            locMapRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                            rec.SetInt32(Convert.ToInt32(SysCommon.LocMapWhsFields.LocMapRefno), locMapRefno);
                            rec.SetInt32(
                              Convert.ToInt32(SysCommon.LocMapWhsFields.LocMapSubType), 
                              Convert.ToInt32(SysCommon.LocMapSubTypes.Warehouse));
                            rec.SetInt32(Convert.ToInt32(SysCommon.LocMapWhsFields.Code), locMapWhsCode);
                            rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.Name), locMapWhsName);
                            rec.SetString(Convert.ToInt32(SysCommon.LocMapWhsFields.BarCode), locMapWhsBarCode);
                            rec.SetInt32(Convert.ToInt32(SysCommon.LocMapWhsFields.FKRefno), locMapWhsFKRefno);
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
                    }
                  }
                }

                drWhs.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehousesComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingWarehousesFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouses failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingWarehousesFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving warehouses failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST WAREHOUSES</action>"
          + SysCommon.GetRequestDefaults(true)
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
