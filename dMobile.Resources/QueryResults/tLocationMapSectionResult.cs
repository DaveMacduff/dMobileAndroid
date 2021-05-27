namespace dMobile.Resources.QueryResults
{
    //string query = "SELECT tLocationMapSection.LocationMapRefno, "
    //+ "tLocationMapSection.LocationMapSectionCode, "
    //+ "tLocationMapSection.LocationMapSectionName, "
    //+ "tLocationMapSection.LocationMapSectionFKRefno, "
    //+ "tLocationMapSection.WarehouseLocationMapRefno, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseName, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseCode, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseIsWMS, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseFKRefno, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseMask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSectionMask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotMask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Mask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Mask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Mask, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Name, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Name, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Name, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseIsSectionTracked, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseIsSlotTracked, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseAdhocEnabled "
    //+ "FROM tLocationMapSection "
    //+ "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
    //+ "= tLocationMapSection.WarehouseLocationMapRefno "
    //+ "WHERE tLocationMapSection.LocationMapRefno = ?";    

    public class tLocationMapSectionQueryResult
    {
        private int locationMapRefno;
        private int locationMapSectionCode;
        private string locationMapSectionName;
        private int locationMapSectionFKRefno;
        private int warehouseLocationMapRefno;
        private int larehouseLocationMapRefno;
        private string locationMapWarehouseName;
        private int locationMapWarehouseCode;
        private bool locationMapWarehouseIsWMS;
        private int locationMapWarehouseFKRefno;
        private string locationMapWarehouseMask;
        private string locationMapWarehouseSectionMask;
        private bool locationMapWarehouseIsSectionTracked;
        private bool locationMapWarehouseIsSlotTracked;
        private string locationMapWarehouseSlotCoordinate1Name;
        private string locationMapWarehouseSlotCoordinate2Name;
        private string locationMapWarehouseSlotCoordinate3Name;
        private string locationMapWarehouseSlotMask;
        private string locationMapWarehouseSlotCoordinate1Mask;
        private string locationMapWarehouseSlotCoordinate2Mask;
        private string locationMapWarehouseSlotCoordinate3Mask;

        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapSectionCode { get => locationMapSectionCode; set => locationMapSectionCode = value; }
        public string LocationMapSectionName { get => locationMapSectionName; set => locationMapSectionName = value; }
        public int LocationMapSectionFKRefno { get => locationMapSectionFKRefno; set => locationMapSectionFKRefno = value; }
        public int WarehouseLocationMapRefno { get => warehouseLocationMapRefno; set => warehouseLocationMapRefno = value; }
        public int LarehouseLocationMapRefno { get => larehouseLocationMapRefno; set => larehouseLocationMapRefno = value; }
        public string LocationMapWarehouseName { get => locationMapWarehouseName; set => locationMapWarehouseName = value; }
        public int LocationMapWarehouseCode { get => locationMapWarehouseCode; set => locationMapWarehouseCode = value; }
        public bool LocationMapWarehouseIsWMS { get => locationMapWarehouseIsWMS; set => locationMapWarehouseIsWMS = value; }
        public int LocationMapWarehouseFKRefno { get => locationMapWarehouseFKRefno; set => locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseMask { get => locationMapWarehouseMask; set => locationMapWarehouseMask = value; }
        public string LocationMapWarehouseSectionMask { get => locationMapWarehouseSectionMask; set => locationMapWarehouseSectionMask = value; }
        public bool LocationMapWarehouseIsSectionTracked { get => locationMapWarehouseIsSectionTracked; set => locationMapWarehouseIsSectionTracked = value; }
        public bool LocationMapWarehouseIsSlotTracked { get => locationMapWarehouseIsSlotTracked; set => locationMapWarehouseIsSlotTracked = value; }
        public string LocationMapWarehouseSlotCoordinate1Name { get => locationMapWarehouseSlotCoordinate1Name; set => locationMapWarehouseSlotCoordinate1Name = value; }
        public string LocationMapWarehouseSlotCoordinate2Name { get => locationMapWarehouseSlotCoordinate2Name; set => locationMapWarehouseSlotCoordinate2Name = value; }
        public string LocationMapWarehouseSlotCoordinate3Name { get => locationMapWarehouseSlotCoordinate3Name; set => locationMapWarehouseSlotCoordinate3Name = value; }
        public string LocationMapWarehouseSlotMask { get => locationMapWarehouseSlotMask; set => locationMapWarehouseSlotMask = value; }
        public string LocationMapWarehouseSlotCoordinate1Mask { get => locationMapWarehouseSlotCoordinate1Mask; set => locationMapWarehouseSlotCoordinate1Mask = value; }
        public string LocationMapWarehouseSlotCoordinate2Mask { get => locationMapWarehouseSlotCoordinate2Mask; set => locationMapWarehouseSlotCoordinate2Mask = value; }
        public string LocationMapWarehouseSlotCoordinate3Mask { get => locationMapWarehouseSlotCoordinate3Mask; set => locationMapWarehouseSlotCoordinate3Mask = value; }
    }
}
