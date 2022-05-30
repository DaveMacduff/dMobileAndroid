// <copyright file="tLocationMapSlotCoordinateModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapSlotCoordinate"
    //+ "("

    //+ "LocationMapSlotCoordinateRefno INTEGER NOT NULL CONSTRAINT PK_LocationMapSlotCoordinateRefno PRIMARY KEY,"
    //+ "LocationMapSlotCoordinateEnum INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno int NOT NULL,"
    //+ "LocationMapSlotCoordinateCode nvarchar(10) NOT NULL"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating location map slot table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode ON tLocationMapSlotCoordinate(LocationMapSlotCoordinateEnum ASC, WarehouseLocationMapRefno ASC, LocationMapSlotCoordinateCode ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tLocationMapSlotCoordinate")]
    public class tLocationMapSlotCoordinateModel
    {
        [PrimaryKey, NotNull]
        public int LocationMapSlotCoordinateRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 1, Unique = true)]
        public int LocationMapSlotCoordinateEnum { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 2, Unique = true)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_LocationMapSlotCoordOnLocationMapSlotCoordEnumAndWarehouseLocationMapRefnoAndLocationMapSlotCoordCode", Order = 3, Unique = true)]
        public string LocationMapSlotCoordinateCode { get; set; }
    }
}
