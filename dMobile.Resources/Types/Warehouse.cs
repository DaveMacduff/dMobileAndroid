// <copyright file="Warehouse.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class Warehouse
    {
        private int locationMapSubType;
        private int locationMapWarehouseFKRefno;

        public string DisplayName { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapSubType { get => this.locationMapSubType; set => this.locationMapSubType = value; }
        public string LocationMapWarehouseName { get; set; }
        public int LocationMapWarehouseCode { get; set; }
        public int LocationMapWarehouseFKRefno { get => this.locationMapWarehouseFKRefno; set => this.locationMapWarehouseFKRefno = value; }
        public string LocationMapWarehouseBarCode { get; set; }
    }
}
