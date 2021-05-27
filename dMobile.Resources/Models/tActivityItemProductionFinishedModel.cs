using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemProductionFinished"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemProductionFinishedOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemProductionFinishedFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemProductionFinishedStringFKRefno nvarchar(8) NOT NULL, "

    //+ "CONSTRAINT ActivityItemProductionFinishedTactItemProductionOnActivityRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemProduction(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item production finished table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno ON tActivityItemProductionFinished(ActivityItemProductionFinishedFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemProductionFinished")]
    public class tActivityItemProductionFinishedModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionFinishedOnActivityItemProductionFinishedFKRefno", Order = 1, Unique = true)]
        public int ActivityItemProductionFinishedFKRefno { get; set; }

        [NotNull]
        public string ActivityItemProductionFinishedStringFKRefno { get; set; }
    }
}
