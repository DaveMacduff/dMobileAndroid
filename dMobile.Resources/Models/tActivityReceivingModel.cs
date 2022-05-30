// <copyright file="tActivityReceivingModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //// TODO: tActivityReceiving.ActivityReceivingVendorSupplierGTINCompanyPrefix is not being used as of Apr 4/2014
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityReceiving"
    //    + "("
    //    + "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityReceivingOnActivityRefno PRIMARY KEY,"
    //    + "ActivitySubType INTEGER NOT NULL,"
    //    + "ActivityReceivingFKRefno INTEGER NOT NULL,"
    //    + "ActivityReceivingStringFKRefno nvarchar(8) NOT NULL,"
    //    + "ActivityReceivingPONumber nvarchar(8) NOT NULL,"
    //    + "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //    + "ActivityReceivingIssuedDateTime datetime NOT NULL,"
    //    + "ActivityReceivingArrivalDateTime datetime NOT NULL,"
    //    + "ActivityReceivingVendorContactName nvarchar(40) NOT NULL,"
    //    + "ActivityReceivingVendorSupplierContactName nvarchar(40) NOT NULL,"
    //    + "ActivityReceivingVendorSupplierGTINCompanyPrefix nvarchar(11) NOT NULL, "

    //    + "CONSTRAINT ActivityReceivingToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //    + "CONSTRAINT ActivityReceivingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //    + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating receiving table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityRefnoAndActivitySubType ON tActivityReceiving(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityReceivingFKRefno ON tActivityReceiving(ActivityReceivingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReceivingOnActivityReceivingPONumber ON tActivityReceiving(ActivityReceivingPONumber ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityReceivingOnWarehouseLocationMapRefno ON tActivityReceiving(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityReceiving")]
    public class tActivityReceivingModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityModel)), Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityReceivingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityReceivingOnActivityReceivingFKRefno", Order = 1, Unique = true)]
        public int ActivityReceivingFKRefno { get; set; }

        [NotNull]
        public string ActivityReceivingStringFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityReceivingOnActivityReceivingPONumber", Order = 1, Unique = true)]
        public string ActivityReceivingPONumber { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityReceivingIssuedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityReceivingArrivalDateTime { get; set; }

        [NotNull]
        public string ActivityReceivingVendorContactName { get; set; }

        [NotNull]
        public string ActivityReceivingVendorSupplierContactName { get; set; }

        [NotNull]
        public string ActivityReceivingVendorSupplierGTINCompanyPrefix { get; set; }
    }
}
