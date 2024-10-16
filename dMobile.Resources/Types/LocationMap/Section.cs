// <copyright file="Section.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.LocationMap
{
    public class Section
    {
        public int LocationMapRefno { get; set; }
        public int LocationMapSubType { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int LocationMapSectionCode { get; set; }
        public string LocationMapSectionName { get; set; }
        public int LocationMapSectionFKRefno { get; set; }
        public string LocationMapSectionBarCode { get; set; }
    }
}
