// <copyright file="PickingActDetail.cs" company="InterSect Business Systems Inc">
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
using Xamarin.Forms.Xaml;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Create the picking activity detail class.
    /// </summary>
    public class PickingActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingActDetail"/> class.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item instance.</param>
        public PickingActDetail(ActItem actItem)
          : base(actItem)
        {
            this.FullClassName = typeof(PickingActDetail).FullName;

            var dataTemplate = new DataTemplate(
                () =>
                {
                    var gridData = new Grid
                    {
                        Padding = new Thickness(5),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.ColumnWidth3, GridUnitType.Absolute) },
                            new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                        },
                    };

                    var labelPickingDetailConfirmed = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                    };

                    var labelPickingQuantity = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                    };

                    var labelPickingWeightQuantity = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                    };

                    var labelPickingUnitCode = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                    };

                    var labelPickingSchemeId = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start,
                    };

                    labelPickingDetailConfirmed.SetBinding(Label.TextProperty, new Binding("ActivityDetailIsConfirmedString"));
                    labelPickingQuantity.SetBinding(Label.TextProperty, new Binding(
                        "ActivityDetailQuantity",
                        BindingMode.Default,
                        null,
                        null,
                        "{0:F3}"));
                    labelPickingWeightQuantity.SetBinding(Label.TextProperty, new Binding(
                        "ActivityDetailWeightQuantity",
                        BindingMode.Default,
                        null,
                        null,
                        "{0:F3}"));
                    labelPickingUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                    labelPickingSchemeId.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        gridData.Children.Add(labelPickingDetailConfirmed);
                    }

                    gridData.Children.Add(labelPickingQuantity);
                    gridData.Children.Add(labelPickingWeightQuantity);
                    gridData.Children.Add(labelPickingUnitCode);
                    gridData.Children.Add(labelPickingSchemeId);

                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        Grid.SetColumn(labelPickingDetailConfirmed, 0);
                    }

                    Grid.SetColumn(labelPickingQuantity, 1);
                    Grid.SetColumn(labelPickingWeightQuantity, 2);
                    Grid.SetColumn(labelPickingUnitCode, 3);
                    Grid.SetColumn(labelPickingSchemeId, 4);

                    return new ViewCell { View = gridData };
                });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplatePickingDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplatePickingDetail", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width.
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width.
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width.
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width.
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets a list of picking details.
        /// </summary>
        public List<PickingDetail> ListPickingDetails { get; set; } = new List<PickingDetail>();

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail GatherDetail()
        {
            PickingDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new PickingDetail
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

        /// <summary>
        /// Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int"/> Zero based list detail index.</param>
        public override async void UpdateListDetail(int listDetailIdx)
        {
            await Task.Run(() =>
            {
                var detail = (PickingDetail)this.ListDetails[listDetailIdx];
                detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
                detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
                detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
            });
        }

        /// <summary>
        /// Perform the insert skid call to the comm server, and fill the this activity detail instance.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task InsertSkid(SysSkidInfo skidInfo)
        {
            var request = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                <params>
                <skid_data>{skidInfo.CurrSkid.SkidCode}</skid_data>
                </params>");

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
                string xmlDelete;
                xmlDelete = await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));

                _ = SysCommon.SubmitComm(xmlDelete);
            }
        }

        /// <summary>
        /// Update the confirmed status of the picking detail in the database.
        /// </summary>
        /// <param name="listDetailIdx">List index.</param>
        public async void UpdateConfirmedStatus(int listDetailIdx)
        {
            var detail = (PickingDetail)this.ListDetails[listDetailIdx];
            var query = $@"UPDATE tActivityDetailPicking
                SET ActivityDetailPickingIsConfirmed = {detail.ActivityDetailIsConfirmed}
                WHERE ActivityDetailRefno = {detail.ActivityDetailRefno}";
            await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(query);
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data)
        {
            await this.FillActDetailObjFromXml(data);

            // Get temporary item FK refno for newly inserted line item.
            this.DetailFkRefno = await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(
                "SELECT MAX(tActivityDetailPicking.ActivityDetailFKRefno) FROM tActivityDetailPicking");
            this.DetailFkRefno++;

            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Submit(this, data);
            }

            // In order to support base tracking the DetailFKRefno is the itm_refno rather than the serial.refno.
            this.DetailRefno = this.FindListDetailRefno(this.DetailFkRefno);
            if (this.DetailRefno < 0)
            {
                this.DetailRefno = await this.InsertDetailIntoTablesDetail();
                const string query = @"
                    INSERT INTO tActivityDetailPicking (
                            ActivityDetailRefno,
                            ActivityDetailFKRefno,
                            ActivityDetailPickingIsConfirmed)
                        VALUES
                            (?, ?, ?)";
                var rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                    query,
                    Convert.ToInt32(this.DetailRefno),
                    Convert.ToInt32(this.DetailFkRefno),
                    true);
                if (rowsAffected <= 0)
                {
                    throw new Exception("Unable to insert activity detail in disconnected mode.");
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
            var conn = DatabaseConnection.GetInstance().SqlConn;
            await conn.RunInTransactionAsync(DeleteFromTables);

            return 0;

            //////////////////////////////////////////////////////////////////////////
            async void DeleteFromTables(SQLiteConnection delete)
            {
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?",
                    this.DetailRefno);
                await conn.ExecuteAsync(
                    "DELETE FROM tActivityDetailPicking WHERE ActivityDetailRefno = ?",
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
                    if (this.ActItem.SelectedItem is PickingItem item)
                    {
                        item.ActivityItemFKRefno = this.ActItem.ItemFkRefno;
                        item.ActivityItemStringFKRefno = this.ActItem.ItemFkRefno.ToString();

                        rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                            @"UPDATE tActivityItemPicking
                                SET ActivityItemFKRefno = ?
                                    ,ActivityItemStringFKRefno = ?
                                WHERE ActivityItemRefno = ?",
                            item.ActivityItemFKRefno,
                            item.ActivityItemStringFKRefno,
                            this.ActItem.ItemRefno);
                        if (rowsAffected < 0)
                        {
                            throw new Exception(
                                "Unable to locate and update the picking activity line item with a new foreign" +
                                " key upon inserting.");
                        }
                    }

                    this.ActItem.ActStatus = SysCommon.ActStatus.NoChange;

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                        "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.ActItem.ItemRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception(
                            "Unable to locate and update the activity line item with a new foreign" +
                            " key upon inserting.");
                    }

                    if (selectedActivityDetail is PickingDetail selectedInserted)
                    {
                        selectedInserted.ActivityDetailFKRefno = this.DetailFkRefno;
                        selectedInserted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is PickingDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Deleted:
                    if (selectedActivityDetail is PickingDetail selectedDeleted)
                    {
                        selectedDeleted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
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
                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is AdjustmentDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.Updated);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
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

                    rowsAffected = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
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
            var tableActPickingDetail = new TableActivityPickingDetail();
            tableActPickingDetail.RegisterDataAccessListenerProgressOut(this);
            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 1);
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await tableActPickingDetail.PurgeData();
            }

            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 3);
            await tableActPickingDetail.RequestDetailData(this.ActItem.ItemFkRefno, this.ActItem.ItemRefno);
            this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);
            tableActPickingDetail.UnregisterDataAccessListenerProgressOut(this);
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as PickingDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as PickingDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get a data view of the activity detail.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>Data view result of the query.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var sql = string.Empty;

            if (isSubmitBatch)
            {
                sql = $@"SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno AS LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode AS UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode AS ProductCode,
                    tProduct.ProductRefno AS ProductRefno,
                    tProduct.ProductDescription AS ProductDescription,
                    tProduct.ProductBrandDescription AS ProductBrandDescription,
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
                    CAST('Slot' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END 
                    AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date 
                    FROM tActivityDetailPicking 
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno 
                    = tActivityDetailPicking.ActivityDetailRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno 
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno 
                    = tActivityDetail.LocationMapRefno 
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON 
                    tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = 
                    tActivityDetailPicking.ActivityDetailRefno 
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno 
                    = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno 
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno 
                    = tActivityDetail.ActivityDetailRefno 
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno} 
                    AND tActivityDetail.ActivityDetailStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}
                    UNION 
                    SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode,
                    tProduct.ProductRefno,
                    tProduct.ProductDescription,
                    tProduct.ProductBrandDescription,
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
                    CAST('Section' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END 
                    AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date 
                    FROM tActivityDetailPicking 
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno 
                    = tActivityDetailPicking.ActivityDetailRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno 
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno 
                    = tActivityDetail.LocationMapRefno 
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON 
                    tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno 
                    = tActivityDetailPicking.ActivityDetailRefno 
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno 
                    = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno 
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno 
                    = tActivityDetail.ActivityDetailRefno 
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno} 
                    AND tActivityDetail.ActivityDetailStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}
                    UNION 
                    SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode,
                    tProduct.ProductRefno,
                    tProduct.ProductDescription,
                    tProduct.ProductBrandDescription,
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
                    CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,
                    CAST('Whs' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END 
                    AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date 
                    FROM tActivityDetailPicking 
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno 
                    = tActivityDetailPicking.ActivityDetailRefno 
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno 
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno 
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno 
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno 
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno 
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno 
                    = tActivityDetail.LocationMapRefno 
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON 
                    tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno 
                    = tActivityDetailPicking.ActivityDetailRefno 
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno 
                    = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno 
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno 
                    = tActivityDetail.ActivityDetailRefno 
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno} 
                    AND tActivityDetail.ActivityDetailStatus <> {Convert.ToInt32(SysCommon.ActStatus.NoChange)}
                    ORDER BY tActivityDetail.ActivityDetailRefno";
            }
            else
            {
                sql = $@"
                    SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno AS LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode AS UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode AS ProductCode,
                    tProduct.ProductRefno AS ProductRefno,
                    tProduct.ProductDescription AS ProductDescription,
                    tProduct.ProductBrandDescription AS ProductBrandDescription,
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
                    CAST('Slot' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode AS ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo AS ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription AS ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus AS ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date
                    FROM tActivityDetailPicking
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno}
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode,
                    tProduct.ProductRefno,
                    tProduct.ProductDescription,
                    tProduct.ProductBrandDescription,
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
                    CAST('Section' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailPicking
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno}
                    UNION
                    SELECT tActivityDetail.ActivityDetailRefno,
                    tActivityDetail.LocationMapRefno,
                    tActivityDetailPicking.ActivityDetailFKRefno,
                    tUnit.UnitCode,
                    aUnitWeight.UnitCode AS WtUnitCode,
                    aUnitBase.UnitCode AS BaseUnitCode,
                    tProduct.ProductCode,
                    tProduct.ProductRefno,
                    tProduct.ProductDescription,
                    tProduct.ProductBrandDescription,
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
                    CAST(tLocationMapWarehouse.LocationMapWarehouseName AS NVARCHAR(22)) AS LocationName,
                    CAST('Whs' AS NVARCHAR(8)) AS Location,
                    tProductSchemeVariant.ProductSchemeVariantCode,
                    tProductSchemeVariant.ProductSchemeVariantNo,
                    tProductSchemeVariant.ProductSchemeVariantDescription,
                    tActivityDetail.ActivityDetailStatus,
                    tActivityDetailPicking.ActivityDetailPickingIsConfirmed AS ActivityDetailIsConfirmed,
                    CASE tActivityDetailPicking.ActivityDetailPickingIsConfirmed WHEN 1 THEN 'C' ELSE ' ' END AS ActivityDetailIsConfirmedString,
                    tActivityDetailExtra.ActivityDetailExtra1Text,
                    tActivityDetailExtra.ActivityDetailExtra2Text,
                    tActivityDetailExtra.ActivityDetailExtra3Text,
                    tActivityDetailExtra.ActivityDetailExtra4Text,
                    tActivityDetailExtra.ActivityDetailExtra5Date
                    FROM tActivityDetailPicking
                    INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno
                    INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno
                    INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno
                    INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno
                    INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno
                    INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tActivityDetail.LocationMapRefno
                    LEFT JOIN tActivityDetailProductSchemeVariantXRef ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = tActivityDetailPicking.ActivityDetailRefno
                    LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno = tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno
                    LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno = tActivityDetail.ActivityDetailRefno
                    WHERE tActivityDetail.ActivityItemRefno = {this.ActItem.ItemRefno}
                    ORDER BY tActivityDetail.ActivityDetailRefno";
            }

            this.ListPickingDetails = await DatabaseConnection.GetInstance().SqlConn.QueryAsync<PickingDetail>(sql);
            this.ListDetails = this.ListPickingDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Activity grid.</param>
        /// <param name="listViewMain"><see cref="ListView"/> Main activity.</param>
        /// <returns><see cref="ObservableCollection(object)"/> Observable collection.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMain,
            ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (var pickingDetail in this.ListDetails.Cast<PickingDetail>())
            {
                observableCollection.Add(pickingDetail);
            }

            var labelPickingDetailConfirmedHeader = new Label
            {
                Text = "C",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingQuantityHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingWeightQuantityHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingSchemeIdHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDetailConfirmedHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingQuantityHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingWeightQuantityHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingSchemeIdHeader.Text);

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
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                gridHeader.Children.Add(labelPickingDetailConfirmedHeader);
            }

            gridHeader.Children.Add(labelPickingQuantityHeader);
            gridHeader.Children.Add(labelPickingWeightQuantityHeader);
            gridHeader.Children.Add(labelPickingUnitCodeHeader);
            gridHeader.Children.Add(labelPickingSchemeIdHeader);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                Grid.SetColumn(labelPickingDetailConfirmedHeader, 0);
            }

            Grid.SetColumn(labelPickingQuantityHeader, 1);
            Grid.SetColumn(labelPickingWeightQuantityHeader, 2);
            Grid.SetColumn(labelPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelPickingSchemeIdHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            foreach (var pickingDetail in observableCollection.Cast<PickingDetail>())
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    pickingDetail.ActivityDetailIsConfirmedString);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{pickingDetail.ActivityDetailQuantity:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{pickingDetail.ActivityDetailWeightQuantity:F3}");
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(pickingDetail.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    pickingDetail.ActivityDetailSchemeID);

                if (this.SysEnv.SysSettings.IsPickConfirmDet)
                {
                    if (detailWidth0 > this.ColumnWidth0)
                    {
                        this.ColumnWidth0 = detailWidth0;
                        gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                    }
                }
                else
                {
                    this.ColumnWidth0 = 0;
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

                if (detailWidth4 > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidth4;
                    gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
                }
            }

            ////var template = new DataTemplate();
            ////template.LoadFromXaml(@"<ViewCell xmlns=""http://xamarin.com/schemas/2014/forms""
            ////    xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"" 
            ////    x:Class=""ImfMobile.Views.PickingDetailTemplate"">
            ////    <Grid Padding=""5"">
            ////        <Grid.ColumnDefinitions>
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[0].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[1].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[2].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[3].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[4].Width}"" />
            ////        </Grid.ColumnDefinitions>
            ////        <Label Grid.Column=""0"" Text=""{Binding ActivityDetailIsConfirmedString}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""1"" Text=""{Binding ActivityDetailQuantity:F3}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""2"" Text=""{Binding ActivityDetailWeightQuantity:F3}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""3"" Text=""{Binding UnitCode}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""4"" Text=""{Binding ActivityDetailSchemeID}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////    </Grid>)");
            ////listViewMain.ItemTemplate = template;

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplatePickingDetail"];

            return observableCollection;
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
            var labelPickingDetailConfirmedHeader = new Label
            {
                Text = "C",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingQuantityHeader = new Label
            {
                Text = "Qty",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingWeightQuantityHeader = new Label
            {
                Text = "Weight",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingUnitCodeHeader = new Label
            {
                Text = string.Empty,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            var labelPickingSchemeIdHeader = new Label
            {
                Text = "Scheme ID",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = FontAttributes.Bold,
            };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingDetailConfirmedHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingQuantityHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingWeightQuantityHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelPickingSchemeIdHeader.Text);

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
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth4, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                gridHeader.Children.Add(labelPickingDetailConfirmedHeader);
            }

            gridHeader.Children.Add(labelPickingQuantityHeader);
            gridHeader.Children.Add(labelPickingWeightQuantityHeader);
            gridHeader.Children.Add(labelPickingUnitCodeHeader);
            gridHeader.Children.Add(labelPickingSchemeIdHeader);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                Grid.SetColumn(labelPickingDetailConfirmedHeader, 0);
            }

            Grid.SetColumn(labelPickingQuantityHeader, 1);
            Grid.SetColumn(labelPickingWeightQuantityHeader, 2);
            Grid.SetColumn(labelPickingUnitCodeHeader, 3);
            Grid.SetColumn(labelPickingSchemeIdHeader, 4);

            Grid.SetRow(gridHeader, 0);

            gridMain.Children.Add(gridHeader);

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                ((PickingDetail)actDetail).ActivityDetailIsConfirmedString);
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailQuantity:F3}");
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailWeightQuantity:F3}");
            var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(actDetail.UnitCode);
            var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.ActivityDetailSchemeID);
            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }
            }
            else
            {
                this.ColumnWidth0 = 0;
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

            if (detailWidth4 > this.ColumnWidth4)
            {
                this.ColumnWidth4 = detailWidth4;
                gridHeader.ColumnDefinitions[4].Width = this.ColumnWidth4;
            }

            ////var template = new DataTemplate();
            ////template.LoadFromXaml(@"<ViewCell xmlns=""http://xamarin.com/schemas/2014/forms"" 
            ////    xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"" 
            ////    x:Class=""ImfMobile.Views.PickingDetailTemplate"">
            ////    <Grid Padding=""5"">
            ////        <Grid.ColumnDefinitions>
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[0].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[1].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[2].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[3].Width}"" />
            ////            <ColumnDefinition Width=""{Binding Source={x:Reference Name=gridHeader}, Path=ColumnDefinitions[4].Width}"" />
            ////        </Grid.ColumnDefinitions>
            ////        <Label Grid.Column=""0"" Text=""{Binding ActivityDetailIsConfirmedString}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""1"" Text=""{Binding ActivityDetailQuantity:F3}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""2"" Text=""{Binding ActivityDetailWeightQuantity:F3}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""3"" Text=""{Binding UnitCode}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <Label Grid.Column=""4"" Text=""{Binding ActivityDetailSchemeID}"" VerticalOptions=""Center"" HorizontalOptions=""Start"" />
            ////        <ViewCell>
            ////            <StackLayout BackgroundColor=""{Binding BackgroundColor}"">
            ////                <!-- Your cell content here -->
            ////                <Label Text=""{Binding Title}"" />
            ////            </StackLayout>
            ////        </ViewCell>
            ////    </Grid>)");

            return (DataTemplate)this.ResourceDictionary["dataTemplatePickingDetail"];
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.ListPickingDetails.Where(adjDetail =>
                adjDetail.ActivityDetailSchemeID.ToUpper().Contains(searchText.ToUpper().Trim())).Cast<object>().ToList();

            foreach (var pickingDetail in this.ListDetails.Cast<PickingDetail>())
            {
                observableCollection.Add(pickingDetail);
            }

            return observableCollection;
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="sendData">.</param>
        /// <returns>.</returns>
        public override async Task<string> CreateSubmitXml(XDocument sendData)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (this.ActItem.ActHdr.XmlDataCriteria == null)
            {
                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            @$"<?xml version = ""1.0"" encoding = ""utf - 16"" standalone = ""yes"" ?>
                            <imclient>
                                <command>
                                    <action>SUBMIT SALES ORDER DETAIL</action>
                                    <whs_id></whs_id>
                                    <sect_refno></sect_refno>
                                </command>
                                {await SysCommon.GetRequestDefaults(false)}
                                {this.GetRequestDetailDefaults()}
                                <skid_fk_refno>0</skid_fk_refno>
                            </imclient>");
                    }
                    else
                    {
                        request = XDocument.Parse(
                            @$"<?xml version = ""1.0"" encoding = ""utf - 16"" standalone = ""yes"" ?>
                            <imclient>
                                <command>
                                    <action>SUBMIT SALES ORDER DETAIL</action>
                                    <whs_id></whs_id>
                                    <sect_refno></sect_refno>
                                </command>
                                {await SysCommon.GetRequestDefaults(false)}
                                {this.GetRequestDetailDefaults()}
                                <skid_fk_refno>{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno}</skid_fk_refno>
                            </imclient>");
                    }
                }
                else
                {
                    request = XDocument.Parse(@$"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                        <imclient>
                            <command>
                                <action>SUBMIT SALES ORDER DETAIL SKID</action>
                                <whs_id></whs_id>
                                <sect_refno></sect_refno>
                            </command>
                            {await SysCommon.GetRequestDefaults(false)}
                            <skid_data>{sendData.Root.Element("skid_data").Value}</skid_data>
                            <whs_id>{this.LocMap.CurrLocMap.WhsFkRefno}</whs_id>
                            <refno>{this.ActItem.ActHdr.HdrFkRefno}</refno>
                            <itm_refno>{this.ActItem.ItemFkRefno}</itm_refno>
                            <is_negate_qty>false</is_negate_qty>
                            <extra1>{this.ActExtra.Text1}</extra1>
                            <extra2>{this.ActExtra.Text2}</extra2>
                            <extra3>{this.ActExtra.Text3}</extra3>
                            <extra4>{this.ActExtra.Text4}</extra4>
                            <extra5>{this.ActExtra.DateTime1}</extra5>
                        </imclient>");
                }
            }
            else
            {
                var whsRefno = Convert.ToInt32(this.ActItem.ActHdr.XmlDataCriteria.Root.Element("whs_id").Value);
                var sectRefno = Convert.ToInt32(this.ActItem.ActHdr.XmlDataCriteria.Root.Element("sect_refno").Value);

                if (sendData.Document.Root.Element("skid_data") == null)
                {
                    if (this.SysEnv.SysSettings.IsDisconnectedMode)
                    {
                        request = XDocument.Parse(
                            "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                            + "<imclient>"
                            + "<command>"
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id>" + whsRefno + "</whs_id>"
                            + "<sect_refno>" + sectRefno + "</sect_refno>"
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
                            + "<action>SUBMIT SALES ORDER DETAIL</action>"
                            + "<whs_id>" + whsRefno + "</whs_id>"
                            + "<sect_refno>" + sectRefno + "</sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
                            + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
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
                        + "<whs_id>" + whsRefno + "</whs_id>"
                        + "<sect_refno>" + sectRefno + "</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFkRefno + "</whs_id>"
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
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>DELETE SALES ORDER DETAIL</action>"
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
                        + "<action>DELETE SALES ORDER DETAIL</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFkRefno + "</skid_fk_refno>"
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

        /// <summary>
        /// Total all activity detail quantity from the data view.
        /// </summary>
        /// <returns>SFillQuantity structure.</returns>
        public override Task<SFillQuantity> GetTotalQuantityFromListDetail()
        {
            var fillQty = new SFillQuantity();

            if (this.ListDetails != null && this.ListDetails.Count > 0)
            {
                foreach (var pickingDetail in this.ListDetails.Cast<PickingDetail>())
                {
                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        if (Convert.ToBoolean(pickingDetail.ActivityDetailIsConfirmed) == true)
                        {
                            fillQty.Quantity += Convert.ToDecimal(pickingDetail.ActivityDetailQuantity);
                            fillQty.BaseQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailBaseQuantity);
                            fillQty.WeightQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailWeightQuantity);
                        }
                    }
                    else
                    {
                        fillQty.Quantity += Convert.ToDecimal(pickingDetail.ActivityDetailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailBaseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(pickingDetail.ActivityDetailWeightQuantity);
                    }
                }
            }

            return Task.FromResult(fillQty);
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            ////string prod = $"Prod: {this.ActItem.ProdInfo.Code} {this.ActItem.ProdInfo.UnitBC}
            ////{this.ActItem.ProdInfo.UnitCode}";
            ////return prod;

            return $"Pick # {this.ActItem.ActHdr.HdrFkRefno}";
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }
    }
}
