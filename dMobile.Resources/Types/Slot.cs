// <copyright file="Slot.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class Slot
    {
        private int locationMapSlotCode;
        private int slotCoord3LocationMapRefno;
        private int locationMapSlotCoordinateEnum;

        public int LocationMapRefno { get; set; }
        public int LocationMapSubType { get; set; }
        public int SectionLocationMapRefno { get; set; }
        public int LocationMapSlotCode { get => this.locationMapSlotCode; set => this.locationMapSlotCode = value; }
        public string LocationMapSlotName { get; set; }
        public int SlotCoord1LocationMapRefno { get; set; }
        public int SlotCoord2LocationMapRefno { get; set; }
        public int SlotCoord3LocationMapRefno { get => this.slotCoord3LocationMapRefno; set => this.slotCoord3LocationMapRefno = value; }
        public int LocationMapSlotFKRefno { get; set; }
        public string LocationMapSlotBarCode { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int LocationMapSlotCoordinateRefno { get; set; }
        public int LocationMapSlotCoordinateEnum { get => this.locationMapSlotCoordinateEnum; set => this.locationMapSlotCoordinateEnum = value; }
        public string LocationMapSlotCoordinateCode { get; set; }
    }
}
