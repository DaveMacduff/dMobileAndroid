// <copyright file="ReturnsActDetail.cs" company="InterSect Business Systems Inc">
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
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="ReturnsActDetail" />.
    /// </summary>
    public class ReturnsActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnsActDetail"/> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        public ReturnsActDetail(ActItem activityItem)
          : base(activityItem)
        {
            this.FullClassName = typeof(ReturnsActDetail).FullName;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
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

                var labelConfirmed = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelQuantity = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelWeightQuantity = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelUnitCode = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };
                var labelSchemeId = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start };

                labelConfirmed.SetBinding(Label.TextProperty, new Binding("ActivityDetailIsConfirmedString"));
                labelQuantity.SetBinding(Label.TextProperty, new Binding("ActivityDetailQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelWeightQuantity.SetBinding(Label.TextProperty, new Binding("ActivityDetailWeightQuantity", BindingMode.Default, null, null, "{0:F3}"));
                labelUnitCode.SetBinding(Label.TextProperty, new Binding("UnitCode"));
                labelSchemeId.SetBinding(Label.TextProperty, new Binding("ActivityDetailSchemeID"));

                if (this.SysEnv.SysSettings.IsRetConfirmDet)
                {
                    gridData.Children.Add(labelConfirmed);
                }

                gridData.Children.Add(labelQuantity);
                gridData.Children.Add(labelWeightQuantity);
                gridData.Children.Add(labelUnitCode);
                gridData.Children.Add(labelSchemeId);

                if (this.SysEnv.SysSettings.IsRetConfirmDet)
                {
                    Grid.SetColumn(labelConfirmed, 0);
                }

                Grid.SetColumn(labelQuantity, 1);
                Grid.SetColumn(labelWeightQuantity, 2);
                Grid.SetColumn(labelUnitCode, 3);
                Grid.SetColumn(labelSchemeId, 4);

                return new ViewCell { View = gridData };
            });

            var isAddKey = true;
            foreach (var item in this.ResourceDictionary)
            {
                if (item.Key == "dataTemplateReturnsDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplateReturnsDetail", dataTemplate);
            }
        }

        /// <summary>
        /// Gets or sets the grid column 0 width..
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 1 width..
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 2 width..
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 3 width..
        /// </summary>
        public double ColumnWidth3 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the grid column 4 width..
        /// </summary>
        public double ColumnWidth4 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary..
        /// </summary>
        public ResourceDictionary ResourceDictionary { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets the ListReturnDetails
        /// Gets or sets a list of picking details..
        /// </summary>
        public List<ReturnDetail> ListReturnDetails { get; set; } = new List<ReturnDetail>();

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
                _ = SysCommon.SubmitComm(await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>")));
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

            // Get temporary item FK refno for newly inserted line item.
            this.DetailFkRefno = await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(
                "SELECT MAX(tActivityDetailReturns.ActivityDetailFKRefno) FROM tActivityDetailReturns");
            this.DetailFkRefno++;

            // Insert the common activity detail data.
            this.DetailRefno = await this.InsertDetail();

            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Submit(this, data);
            }

            var query = "INSERT INTO tActivityDetailReturns (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)";
            var rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                query,
                Convert.ToInt32(this.DetailRefno),
                Convert.ToInt32(this.DetailFkRefno));
            if (rowsAffected <= 0)
            {
                throw new Exception("Unable to insert activity detail in disconnected mode.");
            }

            FindListItemAndSetFkRefno();

            /////
            void FindListItemAndSetFkRefno()
            {
                ReturnItem foundItem = null;
                foreach (ReturnItem item in this.ActItem.ListItems)
                {
                    if (item.ActivityItemRefno == this.ActItem.ItemRefno)
                    {
                        foundItem = item;
                        break;
                    }
                }

                // Also verify that line item reference number matches, just in case. If a FirstOrDefault
                // was used it could have returned an incorrect default reference.
                if (foundItem == null)
                {
                    throw new Exception("Unable to locate inserted list detail line item.");
                }
                else if (foundItem.ActivityItemRefno == this.ActItem.ItemRefno)
                {
                    foundItem.ActivityItemFKRefno = this.ActItem.ItemFkRefno;
                    foundItem.ActivityItemStringFKRefno = Convert.ToString(this.ActItem.ItemFkRefno);
                }
                else
                {
                    throw new Exception("Unable to locate inserted list detail line item.");
                }
            }
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                _ = SysCommon.SubmitComm(xmlDelete);
            }

            // delete row from DataView
            // TODO: Fix sorting
            // string oldSort = this.DataViewDetails.Sort;
            // this.DataViewDetails.Sort = "ActivityDetailFKRefno";
            // int rowIdx = this.DataViewDetails.Find(FKRefno);
            var returnDetail = this.ActItem.ListItems.OfType<ReturnDetail>().Where(returnDetail =>
                returnDetail.ActivityDetailFKRefno == this.DetailFkRefno).FirstOrDefault();

            if (this.ListDetails != null && returnDetail != null)
            {
                this.ListDetails.Remove(returnDetail);
            }

            // TODO: Fix sorting
            // this.DataViewDetails.Sort = oldSort;

            // delete from database
            var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailReturns WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

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
                    if (this.ActItem.SelectedItem is ReturnItem item)
                    {
                        item.ActivityItemFKRefno = this.ActItem.ItemFkRefno;
                    }

                    this.ActItem.ActStatus = SysCommon.ActStatus.NoChange;

                    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        "UPDATE tActivityItem SET ActivityItemStatus = ? WHERE ActivityItemRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.ActItem.ItemRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception(
                            "Unable to locate and update the activity line item with a new foreign" +
                            " key upon inserting.");
                    }

                    if (selectedActivityDetail is ReturnDetail selectedInserted)
                    {
                        selectedInserted.ActivityDetailFKRefno = this.DetailFkRefno;
                        selectedInserted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Updated:
                    if (selectedActivityDetail is ReturnDetail selectedUpdated)
                    {
                        selectedUpdated.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                        "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                        Convert.ToInt32(SysCommon.ActStatus.NoChange),
                        this.DetailRefno);

                    if (rowsAffected < 0)
                    {
                        throw new Exception("Unable to locate and update the activity detail status to no change.");
                    }

                    break;

                case SysCommon.ActStatus.Deleted:
                    if (selectedActivityDetail is ReturnDetail selectedDeleted)
                    {
                        selectedDeleted.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
                    }

                    rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
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

                var tableActReturnsDetail = new TableActivityReturnsDetail();
                await tableActReturnsDetail.PurgeData();

                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableActReturnsDetail.RegisterDataAccessListenerProgressOut((ReturnsActDetail)this);
                await tableActReturnsDetail.RequestDetailData(this.ActItem.ItemRefno);
                tableActReturnsDetail.UnregisterDataAccessListenerProgressOut((ReturnsActDetail)this);
            }
        }

        /// <summary>
        /// The GetRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as ReturnDetail).ActivityDetailRefno;
        }

        /// <summary>
        /// The GetFKRefno.
        /// </summary>
        /// <param name="selectedActivityDetail">The selectedActivityDetail<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as ReturnDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        /// Get a data view of the activity detail.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>Data view result of the query.</returns>
        public override async Task<List<object>> GetActivityDetailList(bool isSubmitBatch)
        {
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno = ");
                sql.Append("tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = ");
                sql.Append("tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = ");
                sql.Append("tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ");
                sql.Append("ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
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
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno AS LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' ");
                sql.Append("END AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text AS ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text AS ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text AS ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text AS ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date AS ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSlot ON tLocationMapSlot.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno = ");
                sql.Append("tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno = ");
                sql.Append("tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = ");
                sql.Append("tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ");
                sql.Append("ON tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("UNION ");
                sql.Append("SELECT tActivityDetail.ActivityDetailRefno,");
                sql.Append("tActivityDetail.LocationMapRefno,");
                sql.Append("tActivityDetailReturns.ActivityDetailFKRefno,");
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
                sql.Append("tActivityDetailReturns.ActivityDetailReturnsIsConfirmed AS ActivityDetailIsConfirmed,");
                sql.Append("CASE tActivityDetailReturns.ActivityDetailReturnsIsConfirmed WHEN 1 THEN \'C\' ELSE \' \' END ");
                sql.Append("AS ActivityDetailIsConfirmedString,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra1Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra2Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra3Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra4Text,");
                sql.Append("tActivityDetailExtra.ActivityDetailExtra5Date ");
                sql.Append("FROM tActivityDetailReturns ");
                sql.Append("INNER JOIN tActivityDetail ON tActivityDetail.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("INNER JOIN tProductUnit ON tProductUnit.ProductUnitRefno = tActivityDetail.ProductUnitRefno ");
                sql.Append("INNER JOIN tUnit ON tUnit.UnitRefno = tProductUnit.UnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitWeight ON aUnitWeight.UnitRefno = tProductUnit.WtUnitRefno ");
                sql.Append("INNER JOIN tUnit AS aUnitBase ON aUnitBase.UnitRefno = tProductUnit.BaseUnitRefno ");
                sql.Append("INNER JOIN tProduct ON tProduct.ProductRefno = tProductUnit.ProductRefno ");
                sql.Append("INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno ");
                sql.Append("= tActivityDetail.LocationMapRefno ");
                sql.Append("LEFT JOIN tActivityDetailProductSchemeVariantXRef ON ");
                sql.Append("tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno ");
                sql.Append("= tActivityDetailReturns.ActivityDetailRefno ");
                sql.Append("LEFT JOIN tProductSchemeVariant ON tProductSchemeVariant.ProductSchemeVariantRefno ");
                sql.Append("= tActivityDetailProductSchemeVariantXRef.ProductSchemeVariantRefno ");
                sql.Append("LEFT JOIN tActivityDetailExtra ON tActivityDetailExtra.ActivityDetailRefno ");
                sql.Append("= tActivityDetail.ActivityDetailRefno ");
                sql.AppendFormat("WHERE tActivityDetail.ActivityItemRefno = {0} ", this.ActItem.ItemRefno);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }

            this.ListReturnDetails = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<ReturnDetail>(sql.ToString());
            this.ListDetails = this.ListReturnDetails.Cast<object>().ToList();
            return this.ListDetails;
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

            foreach (ReturnDetail returnDetail in this.ListDetails)
            {
                observableCollection.Add(returnDetail);
            }

            var labelConfirmedHeader = new Label { Text = "C", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelQuantityHeader = new Label { Text = "Qty", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelWeightQuantityHeader = new Label { Text = "Weight", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelUnitCodeHeader = new Label { Text = string.Empty, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
            var labelSchemeIdHeader = new Label { Text = "Scheme ID", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };

            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelConfirmedHeader.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelQuantityHeader.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelWeightQuantityHeader.Text);
            this.ColumnWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(labelUnitCodeHeader.Text);
            this.ColumnWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(labelSchemeIdHeader.Text);

            // Header grid
            var gridHeaderReturnsActivityDetail = new Grid
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

            gridHeaderReturnsActivityDetail.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            if (this.SysEnv.SysSettings.IsRetConfirmDet)
            {
                gridHeaderReturnsActivityDetail.Children.Add(labelConfirmedHeader);
            }

            gridHeaderReturnsActivityDetail.Children.Add(labelQuantityHeader);
            gridHeaderReturnsActivityDetail.Children.Add(labelWeightQuantityHeader);
            gridHeaderReturnsActivityDetail.Children.Add(labelUnitCodeHeader);
            gridHeaderReturnsActivityDetail.Children.Add(labelSchemeIdHeader);

            if (this.SysEnv.SysSettings.IsRetConfirmDet)
            {
                Grid.SetColumn(labelConfirmedHeader, 0);
            }

            Grid.SetColumn(labelQuantityHeader, 1);
            Grid.SetColumn(labelWeightQuantityHeader, 2);
            Grid.SetColumn(labelUnitCodeHeader, 3);
            Grid.SetColumn(labelSchemeIdHeader, 4);

            Grid.SetRow(gridHeaderReturnsActivityDetail, 0);

            gridMain.Children.Add(gridHeaderReturnsActivityDetail);

            foreach (ReturnDetail returnDetail in observableCollection)
            {
                var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(returnDetail.ActivityDetailIsConfirmedString);
                var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{returnDetail.ActivityDetailQuantity:F3}");
                var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    $"{returnDetail.ActivityDetailWeightQuantity:F3}");
                var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(returnDetail.UnitCode);
                var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(returnDetail.ActivityDetailSchemeID);

                if (this.SysEnv.SysSettings.IsRetConfirmDet)
                {
                    if (detailWidth0 > this.ColumnWidth0)
                    {
                        this.ColumnWidth0 = detailWidth0;
                        gridHeaderReturnsActivityDetail.ColumnDefinitions[0].Width = this.ColumnWidth0;
                    }
                }
                else
                {
                    this.ColumnWidth0 = 0;
                    gridHeaderReturnsActivityDetail.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (detailWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = detailWidth1;
                    gridHeaderReturnsActivityDetail.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (detailWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = detailWidth2;
                    gridHeaderReturnsActivityDetail.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }

                if (detailWidth3 > this.ColumnWidth3)
                {
                    this.ColumnWidth3 = detailWidth3;
                    gridHeaderReturnsActivityDetail.ColumnDefinitions[3].Width = this.ColumnWidth3;
                }

                if (detailWidth4 > this.ColumnWidth4)
                {
                    this.ColumnWidth4 = detailWidth4;
                    gridHeaderReturnsActivityDetail.ColumnDefinitions[4].Width = this.ColumnWidth4;
                }
            }

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplateReturnsDetail"];

            return observableCollection;
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.ListReturnDetails.Where(returnDetail => returnDetail.ActivityDetailFKRefno.
                ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (ReturnDetail returnDetail in this.ListDetails)
            {
                observableCollection.Add(returnDetail);
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
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            XDocument request;
            if (this.ActItem.ActHdr.XmlDataCriteria == null)
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
                            + "<whs_id></whs_id>"
                            + "<sect_refno></sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
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
                        + "<action>SUBMIT SALES ORDER DETAIL SKID</action>"
                        + "<whs_id></whs_id>"
                        + "<sect_refno></sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
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
                            + "<action>SUBMIT SALES RETURN DETAIL</action>"
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
                            + "<action>SUBMIT SALES RETURN DETAIL</action>"
                            + "<whs_id>" + whsRefno + "</whs_id>"
                            + "<sect_refno>" + sectRefno + "</sect_refno>"
                            + "</command>"
                            + await SysCommon.GetRequestDefaults(false)
                            + this.GetRequestDetailDefaults()
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
                        + "<action>SUBMIT SALES RETURN DETAIL SKID</action>"
                        + "<whs_id>" + whsRefno + "</whs_id>"
                        + "<sect_refno>" + sectRefno + "</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + "<skid_data>" + sendData.Root.Element("skid_data").Value + "</skid_data>"
                        + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
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
                        + "<action>DELETE SALES RETURN DETAIL</action>"
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
                        + "<action>DELETE SALES RETURN DETAIL</action>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
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
                    + "<action>DELETE SALES RETURN DETAIL SKID</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
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
            return $"Return order # {this.ActItem.ActHdr.HdrFkRefno}";
        }

        /// <summary>
        /// Total all activity detail quantity from the data view.
        /// </summary>
        /// <returns>SFillQuantity structure.</returns>
        public override async Task<SFillQuantity> GetTotalQuantityFromListDetail()
        {
            var fillQty = new SFillQuantity();

            if (this.ListDetails != null && this.ListDetails.Count > 0)
            {
                foreach (ReturnDetail returnDetail in this.ListDetails)
                {
                    if (this.SysEnv.SysSettings.IsPickConfirmDet)
                    {
                        if (Convert.ToBoolean(returnDetail.ActivityDetailIsConfirmed) == true)
                        {
                            fillQty.Quantity += Convert.ToDecimal(returnDetail.ActivityDetailQuantity);
                            fillQty.BaseQuantity += Convert.ToDecimal(returnDetail.ActivityDetailBaseQuantity);
                            fillQty.WeightQuantity += Convert.ToDecimal(returnDetail.ActivityDetailWeightQuantity);
                        }
                    }
                    else
                    {
                        fillQty.Quantity += Convert.ToDecimal(returnDetail.ActivityDetailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(returnDetail.ActivityDetailBaseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(returnDetail.ActivityDetailWeightQuantity);
                    }
                }
            }

            return fillQty;
        }

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{object}"/> Main activity items.</param>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail InsertListDetail(ObservableCollection<object> mainActivityItems)
        {
            ReturnDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new ReturnDetail();
                detail.ActivityDetailRefno = this.DetailRefno;
                detail.ActivityDetailFKRefno = this.DetailFkRefno;

                this.InsertListDetailProduct(detail);
                this.InsertListDetailQuantity(detail);
                this.InsertListDetailScheme(detail);
                this.InsertListDetailExtra(detail);
                this.InsertListDetailLocation(detail);

                detail.ActivityDetailSchemeID = this.SchemeId;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeId;
                ////detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
                this.ListDetails.Add(detail);
            }

            return detail;
        }

        /// <summary>
        /// Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int"/> Zero based list detail index.</param>
        public override void UpdateListDetail(int listDetailIdx)
        {
            var detail = (ReturnDetail)this.ListDetails[listDetailIdx];
            if (detail.ActivityDetailStatus == (int)SysCommon.ActStatus.NoChange)
            {
                detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
            }

            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
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

            mainActivityItems.Add(actDetail);

            var detailWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                ((PickingDetail)actDetail).ActivityDetailIsConfirmedString);
            var detailWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailQuantity:F3}");
            var detailWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                $"{actDetail.ActivityDetailWeightQuantity:F3}");
            var detailWidth3 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.UnitCode);
            var detailWidth4 = DependencyService.Get<IGridService>().CalculateTextWidth(
                actDetail.ActivityDetailSchemeID);

            if (this.SysEnv.SysSettings.IsPickConfirmDet)
            {
                if (detailWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = detailWidth0;
                    gridMain.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }
            }
            else
            {
                this.ColumnWidth0 = 0;
                gridMain.ColumnDefinitions[0].Width = this.ColumnWidth0;
            }

            if (detailWidth1 > this.ColumnWidth1)
            {
                this.ColumnWidth1 = detailWidth1;
                gridMain.ColumnDefinitions[1].Width = this.ColumnWidth1;
            }

            if (detailWidth2 > this.ColumnWidth2)
            {
                this.ColumnWidth2 = detailWidth2;
                gridMain.ColumnDefinitions[2].Width = this.ColumnWidth2;
            }

            if (detailWidth3 > this.ColumnWidth3)
            {
                this.ColumnWidth3 = detailWidth3;
                gridMain.ColumnDefinitions[3].Width = this.ColumnWidth3;
            }

            if (detailWidth4 > this.ColumnWidth4)
            {
                this.ColumnWidth4 = detailWidth4;
                gridMain.ColumnDefinitions[4].Width = this.ColumnWidth4;
            }

            return (DataTemplate)this.ResourceDictionary["dataTemplateReturnsDetail"];
        }
    }
}
