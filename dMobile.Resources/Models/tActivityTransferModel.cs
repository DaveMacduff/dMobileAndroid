using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityTransfer"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL CONSTRAINT PK_ActivityTransferOnActivityRefno PRIMARY KEY,"
    //+ "ActivitySubType INTEGER NOT NULL,"
    //+ "ActivityTransferFKRefno INTEGER NOT NULL,"
    //+ "ActivityTransferStringFKRefno nvarchar(8) NOT NULL,"
    //+ "TransferToWarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityTransferDescription nvarchar(30) NOT NULL,"
    //+ "ActivityTransferDateTime datetime NOT NULL,"
    //+ "ActivityTransferArrivalDateTime datetime NOT NULL, "

    //+ "CONSTRAINT ActivityTransferToActivityOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivity(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityTransferToLocationMapWarehouseOnTransferToWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (TransferToWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating transfer table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityTransferOnActivityRefnoAndActivitySubType ON tActivityTransfer(ActivityRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityTransferOnActivityTransferFKRefnoAndActivitySubType ON tActivityTransfer(ActivityTransferFKRefno ASC, ActivitySubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityTransferOnTransferToWarehouseLocationMapRefno ON tActivityTransfer(TransferToWarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityTransfer")]
    public class tActivityTransferModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityModel)), Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityTransferOnActivityRefnoAndActivitySubType", Order = 2, Unique = true)]
        public int ActivitySubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityTransferOnActivityTransferFKRefno", Order = 1, Unique = true)]
        public int ActivityTransferFKRefno { get; set; }

        [NotNull]
        public string ActivityTransferStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityTransferOnTransferToWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int TransferToWarehouseLocationMapRefno { get; set; }

        [NotNull]
        public string ActivityTransferDescription { get; set; }

        [NotNull]
        public DateTime ActivityTransferDateTime { get; set; }

        [NotNull]
        public DateTime ActivityTransferArrivalDateTime { get; set; }
    }
}
