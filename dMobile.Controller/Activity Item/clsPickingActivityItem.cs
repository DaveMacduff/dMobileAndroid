namespace dMobile.Controller
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using dMobile.Bedrock;
  using dMobile.GroundWork;
  using Microsoft.VisualBasic;

  public class PickingActItem : ActItem
  {
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activity"></param>
    public PickingActItem(Activity activity)
      : base(activity)
    {
      this.FullClassName = typeof(PickingActItem).FullName;
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="locMap"></param>
    /// <param name="prodInfo"></param>
    /// <returns></returns>
    public override string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
    {
      string msg = base.GetItemNotFoundMessage(locMap, prodInfo);

            if (this.SysEnv.SysSettings.IsPickOverrideMsg1)
      {
        msg = string.Format(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.ProductScannnedCodeBcUnitIsNotFoundOnThisOrderForWarehouse), 
#else
            "Product scannned, code \'{0}\' bc \'{1}\' unit \'{2}\', is not found on this order for warehouse {3}.", 
#endif
            prodInfo.Code,
            prodInfo.UnitBC, 
            prodInfo.UnitCode, 
            locMap.CurrLocMap.WhsFKRefno);
      }

      return msg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsCatchAsk()
    {
      bool isCatchAsk = base.IsCatchAsk();
      if (isCatchAsk)
      {
          // modules are for exclusions
          isCatchAsk = this.SysEnv.SysSettings.IsPickCatchAsk;
      }

      return isCatchAsk;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool IsCatchAskZero()
    {
      bool isCatchAskZero = base.IsCatchAskZero();
      if (isCatchAskZero)
      {
        isCatchAskZero = !this.SysEnv.SysSettings.IsPickingNoCatchAskZeroEnabled;
      }

      return isCatchAskZero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override void IsReadyToApply(XDocument data)
    {
      base.IsReadyToApply(data);

      // Weight tolerance within percentage check.
      if (this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent != 0)
      {
        decimal weight = decimal.Parse(data.Root.Element("wt_qty").Value);
        decimal qty = decimal.Parse(data.Root.Element("alt_qty").Value);
        decimal per = this.SysEnv.SysSettings.ProdWeightUnitTolerancePercent / 100;
        decimal weightUpper = (this.ProdInfo.UnitWeight + (this.ProdInfo.UnitWeight * per)) * qty;
        decimal weightLower = (this.ProdInfo.UnitWeight - (this.ProdInfo.UnitWeight * per)) * qty;

        if (weight < weightLower || weight > weightUpper)
        {
          // clsPickingSingleAcivityitem is different, it only raises a messagebox as a warning, but continues.
          // Lekker changed it's mind again and only wanting a warning rather than stop.
          // TODO: Figure out why raising a MessageOnlyException is not being caught as such, but rather
          // as a System.Exception.
#if MULTILANGUAGE
          throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.ItemIsOutsideOfExpectedWeight));
#else
          throw new MessageOnlyException("Item is outside of expected weight.");
#endif
        }
      }

      // New items are identified with 0 < this.FKRefno, do not validate.
      if (this.SysEnv.SysSettings.PickQtyOverWarn
          != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning
          && 0 < this.FKRefno)
      {
        try
        {
          this.PickingModuleQuantityOverCheck(
              int.Parse(data.Root.Element("itm_refno").Value),
              decimal.Parse(data.Root.Element("alt_qty").Value));
        }
        catch (Exception ex)
        {
          switch (this.SysEnv.SysSettings.PickQtyOverWarn)
          {
            case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndContinue:
                            // TODO: DisplayAlert
                            //              if (MessageBox.Show(
                            //#if MULTILANGUAGE
                            //                  ex.Message + Environment.NewLine + Translator.GetInstance().Translate(TranslatorVocabulary.Continue),
                            //                  Translator.GetInstance().Translate(TranslatorVocabulary.QuantityOver),
                            //#else
                            //                  ex.Message + Environment.NewLine + "Continue?",
                            //                  "Quantity Over",
                            //#endif
                            //                  MessageBoxButtons.YesNo,
                            //                  MessageBoxIcon.Question,
                            //                  MessageBoxDefaultButton.Button2) == DialogResult.No)
                            //              {
                            //                throw new IsReadyToApplyException(ex.Message, true, false);
                            //              }
                            //              else
                            //              {
                            throw new IsReadyToApplyException(ex.Message, true, true);
                            //              }
                            //break;

            case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndStop:
              throw new IsReadyToApplyException(ex.Message, false, false);
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityDetailFKRefno"></param>
    /// <returns></returns>
    public override SActItemSchemeData GetProdSchemeDataFromDetailFKRefno(int activityDetailFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      SActItemSchemeData itemSchemeData = base.GetProdSchemeDataFromDetailFKRefno(activityDetailFKRefno);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST SALES ORDER ITEM FROM DETAIL</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<ser_refno>" + activityDetailFKRefno + "</ser_refno>") 
          + "</command>" 
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      string reqData = SysCommon.SubmitComm(xmlString.ToString());

      if (reqData.Length > 0)
      {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(reqData);
        if (reqData.Contains("<message>ERROR</message>"))
        {
          throw new Exception(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.RequestPickingItemRefnoFromDetailRefnoError) 
#else
              "Request picking item refno from detail refno error." 
#endif
              + Environment.NewLine + xmlDoc.DocumentElement["detail"].InnerText);
        }
        else
        {
          SProductSchemeData tempSchemeData = new SProductSchemeData();
          itemSchemeData.ActItemFKRefno = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/activityitempickingfkrefno").InnerText);
          itemSchemeData.ActDetailQuantity = decimal.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailquantity").InnerText);
          itemSchemeData.UnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/unitcode").InnerText;
          itemSchemeData.ActDetailBaseQuantity = decimal.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailbasequantity").InnerText);
          itemSchemeData.BaseUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/baseunitcode").InnerText;
          itemSchemeData.ActDetailWeightQuantity = decimal.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailweightquantity").InnerText);
          itemSchemeData.WeightUnitCode = xmlDoc.SelectSingleNode("/VFPData/seg/weightunitcode").InnerText;
          itemSchemeData.LocMapWhsFKRefno = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/locationmapwarehousefkrefno").InnerText);
          itemSchemeData.LocMapSectFKRefno = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/locationmapsectionfkrefno").InnerText);
          itemSchemeData.LocMapSlotFKRefno = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/locationmapslotfkrefno").InnerText);
          tempSchemeData.SingleTradeItemSerialNumber 
              = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
          tempSchemeData.LogisticUnitBatchLotNumber 
              = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
          tempSchemeData.LogisticUnitSkidNumber 
              = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
          tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
          tempSchemeData.VariantDescription 
              = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantdescription").InnerText;
          tempSchemeData.VariantNumber = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);

          if (!(xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
          }

          if (!(xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
          }

          if (!(xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
          }

          if (!(xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitDueDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
          }

          if (!(xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
          }

          itemSchemeData.ProdSchemeData = tempSchemeData;
          itemSchemeData.ActDetailExtra.Text1 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra1").InnerText;
          itemSchemeData.ActDetailExtra.Text2 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra2").InnerText;
          itemSchemeData.ActDetailExtra.Text3 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra3").InnerText;
          itemSchemeData.ActDetailExtra.Text4 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra4").InnerText;
          itemSchemeData.ActDetailExtra.Text5 = xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText;
        }
      }

      return itemSchemeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedIndex"></param>
    /// <returns></returns>
    public override string GetComment(int selectedIndex)
    {
      string comment = base.GetComment(selectedIndex);

      //using (SQLiteCommand cmndItemPickComment = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      //{
      //  cmndItemPickComment.CommandType = CommandType.TableDirect;
      //  cmndItemPickComment.CommandText = "tActivityItemPickingComment";
      //  cmndItemPickComment.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";

      //  using (SQLiteDataReader drItemPickingComment = cmndItemPickComment.ExecuteReader())
      //  {
      //    if (drItemPickingComment.Seek(DbSeekOptions.FirstEqual, new object[] { selectedIndex }))
      //    {
      //      if (drItemPickingComment.Read())
      //      {
      //        comment = drItemPickingComment["ActivityItemPickingCommentComment"].ToString();
      //      }
      //      else
      //      {
      //        throw new Exception("drItemPickingComment.Read() failed.");
      //      }
      //    }
      //  }
      //}

      return comment;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseSalesOrderItem), actItemStringFKRefno);
#else
      return string.Format("Unable to parse sales order item # {0}.", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindSalesOrderItem), actItemStringFKRefno);
#else
      return string.Format("Unable to find sales order item # {0}.", actItemStringFKRefno);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDataViewSortFKRefnoIdx()
    {
      return "ActivityItemPickingStringFKRefno";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="askToInsertAsNewItem"></param>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
    // TODO: SQL
      //int foundActItemRefno = 0;
      //DialogResult result = DialogResult.None;
      //int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

//      if (prodUnitRefno != 0)
//      {
//        foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

//        if (foundActItemRefno > 0 && askToInsertAsNewItem)
//        {
//          result = MessageBox.Show(
//#if MULTILANGUAGE
//              Translator.GetInstance().Translate(TranslatorVocabulary.ALineItemAlreadyExistsForThisLocationAndProduct) + Environment.NewLine +
//              Translator.GetInstance().Translate(TranslatorVocabulary.DoYouWishToCreateOne), 
//              Translator.GetInstance().Translate(TranslatorVocabulary.NewItem),
//#else
//              "A line item already exists for this location and product." + Environment.NewLine +
//              "Do you wish to create one?", 
//              "New Item",
//#endif
//              MessageBoxButtons.YesNoCancel, 
//              MessageBoxIcon.Question,
//              MessageBoxDefaultButton.Button3);
//        }
//        else
//        {
//          // If the selected product to insert was not found, force the dialog result to insert it.
//          result = DialogResult.Yes;
//        }

//        if (result == DialogResult.Yes)
//        {
//          using (SQLiteCommand cmndPickItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//          {
//            cmndPickItem.CommandText = "tActivityItemPicking";
//            cmndPickItem.CommandType = CommandType.TableDirect;
//            cmndPickItem.IndexName = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno";

//            using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//            {
//              cmndItem.CommandText = "tActivityItem";
//              cmndItem.CommandType = CommandType.TableDirect;

//              using (SQLiteCommand cmndID = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
//              {
//                cmndID.CommandText = "SELECT @@IDENTITY";
//                cmndID.CommandType = CommandType.Text;

//                using (SqlCeResultSet rs = cmndPickItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                {
//                  SqlCeUpdatableRecord rec = rs.CreateRecord();

//                  using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
//                  {
//                    SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

//                    recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Picking);
//                    recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
//                    recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);

//                    rsActItem.Insert(recActItem);
                    
//                    this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

//                    rec.SetInt32((int)SysCommon.ActItemPickingFields.ActItemRefno, this.Refno);
//                    rec.SetInt32(
//                        (int)SysCommon.ActItemPickingFields.ActItemSubType, 
//                        (int)SysCommon.ActSubTypes.Picking);
//                    rec.SetInt32((int)SysCommon.ActItemPickingFields.FKRefno, this.Refno * -1);
//                    rec.SetString((int)SysCommon.ActItemPickingFields.StringFKRefno, Convert.ToString(this.Refno * -1));
//                    rec.SetInt32((int)SysCommon.ActItemPickingFields.ProdUnitRefno, prodUnitRefno);
//                    rec.SetInt32(
//                        (int)SysCommon.ActItemPickingFields.WhsLocMapRefno, 
//                        this.LocMap.CurrLocMap.WhsLocMapRefno);
//                    rec.SetString(
//                      (int)SysCommon.ActItemPickingFields.Description, 
//                      this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc);

//                    rec.SetString((int)SysCommon.ActItemPickingFields.CustomerProductUnitCode, string.Empty);
//                    rec.SetBoolean((int)SysCommon.ActItemPickingFields.IsReviewed, false);

//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityOrdered, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityOrdered, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityOrdered, 0);

//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityShipped, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityShipped, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityShipped, 0);

//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityAvailable, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityAvailable, 0);
//                    rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityAvailable, 0);

//                    rs.Insert(rec);

//                    this.FKRefno = this.Refno * -1;

//                    this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
//                    this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

//                    if (this.DataViewItems != null)
//                    {
//                      DataRowView rowView = this.DataViewItems.AddNew();

//                      rowView["ActivityItemRefno"] = this.Refno;
//                      rowView["ActivityItemPickingFKRefno"] = this.FKRefno;
//                      rowView["ActivityItemPickingStringFKRefno"] = this.FKRefno.ToString();
//                      rowView["ProductUnitRefno"] = prodUnitRefno;
//                      rowView["ProductRefno"] = this.ProdInfo.Refno;
//                      rowView["ProductCode"] = this.ProdInfo.Code;
//                      rowView["ProductDescription"] = this.ProdInfo.Desc;
//                      rowView["UnitCode"] = this.ProdInfo.UnitCode;
//                      rowView["ProductUnitBC"] = this.ProdInfo.UnitBC;
//                      rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
//                      rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
//                      rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
//                      rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
//                      rowView["ActivityItemPickingCustomerProductUnitCode"] = string.Empty;
//                      rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
//                      rowView["IsActivityItemPickingComment"] = " ";

//                      rowView["ActivityItemPickingQuantityOrdered"] = 0;
//                      rowView["ActivityItemPickingBaseQuantityOrdered"] = 0;
//                      rowView["ActivityItemPickingWeightQuantityOrdered"] = 0;
                      
//                      rowView["ActivityItemPickingQuantityShipped"] = 0;
//                      rowView["ActivityItemPickingBaseQuantityShipped"] = 0;
//                      rowView["ActivityItemPickingWeightQuantityShipped"] = 0;

//                      rowView["ActivityItemPickingQuantityAvailable"] = 0;
//                      rowView["ActivityItemPickingBaseQuantityAvailable"] = 0;
//                      rowView["ActivityItemPickingWeightQuantityAvailable"] = 0;

//                      rowView.EndEdit();
//                    }
//                  }
//                }
//              }
//            }
//          }
//        }
//      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public override void Refresh(XDocument data)
    {
      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityPickingItem tableActPickingItem = new TableActivityPickingItem();
        TableActivityPickingDetail tableActPickingDetail = new TableActivityPickingDetail();
        tableActPickingDetail.Purge();
        tableActPickingItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        tableActPickingItem.RegisterDataAccessListenerProgressOut((PickingActItem)this);
        tableActPickingItem.RequestData(this.ActHdr.Refno, data);
        tableActPickingItem.UnregisterDataAccessListenerProgressOut((PickingActItem)this);
      }
    }

//    /// <summary>
//    ///
//    /// </summary>
//    /// <param name="dataGridMain"></param>
//    /// <remarks></remarks>
//    public override void DesignDataGrid(DataGrid dataGridMain)
//    {
//      int columnWidthFactor = 1;

//      if (this.SysEnv.IsWindowsMobile6Device)
//      {
//        columnWidthFactor = 2;
//      }

//      if (dataGridMain.TableStyles.Count > 0)
//      {
//        dataGridMain.TableStyles.Clear();
//      }

//      DataGridTableStyle dataGridTableStyle = new DataGridTableStyle();
//      dataGridTableStyle.MappingName = this.MainGridMappingName;

//      DataGridTextBoxColumn dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityItemRefno";
//      dataGridCol.HeaderText = string.Empty;
//      dataGridCol.Width = -1;
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "IsActivityItemPickingComment";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.C);
//#else
//      dataGridCol.HeaderText = "C";
//#endif
//      dataGridCol.Width = 12 * columnWidthFactor;
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ProductCode";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.ProductId);
//#else
//      dataGridCol.HeaderText = "Product ID";
//#endif
//      dataGridCol.Width = 65 * columnWidthFactor;
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ProductUnitBC";
//      dataGridCol.HeaderText = string.Empty;
//      dataGridCol.Width = 20 * columnWidthFactor;
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "UnitCode";
//      dataGridCol.HeaderText = string.Empty;
//      dataGridCol.Width = 22 * columnWidthFactor;
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityItemPickingQuantityOrdered";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Ordered);
//#else
//      dataGridCol.HeaderText = "Ordered";
//#endif
//      dataGridCol.Width = 52 * columnWidthFactor;
//      dataGridCol.Format = "0.000";
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      dataGridCol = new DataGridTextBoxColumn();
//      dataGridCol.MappingName = "ActivityItemPickingQuantityShipped";
//#if MULTILANGUAGE
//      dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Shippedzz);
//#else
//      dataGridCol.HeaderText = "Shipped";
//#endif
//      dataGridCol.Width = 52 * columnWidthFactor;
//      dataGridCol.Format = "0.000";
//      dataGridTableStyle.GridColumnStyles.Add(dataGridCol);

//      if (this.SysEnv.SysSettings.IsPickShowItemQtyAvail)
//      {
//        dataGridCol = new DataGridTextBoxColumn();
//        dataGridCol.MappingName = "ActivityItemPickingQuantityAvailable";
//#if MULTILANGUAGE
//        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Available);
//#else
//        dataGridCol.HeaderText = "Available";
//#endif
//        dataGridCol.Width = 52 * columnWidthFactor;
//        dataGridTableStyle.GridColumnStyles.Add(dataGridCol);
//      }

//      ////dataGridCol.MappingName = string.Empty
//      ////dataGridCol.HeaderText = "R"
//      ////dataGridCol.Width = 12 * colWidthFactor
//      ////dataGridStyle.GridColumnStyles.Add(dataGridCol)

//      dataGridMain.TableStyles.Add(dataGridTableStyle);
//      dataGridMain.RowHeadersVisible = false;
//      dataGridMain.ColumnHeadersVisible = true;
//    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
      {
        // Gather any newly added line items.
        // Also perform union into tActivityDetail to check of detail exists.
        sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProduct.ProductBrandDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable,");
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL "); 
        sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
        sql.Append("UNION ");
        sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProduct.ProductBrandDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable,");
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
        sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0}", (int)SysCommon.ActStatus.NoChange);
      }
      else
      {
        sql.Append("SELECT tActivityItemPicking.ActivityItemRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno,");
        sql.Append("tProductUnit.ProductUnitRefno,");
        sql.Append("tProduct.ProductRefno,");
        sql.Append("tProduct.ProductCode,");
        sql.Append("tProduct.ProductDescription,");
        sql.Append("tProduct.ProductBrandDescription,");
        sql.Append("tUnit.UnitCode,");
        sql.Append("tProductUnit.ProductUnitBC,");
        sql.Append("tProductUnit.ProductUnitSizeDescription,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped,");
        sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
        sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
        sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable,");
        sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable,");
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
        sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItem.ActivityItemStatus ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0}", this.ActHdr.Refno);
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(
          sql.ToString(), 
          DatabaseConnection.GetInstance().SQLConn))
      {
        using (DataSet ds = new DataSet())
        {
          da.Fill(ds);
          this.DataViewItems = ds.Tables[0].DefaultView;
          this.DataViewItems.Table.TableName = this.MainGridMappingName;
          this.DataViewItems.AllowNew = true;
          this.DataViewItems.AllowEdit = true;
          this.DataViewItems.AllowDelete = true;
          this.DataViewItems.Sort = "ActivityItemRefno";
        }
      }

      return this.DataViewItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public override DataView Search(string searchText)
    {
      if (string.IsNullOrEmpty(this.ActItemFilter))
      {
        this.DataViewItems.RowFilter = "ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }
      else
      {
        this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'" + searchText.Trim() + "%\'";
      }

      return this.DataViewItems;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void SelectItem(int idx)
    {
      base.SelectItem(idx);
      this.FKRefno = (int)this.DataViewItems[idx]["ActivityItemPickingFKRefno"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodUnitRefno"></param>
    /// <returns></returns>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundActItemRefno = 0;

      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        StringBuilder sql = new StringBuilder();
        sql.Append("SELECT tActivityItem.ActivityItemRefno,");
        sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
        sql.Append("tActivityItemPicking.ProductUnitRefno,");
        sql.Append("tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
        sql.Append("= tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
        sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat(
            "AND tActivityItemPicking.WarehouseLocationMapRefno = {0} ", 
            this.LocMap.CurrLocMap.WhsLocMapRefno);
        sql.AppendFormat("AND tActivityItemPicking.ProductUnitRefno = {0}", prodUnitRefno);

        cmndItem.CommandType = CommandType.Text;
        cmndItem.CommandText = sql.ToString();

        using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
        {
          if (drItem.Read())
          {
            ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
            this.FKRefno = (int)drItem["ActivityItemPickingFKRefno"];
            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno((int)drItem["ProductUnitRefno"]);
            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            this.Refno = (int)drItem["ActivityItemRefno"];
            foundActItemRefno = this.Refno;
          }

          drItem.Close();
        }
      }

      return foundActItemRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Delete()
    {
      if (!this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        string sendReq = this.CreateDeleteXML(XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
            + "<params>" 
            + "<skid_data>" + this.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_data>" 
            + "</params>"));

        string reqData = SysCommon.SubmitComm(sendReq);
        if (reqData.Length > 0)
        {
          if (reqData.Contains("<message>ERROR</message>"))
          {
            XmlDocument docError = new XmlDocument();
            docError.LoadXml(reqData);
            string errorDetail = docError.DocumentElement["detail"].InnerText;
            if (errorDetail.Contains("MSGBOX-"))
            {
              throw new MessageOnlyException(errorDetail);
            }
            else
            {
              throw new Exception(errorDetail);
            }
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateDeleteXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      XDocument request = null;
      if (data.Document.Root.Element("skid_data") == null)
      {
#if MULTILANGUAGE
        throw new NotImplementedException(Translator.GetInstance().Translate(TranslatorVocabulary.OnlySkidDeletionIsSupported));
#else
        throw new NotImplementedException("Only skid deletion is supported.");
#endif
      }
      else
      {
        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
            + "<imclient>" 
            + "<command>" 
            + "<action>DELETE SALES ORDER DETAIL SKID</action>" 
            + "</command>"
            + SysCommon.GetRequestDefaults(false)
            + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
            + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
            + "<refno>" + this.ActHdr.FKRefno + "</refno>"
            + "<itm_refno>" + this.FKRefno + "</itm_refno>" 
            + "<is_negate_qty>false</is_negate_qty>" 
            + "</imclient>");
      }

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override string CreateSubmitXML(XDocument data)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      string prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
      string barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
      bool prodUnitIsBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
      string unitCode = data.Root.Element("UnitCode").Value;
      string symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>SUBMIT PICKING ITEM</action>" 
          + "</command>" 
          + SysCommon.GetRequestDefaults(false) 
          + "<prod_id>" + prodCode + "</prod_id>" 
          + "<is_bc>" + prodUnitIsBC + "</is_bc>"
          + "<symbology>" + symbologyPrefix + "</symbology>"
          + "<gtin>" + barCodeCode + "</gtin>"
          + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
          + "<sect_refno></sect_refno>"
          + "<slot_refno></slot_refno>" 
          + "<refno></refno>"
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] = quantity;
      this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] = baseQuantity;
      this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] = weightQuantity;
      this.DataViewItems[idx].EndEdit();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="iRowIdx"></param>
    /// <param name="decQuantity"></param>
    /// <param name="decBaseQuantity"></param>
    /// <param name="decWeightQuantity"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] 
        = (decimal)this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] + quantity;
      this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] 
        = (decimal)this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] + baseQuantity;
      this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] 
        = (decimal)this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] + weightQuantity;
      this.DataViewItems[idx].EndEdit();

      return idx;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="activityItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int activityItemRefno)
    {
      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandType = CommandType.Text;
        //cmndItem.IndexName = string.Empty;
        cmndItem.CommandText = "UPDATE tActivityItemPicking " 
            + "SET ActivityItemPickingQuantityShipped = " 
            + this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] + "," 
            + " ActivityItemPickingBaseQuantityShipped = " 
            + this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] + "," 
            + " ActivityItemPickingWeightQuantityShipped = " 
            + this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"]
            + " WHERE tActivityItemPicking.ActivityItemRefno = " + activityItemRefno;
        cmndItem.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actFKRefno"></param>
    /// <returns></returns>
    public string CreateRequestTotalQuantityXML(int actFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
          + "<imclient>"
          + "<command>"
          + "<action>REQUEST PICKING TOTAL QUANTITY</action>"
          + SysCommon.GetRequestDefaults(false)
          + ("<refno>" + actFKRefno + "</refno>")
          + "</command>"
          + "</imclient>");

      request.Save(xml);
      xml.Flush();
      xml.Close();

      return xmlString.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetTotalQuantity()
    {
      string msg = string.Empty;

      try
      {
        using (SQLiteCommand cmndItemPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndItemPicking.CommandType = CommandType.Text;
          //cmndItemPicking.IndexName = string.Empty;
          cmndItemPicking.CommandText = 
              "SELECT SUM(tActivityItemPicking.ActivityItemPickingQuantityOrdered) AS Ordered,"
              + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered) AS WeightOrdered,"
              + "SUM(tActivityItemPicking.ActivityItemPickingQuantityShipped) AS Shipped,"
              + "SUM(tActivityItemPicking.ActivityItemPickingWeightQuantityShipped) AS WeightShipped,"
              + "tUnit.UnitCode AS WtUnitCode "
              + "FROM tActivityItem "
              + "INNER JOIN tActivityItemPicking ON tActivityItemPicking.ActivityItemRefno = "
              + "tActivityItem.ActivityItemRefno "
              + "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno "
              + "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.WtUnitRefno "
              + string.Format("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno)
              + "GROUP BY tUnit.UnitCode";

          using (SQLiteDataReader dr = cmndItemPicking.ExecuteReader())
          {
            while (dr.Read())
            {
              // Because an agregate sum function is used in a query. There will always be a result record.
              // This causes problems detecting if there is nothing to sum, resulting in null.
              // Converting Convert.ToDecimal(dr["Ordered"]) fails because there is not data. However to a string works.
              msg = msg + string.Format(
#if MULTILANGUAGE
                  Translator.GetInstance().Translate(TranslatorVocabulary.TotalOrderedrRrtotalShippedr),
#else
                  "Total ordered:\r{0} @ {1}{2}\r\rTotal shipped:\r{3} @ {4}{5}",
#endif
                  (int)(decimal)dr["Ordered"],
                  (decimal)dr["WeightOrdered"],
                  (string)dr["WtUnitCode"],
                  (int)(decimal)dr["Shipped"],
                  (decimal)dr["WeightShipped"],
                  (string)dr["WtUnitCode"]);
            }
          }
        }
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), ex);
#else
        throw new Exception("Database exception occurred.", ex);
#endif
      }

      return msg;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemPickFKRefno"></param>
    /// <param name="actItemPickQuantity"></param>
    private void PickingModuleQuantityOverCheck(int actItemPickFKRefno, decimal actItemPickQuantity)
    {
      ////if (0 < skidFKRefno)
      ////{
      ////  StringBuilder xmlString = new StringBuilder();
      ////  XmlWriter xml = XmlWriter.Create(xmlString);

      ////  // Sending the alt_qty is not used. Only the skid_no is used.
      ////  // Kept the alt_qty
      ////  XDocument request = XDocument.Parse(
      ////      "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
      ////      + "<imclient>"
      ////      + "<command>"
      ////      + "<action>REQUEST SALES ORDER ITEM QUANTITY OVER CHECK</action>"
      ////      + SysCommon.GetRequestDefaults(false)
      ////      + ("<itm_refno>" + actItemPickFKRefno + "</itm_refno>")
      ////      + ("<alt_qty>" + actItemPickQuantity + "</alt_qty>")
      ////      + "</command>"
      ////      + "</imclient>");

      ////  request.Save(xml);
      ////  xml.Flush();
      ////  xml.Close();

      ////  string reqData = SysCommon.SubmitComm(request.ToString());

      ////}

      if (this.CurrRowIdx >= 0 && this.CurrRowIdx < this.DataViewItems.Count)
      {
        decimal shipped = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityShipped"];
        decimal ordered = (decimal)this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityOrdered"];
        if (shipped + actItemPickQuantity > ordered)
        {
          throw new Exception(
              string.Format(
#if MULTILANGUAGE
              Translator.GetInstance().Translate(TranslatorVocabulary.TheResultingQuantityShippedWillBeGreaterThanTheQuantityOrdered),
#else
              "The resulting quantity shipped {0} {1} will be greater than the quantity ordered {2} {3}.",
#endif
              shipped + actItemPickQuantity,
              this.ProdInfo.UnitCode,
              ordered,
              this.ProdInfo.UnitCode));
        }
      }
    }

    #endregion
  }
}
