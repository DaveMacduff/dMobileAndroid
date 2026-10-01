// <copyright file="CutSheetFinishedActDetail.cs" company="InterSect Business Systems Inc">
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
    /// Defines the <see cref="CutSheetFinishedActDetail" />.
    /// </summary>
    public class CutSheetFinishedActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CutSheetFinishedActDetail"/> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        public CutSheetFinishedActDetail(ActItem activityItem)
          : base(activityItem)
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
                if (item.Key == "dataTemplateFinishedDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateFinishedDetail", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width.
        /// </summary>
        public double ColumnWidth0 { get; set; }

        /// <summary>
        /// Gets or sets the grid column 1 width.
        /// </summary>
        public double ColumnWidth1 { get; set; }

        /// <summary>
        /// Gets or sets the grid column 2 width.
        /// </summary>
        public double ColumnWidth2 { get; set; }

        /// <summary>
        /// Gets or sets the grid column 3 width.
        /// </summary>
        public double ColumnWidth3 { get; set; }

        /// <summary>
        /// Gets or sets the grid column 4 width.
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the finished details.
        /// </summary>
        public List<CutSheetFinishedDetail> FinishedDetails { get; set; } = new List<CutSheetFinishedDetail>();

        /// <inheritdoc/>
        public override async Task<ActDetailExtraDisplayInfo> GetExtraInfoAsync(object selectedItem)
        {
            if (selectedItem is CutSheetFinishedDetail detail)
            {
                // Get the line item location.
                var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    Convert.ToInt32(detail.LocationMapRefno));
                var detailLoc = LocationMapTasks.GetReadableLocation(itemLocMap);

                // Using the line item warehouse, get the product default location.
                var defLocMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    itemLocMap.WhsLocMapRefno,
                    Convert.ToInt32(detail.ProductRefno),
                    SysCommon.ProdLocMapTypes.Receiving);
                var defProdLoc = LocationMapTasks.GetReadableLocation(
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno));

                var prodDesc = detail.ProductDescription;
                var prodUnitSizeDesc = detail.ProductUnitSizeDescription;
                this.ActExtraDisplayInfo.Text1 = prodDesc.Trim() + " " + prodUnitSizeDesc.Trim();
                this.ActExtraDisplayInfo.Text2 = "Default Location: " + defProdLoc.Trim();
                this.ActExtraDisplayInfo.Text3 = "Finished Location: " + detailLoc.Trim();
            }

            return this.ActExtraDisplayInfo;
        }

        /// <inheritdoc/>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<skid_data>" + skidInfo.CurrSkid.SkidCode + "</skid_data>"
                + "</params>");

            var xmlSend = await this.CreateSubmitXml(request);
            var responseData = SysCommon.SubmitComm(xmlSend);
            if (responseData.Length > 0)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseData);
                this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
                this.ActItem.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
                this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
                this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
                this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);
            }
        }

        /// <inheritdoc/>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                _ = SysCommon.SubmitComm(xmlDelete);
            }
        }

        /// <inheritdoc/>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXml(data);

            // Get temporary item FK refno for newly inserted line item.
            this.DetailFkRefno = await Conn.ExecuteScalarAsync<int>(
                "SELECT MAX(tActivityDetailCutSheetFinished.ActivityDetailFKRefno) FROM tActivityDetailCutSheetFinished");
            this.DetailFkRefno++;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Submit(this, data);
            }

            // In order to support base tracking the DetailFKRefno is the itm_refno rather than the serial.refno.
            var detailRefno = this.FindListDetailRefno(this.DetailFkRefno);
            if (detailRefno < 0)
            {
                var query = "SELECT tActivityDetail.ActivityDetailRefno FROM tActivityDetail" +
                            " INNER JOIN tActivityDetailCutSheetFinished ON tActivityDetailCutSheetFinished.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno " +
                            $"WHERE tActivityDetailCutSheetFinished.ActivityDetailFKRefno = {this.DetailFkRefno}";
                detailRefno = await Conn.ExecuteScalarAsync<int>(
                    query,
                    this.DetailRefno,
                    this.DetailFkRefno);
                if (detailRefno == 0)
                {
                    detailRefno = -1;
                }
            }

            this.DetailRefno = detailRefno;

            if (this.DetailRefno < 0)
            {
                this.DetailRefno = await this.InsertDetailIntoTablesDetail();
                const string query = "INSERT INTO tActivityDetailCutSheetFinished (ActivityDetailRefno," +
                    " ActivityDetailFKRefno) VALUES (?, ?)";
                var rowsAffected = await Conn.ExecuteAsync(
                    query,
                    Convert.ToInt32(this.DetailRefno),
                    Convert.ToInt32(this.DetailFkRefno));
                if (rowsAffected <= 0)
                {
                    throw new BaseException("Unable to insert activity detail.");
                }
            }

            // KLUDGE: Save the new detail reference number's to be restored.
            this.SysEnv.InsertedDetailRefno = this.DetailRefno;
            this.SysEnv.InsertedDetailFkRefno = this.DetailFkRefno;
        }

        /// <inheritdoc/>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                _ = SysCommon.SubmitComm(xmlDelete);
            }

            // Delete from database
            var conn = Conn;
            await conn.RunInTransactionAsync(DeleteFromTables);

            return 0;

            async void DeleteFromTables(SQLiteConnection delete)
            {
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailCutSheetFinished WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
            }
        }

        /// <inheritdoc/>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            int rowsAffected;

            var xmlDoc = new XmlDocument();
            var strXmlSend = await this.CreateSubmitXml(data);
            var responseData = SysCommon.SubmitComm(strXmlSend);
            xmlDoc.LoadXml(responseData);

            if (xmlDoc.DocumentElement == null)
            {
                throw new BaseException("xmlDoc.DocumentElement == null");
            }

            this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"] !.InnerText);

            switch (this.ActItem.ActStatus)
            {
                case SysCommon.ActStatus.NoChange:
                    break;

                case SysCommon.ActStatus.Inserted:
                    this.ActItem.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"] !.InnerText);

                    // Update the selected item also.
                    if (this.ActItem.SelectedItem is CutSheetFinishedItem item)
                    {
                        item.ActivityItemFKRefno = this.ActItem.ItemFkRefno;
                    }

                    this.ActItem.ActStatus = SysCommon.ActStatus.NoChange;

                    rowsAffected = await Conn.ExecuteAsync(
                        "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.ActItem.ItemRefno);

                    if (rowsAffected < 0)
                    {
                        throw new BaseException(
                            "Unable to locate and update the activ ity line item with a new foreign key upon inserting.");
                    }

                    if (selectedActivityDetail is CutSheetFinishedDetail selectedInserted)
                    {
                        selectedInserted.ActivityDetailFKRefno = this.DetailFkRefno;
                        selectedInserted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new BaseException("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is CutSheetFinishedDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new BaseException("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Deleted:
                    if (selectedActivityDetail is CutSheetFinishedDetail selectedDeleted)
                    {
                        selectedDeleted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await Conn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new BaseException("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                default:
                    throw new BaseException("Unknown activity item status upon submitting.");
            }
        }

        /// <inheritdoc/>
        public override async Task Refresh()
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 2);

                var tableDetail = new TableActivityCutSheetFinishedDetail();
                await tableDetail.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableDetail.RegisterDataAccessListenerProgressOut(this);
                await tableDetail.RequestDetailData(this.ActItem.ItemFkRefno, this.ActItem.ItemRefno);
                tableDetail.UnregisterDataAccessListenerProgressOut(this);
            }
        }

        /// <inheritdoc/>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CutSheetFinishedDetail).ActivityDetailRefno;
        }

        /// <inheritdoc/>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CutSheetFinishedDetail).ActivityDetailFKRefno;
        }

        /// <inheritdoc/>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var itemRefno = this.ActItem.ItemRefno;
            const int statusNoChange = (int)SysCommon.ActStatus.NoChange;
            string sql;
            object[] parameters;

            if (isSubmitBatch)
            {
                sql = @"
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno AS LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode AS UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductDescription AS ProductDescription,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapSlot.LocationMapSlotCode AS LocationCode,
                           CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,
                           'Slot' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                      AND tActivityDetail.ActivityDetailStatus <> ?
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode,
                           tProduct.ProductRefno,
                           tProduct.ProductDescription,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapSection.LocationMapSectionCode AS LocationCode,
                           CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,
                           'Section' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                      AND tActivityDetail.ActivityDetailStatus <> ?
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode,
                           tProduct.ProductRefno,
                           tProduct.ProductDescription,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           'Whs' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                      AND tActivityDetail.ActivityDetailStatus <> ?
                    ORDER BY tActivityDetail.ActivityDetailRefno";
                parameters = new object[] { itemRefno, statusNoChange, itemRefno, statusNoChange, itemRefno, statusNoChange };
            }
            else
            {
                sql = @"
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno AS LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode AS UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode AS ProductCode,
                           tProduct.ProductRefno AS ProductRefno,
                           tProduct.ProductDescription AS ProductDescription,
                           tProductUnit.ProductUnitBC AS ProductUnitBC,
                           tProductUnit.ProductUnitIsBC AS ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription AS ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity AS ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity AS ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity AS ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate AS ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID AS ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID AS ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber AS ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber AS ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate AS ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate AS ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber AS ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapSlot.LocationMapSlotCode AS LocationCode,
                           CAST(tLocationMapSlot.LocationMapSlotName AS NVARCHAR(22)) AS LocationName,
                           'Slot' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode,
                           tProduct.ProductRefno,
                           tProduct.ProductDescription,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapSection.LocationMapSectionCode AS LocationCode,
                           CAST(tLocationMapSection.LocationMapSectionName AS NVARCHAR(22)) AS LocationName,
                           'Section' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                           tActivityDetail.LocationMapRefno,
                           tActivityDetailCutSheetFinished.ActivityDetailFKRefno,
                           tUnit.UnitCode,
                           aUnitWeight.UnitCode AS WtUnitCode,
                           aUnitBase.UnitCode AS BaseUnitCode,
                           tProduct.ProductCode,
                           tProduct.ProductRefno,
                           tProduct.ProductDescription,
                           tProductUnit.ProductUnitBC,
                           tProductUnit.ProductUnitIsBC,
                           tProductUnit.ProductUnitSizeDescription,
                           tActivityDetail.ActivityDetailQuantity,
                           tActivityDetail.ActivityDetailBaseQuantity,
                           tActivityDetail.ActivityDetailWeightQuantity,
                           tActivityDetail.ActivityDetailLogisticUnitBestBeforeSellByDate,
                           tActivityDetail.ActivityDetailSchemeID,
                           tActivityDetail.ActivityDetailUniqueSchemeID,
                           tActivityDetail.ActivityDetailSingleTradeItemSerialNumber,
                           tActivityDetail.ActivityDetailLogisticUnitBatchLotNumber,
                           tActivityDetail.ActivityDetailLogisticUnitExpiryUseByDate,
                           tActivityDetail.ActivityDetailLogisticUnitProductionDate,
                           tActivityDetail.ActivityDetailLogisticUnitSkidNumber,
                           tLocationMapWarehouse.LocationMapWarehouseCode AS LocationCode,
                           tLocationMapWarehouse.LocationMapWarehouseName AS LocationName,
                           'Whs' AS Location,
                           tProductSchemeVariant.ProductSchemeVariantCode,
                           tProductSchemeVariant.ProductSchemeVariantNo,
                           tProductSchemeVariant.ProductSchemeVariantDescription,
                           tActivityDetail.ActivityDetailStatus,
                           tActivityDetailExtra.ActivityDetailExtra1Text,
                           tActivityDetailExtra.ActivityDetailExtra2Text,
                           tActivityDetailExtra.ActivityDetailExtra3Text,
                           tActivityDetailExtra.ActivityDetailExtra4Text,
                           tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailCutSheetFinished
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailCutSheetFinished.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = ?
                    ORDER BY tActivityDetail.ActivityDetailRefno";
                parameters = new object[] { itemRefno, itemRefno, itemRefno };
            }

            this.FinishedDetails = await Conn.QueryAsync<CutSheetFinishedDetail>(sql, parameters);
            this.ListDetails = this.FinishedDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <inheritdoc/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var productionCutSheetFinishedDetail in this.ListDetails.Cast<CutSheetFinishedDetail>())
            {
                observableCollection.Add(productionCutSheetFinishedDetail);
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

            foreach (var productionCutSheetFinishedDetail in observableCollection.Cast<CutSheetFinishedDetail>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionCutSheetFinishedDetail.ActivityDetailQuantity:F3}");
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{productionCutSheetFinishedDetail.ActivityDetailWeightQuantity:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(productionCutSheetFinishedDetail.UnitCode);
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(productionCutSheetFinishedDetail.ActivityDetailSchemeID);

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

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateFinishedDetail"];

            return observableCollection;
        }

        /// <inheritdoc/>
        public override DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems)
        {
            mainActivityItems.Add(actDetail);

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

            return (DataTemplate)this.ResourceDictionary["dataTemplateFinishedDetail"];
        }

        /// <inheritdoc cref="ActivityDetail.Search(string)"/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.FinishedDetails.Where(adjDetail =>
                adjDetail.ActivityDetailSchemeID.ToUpper().Contains(searchText.ToUpper().Trim())).Cast<object>().ToList();

            foreach (var productionCutSheetFinishedDetail in this.ListDetails.Cast<CutSheetFinishedDetail>())
            {
                observableCollection.Add(productionCutSheetFinishedDetail);
            }

            return observableCollection;
        }

        /// <inheritdoc/>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var sb = new StringBuilder(256);
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = false }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("imclient");
                writer.WriteStartElement("command");
                if (data.Document.Root.Element("skid_data") == null)
                {
                    writer.WriteElementString("action", "SUBMIT CUT SHEET FINISHED DETAIL");
                    writer.WriteElementString("whs_id", "0");
                    writer.WriteElementString("sect_refno", "0");
                    writer.WriteEndElement(); // command

                    // Insert request defaults as raw XML
                    var requestDefaults = await SysCommon.GetRequestDefaults(false);
                    if (!string.IsNullOrEmpty(requestDefaults))
                    {
                        var defaultsDoc = XDocument.Parse("<root>" + requestDefaults + "</root>");
                        foreach (var elem in defaultsDoc.Root.Elements())
                        {
                            elem.WriteTo(writer);
                        }
                    }

                    // Insert detail defaults as raw XML
                    var detailDefaults = this.GetRequestDetailDefaults();
                    if (!string.IsNullOrEmpty(detailDefaults))
                    {
                        var defaultsDoc = XDocument.Parse("<root>" + detailDefaults + "</root>");
                        foreach (var elem in defaultsDoc.Root.Elements())
                        {
                            elem.WriteTo(writer);
                        }
                    }

                    writer.WriteElementString("skid_fk_refno", this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno.ToString());
                }
                else
                {
                    writer.WriteElementString("action", "SUBMIT CUT SHEET FINISHED DETAIL SKID");
                    writer.WriteElementString("whs_id", "0");
                    writer.WriteElementString("sect_refno", "0");
                    writer.WriteEndElement(); // command

                    var requestDefaults = await SysCommon.GetRequestDefaults(false);
                    if (!string.IsNullOrEmpty(requestDefaults))
                    {
                        var defaultsDoc = XDocument.Parse("<root>" + requestDefaults + "</root>");
                        foreach (var elem in defaultsDoc.Root.Elements())
                        {
                            elem.WriteTo(writer);
                        }
                    }

                    writer.WriteElementString("skid_data", data.Root.Element("skid_data").Value);
                    writer.WriteElementString("whs_id", this.LocMap.CurrLocMap.WhsFkRefno.ToString());
                    writer.WriteElementString("refno", this.ActItem.ActHdr.HdrFkRefno.ToString());
                    writer.WriteElementString("itm_refno", this.ActItem.ItemFkRefno.ToString());
                    writer.WriteElementString("is_negate_qty", "false");
                    writer.WriteElementString("extra1", this.ActExtra.Text1 ?? string.Empty);
                    writer.WriteElementString("extra2", this.ActExtra.Text2 ?? string.Empty);
                    writer.WriteElementString("extra3", this.ActExtra.Text3 ?? string.Empty);
                    writer.WriteElementString("extra4", this.ActExtra.Text4 ?? string.Empty);
                    writer.WriteElementString("extra5", this.ActExtra.DateTime1.ToString());
                }

                writer.WriteEndElement(); // imclient
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
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
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CUT SHEET FINISHED DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
                    + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                    + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                    + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                    + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                    + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE CUT SHEET FINISHED DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFkRefno + "</whs_id>"
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

        /// <inheritdoc/>
        public override string GetControlDescription()
        {
            return $"Finished # {this.ActItem.ActHdr.HdrFkRefno}";
        }

        /// <inheritdoc/>
        public override ActivityDetail GatherDetail()
        {
            CutSheetFinishedDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new CutSheetFinishedDetail
                {
                    // KLUDGE: Restore the new detail reference number's that got over written somehow.
                    ActivityDetailRefno = this.SysEnv.InsertedDetailRefno,
                    ActivityDetailFKRefno = this.SysEnv.InsertedDetailFkRefno,
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

        /// <inheritdoc/>
        public override void UpdateListDetail(int listDetailIdx)
        {
            var detail = (CutSheetFinishedDetail)this.ListDetails[listDetailIdx];
            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
        }

        /// <inheritdoc/>
        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
