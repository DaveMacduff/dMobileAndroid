// <copyright file="tContactVendorModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tContactVendor"
    //+ "("

    //+ "ContactRefno INTEGER NOT NULL CONSTRAINT PK_UserOnContactRefno PRIMARY KEY,"
    //+ "ContactSubType INTEGER NOT NULL,"
    //+ "ContactVendorFKRefno INTEGER NOT NULL,"
    //+ "ContactVendorCode nvarchar(8) NOT NULL,"
    //+ "ContactVendorName nvarchar(40) NOT NULL, "

    //+ "CONSTRAINT ContactVendorToContactOnContactRefno FOREIGN KEY (ContactRefno) REFERENCES tContact(ContactRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating contact vendors table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactRefnoAndContactSubType ON tContactVendor(ContactRefno ASC, ContactSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorFKRefno ON tContactVendor(ContactVendorFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorCode ON tContactVendor(ContactVendorCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ContactVendorOnContactVendorName ON tContactVendor(ContactVendorName ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tContactVendor")]
    public class tContactVendorModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tContactModel)), Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 1, Unique = true)]
        public int ContactRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactRefnoAndContactSubType", Order = 2, Unique = true)]
        public int ContactSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorFKRefno", Order = 1, Unique = true)]
        public int ContactVendorFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorCode", Order = 1, Unique = true)]
        public string ContactVendorCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ContactVendorOnContactVendorName", Order = 1, Unique = true)]
        public string ContactVendorName { get; set; }
    }
}
