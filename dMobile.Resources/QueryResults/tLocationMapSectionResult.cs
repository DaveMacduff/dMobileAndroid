// <copyright file="tLocationMapSectionResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.QueryResults
{
    // string query = "SELECT tLocationMapSection.LocationMapRefno, "
    // + "tLocationMapSection.LocationMapSectionCode, "
    // + "tLocationMapSection.LocationMapSectionName, "
    // + "tLocationMapSection.LocationMapSectionFKRefno, "
    // + "tLocationMapSection.WarehouseLocationMapRefno, "
    // + "tLocationMapWarehouse.LocationMapWarehouseName, "
    // + "tLocationMapWarehouse.LocationMapWarehouseCode, "
    // + "tLocationMapWarehouse.LocationMapWarehouseIsWMS, "
    // + "tLocationMapWarehouse.LocationMapWarehouseFKRefno, "
    // + "tLocationMapWarehouse.LocationMapWarehouseMask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSectionMask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotMask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Mask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Mask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Mask, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate1Name, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate2Name, "
    // + "tLocationMapWarehouse.LocationMapWarehouseSlotCoordinate3Name, "
    // + "tLocationMapWarehouse.LocationMapWarehouseIsSectionTracked, "
    // + "tLocationMapWarehouse.LocationMapWarehouseIsSlotTracked, "
    // + "tLocationMapWarehouse.LocationMapWarehouseAdhocEnabled "
    // + "FROM tLocationMapSection "
    // + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno "
    // + "= tLocationMapSection.WarehouseLocationMapRefno "
    // + "WHERE tLocationMapSection.LocationMapRefno = ?";    
    public class tLocationMapSectionQueryResult
    {
        private int locationMapSectionCode;
        private int larehouseLocationMapRefno;
        private string locationMapWarehouseSectionMask;
        private bool locationMapWarehouseIsSlotTracked;
        private string locationMapWarehouseSlotCoordinate3Name;
        private string locationMapWarehouseSlotCoordinate3Mask;

        public int LocationMapRefno { get; set; }
        public int LocationMapSectionCode { get => this.locationMapSectionCode; set => this.locationMapSectionCode = value; }
        public string LocationMapSectionName { get; set; }
        public int LocationMapSectionFKRefno { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int LarehouseLocationMapRefno { get => this.larehouseLocationMapRefno; set => this.larehouseLocationMapRefno = value; }
        public string LocationMapWarehouseName { get; set; }
        public int LocationMapWarehouseCode { get; set; }
        public bool LocationMapWarehouseIsWMS { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseMask { get; set; }
        public string LocationMapWarehouseSectionMask { get => this.locationMapWarehouseSectionMask; set => this.locationMapWarehouseSectionMask = value; }
        public bool LocationMapWarehouseIsSectionTracked { get; set; }
        public bool LocationMapWarehouseIsSlotTracked { get => this.locationMapWarehouseIsSlotTracked; set => this.locationMapWarehouseIsSlotTracked = value; }
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }
        public string LocationMapWarehouseSlotCoordinate3Name { get => this.locationMapWarehouseSlotCoordinate3Name; set => this.locationMapWarehouseSlotCoordinate3Name = value; }
        public string LocationMapWarehouseSlotMask { get; set; }
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }
        public string LocationMapWarehouseSlotCoordinate3Mask { get => this.locationMapWarehouseSlotCoordinate3Mask; set => this.locationMapWarehouseSlotCoordinate3Mask = value; }
    }
}
