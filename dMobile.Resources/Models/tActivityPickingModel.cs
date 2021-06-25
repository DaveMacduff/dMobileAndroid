using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityPicking"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityPickingOnActivityRefno PRIMARY KEY,"
    //+ "ActivitySubType INTEGER NOT NULL,"
    //+ "ActivityPickingFKRefno INTEGER NOT NULL,"
    //+ "ActivityPickingStringFKRefno nvarchar(8) NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityPickingOrderedDateTime datetime NOT NULL,"
    //+ "ActivityPickingDeliveryDateTime datetime NOT NULL,"
    //+ "ActivityPickingCustomerContactName nvarchar(40) NOT NULL,"
    //+ "ActivityPickingCustomerShipToInfo nvarchar(40) NOT NULL,"
    //+ "ActivityPickingIsReviewed bit NOT NULL, "

    //+ "CONSTRAINT ActivityPickingToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityPickingToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating picking table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingOnActivityRefnoAndActivitySubType "
    //            + "ON tActivityPicking(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingOnActivityPickingFKRefno ON "
    //            + "tActivityPicking(ActivityPickingFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityPickingOnWarehouseLocationMapRefno ON "
    //            + "tActivityPicking(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityPicking")]
    public class tActivityPickingModel
    {
        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 1, Unique = true), PrimaryKey, NotNull, ForeignKey(typeof(tActivityModel))]
        public int ActivityRefno { get; set; }

        [Indexed(Name = "UQ_ActivityPickingOnActivityRefnoAndActivitySubType", Order = 2, Unique = true), NotNull]
        public int ActivitySubType { get; set; }

        [Indexed(Name = "UQ_ActivityPickingOnActivityPickingFKRefno", Order = 1, Unique = true), NotNull]
        public int ActivityPickingFKRefno { get; set; }

        [NotNull]
        public string ActivityPickingStringFKRefno { get; set; }

        [Indexed(Name = "FK_ActivityPickingOnWarehouseLocationMapRefno", Order = 1, Unique = false), ForeignKey(typeof(tLocationMapWarehouseModel)), NotNull]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityPickingOrderedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityPickingDeliveryDateTime { get; set; }

        [NotNull]
        public string ActivityPickingCustomerContactName { get; set; }

        [NotNull]
        public string ActivityPickingCustomerShipToInfo { get; set; }

        [NotNull]
        public bool ActivityPickingIsReviewed { get; set; }
    }
}
