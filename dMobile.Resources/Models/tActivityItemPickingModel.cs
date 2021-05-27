using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPicking"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPickingOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemPickingFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemPickingStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemPickingQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingBaseQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingWeightQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingQuantityShipped numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingBaseQuantityShipped numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingWeightQuantityShipped numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingCustomerProductUnitCode nvarchar(10) NOT NULL,"
    //+ "ActivityItemPickingDescription nvarchar(40) NOT NULL,"
    //+ "ActivityItemPickingQuantityAvailable numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingBaseQuantityAvailable numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingWeightQuantityAvailable numeric(11, 3) NOT NULL,"
    //+ "ActivityItemPickingIsReviewed bit NOT NULL, "

    //+ "CONSTRAINT ActivityItemPickingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemPickingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemPickingTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating picking item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType ON tActivityItemPicking(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemPickingOnActivityItemPickingFKRefno ON tActivityItemPicking(ActivityItemPickingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemPickingOnWarehouseLocationMapRefno ON tActivityItemPicking(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemPicking")]
    public class tActivityItemPickingModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPickingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemPickingOnActivityItemPickingFKRefno", Order = 1, Unique = true)]
        public int ActivityItemPickingFKRefno { get; set; }

        [NotNull]
        public string ActivityItemPickingStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityItemPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityShipped { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityShipped { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityShipped { get; set; }

        [NotNull]
        public string ActivityItemPickingCustomerProductUnitCode { get; set; }

        [NotNull]
        public string ActivityItemPickingDescription { get; set; }

        [NotNull]
        public decimal ActivityItemPickingQuantityAvailable { get; set; }

        [NotNull]
        public decimal ActivityItemPickingBaseQuantityAvailable { get; set; }

        [NotNull]
        public decimal ActivityItemPickingWeightQuantityAvailable { get; set; }

        [NotNull]
        public bool ActivityItemPickingIsReviewed { get; set; }
    }
}
