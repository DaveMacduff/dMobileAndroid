// <copyright file="tLocationMapSlotQueryResult.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.QueryResults
{
    using System;

    // string query = "SELECT tLocationMapSlot.LocationMapRefno, "
    // + "tLocationMapSlot.LocationMapSlotCode, "
    // + "tLocationMapSlot.LocationMapSlotName, "
    // + "tLocationMapSlot.LocationMapSlotFKRefno, "
    // + "tLocationMapSlot.SectionLocationMapRefno, "
    // + "tLocationMapSlot.SlotCoord1LocationMapRefno, "
    // + "tLocationMapSlot.SlotCoord2LocationMapRefno, "
    // + "tLocationMapSlot.SlotCoord3LocationMapRefno, "
    // + "tLocationMapSection.LocationMapSectionCode, "
    // + "tLocationMapSection.LocationMapSectionName, "
    // + "tLocationMapSection.LocationMapSectionFKRefno, "
    // + "tLocationMapSection.WarehouseLocationMapRefno, "
    // + "tLocationMapWarehouse.LocationMapWarehouseName, "
    // + "tLocationMapWarehouse.LocationMapWarehouseCode, "
    // + "tLocationMapWarehouse.LocationMapWarehouseIsWMS,  "
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
    // + "FROM tLocationMapSlot "
    // + "INNER JOIN tLocationMapSection ON tLocationMapSection.LocationMapRefno = tLocationMapSlot.SectionLocationMapRefno "
    // + "INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = tLocationMapSection.WarehouseLocationMapRefno "
    // + "WHERE tLocationMapSlot.LocationMapRefno = ?";
    public class tLocationMapSlotQueryResult
    {
        private string locationMapWarehouseSlotCoordinate3Mask;

        public int LocationMapRefno { get; set; }
        public int LocationMapSlotCode { get; set; }
        public string LocationMapSlotName { get; set; }
        public int LocationMapSlotFKRefno { get; set; }
        public int SlotCoord1LocationMapRefno { get; set; }
        public int SlotCoord2LocationMapRefno { get; set; }
        public int SlotCoord3LocationMapRefno { get; set; }
        public int SectionLocationMapRefno { get; set; }
        public int LocationMapSectionCode { get; set; }
        public string LocationMapSectionName { get; set; }
        public int LocationMapSectionFKRefno { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public int LocationMapWarehouseCode { get; set; }
        public bool LocationMapWarehouseIsWMS { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseMask { get; set; }
        public string LocationMapWarehouseSectionMask { get; set; }
        public bool LocationMapWarehouseIsSectionTracked { get; set; }
        public bool LocationMapWarehouseIsSlotTracked { get; set; }
        public string LocationMapWarehouseSlotCoordinate1Name { get; set; }
        public string LocationMapWarehouseSlotCoordinate2Name { get; set; }
        public string LocationMapWarehouseSlotCoordinate3Name { get; set; }
        public string LocationMapWarehouseSlotMask { get; set; }
        public string LocationMapWarehouseSlotCoordinate1Mask { get; set; }
        public string LocationMapWarehouseSlotCoordinate2Mask { get; set; }
        public string LocationMapWarehouseSlotCoordinate3Mask { get => this.locationMapWarehouseSlotCoordinate3Mask; set => this.locationMapWarehouseSlotCoordinate3Mask = value; }
    }
}
