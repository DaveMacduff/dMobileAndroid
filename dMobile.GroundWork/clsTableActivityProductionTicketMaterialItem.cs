namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public class TableActivityProductionTicketMaterialItem : TableBase
  {
    #region Constructors

    public TableActivityProductionTicketMaterialItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityProductionTicketMaterialItem";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndActivity.CommandType = CommandType.Text;
            cmndActivity.CommandText = "SELECT tActivity.ActivityRefno"
                + " FROM tActivity"
                + " WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
            using (SQLiteDataReader drActivity = cmndActivity.ExecuteReader())
            {
              while (drActivity.Read())
              {
                actRefno = Convert.ToInt32(drActivity["ActivityRefno"]);
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

              drActivity.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingTicketMaterialItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding ticket material items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding ticket material items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingTicketMaterialItemsFailed), ex);
#else
        throw new Exception("Seeding ticket material items failed.", ex);
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
              + " WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
          cmndStatus.CommandType = CommandType.Text;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.TicketMaterialItems), rowsAffected),
#else
              string.Format("{0} ticket material items.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredMaterialTicketItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Material ticket items status failed.", ex);
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
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterialItems), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef "
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketMaterial + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProductionMaterial "
              + "WHERE tActivityItemProductionMaterial.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketMaterial + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemProduction "
              + "WHERE tActivityItemProduction.ActivityItemRefno IN "
              + "(SELECT tActivityItem.ActivityItemRefno "
              + "FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketMaterial + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem "
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.TicketMaterial;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterialItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingTicketMaterialItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging ticket material items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingTicketMaterialItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging ticket material items failed.", ex);
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
      int actProductionFKRefno = 0;

//      using (SQLiteCommand cmndActMaterial = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndActMaterial.CommandText = "tActivityProductionMaterial";
//        cmndActMaterial.CommandType = CommandType.TableDirect;
//        cmndActMaterial.IndexName = "PK_ActivityProductionMaterialOnActivityRefno";
//        using (SQLiteDataReader drActMaterial = cmndActMaterial.ExecuteReader())
//        {
//          if (drActMaterial.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
//          {
//            if (drActMaterial.Read())
//            {
//              actProductionFKRefno = drActMaterial.GetInt32((int)SysCommon.ActProdMaterialFields.FKRefno);
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DractmaterialreadFailed));
//#else
//              throw new Exception("drActMaterial.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//                string.Format(
//#if MULTILANGUAGE
//                Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheActivityProductionTicketMaterialRefnoIn)
//#else
//                "Unable to locate the activity production ticket material refno {0} in "
//#endif
//                + "tActivityProductionMaterial",
//                actRefno));
//          }
//        }
//      }

      return this.SendComm(this.Create(actProductionFKRefno));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reqData"></param>
    /// <remarks></remarks>
    protected override void Save(string reqData, int actRefno)
    {
//      SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
//      string errMsg = string.Empty;

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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterialItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemProduction"].Rows.Count);

//#if MULTILANGUAGE
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), false);
//#else
//            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", progMax), false);
//#endif

//            using (SQLiteCommand cmndItemProduction = conn.CreateCommand())
//            {
//              cmndItemProduction.CommandText = "tActivityItemProduction";
//              cmndItemProduction.CommandType = CommandType.TableDirect;
//              cmndItemProduction.IndexName = "PK_ActivityItemProductionOnActivityItemRefno";

//              using (SQLiteCommand cmndItem = conn.CreateCommand())
//              {
//                cmndItem.CommandText = "tActivityItem";
//                cmndItem.CommandType = CommandType.TableDirect;

//                using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

//                  using (SQLiteCommand cmndItemMaterial = conn.CreateCommand())
//                  {
//                    cmndItemMaterial.CommandText = "tActivityItemProductionMaterial";
//                    cmndItemMaterial.CommandType = CommandType.TableDirect;
//                    cmndItemMaterial.IndexName
//                        = "UQ_ActivityItemProductionMaterialOnActivityItemProductionMaterialFKRefno";

//                    using (SqlCeResultSet rsActItemMaterial
//                        = cmndItemMaterial.ExecuteResultSet(ResultSetOptions.Updatable))
//                    {
//                      SqlCeUpdatableRecord recActItemMaterial = rsActItemMaterial.CreateRecord();

//                      using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                      {
//                        cmndProdUnit.CommandText = "tProductUnit";
//                        cmndProdUnit.CommandType = CommandType.TableDirect;
//                        cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                        using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                        {
//                          using (SQLiteCommand cmndProduct = conn.CreateCommand())
//                          {
//                            cmndProduct.CommandText = "tProduct";
//                            cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";
//                            cmndProduct.CommandType = CommandType.TableDirect;

