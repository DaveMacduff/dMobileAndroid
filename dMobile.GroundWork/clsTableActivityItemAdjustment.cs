namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityAdjustmentItem : TableBase
  {
    #region Constructors

    public TableActivityAdjustmentItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityAdjustmentItem";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndActAdj = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActAdj.CommandType = CommandType.Text;
            cmndActAdj.CommandText = "SELECT tActivity.ActivityRefno "
                + "FROM tActivity "
                + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Adjustment;

            using (SqliteDataReader drActAdj = cmndActAdj.ExecuteReader())
            {
              while (drActAdj.Read())
              {
                actRefno = Convert.ToInt32(drActAdj["ActivityRefno"]);
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

              drActAdj.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingAdjustmentItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding adjustment items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding adjustment items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingAdjustmentItemsFailed), ex);
#else
        throw new Exception("Seeding adjustment items failed.", ex);
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
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this, 
              EventArgs.Empty, 
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.AdjustmentItems), rowsAffected), 
#else
              string.Format("{0} adjustment items.", rowsAffected), 
#endif
              true);
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredAdjustmentItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Adjustment items status failed.", ex);
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
#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingExistingAdjustmentItems), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging existing adjustment items.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef " 
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN " 
              + "(SELECT tActivityItem.ActivityItemRefno " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Adjustment + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemAdjustment " 
              + "WHERE tActivityItemAdjustment.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Adjustment;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingExistingAdjustmentItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging existing adjustment items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingAdjustmentItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging adjustment items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingAdjustmentItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging adjustment items failed.", ex);
#endif
      }
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
      int actAdjFKRefno = 0;

      using (SQLiteCommand cmndActAdj = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndActAdj.CommandText = "tActivityAdjustment";
        cmndActAdj.CommandType = CommandType.TableDirect;
        cmndActAdj.IndexName = "PK_ActivityAdjustmentOnActivityRefno";
        using (SqliteDataReader drActAdj = cmndActAdj.ExecuteReader())
        {
          if (drActAdj.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
          {
            if (drActAdj.Read())
            {
              actAdjFKRefno = Convert.ToInt32(drActAdj.GetValue((int)SysCommon.ActAdjFields.FKRefno));
            }
            else
            {
#if MULTILANGUAGE
              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DractadjreadFailed));
#else
              throw new Exception("drActAdj.Read() failed.");
#endif
            }
          }
          else
          {
            throw new Exception(
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheAdjustmentRefnoInTactivityadjustment), actRefno));
#else
                string.Format("Unable to locate the adjustment refno {0} in tActivityAdjustment", actRefno));
