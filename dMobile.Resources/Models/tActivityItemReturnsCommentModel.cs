using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemReturnsComment"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemReturnsCommentOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemReturnsCommentComment nvarchar(255) NOT NULL, "

    //+ "CONSTRAINT ActivityItemReturnsCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemReturns(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating returns comment item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    [Table("tActivityItemReturnsComment")]
    public class tActivityItemReturnsCommentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemReturnsModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemReturnsCommentComment { get; set; }
    }
}
