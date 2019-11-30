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

  public class PickSingleActItem : ActItem
  {
    #region Constructors

    /// <summary>
    ///
    /// </summary>
    /// <param name="_oActivity"></param>
    /// <remarks></remarks>
    public PickSingleActItem(Activity activity)
      : base(activity)
    {
      this.IsAllowAddItem = this.SysEnv.SysSettings.AllowAddProd2Pick;
      this.FullClassName = typeof(PickSingleActItem).FullName;
      this.IsAskCreateNewItem = !this.SysEnv.SysSettings.IsPickCreateNewItemNoAsk;
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override SysCommon.SLabelQuantity GetLabelQuantity(bool isRepackQuantityOnly)
    {
      SysCommon.SLabelQuantity labelQty = new SysCommon.SLabelQuantity();

      try
      {
        string sendReq = string.Empty;
        string reqData = string.Empty;
        StringBuilder xmlString = new StringBuilder();
        XmlWriter xml = XmlWriter.Create(xmlString);
        XDocument request = null;

        request = XDocument.Parse(
            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
            + "<imclient>"
            + "<command>"
            + "<action>REQUEST SHIPPING LABEL ITEM QUANTITY</action>"
            + "</command>"
            + SysCommon.GetRequestDefaults(false)
            + ("<itm_refno>" + this.FKRefno + "</itm_refno>")
            + "</imclient>");

        request.Save(xml);
        xml.Flush();
        xml.Close();

        sendReq = xmlString.ToString();

        reqData = SysCommon.SubmitComm(sendReq);
        if (reqData.Length > 0)
        {
          XmlDocument xmlResult = new XmlDocument();
          xmlResult.LoadXml(reqData);

          // Check if result is an error message
          if (reqData.Contains("<message>ERROR</message>"))
          {
            string errorDetail = string.Format(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.OrderRequestShippingLabelsQuantityFailed) + Environment.NewLine + "{1}",
#else
                "Order# {0} request shipping label(s) quantity failed." + Environment.NewLine + "{1}",
#endif
                this.FKRefno,
                xmlResult.DocumentElement["detail"].InnerText);
            if (errorDetail.Contains("MSGBOX-"))
            {
              throw new MessageOnlyException(errorDetail);
            }
            else
            {
              throw new Exception(errorDetail);
            }
          }
          else
          {
            if (!isRepackQuantityOnly)
            {
              labelQty.FullQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/FullCaseQuantity").FirstChild.Value);

              labelQty.DryQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/DryQuantity").FirstChild.Value);

              labelQty.FrozenQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/FrozenQuantity").FirstChild.Value);

              labelQty.LiveQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/LiveQuantity").FirstChild.Value);

              labelQty.FreshQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/FreshQuantity").FirstChild.Value);

              labelQty.BundleQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/BundleQuantity").FirstChild.Value);

              labelQty.PalletQuantity = int.Parse(
                  xmlResult.SelectSingleNode("/VFPData/PalletQuantity").FirstChild.Value);
            }

            labelQty.RepackQuantity = int.Parse(
                xmlResult.SelectSingleNode("/VFPData/RepackQuantity").FirstChild.Value);

            labelQty.RepackDryQuantity = int.Parse(
                xmlResult.SelectSingleNode("/VFPData/RepackDryQuantity").FirstChild.Value);

            labelQty.RepackFrozenQuantity = int.Parse(
                xmlResult.SelectSingleNode("/VFPData/RepackFrozenQuantity").FirstChild.Value);

            labelQty.RepackLiveQuantity = int.Parse(
                xmlResult.SelectSingleNode("/VFPData/RepackLiveQuantity").FirstChild.Value);

            labelQty.RepackFreshQuantity = int.Parse(
                xmlResult.SelectSingleNode("/VFPData/RepackFreshQuantity").FirstChild.Value);

            // Used to determine if there should be a repack quantity gathered.
            labelQty.IsRepack = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackCase").FirstChild.Value);
            labelQty.IsRepackLive = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackLive").FirstChild.Value);
            labelQty.IsRepackFrozen = bool.Parse(
                  xmlResult.SelectSingleNode("/VFPData/IsRepackFrozen").FirstChild.Value);
            labelQty.IsRepackFresh = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackFresh").FirstChild.Value);
            labelQty.IsRepackDry = bool.Parse(xmlResult.SelectSingleNode("/VFPData/IsRepackDry").FirstChild.Value);
          }
        }
        else
        {
          throw new Exception(
#if MULTILANGUAGE
              string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.OrderItemRequestShippingLabelsQuantityFailed), this.FKRefno));
#else
              string.Format("Order # {0} item request shipping label(s) quantity failed.", this.FKRefno));
#endif
        }
      }
      catch (Exception ex)
      {
// TODO: DisplayAlert
//        MessageBox.Show(
//#if MULTILANGUAGE
//            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.GetFullUnitLabelQuantityFailed) + Environment.NewLine + " {0}", ex.Message, null),
//#else
//            string.Format("Get full unit label quantity failed." + Environment.NewLine + " {0}", ex.Message, null),
//#endif
//            this.ActHdr.GetControlDescription(),
//            MessageBoxButtons.OK,
//            MessageBoxIcon.Exclamation,
//            MessageBoxDefaultButton.Button1);
      }

      return labelQty;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override bool IsCatchAsk()
    {
      bool isCatchAsk = base.IsCatchAsk();
      if (isCatchAsk)
      {
          // modules are for exclusions
          isCatchAsk = this.SysEnv.SysSettings.IsPickSingleCatchAsk;
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
        isCatchAskZero = !this.SysEnv.SysSettings.IsPickingSingleModuleNoCatchAskZeroEnabled;
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
          // TODO: Figure out why raising a MessageOnlyException is not being caught as such, but rather
          // as a System.Exception.
          ////throw new MessageOnlyException("Item is outside of expected weight.");
#if MULTILANGUAGE
          string msg = Translator.GetInstance().Translate(TranslatorVocabulary.ItemIsOutsideOfExpectedWeight);
#else
          string msg = "Item is outside of expected weight.";
#endif
// TODO: DisplayAlert
//          MessageBox.Show(
//              msg,
//#if MULTILANGUAGE
//              Translator.GetInstance().Translate(TranslatorVocabulary.WeightRangeCheck),
//#else
//              "Weight Range Check",
//#endif
//              MessageBoxButtons.OK,
//              MessageBoxIcon.Exclamation,
//              MessageBoxDefaultButton.Button2);
          ////throw new IsReadyToApplyException(msg, true, false);
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
              //}
              //else
              //{
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            //}

                            break;

            case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndStop:
              throw new IsReadyToApplyException(ex.Message, false, false);
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actDetailFKRefno"></param>
    /// <returns></returns>
    public override SActItemSchemeData GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
    {
      StringBuilder xmlString = new StringBuilder();
      XmlWriter xml = XmlWriter.Create(xmlString);
      SActItemSchemeData itemSchemeData = base.GetProdSchemeDataFromDetailFKRefno(actDetailFKRefno);

      XDocument request = XDocument.Parse(
          "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" 
          + "<imclient>" 
          + "<command>" 
          + "<action>REQUEST SALES ORDER ITEM FROM DETAIL</action>" 
          + SysCommon.GetRequestDefaults(false) 
          + ("<ser_refno>" + actDetailFKRefno + "</ser_refno>") 
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
          string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;
          if (errorDetail.Contains("MSGBOX-"))
          {
            throw new MessageOnlyException(errorDetail);
          }
          else
          {
            throw new Exception(errorDetail);
          }
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

          tempSchemeData.SingleTradeItemSerialNumber = xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetailsingletradeitemserialnumber").InnerText;
          tempSchemeData.LogisticUnitBatchLotNumber = xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitbatchlotnumber").InnerText;
          tempSchemeData.LogisticUnitSkidNumber = xmlDoc.SelectSingleNode(
              "/VFPData/seg/activitydetaillogisticunitskidcode").InnerText;
          tempSchemeData.VariantCode = xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantcode").InnerText;
          tempSchemeData.VariantDescription = xmlDoc.SelectSingleNode(
              "/VFPData/seg/productschemevariantdescription").InnerText;
          tempSchemeData.VariantNumber = int.Parse(
              xmlDoc.SelectSingleNode("/VFPData/seg/productschemevariantno").InnerText);
          
          if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText 
              == string.Empty))
          {
            tempSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitbestbeforesellbydate").InnerText);
          }
          
          if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText 
              == string.Empty))
          {
            tempSchemeData.LogisticUnitProductionDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitproductiondate").InnerText);
          }
          
          if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText 
              == string.Empty))
          {
            tempSchemeData.LogisticUnitExpiryUseByDate = DateTime.Parse(
                xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitexpiryusebydate").InnerText);
          }
          
          if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitduedate").InnerText == string.Empty))
          {
            tempSchemeData.LogisticUnitDueDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitduedate").InnerText);
          }
          
          if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText 
              == string.Empty))
          {
            tempSchemeData.LogisticUnitPackagingDate = DateTime.Parse(xmlDoc.SelectSingleNode(
                "/VFPData/seg/activitydetaillogisticunitpackagingdate").InnerText);
          }

          itemSchemeData.ProdSchemeData = tempSchemeData;
        }
      }

      return itemSchemeData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public override string GetComment(int idx)
    {
      string itemCmnt = base.GetComment(idx);
      //using (SQLiteCommand cmndItemPickCmnt = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      //{
      //  cmndItemPickCmnt.CommandType = CommandType.TableDirect;
      //  cmndItemPickCmnt.CommandText = "tActivityItemPickingComment";
      //  cmndItemPickCmnt.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";

      //  using (SQLiteDataReader drItemPickCmnt = cmndItemPickCmnt.ExecuteReader())
      //  {
      //    if (drItemPickCmnt.Seek(DbSeekOptions.FirstEqual, new object[] { idx }))
      //    {
      //      if (drItemPickCmnt.Read())
      //      {
      //        itemCmnt = drItemPickCmnt["ActivityItemPickingCommentComment"].ToString();
      //      }
      //    }
      //  }
      //}

      return itemCmnt;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actItemStringFKRefno"></param>
    /// <returns></returns>
    public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
    {
#if MULTILANGUAGE
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToParseSalesOrderItemz), actItemStringFKRefno);
#else
      return string.Format("Unable to parse sales order item # {0}", actItemStringFKRefno);
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
      return string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToFindSalesOrderItemz), actItemStringFKRefno);
