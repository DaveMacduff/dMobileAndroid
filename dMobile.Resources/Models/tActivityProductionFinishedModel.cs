using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityProductionFinished"
    //+ "("
    //+ "ActivityRefno INTEGER NOT NULL,"
    //+ "ActivityProductionFKRefno INTEGER NOT NULL,"
    //+ "ActivityProductionStringFKRefno nvarchar(8) NOT NULL,"
    //+ "ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber nvarchar(20) NOT NULL, "

    //+ "CONSTRAINT ActivityProductionFinishedToActivityProductionOnActivityRefno FOREIGN KEY (ActivityRefno) REFERENCES tActivityProduction(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating production finished table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ActivityProductionFinishedOnActivityRefno ON tActivityProductionFinished(ActivityRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityProductionFinishedOnActivityProductionFKRefno ON tActivityProductionFinished(ActivityProductionFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityProductionFinished")]
    public class tActivityProductionFinishedModel
    {
        [NotNull, ForeignKey(typeof(tActivityProductionModel)), Indexed(Name = "PK_ActivityProductionFinishedOnActivityRefno", Order = 1, Unique = true)]
        public int ActivityRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityProductionFinishedOnActivityProductionFKRefno", Order = 1, Unique = true)]
        public int ActivityProductionFKRefno { get; set; }

        [NotNull]
        public string ActivityProductionStringFKRefno { get; set; }

        [NotNull]
        public string ActivityProductionFinishedDefaultLogisticUnitBatchLotNumber { get; set; }
    }
}
