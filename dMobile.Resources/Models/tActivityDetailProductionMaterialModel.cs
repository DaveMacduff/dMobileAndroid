using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityDetailProductionMaterial"
    //+ "("

    //+ "ActivityDetailRefno INTEGER NOT NULL CONSTRAINT PK_ActivityDetailProductionMaterialOnActivityDetailRefno PRIMARY KEY,"
    //+ "ActivityDetailProductionMaterialFKRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityDetailProductionMaterialTactDetailOnActivityDetailRefno FOREIGN KEY (ActivityDetailRefno) REFERENCES tActivityDetail(ActivityDetailRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating detail production material table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityDetailProductionMaterialOnActivityDetailProductionMaterialFKRefno ON tActivityDetailProductionMaterial(ActivityDetailProductionMaterialFKRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityDetailProductionMaterial")]
    public class tActivityDetailProductionMaterialModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityDetailModel))]
        public int ActivityDetailRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityDetailProductionMaterialOnActivityDetailProductionMaterialFKRefno", Order = 1, Unique = true)]
        public int ActivityDetailProductionMaterialFKRefno { get; set; }
    }
}
