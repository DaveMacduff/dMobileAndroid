namespace dMobile.Controller
{
    using System;
    using System.Data;
    
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using dMobile.Bedrock;

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
        public override int UpdateDataViewQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
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
        public override void Insert(XDocument data, bool askToInsertAsNewItem)
        {
            int prodUnitRefno = int.Parse(data.Root.Element("ProductUnitRefno").Value);

            if (prodUnitRefno != 0)
            {
                this.ProdInfo = ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);
                this.ProdInfo.Scheme = ProductInfoTasks.GetSchemeComponentsFromRefno(this.ProdInfo.Refno);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataGridMain"></param>
        /// <remarks></remarks>
        public override void DesignDataGrid(DataGrid dataGridMain)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        public override DataView GetDataView(bool isSubmitBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public override DataView Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prodUnitRefno"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromProdUnitRefno(int prodUnitRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectItem(int idx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override string CreateSubmitXML(XDocument data)
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
        public override void RefreshQuantity(int idx, decimal quantity, decimal baseQuantity, decimal weightQuantity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="actItemRefno"></param>
        public override void UpdateQuantityFromDataView(int idx, int actItemRefno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void Update(XDocument data)
        {
            ////MyBase.UpdateActivityItem(xmlParams)

            ////<?xml version="1.0" encoding="utf-16" standalone="yes"?>
            ////<param>
            ////    <prod_refno><%= %></prod_refno>
            ////    <receiving_is_update><%= %></receiving_is_update>
            ////    <receiving_warehouse_location_refno><%= %></receiving_warehouse_location_refno>
            ////    <receiving_old_location_refno><%= %></receiving_old_location_refno>
            ////    <receiving_new_location_refno><%= %></receiving_new_location_refno>
            ////    <picking_is_update><%= %></picking_is_update>
            ////    <picking_warehouse_location_refno><%=  %></picking_warehouse_location_refno>
            ////    <picking_old_location_refno><%= %></picking_old_location_refno>
            ////    <picking_new_location_refno><%= %></picking_new_location_refno>
            ////</param>

            using (SqlCeCommand cmndProdtLocMapType = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
            {
                cmndProdtLocMapType.CommandText = "tProductLocationMapType";
                cmndProdtLocMapType.CommandType = CommandType.TableDirect;
                cmndProdtLocMapType.IndexName
                    = "PK_ProductLocationMapTypeOnProductRefnoAndLocationMapRefnoAndProductLocationMapTypeEnum";
                using (SqlCeDataReader drProdLocMapType = cmndProdtLocMapType.ExecuteReader())
                {
                    if (bool.Parse(data.Root.Element("picking_is_update").Value))
                    {
                        if (drProdLocMapType.Seek(
                            DbSeekOptions.FirstEqual,
                            new object[] 
                {
                  int.Parse(data.Root.Element("prod_refno").Value), 
                  int.Parse(data.Root.Element("picking_old_location_refno").Value), 
                  (int)SysCommon.ProdLocMapTypes.Picking 
                }))
                        {
                            cmndProdtLocMapType.CommandType = CommandType.Text;
                            cmndProdtLocMapType.IndexName = string.Empty;

                            StringBuilder sql = new StringBuilder();
                            sql.Append("UPDATE tProductLocationMapType");
                            sql.AppendFormat(
                                " SET LocationMapRefno = {0}",
                                int.Parse(data.Root.Element("picking_new_location_refno").Value));
                            sql.AppendFormat(
                                " WHERE ProductRefno = {0}",
                                int.Parse(data.Root.Element("prod_refno").Value));
                            sql.AppendFormat(
                                " AND WarehouseLocationMapRefno = {0}",
                                int.Parse(data.Root.Element("picking_warehouse_location_refno").Value));
                            sql.AppendFormat(
                                " AND LocationMapRefno = {0}",
                                int.Parse(data.Root.Element("picking_old_location_refno").Value));
                            sql.AppendFormat(
                                " AND ProductLocationMapTypeEnum = {0}",
                                (int)SysCommon.ProdLocMapTypes.Picking);

                            cmndProdtLocMapType.CommandText = sql.ToString();
                            cmndProdtLocMapType.ExecuteNonQuery();
                        }
                        else
                        {
                            cmndProdtLocMapType.CommandType = CommandType.Text;
                            cmndProdtLocMapType.IndexName = string.Empty;

                            StringBuilder sql = new StringBuilder();
                            sql.Append("INSERT INTO tProductLocationMapType");
                            sql.Append(" (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum)");
                            sql.AppendFormat(
                                " VALUES ({0}, {1}, {2}, {3})",
                                int.Parse(data.Root.Element("prod_refno").Value),
                                int.Parse(data.Root.Element("picking_warehouse_location_refno").Value),
                                int.Parse(data.Root.Element("picking_new_location_refno").Value),
                                (int)SysCommon.ProdLocMapTypes.Picking);

                            cmndProdtLocMapType.CommandText = sql.ToString();
                            cmndProdtLocMapType.ExecuteNonQuery();
                        }
                    }

                    if (bool.Parse(data.Root.Element("receiving_is_update").Value))
                    {
                        cmndProdtLocMapType.CommandText = "tProduct";
                        cmndProdtLocMapType.CommandType = CommandType.TableDirect;
                        cmndProdtLocMapType.IndexName
                            = "PK_ProductLocationMapTypeOnProductRefnoAndLocationMapRefnoAndProductLocationMapTypeEnum";
                        if (drProdLocMapType.Seek(
                            DbSeekOptions.FirstEqual,
                            new object[] 
                {
                  int.Parse(data.Root.Element("prod_refno").Value), 
                  int.Parse(data.Root.Element("receiving_old_location_refno").Value), 
                  (int)SysCommon.ProdLocMapTypes.Receiving 
                }))
                        {
                            cmndProdtLocMapType.CommandType = CommandType.Text;
                            cmndProdtLocMapType.IndexName = string.Empty;

                            StringBuilder sql = new StringBuilder();
                            sql.Append("UPDATE tProductLocationMapType");
                            sql.AppendFormat(
                                " SET LocationMapRefno = {0}",
                                int.Parse(data.Root.Element("receiving_new_location_refno").Value));
                            sql.AppendFormat(
                                " WHERE ProductRefno = {0}",
                                int.Parse(data.Root.Element("prod_refno").Value));
                            sql.AppendFormat(
                                " AND WarehouseLocationMapRefno = {0}",
                                int.Parse(data.Root.Element("receiving_warehouse_location_refno").Value));
                            sql.AppendFormat(
                                " AND LocationMapRefno = {0}",
                                int.Parse(data.Root.Element("receiving_old_location_refno").Value));
                            sql.AppendFormat(
                                " AND ProductLocationMapTypeEnum = {0}",
                                (int)SysCommon.ProdLocMapTypes.Receiving);

                            cmndProdtLocMapType.CommandText = sql.ToString();
                            cmndProdtLocMapType.ExecuteNonQuery();
                        }
                        else
                        {
                            cmndProdtLocMapType.CommandType = CommandType.Text;
                            cmndProdtLocMapType.IndexName = string.Empty;

                            StringBuilder sql = new StringBuilder();
                            sql.Append("INSERT INTO tProductLocationMapType");
                            sql.Append(" (ProductRefno, WarehouseLocationMapRefno, LocationMapRefno, ProductLocationMapTypeEnum)");
                            sql.AppendFormat(
                                " VALUES ({0}, {1}, {2}, {3})",
                                int.Parse(data.Root.Element("prod_refno").Value),
                                int.Parse(data.Root.Element("receiving_warehouse_location_refno").Value),
                                int.Parse(data.Root.Element("receiving_new_location_refno").Value),
                                (int)SysCommon.ProdLocMapTypes.Receiving);

                            cmndProdtLocMapType.CommandText = sql.ToString();
                            cmndProdtLocMapType.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
