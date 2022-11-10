// <copyright file="Location.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Location
    {
        public int LocationMapWarehouseCode { get; set; }
        public string LocationMapSectionName { get; set; }
        public string LocationMapSlotName { get; set; }
        public string LocationMapType { get; set; }
    }
}
