namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using Microsoft.VisualBasic;

  public class TableActivityRecevingItem : TableBase
  {
    #region Constructors

    public TableActivityRecevingItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItem";
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

      //GC.Collect();
      //GC.WaitForPendingFinalizers();

      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items.", true);
#endif

        if (actRefno == 0)
        {
          using (SQLiteCommand cmndRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
          {
            cmndRecv.CommandType = CommandType.Text;
            cmndRecv.CommandText = "SELECT tActivity.ActivityRefno " 
                + "FROM tActivity " 
                + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.Receiving;

            using (SQLiteDataReader drRecv = cmndRecv.ExecuteReader())
            {
              while (drRecv.Read())
              {
                actRefno = Convert.ToInt32(drRecv["ActivityRefno"]);
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

              drRecv.Close();
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingItemsFailed), false);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", false);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSeedingReceivingItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Seeding receiving items failed.", ex);
#endif
      }
      catch (Exception ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding receiving items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.SeedingReceivingItemsFailed), ex);
#else
        throw new Exception("Seeding receiving items failed.", ex);
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
          cmndStatus.CommandType = CommandType.Text;
          cmndStatus.CommandText = "SELECT COUNT(tActivityItem.ActivityItemRefno) " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Receiving;

          rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

          this.DataAccessStatusOutHandler(
              this,
              EventArgs.Empty,
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.ReceivingItems), rowsAffected),
#else
              string.Format("{0} receiving items.", rowsAffected),
#endif
              true);
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredReceivingItemsStatusFailed), ex);
#else
        throw new Exception("Database exception occurred. Receiving items status failed.", ex);
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef " 
              + "WHERE tActivityItemBarCodeAssociationXRef.ActivityItemRefno IN " 
              + "(SELECT tActivityItem.ActivityItemRefno " 
              + "FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Receiving + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItemReceiving " 
              + "WHERE tActivityItemReceiving.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Receiving;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.Receiving;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingReceivingItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging receiving items failed.", ex);
#endif
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    protected override string Send(int actRefno)
    {
      int recvFKRefno = 0;

//      using (SQLiteCommand cmndRecv = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//      {
//        cmndRecv.CommandText = "tActivityReceiving";
//        cmndRecv.CommandType = CommandType.TableDirect;
//        //cmndRecv.IndexName = "PK_ActivityReceivingOnActivityRefno";

//        using (SQLiteDataReader drRecv = cmndRecv.ExecuteReader())
//        {
//          if (drRecv.Seek(DbSeekOptions.FirstEqual, new object[] { actRefno }))
//          {
//            if (drRecv.Read())
//            {
//              recvFKRefno = drRecv.GetInt32((int)SysCommon.ActReceivingFields.FKRefno);
//            }
//            else
//            {
//#if MULTILANGUAGE
//              throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrrecvreadFailed));
//#else
//              throw new Exception("drRecv.Read() failed.");
//#endif
//            }
//          }
//          else
//          {
//            throw new Exception(
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheReceivingActivityRefnoInTactivityreceiving), actRefno));
//#else
//                string.Format("Unable to locate the receiving activity refno {0} in tActivityReceiving", actRefno));
//#endif
//          }
//        }
//      }
      
      return this.SendComm(this.Create(recvFKRefno));
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
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingItems), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items.", false);
//#endif
//          if (ds.Tables.Count != 0)
//          {
//            int progMax = Convert.ToInt32(ds.Tables["tActivityItemPurchaseOrder"].Rows.Count);

//            this.DataAccessStatusOutHandler(
//                this, 
//                EventArgs.Empty, 
//#if MULTILANGUAGE
//                string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.Recordsz), progMax), 
//#else
//                string.Format("Records {0}", progMax), 
//#endif
//                false);

//            using (SQLiteCommand cmndRecvItem = conn.CreateCommand())
//            {
//              cmndRecvItem.CommandText = "tActivityItemReceiving";
//              cmndRecvItem.CommandType = CommandType.TableDirect;
//              //cmndRecvItem.IndexName = "UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno";

