namespace dMobile.Resources.Types
{
    public class Slot
    {
        private int locationMapRefno;
        private int locationMapSubType;
        private int sectionLocationMapRefno;
        private int locationMapSlotCode;
        private string locationMapSlotName;
        private int slotCoord1LocationMapRefno;
        private int slotCoord2LocationMapRefno;
        private int slotCoord3LocationMapRefno;
        private int locationMapSlotFKRefno;
        private string locationMapSlotBarCode;
        private int warehouseLocationMapRefno;
        private int locationMapSlotCoordinateRefno;
        private int locationMapSlotCoordinateEnum;
        private string locationMapSlotCoordinateCode;

        public int LocationMapRefno { get => locationMapRefno; set => locationMapRefno = value; }
        public int LocationMapSubType { get => locationMapSubType; set => locationMapSubType = value; }
        public int SectionLocationMapRefno { get => sectionLocationMapRefno; set => sectionLocationMapRefno = value; }
        public int LocationMapSlotCode { get => locationMapSlotCode; set => locationMapSlotCode = value; }
        public string LocationMapSlotName { get => locationMapSlotName; set => locationMapSlotName = value; }
        public int SlotCoord1LocationMapRefno { get => slotCoord1LocationMapRefno; set => slotCoord1LocationMapRefno = value; }
        public int SlotCoord2LocationMapRefno { get => slotCoord2LocationMapRefno; set => slotCoord2LocationMapRefno = value; }
        public int SlotCoord3LocationMapRefno { get => slotCoord3LocationMapRefno; set => slotCoord3LocationMapRefno = value; }
        public int LocationMapSlotFKRefno { get => locationMapSlotFKRefno; set => locationMapSlotFKRefno = value; }
        public string LocationMapSlotBarCode { get => locationMapSlotBarCode; set => locationMapSlotBarCode = value; }
        public int WarehouseLocationMapRefno { get => warehouseLocationMapRefno; set => warehouseLocationMapRefno = value; }
        public int LocationMapSlotCoordinateRefno { get => locationMapSlotCoordinateRefno; set => locationMapSlotCoordinateRefno = value; }
        public int LocationMapSlotCoordinateEnum { get => locationMapSlotCoordinateEnum; set => locationMapSlotCoordinateEnum = value; }
        public string LocationMapSlotCoordinateCode { get => locationMapSlotCoordinateCode; set => locationMapSlotCoordinateCode = value; }
    }
}
