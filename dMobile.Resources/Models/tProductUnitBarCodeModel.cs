using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //// 255 max len for a 1D barcode
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnitBarCode"
    //    + "("
    //    + "ProductUnitBarCodeRefno INTEGER NOT NULL CONSTRAINT PK_ProductUnitBarCodeOnProductUnitBarCodeRefno PRIMARY KEY,"
    //    + "ProductUnitRefno INTEGER NOT NULL,"
    //    + "ProductUnitBarCodeCode nvarchar(20) NOT NULL,"
    //    + "ProductUnitBarCodeRaw nvarchar(255) NOT NULL,"
    //    + "ProductUnitBarCodeSymbologyEnum INTEGER NOT NULL,"
    //    + "ProductUnitBarCodeModifier1 nvarchar(1) NOT NULL,"
    //    + "ProductUnitBarCodeStatus INTEGER NOT NULL, "

    //    + "CONSTRAINT ProductUnitBarCodeToProductUnitOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //    + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product unit bar codes table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitBarCodeOnProductUnitRefno ON tProductUnitBarCode(ProductUnitRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1 ON tProductUnitBarCode(ProductUnitBarCodeCode ASC, ProductUnitBarCodeModifier1 ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tProductUnitBarCode")]
    public class tProductUnitBarCodeModel
    {
        [PrimaryKey, NotNull]
        public int ProductUnitBarCodeRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_ProductUnitBarCodeOnProductUnitRefno", Order = 1, Unique = false)]
        public int ProductUnitRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1", Order = 1, Unique = true)]
        public string ProductUnitBarCodeCode { get; set; }

        [NotNull]
        public string ProductUnitBarCodeRaw { get; set; }

        [NotNull]
        public int ProductUnitBarCodeSymbologyEnum { get; set; }

        [NotNull, Indexed(Name = "UQ_ProductUnitBarCodeOnProductUnitBarCodeCodeAndProductUnitBarCodeModifier1", Order = 2, Unique = true)]
        public string ProductUnitBarCodeModifier1 { get; set; }

        [NotNull]
        public int ProductUnitBarCodeStatus { get; set; }
    }
}