//              using (SQLiteCommand cmndRecvItemComment = conn.CreateCommand())
//              {
//                cmndRecvItemComment.CommandText = "tActivityItemReceivingComment";
//                cmndRecvItemComment.CommandType = CommandType.TableDirect;
//                //cmndRecvItemComment.IndexName = "PK_ActivityItemReceivingCommentOnActivityItemRefno";

//                using (SQLiteCommand cmndItem = conn.CreateCommand())
//                {
//                  cmndItem.CommandText = "tActivityItem";
//                  cmndItem.CommandType = CommandType.TableDirect;

//                  using (SqlCeResultSet rsItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recItem = rsItem.CreateRecord();

//                    using (SQLiteCommand cmndProdUnit = conn.CreateCommand())
//                    {
//                      cmndProdUnit.CommandText = "tProductUnit";
//                      cmndProdUnit.CommandType = CommandType.TableDirect;
//                      //cmndProdUnit.IndexName = "UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC";

//                      using (SQLiteDataReader drProdUnit = cmndProdUnit.ExecuteReader())
//                      {
//                        using (SQLiteCommand cmndProd = conn.CreateCommand())
//                        {
//                          cmndProd.CommandText = "tProduct";
//                          //cmndProd.IndexName = "UQ_ProductOnProductFKRefno";
//                          cmndProd.CommandType = CommandType.TableDirect;

//                          using (SQLiteDataReader drProd = cmndProd.ExecuteReader())
//                          {
//                            using (SQLiteCommand cmndUnit = conn.CreateCommand())
//                            {
//                              cmndUnit.CommandText = "tUnit";
//                              //cmndUnit.IndexName = "UQ_UnitOnUnitCode";
//                              cmndUnit.CommandType = CommandType.TableDirect;

//                              using (SQLiteDataReader drUnit = cmndUnit.ExecuteReader())
//                              {
//                                using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
//                                {
//                                  cmndLocMapWhs.CommandText = "tLocationMapWarehouse";
//                                  cmndLocMapWhs.CommandType = CommandType.TableDirect;
//                                  //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";

//                                  using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
//                                  {
//                                    using (SQLiteCommand cmndID = conn.CreateCommand())
//                                    {
//                                      cmndID.CommandText = "SELECT @@IDENTITY";
//                                      cmndID.CommandType = CommandType.Text;

//                                      using (SqlCeResultSet rsRecvItemComment 
//                                          = cmndRecvItemComment.ExecuteResultSet(ResultSetOptions.Updatable))
//                                      {
//                                        SqlCeUpdatableRecord recRecvItemComment = rsRecvItemComment.CreateRecord();

//                                        using (SqlCeResultSet rs = cmndRecvItem.ExecuteResultSet(
//                                            ResultSetOptions.Updatable))
//                                        {
//                                          SqlCeUpdatableRecord rec = rs.CreateRecord();
//                                          int actItemRefno = 0;
//                                          int progValue = 0;
//                                          int prodUnitRefno = 0;
//                                          int locMapRefno = 0;
//                                          string recvItemComment = string.Empty;
//                                          int recvItemFKRefno = 0;

//                                          this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

//                                          foreach (DataRow drow in ds.Tables["tActivityItemPurchaseOrder"].Rows)
//                                          {
//                                            errMsg = "Arguments:";

//                                            recItem.SetInt32(
//                                                (int)SysCommon.ActItemFields.SubType,
//                                                (int)SysCommon.ActSubTypes.Receiving);
//                                            recItem.SetInt32((int)SysCommon.ActItemFields.Status, 0);
//                                            recItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, actRefno);

//                                            rsItem.Insert(recItem);

//                                            actItemRefno = Convert.ToInt32(cmndID.ExecuteScalar());

