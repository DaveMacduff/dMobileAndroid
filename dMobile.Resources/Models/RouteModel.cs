// <copyright file="RouteModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a route.
    /// </summary>
    [Table("tRoute")]
    public class RouteModel
    {
        /// <summary>
        /// Gets or sets the route reference number.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int RouteRefno { get; set; }

        /// <summary>
        /// Gets or sets the route code.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_RouteOnRouteCode", Order = 1, Unique = true)]
        public string RouteCode { get; set; }

        /// <summary>
        /// Gets or sets the route description.
        /// </summary>
        [NotNull]
        [Indexed(Name = "RouteOnRouteDescription", Order = 1, Unique = false)]
        public string RouteDescription { get; set; }

        /// <summary>
        /// Gets or sets the route day of the week.
        /// </summary>
        [NotNull]
        public string RouteDayOfWeek { get; set; }
    }
}