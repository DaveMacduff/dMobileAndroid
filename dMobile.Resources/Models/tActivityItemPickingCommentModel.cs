using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemPickingComment"
    //        + "("
    //        + "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemPickingCommentOnActivityItemRefno PRIMARY KEY,"
    //        + "ActivityItemPickingCommentComment nvarchar(255) NOT NULL, "

    //        + "CONSTRAINT ActivityItemPickingCommentTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItemPicking(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //        + ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating picking comment item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    [Table("tActivityItemPickingComment")]
    public class tActivityItemPickingCommentModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemPickingModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        public string ActivityItemPickingCommentComment { get; set; }
    }
}
