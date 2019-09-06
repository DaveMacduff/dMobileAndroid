namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableLocationMapSlotCoordinate : TableBase
  {
    #region Constructors

    public TableLocationMapSlotCoordinate()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableLocationMapSlotCoordinate";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotCoordinates), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates.", true);
#endif

        string reqData = string.Empty;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        reqData = this.Send("slot coordinates");
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotCoordinatesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates complete.", true);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotCoordinatesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding slot coordinates failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingSlotCoordinatesFailed), ex);
#else
        throw new Exception("Seeding slot coordinates failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlotCoordinates), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.CommandText = "DELETE FROM tLocationMapSlotCoordinate";
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlotCoordinatesComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSlotCoordinatesFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging slot coordinates failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSlotCoordinatesFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging slot coordinates failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapSlotCoordinate";
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.SlotCoordinates), rowsAffected), 
#else
              string.Format("{0} slot coordinates.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSlotCoordinatesStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Slot coordinates status failed.", ex);
#endif
      }

      return rowsAffected;
    }

    #endregion

    #region Private Members

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData)
    {
      string errMsg = string.Empty;
      
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotCoordinates), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tLocationMapSlotCoordinate"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
#else
                string.Format("Records {0}", progMax), 
#endif
                false);

            using (SQLiteCommand cmndSlotCoord = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
              cmndSlotCoord.CommandText = "tLocationMapSlotCoordinate";
              cmndSlotCoord.IndexName = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";
              cmndSlotCoord.CommandType = CommandType.TableDirect;

              using (SqliteDataReader drSlotCoord = cmndSlotCoord.ExecuteReader())
              {
                using (SqlCeResultSet rs = cmndSlotCoord.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord rec = rs.CreateRecord();
                  int progValue = 0;
                  bool isSlotCoordFound = false;
                  int slotCoordType = (int)SysCommon.LocMapSlotCoordTypes.Coord1;
                  string slotCoordCode = string.Empty;

                  this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                  foreach (DataRow drow in ds.Tables["tLocationMapSlotCoordinate"].Rows)
                  {
                    slotCoordCode = drow["LocationMapSlotCoordinateCode"].ToString();
                    errMsg = "Coordinate code \'" + slotCoordCode + "\'";

                    slotCoordType = Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]);
                    switch (slotCoordType)
                    {
                      case (int)SysCommon.LocMapSlotCoordTypes.Coord1:
                        errMsg += Environment.NewLine + "Type coordinate 1";
                        break;
                      
                      case (int)SysCommon.LocMapSlotCoordTypes.Coord2:
                        errMsg += Environment.NewLine + "Type coordinate 2";
                        break;
                      
                      case (int)SysCommon.LocMapSlotCoordTypes.Coord3:
                        errMsg += Environment.NewLine + "Type coordinate 3";
                        break;

                      default:
#if MULTILANGUAGE
                        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnidentifiedSlotCoordinateEnumType) + slotCoordType);
#else
                        throw new Exception("Unidentified slot coordinate enum type " + slotCoordType);
#endif
                    }

                    isSlotCoordFound = false;
                    if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                    {
                      cmndSlotCoord.CommandText = "tLocationMapSlotCoordinate";
                      cmndSlotCoord.CommandType = CommandType.TableDirect;
                      cmndSlotCoord.IndexName = 
                          "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";

                      if (drSlotCoord.Seek(DbSeekOptions.FirstEqual, new object[] { slotCoordType, slotCoordCode }))
                      {
                        isSlotCoordFound = true;
                      }
                    }

                    if (!isSlotCoordFound)
                    {
                      cmndSlotCoord.CommandText = "tLocationMapSlotCoordinate";
                      cmndSlotCoord.CommandType = CommandType.TableDirect;
                      cmndSlotCoord.IndexName = 
                          "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndLocationMapSlotCoordCode";

                      rec.SetInt32(
                          (int)SysCommon.LocMapSlotCoordFields.EnumType, 
                          Convert.ToInt32(drow["LocationMapSlotCoordinateEnum"]));
                      rec.SetString(
                          (int)SysCommon.LocMapSlotCoordFields.Code, 
                          drow["LocationMapSlotCoordinateCode"].ToString());

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

                drSlotCoord.Close();
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotCoordinatesComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingSlotCoordinatesFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving slot coordinates failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingSlotCoordinatesFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving slot coordinates failed." + Environment.NewLine + errMsg,
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
          + "<action>REQUEST SLOT COORDINATES</action>"
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
