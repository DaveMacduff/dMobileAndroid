namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityTransferItem : TableBase
  {
    #region Constructors

    public TableActivityTransferItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityTransferItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Request(int actRefno)
    {
      string reqData = string.Empty;

      GC.Collect();
      GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndTrans.CommandType = CommandType.Text;
            cmndTrans.CommandText = "SELECT tActivity.ActivityRefno" 
                + " FROM tActivity" 
                + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Transfers;

            using (SqliteDataReader drTrans = cmndTrans.ExecuteReader())
            {
              while (drTrans.Read())
              {
                actRefno = Convert.ToInt32(drTrans["ActivityRefno"]);
                reqData = this.Send(actRefno);
                if (0 < reqData.Length)
                {
                  if (reqData.Contains("<message>ERROR</message>"))
                  {
                    XmlDocument reqErr = new XmlDocument();
                    reqErr.LoadXml(reqData);
                    throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                  }

                  this.Save(reqData, actRefno);
                }
              }
              drTrans.Close();
            }
          }
        }
        else
        {
          reqData = this.Send(actRefno);
          if (0 < reqData.Length)
          {
            if (reqData.Contains("<message>ERROR</message>"))
            {
              XmlDocument reqErr = new XmlDocument();
              reqErr.LoadXml(reqData);
              throw new Exception(reqErr.DocumentElement["detail"].InnerText);
            }

            this.Save(reqData, actRefno);
          }
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTransferItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding transfer items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding transfer items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTransferItemsFailed), ex);
#else
        throw new Exception("Seeding transfer items failed.", ex);
#endif
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferItems), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef" 
              + " WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN" 
              + " (SELECT tActivityItem.ActivityItemRefno" 
              + " FROM tActivityItem" 
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Transfers + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemTransfer " 
              + "WHERE tActivityItemTransfer.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTransferItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging transfer items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTransferItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging transfer items failed.", ex);
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
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno)" 
              + " FROM tActivityItem" 
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Transfers;
          cmndStatus.CommandType = CommandType.Text;
          
          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TransferItems), rowsAffected), 
#else
              string.Format("{0} transfer items.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredTransferItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Transfer items status failed.", ex);
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
    protected override string Send(int actRefno)
    {
      int transFKRefno = 0;

      using (SQLiteCommand cmndTrans = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndTrans.CommandText = "tActivityTransfer";
        cmndTrans.CommandType = CommandType.TableDirect;
        cmndTrans.IndexName = "PK_ActivityTransferOnActivityRefno";
        using (SqliteDataReader drTrans = cmndTrans.ExecuteReader())
        {
          if (drTrans.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
          {
            if (drTrans.Read())
            {
              transFKRefno = drTrans.GetInt32((int)SysCommon.ActTransferFields.FKRefno);
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrtransreadFailed));
#else
              throw new Exception("drTrans.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
                string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheTransferRefnoInTactivitytransfer), 
#else
                "Unable to locate the transfer refno {0} in tActivityTransfer.", 
#endif
                actRefno));
          }
        }
      }

      return this.SendComm(this.Create(transFKRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData, int actRefno)
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferItems), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityItemTransfer"].Rows.Count);

#if MULTILANGUAGE
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
#else
            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