//                                            prodUnitRefno = this.GetProductUnitRefnoFromDataRow(
//                                                drow, 
//                                                ref errMsg, 
//                                                drProd, 
//                                                drUnit, 
//                                                drProdUnit,
//                                                false);

//                                            locMapRefno = this.GetWhsLocMapRefnoFromDataRow(
//                                                drow, 
//                                                ref errMsg, 
//                                                drLocMapWhs);

//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemReceivingFields.ActItemRefno, 
//                                                actItemRefno);

//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemReceivingFields.ActItemSubType, 
//                                                (int)SysCommon.ActSubTypes.Receiving);

//                                            recvItemFKRefno = Convert.ToInt32(
//                                                drow["ActivityItemPurchaseOrderFKRefno"]);
//                                            errMsg += Environment.NewLine + "Item fk refno " + recvItemFKRefno;
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemReceivingFields.FKRefno,
//                                                recvItemFKRefno);
//                                            rec.SetString(
//                                                (int)SysCommon.ActItemReceivingFields.StringFKRefno,
//                                                Convert.ToString(recvItemFKRefno));
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemReceivingFields.ProdUnitRefno, 
//                                                prodUnitRefno);
//                                            rec.SetInt32(
//                                                (int)SysCommon.ActItemReceivingFields.WhsLocMapRefno, 
//                                                locMapRefno);
//                                            rec.SetString(
//                                                (int)SysCommon.ActItemReceivingFields.VendorProductUnitCode, 
//                                                drow["ActivityItemPurchaseOrderVendorProductUnitCode"].ToString());
//                                            rec.SetString(
//                                                (int)SysCommon.ActItemReceivingFields.Description, 
//                                                drow["ActivityItemPurchaseOrderDescription"].ToString());
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.QuantityOrdered, 
//                                                Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityOrdered"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.BaseQuantityOrdered, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderBaseQuantityOrdered"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.WeightQuantityOrdered, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWeightQuantityOrdered"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.QuantityReceived, 
//                                                Convert.ToDecimal(drow["ActivityItemPurchaseOrderQuantityReceived"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.BaseQuantityReceived, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderBaseQuantityReceived"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.WeightQuantityReceived, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWeightQuantityReceived"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.QuantityReceivedToDate, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderQuantityReceivedToDate"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.BaseQuantityReceivedToDate, 
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderBaseQuantityReceivedToDate"]));
//                                            rec.SetDecimal(
//                                                (int)SysCommon.ActItemReceivingFields.WeightQuantityReceivedToDate,
//                                                Convert.ToDecimal(
//                                                drow["ActivityItemPurchaseOrderWeightQuantityReceivedToDate"]));
//                                            rs.Insert(rec);

//                                            recvItemComment = drow["ActivityItemPurchaseOrderComment"].ToString();
//                                            if (!string.IsNullOrEmpty(recvItemComment))
//                                            {
//                                              recRecvItemComment.SetInt32(
//                                                  (int)SysCommon.ActItemReceivingCommentFields.ActItemRefno, 
//                                                  actItemRefno);
//                                              recRecvItemComment.SetString(
//                                                  (int)SysCommon.ActItemReceivingCommentFields.Comment, 
//                                                  recvItemComment);

//                                              rsRecvItemComment.Insert(recRecvItemComment);
//                                            }

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
//                                    }

//                                    drLocMapWhs.Close();
//                                  }
//                                }

//                                drUnit.Close();
//                              }
//                            }

//                            drProd.Close();
//                          }
//                        }

//                        drProdUnit.Close();
//                      }
//                    }
//                  }
//                }
//              }
//            }
//          }

