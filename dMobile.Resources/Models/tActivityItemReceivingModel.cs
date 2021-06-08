using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReceiving"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReceivingOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemReceivingFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemReceivingStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemReceivingQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingBaseQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingWeightQuantityOrdered numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingQuantityReceived numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingBaseQuantityReceived numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingWeightQuantityReceived numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingVendorProductUnitCode nvarchar(10) NOT NULL,"
    //+ "ActivityItemReceivingDescription nvarchar(40) NOT NULL,"
    //+ "ActivityItemReceivingQuantityReceivedToDate numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingBaseQuantityReceivedToDate numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReceivingWeightQuantityReceivedToDate numeric(11, 3) NOT NULL, "

    //+ "CONSTRAINT ActivityItemReceivingTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemReceivingToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemReceivingTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating receiving item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType ON tActivityItemReceiving(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno ON tActivityItemReceiving(ActivityItemReceivingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReceivingOnWarehouseLocationMapRefno ON tActivityItemReceiving(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemReceiving")]
    public class tActivityItemReceivingModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemReceivingOnActivityItemReceivingFKRefno", Order = 1, Unique = true)]
        public int ActivityItemReceivingFKRefno { get; set; }

        [NotNull]
        public string ActivityItemReceivingStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityItemReceivingOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityOrdered { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityReceived { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceived { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceived { get; set; }

        [NotNull]
        public string ActivityItemReceivingVendorProductUnitCode { get; set; }

        [NotNull]
        public string ActivityItemReceivingDescription { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingQuantityReceivedToDate { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingBaseQuantityReceivedToDate { get; set; }

        [NotNull]
        public decimal ActivityItemReceivingWeightQuantityReceivedToDate { get; set; }
    }
}
