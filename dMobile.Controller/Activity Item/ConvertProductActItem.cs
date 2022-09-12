// <copyright file="clsConvertProductActivityItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.Resources.Types;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ConvertProductActItem" />.
    /// </summary>
    public class ConvertProductActItem : ActItem
    {
        /// <summary>
        /// Defines the toProdRefno.
        /// </summary>
        private int toProdRefno;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertProductActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ConvertProductActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(ConvertProductActItem).FullName;
        }

        /// <summary>
        /// Set the activity item selected activity item.
        /// </summary>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public override void SetSelectedActivityItem(int actItemFoundRefno)
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
                isCatchAsk = this.SysEnv.SysSettings.IsConvertUnitCatchAskEnabled;
            }

            return isCatchAsk;
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsCatchAskZero()
        {
            var result = this.IsCatchAsk();
            if (result && this.ProdInfo.IsCatchAskZero)
            {
                result = !this.SysEnv.SysSettings.IsConvertUnitModuleNoCatchAskZeroEnabled;
            }

            return result;
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
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetUnableToFindItemFKRefnoMessage.
        /// </summary>
        /// <param name="actItemStringFKRefno">.</param>
        /// <returns>.</returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// The GetDataViewSortFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// Insert a new adjustment activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Adjustment activity information to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Stop and ask to insert an item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            if (this.ListItems != null)
            {
                var item = new ConvertProductItem()
                {
                    From_prod_fk_refno = Convert.ToInt32(data.Root.Element("from_prod_fk_refno").Value),
                    To_prod_fk_refno = Convert.ToInt32(data.Root.Element("to_prod_fk_refno").Value),
                    Location_map_refno = Convert.ToInt32(data.Root.Element("location_map_refno").Value),
                };

                this.ListItems.Add(item);
                this.SelectedListItemIdx = this.ListItems.Count - 1;
                _ = this.AssignActivityItem(item);
            }
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetActivityItemList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override async Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            var moveTable = new DataTable("ActivityConvertProductItem");

            moveTable.Columns.Add("from_prod_fk_refno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("to_prod_fk_refno", Type.GetType("System.Int32"));
            moveTable.Columns.Add("location_map_refno", Type.GetType("System.Int32"));

            // this.DataViewItems = moveTable.DefaultView;
            // this.DataViewItems.Table.TableName = this.MainGridMappingName;
            this.ListItems = moveTable.Select().ToList<object>();
            return this.ListItems;
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
        public override Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task AssignActivityItem(object activityItem)
        {
            throw new NotImplementedException();
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
            var reqData = SysCommon.SubmitComm(await this.CreateSubmitXML(data));

            if (reqData.Length > 0)
            {
                // check if result is an error message
                if (reqData.Contains("<message>ERROR</message>"))
                {
                    var xmlError = new XmlDocument();
                    xmlError.LoadXml(reqData);
                    var errorDetail = xmlError.DocumentElement["detail"].InnerText;
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

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public async override Task<string> CreateSubmitXML(XDocument data)
        {
            var strXML = new StringBuilder();
            using var writer = XmlWriter.Create(strXML);
            var prodCode = data.Root.Element("ProductCode").Value.PadRight(10);
            var barCodeCode = data.Root.Element("ProductUnitBarCodeCode").Value.PadRight(20);
            var isBC = bool.Parse(data.Root.Element("ProductUnitIsBC").Value);
            var unitCode = data.Root.Element("UnitCode").Value;
            var symbologyPrefix = data.Root.Element("ProductUnitBarCodeSymbologyPrefix").Value;

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                + "<command>"
                + "<action>SUBMIT CONVERT PRODUCT</action>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + prodCode + "</prod_id>"
                + "<is_bc>" + isBC.ToString() + "</is_bc>"
                + "<symbology>" + symbologyPrefix + "</symbology>"
                + "<alt_unit>" + unitCode + "</alt_unit>"
                + "<gtin>" + barCodeCode + "</gtin>"
                + "<modifier1>" + this.SysEnv.CurrentBarCodeModifier1 + "</modifier1>"
                + "</imclient>");

            request.Save(writer);
            writer.Flush();
            writer.Close();

            return strXML.ToString();
        }

        /// <summary>
        /// The RefreshQuantity.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="quantity">.</param>
        /// <param name="baseQuantity">.</param>
        /// <param name="weightQuantity">.</param>
        public override void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
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
        public override int GetFKRefno(object resourceType)
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
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            var fkRefno = 0;

            if (this.ListItems != null)
            {
                this.ListItems.Clear();
            }

            return fkRefno;
        }

        /// <summary>
        /// The GetProdSchemeDataFromDetailFKRefno.
        /// </summary>
        /// <param name="actDetailFKRefno">The actDetailFKRefno<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{SActItemSchemeData}"/>.</returns>
        public override Task<SActItemSchemeData> GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetComment.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> GetComment(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateDeleteXML.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        public override Task<string> CreateDeleteXML(XDocument data)
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
        /// <returns>The <see cref="Task{string}"/>.</returns>
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

        /// <summary>
        /// Gets or sets the FromProdRefno
        /// Gets the FROM product reference number..
        /// </summary>
        public int FromProdRefno { get; set; }

        /// <summary>
        /// Gets or sets the ToProdRefno
        /// Gets the TO product reference number..
        /// </summary>
        public int ToProdRefno
        {
            get
            {
                return this.toProdRefno;
            }
            set
            {
                this.toProdRefno = value;
            }
        }
    }
}
