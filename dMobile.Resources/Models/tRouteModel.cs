// <copyright file="tRouteModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tRoute"
    //+ "("
    //+ "RouteRefno INTEGER NOT NULL CONSTRAINT PK_RouteOnRouteRefno PRIMARY KEY,"
    //+ "RouteCode nvarchar(5) NOT NULL,"
    //+ "RouteDescription nvarchar(35) NOT NULL,"
    //+ "RouteDayOfWeek nvarchar(3) NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating routes table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_RouteOnRouteCode ON tRoute(RouteCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS RouteOnRouteDescription ON tRoute(RouteDescription ASC)";
    //await this.RunDDLCommand(sqlNdx);
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
