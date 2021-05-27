using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemBarCodeAssociationXRef"
    //+ "("

    //+ "ActivityItemRefno INTEGER NOT NULL,"
    //+ "ProductUnitBarCodeRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityItemBarCodeAssociationXRefTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemBarCodeAssociationXRefToProductUnitBarCodeOnProductUnitBarCodeRefno FOREIGN KEY (ProductUnitBarCodeRefno) REFERENCES tProductUnitBarCode(ProductUnitBarCodeRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    this.MsgEventArgs.StatusOutMsg = "Creating bar code association item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS "
    //            + "PK_ActivityItemBarCodeAssociationXRefOnActivityItemRefnoAndProductUnitBarCodeRefno ON "
    //            + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC, ProductUnitBarCodeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemBarCodeAssociationXRefOnActivityItemRefno ON "
    //            + "tActivityItemBarCodeAssociationXRef(ActivityItemRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemBarCodeAssociationXRefOnProductUnitBarCodeRefno ON "
    //            + "tActivityItemBarCodeAssociationXRef(ProductUnitBarCodeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemBarCodeAssociationXRef")]
    public class tActivityItemBarCodeAssociationXRefModel
    {
        [Indexed(Name = "FK_ActivityItemBarCodeAssociationXRefOnActivityItemRefno", Order = 1, Unique = true), NotNull, ForeignKey(typeof(tActivityItemModel))]
        public int ActivityItemRefno { get; set; }

        [Indexed(Name = "FK_ActivityItemBarCodeAssociationXRefOnProductUnitBarCodeRefno", Order = 1, Unique = true), NotNull, ForeignKey(typeof(tProductUnitBarCodeModel))]
        public int ProductUnitBarCodeRefno { get; set; }
    }
}
