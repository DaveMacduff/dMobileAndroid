using System;
namespace dMobile.Resources.Types
{
    public class ConvertProductItem
    {
        //DataTable moveTable = new DataTable("ActivityConvertProductItem");
        //moveTable.Columns.Add("from_prod_fk_refno", Type.GetType("System.Int32"));
        //moveTable.Columns.Add("to_prod_fk_refno", Type.GetType("System.Int32"));
        //moveTable.Columns.Add("location_map_refno", Type.GetType("System.Int32"));

        private int from_prod_fk_refno;
        private int to_prod_fk_refno;
        private int location_map_refno;

        public int From_prod_fk_refno { get => from_prod_fk_refno; set => from_prod_fk_refno = value; }
        public int To_prod_fk_refno { get => to_prod_fk_refno; set => to_prod_fk_refno = value; }
        public int Location_map_refno { get => location_map_refno; set => location_map_refno = value; }
    }
}
