// <copyright file="clsAssignDefaultLocationActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.Resources.Models;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="AssignDefaultLocActItem" />.
    /// </summary>
    public class AssignDefaultLocActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignDefaultLocActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public AssignDefaultLocActItem(Activity.Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(AssignDefaultLocActItem).FullName;
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
        /// Insert an activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Data to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Ask a question to insert a new item.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            var prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);
            if (prodUnitRefno != 0)
            {
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
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
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            throw new NotImplementedException();
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
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The CreateSubmitXML.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>.</returns>
        public override Task<string> CreateSubmitXML(XDocument data)
        {
            throw new NotImplementedException();
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
        /// The Update.
        /// </summary>
        /// <param name="data">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Update(XDocument data)
        {
            if (bool.Parse(data.Root.Element("picking_is_update").Value))
            {
                var query = "SELECT * FROM tProductLocationMapType WHERE ProductRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                var parameters = new object[]
                {
                    Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                    Convert.ToInt32(int.Parse(data.Root.Element("picking_old_location_refno").Value)),
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking),
                };

                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductLocationMapTypeModel>(query, parameters);

                if (results != null && results.Count > 0)
                {
                    var queryUpdate = "UPDATE tProductLocationMapType SET LocationMapRefno = ? WHERE ProductRefno = ? AND WarehouseLocationMapRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                    var parametersUpdate = new object[]
                    {
                        Convert.ToInt32(int.Parse(data.Root.Element("picking_new_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("picking_warehouse_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("picking_old_location_refno").Value)),
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking),
                    };

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(queryUpdate, parametersUpdate);
                }
                else
                {
                    var queryInsert = "INSERT INTO tProductLocationMapType (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum) VALUES (?, ?, ?, ?)";
                    var parametersInsert = new object[]
                    {
                        Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("picking_warehouse_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("picking_new_location_refno").Value)),
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking),
                    };

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(queryInsert, parametersInsert);
                }
            }

            if (bool.Parse(data.Root.Element("receiving_is_update").Value))
            {
                var query = "SELECT * FROM tProductLocationMapType WHERE ProductRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                var parameters = new object[]
                {
                    Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                    Convert.ToInt32(int.Parse(data.Root.Element("receiving_old_location_refno").Value)),
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving),
                };

                var results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductLocationMapTypeModel>(query, parameters);

                if (results != null && results.Count > 0)
                {
                    var queryUpdate = "UPDATE tProductLocationMapType SET LocationMapRefno = ? WHERE ProductRefno = ? AND WarehouseLocationMapRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                    var parametersUpdate = new object[]
                    {
                        Convert.ToInt32(int.Parse(data.Root.Element("receiving_new_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("receiving_warehouse_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("receiving_old_location_refno").Value)),
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving),
                    };

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(queryUpdate, parametersUpdate);
                }
                else
                {
                    var queryInsert = "INSERT INTO tProductLocationMapType (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum) VALUES (?, ?, ?, ?)";
                    var parametersInsert = new object[]
                    {
                        Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("receiving_warehouse_location_refno").Value)),
                        Convert.ToInt32(int.Parse(data.Root.Element("receiving_new_location_refno").Value)),
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving),
                    };

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(queryInsert, parametersInsert);
                }
            }
        }

        /// <summary>
        /// Get the activity item foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type.</param>
        /// <returns><see cref="int"/> Found activity item foreign key refno, else 0 if not found.</returns>
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
            return "Assign default location.";
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override async Task<int> Delete()
        {
            throw new NotImplementedException();
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
        /// The Submit.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Submit(XDocument data, object selectedActivityItem)
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
        /// The IsCatchAsk.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAsk()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IsCatchAskZero.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsCatchAskZero()
        {
            throw new NotImplementedException();
        }
    }
}
