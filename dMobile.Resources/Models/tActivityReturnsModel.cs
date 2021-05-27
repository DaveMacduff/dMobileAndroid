using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityReturns"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityReturnsOnActivityRefno PRIMARY KEY,"
    //+ "ActivitySubType INTEGER NOT NULL,"
    //+ "ActivityReturnsFKRefno INTEGER NOT NULL,"
    //+ "ActivityReturnsStringFKRefno nvarchar(8) NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityReturnsOrderedDateTime datetime NOT NULL,"
    //+ "ActivityReturnsDeliveryDateTime datetime NOT NULL,"
    //+ "ActivityReturnsCustomerContactName nvarchar(40) NOT NULL,"
    //+ "ActivityReturnsCustomerShipToInfo nvarchar(40) NOT NULL, "

    //+ "CONSTRAINT ActivityReturnsToActivityOnActivityRefnoAndActivitySubType FOREIGN KEY (ActivityRefno, ActivitySubType) REFERENCES tActivity(ActivityRefno, ActivitySubType) ON DELETE CASCADE, "
    //+ "CONSTRAINT ActivityReturnsToLocationMapWarehouseOnWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE NO ACTION"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating returns table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReturnsOnActivityRefnoAndActivitySubType ON tActivityReturns(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityReturnsOnActivityReturnsFKRefno ON tActivityReturns(ActivityReturnsFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityReturnsOnWarehouseLocationMapRefno ON tActivityReturns(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityReturns")]
    public class tActivityReturnsModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityModel)), Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityReturnsOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityReturnsOnActivityReturnsFKRefno", Order = 1, Unique = true)]
        public int ActivityReturnsFKRefno { get; set; }

        [NotNull]
        public string ActivityReturnsStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityReturnsOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull]
        public DateTime ActivityReturnsOrderedDateTime { get; set; }

        [NotNull]
        public DateTime ActivityReturnsDeliveryDateTime { get; set; }

        [NotNull]
        public string ActivityReturnsCustomerContactName { get; set; }

        [NotNull]
        public string ActivityReturnsCustomerShipToInfo { get; set; }
    }
}
