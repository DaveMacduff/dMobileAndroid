namespace dMobile.View.Types
{
    public class Section
    {
        private int locationMapRefno;
        private int locationMapSubType;
        private int warehouseLocationMapRefno;
        private int locationMapSectionCode;
        private string locationMapSectionName;
        private int locationMapSectionFKRefno;
        private string locationMapSectionBarCode;

        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapSubType { get => locationMapSubType; set => locationMapSubType = value; }
        public int WarehouseLocationMapRefno { get => warehouseLocationMapRefno; set => warehouseLocationMapRefno = value; }
        public int LocationMapSectionCode { get => locationMapSectionCode; set => locationMapSectionCode = value; }
        public string LocationMapSectionName { get => locationMapSectionName; set => locationMapSectionName = value; }
        public int LocationMapSectionFKRefno { get => locationMapSectionFKRefno; set => locationMapSectionFKRefno = value; }
        public string LocationMapSectionBarCode { get => locationMapSectionBarCode; set => locationMapSectionBarCode = value; }
    }
}
