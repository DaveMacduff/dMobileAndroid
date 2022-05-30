// <copyright file="Section.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class Section
    {
        private int locationMapSectionCode;
        private string locationMapSectionBarCode;

        public int LocationMapRefno { get; set; }
        public int LocationMapSubType { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int LocationMapSectionCode { get => this.locationMapSectionCode; set => this.locationMapSectionCode = value; }
        public string LocationMapSectionName { get; set; }
        public int LocationMapSectionFKRefno { get; set; }
        public string LocationMapSectionBarCode { get => this.locationMapSectionBarCode; set => this.locationMapSectionBarCode = value; }
    }
}
