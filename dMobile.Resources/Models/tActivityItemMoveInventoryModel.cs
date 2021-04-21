using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemMoveInventory"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemMoveInventoryOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemMoveInventoryType INTEGER NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating item move inventory table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string alterTable = "ALTER TABLE tActivityItemMoveInventory ADD CONSTRAINT ActivityItemMoveInventoryTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);

    //alterTable = "ALTER TABLE tActivityItemMoveInventory ADD CONSTRAINT ActivityItemMoveInventoryTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE";
    //await this.RunDDLCommand(alterTable);

    //string sqlNdx = "CREATE UNIQUE NONCLUSTERED INDEX UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType ON tActivityItemMoveInventory(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityItemMoveInventory")]
    public class tActivityItemMoveInventoryModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), OneToOne(CascadeOperations = CascadeOperation.All), Indexed(Name = "UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemMoveInventoryOnActivityItemRefnoAndActivityItemSubType", Order = 2, Unique = true)]
        public int ActivityItemSubType { get; set; }

        [NotNull]
        public int ActivityItemMoveInventoryType { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), OneToMany(CascadeOperations = CascadeOperation.All)]
        public int ProductUnitRefno { get; set; }
    }
}
