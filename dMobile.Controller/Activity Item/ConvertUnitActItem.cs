// <copyright file="ConvertUnitActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

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
using dMobile.Resources.Types.Items;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="ConvertUnitActItem" />.
    /// </summary>
    public class ConvertUnitActItem : ActItem
    {
        /// <summary>
        /// Defines the listConvertUnitItems.
        /// </summary>
        private List<ConvertUnitItem> listConvertUnitItems = new List<ConvertUnitItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertUnitActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ConvertUnitActItem(Activity.Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(ConvertUnitActItem).FullName;
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItemFromList(int actItemFoundRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public override bool IsAllowAddSkidDetail()
        {
            return false;
        }

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            var isCatchAsk = this.ProdInfo.IsCatchAsk;
            if (isCatchAsk)
            {
                // modules are for exclusions
                ////isCatchAsk = this.SysEnv.SysSettings.IsConvertUnitCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// Check if the product is catch count.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch count, else false.</returns>
        public override bool IsCatchAskZero()
        {
            var result = false;
            if (this.IsCatchAsk() && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsConvertUnitModuleNoCatchAskZeroEnabled;
            }

            return result;
        }

        /// <summary>
        /// Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign key reference number.</param>
        /// <returns><see cref="int"/> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;
            return foundIdx;
        }

        /// <summary>
        /// The UpdateListItemQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetUnableToParseItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFkRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFkRefnoMessage(string actItemStringFkRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFkRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override Task Insert(XDocument data)
        {
            if (this.ListItems != null)
            {
                var schemeData = Common.FillSActItemSchemeDataFromXml(data);
                var item = new ConvertUnitItem()
                {
                    ProductUnitRefno = this.ProdInfo.UnitRefno,
                    ProductRefno = this.ProdInfo.Refno,
                    ProductCode = this.ProdInfo.Code,
                    ProductUnitBC = this.ProdInfo.UnitBc,
                    UnitCode = this.ProdInfo.UnitCode,
                    ProductDescription = this.ProdInfo.Desc,
                    ProductUnitSizeDescription = this.ProdInfo.UnitSizeDesc,
                    ActivityDetailQuantity = schemeData.ActDetailQuantity,
                    ActivityDetailBaseQuantity = schemeData.ActDetailBaseQuantity,
                    ActivityDetailWeightQuantity = schemeData.ActDetailWeightQuantity,
                    ActivityDetailLogisticUnitBestBeforeSellByDate = schemeData.ProdSchemeData.LogisticUnitBestBeforeSellByDate,
                    ActivityDetailLogisticUnitProductionDate = schemeData.ProdSchemeData.LogisticUnitProductionDate,
                    ActivityDetailSingleTradeItemSerialNumber = schemeData.ProdSchemeData.SingleTradeItemSerialNumber,
                    ActivityDetailLogisticUnitBatchLotNumber = schemeData.ProdSchemeData.LogisticUnitBatchLotNumber,
                    ProductSchemeVariantCode = schemeData.ProdSchemeData.VariantCode,
                    ProductSchemeVariantNumber = schemeData.ProdSchemeData.VariantNumber,
                    ActivityDetailSchemeID = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                        this.ProdInfo.Code,
                        this.ProdInfo.Scheme,
                        schemeData.ProdSchemeData),
                    ActivityDetailUniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                        this.ProdInfo.Code,
                        this.ProdInfo.Scheme,
                        schemeData.ProdSchemeData),
                    ActivityDetailFKRefno = this.ItemFkRefno,
                    ActivityDetailRefno = this.ItemRefno,
                };
                this.ListItems.Add(item);
                this.SelectedListIdx = this.ListItems.Count - 1;
                _ = this.AssignActivityItem(item);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var moveTable = new DataTable("ActivityConvertUnitItem");

            moveTable.Columns.Add("ProductUnitRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ProductRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ProductCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductUnitBC", Type.GetType("System.String"));
            moveTable.Columns.Add("UnitCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductDescription", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductUnitSizeDescription", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailBaseQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailWeightQuantity", Type.GetType("System.Decimal"));
            moveTable.Columns.Add("ActivityDetailSchemeID", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailUniqueSchemeID", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitBestBeforeSellByDate", Type.GetType("System.DateTime"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitProductionDate", Type.GetType("System.DateTime"));
            moveTable.Columns.Add("ActivityDetailSingleTradeItemSerialNumber", Type.GetType("System.String"));
            moveTable.Columns.Add("ActivityDetailLogisticUnitBatchLotNumber", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductSchemeVariantCode", Type.GetType("System.String"));
            moveTable.Columns.Add("ProductSchemeVariantNumber", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ActivityDetailFKRefno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("ActivityDetailRefno", Type.GetType("System.Int32"));

            this.listConvertUnitItems = moveTable.Rows.Cast<ConvertUnitItem>().ToList();
            this.ListItems = this.listConvertUnitItems.Cast<object>().ToList();
            return Task.FromResult(this.ListItems);
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item refno else 0.</returns>
        public override Task<int> GetActivityItemRefno(int productUnitRefno, SLocMap locMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override async Task<int> AssignActivityItem(object selectedActivityItem)
        {
            if (selectedActivityItem is ConvertUnitItem item)
            {
                this.SelectedItem = item;
                this.ItemRefno = item.ActivityDetailRefno;
                this.ItemFkRefno = item.ActivityDetailFKRefno;
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(item.ProductUnitRefno);
                this.ActStatus = SysCommon.ActStatus.NoChange;
                await AssignItemLocation(item);

                // LocationMap.GetInstance().CurrLocMap warehouse needs to be set before call to GetSchemeComponentsFromRefno.
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }

            return this.ItemFkRefno;

            //////////////////////////////////////////////////////////////////////////
            async Task AssignItemLocation(ConvertUnitItem item)
            {
                // This code needs to be before the call to GetSelectedItemLocation, because the skid location may be
                // used if the product unit and warehouse match.
                if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                    this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
                {
                    this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
                }

                var itemLocMap = await this.GetSelectedItemLocation(LocationMap.GetInstance().LoginWhsLocMapRefno);
                if (itemLocMap == null)
                {
                    // Current location used, we need one for popup forms.
                    this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.ActHdr.LocationMapRefno);
                    if (this.LocMap.WhsLocMapRefno == 0)
                    {
                        var defLocMapRefno = LocationMapTasks.DefaultLocMapRefnoToWhsIfUnassigned(locationMapRefno: 0);
                        this.LocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(defLocMapRefno);
                    }
                }
                else
                {
                    this.LocMap = itemLocMap.Clone();

                    // Override the location with the product default if exists and complete.
                    if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                    {
                        // Only assign the current location if the map is complete else leave it alone.
                        if (await LocationMapTasks.IsCompleteLocMap(itemLocMap))
                        {
                            // Update the chosen location with the current when defaulting is turned on.
                            LocationMap.GetInstance().CurrLocMap = itemLocMap.Clone();
                            LocationMap.GetInstance().ChosenLocMap = itemLocMap.Clone();

                            // Assign the skid the location, because defaulting is enabled.
                            if (this.ActHdr.SkidInfo.CurrSkid.SkidFkRefno > 0)
                            {
                                this.ActHdr.SkidInfo.CurrSkid.LocMap = itemLocMap.Clone();
                            }

                            // Update the activity item location.
                            this.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// The Refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Submit.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Submit(XDocument data, object selectedActivityItem)
        {
            _ = SysCommon.SubmitComm(await this.CreateSubmitXml(data));
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            var strXml = new StringBuilder();
            using var writer = XmlWriter.Create(strXml);
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var isBc = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CONVERT UNIT</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + isBc.ToString() + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<alt_unit>" + unitCode + "</alt_unit>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<modifier1>" + this.SysEnv.CurrentBarCodeModifier1 + "</modifier1>"
                + "</imclient>");

            request.Save(writer);
            writer.Flush();
            writer.Close();

            return strXml.ToString();
        }

        /// <summary>
        /// Replace activity line item quantity.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <param name="quantity"><see cref="decimal"/> Quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        /// <exception cref="NotImplementedException"> Method is not implemented.</exception>
        public override void ReplaceQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override Task UpdateItemTableQuantity(object activityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
        public override int GetFkRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Get activity item control description.</returns>
        public override string GetControlDescription()
        {
            return "Convert product unit.";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Delete()
        {
            if (this.ListItems != null)
            {
                this.ListItems.Clear();
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFkRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<ActItemSchemeData> GetProdSchemeDataFromDetailFkRefno(int actDetailFkRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> CreateDeleteXml(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Update(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetTotalQuantity.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override Task<string> GetTotalQuantity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the default location for the activity base on the sign quantity of the transaction.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Location map reference number.</returns>
        public override async Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno)
        {
            var locMapRefno = 0;
            if (this.SelectedItem is ConvertUnitItem item)
            {
                if (Math.Sign(item.ActivityDetailQuantity) == 0 ||
                    Math.Sign(item.ActivityDetailQuantity) == 1)
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Picking);
                }
                else
                {
                    locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                        whsLocMapRefno,
                        prodRefno,
                        SysCommon.ProdLocMapTypes.Receiving);
                }
            }

            if (locMapRefno == 0)
            {
                locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                    whsLocMapRefno,
                    prodRefno,
                    SysCommon.ProdLocMapTypes.Floor);
            }

            if (!await LocationMapTasks.IsCompleteLocMap(locMapRefno))
            {
                locMapRefno = 0;
            }

            return locMapRefno;
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        public override XDocument CreateDump()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override DataTemplate GetDataTemplate()
        {
            throw new NotImplementedException();
        }
    }
}