#endif
            
            using (SQLiteCommand cmndTransItem = conn.CreateCommand())
            {
              cmndTransItem.CommandText = "tActivityItemTransfer";
              cmndTransItem.CommandType = CommandType.TableDirect;
              cmndTransItem.IndexName = "UQ_ActivityItemTransferOnActivityItemTransferFKRefno";

              using (SQLiteCommand cmndActItem = conn.CreateCommand())
              {
                cmndActItem.CommandText = "tActivityItem";
                cmndActItem.CommandType = CommandType.TableDirect;

                using (SqlCeResultSet rsActItem = cmndActItem.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                  using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
                  {
                    cmndProdUnit.CommandText = "tProductUnit";
                    cmndProdUnit.CommandType = CommandType.TableDirect;
                    cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

                    using (SqliteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
                    {
                      using (SQLiteCommand cmndProd = conn.CreateCommand())
                      {
                        cmndProd.CommandText = "tProduct";
                        cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
                        cmndProd.CommandType = CommandType.TableDirect;

                        using (SqliteDataReader drProd = cmndProd.ExecuteReader())
                        {
                          using (SQLiteCommand cmndUnit = conn.CreateCommand())
                          {
                            cmndUnit.CommandText = "tUnit";
                            cmndUnit.IndexName = "UQ_UnitOnUnitCode";
                            cmndUnit.CommandType = CommandType.TableDirect;

                            using (SqliteDataReader drUnit = cmndUnit.ExecuteReader())
                            {
                              using (SQLiteCommand cmndWhs = conn.CreateCommand())
                              {
                                cmndWhs.CommandText = "tLocationMapWarehouse";
                                cmndWhs.CommandType = CommandType.TableDirect;
                                cmndWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                using (SqliteDataReader drWhs = cmndWhs.ExecuteReader())
                                {
                                  using (SQLiteCommand cmndID = conn.CreateCommand())
                                  {
                                    cmndID.CommandText = "SELECT @@IDENTITY";
                                    cmndID.CommandType = CommandType.Text;

                                    using (SqlCeResultSet rs = 
                                        cmndTransItem.ExecuteResultSet(ResultSetOptions.Updatable))
                                    {
                                      SqlCeUpdatableRecord rec = rs.CreateRecord();
                                      int actItemRefno = 0;
                                      int progValue = 0;
                                      int prodUnitRefno = 0;
                                      int locMapRefno = 0;
                                      int locMapWhsFKRefno = 0;
                                      int transItemFKRefno = 0;

                                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                      foreach (DataRow drow in ds.Tables["tActivityItemTransfer"].Rows)
                                      {
                                        errMsg = "Arguments:";

                                        recActItem.SetInt32(
                                            (int)SysCommon.ActItemFields.SubType, 
                                            (int)SysCommon.ActSubTypes.Transfers);
                                        recActItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
                                        recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

                                        rsActItem.Insert(recActItem);
                                        
                                        actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                        prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                            drow, 
                                            ref errMsg, 
                                            drProd, 
                                            drUnit, 
                                            drProdUnit,
                                            false);

                                        locMapRefno = 0;
                                        if (locMapRefno == 0)
                                        {
                                          locMapWhsFKRefno = 
                                              Convert.ToInt32(drow["TransferFromLocationMapWarehouseFKRefno"]);
                                          errMsg += Environment.NewLine + "whs fk refno " + locMapWhsFKRefno;
                                          if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                          {
                                            if (drWhs.Read())
                                            {
                                              locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
                                            }
                                            else
                                            {
                                              throw new Exception(
#if MULTILANGUAGE
                                                  Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine + errMsg);
#else
                                                  "drWhs.Read() failed." + Environment.NewLine + errMsg);
#endif
                                            }
                                          }
                                        }

                                        if (locMapRefno == 0)
                                        {
                                          throw new Exception(
#if MULTILANGUAGE
                                              Translator.GetInstance().Translate(TranslatorVocabulary.UnableToIndentifyTheLocationMapRefnoz)
#else
                                              "Unable to indentify the location map refno."
#endif
                                              + Environment.NewLine + errMsg);
                                        }

                                        rec.SetInt32((int)SysCommon.ActItemTransferFields.ActItemRefno, actItemRefno);
                                        rec.SetInt32(
                                            (int)SysCommon.ActItemTransferFields.ActItemSubType, 
                                            (int)SysCommon.ActSubTypes.Transfers);

                                        transItemFKRefno = Convert.ToInt32(drow["ActivityItemTransferFKRefno"]);
                                        errMsg += Environment.NewLine + "Item fk refno " + transItemFKRefno;
                                        rec.SetInt32(
                                            (int)SysCommon.ActItemTransferFields.FKRefno,
                                            Convert.ToInt32(drow["ActivityItemTransferFKRefno"]));
                                        rec.SetString(
                                            (int)SysCommon.ActItemTransferFields.StringFKRefno, 
                                            Convert.ToString(transItemFKRefno));

                                        rec.SetInt32(
                                            (int)SysCommon.ActItemTransferFields.ProductUnitRefno, 
                                            prodUnitRefno);
                                        rec.SetInt32(
                                            (int)SysCommon.ActItemTransferFields.TransferFromWhsLocMapRefno, 
                                            locMapRefno);
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemTransferFields.RequiredQuantity,
                                            Convert.ToDecimal(drow["ActivityItemTransferRequiredQuantity"]));
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemTransferFields.Quantity, 
                                            Convert.ToDecimal(drow["ActivityItemTransferQuantity"]));
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemTransferFields.BaseQuantity, 
                                            Convert.ToDecimal(drow["ActivityItemTransferBaseQuantity"]));
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemTransferFields.WeightQuantity, 
                                            Convert.ToDecimal(drow["ActivityItemTransferWeightQuantity"]));
                                        
                                        rs.Insert(rec);

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
                                  }

                                  drWhs.Close();
                                }
                              }

                              drUnit.Close();
                            }
                          }

                          drProd.Close();
                        }
                      }

                      drProdUnit.Close();
                    }
                  }
                }
              }
            }
          }

#if MULTILANGUAGE
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferItemsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTransferItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving transfer items failed.", false);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTransferItemsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving transfer items failed." + Environment.NewLine + errMsg,
#endif
            ex);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int transFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST TRANSFER ITEMS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + transFKRefno.ToString() + "</refno>")
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
