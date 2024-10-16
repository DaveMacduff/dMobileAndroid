// <copyright file="Count.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types.Headers
{
    using System;

    public class Count
    {
        public int ActivityRefno { get; set; }
        public int ActivitySubType { get; set; }
        public int ActivityFKRefno { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int ActivityCountType { get; set; }
        public DateTime ActivityCountDateTime { get; set; }
        public string ActivityCountTypeDescription { get; set; }
        public string ActivityCountDescription { get; set; }
    }
}
