using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
//    string sqlTable = "CREATE TABLE IF NOT EXISTS tUnit"
//    + "("

//    + "UnitRefno INTEGER NOT NULL CONSTRAINT PK_UnitOnUnitRefno PRIMARY KEY,"
//    + "UnitCode nvarchar(2) NOT NULL"

//    + ")";

//if (this.OnNubStatOut != null)
//{
//  this.MsgEventArgs.StatusOutMsg = "Creating unit of measure table.";
//  this.OnNubStatOut(this, this.MsgEventArgs);
//}

//await this.RunDDLCommand(sqlTable);

//string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_UnitOnUnitCode ON tUnit(UnitCode ASC)";
//await this.RunDDLCommand(sqlNdx);

    [Table("tUnit")]
    public class tUnitModel
    {
        [PrimaryKey, NotNull]
        public int UnitRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_UnitOnUnitCode", Order = 1, Unique = true)]
        public string UnitCode { get; set; }
    }
}
