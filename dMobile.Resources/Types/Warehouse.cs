namespace dMobile.Resources.Types
{
    public class Warehouse
    {
        private string displayName;
        private int locationMapRefno;
        private int locationMapSubType;
        private string locationMapWarehouseName;
        private int locationMapWarehouseCode;
        private int locationMapWarehouseFKRefno;
        private string locationMapWarehouseBarCode;

        public string DisplayName { get => this.displayName; set => this.displayName = value; }
        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapSubType { get => locationMapSubType; set => locationMapSubType = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public int LocationMapWarehouseCode { get => locationMapWarehouseCode; set => locationMapWarehouseCode = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseBarCode { get => locationMapWarehouseBarCode; set => locationMapWarehouseBarCode = value; }
    }
}
