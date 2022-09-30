// <copyright file="Warehouse.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class Warehouse
    {
        public string DisplayName { get; set; }
        public int LocationMapRefno { get; set; }
        public int LocationMapSubType { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public int LocationMapWarehouseCode { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public string LocationMapWarehouseBarCode { get; set; }
    }
}
