// <copyright file="ActivityPickingRouteModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    /// <summary>
    /// Represents an activity picking route.
    /// </summary>
    [Table("tActivityPickingRoute")]
    public class ActivityPickingRouteModel
    {
        /// <summary>
        /// Gets or sets the reference number of the picking activity.
        /// </summary>
        [PrimaryKey]
        [NotNull]
        [ForeignKey(typeof(ActivityPickingModel))]
        [Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 1, Unique = true)]
        public int PickingActivityRefno { get; set; }

        /// <summary>
        /// Gets or sets the reference number of the route.
        /// </summary>
        [NotNull]
        [ForeignKey(typeof(RouteModel))]
        [Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 2, Unique = true)]
        public int RouteRefno { get; set; }

        /// <summary>
        /// Gets or sets the stop number of the route.
        /// </summary>
        [NotNull]
        [Indexed(Name = "FK_ActivityPickingRouteOnRouteRefno", Order = 1, Unique = false)]
        public int RouteStop { get; set; }
    }
}