// <copyright file="clsAdjustmentActivityDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Services;
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="AdjActDetail" />.
    /// </summary>
    public class AdjActDetail : ActDetail
    {
        /// <summary>
        /// Defines the columnWidth0.
        /// </summary>
        private double columnWidth0 = 0;

        /// <summary>
        /// Defines the columnWidth1.
        /// </summary>
        private double columnWidth1 = 0;

        /// <summary>
        /// Defines the columnWidth2.
        /// </summary>
        private double columnWidth2 = 0;

        /// <summary>
        /// Defines the columnWidth3.
        /// </summary>
        private double columnWidth3 = 0;

        /// <summary>
        /// Defines the resourceDictionary.
        /// </summary>
        private ResourceDictionary resourceDictionary = new ResourceDictionary();

        /// <summary>
        /// Defines the listAdjustmentDetails.
        /// </summary>
        private List<AdjustmentDetail> listAdjustmentDetails = new List<AdjustmentDetail>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjActDetail"/> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        public AdjActDetail(ActItem activityItem)
            : base(activityItem)
        {
            this.FullClassName = typeof(AdjActDetail).FullName;

            // Data grid
            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                Grid gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                    },
                };

                Label labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                Label labelSchemeID = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding(
                    "ActivityDetailWeightQuantity",
                    BindingMode.Default,
                    null,
                    null,
                    "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeID.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelSchemeID);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelSchemeID, 3);

                return new ViewCell { View = gridData };
            });

            this.resourceDictionary.Add("dataTemplate", dataTemplate);
        }

        /// <summary>
        /// The InsertSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>"
                + "</params>");

            string xmlSend = await this.CreateSubmitXML(request);
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
                    this.DetailFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                    this.NewItemFKRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                    this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                    this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                    this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);

                    // NOTE: should we have sent the extra detail from the comm server? should we load the extra detail
                    if (this.NewItemFKRefno != this.ActItem.ItemFKRefno)
                    {
                        // Update the Activity Item quantity Adjustment
                        string query = "UPDATE tActivityItemAdjustment SET ActivityItemAdjustmentStringFKRefno = ?," +
                            " ActivityItemAdjustmentFKRefno = ? WHERE ActivityItemRefno = ?";
                        _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                            query,
                            Convert.ToString(this.NewItemFKRefno),
                            this.NewItemFKRefno,
                            this.ActItem.ItemRefno);

                        this.ActItem.ItemFKRefno = this.NewItemFKRefno;

                        // TODO: Fix sorting
                        // string oldSort = this.ActItem.DataViewItems.Sort;
                        // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                        AdjustmentItem adj = this.ActItem.ListItems.OfType<AdjustmentItem>().FirstOrDefault(
                            adjItem => adjItem.ActivityItemRefno == this.ActItem.ItemRefno);

                        if (adj != null)
                        {
                            adj.ActivityItemAdjustmentFKRefno = this.NewItemFKRefno;
                            adj.ActivityItemAdjustmentStringFKRefno = this.NewItemFKRefno.ToString();
                        }

                        // TODO: Fix sorting
                        // this.ActItem.DataViewItems.Sort = oldSort;
                    }
                }
            }
        }

        /// <summary>
        /// The DeleteSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string xmlDelete = await this.CreateDeleteXML(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                string reqData = SysCommon.SubmitComm(xmlDelete);

                if (reqData.Length > 0)
                {
                    // Check if result is an error message
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
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXML(data);

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                int actDetailRefno = await this.InsertDetailDisconnected();
                _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("INSERT INTO tActivityDetailAdjustment (ActivityDetailRefno, ActivityDetailAdjustmentFKRefno) VALUES (?, ?)", Convert.ToInt32(actDetailRefno), Convert.ToInt32(this.DetailFKRefno));
                this.IsUpdateView = true;
            }
            else
            {
                if (this.InsertDetail(await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"))))
                {
                    _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        "UPDATE tActivityItemAdjustment SET ActivityItemAdjustmentStringFKRefno = ?," +
                        " ActivityItemAdjustmentFKRefno = ? WHERE ActivityItemRefno = ?",
                        Convert.ToString(this.NewItemFKRefno),
                        this.NewItemFKRefno,
                        this.ActItem.ItemRefno);

                    // TODO: Fix sorting
                    // string oldSort = this.ActItem.DataViewItems.Sort;
                    // this.ActItem.DataViewItems.Sort = "ActivityItemRefno";
                    AdjustmentItem adj = this.ActItem.ListItems.OfType<AdjustmentItem>().Where(adjItem => adjItem.ActivityItemRefno == this.ActItem.ItemRefno).FirstOrDefault();

                    if (adj != null)
                    {
                        adj.ActivityItemAdjustmentFKRefno = this.NewItemFKRefno;
                        adj.ActivityItemAdjustmentStringFKRefno = Convert.ToString(this.NewItemFKRefno);
                    }

                    // TODO: Fix sorting
                    // this.ActItem.DataViewItems.Sort = oldSort;
                }
            }

            this.InsertListDetail("ActivityDetailAdjustmentFKRefno");
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                XDocument extraData = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\"" +
                    " standalone=\"yes\"?><params></params>");
                string xmlDelete = await this.CreateDeleteXML(extraData);
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
                else
                {
                    throw new Exception("Unexpected delete activity detail error.");
                }
            }

            // Delete from database
            var conn = DatabaseConnection.GetInstance().SQLConn;
            await conn.RunInTransactionAsync(async delete =>
            {
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailProductSchemeVariantXRef" +
                    " WHERE ActivityDetailRefno = ?", this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailAdjustment" +
                    " WHERE ActivityDetailRefno = ?", this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailExtra" +
                    " WHERE ActivityDetailRefno = ?", this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetail" +
                    " WHERE ActivityDetailRefno = ?", this.DetailRefno);
            });

            //// delete row from DataView
            AdjustmentDetail adj = this.ListDetails.OfType<AdjustmentDetail>().Where(
                adjItem => adjItem.ActivityDetailFKRefno == this.DetailFKRefno).FirstOrDefault();
            if (adj != null)
            {
                _ = this.ListDetails.Remove(adj);
            }

            return 0;
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string strXmlSend = await this.CreateSubmitXML(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            string reqData = SysCommon.SubmitComm(strXmlSend);

            if (reqData.Length > 0)
            {
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
                    this.DetailFKRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

                    if (selectedActivityDetail is AdjustmentDetail adjustmentDetail)
                    {
                        adjustmentDetail.ActivityDetailFKRefno = this.DetailFKRefno;
                        adjustmentDetail.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync("UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?", Convert.ToInt32(SysCommon.ActStatus.NoChange), this.DetailRefno);
                }
            }
        }

        /// <summary>
        /// The GetInfo.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="isAdd">.</param>
        /// <returns>.</returns>
        public override async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            await base.GetInfo(selectedActivityDetail, isAdd);

            if (!isAdd && selectedActivityDetail is AdjustmentDetail adjustmentDetail)
            {
                this.DetailFKRefno = Convert.ToInt32(adjustmentDetail.ActivityDetailFKRefno);
                this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(Convert.ToInt32(adjustmentDetail.LocationMapRefno));
            }

            return this;
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);

                TableActivityAdjustmentDetail adjDetail = new TableActivityAdjustmentDetail();

                await adjDetail.PurgeData();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                adjDetail.RegisterDataAccessListenerProgressOut((AdjActDetail)this);
                await adjDetail.RequestData(this.ActItem.ItemRefno);
                adjDetail.UnregisterDataAccessListenerProgressOut((AdjActDetail)this);
            }
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as AdjustmentDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFKRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as AdjustmentDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get a data view of the activity detail.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>Data view result of the query.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            StringBuilder sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno AS ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
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
                sql.Append("\'Slot\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno as ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("\'Section\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno as ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("\'Whs\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno AS ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno AS ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode AS UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode AS ProductCode,");
                sql.Append("tProduct.ProductRefno AS ProductRefno,");
                sql.Append("tProduct.ProductDescription AS ProductDescription,");
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
                sql.Append("\'Slot\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno as ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("\'Section\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailAdjustment.ActivityDetailAdjustmentFKRefno as ActivityDetailFKRefno,");
                sql.Append("tUnit.UnitCode,");
                sql.Append("aUnitWeight.UnitCode AS WtUnitCode,");
                sql.Append("aUnitBase.UnitCode AS BaseUnitCode,");
                sql.Append("tProduct.ProductCode,");
                sql.Append("tProduct.ProductRefno,");
                sql.Append("tProduct.ProductDescription,");
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
                sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,");
                sql.Append("\'Whs\' AS Location,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantCode,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantNo,");
                sql.Append("tProductSchemeVariant.ProductSchemeVariantDescription,");
                sql.Append("tActivityDetail.ActivityDetailStatus,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailAdjustment ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailAdjustment.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }

            // using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql.ToString(), DatabaseConnection.GetInstance().SQLConn))
            // {
            //    using (DataSet ds = new DataSet())
            //    {
            //        da.Fill(ds);
            //        this.DataViewDetails = ds.Tables[0].DefaultView;
            //        this.DataViewDetails.Table.TableName = this.MainGridMappingName;
            //    }
            // }
            this.listAdjustmentDetails = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<AdjustmentDetail>(sql.ToString());
            this.ListDetails = this.listAdjustmentDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityExtraInfoItems"><see cref="ObservableCollection(object)"/> Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityExtraInfoItems)
        {
            mainActivityExtraInfoItems.Add(actDetail);

            Label labelQtyHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };
            Label labelWeightHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailQuantity));
            double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                string.Format("{0:F3}", actDetail.ActivityDetailWeightQuantity));
            double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.ActivityDetailSchemeID);

            if (detailWidth0 > this.columnWidth0)
            {
                this.columnWidth0 = detailWidth0;
                gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
            }

            if (detailWidth1 > this.columnWidth1)
            {
                this.columnWidth1 = detailWidth1;
                gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
            }

            if (detailWidth2 > this.columnWidth2)
            {
                this.columnWidth2 = detailWidth2;
                gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
            }

            if (detailWidth3 > this.columnWidth3)
            {
                this.columnWidth3 = detailWidth3;
                gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
            }

            return (DataTemplate)this.resourceDictionary["dataTemplate"];
        }

        /// <summary>
        /// Design grid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns><see cref="ObservableCollection{object}"/> Constructed observable collection.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid GridMainActivity,
            ref ListView ListViewMainActivity)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();

            foreach (AdjustmentDetail adjustmentDetail in this.ListDetails)
            {
                observableCollection.Add(adjustmentDetail);
            }

            Label labelQtyHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };
            Label labelWeightHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            Label labelSchemeIDHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            this.columnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.columnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.columnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.columnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIDHeader.Text);

            // Header grid
            Grid gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.columnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.columnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIDHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIDHeader, 3);

            Grid.SetRow(gridHeader, 0);

            GridMainActivity.Children.Add(gridHeader);

            foreach (AdjustmentDetail adjustmentDetail in observableCollection)
            {
                double detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", adjustmentDetail.ActivityDetailQuantity));
                double detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    string.Format("{0:F3}", adjustmentDetail.ActivityDetailWeightQuantity));
                double detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    adjustmentDetail.UnitCode);
                double detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    adjustmentDetail.ActivityDetailSchemeID);

                if (detailWidth0 > this.columnWidth0)
                {
                    this.columnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.columnWidth0;
                }

                if (detailWidth1 > this.columnWidth1)
                {
                    this.columnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.columnWidth1;
                }

                if (detailWidth2 > this.columnWidth2)
                {
                    this.columnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.columnWidth2;
                }

                if (detailWidth3 > this.columnWidth3)
                {
                    this.columnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.columnWidth3;
                }
            }

            ListViewMainActivity.ItemTemplate = (DataTemplate)this.resourceDictionary["dataTemplate"];

            return observableCollection;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.listAdjustmentDetails.Where(adjDetail =>
                adjDetail.ActivityDetailFKRefno.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (AdjustmentDetail adjustmentDetail in this.ListDetails)
            {
                observableCollection.Add(adjustmentDetail);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select the activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivityDetail(object selectedActivityDetail)
        {
            await base.SelectActivityDetail(selectedActivityDetail);
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            AdjustmentSettings adjSettings = new AdjustmentSettings().LoadData();

            SysCommon.AdjTypes adjType = (SysCommon.AdjTypes)Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(
    "SELECT ActivityAdjustmentType FROM tActivityAdjustment WHERE ActivityRefno = ?",
    this.ActItem.ActHdr.HdrRefno));

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT ADJUSTMENT DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + "<adjustment_type>" + (int)adjType + "</adjustment_type>"
                    + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT ADJUSTMENT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<adjustment_type>" + (int)adjType + "</adjustment_type>"
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFKRefno.ToString() + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFKRefno.ToString() + "</itm_refno>"
                    + "<is_negate_qty>" + adjSettings.NegateScanQuantity + "</is_negate_qty>"
                    + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                    + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                    + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                    + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                    + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateDeleteXML(XDocument data)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);
            SysCommon.AdjTypes adjType = (SysCommon.AdjTypes)Convert.ToInt32(await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(
    "SELECT ActivityAdjustmentType FROM tActivityAdjustment WHERE ActivityRefno = ?",
    this.ActItem.ActHdr.HdrRefno));
            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE ADJUSTMENT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<adjustment_type>" + (int)adjType + "</adjustment_type>"
                        + "<skid_fk_refno>0</skid_fk_refno>"
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE ADJUSTMENT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<adjustment_type>" + (int)adjType + "</adjustment_type>"
                        + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                        + "</imclient>");
                }
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE ADJUSTMENT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<adjustment_type>" + (int)adjType + "</adjustment_type>"
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                    + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                    + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                    + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                    + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                    + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"

                    + "</imclient>");
            }

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return string.Format("Adjustment # {0}", this.ActItem.ActHdr.HdrFKRefno);
        }

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="actDetailFKRefnoFldName"><see cref="string"/> Foreign key name field name to find existing
        /// value in.</param>
        public override ActivityDetail InsertListDetail(string actDetailFKRefnoFldName)
        {
            AdjustmentDetail detail = null;

            if (this.IsUpdateView && this.ListDetails != null)
            {
                bool detailToBeAdded = false;
                if (this.DetailFKRefno > 0)
                {
                    detail = this.ListDetails.OfType<AdjustmentDetail>().FirstOrDefault(adjDetail =>
                        adjDetail.ActivityDetailFKRefno == this.DetailFKRefno);
                }

                if (detail == null)
                {
                    detail = new AdjustmentDetail();
                    detailToBeAdded = true;
                    detail.ActivityDetailRefno = 0;
                    detail.ProductRefno = this.ActItem.ProdInfo.Refno;
                    detail.ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno;
                    detail.ProductCode = this.ActItem.ProdInfo.Code;
                    detail.ProductUnitBC = this.ActItem.ProdInfo.UnitBC;
                    detail.ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBC;
                    detail.ProductDescription = this.ActItem.ProdInfo.Desc;
                    detail.ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc;
                    detail.UnitCode = this.ActItem.ProdInfo.UnitCode;
                    detail.BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode;
                    detail.WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode;
                    detail.ActivityDetailQuantity = 0;
                    detail.ActivityDetailBaseQuantity = 0;
                    detail.ActivityDetailWeightQuantity = 0;
                    detail.ActDetailFKRefnoFldName = actDetailFKRefnoFldName;
                    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
                }
                else
                {
                    detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
                }

                detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;
                detail.ActivityDetailQuantity = Convert.ToDecimal(detail.ActivityDetailQuantity) +
                    Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity) +
                    Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity) +
                    Math.Round(this.WeightQuantity, 3);
                detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
                detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
                detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
                detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
                detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
                detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
                detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
                detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
                detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
                detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
                detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
                detail.ActivityDetailExtra5Date = this.ActExtra.DateTime1;

                int locCode = 0;
                string locName = string.Empty;

                if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SlotCode;
                    locName = this.LocMap.CurrLocMap.SlotName;
                }
                else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
                {
                    locCode = this.LocMap.CurrLocMap.SectCode;
                    locName = this.LocMap.CurrLocMap.SectName;
                }
                else
                {
                    locCode = this.LocMap.CurrLocMap.WhsCode;
                    locName = this.LocMap.CurrLocMap.WhsName;
                }

                detail.LocationCode = locCode;
                detail.LocationName = locName;
                detail.ActivityDetailSchemeID = this.SchemeID;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeID;

                if (detailToBeAdded)
                {
                    this.ListDetails.Add(detail);
                }
            }

            return detail;
        }
    }
}