#else
      return string.Format("Unable to find sales order item # {0}", actItemStringFKRefno);
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
    /// <remarks></remarks>
    public override void Insert(XDocument data, bool askToInsertAsNewItem)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="pbProgressBar"></param>
    /// <remarks></remarks>
    public override void Refresh(XDocument data)
    {
      if (this.SysEnv.SysSettings.IsDisconnectedMode)
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
      }
      else
      {
        this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

        TableActivityPickingItem tableActPickItem = new TableActivityPickingItem();
        TableActivityPickingDetail tableActPickDetail = new TableActivityPickingDetail();
        tableActPickDetail.Purge();
        tableActPickItem.Purge();

        this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

        tableActPickItem.RegisterDataAccessListenerProgressOut((PickSingleActItem)this);
        tableActPickItem.RequestData(ActHdr.Refno, data);
        tableActPickItem.UnregisterDataAccessListenerProgressOut((PickSingleActItem)this);
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
    /// <param name="isSubmitBatch"></param>
    /// <returns></returns>
    public override DataView GetDataView(bool isSubmitBatch)
    {
      StringBuilder sql = new StringBuilder();

      if (isSubmitBatch)
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
        sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
        sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItem.ActivityItemStatus,");
        sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityItem.ActivityItemStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
        sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
        sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItem.ActivityItemStatus,");
        sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
        sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
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
        sql.Append("tUnitBase.UnitCode AS BaseUnitCode,");
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
        sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL THEN \' \' ");
        sql.Append("ELSE \'*\' END AS IsActivityItemPickingComment,");
        sql.Append("tActivityItemPicking.ActivityItemPickingIsReviewed ");
        sql.Append("FROM tActivityItemPicking ");
        sql.Append("INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityItemPicking.ProductUnitRefno ");
        sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
        sql.Append("INNER JOIN tUnit AS tUnitBase ON tUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
        sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
        sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
        sql.Append("tActivityItemPicking.WarehouseLocationMapRefno ");
        sql.Append("LEFT JOIN tActivityItemPickingComment ON tActivityItemPickingComment.ActivityItemRefno = ");
        sql.Append("tActivityItemPicking.ActivityItemRefno ");
        sql.AppendFormat("WHERE tActivityItem.ActivityRefno = {0} ", this.ActHdr.Refno);
        sql.Append("ORDER BY tActivityItemPicking.ActivityItemRefno");
      }

      using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
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
    /// <remarks></remarks>
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
    /// <param name="idx"></param>
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
    /// <remarks></remarks>
    public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
    {
      int foundActItemRefno = 0;

      ////using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      ////{
      ////  StringBuilder sql = new StringBuilder();
      ////  sql.Append("SELECT tActivityItem.ActivityItemRefno,");
      ////  sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno,");
      ////  sql.Append("tActivityItemPicking.ProductUnitRefno,");
      ////  sql.Append("tLocationMapWarehouse.LocationMapRefno");
      ////  sql.Append(" FROM tActivityItemPicking");
      ////  sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityItemRefno ");
      ////  sql.Append("= tActivityItemPicking.ActivityItemRefno");
      ////  sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
      ////  sql.Append("= tActivityItemPicking.WarehouseLocationMapRefno");
      ////  sql.AppendFormat(" WHERE tActivityItem.ActivityRefno = {0}", ActHdr.Refno);
      ////  sql.AppendFormat(" AND tActivityItemPicking.WarehouseLocationMapRefno = {0}", LocMap.CurrLocMap.WhsLocMapRefno);
      ////  sql.AppendFormat(" AND tActivityItemPicking.ProductUnitRefno = {0}", prodUnitRefno);

      ////  cmndItem.CommandType = CommandType.Text;
      ////  cmndItem.CommandText = sql.ToString();

      ////  using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
      ////  {
      ////    if (drItem.Read())
      ////    {
      ////      this.SelectedLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(drItem["LocationMapRefno"]));
      ////      this.FKRefno = Convert.ToInt32(drItem["ActivityItemPickingFKRefno"]);
      ////      this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
      ////      this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
      ////      this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
      ////      foundActItemRefno = this.Refno;
      ////    }

      ////    drItem.Close();
      ////  }
      ////}

      foundActItemRefno = this.Refno;

      return foundActItemRefno;
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
          + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
          + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>" 
          + "<refno></refno>" + "</imclient>");

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
    /// <param name="idx"></param>
    /// <param name="quantity"></param>
    /// <param name="baseQuantity"></param>
    /// <param name="weightQuantity"></param>
    /// <returns></returns>
    public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
    {
      this.DataViewItems[idx].BeginEdit();
      this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] =
          (decimal)this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] + quantity;
      this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] =
          (decimal)this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] + baseQuantity;
      this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] =
          (decimal)this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] + weightQuantity;
      this.DataViewItems[idx].EndEdit();

      return idx;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="actItemRefno"></param>
    public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
    {
      using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
      {
        cmndItem.CommandText = "tActivityItemPicking";
        cmndItem.CommandType = CommandType.Text;
        //cmndItem.IndexName = string.Empty;
        cmndItem.CommandText = "UPDATE tActivityItemPicking "
            + "SET ActivityItemPickingQuantityShipped = " 
            + this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] + "," 
            + " ActivityItemPickingBaseQuantityShipped = "
            + this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] + "," +
            " ActivityItemPickingWeightQuantityShipped = "
            + this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] +
            " WHERE tActivityItemPicking.ActivityItemRefno = " + actItemRefno;
        cmndItem.ExecuteNonQuery();
      }
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
