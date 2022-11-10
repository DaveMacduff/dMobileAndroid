// <copyright file="Adjustment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Adjustment : object
    {
        public int ActivityRefno { get; set; }
        public int ActivityFKRefno { get; set; }
        public int ActivityAdjustmentType { get; set; }
        public int LocationMapWarehouseFKRefno { get; set; }
        public int LocationMapRefno { get; set; }
        public int ActivityStatus { get; set; }
        public string ActivityStringFKRefno { get; set; }
        public string ActivityAdjustmentDescription { get; set; }
        public string ActivityAdjustmentTypeDescription { get; set; }
        public string LocationMapWarehouseName { get; set; }
        public DateTime ActivityAdjustmentDateTime { get; set; }
    }
}
