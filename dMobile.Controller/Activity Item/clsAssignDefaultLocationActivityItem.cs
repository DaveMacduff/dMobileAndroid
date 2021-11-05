namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using SQLite;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;
    using System.Collections.Generic;
    using dMobile.Resources.Models;

    public class AssignDefaultLocActItem : ActItem
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="_oActivity"></param>
        /// <remarks></remarks>
        public AssignDefaultLocActItem(Activity activity)
            : base(activity)
        {
            this.FullClassName = typeof(AssignDefaultLocActItem).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        /// <returns></returns>
        public override void UpdateListItemQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actItemStringFKRefno"></param>
        /// <returns></returns>
        public override string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno)
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetDataViewSortFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="askToInsertAsNewItem"></param>
        public override async Task Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                this.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = await ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="dataGridMain"></param>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //    throw new NotImplementedException();
        //}

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override Task<List<object>> GetActivityItemList(bool isSubmitBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Task<int> GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override Task SelectActivityItem(object selectedActivityItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Task<string> CreateSubmitXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="quantity"></param>
        /// <param name="baseQuantity"></param>
        /// <param name="weightQuantity"></param>
        public override void RefreshQuantity(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override Task UpdateQuantityFromActivityItemList(object selectedActivityItem, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override async Task Update(XDocument data)
        {
            if (bool.Parse(data.Root.Element("picking_is_update").Value))
            {
                string query = "SELECT * FROM tProductLocationMapType WHERE ProductRefno = ? AND LocationMapRefno = ? AND ProductLocationMapTypeEnum = ?";
                object[] parameters = new object[]
                {
                    Convert.ToInt32(int.Parse(data.Root.Element("prod_refno").Value)),
                    Convert.ToInt32(int.Parse(data.Root.Element("picking_old_location_refno").Value)),
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking)
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
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking)
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
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Picking)
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
                    Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving)
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
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving)
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
                        Convert.ToInt32(SysCommon.ProdLocMapTypes.Receiving)
                    };

                    await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(queryInsert, parametersInsert);
                }
            }
        }

        #endregion
    }
}