// <copyright file="PhysicalCountActDetail.cs" company="InterSect Business Systems Inc">
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
    public class PhysicalCountActDetail : ActDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalCountActDetail"/> class.
        /// </summary>
        /// <param name="activityItem"></param>
        public PhysicalCountActDetail(ActItem activityItem)
          : base(activityItem)
        {
            this.FullClassName = typeof(PhysicalCountActDetail).FullName;

            // Data grid
            var dataTemplate = new DataTemplate(() =>
            {
                // Header grid
                var gridData = new Grid()
                {
                    Padding = new Thickness(5),

                    // ColumnSpacing = 15,
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
                if (item.Key == "dataTemplatePhysicalCountDetail")
                {
                    isAddKey = false;
                }
            }

            if (isAddKey)
            {
                this.ResourceDictionary.Add("dataTemplatePhysicalCountDetail", dataTemplate);
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
        /// Gets or sets a list of count details.
        /// </summary>
        public List<CountDetail> ListCountDetails { get; set; } = new List<CountDetail>();

        /// <summary>
        /// Find or insert an activity detail into the list details.
        /// </summary>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{object}"/> Main activity items.</param>
        /// <returns><see cref="ActivityDetail"/> Activity detail.</returns>
        public override ActivityDetail InsertListDetail(ObservableCollection<object> mainActivityItems)
        {
            CountDetail detail = null;
            if (this.ListDetails != null)
            {
                detail = new CountDetail();
                detail.ActivityDetailRefno = this.DetailRefno;
                detail.ActivityDetailFKRefno = this.DetailFkRefno;

                this.InsertListDetailProduct(detail);
                this.InsertListDetailQuantity(detail);
                this.InsertListDetailScheme(detail);
                this.InsertListDetailExtra(detail);
                this.InsertListDetailLocation(detail);

                detail.ActivityDetailSchemeID = this.SchemeId;
                detail.ActivityDetailUniqueSchemeID = this.UniqueSchemeId;
                detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Inserted;
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
            var detail = (CountDetail)this.ListDetails[listDetailIdx];
            if (detail.ActivityDetailStatus == (int)SysCommon.ActStatus.NoChange)
            {
                detail.ActivityDetailStatus = (int)SysCommon.ActStatus.Updated;
            }

            detail.ActivityDetailQuantity += Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity += Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity += Math.Round(this.WeightQuantity, 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skidInfo"></param>
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
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);
            this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);
            this.ActItem.ItemFkRefno = int.Parse(xmlDoc.DocumentElement["itm_refno"].InnerText);
            this.Quantity = decimal.Parse(xmlDoc.DocumentElement["alt_qty"].InnerText);
            this.BaseQuantity = decimal.Parse(xmlDoc.DocumentElement["base_qty"].InnerText);
            this.WeightQuantity = decimal.Parse(xmlDoc.DocumentElement["wt_qty"].InnerText);
            this.IsUpdateView = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="skidInfo"></param>
        /// <inheritdoc/>
        public override async Task DeleteSkid(SysSkidInfo skidInfo)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var xmlDelete = await this.CreateDeleteXml(XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
                _ = SysCommon.SubmitComm(xmlDelete);
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

            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                // At this point the 
                var actDetailRefno = await this.InsertDetailDisconnected();

                var query = "INSERT INTO tActivityDetailCount (ActivityDetailRefno, ActivityDetailFKRefno) VALUES (?, ?)";
                var rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    query,
                    Convert.ToInt32(actDetailRefno),
                    Convert.ToInt32(this.DetailFkRefno));
                if (rowsAffected <= 0)
                {
                    throw new Exception("Unable to insert activity detail in disconnected mode.");
                }

                this.IsUpdateView = true;
            }
            else
            {
                this.SubmitDetailToCommServer(await this.CreateSubmitXml(XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>")));
                if (this.DetailRefno >= 0)
                {
                    throw new Exception("Unexpected submit detail to comm server returned a detail reference number" +
                        " greater than or equal to zero.");
                }

                var query = "UPDATE tActivityItemCount SET ActivityItemStringFKRefno = ?," +
                            " ActivityItemFKRefno = ? WHERE ActivityItemRefno = ?";
                var rowsAffected = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    query,
                    Convert.ToString(this.ActItem.ItemFkRefno),
                    this.ActItem.ItemFkRefno,
                    this.ActItem.ItemRefno);
                if (rowsAffected <= 0)
                {
                    throw new Exception("Unable to locate and update the activity line item with a new foreign" +
                        " key upon inserting.");
                }

                // Find the activity item that may also have been newly created, and update the line item reference
                // returned from the communication server.
                // Do not use FirstOrDefault because we do not want to overwrite a wrongly matched item.
                // Linq returns new references only for base types (int, string, double etc...).
                // The IEnumerable will be same instance of initial list, you have to call ToList or ToArray
                // to create a new collection.
                // Linq is slower than a for loop, but the number of line items would likely be sub 500 normally.
                CountItem foundItem = null;
                foreach (CountItem item in this.ActItem.ListItems)
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
        ///
        /// </summary>
        /// <inheritdoc/>
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
            // int rowIdx = this.DataViewDetails.Find(this.FKRefno);
            var countDetail = this.ListDetails.OfType<CountDetail>().Where(countDetail => countDetail.ActivityDetailFKRefno == this.DetailFkRefno).FirstOrDefault();

            if (countDetail != null)
            {
                this.ListDetails.Remove(countDetail);
            }

            // TODO: Fix sorting
            // this.DataViewDetails.Sort = oldSort;

            // Delete from database
            var query = "DELETE FROM tActivityDetailProductSchemeVariantXRef WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailCount WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetailExtra WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            query = "DELETE FROM tActivityDetail WHERE ActivityDetailRefno = ?";
            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(query, this.DetailRefno);

            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <param name="data"></param>
        /// <inheritdoc/>
        public override async Task Submit(object selectedActivityDetail, XDocument data)
        {
            var xmlDoc = new XmlDocument();
            var xmlSend = await this.CreateSubmitXml(XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><params></params>"));
            var responseData = SysCommon.SubmitComm(xmlSend);
            xmlDoc.LoadXml(responseData);
            this.DetailFkRefno = int.Parse(xmlDoc.DocumentElement["ser_refno"].InnerText);

            if (selectedActivityDetail is CountDetail countDetail)
            {
                countDetail.ActivityDetailFKRefno = this.DetailFkRefno;
                countDetail.ActivityDetailStatus = Convert.ToInt32(SysCommon.ActStatus.NoChange);
            }

            await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                "UPDATE tActivityDetail SET ActivityDetailStatus = ? WHERE ActivityDetailRefno = ?",
                Convert.ToInt32(SysCommon.ActStatus.NoChange),
                this.DetailRefno);
        }

        /// <summary>
        ///
        /// </summary>
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

                var tableCountDet = new TableActivityPhysicalCountDetail();
                await tableCountDet.PurgeData();
                this.DataLayerProgressOutHandler(this, EventArgs.Empty, 5);

                tableCountDet.RegisterDataAccessListenerProgressOut((PhysicalCountActDetail)this);
                await tableCountDet.RequestDetailData(this.ActItem.ItemRefno);
                tableCountDet.UnregisterDataAccessListenerProgressOut((PhysicalCountActDetail)this);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override int GetRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailRefno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override int GetFkRefno(object selectedActivityDetail)
        {
            return (selectedActivityDetail as CountDetail).ActivityDetailFKRefno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <inheritdoc/>
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.AppendFormat("AND tActivityDetail.ActivityDetailStatus <> {0} ", (int)SysCommon.ActStatus.NoChange);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }
            else
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
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
                sql.AppendFormat("AND tActivityDetail.ActivityDetailSubType = {0} ", (int)SysCommon.ActSubTypes.PhysicalCounts);
                sql.Append("ORDER BY tActivityDetail.ActivityDetailRefno");
            }

            this.ListCountDetails = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<CountDetail>(sql.ToString());
            this.ListDetails = this.ListCountDetails.Cast<object>().ToList();
            return this.ListDetails;
        }

        /// <inheritdoc/>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain)
        {
            var observableCollection = new ObservableCollection<object>();

            foreach (CountDetail countDetail in this.ListDetails)
            {
                // CountDetail countDetail = new CountDetail
                // {
                //    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                //    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                //    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                //    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                //    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                //    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                //    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                //    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                //    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                //    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                //    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                //    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                //    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                //    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                //    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null)
                // };

                // if (DateTime.TryParse(Convert.ToString(row["ActivityDetailExtra5Date"]), out _))
                // {
                //    countDetail.ActivityDetailExtra5Date = Convert.ToDateTime(row["ActivityDetailExtra5Date"] != DBNull.Value ? row["ActivityDetailExtra5Date"] : null);
                // }
                // else
                // {
                //    countDetail.ActivityDetailExtra5Date = new DateTime(1900, 1, 1);
                // }
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

                // ColumnSpacing = 15,
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

            foreach (CountDetail countDetail in observableCollection)
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

            listViewMain.ItemTemplate = (DataTemplate)this.ResourceDictionary["dataTemplatePhysicalCountDetail"];

            return observableCollection;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override ObservableCollection<object> Search(string searchText)
        {
            var observableCollection = new ObservableCollection<object>();
            this.ListDetails = this.ListCountDetails.Where(countDetail => countDetail.ActivityDetailFKRefno.ToString().ToLower().Contains(searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (CountDetail countDetail in this.ListDetails)
            {
                // CountDetail countDetail = new CountDetail
                // {
                //    ActivityDetailRefno = Convert.ToInt32(row["ActivityDetailRefno"] != DBNull.Value ? row["ActivityDetailRefno"] : null),
                //    ActivityDetailLogisticUnitExpiryUseByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitExpiryUseByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitExpiryUseByDate"] : null),
                //    ActivityDetailStatus = Convert.ToInt32(row["ActivityDetailStatus"] != DBNull.Value ? row["ActivityDetailStatus"] : null),
                //    ActivityDetailFKRefno = Convert.ToInt32(row["ActivityDetailFKRefno"] != DBNull.Value ? row["ActivityDetailFKRefno"] : null),
                //    BaseUnitCode = Convert.ToString(row["BaseUnitCode"] != DBNull.Value ? row["BaseUnitCode"] : null),
                //    Location = Convert.ToString(row["Location"] != DBNull.Value ? row["Location"] : null),
                //    ProductSchemeVariantDescription = Convert.ToString(row["ProductSchemeVariantDescription"] != DBNull.Value ? row["ProductSchemeVariantDescription"] : null),
                //    ProductUnitIsBC = Convert.ToBoolean(row["ProductUnitIsBC"] != DBNull.Value ? row["ProductUnitIsBC"] : null),
                //    WtUnitCode = Convert.ToString(row["WtUnitCode"] != DBNull.Value ? row["WtUnitCode"] : null),
                //    LocationMapRefno = Convert.ToInt32(row["LocationMapRefno"] != DBNull.Value ? row["LocationMapRefno"] : null),
                //    UnitCode = Convert.ToString(row["UnitCode"] != DBNull.Value ? row["UnitCode"] : null),
                //    ProductCode = Convert.ToString(row["ProductCode"] != DBNull.Value ? row["ProductCode"] : null),
                //    ProductRefno = Convert.ToInt32(row["ProductRefno"] != DBNull.Value ? row["ProductRefno"] : null),
                //    ProductDescription = Convert.ToString(row["ProductDescription"] != DBNull.Value ? row["ProductDescription"] : null),
                //    ProductUnitBC = Convert.ToString(row["ProductUnitBC"] != DBNull.Value ? row["ProductUnitBC"] : null),
                //    ProductUnitSizeDescription = Convert.ToString(row["ProductUnitSizeDescription"] != DBNull.Value ? row["ProductUnitSizeDescription"] : null),
                //    ActivityDetailQuantity = Convert.ToDecimal(row["ActivityDetailQuantity"] != DBNull.Value ? row["ActivityDetailQuantity"] : null),
                //    ActivityDetailBaseQuantity = Convert.ToDecimal(row["ActivityDetailBaseQuantity"] != DBNull.Value ? row["ActivityDetailBaseQuantity"] : null),
                //    ActivityDetailWeightQuantity = Convert.ToDecimal(row["ActivityDetailWeightQuantity"] != DBNull.Value ? row["ActivityDetailWeightQuantity"] : null),
                //    ActivityDetailLogisticUnitBestBeforeSellByDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitBestBeforeSellByDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitBestBeforeSellByDate"] : null),
                //    ActivityDetailSchemeID = Convert.ToString(row["ActivityDetailSchemeID"] != DBNull.Value ? row["ActivityDetailSchemeID"] : null),
                //    ActivityDetailUniqueSchemeID = Convert.ToString(row["ActivityDetailUniqueSchemeID"] != DBNull.Value ? row["ActivityDetailUniqueSchemeID"] : null),
                //    ActivityDetailSingleTradeItemSerialNumber = Convert.ToString(row["ActivityDetailSingleTradeItemSerialNumber"] != DBNull.Value ? row["ActivityDetailSingleTradeItemSerialNumber"] : null),
                //    ActivityDetailLogisticUnitBatchLotNumber = Convert.ToString(row["ActivityDetailLogisticUnitBatchLotNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitBatchLotNumber"] : null),
                //    ActivityDetailLogisticUnitProductionDate = Convert.ToDateTime(row["ActivityDetailLogisticUnitProductionDate"] != DBNull.Value ? row["ActivityDetailLogisticUnitProductionDate"] : null),
                //    ActivityDetailLogisticUnitSkidNumber = Convert.ToString(row["ActivityDetailLogisticUnitSkidNumber"] != DBNull.Value ? row["ActivityDetailLogisticUnitSkidNumber"] : null),
                //    LocationCode = Convert.ToInt32(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    LocationName = Convert.ToString(row["LocationName"] != DBNull.Value ? row["LocationName"] : null),
                //    ProductSchemeVariantCode = Convert.ToString(row["LocationCode"] != DBNull.Value ? row["LocationCode"] : null),
                //    ProductSchemeVariantNo = Convert.ToInt32(row["ProductSchemeVariantNo"] != DBNull.Value ? row["ProductSchemeVariantNo"] : null),
                //    ActivityDetailExtra1Text = Convert.ToString(row["ActivityDetailExtra1Text"] != DBNull.Value ? row["ActivityDetailExtra1Text"] : null),
                //    ActivityDetailExtra2Text = Convert.ToString(row["ActivityDetailExtra2Text"] != DBNull.Value ? row["ActivityDetailExtra2Text"] : null),
                //    ActivityDetailExtra3Text = Convert.ToString(row["ActivityDetailExtra3Text"] != DBNull.Value ? row["ActivityDetailExtra3Text"] : null),
                //    ActivityDetailExtra4Text = Convert.ToString(row["ActivityDetailExtra4Text"] != DBNull.Value ? row["ActivityDetailExtra4Text"] : null)
                // };

                // if (DateTime.TryParse(Convert.ToString(row["ActivityDetailExtra5Date"]), out _))
                // {
                //    this.ActExtra.DateTime5 = Convert.ToDateTime(row["ActivityDetailExtra5Date"]);
                // }
                // else
                // {
                //    this.ActExtra.DateTime5 = Convert.ToDateTime(new DateTime(1900, 1, 1));
                // }
                observableCollection.Add(countDetail);
            }

            return observableCollection;
        }

        /// <summary>
        /// Select the activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task AssignActivityDetail(object selectedActivityDetail)
        {
            await base.AssignActivityDetail(selectedActivityDetail);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override async Task<string> CreateSubmitXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                if (this.SysEnv.SysSettings.IsDisconnectedMode)
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT PHYSICAL COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>0</skid_fk_refno>"
                        + "</imclient>");
                }
                else
                {
                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT PHYSICAL COUNT DETAIL</action>"
                        + "<whs_id>0</whs_id>"
                        + "<sect_refno>0</sect_refno>"
                        + "</command>"
                        + await SysCommon.GetRequestDefaults(false)
                        + this.GetRequestDetailDefaults()
                        + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
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
                    + "<action>SUBMIT PHYSICAL COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                    + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
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
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public override async Task<string> CreateDeleteXml(XDocument data)
        {
            var xmlString = new StringBuilder();
            var xml = XmlWriter.Create(xmlString);

            XDocument request;
            if (data.Document.Root.Element("skid_data") == null)
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE PHYSICAL COUNT DETAIL</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + this.GetRequestDetailDefaults()
                    + "<skid_fk_refno>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidFKRefno + "</skid_fk_refno>"
                    + "</imclient>");
            }
            else
            {
                request = XDocument.Parse(
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<imclient>"
                    + "<command>"
                    + "<action>DELETE PHYSICAL COUNT DETAIL SKID</action>"
                    + "<whs_id>0</whs_id>"
                    + "<sect_refno>0</sect_refno>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<skid_data>" + data.Root.Element("skid_data").Value + "</skid_data>"
                    + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                    + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                    + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
                    + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                    + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                    + "<is_negate_qty>false</is_negate_qty>"
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
            return $"Physical count # {this.ActItem.ActHdr.HdrFkRefno}";
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

            return (DataTemplate)this.ResourceDictionary["dataTemplatePhysicalCountDetail"];
        }
    }
}