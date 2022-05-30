// <copyright file="tProductSchemeLotNumberModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeLotNumber"
    //+ "("
    //+ "ProductSchemeLotNumberRefno INTEGER NOT NULL CONSTRAINT PK_ProductSchemeLotNumberOnProductSchemeLotNumberRefno PRIMARY KEY,"
    //+ "ProductSchemeLotNumberCode nvarchar(20) NOT NULL,"
    //+ "ProductSchemeLotNumberCodeDefault nvarchar(20) NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product scheme lots table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode ON tProductSchemeLotNumber(ProductSchemeLotNumberCode ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductSchemeLotNumber")]
    public class tProductSchemeLotNumberModel
    {
        [PrimaryKey, NotNull]
        public int ProductSchemeLotNumberRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductSchemeLotNumberOnProductSchemeLotNumberCode", Order = 1, Unique = true)]
        public string ProductSchemeLotNumberCode { get; set; }

        [NotNull]
        public string ProductSchemeLotNumberCodeDefault { get; set; }
    }
}
