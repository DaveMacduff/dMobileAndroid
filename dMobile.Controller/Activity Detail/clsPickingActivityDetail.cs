namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    public class PickingActDetail : ActDetail
    {
        private double columnWidth0 = 0;
        private double columnWidth1 = 0;
        private double columnWidth2 = 0;
        private double columnWidth3 = 0;
        private double columnWidth4 = 0;
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_actItem"></param>
        /// <remarks></remarks>
        public PickingActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(PickingActDetail).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                Grid gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                    }
                };

                Label labelPickingDetailConfirmed = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelPickingQuantity = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelPickingWeightQuantity = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelPickingUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                Label labelPickingSchemeID = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelPickingDetailConfirmed.SetBinding(Label.TextProperty, new Binding("ActivityDetailIsConfirmedString"));
                labelPickingQuantity.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelPickingWeightQuantity.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelPickingSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    gridData.Children.Add(labelPickingDetailConfirmed);
                }

                gridData.Children.Add(labelPickingQuantity);
                gridData.Children.Add(labelPickingWeightQuantity);
                gridData.Children.Add(labelPickingUnitCode);
                gridData.Children.Add(labelPickingSchemeID);

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    Grid.SetColumn(labelPickingDetailConfirmed, 0);
                }

                Grid.SetColumn(labelPickingQuantity, 1);
                Grid.SetColumn(labelPickingWeightQuantity, 2);
                Grid.SetColumn(labelPickingUnitCode, 3);
                Grid.SetColumn(labelPickingSchemeID, 4);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void InsertSkid(SysSkidInfo skidInfo)
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + ("<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>")
                + "</params>");

            string xmlSend = this.CreateSubmitXML(request);
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
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
                    this.IsUpdateView = true;
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                    this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                    this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                    this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

                    if (this.NewItemFKRefno != this.ActItem.FKRefno)
                    {
                        using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                        {
                            cmndItem.CommandType = CommandType.Text;
                            cmndItem.CommandText = "UPDATE tActivityItemPicking "
                                + "SET ActivityItemPickingStringFKRefno = \'" + this.NewItemFKRefno + "\', "
                                + "ActivityItemPickingFKRefno = " + this.NewItemFKRefno
                                + " WHERE ActivityItemRefno = " + this.ActItem.Refno;

                            cmndItem.ExecuteNonQuery();

                            this.ActItem.FKRefno = this.NewItemFKRefno;
                            string oldSort = this.ActItem.DataViewItems.Sort;
                            this.ActItem.DataViewItems.Sort = "ActivityItemRefno";  
                            var rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });

                            if (rowIdx != -1)
                            {
                                DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
                                rowView.BeginEdit();
                                rowView["ActivityItemPickingFKRefno"] = this.NewItemFKRefno;
                                rowView["ActivityItemPickingStringFKRefno"] = this.NewItemFKRefno.ToString();
                                rowView.EndEdit();
                            }

                            this.ActItem.DataViewItems.Sort = oldSort;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
        public override void DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete;
                xmlDelete = this.CreateDeleteXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // check if result is an error message
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
        public override void Insert(XDocument data)
        {
            this.FillActDetailObjFromXML(data);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int actDetailRefno = this.InsertDetailDisconnected();

                using (SQLiteCommand cmndDetailPicking = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndDetailPicking.CommandText = "INSERT INTO tActivityDetailPicking (ActivityDetailRefno, ActivityDetailPickingFKRefno, ActivityDetailPickingIsConfirmed) VALUES (@activityDetailRefno, @activityDetailPickingFKRefno, @activityDetailPickingIsConfirmed)";
                    cmndDetailPicking.CommandType = CommandType.Text;
                    //cmndDetailPicking.IndexName = "UQ_ActivityDetailPickingOnActivityDetailPickingFKRefno";
                    cmndDetailPicking.Parameters.Add(new SQLiteParameter("@activityDetailRefno", Convert.ToInt32(actDetailRefno)));
                    cmndDetailPicking.Parameters.Add(new SQLiteParameter("@activityDetailPickingFKRefno", Convert.ToInt32(this.FKRefno)));
                    cmndDetailPicking.Parameters.Add(new SQLiteParameter("@activityDetailPickingIsConfirmed", Convert.ToBoolean(true)));

                    cmndDetailPicking.ExecuteNonQuery();

                    //using (SqlCeResultSet rsActDetailPicking = cmndDetailPicking.ExecuteResultSet(ResultSetOptions.Updatable))
                    //{
                        //SqlCeUpdatableRecord recActDetailPicking = rsActDetailPicking.CreateRecord();
                        //recActDetailPicking.SetInt32((int)SysCommon.ActDetailPickingFields.ActDetailRefno, actDetailRefno);
                        //recActDetailPicking.SetInt32((int)SysCommon.ActDetailPickingFields.FKRefno, this.FKRefno);
                        //recActDetailPicking.SetBoolean((int)SysCommon.ActDetailPickingFields.IsConfirmed, true);
                        //rsActDetailPicking.Insert(recActDetailPicking);

                        this.IsUpdateView = true;
                    //}
                }
            }
            else
            {
                if (this.InsertDetail(this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
                {
                    using (SQLiteCommand cmndItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndItem.CommandType = CommandType.Text;
                        cmndItem.CommandText = "UPDATE tActivityItemPicking "
                            + "SET ActivityItemPickingStringFKRefno = \'" + this.NewItemFKRefno + "\', "
                            + "ActivityItemPickingFKRefno = " + this.NewItemFKRefno
                            + " WHERE ActivityItemRefno = " + this.ActItem.Refno;
                        cmndItem.ExecuteNonQuery();
                    }

                    string oldSort = this.ActItem.DataViewItems.Sort;
                    this.ActItem.DataViewItems.Sort = "ActivityItemRefno";

                    int rowIdx = this.ActItem.DataViewItems.Find(new object[] { this.ActItem.Refno });
                    if (rowIdx != -1)
                    {
                        DataRowView rowView = this.ActItem.DataViewItems[rowIdx];
                        rowView.BeginEdit();
                        rowView["ActivityItemPickingFKRefno"] = this.NewItemFKRefno;
                        rowView["ActivityItemPickingStringFKRefno"] = this.NewItemFKRefno.ToString();
                        rowView.EndEdit();
                    }

                    this.ActItem.DataViewItems.Sort = oldSort;
                }
            }

            this.InsertUpdateDataViewDetail("ActivityDetailPickingFKRefno", "ActivityDetailPickingFKRefno");
        }

        /// <summary>
        ///
        /// </summary>
        public override void Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = this.CreateDeleteXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // check if result is an error message
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        XmlDocument docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        string errorDetail = docError.DocumentElement["detail"].InnerText;

                        if (errorDetail.Contains("MSGBOX-"))
                        {
                            throw new MessageOnlyException(errorDetail.Substring(7));
                        }
                        else
                        {
                            // TODO: Resource strings
                            throw new Exception("Request delete detail error." + Environment.NewLine + errorDetail);
                        }
                    }
                }
            }

            string currSort = this.DataViewDetails.Sort;
            this.DataViewDetails.Sort = "ActivityDetailPickingFKRefno";
            int rowIdx = this.DataViewDetails.Find(this.FKRefno);

            if (rowIdx >= 0)
            {
                // delete row from DataView
                this.DataViewDetails.Delete(rowIdx);
            }

            this.DataViewDetails.Sort = currSort;

            // Delete from database
            using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndPurge.CommandType = CommandType.Text;

                cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetailPicking WHERE ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();

                cmndPurge.CommandText = "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = " + this.Refno;
                cmndPurge.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedRowIndex"></param>
        /// <param name="data"></param>
        public override void Submit(int idx, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlSend = this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(xmlSend);

            if (reqData.Length > 0)
            {
                xmlDoc.LoadXml(reqData);

                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlDoc.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail.Substring(7));
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("Request add detail error." + Environment.NewLine + errorDetail);
                    }
                }
                else
                {
                    this.FKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (this.DataViewDetails != null)
                    {
                        this.DataViewDetails[idx].BeginEdit();
                        this.DataViewDetails[idx]["ActivityDetailPickingFKRefno"] = this.FKRefno;
                        this.DataViewDetails[idx]["ActivityDetailStatus"] = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                        this.DataViewDetails[idx].EndEdit();
                    }

                    using (SQLiteCommand cmndDetail = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndDetail.CommandType = CommandType.Text;
                        cmndDetail.CommandText = "UPDATE tActivityDetail "
                            + "SET ActivityDetailStatus = " + Convert.ToInt32(SysCommon.ActStatus.NoChange)
                            + " WHERE ActivityDetailRefno = " + this.Refno;
                        cmndDetail.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public override ActDetail GetInfo(int idx, bool isAdd)
        {
            base.GetInfo(idx, isAdd);

            if (!isAdd)
            {
                this.FKRefno = Convert.ToInt32(this.DataViewDetails[idx]["ActivityDetailPickingFKRefno"]);
                this.LocMap.CurrLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewDetails[idx]["LocationMapRefno"]));
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityPickingDetail tableActPickingDetail = new TableActivityPickingDetail();
                tableActPickingDetail.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                tableActPickingDetail.RegisterDataAccessListenerProgressOut((PickingActDetail)this);
                tableActPickingDetail.Request(this.ActItem.Refno);
                tableActPickingDetail.UnregisterDataAccessListenerProgressOut((PickingActDetail)this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override int GetRefno(int selectedIndex)
        {
            return Convert.ToInt32(this.DataViewDetails[selectedIndex]["ActivityDetailRefno"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public override int GetFKRefno(int selectedIndex)
        {
            return Convert.ToInt32(this.DataViewDetails[selectedIndex]["ActivityDetailPickingFKRefno"]);
        }

        /// <summary>
        /// Get a data view of the activity detail.
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <remarks>The sql queries use a UNION this is needed to support base distrib-u-tec with a WMS Map turned on. 
        /// The map could include slots.
        /// Because the current location in dMobile may be a slot, base distrib-u-tec does not contain a slot location
        /// where the detail is stored, only the warehouse. So a union including the warehouse is needed to included 
        /// the received data from the base to match up with location refno. Have a default location on the slot level
        /// really only shows the user where to find the product. However the detail sent is lost if base wms is being 
        /// used.</remarks>
        /// <returns>Data view result of the query.</returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno AS ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Slot\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = ");
                sql.Append("tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Section\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Whs\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno AS ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription AS ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC AS ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSlot.LocationMapSlotCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Slot\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapSection.LocationMapSectionCode AS LocationCode,");
                sql.Append("CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Section\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
                sql.Append("tProduct.ProductBrandDescription,");
                sql.Append("tProductUnit.ProductUnitBC,");
                sql.Append("tProductUnit.ProductUnitIsBC,");
                sql.Append("tProductUnit.ProductUnitSizeDescription,");
                sql.Append("tActivityDetail.ActivityDetailQuantity,");
                sql.Append("tActivityDetail.ActivityDetailBaseQuantity,");
                sql.Append("tActivityDetail.ActivityDetailWeightQuantity,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,");
                sql.Append("tActivityDetail.ActivityDetailSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailUniqueSchemeID,");
                sql.Append("tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitProductionDate,");
                sql.Append("tActivityDetail.ActivityDetailLogisticUnitSkidNumber,");
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,");
                sql.Append("CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,");
                sql.Append("CAST(\'Whs\' AS NVARCHAR(8)) AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailPicking ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                sql.Append("tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailPicking.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.Refno);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }

            using (SQLiteDataAdapter dataAdapterDetail = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            {
                using (DataSet dataSetDetail = new DataSet())
                {
                    dataAdapterDetail.Fill(dataSetDetail);
                    this.DataViewDetails = dataSetDetail.Tables[0].DefaultView;
                    this.DataViewDetails.Table.TableName = this.MainGridMappingName;
                }
            }

            return this.DataViewDetails;
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (DataRow row in this.DataViewDetails.ToTable().Rows)
            {
                PickingDetail pickingDetail = new PickingDetail
                {
                    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    ActivityDetailPickingFKRefno = Convert.ToInt32(row["ActivityDetailPickingFKRefno"] != DBNull.Value ? row["ActivityDetailPickingFKRefno"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                    ActivityDetailIsConfirmed = Convert.ToBoolean(row["ActivityDetailIsConfirmed"] != DBNull.Value ? row["ActivityDetailIsConfirmed"] : null),
                    ActivityDetailIsConfirmedString = Convert.ToString(row["ActivityDetailIsConfirmedString"] != DBNull.Value ? row["ActivityDetailIsConfirmedString"] : null),
                    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null),
                    ActivityDetailExtra5Date = Convert.ToDateTime(row["ActivityDetailExtra5Date"] != DBNull.Value ? row["ActivityDetailExtra5Date"] : null)
                };

                observableCollection.Add(pickingDetail);
            }

            // TODO: Resource strings
            Label labelPickingDetailConfirmedHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelPickingQuantityHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelPickingWeightQuantityHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelPickingUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            Label labelPickingSchemeIDHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDetailConfirmedHeader.Text);
            columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingQuantityHeader.Text);
            columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingWeightQuantityHeader.Text);
            columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingUnitCodeHeader.Text);
            columnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth3, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(columnWidth4, GridUnitType.Absolute) }
                }
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                gridHeader.Children.Add(labelPickingDetailConfirmedHeader);
            }

            gridHeader.Children.Add(labelPickingQuantityHeader);
            gridHeader.Children.Add(labelPickingWeightQuantityHeader);
            gridHeader.Children.Add(labelPickingUnitCodeHeader);
            gridHeader.Children.Add(labelPickingSchemeIDHeader);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                Grid.SetColumn(labelPickingDetailConfirmedHeader, 0);
            }

            Grid.SetColumn(labelPickingQuantityHeader, 1);
            Grid.SetColumn(labelPickingWeightQuantityHeader, 2);
            Grid.SetColumn(labelPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelPickingSchemeIDHeader, 4);

            Grid.SetRow(gridHeader, 0);

            dataGridMainActivityHeader.Children.Add(gridHeader);

            foreach (PickingDetail pickingDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingDetail.ActivityDetailIsConfirmedString);
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", pickingDetail.ActivityDetailQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(string.Format("{0:F3}", pickingDetail.ActivityDetailWeightQuantity));
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingDetail.UnitCode);
                double detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingDetail.ActivityDetailSchemeID);

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    if (detailWidth0 > columnWidth0)
                    {
                        columnWidth0 = detailWidth0;
                        gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                    }
                }
                else
                {
                    columnWidth0 = 0;
                    gridHeader.ColumnDefinitions[0].Width = columnWidth0;
                }

                if (detailWidth1 > columnWidth1)
                {
                    columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = columnWidth1;
                }

                if (detailWidth2 > columnWidth2)
                {
                    columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = columnWidth2;
                }

                if (detailWidth3 > columnWidth3)
                {
                    columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = columnWidth3;
                }

                if (detailWidth4 > columnWidth4)
                {
                    columnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = columnWidth4;
                }
            }

            dataGridMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <param name="dataGridMain"></param>
        //        /// <remarks></remarks>
        //        public override void DesignDataGrid(DataGrid dataGridMain)
        //        {

        //            DataGridTableStyle dataGridStyle = new DataGridTableStyle();
        //            dataGridStyle.MappingName = this.MainGridMappingName;
        //            DataGridTextBoxColumn dataGridCol = null;

        //            if (this.SysEnv.SysSettings.IsPickConfirmDet)
        //            {
        //                CustomColumnStyle colStyle = null;
        //                colStyle = new CustomColumnStyle(0);
        //                colStyle.MappingName = "ActivityDetailIsConfirmedString";
        //                colStyle.HeaderText = "C";
        //                colStyle.Width = Convert.ToInt32(12 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(colStyle);
        //            }

        //            // WO# 11681 Heritage Foods Grid Layout
        //            if (this.SysEnv.SysSettings.PickDetGridDesignLayout == 1)
        //            {
        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailQuantity";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //                dataGridCol.HeaderText = "Qty";
        //#endif
        //                dataGridCol.Width = Convert.ToInt32((50 * colWidthFactor));
        //                dataGridCol.Format = "0.000";
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailLogisticUnitSkidNumber";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Skidzz);
        //#else
        //                dataGridCol.HeaderText = "Skid";
        //#endif
        //                dataGridCol.Width = (int)(50 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "LocationName";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Slot);
        //#else
        //                dataGridCol.HeaderText = "Slot";
        //#endif
        //                dataGridCol.Width = (int)(50 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailLogisticUnitBestBeforeSellByDate";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.BestBefore);
        //#else
        //                dataGridCol.HeaderText = "Best Before";
        //#endif
        //                dataGridCol.Width = (int)(60 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);
        //            }
        //            else
        //            {
        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailQuantity";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Qtyzzz);
        //#else
        //                dataGridCol.HeaderText = "Qty";
        //#endif
        //                dataGridCol.Width = (int)(50 * colWidthFactor);
        //                dataGridCol.Format = "0.000";
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailWeightQuantity";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.Weight);
        //#else
        //                dataGridCol.HeaderText = "Weight";
        //#endif
        //                dataGridCol.Width = (int)(55 * colWidthFactor);
        //                dataGridCol.Format = "0.000";
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "UnitCode";
        //                dataGridCol.HeaderText = string.Empty;
        //                dataGridCol.Width = (int)(30 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);

        //                dataGridCol = new DataGridTextBoxColumn();
        //                dataGridCol.MappingName = "ActivityDetailSchemeID";
        //#if MULTILANGUAGE
        //        dataGridCol.HeaderText = Translator.GetInstance().Translate(TranslatorVocabulary.SchemeId);
        //#else
        //                dataGridCol.HeaderText = "Scheme ID";
        //#endif
        //                dataGridCol.Width = (int)(500 * colWidthFactor);
        //                dataGridStyle.GridColumnStyles.Add(dataGridCol);
        //            }

        //            dataGridMain.TableStyles.Add(dataGridStyle);
        //            dataGridMain.RowHeadersVisible = false;
        //            dataGridMain.ColumnHeadersVisible = true;
        //        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            string fieldName = "ActivityPickingStringFKRefno";
            this.DataViewDetails.RowFilter = fieldName + " LIKE \'%" + searchText.Trim() + "%\'";

            foreach (DataRow row in this.DataViewDetails.ToTable().Rows)
            {
                PickingDetail pickingDetail = new PickingDetail
                {
                    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                    ActivityDetailPickingFKRefno = Convert.ToInt32(row["ActivityDetailPickingFKRefno"] != DBNull.Value ? row["ActivityDetailPickingFKRefno"] : null),
                    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                    ProductBrandDescription = Convert.ToString(row["ProductBrandDescription"] != DBNull.Value ? row["ProductBrandDescription"] : null),
                    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                    ProductSchemeVariantCode = Convert.ToString(row["ProductSchemeVariantCode"] != DBNull.Value ? row["ProductSchemeVariantCode"] : null),
                    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                    ActivityDetailIsConfirmed = Convert.ToBoolean(row["ActivityDetailIsConfirmed"] != DBNull.Value ? row["ActivityDetailIsConfirmed"] : null),
                    ActivityDetailIsConfirmedString = Convert.ToString(row["ActivityDetailIsConfirmedString"] != DBNull.Value ? row["ActivityDetailIsConfirmedString"] : null),
                    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null),
                    ActivityDetailExtra5Date = Convert.ToDateTime(row["ActivityDetailExtra5Date"] != DBNull.Value ? row["ActivityDetailExtra5Date"] : null)
                };

                observableCollection.Add(pickingDetail);
            }

            return observableCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectDetail(int idx)
        {
            if (idx >= 0)
            {
                this.SelectedLocMap = LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(this.DataViewDetails[idx]["LocationMapRefno"]));
                this.Refno = Convert.ToInt32(this.DataViewDetails[idx]["ActivityDetailRefno"]);
                this.FKRefno = Convert.ToInt32(this.DataViewDetails[idx]["ActivityDetailPickingFKRefno"]);
                this.SchemeID = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailSchemeID"]);
                this.UniqueSchemeID = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailUniqueSchemeID"]);

                this.ActExtra.Text1 = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailExtra1Text"]);
                this.ActExtra.Text2 = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailExtra2Text"]);
                this.ActExtra.Text3 = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailExtra3Text"]);
                this.ActExtra.Text4 = Convert.ToString(this.DataViewDetails[idx]["ActivityDetailExtra4Text"]);
                this.ActExtra.DateTime5 = Convert.ToDateTime(this.DataViewDetails[idx]["ActivityDetailExtra5Date"]);
            }
            else
            {
                this.ClearProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument sendData)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;

            if (this.ActItem.ActHdr.XMLDataCriteria == null)
            {
                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id></whs_id>"
                            + "<sect_refno></sect_refno>"
                            + "</command>"
                            + SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + ("<skid_fk_refno>" + 0 + "</skid_fk_refno>")
                            + "</imclient>");
                    }
                    else
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id></whs_id>"
                            + "<sect_refno></sect_refno>"
                            + "</command>"
                            + SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                            + "</imclient>");
                    }
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT SALES ORDER DETAIL SKID</action>"
                        + "<whs_id></whs_id>"
                        + "<sect_refno></sect_refno>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + ("<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>")
                        + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                        + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                        + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                        + "<is_negate_qty>false</is_negate_qty>"
                        + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
                        + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
                        + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
                        + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
                        + ("<extra5>" + this.ActExtra.DateTime5 + "</extra5>")
                        + "</imclient>");
                }
            }
            else
            {
                int whsRefno = Convert.ToInt32(this.ActItem.ActHdr.XMLDataCriteria.Root.Element("whs_id").Value);
                int sectRefno = Convert.ToInt32(this.ActItem.ActHdr.XMLDataCriteria.Root.Element("sect_refno").Value);

                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + ("<whs_id>" + whsRefno + "</whs_id>")
                            + ("<sect_refno>" + sectRefno + "</sect_refno>")
                            + "</command>"
                            + SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + ("<skid_fk_refno>" + 0 + "</skid_fk_refno>")
                            + "</imclient>");
                    }
                    else
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + ("<whs_id>" + whsRefno + "</whs_id>")
                            + ("<sect_refno>" + sectRefno + "</sect_refno>")
                            + "</command>"
                            + SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                            + "</imclient>");
                    }
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT SALES ORDER DETAIL SKID</action>"
                        + ("<whs_id>" + whsRefno + "</whs_id>")
                        + ("<sect_refno>" + sectRefno + "</sect_refno>")
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + ("<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>")
                        + ("<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                        + ("<refno>" + this.ActItem.ActHdr.FKRefno + "</refno>")
                        + ("<itm_refno>" + this.ActItem.FKRefno + "</itm_refno>")
                        + "<is_negate_qty>false</is_negate_qty>"
                        + ("<extra1>" + this.ActExtra.Text1 + "</extra1>")
                        + ("<extra2>" + this.ActExtra.Text2 + "</extra2>")
                        + ("<extra3>" + this.ActExtra.Text3 + "</extra3>")
                        + ("<extra4>" + this.ActExtra.Text4 + "</extra4>")
                        + ("<extra5>" + this.ActExtra.DateTime5 + "</extra5>")
                        + "</imclient>");
                }
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
        public override string CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = null;

            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE SALES ORDER DETAIL</action>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + 0 + "</skid_fk_refno>")
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE SALES ORDER DETAIL</action>"
                        + "</command>"
                        + SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + ("<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>")
                        + "</imclient>");
                }
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
                    + ("<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>")
                    + ("<whs_id>" + LocMap.CurrLocMap.WhsFKRefno + "</whs_id>")
                    + ("<refno>" + ActItem.ActHdr.FKRefno + "</refno>")
                    + ("<itm_refno>" + ActItem.FKRefno + "</itm_refno>")
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Total all activity detail quantity from the dataview
        /// </summary>
        /// <returns>SFillQuantity structure</returns>
        /// <remarks>Override the base to handle IsPickingModuleConfirmDetailMode</remarks>
        public override SysCommon.SFillQuantity GetDataViewTotalQuantity()
        {
            SysCommon.SFillQuantity fillQty = new SysCommon.SFillQuantity();

            if (this.DataViewDetails != null && this.DataViewDetails.Count > 0)
            {
                for (int i = 0; i <= this.DataViewDetails.Count - 1; i++)
                {
                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        if (Convert.ToBoolean(this.DataViewDetails[i]["ActivityDetailIsConfirmed"]) == true)
                        {
                            fillQty.Quantity += Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailQuantity"]);
                            fillQty.BaseQuantity += Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailBaseQuantity"]);
                            fillQty.WeightQuantity += Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailWeightQuantity"]);
                        }
                    }
                    else
                    {
                        fillQty.Quantity = fillQty.Quantity + Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailQuantity"]);
                        fillQty.BaseQuantity = fillQty.BaseQuantity + Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailBaseQuantity"]);
                        fillQty.WeightQuantity = fillQty.WeightQuantity + Convert.ToDecimal(this.DataViewDetails[i]["ActivityDetailWeightQuantity"]);
                    }
                }
            }

            return fillQty;
        }

        #endregion
    }
}