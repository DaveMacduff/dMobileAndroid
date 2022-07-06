// <copyright file="tActivityPickingRouteModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityPickingRoute")]
    public class tActivityPickingRouteModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityPickingModel)), Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 1, Unique = true)]
        public int PickingActivityRefno { get; set; }

        [NotNull, ForeignKey(typeof(tRouteModel)), Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 2, Unique = true)]
        public int RouteRefno { get; set; }

        [NotNull, Indexed(Name = "FK_ActivityPickingRouteOnRouteRefno", Order = 1, Unique = false)]
        public int RouteStop { get; set; }
    }
}
