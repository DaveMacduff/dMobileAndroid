// <copyright file="clsAssignDefaultLocationActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.Resources.Models;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="AssignDefaultLocActItem" />.
    /// </summary>
    public class AssignDefaultLocActItem : ActItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignDefaultLocActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public AssignDefaultLocActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(AssignDefaultLocActItem).FullName;
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
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
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
        /// The Insert.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="askToInsertAsNewItem">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

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
        /// The GetActivityItemRefno.
        /// </summary>
        /// <param name="prodUnitRefno">.</param>
        /// <returns>.</returns>
        public override Task<int> GetActivityItemRefno(int prodUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SelectActivityItem.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task SelectActivityItem(object selectedActivityItem)
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
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The UpdateQuantityFromActivityItemList.
        /// </summary>
        /// <param name="selectedActivityItem">The selectedActivityItem<see cref="object"/>.</param>
        /// <param name="actItemRefno">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
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
                string query = "SELECT * FROM tProductLocationMapType WHERE ProductRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                object[] parameters = new object[]
                {
                    Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                    Convert.ToInt32(int.Parse(data.Root.Element("picking_old_location_refno").Value)),
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking),
                };

                List<tProductLocationMapTypeModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductLocationMapTypeModel>(query, parameters);

                if (results != null && results.Count > 0)
                {
                    string queryUpdate = "UPDATE tProductLocationMapType SET LocationMapRefno = ? WHERE ProductRefno = ? AND WarehouseLocationMapRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                    object[] parametersUpdate = new object[]
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
                    string queryInsert = "INSERT INTO tProductLocationMapType (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum) VALUES (?, ?, ?, ?)";
                    object[] parametersInsert = new object[]
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
                string query = "SELECT * FROM tProductLocationMapType WHERE ProductRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                object[] parameters = new object[]
                {
                    Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                    Convert.ToInt32(int.Parse(data.Root.Element("receiving_old_location_refno").Value)),
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving),
                };

                List<tProductLocationMapTypeModel> results = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tProductLocationMapTypeModel>(query, parameters);

                if (results != null && results.Count > 0)
                {
                    string queryUpdate = "UPDATE tProductLocationMapType SET LocationMapRefno = ? WHERE ProductRefno = ? AND WarehouseLocationMapRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                    object[] parametersUpdate = new object[]
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
                    string queryInsert = "INSERT INTO tProductLocationMapType (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum) VALUES (?, ?, ?, ?)";
                    object[] parametersInsert = new object[]
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
            int locMapRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(
                whsLocMapRefno,
                prodRefno,
                SysCommon.ProdLocMapTypes.Picking);

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
