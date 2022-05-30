// <copyright file="tProductModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //    string sqlTable = "CREATE TABLE IF NOT EXISTS tProduct"
    //+ "("
    //+ "ProductRefno INTEGER NOT NULL CONSTRAINT PK_ProductOnProductRefno PRIMARY KEY,"
    //+ "ProductSchemeRefno INTEGER NOT NULL,"
    //+ "FamilyRefno INTEGER NOT NULL,"
    //+ "ProductCode nvarchar(10) NOT NULL,"
    //+ "ProductFKRefno INTEGER NOT NULL,"
    //+ "ProductDescription nvarchar(25) NOT NULL,"
    //+ "ProductIsCatchCount bit NOT NULL,"
    //+ "ProductIsCatchWeight bit NOT NULL,"
    //+ "ProductIsCatchAsk bit NOT NULL,"
    //+ "ProductIsCatchAskZero bit NOT NULL,"
    //+ "ProductBrandDescription nvarchar(10) NOT NULL,"
    //+ "ProductAllowConvertProduct bit NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //  this.MsgEventArgs.StatusOutMsg = "Creating products table.";
    //  this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //    await this.RunDDLCommand(sqlTable);

    //  string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductOnProductCode ON tProduct(ProductCode ASC)";
    //    await this.RunDDLCommand(sqlNdx);

    //    sqlNdx = "CREATE INDEX IF NOT EXISTS ProductOnProductDescription ON tProduct(ProductDescription ASC)";

    //    await this.RunDDLCommand(sqlNdx);

    //    sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductOnProductFKRefno ON tProduct(ProductFKRefno ASC)";

    //    await this.RunDDLCommand(sqlNdx);
    [Table("tProduct")]
    public class tProductModel
    {
        [PrimaryKey, NotNull]
        public int ProductRefno { get; set; }

        [NotNull]
        public int ProductSchemeRefno { get; set; }

        [NotNull]
        public int FamilyRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductOnProductCode", Order = 1, Unique = true)]
        public string ProductCode { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductOnProductFKRefno", Order = 1, Unique = true)]
        public int ProductFKRefno { get; set; }

        [NotNull, Indexed(Name = "ProductOnProductDescription", Order = 1, Unique = false)]
        public string ProductDescription { get; set; }

        [NotNull]
        public bool ProductIsCatchCount { get; set; }

        [NotNull]
        public bool ProductIsCatchWeight { get; set; }

        [NotNull]
        public bool ProductIsCatchAsk { get; set; }

        [NotNull]
        public bool ProductIsCatchAskZero { get; set; }

        [NotNull]
        public string ProductBrandDescription { get; set; }

        [NotNull]
        public bool ProductAllowConvertProduct { get; set; }
    }
}