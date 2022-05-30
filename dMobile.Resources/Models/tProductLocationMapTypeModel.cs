// <copyright file="tProductLocationMapTypeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductLocationMapType"
    //+ "("

    //+ "ProductRefno INTEGER NOT NULL,"
    //+ "WarehouseLocationMapRefno INTEGER NOT NULL,"
    //+ "ProductLocationMapTypeEnum INTEGER NOT NULL,"
    //+ "LocationMapRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT ProductLocationMapTypeToProductOnProductRefno FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ProductLocationMapTypeToLocationMapOnLocationMapRefno FOREIGN KEY (LocationMapRefno) REFERENCES tLocationMap(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ProductLocationMapTypeToLocationMapOnWarehouseLocationMapRefno FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMap(LocationMapRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product location map table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ProductLocationMapTypeOnProductRefnoAndLocationMapRefnoAndProductLocationMapTypeEnum ON tProductLocationMapType(ProductRefno, LocationMapRefno, ProductLocationMapTypeEnum)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnWarehouseLocationMapRefnoAndProductRefnoAndProductLocationMapTypeEnum ON tProductLocationMapType(WarehouseLocationMapRefno, ProductRefno, ProductLocationMapTypeEnum)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnProductRefno ON tProductLocationMapType(ProductRefno)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnWarehouseLocationMapRefno ON tProductLocationMapType(WarehouseLocationMapRefno)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductLocationMapTypeOnLocationMapRefno ON tProductLocationMapType(LocationMapRefno)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductLocationMapType")]
    public class tProductLocationMapTypeModel
    {
        [NotNull, ForeignKey(typeof(tProductModel)), Indexed(Name = "FK_ProductLocationMapTypeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_ProductLocationMapTypeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, Indexed(Name = "FK_ProductLocationMapTypeOnProductLocationMapTypeEnum", Order = 1, Unique = false)]
        public int ProductLocationMapTypeEnum { get; set; }

        [NotNull, ForeignKey(typeof(tLocationMapModel)), Indexed(Name = "FK_ProductLocationMapTypeOnLocationMapRefno", Order = 1, Unique = false)]
        public int LocationMapRefno { get; set; }
    }
}