//                            using (SQLiteDataReader drProd = cmndProduct.ExecuteReader())
//                            {
//                              using (SQLiteCommand cmndUnit = conn.CreateCommand())
//                              {
//                                cmndUnit.CommandText = "tUnit";
//                                cmndUnit.IndexName = "UQ_UnitOnUnitCode";
//                                cmndUnit.CommandType = CommandType.TableDirect;

//                                using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
//                                {
//                                  using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                  {
//                                    cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                    cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                    cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                    using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                    {
//                                      using (SQLiteCommand cmndID = conn.CreateCommand())
//                                      {
//                                        cmndID.CommandText = "SELECT @@IDENTITY";
//                                        cmndID.CommandType = CommandType.Text;

//                                        using (SqlCeResultSet rsActItemProduction
//                                            = cmndItemProduction.ExecuteResultSet(ResultSetOptions.Updatable))
//                                        {
//                                          SqlCeUpdatableRecord recActItemProduction
//                                              = rsActItemProduction.CreateRecord();
//                                          int actItemRefno = 0;
//                                          int progValue = 0;
//                                          int prodUnitRefno = 0;
//                                          int locMapRefno = 0;
//                                          int actItemProductionFKRefno = 0;

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                          foreach (DataRow drow in ds.Tables["tActivityItemProduction"].Rows)
//                                          {
//                                            errMsg = "Arguments:";

//                                            recActItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.SubType,
//                                                (int)SysCommon.ActSubTypes.TicketMaterial);
//                                            recActItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.Status,
//                                                (int)SysCommon.ActStatus.NoChange);
//                                            recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

//                                            rsActItem.Insert(recActItem);

//                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            recActItemProduction.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ActItemRefno,
//                                                actItemRefno);
//                                            recActItemProduction.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ActivityItemSubType,
//                                                (int)SysCommon.ActSubTypes.TicketMaterial);

//                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drProd,
//                                                drUnit,
//                                                drProdUnit,
//                                                false);
//                                            recActItemProduction.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.ProdUnitRefno,
//                                                prodUnitRefno);

//                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
//                                                drow,
//                                                ref errMsg,
//                                                drLocMapWhs);
//                                            recActItemProduction.SetInt32(
//                                                (int)SysCommon.ActItemProdFields.LocMapRefno,
//                                                locMapRefno);

//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.Quantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionQuantity"]));
//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.BaseQuantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantity"]));
//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.WeightQuantity,
//                                                Convert.ToDecimal(drow["ActivityItemProductionWeightQuantity"]));
//                                            recActItemProduction.SetString(
//                                                (int)SysCommon.ActItemProdFields.Description,
//                                                drow["ActivityItemProductionDescription"].ToString());
//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.QuantityExpected,
//                                                Convert.ToDecimal(drow["ActivityItemProductionQuantityExpected"]));
//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.BaseQuantityExpected,
//                                                Convert.ToDecimal(drow["ActivityItemProductionBaseQuantityExpected"]));
//                                            recActItemProduction.SetDecimal(
//                                                (int)SysCommon.ActItemProdFields.WeightQuantityExpected,
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemProductionWeightQuantityExpected"]));

//                                            rsActItemProduction.Insert(recActItemProduction);

//                                            recActItemMaterial.SetInt32(
//                                                (int)SysCommon.ActItemProdMaterialFields.ActItemRefno,
//                                                actItemRefno);

//                                            actItemProductionFKRefno = Convert.ToInt32(
//                                                drow["ActivityItemProductionFKRefno"]);
//                                            errMsg += Environment.NewLine + "Item fk refno" + actItemProductionFKRefno;
//                                            recActItemMaterial.SetInt32(
//                                                (int)SysCommon.ActItemProdMaterialFields.FKRefno,
//                                                actItemProductionFKRefno);
//                                            recActItemMaterial.SetString(
//                                                (int)SysCommon.ActItemProdMaterialFields.StringFKRefno,
//                                                actItemProductionFKRefno.ToString());

//                                            rsActItemMaterial.Insert(recActItemMaterial);

//                                            progValue++;
//                                            if (progValue % 100 == 0)
//                                            {
//                                              this.DataAccessProgressOutHandler(
//                                                  this,
//                                                  EventArgs.Empty,
//                                                  progValue,
//                                                  0,
//                                                  progMax);
//                                            }
//                                          }

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
//                                        }
//                                      }

//                                      drLocMapWhs.Close();
//                                    }
//                                  }

//                                  drUnit.Close();
//                                }
//                              }

//                              drProd.Close();
//                            }
//                          }

//                          drProdUnit.Close();
//                        }
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterialItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingTicketMaterialItemsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving ticket material items failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingTicketMaterialItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving ticket material items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Create(int actProductionFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST PRODUCTION TICKET MATERIAL ITEMS</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actProductionFKRefno + "</refno>")
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
