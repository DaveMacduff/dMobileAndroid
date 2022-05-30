// <copyright file="tActivityItemTransferModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityItemTransfer"
    //+ "("
    //+ "ActivityItemRefno INTEGER NOT NULL CONSTRAINT PK_ActivityItemTransferOnActivityItemRefno PRIMARY KEY,"
    //+ "ActivityItemSubType INTEGER NOT NULL,"
    //+ "ActivityItemTransferFKRefno INTEGER NOT NULL,"
    //+ "ActivityItemTransferStringFKRefno NVARCHAR(8) NOT NULL,"
    //+ "ProductUnitRefno INTEGER NOT NULL,"
    //+ "TransferFromWarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ActivityItemTransferRequiredQuantity NUMERIC(11, 3) NOT NULL,"
    //+ "ActivityItemTransferQuantity NUMERIC(11, 3) NOT NULL,"
    //+ "ActivityItemTransferBaseQuantity NUMERIC(11, 3) NOT NULL,"
    //+ "ActivityItemTransferWeightQuantity NUMERIC(11, 3) NOT NULL, "

    //+ "CONSTRAINT ActivityItemTransferTactItemOnActivityItemRefno FOREIGN KEY (ActivityItemRefno) REFERENCES tActivityItem(ActivityItemRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemTransferToLocationMapWarehouseOnTransferFromWarehouseLocationMapRefnoAndLocationMapRefno FOREIGN KEY (TransferFromWarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityItemTransferTactItemOnProductUnitRefno FOREIGN KEY (ProductUnitRefno) REFERENCES tProductUnit(ProductUnitRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    
    //    this.MsgEventArgs.StatusOutMsg = "Creating transfer item table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemTransferOnActivityItemRefnoAndActivityItemSubType ON tActivityItemTransfer(ActivityItemRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityItemTransferOnActivityItemTransferFKRefnoAndActivitySubType ON tActivityItemTransfer(ActivityItemTransferFKRefno ASC, ActivityItemSubType ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno ON tActivityItemTransfer(TransferFromWarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tActivityItemTransfer")]
    public class tActivityItemTransferModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemModel)), Indexed(Name = "UQ_ActivityItemTransferOnActivityItemRefno", Order = 1, Unique = true)]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemTransferOnActivityItemSubType", Order = 1, Unique = false)]
        public int ActivityItemSubType { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemTransferOnActivityItemTransferFKRefnoAndActivitySubType", Order = 1, Unique = false)]
        public int ActivityItemTransferFKRefno { get; set; }

        [NotNull]
        public string ActivityItemTransferStringFKRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel))]
        public int ProductUnitRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), Indexed(Name = "FK_ActivityItemTransferOnTransferFromWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int TransferFromWarehouseLocationMapRefno { get; set; }

        [NotNull]
        public decimal ActivityItemTransferRequiredQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferBaseQuantity { get; set; }

        [NotNull]
        public decimal ActivityItemTransferWeightQuantity { get; set; }
    }
}
