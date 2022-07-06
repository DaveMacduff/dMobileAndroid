// <copyright file="tRouteModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    [Table("tRoute")]
    public class tRouteModel
    {
        [PrimaryKey, NotNull]
        public int RouteRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_RouteOnRouteCode", Order = 1, Unique = true)]
        public string RouteCode { get; set; }

        [NotNull, Indexed(Name = "RouteOnRouteDescription", Order = 1, Unique = false)]
        public string RouteDescription { get; set; }

        [NotNull]
        public string RouteDayOfWeek { get; set; }
    }
}
