// <copyright file="tProductUnitModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnit"
    //+ "("
    //+ "ProductUnitRefno INTEGER NOT NULL CONSTRAINT PK_ProductUnitOnProductUnitRefno PRIMARY KEY,"
    //+ "ProductRefno INTEGER NOT NULL,"
    //+ "UnitRefno INTEGER NOT NULL,"
    //+ "BaseUnitRefno INTEGER NOT NULL,"
    //+ "WtUnitRefno INTEGER NOT NULL,"
    //+ "ProductUnitIsBC bit NOT NULL,"
    //+ "ProductUnitBC nvarchar(1) NOT NULL,"
    //+ "ProductUnitWeight numeric(11, 3) NOT NULL,"
    //+ "ProductUnitSizeDescription nvarchar(15) NOT NULL,"
    //+ "ProductUnitIsSellUnit bit NOT NULL,"
    //+ "ProductUnitIsDefault bit NOT NULL,"
    //+ "ProductUnitBoxCount numeric(10, 4) NOT NULL, "

    //+ "CONSTRAINT ProductUnitToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ProductUnitToUnitOnUnitRefno FOREIGN KEY (UnitRefno) REFERENCES tUnit(UnitRefno), "
    //+ "CONSTRAINT ProductUnitToUnitOnBaseUnitRefno FOREIGN KEY (BaseUnitRefno) REFERENCES tUnit(UnitRefno), "
    //+ "CONSTRAINT ProductUnitToUnitOnWtUnitRefno FOREIGN KEY (WtUnitRefno) REFERENCES tUnit(UnitRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product units table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnProductRefno ON tProductUnit(ProductRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnUnitRefno ON tProductUnit(UnitRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnBaseUnitRefno ON tProductUnit(BaseUnitRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitOnWtUnitRefno ON tProductUnit(WtUnitRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductUnitOnProductRefnoAndUnitRefnoAndProductUnitIsBC ON tProductUnit(ProductRefno ASC, UnitRefno ASC, ProductUnitIsBC ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductUnit")]
    public class tProductUnitModel
    {
        [PrimaryKey, NotNull]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductModel)), Indexed(Name = "FK_ProductUnitOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull, ForeignKey(typeof(tUnitModel)), Indexed(Name = "FK_ProductUnitOnUnitRefno", Order = 1, Unique = false)]
        public int UnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tUnitModel)), Indexed(Name = "FK_ProductUnitOnBaseUnitRefno", Order = 1, Unique = false)]
        public int BaseUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tUnitModel)), Indexed(Name = "FK_ProductUnitOnWtUnitRefno", Order = 1, Unique = false)]
        public int WtUnitRefno { get; set; }

        [NotNull]
        public bool ProductUnitIsBC { get; set; }

        [NotNull]
        public string ProductUnitBC { get; set; }

        [NotNull]
        public decimal ProductUnitWeight { get; set; }

        [NotNull]
        public string ProductUnitSizeDescription { get; set; }

        [NotNull]
        public bool ProductUnitIsSellUnit { get; set; }

        [NotNull]
        public bool ProductUnitIsDefault { get; set; }

        [NotNull]
        public decimal ProductUnitBoxCount { get; set; }

        [NotNull]
        public decimal ProductUnitWeightTolerance { get; set; }
    }
}
