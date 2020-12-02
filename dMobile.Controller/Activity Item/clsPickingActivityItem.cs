namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using Microsoft.VisualBasic;
    using Xamarin.Forms;

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
                // TODO: Resource strings
                msg = string.Format("Product scannned, code \'{0}\' bc \'{1}\' unit \'{2}\', is not found on this order for warehouse {3}.",
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
        public override async Task IsReadyToApply(XDocument data)
        {
            await base.IsReadyToApply(data);

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

                    // TODO: Resource strings
                    throw new MessageOnlyException("Item is outside of expected weight.");
                }
            }

            // New items are identified with 0 < this.FKRefno, do not validate.
            if (this.SysEnv.SysSettings.PickQtyOverWarn != SysCommon.ActPickingQtyOverValLevels.NoValidationOrWarning && 0 < this.FKRefno)
            {
                try
                {
                    this.PickingModuleQuantityOverCheck(int.Parse(data.Root.Element("itm_refno").Value), decimal.Parse(data.Root.Element("alt_qty").Value));
                }
                catch (Exception ex)
                {
                    switch (this.SysEnv.SysSettings.PickQtyOverWarn)
                    {
                        case SysCommon.ActPickingQtyOverValLevels.ValidateWarnAndContinue:
                            // TODO: Resource strings
                            if (await Application.Current.MainPage.DisplayAlert("Quantity Over", ex.Message + Environment.NewLine + "Continue?", "Yes", "No"))
                            {
                                throw new IsReadyToApplyException(ex.Message, true, true);
                            }
                            else
                            {
                                throw new IsReadyToApplyException(ex.Message, true, false);
                            }
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
                    // TODO: Resource strings
                    throw new Exception("Request picking item refno from detail refno error." + Environment.NewLine + xmlDoc.DocumentElement["detail"].InnerText);
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
                 
                    if (!(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText == string.Empty))
                    {
                        itemSchemeData.ActDetailExtra.DateTime5 = DateTime.Parse(xmlDoc.SelectSingleNode("/VFPData/seg/activitydetailextra5").InnerText);
                    }
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

            using (SQLiteCommand cmndItemPickComment = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndItemPickComment.CommandType = CommandType.Text;
                cmndItemPickComment.CommandText = "SELECT * FROM tActivityItemPickingComment WHERE ActivityItemRefno = @activityItemRefno";
                //cmndItemPickComment.IndexName = "PK_ActivityItemPickingCommentOnActivityItemRefno";
                cmndItemPickComment.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(selectedIndex)));

                using (SQLiteDataReader drItemPickingComment = cmndItemPickComment.ExecuteReader())
                {
                    //if (drItemPickingComment.Seek(DbSeekOptions.FirstEqual, new object[] { selectedIndex }))
                    //{
                    if (drItemPickingComment.Read())
                    {
                        comment = Convert.ToString(drItemPickingComment["ActivityItemPickingCommentComment"]);
                    }
                    //else
                    //{
                    //    throw new Exception("drItemPickingComment.Read() failed.");
                    //}
                    //}
                }
            }

            return comment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to parse sales order item # {0}.", actItemStringFKRefno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            // TODO: Resource strings
            return string.Format("Unable to find sales order item # {0}.", actItemStringFKRefno);
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
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int foundActItemRefno = 0;
            DialogResult result = DialogResult.None;
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                foundActItemRefno = this.GetRefnoFromProdUnitRefno(prodUnitRefno);

                if (foundActItemRefno > 0 && askToInsertAsNewItem)
                {
                    //result = MessageBox.Show(

                    //      "A line item already exists for this location and product." + Environment.NewLine +
                    //    "Do you wish to create one?",
                    //    "New Item",
                    //      MessageBoxButtons.YesNoCancel,
                    //    MessageBoxIcon.Question,
                    //    MessageBoxDefaultButton.Button3);

                    if (await Application.Current.MainPage.DisplayAlert("New Item", "A line item already exists for this location and product." + Environment.NewLine + "Do you wish to create one?", "Yes", "No"))
                    {
                        result = DialogResult.Yes;
                    }
                }
                else
                {
                    // If the selected product to insert was not found, force the dialog result to insert it.
                    result = DialogResult.Yes;
                }

                if (result == DialogResult.Yes)
                {
                    SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;

                    using (SQLiteCommand cmndPickItem = conn.CreateCommand())
                    using (SQLiteCommand cmndItem = conn.CreateCommand())
                    using (SQLiteCommand cmndID = conn.CreateCommand())
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            //using (SqlCeResultSet rs = cmndPickItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //SqlCeUpdatableRecord rec = rs.CreateRecord();
                            //using (SqlCeResultSet rsActItem = cmndItem.ExecuteResultSet(ResultSetOptions.Updatable))
                            //SqlCeUpdatableRecord recActItem = rsActItem.CreateRecord();

                            cmndItem.CommandText = "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus, ActivityRefno) VALUES (@activityItemSubType, @activityItemStatus, @activityRefno)";
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityItemStatus", Convert.ToInt32(SysCommon.ActStatus.NoChange)));
                            cmndItem.Parameters.Add(new SQLiteParameter("@activityRefno", Convert.ToInt32(this.ActHdr.Refno)));

                            cmndItem.ExecuteNonQuery();

                            //recActItem.SetInt32((int)SysCommon.ActItemFields.SubType, (int)SysCommon.ActSubTypes.Picking);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.Status, (int)SysCommon.ActStatus.NoChange);
                            //recActItem.SetInt32((int)SysCommon.ActItemFields.ActRefno, this.ActHdr.Refno);
                            //rsActItem.Insert(recActItem);

                            cmndID.CommandText = "SELECT last_insert_rowid() FROM tActivityItem";
                            cmndID.CommandType = CommandType.Text;

                            this.Refno = Convert.ToInt32(cmndID.ExecuteScalar());

                            cmndPickItem.CommandText = "INSERT INTO tActivityItemPicking (" +
                                "ActivityItemRefno, " +
                                "ActivityItemSubType, " +
                                "ActivityItemPickingFKRefno, " +
                                "ActivityItemPickingStringFKRefno, " +
                                "ProductUnitRefno, " +
                                "WarehouseLocationMapRefno, " +
                                "ActivityItemPickingQuantityOrdered, " +
                                "ActivityItemPickingBaseQuantityOrdered, " +
                                "ActivityItemPickingWeightQuantityOrdered, " +
                                "ActivityItemPickingQuantityShipped, " +
                                "ActivityItemPickingBaseQuantityShipped, " +
                                "ActivityItemPickingWeightQuantityShipped, " +
                                "ActivityItemPickingCustomerProductUnitCode, " +
                                "ActivityItemPickingDescription, " +
                                "ActivityItemPickingQuantityAvailable, " +
                                "ActivityItemPickingBaseQuantityAvailable, " +
                                "ActivityItemPickingWeightQuantityAvailable, " +
                                "ActivityItemPickingIsReviewed" +
                                ") VALUES (" +
                                "@activityItemRefno, " +
                                "@activityItemSubType, " +
                                "@activityItemPickingFKRefno, " +
                                "@activityItemPickingStringFKRefno, " +
                                "@productUnitRefno, " +
                                "@warehouseLocationMapRefno, " +
                                "@activityItemPickingQuantityOrdered, " +
                                "@activityItemPickingBaseQuantityOrdered, " +
                                "@activityItemPickingWeightQuantityOrdered, " +
                                "@activityItemPickingQuantityShipped, " +
                                "@activityItemPickingBaseQuantityShipped, " +
                                "@activityItemPickingWeightQuantityShipped, " +
                                "@activityItemPickingCustomerProductUnitCode, " +
                                "@activityItemPickingDescription, " +
                                "@activityItemPickingQuantityAvailable, " +
                                "@activityItemPickingBaseQuantityAvailable, " +
                                "@activityItemPickingWeightQuantityAvailable, " +
                                "@activityItemPickingIsReviewed" +
                                ")";
                            cmndPickItem.CommandType = CommandType.Text;
                            //cmndPickItem.IndexName = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno";
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemRefno", Convert.ToInt32(this.Refno)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemSubType", Convert.ToInt32(SysCommon.ActSubTypes.Picking)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingFKRefno", Convert.ToInt32(this.Refno * -1)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingStringFKRefno", Convert.ToString(this.Refno * -1)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@productUnitRefno", Convert.ToInt32(prodUnitRefno)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(this.LocMap.CurrLocMap.WhsLocMapRefno)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityOrdered", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityOrdered", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityOrdered", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityShipped", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityShipped", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityShipped", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingCustomerProductUnitCode", Convert.ToString(string.Empty)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingDescription", Convert.ToString(this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingQuantityAvailable", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingBaseQuantityAvailable", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingWeightQuantityAvailable", Convert.ToInt32(0)));
                            cmndPickItem.Parameters.Add(new SQLiteParameter("@activityItemPickingIsReviewed", Convert.ToInt32(false)));

                            cmndPickItem.ExecuteNonQuery();

                            transaction.Commit();

                            //rec.SetInt32((int)SysCommon.ActItemPickingFields.ActItemRefno, this.Refno);
                            //rec.SetInt32((int)SysCommon.ActItemPickingFields.ActItemSubType, (int)SysCommon.ActSubTypes.Picking);
                            //rec.SetInt32((int)SysCommon.ActItemPickingFields.FKRefno, this.Refno * -1);
                            //rec.SetString((int)SysCommon.ActItemPickingFields.StringFKRefno, Convert.ToString(this.Refno * -1));
                            //rec.SetInt32((int)SysCommon.ActItemPickingFields.ProdUnitRefno, prodUnitRefno);
                            //rec.SetInt32((int)SysCommon.ActItemPickingFields.WhsLocMapRefno, this.LocMap.CurrLocMap.WhsLocMapRefno);
                            //rec.SetString((int)SysCommon.ActItemPickingFields.Description, this.ProdInfo.Desc.Trim() + " " + this.ProdInfo.UnitSizeDesc);
                            //rec.SetString((int)SysCommon.ActItemPickingFields.CustomerProductUnitCode, string.Empty);
                            //rec.SetBoolean((int)SysCommon.ActItemPickingFields.IsReviewed, false);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityOrdered, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityOrdered, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityOrdered, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityShipped, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityShipped, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityShipped, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.QuantityAvailable, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.BaseQuantityAvailable, 0);
                            //rec.SetDecimal((int)SysCommon.ActItemPickingFields.WeightQuantityAvailable, 0);
                            //rs.Insert(rec);

                            this.FKRefno = this.Refno * -1;

                            this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                            this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);

                            if (this.DataViewItems != null)
                            {
                                DataRowView rowView = this.DataViewItems.AddNew();

                                rowView["ActivityItemRefno"] = this.Refno;
                                rowView["ActivityItemPickingFKRefno"] = this.FKRefno;
                                rowView["ActivityItemPickingStringFKRefno"] = this.FKRefno.ToString();
                                rowView["ProductUnitRefno"] = prodUnitRefno;
                                rowView["ProductRefno"] = this.ProdInfo.Refno;
                                rowView["ProductCode"] = this.ProdInfo.Code;
                                rowView["ProductDescription"] = this.ProdInfo.Desc;
                                rowView["UnitCode"] = this.ProdInfo.UnitCode;
                                rowView["ProductUnitBC"] = this.ProdInfo.UnitBC;
                                rowView["ProductUnitSizeDescription"] = this.ProdInfo.UnitSizeDesc;
                                rowView["LocationMapRefno"] = this.LocMap.CurrLocMap.WhsLocMapRefno;
                                rowView["LocationCode"] = this.LocMap.CurrLocMap.WhsCode;
                                rowView["LocationName"] = this.LocMap.CurrLocMap.WhsName;
                                rowView["ActivityItemPickingCustomerProductUnitCode"] = string.Empty;
                                rowView["ActivityItemStatus"] = SysCommon.ActStatus.Inserted;
                                rowView["IsActivityItemPickingComment"] = " ";

                                rowView["ActivityItemPickingQuantityOrdered"] = 0;
                                rowView["ActivityItemPickingBaseQuantityOrdered"] = 0;
                                rowView["ActivityItemPickingWeightQuantityOrdered"] = 0;

                                rowView["ActivityItemPickingQuantityShipped"] = 0;
                                rowView["ActivityItemPickingBaseQuantityShipped"] = 0;
                                rowView["ActivityItemPickingWeightQuantityShipped"] = 0;

                                rowView["ActivityItemPickingQuantityAvailable"] = 0;
                                rowView["ActivityItemPickingBaseQuantityAvailable"] = 0;
                                rowView["ActivityItemPickingWeightQuantityAvailable"] = 0;

                                rowView.EndEdit();
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            // TODO: Logging
                            Console.WriteLine(ex);
                            transaction.Rollback();
                        }
                    }
                }
            }
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

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            // TODO: Calculate column widths
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            // Header grid
            Grid gridHeaderPicking = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
                }
            };

            gridHeaderPicking.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            Label labelIsActivityItemPickingCommentHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingProductCodeHeader = new Label { Text = "Product ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingProductUnitBCHeader = new Label { Text = " ", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingUnitCodeHeader = new Label { Text = " ", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingQuantityOrderedHeader = new Label { Text = "Ordered", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelActivityItemPickingQuantityReceivedHeader = new Label { Text = "Shipped", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            gridHeaderPicking.Children.Add(labelIsActivityItemPickingCommentHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingProductUnitBCHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingUnitCodeHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityOrderedHeader);
            gridHeaderPicking.Children.Add(labelActivityItemPickingQuantityReceivedHeader);

            Grid.SetColumn(labelIsActivityItemPickingCommentHeader, 0);
            Grid.SetColumn(labelActivityItemPickingProductCodeHeader, 1);
            Grid.SetColumn(labelActivityItemPickingProductUnitBCHeader, 2);
            Grid.SetColumn(labelActivityItemPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelActivityItemPickingQuantityOrderedHeader, 4);
            Grid.SetColumn(labelActivityItemPickingQuantityReceivedHeader, 5);

            Grid.SetRow(gridHeaderPicking, 0);

            dataGridMainActivityHeader.Children.Add(gridHeaderPicking);

            // Data grid
            DataTemplate dataTemplatePicking = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
                    }
                };

                Label labelActivityItemPickingIsActivityItemPickingComment = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingProductCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingProductUnitBC = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingQuantityOrdered = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelActivityItemPickingQuantityReceived = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelActivityItemPickingIsActivityItemPickingComment.SetBinding(Label.TextProperty, new Binding("IsActivityItemPickingComment"));
                labelActivityItemPickingProductCode.SetBinding(Label.TextProperty, new Binding("ProductCode"));
                labelActivityItemPickingProductUnitBC.SetBinding(Label.TextProperty, new Binding("ProductUnitBC"));
                labelActivityItemPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelActivityItemPickingQuantityOrdered.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityOrdered", BindingMode.Default, null, null, "{0:F3}"));
                labelActivityItemPickingQuantityReceived.SetBinding(Label.TextProperty, new Binding("ActivityItemPickingQuantityShipped", BindingMode.Default, null, null, "{0:F3}"));

                gridData.Children.Add(labelActivityItemPickingIsActivityItemPickingComment);
                gridData.Children.Add(labelActivityItemPickingProductCode);
                gridData.Children.Add(labelActivityItemPickingProductUnitBC);
                gridData.Children.Add(labelActivityItemPickingUnitCode);
                gridData.Children.Add(labelActivityItemPickingQuantityOrdered);
                gridData.Children.Add(labelActivityItemPickingQuantityReceived);

                Grid.SetColumn(labelActivityItemPickingIsActivityItemPickingComment, 0);
                Grid.SetColumn(labelActivityItemPickingProductCode, 1);
                Grid.SetColumn(labelActivityItemPickingProductUnitBC, 2);
                Grid.SetColumn(labelActivityItemPickingUnitCode, 3);
                Grid.SetColumn(labelActivityItemPickingQuantityOrdered, 4);
                Grid.SetColumn(labelActivityItemPickingQuantityReceived, 5);

                return new ViewCell { View = gridData };
            });

            dataGridMainActivity.ItemTemplate = dataTemplatePicking;
            observableCollection.Clear();

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                PickingItem pickingItem = new PickingItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(pickingItem);
            }

            return observableCollection;
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
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno AS ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno AS ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
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
                sql.Append("SELECT tActivityItemPicking.ActivityItemRefno AS ActivityItemRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingFKRefno AS ActivityItemPickingFKRefno,");
                sql.Append("tActivityItemPicking.ActivityItemPickingStringFKRefno AS ActivityItemPickingStringFKRefno,");
                sql.Append("tProductUnit.ProductUnitRefno AS ProductUnitRefno,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityOrdered AS ActivityItemPickingQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityOrdered AS ActivityItemPickingBaseQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityOrdered AS ActivityItemPickingWeightQuantityOrdered,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityShipped AS ActivityItemPickingQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityShipped AS ActivityItemPickingBaseQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityShipped AS ActivityItemPickingWeightQuantityShipped,");
                sql.Append("tActivityItemPicking.ActivityItemPickingCustomerProductUnitCode AS ActivityItemPickingCustomerProductUnitCode,");
                sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("tActivityItemPicking.ActivityItemPickingQuantityAvailable AS ActivityItemPickingQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingBaseQuantityAvailable AS ActivityItemPickingBaseQuantityAvailable,");
                sql.Append("tActivityItemPicking.ActivityItemPickingWeightQuantityAvailable AS ActivityItemPickingWeightQuantityAvailable,");
                sql.Append("CASE WHEN tActivityItemPickingComment.ActivityItemPickingCommentComment IS NULL ");
                sql.Append("THEN \' \' ELSE \'*\' END AS IsActivityItemPickingComment,");
                sql.Append("tActivityItem.ActivityItemStatus AS ActivityItemStatus ");
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
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            if (string.IsNullOrEmpty(this.ActItemFilter))
            {
                this.DataViewItems.RowFilter = "ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }
            else
            {
                this.DataViewItems.RowFilter = this.ActItemFilter + " AND ProductCode LIKE \'%" + searchText.Trim() + "%\'";
            }

            observableCollection.Clear();

            foreach (DataRow row in this.DataViewItems.ToTable().Rows)
            {
                PickingItem pickingItem = new PickingItem
                {
                    ActivityItemRefno = Convert.ToInt32(row["ActivityItemRefno"] != DBNull.Value ? row["ActivityItemRefno"] : null),
                    ActivityItemPickingFKRefno = Convert.ToInt32(row["ActivityItemPickingFKRefno"] != DBNull.Value ? row["ActivityItemPickingFKRefno"] : null),
                    ActivityItemPickingStringFKRefno = Convert.ToString(row["ActivityItemPickingStringFKRefno"] != DBNull.Value ? row["ActivityItemPickingStringFKRefno"] : null),
                    ProductUnitRefno = Convert.ToInt32(row["ProductUnitRefno"] != DBNull.Value ? row["ProductUnitRefno"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityItemPickingQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingQuantityOrdered"] : null),
                    ActivityItemPickingBaseQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityOrdered"] : null),
                    ActivityItemPickingWeightQuantityOrdered = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityOrdered"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityOrdered"] : null),
                    ActivityItemPickingQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingQuantityShipped"] : null),
                    ActivityItemPickingBaseQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingBaseQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingBaseQuantityShipped"] : null),
                    ActivityItemPickingWeightQuantityShipped = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityShipped"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityShipped"] : null),
                    ActivityItemPickingCustomerProductUnitCode = Convert.ToString(row["ActivityItemPickingCustomerProductUnitCode"] != DBNull.Value ? row["ActivityItemPickingCustomerProductUnitCode"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    ActivityItemPickingQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingQuantityAvailable"] : null),
                    ActivityItemPickingWeightQuantityAvailable = Convert.ToDecimal(row["ActivityItemPickingWeightQuantityAvailable"] != DBNull.Value ? row["ActivityItemPickingWeightQuantityAvailable"] : null),
                    IsActivityItemPickingComment = !string.IsNullOrWhiteSpace(Convert.ToString(row["IsActivityItemPickingComment"] != DBNull.Value ? row["IsActivityItemPickingComment"] : null)) ? "X" : "",
                    ActivityItemStatus = Convert.ToInt32(row["ActivityItemStatus"] != DBNull.Value ? row["ActivityItemStatus"] : null)
                };

                observableCollection.Add(pickingItem);
            }

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void SelectItem(int idx)
        {
            base.SelectItem(idx);
            this.FKRefno = Convert.ToInt32(this.DataViewItems[idx]["ActivityItemPickingFKRefno"]);
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
                sql.AppendFormat("AND tActivityItemPicking.WarehouseLocationMapRefno = {0} ", this.LocMap.CurrLocMap.WhsLocMapRefno);
                sql.AppendFormat("AND tActivityItemPicking.ProductUnitRefno = {0}", prodUnitRefno);

                cmndItem.CommandType = CommandType.Text;
                cmndItem.CommandText = sql.ToString();

                using (SQLiteDataReader drItem = cmndItem.ExecuteReader())
                {
                    if (drItem.Read())
                    {
                        ////this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno((int)drItem["LocationMapRefno"]);
                        this.FKRefno = Convert.ToInt32(drItem["ActivityItemPickingFKRefno"]);
                        this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(Convert.ToInt32(drItem["ProductUnitRefno"]));
                        this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
                        this.Refno = Convert.ToInt32(drItem["ActivityItemRefno"]);
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
                // TODO: Resource strings
                throw new NotImplementedException("Only skid deletion is supported.");
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
            this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingQuantityShipped"]) + quantity;
            this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingBaseQuantityShipped"]) + baseQuantity;
            this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"] = Convert.ToDecimal(this.DataViewItems[idx]["ActivityItemPickingWeightQuantityShipped"]) + weightQuantity;
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
                            // TODO: Resource strings
                            msg = msg + string.Format("Total ordered:\r{0} @ {1}{2}\r\rTotal shipped:\r{3} @ {4}{5}",
                                Convert.ToInt32(Convert.ToDecimal(dr["Ordered"])),
                                Convert.ToDecimal(dr["WeightOrdered"]),
                                Convert.ToString(dr["WtUnitCode"]),
                                Convert.ToInt32(Convert.ToDecimal(dr["Shipped"])),
                                Convert.ToDecimal(dr["WeightShipped"]),
                                Convert.ToString(dr["WtUnitCode"]));
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred.", ex);
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
                decimal shipped = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityShipped"]);
                decimal ordered = Convert.ToDecimal(this.DataViewItems[this.CurrRowIdx]["ActivityItemPickingQuantityOrdered"]);
                
                if (shipped + actItemPickQuantity > ordered)
                {
                    // TODO: Resource strings
                    throw new Exception(string.Format("The resulting quantity shipped {0} {1} will be greater than the quantity ordered {2} {3}.", shipped + actItemPickQuantity, this.ProdInfo.UnitCode, ordered, this.ProdInfo.UnitCode));
                }
            }
        }

        #endregion
    }
}
