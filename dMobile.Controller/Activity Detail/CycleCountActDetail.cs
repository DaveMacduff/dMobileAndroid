// <copyright file="CycleCountActDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Controller.Activity_Item;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types.Details;
using dMobile.Resources.Types.Items;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="CycleCountActDetail" />.
    /// </summary>
    public class CycleCountActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CycleCountActDetail"/> class.
        /// </summary>
        /// <param name="actItem">.</param>
        public CycleCountActDetail(ActItem actItem)
            : base(actItem)
        {
            var dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                var gridData = new Grid()
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                    },
                };

                var labelQty = new Label { VerticalOptions = LayoutOptions.Center };
                var labelWeight = new Label { VerticalOptions = LayoutOptions.Center };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center };
                var labelSchemeId = new Label { VerticalOptions = LayoutOptions.Center };

                labelQty.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeight.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeId.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                gridData.Children.Add(labelQty);
                gridData.Children.Add(labelWeight);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelSchemeId);

                Grid.SetColumn(labelQty, 0);
                Grid.SetColumn(labelWeight, 1);
                Grid.SetColumn(labelUnitCode, 2);
                Grid.SetColumn(labelSchemeId, 3);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateCycleCountDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateCycleCountDetail", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width...
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width...
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width...
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width...
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width...
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary...
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the ListCountDetails
        /// Gets or sets a list of count details..
        /// </summary>
        public List<CountDetail> ListCountDetails { get; set; } = new List<CountDetail>();

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail GatherDetail()
        {
            CountDetail detail = null;
            if (this.ListDetails != null)
            {
                var sysEnv = SysEnvironment.GetInstance();
                detail = new CountDetail
                {
                    // KLUDGE: Restore the new detail reference number's that got over written somehow.
                    ActivityDetailRefno = sysEnv.InsertedDetailRefno,
                    ActivityDetailFKRefno = sysEnv.InsertedDetailFkRefno,
                };

                this.GatherProductDetail(detail);
                this.GatherQuantityDetail(detail);
                this.GatherSchemeDetail(detail);
                this.GatherExtraDetail(detail);
                this.GatherLocationDetail(detail);

                detail.ActivityDetailSchemeID = this.SchemeId;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeId;
                detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
            }

            return detail;
        }

        /// <summary>
        /// Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int"/> Zero based list detail index.</param>
        public override void UpdateListDetail(int listDetailIdx)
        {
            var detail = (CountDetail)this.ListDetails[listDetailIdx];
            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
        }
        /// <summary>
        /// The InsertSkid.
        /// </summary>
        /// <param name="skidInfo">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>"
                + "</params>");

            var xmlSend = await this.CreateSubmitXml(request);
            var responseData = SysCommon.SubmitComm(xmlSend);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);
            this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
            this.ActItem.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
            this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
            this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
            this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);
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
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                var responseData = SysCommon.SubmitComm(xmlDelete);
            }
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXml(data);

            // Get temporary item FK refno for newly inserted line item, used in disconnected mode.
            this.DetailFkRefno = await this.Conn.ExecuteScalarAsync<int>(
                "SELECT MAX(tActivityDetailCount.ActivityDetailFKRefno) FROM tActivityDetailCount");
            this.DetailFkRefno++;

            // Perform the comm server request first, because of return values. And we do not want to keep the
            // data if the submission fails. User will have to re-enter the data.
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Submit(this.SelectedDetail, data);
            }

            // In order to support base tracking the DetailFKRefno is the itm_refno rather than the serial.refno.
            this.DetailRefno = this.FindListDetailRefno(this.DetailFkRefno);
            if (this.DetailRefno < 0)
            {
                this.DetailRefno = await this.InsertDetailIntoTablesDetail();
                const string query = "INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)";
                var rowsAffected = await this.Conn.ExecuteAsync(
                    query,
                    this.DetailRefno,
                    this.DetailFkRefno);
                if (rowsAffected <= 0)
                {
                    throw new Exception("Unable to insert activity detail.");
                }
            }

            // KLUDGE: Save the new detail reference number's to be restored.
            this.SysEnv.InsertedDetailRefno = this.DetailRefno;
            this.SysEnv.InsertedDetailFkRefno = this.DetailFkRefno;
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                var responseData = SysCommon.SubmitComm(xmlDelete);
            }

            // Delete from database
            var conn = this.Conn;
            await conn.RunInTransactionAsync(DeleteFromTables);

            return 0;

            //////////////////////////////////////////////////////////////////////////
            async void DeleteFromTables(SQLiteConnection delete)
            {
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailCount WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
            }
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            int rowsAffected;

            var xmlDoc = new XmlDocument();
            var strXmlSend = await this.CreateSubmitXml(data);
            var responseData = SysCommon.SubmitComm(strXmlSend);
            xmlDoc.LoadXml(responseData);

            if (xmlDoc.DocumentElement == null)
            {
                throw new Exception("xmlDoc.DocumentElement == null");
            }

            this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"] !.InnerText);

            switch (this.ActItem.ActStatus)
            {
                case SysCommon.ActStatus.NoChange:
                    break;

                case SysCommon.ActStatus.Inserted:
                    this.ActItem.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"] !.InnerText);

                    // Update the selected item also.
                    if (this.ActItem.SelectedItem is CountItem item)
                    {
                        item.ActivityItemFKRefno = this.ActItem.ItemFkRefno;
                        item.ActivityItemStringFKRefno = this.ActItem.ItemFkRefno.ToString();

                        rowsAffected = await this.Conn.ExecuteAsync(
                            "UPDATE tActivityItemCount SET ActivityItemFKRefno = ?" +
                            ",ActivityItemStringFKRefno = ?" +
                            " WHERE ActivityItemRefno = ?",
                            item.ActivityItemFKRefno,
                            item.ActivityItemStringFKRefno,
                            this.ActItem.ItemRefno);
                        if (rowsAffected < 0)
                        {
                            throw new Exception(
                                "Unable to locate and update the cycle count activity line item with a new foreign" +
                                " key upon inserting.");
                        }
                    }

                    this.ActItem.ActStatus = SysCommon.ActStatus.NoChange;

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.ActItem.ItemRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception(
                            "Unable to locate and update the activity line item with a new foreign" +
                            " key upon inserting.");
                    }

                    if (selectedActivityDetail is CountDetail selectedInserted)
                    {
                        selectedInserted.ActivityDetailFKRefno = this.DetailFkRefno;
                        selectedInserted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is CountDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Deleted:
                    if (selectedActivityDetail is CountDetail selectedDeleted)
                    {
                        selectedDeleted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (this.ActStatus)
            {
                case SysCommon.ActStatus.NoChange:
                    break;

                case SysCommon.ActStatus.Inserted:

                    // Submit occurs before adding the data to the tables.
                    this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"] !.InnerText);

                    ////if (selectedActivityDetail is AdjustmentDetail selectedInserted)
                    ////{
                    ////    selectedInserted.ActivityDetailFKRefno = this.DetailFkRefno;
                    ////    selectedInserted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);

                    ////    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    ////        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                    ////        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                    ////        this.DetailRefno);

                    ////    if (rowsAffected < 0)
                    ////    {
                    ////        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    ////    }

                    ////    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    ////        "UPDATE tActivityDetailAdjustment SET ActivityDetailFKRefno = ? WHERE ActivityDetailRefno = ?",
                    ////        selectedInserted.ActivityDetailFKRefno,
                    ////        this.DetailRefno);

                    ////    if (rowsAffected < 0)
                    ////    {
                    ////        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    ////    }
                    ////}

                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is AdjustmentDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.Updated);
                    }

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.Updated),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Deleted:
                    if (selectedActivityDetail is AdjustmentDetail selectedDeleted)
                    {
                        selectedDeleted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.Deleted);
                    }

                    rowsAffected = await this.Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.Deleted),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableCountDet = new TableActivityCycleCountDetail();
                await tableCountDet.PurgeData();
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableCountDet.RegisterDataAccessListenerProgressOut(this);
                await tableCountDet.RequestDetailData(this.ActItem.ItemFkRefno, this.ActItem.ItemRefno);
                tableCountDet.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// The GetActivityDetailList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailFKRefno,");
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
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailFKRefno,");
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
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailCount.ActivityDetailFKRefno,");
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
                sql.Append("FROM tActivityDetailCount ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailCount.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts));
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
            {
                // This large query failed on large data sets like Prairie most of the time. Out of memory exception.
                // The queries are identical other than Slot, Section, Warehouse location labels
                // Run 3 separate queries and then join them afterwards.
                var slotList = await this.GetSlotDetails(this.ActItem.ItemRefno);
                var sectionList = await this.GetSectionDetails(this.ActItem.ItemRefno);
                var warehouseList = await this.GetWarehouseDetails(this.ActItem.ItemRefno);

                // slotTable.Merge(sectionTable, true, MissingSchemaAction.Ignore);
                // slotTable.Merge(warehouseTable, true, MissingSchemaAction.Ignore);
                slotList.AddRange(sectionList);
                slotList.AddRange(warehouseList);

                // DataView view = slotTable.DefaultView;
                // view.Sort = "ActivityDetailRefno ASC";
                // DataViewDetails = view;
                // DataViewDetails.Table.TableName = MainGridMappingName;
                this.ListCountDetails = slotList.Cast<CountDetail>().ToList();
                this.ListDetails = this.ListCountDetails.Cast<object>().ToList();
                return this.ListDetails;
            }

            this.ListCountDetails = await this.Conn.QueryAsync<CountDetail>(sql.ToString());
            this.ListDetails = this.ListCountDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// Gets all slot details for the reference number.
        /// </summary>
        /// <param name="refno">The refno<see cref="int"/>.</param>
        /// <returns>.</returns>
        private async Task<List<CountDetail>> GetSlotDetails(int refno)
        {
            var slotQuery = "SELECT tActivityDetail.ActivityDetailRefno, " +
                            "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
                            "tActivityDetailCount.ActivityDetailFKRefno, " +
                            "tUnit.UnitCode AS UnitCode, " +
                            "aUnitWeight.UnitCode AS WtUnitCode, " +
                            "aUnitBase.UnitCode AS BaseUnitCode, " +
                            "tProduct.ProductCode AS ProductCode, " +
                            "tProduct.ProductRefno AS ProductRefno, " +
                            "tProduct.ProductDescription AS ProductDescription, " +
                            "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                            "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
                            "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                            "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
                            "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
                            "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
                            "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                            "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
                            "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
                            "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
                            "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
                            "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
                            "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
                            "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
                            "tLocationMapSlot.LocationMapSlotCode AS LocationCode, " +
                            "CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName, " +
                            "\'Slot\' AS Location, " +
                            "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
                            "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
                            "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
                            "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
                            "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
                            "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
                            "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
                            "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
                            "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
                            "FROM tActivityDetailCount  " +
                            "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
                            "= tActivityDetailCount.ActivityDetailRefno  " +
                            "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
                            "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
                            "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
                            "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
                            "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
                            "INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno  " +
                            "= tActivityDetail.LocationMapRefno  " +
                            "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
                            "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
                            "= tActivityDetailCount.ActivityDetailRefno  " +
                            "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
                            "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
                            "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
                            "= tActivityDetail.ActivityDetailRefno  " +
                            "WHERE tActivityDetail.ActivityItemRefno = " + refno +
                            " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await this.Conn.QueryAsync<CountDetail>(slotQuery);
        }

        /// <summary>
        /// The GetSectionDetails.
        /// </summary>
        /// <param name="refno">The refno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List{CountDetail}}"/>.</returns>
        private async Task<List<CountDetail>> GetSectionDetails(int refno)
        {
            var sectionQuery = "SELECT tActivityDetail.ActivityDetailRefno, " +
                               "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
                               "tActivityDetailCount.ActivityDetailFKRefno, " +
                               "tUnit.UnitCode AS UnitCode, " +
                               "aUnitWeight.UnitCode AS WtUnitCode, " +
                               "aUnitBase.UnitCode AS BaseUnitCode, " +
                               "tProduct.ProductCode AS ProductCode, " +
                               "tProduct.ProductRefno AS ProductRefno, " +
                               "tProduct.ProductDescription AS ProductDescription, " +
                               "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                               "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
                               "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                               "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
                               "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
                               "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
                               "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                               "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
                               "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
                               "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
                               "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
                               "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
                               "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
                               "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
                               "tLocationMapSection.LocationMapSectionCode AS LocationCode, " +
                               "CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName, " +
                               "\'Section\' AS Location, " +
                               "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
                               "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
                               "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
                               "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
                               "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
                               "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
                               "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
                               "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
                               "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
                               "FROM tActivityDetailCount  " +
                               "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
                               "= tActivityDetailCount.ActivityDetailRefno  " +
                               "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
                               "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
                               "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
                               "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
                               "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
                               "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno  " +
                               "= tActivityDetail.LocationMapRefno  " +
                               "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
                               "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
                               "= tActivityDetailCount.ActivityDetailRefno  " +
                               "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
                               "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
                               "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
                               "= tActivityDetail.ActivityDetailRefno  " +
                               "WHERE tActivityDetail.ActivityItemRefno = " + refno +
                               " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await this.Conn.QueryAsync<CountDetail>(sectionQuery);
        }

        /// <summary>
        /// The GetWarehouseDetails.
        /// </summary>
        /// <param name="refno">The refno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List{CountDetail}}"/>.</returns>
        private async Task<List<CountDetail>> GetWarehouseDetails(int refno)
        {
            var warehouseQuery =
                "SELECT tActivityDetail.ActivityDetailRefno, " +
                 "tActivityDetail.LocationMapRefno AS LocationMapRefno, " +
                 "tActivityDetailCount.ActivityDetailFKRefno, " +
                 "tUnit.UnitCode AS UnitCode, " +
                 "aUnitWeight.UnitCode AS WtUnitCode, " +
                 "aUnitBase.UnitCode AS BaseUnitCode, " +
                 "tProduct.ProductCode AS ProductCode, " +
                 "tProduct.ProductRefno AS ProductRefno, " +
                 "tProduct.ProductDescription AS ProductDescription, " +
                 "tProductUnit.ProductUnitBC AS ProductUnitBC, " +
                 "tProductUnit.ProductUnitIsBC AS ProductUnitIsBC, " +
                 "tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription, " +
                 "tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity, " +
                 "tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity, " +
                 "tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                 "tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID, " +
                 "tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID, " +
                 "tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber, " +
                 "tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate, " +
                 "tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber, " +
                 "tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode, " +
                 "tLocationMapWarehouse.LocationMapWarehouseName AS LocationName, " +
                 "\'Whs\' AS Location, " +
                 "tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode, " +
                 "tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo, " +
                 "tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription, " +
                 "tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus, " +
                 "tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text, " +
                 "tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date  " +
                 "FROM tActivityDetailCount  " +
                 "INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno  " +
                 "INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno  " +
                 "INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno  " +
                 "INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno  " +
                 "INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno  " +
                 "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno  " +
                 "= tActivityDetail.LocationMapRefno  " +
                 "LEFT JOIN tActivityDetailProductSchemeVariantXRef ON  " +
                 "tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno  " +
                 "= tActivityDetailCount.ActivityDetailRefno  " +
                 "LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno  " +
                 "= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno  " +
                 "LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno  " +
                 "= tActivityDetail.ActivityDetailRefno  " +
                "WHERE tActivityDetail.ActivityItemRefno = " + refno +
                " AND tActivityDetail.ActivityDetailSubType = " + Convert.ToInt32(SysCommon.ActSubTypes.CycleCounts);

            return await this.Conn.QueryAsync<CountDetail>(warehouseQuery);
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMain">The gridMain<see cref="Grid"/>.</param>
        /// <param name="listViewMain">The listViewMain<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var countDetail in this.ListDetails.Cast<CountDetail>())
            {
                observableCollection.Add(countDetail);
            }

            var labelQtyHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelWeightHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var labelSchemeIdHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIdHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIdHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIdHeader, 3);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            foreach (var countDetail in observableCollection.Cast<CountDetail>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{countDetail.ActivityDetailQuantity:F3}");
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{countDetail.ActivityDetailWeightQuantity:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(countDetail.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(countDetail.ActivityDetailSchemeID);

                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }

                if (detailWidth3 > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidth3;
                    gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
                }
            }

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateCycleCountDetail"];

            return observableCollection;
        }

        /// <inheritdoc cref="ActivityDetail.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var searchData = searchText.ToLower().Trim();
            this.ListDetails = this.ListCountDetails.Where(adjDetail =>
                adjDetail.ActivityDetailSchemeID.ToUpper().Contains(searchText.ToUpper().Trim())).Cast<object>().ToList();

            var details = new ObservableCollection<object>();
            foreach (var countDetail in this.ListDetails.Cast<CountDetail>())
            {
                details.Add(countDetail);
            }

            return details;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + 0 + "</skid_fk_refno>"
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT CYCLE COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
                        + "</imclient>");
                }
            }
            else
            {
                // TODO: extra? elements may not be needed.
                // Note whs_id, sect_refno and slot_refno are sent because counting will use the selected location not the
                // skid location. This is unique to counting and not picking / transfers etc...
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFkRefno + "</whs_id>"
                    + "<sect_refno>" + this.LocMap.CurrLocMap.SectFkRefno + "</sect_refno>"
                    + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFkRefno + "</slot_refno>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                    + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                    + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                    + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                    + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                    + "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateDeleteXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                // TODO: skid_fk_refno element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
                    + "</imclient>");
            }
            else
            {
                // TODO: is_negate_qty element may not be needed.
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CYCLE COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFkRefno + "</whs_id>"
                    + "<sect_refno>" + this.LocMap.CurrLocMap.SectFkRefno + "</sect_refno>"
                    + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFkRefno + "</slot_refno>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
                    + "</imclient>");
            }

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return $"Cycle count # {this.ActItem.ActHdr.HdrFkRefno}";
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection(object)"/> Main activity items list.</param>
        /// <returns><see cref="DataTemplate"/> Data template.</returns>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems)
        {
            mainActivityItems.Add(actDetail);

            var labelQtyHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };
            var labelWeightHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };
            var labelUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };
            var labelSchemeIdHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQtyHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIdHeader.Text);

            // Header grid
            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelQtyHeader);
            gridHeader.Children.Add(labelWeightHeader);
            gridHeader.Children.Add(labelUnitCodeHeader);
            gridHeader.Children.Add(labelSchemeIdHeader);

            Grid.SetColumn(labelQtyHeader, 0);
            Grid.SetColumn(labelWeightHeader, 1);
            Grid.SetColumn(labelUnitCodeHeader, 2);
            Grid.SetColumn(labelSchemeIdHeader, 3);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailQuantity:F3}");
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailWeightQuantity:F3}");
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.ActivityDetailSchemeID);

            if (detailWidth0 > this.ColumnWidth0)
            {
                this.ColumnWidth0 = detailWidth0;
                gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
            }

            if (detailWidth1 > this.ColumnWidth1)
            {
                this.ColumnWidth1 = detailWidth1;
                gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
            }

            if (detailWidth2 > this.ColumnWidth2)
            {
                this.ColumnWidth2 = detailWidth2;
                gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
            }

            if (detailWidth3 > this.ColumnWidth3)
            {
                this.ColumnWidth3 = detailWidth3;
                gridHeader.ColumnDefinitions[3].Width = this.ColumnWidth3;
            }

            return (DataTemplate)this.ResourceDictionary["dataTemplateCycleCountDetail"];
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