//#if MULTILANGUAGE
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingItemsComplete), false);
//#else
//          this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items complete.", false);
//#endif
//        }
//      }
//      catch (SQLiteException ex)
//      {
//#if MULTILANGUAGE
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.SavingReceivingItemsFailed), false);
//#else
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
//        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving receiving items failed.", false);
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
//            Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredSavingReceivingItemsFailed) + Environment.NewLine + errMsg,
//#else
//            "Database exception occurred. Saving receiving items failed." + Environment.NewLine + errMsg,
//#endif
//            ex);
//      }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="recvFKRefno"></param>
    /// <returns></returns>
    protected override string Create(int recvFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      //XmlWriter xml = XmlWriter.Create(xmlString);
      //string deviceID = SysDeviceID.GetDeviceID();
      //string revision = DateAndTime.DateSerial(1900, 1, 1).ToString("yyyy\'-\'MM\'-\'dd\'T\'HH\':\'mm\':\'ss");
      //string loginID = SysEnv.ContactLog.LoginID.ToString();

      //XDocument request = XDocument.Parse(
      //    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
      //    + "<imclient>"
      //    + "<command>"
      //    + "<action>REQUEST PURCHASE ORDER ITEMS</action>"
      //    + ("<device_id>" + deviceID + "</device_id>") 
      //    + ("<login_id>" + loginID + "</login_id>") 
      //    + ("<revision_datetime>" + revision + "</revision_datetime>") 
      //    + ("<refno>" + recvFKRefno.ToString() + "</refno>") 
      //    + "</command>" 
      //    + "</imclient>");

      //request.Save(xml);
      //xml.Flush();
      //xml.Close();

      return xmlString.ToString();
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="errMsg"></param>
    /// <param name="drProduct"></param>
    /// <param name="drUnit"></param>
    /// <param name="drProdUnit"></param>
    /// <returns></returns>
    private int GetProductXRefUnitRefnoFromDataRow(
        DataRow drow,
        ref string errMsg,
        SQLiteDataReader drProduct,
        SQLiteDataReader drUnit,
        SQLiteDataReader drProdUnit)
    {
            //      int prodRefno = 0;
            //      int prodFKRefno = Convert.ToInt32(drow["ProductFKRefno"]);
            //      errMsg += Environment.NewLine + "Prod fk refno " + prodFKRefno;

            //      if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
            //      {
            //        if (drProduct.Read())
            //        {
            //          prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drProduct.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
            //#else
            //        throw new Exception("Unable to locate the product refno from the product fk refno in tProduct."
            //#endif
            //            + Environment.NewLine + errMsg);
            //      }

            //      int unitRefno = 0;
            //      string unitCode = drow["XRefUnitCode"].ToString();
            //      errMsg += Environment.NewLine + "Unit \'" + unitCode + "\'";

            //      if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
            //      {
            //        if (drUnit.Read())
            //        {
            //          unitRefno = drUnit.GetInt32((int)SysCommon.UnitFields.UnitRefno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //#if MULTILANGUAGE
            //        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheXrefUnitCodeInTunit)
            //#else
            //        throw new Exception("Unable to locate the unit refno from the xref unit code in  tUnit."
            //#endif
            //            + Environment.NewLine + errMsg);
            //      }

            int prodUnitRefno = 0;
            //      bool prodUnitIsBC = Convert.ToBoolean(drow["ProductUnitIsBC"]);
            //      errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

            //      if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, prodUnitIsBC }))
            //      {
            //        if (drProdUnit.Read())
            //        {
            //          prodUnitRefno = drProdUnit.GetInt32((int)SysCommon.ProdUnitFields.Refno);
            //        }
            //        else
            //        {
            //#if MULTILANGUAGE
            //          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodunitreadFailed) + Environment.NewLine + errMsg);
            //#else
            //          throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
            //#endif
            //        }
            //      }
            //      else
            //      {
            //        throw new Exception(
            //#if MULTILANGUAGE
            //          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductFkRefnoBcAndXrefUnitCodeInTproductunit)
            //#else
            //          "Unable to locate the product unit refno from the product fk refno, bc and xref unit code in tProductUnit."
            //#endif
            //          + Environment.NewLine + errMsg);
            //      }

            return prodUnitRefno;
        }

        #endregion
    }
}
