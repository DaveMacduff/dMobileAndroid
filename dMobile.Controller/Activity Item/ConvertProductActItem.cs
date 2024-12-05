// <copyright file="ConvertProductActItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Item
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Types.Items;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ConvertProductActItem" />.
    /// </summary>
    public class ConvertProductActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertProductActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ConvertProductActItem(Activity.Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(ConvertProductActItem).FullName;
        }

        /// <summary>
        /// Gets or sets the FromProdRefno
        /// Gets the FROM product reference number.
        /// </summary>
        public int FromProdRefno { get; set; }

        /// <summary>
        /// Gets or sets the ToProdRefno
        /// Gets the TO product reference number.
        /// </summary>
        public int ToProdRefno { get; set; }

        /// <summary>
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAsk()
        {
            return this.ProdInfo.IsCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            return this.IsCatchAsk();
        }

        /// <summary>
        /// Find the list item index of the item that matches the activity item foreign key reference number.
        /// </summary>
        /// <param name="activityItemFkRefno"><see cref="int"/> Activity item foreign key reference number.</param>
        /// <returns><see cref="int"/> Activity list item found index, else -1.</returns>
        public override int FindActivityItemIndex(int activityItemFkRefno)
        {
            var foundIdx = -1;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in this.ListItems)
            {
                foundIdx++;
                if (((ConvertProductActItem)item).ItemFkRefno == activityItemFkRefno)
                {
                    break;
                }
            }

            return foundIdx;
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
        /// Update activity item quantities.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity"><see cref="decimal"/> Alternate quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public override void AddToQuantityInActivityItem(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity)
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
                var item = new ConvertProductItem()
                {
                    FromProdFKRefno = Convert.ToInt32(data.Root?.Element("from_prod_fk_refno")?.Value),
                    ToProdFKRefno = Convert.ToInt32(data.Root?.Element("to_prod_fk_refno")?.Value),
                    LocMapRefno = Convert.ToInt32(data.Root?.Element("location_map_refno")?.Value),
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
        /// Get activity item list.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns><see cref="List{T}"/> List of activity items.</returns>
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            // This list activity is not used for this activity, filled in list data for future possible disconnected mode.
            this.ListItems.Clear();
            var item = new ConvertProductItem
            {
                LocMapRefno = this.LocMapRefno,
                FromProdFKRefno = this.FromProdRefno,
                ToProdFKRefno = this.ToProdRefno,
            };
            this.ListItems.Add(item);
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
        public override Task<int> GetActivityItemRefno(int prodUnitRefno, SLocMap locMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> The activity item.</param>
        /// <returns><see cref="Task{T}"/> Activity item foreign key reference number.</returns>
        public override Task<int> AssignActivityItem(object activityItem)
        {
            var itemFKRefno = Task.FromResult(0);
            var item = activityItem as ConvertProductItem;
            item = item ?? throw new NullReferenceException("activityItem as ConvertProductItem == null");
            this.FromProdRefno = item.FromProdFKRefno;
            this.ToProdRefno = item.ToProdFKRefno;
            _ = Task.Run(async () =>
            {
                this.LocMap =
                    await LocationMapTasks.GetSLocMapFromLocMapRefno(
                        ((ConvertProductItem)activityItem).LocMapRefno);
            });

            return itemFKRefno;
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
        /// <returns><see cref="string"/> Submission string.</returns>
        public async override Task<string> CreateSubmitXml(XDocument data)
        {
            var result = new StringBuilder();
            using var writer = XmlWriter.Create(result);
            var prodCode = data.Root?.Element("ProductCode")!.Value.PadRight(totalWidth: 10);
            var barCodeCode = data.Root?.Element("ProductUnitBarCodeCode")!.Value.PadRight(totalWidth: 20);
            var isBc = bool.Parse(data.Root?.Element("ProductUnitIsBC")?.Value ?? string.Empty);
            var unitCode = data.Root?.Element("UnitCode")!.Value;
            var symbologyPrefix = data.Root?.Element("ProductUnitBarCodeSymbologyPrefix")?.Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CONVERT PRODUCT</action>"
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
            await writer.FlushAsync();
            writer.Close();

            return result.ToString();
        }

        /// <summary>
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
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
        /// <returns><see cref="int"/> Found activity item foreign key reference number, else 0 if not found.</returns>
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
            return "Convert product.";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override Task<int> Delete()
        {
            const int fkRefno = 0;
            this.ListItems?.Clear();
            return Task.FromResult(fkRefno);
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
            var locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromWhsProdRefno(
                whsLocMapRefno,
                prodRefno,
                SysCommon.ProdLocMapTypes.Picking);

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