#endif
          }
        }
      }

      return this.SendComm(this.Create(actAdjFKRefno));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reqData"></param>
    /// <param name="actRefno"></param>
    protected override void Save(string reqData, int actRefno)
    {
      SqlCeConnection conn = DatabaseConnection.GetInstance().SQLConn;
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentItems), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment items.", false);
#endif
          if (ds.Tables.Count != 0)
          {
            int progMax = Convert.ToInt32(ds.Tables["tActivityItemAdjustment"].Rows.Count);

            this.DataAccessStatusOutHandler(
                this, 
                EventArgs.Empty, 
#if MULTILANGUAGE
                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
#else
                string.Format("Records {0}", progMax), 
#endif
                false);

            using (SQLiteCommand cmndAdjItem = conn.CreateCommand())
            {
              cmndAdjItem.CommandText = "tActivityItemAdjustment";
              cmndAdjItem.CommandType = CommandType.TableDirect;
              cmndAdjItem.IndexName = "UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno";

              using (SQLiteCommand cmndItem = conn.CreateCommand())
              {
                cmndItem.CommandText = "tActivityItem";
                cmndItem.CommandType = CommandType.TableDirect;

                using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                  SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

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
                              using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
                              {
                                cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
                                cmndLocMapWhs.CommandType = CommandType.TableDirect;
                                cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

                                using (SqliteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                {
                                  using (SQLiteCommand cmndID = conn.CreateCommand())
                                  {
                                    cmndID.CommandText = "SELECT @@IDENTITY";
                                    cmndID.CommandType = CommandType.Text;

                                    using (SqlCeResultSet rs = 
                                        cmndAdjItem.ExecuteResultSet(ResultSetOptions.Updatable))
                                    {
                                      SqlCeUpdatableRecord rec = rs.CreateRecord();
                                      int actItemRefno = 0;
                                      int progValue = 0;
                                      int actItemAdjFKRefno = 0;
                                      int prodUnitRefno = 0;
                                      int locMapRefno = 0;

                                      this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                                      foreach (DataRow drow in ds.Tables["tActivityItemAdjustment"].Rows)
                                      {
#if MULTILANGUAGE
                                        errMsg = Translator.GetInstance().Translate(TranslatorVocabulary.Arguments);
#else
                                        errMsg = "Arguments:";
#endif

                                        recItem.SetInt32(
                                            (int)SysCommon.ActItemFields.SubType, 
                                            (int)SysCommon.ActSubTypes.Adjustment);
                                        recItem.SetInt32(
                                            (int)SysCommon.ActItemFields.Status, 
                                            (int)SysCommon.ActStatus.NoChange);
                                        recItem.SetInt32(
                                            (int)SysCommon.ActItemFields.ActRefno, 
                                            actRefno);

                                        rsItem.Insert(recItem);

                                        actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

                                        prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
                                            drow, 
                                            ref errMsg, 
                                            drProd, 
                                            drUnit, 
                                            drProdUnit, 
                                            false);
                                        rec.SetInt32((int)SysCommon.ActItemAdjFields.ProdUnitRefno, prodUnitRefno);

                                        locMapRefno = this.GetWhsLocMapRefnoFromDataRow(drow, ref errMsg, drLocMapWhs);
                                        rec.SetInt32((int)SysCommon.ActItemAdjFields.WhsLocMapRefno, locMapRefno);

                                        rec.SetInt32(
                                              (int)SysCommon.ActItemAdjFields.ActItemRefno, 
                                              actItemRefno);

                                        rec.SetInt32(
                                            (int)SysCommon.ActItemAdjFields.ActItemSubType, 
                                            Convert.ToInt32(SysCommon.ActSubTypes.Adjustment));

                                        actItemAdjFKRefno = Convert.ToInt32(drow["ActivityItemAdjustmentFKRefno"]);
#if MULTILANGUAGE
                                        errMsg += Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.ItemFkRefno) + actItemAdjFKRefno;
#else
                                        errMsg += Environment.NewLine + "Item fk refno " + actItemAdjFKRefno;
#endif
                                        rec.SetInt32((int)SysCommon.ActItemAdjFields.FKRefno, actItemAdjFKRefno);
                                        rec.SetString(
                                            (int)SysCommon.ActItemAdjFields.StringFKRefno, 
                                            Convert.ToString(actItemAdjFKRefno));

                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemAdjFields.Quantity, 
                                            Convert.ToDecimal(drow["ActivityItemAdjustmentQuantity"]));
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemAdjFields.BaseQuantity, 
                                            Convert.ToDecimal(drow["ActivityItemAdjustmentBaseQuantity"]));
                                        rec.SetDecimal(
                                            (int)SysCommon.ActItemAdjFields.WeightQuantity, 
                                            Convert.ToDecimal(drow["ActivityItemAdjustmentWeightQuantity"]));
                                        rec.SetString(
                                            (int)SysCommon.ActItemAdjFields.Description, 
                                            drow["ActivityItemAdjustmentDescription"].ToString());

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

                                  drLocMapWhs.Close();
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
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentItemsComplete), false);
#else
          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving adjustment items complete.", false);
#endif
        }
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingAdjustmentItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "saving adjustment items failed.", true);
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
            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingAdjustmentItemsFailed) + Environment.NewLine + errMsg,
#else
            "Database exception occurred. Saving adjustment items failed." + Environment.NewLine + errMsg,
#endif
            ex);

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingAdjustmentItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Saving adjustment items failed.", ex);
#endif
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actAdjFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int actAdjFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>"
          + "<action>REQUEST ADJUSTMENT ITEMS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actAdjFKRefno.ToString() + "</refno>")
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
