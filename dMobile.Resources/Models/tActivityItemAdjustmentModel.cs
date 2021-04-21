using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemAdjustment"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemAdjustmentOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemAdjustmentFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemAdjustmentStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemAdjustmentQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemAdjustmentBaseQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemAdjustmentWeightQuantity numeric(11, 3) NOT NULL,"
    //+ "ActivityItemAdjustmentDescription nvarchar(42) NOT NULL, "

    //+ "CONSTRAINT ActivityItemAdjustmentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemAdjustmentToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemAdjustmentTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item adjustment table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType ON tActivityItemAdjustment(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno ON tActivityItemAdjustment(ActivityItemAdjustmentFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemAdjustmentOnWarehouseLocationMapRefno ON tActivityItemAdjustment(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemAdjustment")]
    public class tActivityItemAdjustmentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemAdjustmentOnActivityItemAdjustmentFKRefno", Order = 1, Unique = true)]
        public int ActivityItemAdjustmentFKRefno { get; set; }

        [NotNull]
        public string ActivityItemAdjustmentStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), OneToMany(CascadeOperations = CascadeOperation.All)]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_ActivityItemAdjustmentOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemAdjustmentWeightQuantity { get; set; }

        [NotNull]
        public string ActivityItemAdjustmentDescription { get; set; }
    }
}