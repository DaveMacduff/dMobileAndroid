// <copyright file="tProductSchemeVariantModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductSchemeVariant"
    //+ "("
    //+ "ProductSchemeVariantRefno INTEGER NOT NULL CONSTRAINT PK_ProductSchemeVariantOnProductSchemeVariantRefno PRIMARY KEY,"
    //+ "ProductSchemeVariantCode nvarchar(5) NOT NULL,"
    //+ "ProductSchemeVariantDescription nvarchar(30) NOT NULL,"
    //+ "ProductSchemeVariantNo INTEGER NOT NULL,"
    //+ "ProductSchemeVariantIsDefault bit NOT NULL "
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product scheme variants table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeVariantOnProductSchemeVariantCode ON tProductSchemeVariant(ProductSchemeVariantCode ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeVariantOnProductSchemeVariantNo ON tProductSchemeVariant(ProductSchemeVariantNo ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductSchemeVariant")]
    public class tProductSchemeVariantModel
    {
        [PrimaryKey, NotNull]
        public int ProductSchemeVariantRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantCode", Order = 1, Unique = true)]
        public string ProductSchemeVariantCode { get; set; }

        [NotNull]
        public string ProductSchemeVariantDescription { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductSchemeVariantOnProductSchemeVariantNo", Order = 1, Unique = true)]
        public int ProductSchemeVariantNo { get; set; }

        [NotNull]
        public bool ProductSchemeVariantIsDefault { get; set; }
    }
}
