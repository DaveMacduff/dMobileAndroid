// <copyright file="tProductSchemeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductScheme"
    //+ "("
    //+ "ProductSchemeRefno INTEGER NOT NULL CONSTRAINT PK_ProductSchemeOnProductSchemeRefno PRIMARY KEY,"
    //+ "ProductSchemeCode nvarchar(10) NOT NULL,"
    //+ "ProductSchemeIsSerialTracked bit NOT NULL,"
    //+ "ProductSchemeIsProductionDateTracked bit NOT NULL,"
    //+ "ProductSchemeIsLotTracked bit NOT NULL,"
    //+ "ProductSchemeIsBestBeforeDateTracked bit NOT NULL,"
    //+ "ProductSchemeIsVariantTracked bit NOT NULL,"
    //+ "ProductSchemeIsLotUniqueComponent bit NOT NULL,"
    //+ "ProductSchemeIsProductionDateUniqueComponent bit NOT NULL,"
    //+ "ProductSchemeIsBestBeforeDateUniqueComponent bit NOT NULL,"
    //+ "ProductSchemeIsVariantUniqueComponent bit NOT NULL,"
    //+ "ProductSchemeExtra1Type INTEGER NOT NULL,"
    //+ "ProductSchemeExtra2Type INTEGER NOT NULL,"
    //+ "ProductSchemeExtra3Type INTEGER NOT NULL,"
    //+ "ProductSchemeExtra4Type INTEGER NOT NULL,"
    //+ "ProductSchemeExtra5Type INTEGER NOT NULL,"
    //+ "ProductSchemeExtra1Caption nvarchar(10) NOT NULL,"
    //+ "ProductSchemeExtra2Caption nvarchar(10) NOT NULL,"
    //+ "ProductSchemeExtra3Caption nvarchar(10) NOT NULL,"
    //+ "ProductSchemeExtra4Caption nvarchar(10) NOT NULL,"
    //+ "ProductSchemeExtra5Caption nvarchar(10) NOT NULL,"
    //+ "ProductSchemeExtra1IsRequired bit NOT NULL,"
    //+ "ProductSchemeExtra2IsRequired bit NOT NULL,"
    //+ "ProductSchemeExtra3IsRequired bit NOT NULL,"
    //+ "ProductSchemeExtra4IsRequired bit NOT NULL,"
    //+ "ProductSchemeExtra5IsRequired bit NOT NULL,"
    //+ "ProductSchemeIsLotTrackedDefault bit NOT NULL,"
    //+ "ProductSchemeIsProductionDateTrackedDefault bit NOT NULL,"
    //+ "ProductSchemeIsBestBeforeDateTrackedDefault bit NOT NULL"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product scheme table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductSchemeOnProductSchemeCode ON tProductScheme(ProductSchemeCode ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductScheme")]
    public class tProductSchemeModel
    {
        [PrimaryKey, NotNull]
        public int ProductSchemeRefno { get; set; }
        [NotNull, Indexed(Name = "UQ_ProductSchemeOnProductSchemeCode", Order = 1, Unique = true)]
        public string ProductSchemeCode { get; set; }
        [NotNull]
        public bool ProductSchemeIsSerialTracked { get; set; }
        [NotNull]
        public bool ProductSchemeIsProductionDateTracked { get; set; }
        [NotNull]
        public bool ProductSchemeIsLotTracked { get; set; }
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTracked { get; set; }
        [NotNull]
        public bool ProductSchemeIsVariantTracked { get; set; }
        [NotNull]
        public bool ProductSchemeIsLotUniqueComponent { get; set; }
        [NotNull]
        public bool ProductSchemeIsProductionDateUniqueComponent { get; set; }
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateUniqueComponent { get; set; }
        [NotNull]
        public bool ProductSchemeIsVariantUniqueComponent { get; set; }
        [NotNull]
        public int ProductSchemeExtra1Type { get; set; }
        [NotNull]
        public int ProductSchemeExtra2Type { get; set; }
        [NotNull]
        public int ProductSchemeExtra3Type { get; set; }
        [NotNull]
        public int ProductSchemeExtra4Type { get; set; }
        [NotNull]
        public int ProductSchemeExtra5Type { get; set; }
        [NotNull]
        public string ProductSchemeExtra1Caption { get; set; }
        [NotNull]
        public string ProductSchemeExtra2Caption { get; set; }
        [NotNull]
        public string ProductSchemeExtra3Caption { get; set; }
        [NotNull]
        public string ProductSchemeExtra4Caption { get; set; }
        [NotNull]
        public string ProductSchemeExtra5Caption { get; set; }
        [NotNull]
        public bool ProductSchemeExtra1IsRequired { get; set; }
        [NotNull]
        public bool ProductSchemeExtra2IsRequired { get; set; }
        [NotNull]
        public bool ProductSchemeExtra3IsRequired { get; set; }
        [NotNull]
        public bool ProductSchemeExtra4IsRequired { get; set; }
        [NotNull]
        public bool ProductSchemeExtra5IsRequired { get; set; }
        [NotNull]
        public bool ProductSchemeIsLotTrackedDefault { get; set; }
        [NotNull]
        public bool ProductSchemeIsProductionDateTrackedDefault { get; set; }
        [NotNull]
        public bool ProductSchemeIsBestBeforeDateTrackedDefault { get; set; }
    }
}
