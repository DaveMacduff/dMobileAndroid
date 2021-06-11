using System;
using System.Collections.Generic;
using System.Text;

namespace dMobile.Resources.Types
{
    public class Location
    {
        private int locationMapWarehouseCode;
        private string locationMapSectionName;
        private string locationMapSlotName;
        private string locationMapType;

        public int LocationMapWarehouseCode { get => locationMapWarehouseCode; set => locationMapWarehouseCode = value; }
        public string LocationMapSectionName { get => locationMapSectionName; set => locationMapSectionName = value; }
        public string LocationMapSlotName { get => locationMapSlotName; set => locationMapSlotName = value; }
        public string LocationMapType { get => locationMapType; set => locationMapType = value; }
    }
}
