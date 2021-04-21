using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPurchaseOrderWorkSheet"
    // + "("
    // + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefno PRIMARY KEY,"
    // + "ActivityItemSubType INTEGER NOT NULL,"
    // + "ActivityItemPurchaseOrderWorkSheetFKRefno INTEGER NOT NULL,"
    // + "ActivityItemPurchaseOrderWorkSheetStringFKRefno nvarchar(8) NOT NULL,"
    // + "ProductUnitRefno INTEGER NOT NULL,"
    // + "WarehouseLocationMapRefno INTEGER NOT NULL,"
    // + "ActivityItemPurchaseOrderWorkSheetQuantityOrdered numeric(11, 3) NOT NULL,"
    // + "ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered numeric(11, 3) NOT NULL,"
    // + "ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered numeric(11, 3) NOT NULL, "

    // + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    // + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    // + "CONSTRAINT ActivityItemPurchaseOrderWorkSheetTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    // + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating purchase order work sheet item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType ON tActivityItemPurchaseOrderWorkSheet(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno ON tActivityItemPurchaseOrderWorkSheet(ActivityItemPurchaseOrderWorkSheetFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemPurchaseOrderWorkSheetOnWarehouseLocationMapRefno ON tActivityItemPurchaseOrderWorkSheet(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemPurchaseOrderWorkSheet")]
    public class tActivityItemPurchaseOrderWorkSheetModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPurchaseOrderWorkSheetOnActivityItemPurchaseOrderWorkSheetFKRefno", Order = 1, Unique = true)]
        public int ActivityItemPurchaseOrderWorkSheetFKRefno { get; set; }

        [NotNull]
        public string ActivityItemPurchaseOrderWorkSheetStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), OneToMany(CascadeOperations = CascadeOperation.All)]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_ActivityItemPurchaseOrderWorkSheetOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPurchaseOrderWorkSheetWeightQuantityOrdered { get; set; }
    }
}
