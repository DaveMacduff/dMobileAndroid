// <copyright file="tLocationMapSlotQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.QueryResults
{
    using System;

    //string query = "SELECT tLocationMapSlot.LocationMapRefno, "
    //+ "tLocationMapSlot.LocationMapSlotCode, "
    //+ "tLocationMapSlot.LocationMapSlotName, "
    //+ "tLocationMapSlot.LocationMapSlotFKRefno, "
    //+ "tLocationMapSlot.SectionLocationMapRefno, "
    //+ "tLocationMapSlot.SlotCoord1LocationMapRefno, "
    //+ "tLocationMapSlot.SlotCoord2LocationMapRefno, "
    //+ "tLocationMapSlot.SlotCoord3LocationMapRefno, "
    //+ "tLocationMapSection.LocationMapSectionCode, "
    //+ "tLocationMapSection.LocationMapSectionName, "
    //+ "tLocationMapSection.LocationMapSectionFKRefno, "
    //+ "tLocationMapSection.WarehouseLocationMapRefno, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseName, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseCode, "
    //+ "tLocationMapWarehouse.LocationMapWarehouseIsWMS,  "
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
    //+ "FROM tLocationMapSlot "
    //+ "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tLocationMapSlot.SectionLocationMapRefno "
    //+ "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tLocationMapSection.WarehouseLocationMapRefno "
    //+ "WHERE tLocationMapSlot.LocationMapRefno = ?";
    public class tLocationMapSlotQueryResult
    {
        private int locationMapSlotCode;
        private int slotCoord3LocationMapRefno;
        private int locationMapSectionCode;
        private int warehouseLocationMapRefno;
        private string locationMapWarehouseSectionMask;
        private bool locationMapWarehouseIsSlotTracked;
        private string locationMapWarehouseSlotCoordinate3Name;
        private string locationMapWarehouseSlotCoordinate3Mask;

        public int LocationMapRefno { get; set; }
        public int LocationMapSlotCode { get => this.locationMapSlotCode; set => this.locationMapSlotCode = value; }
        public string LocationMapSlotName { get; set; }
        public int LocationMapSlotFKRefno { get; set; }
        public int SlotCoord1LocationMapRefno { get; set; }
        public int SlotCoord2LocationMapRefno { get; set; }
        public int SlotCoord3LocationMapRefno { get => this.slotCoord3LocationMapRefno; set => this.slotCoord3LocationMapRefno = value; }
        public int SectionLocationMapRefno { get; set; }
        public int LocationMapSectionCode { get => this.locationMapSectionCode; set => this.locationMapSectionCode = value; }
        public string LocationMapSectionName { get; set; }
        public int LocationMapSectionFKRefno { get; set; }
        public int WarehouseLocationMapRefno { get => this.warehouseLocationMapRefno; set => this.warehouseLocationMapRefno = value; }
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
