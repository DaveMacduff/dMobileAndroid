// <copyright file="Count.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class Count
    {
        private int ActivityFKRefno;
        private int activityCountType;
        private string activityCountDescription;

        public int ActivityRefno { get; set; }
        public int ActivitySubType { get; set; }
        public int ActivityFKRefno { get => this.ActivityFKRefno; set => this.ActivityFKRefno = value; }
        public string ActivityStringFKRefno { get; set; }
        public int WarehouseLocationMapRefno { get; set; }
        public int ActivityCountType { get => this.activityCountType; set => this.activityCountType = value; }
        public DateTime ActivityCountDateTime { get; set; }
        public string ActivityCountTypeDescription { get; set; }
        public string ActivityCountDescription { get => this.activityCountDescription; set => this.activityCountDescription = value; }
    }
}
