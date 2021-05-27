using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReturns"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReturnsOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemReturnsFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemReturnsStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemReturnsQuantityReturned numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReturnsBaseQuantityReturned numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReturnsWeightQuantityReturned numeric(11, 3) NOT NULL,"
    //+ "ActivityItemReturnsCustomerProductUnitCode nvarchar(10) NOT NULL,"
    //+ "ActivityItemReturnsDescription nvarchar(40) NOT NULL, "

    //+ "CONSTRAINT ActivityItemReturnsTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemReturnsToLocationMapWarehouseOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemReturnsTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating returns item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType ON tActivityItemReturns(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno ON tActivityItemReturns(ActivityItemReturnsFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemReturnsOnWarehouseLocationMapRefno ON tActivityItemReturns(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemReturns")]
    public class tActivityItemReturnsModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemReturnsOnActivityItemReturnsFKRefno", Order = 1, Unique = true)]
        public int ActivityItemReturnsFKRefno { get; set; }

        [NotNull]
        public string ActivityItemReturnsStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityItemReturnsOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemReturnsQuantityReturned { get; set; }

        [NotNull]
        public decimal ActivityItemReturnsBaseQuantityReturned { get; set; }

        [NotNull]
        public decimal ActivityItemReturnsWeightQuantityReturned { get; set; }

        [NotNull]
        public string ActivityItemReturnsCustomerProductUnitCode { get; set; }

        [NotNull]
        public string ActivityItemReturnsDescription { get; set; }
    }
}
