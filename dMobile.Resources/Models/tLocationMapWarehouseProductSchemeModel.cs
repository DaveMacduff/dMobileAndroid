using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tLocationMapWarehouseProductScheme"
    //+ "("

    //+ "WarehouseLocationMapRefno INTEGER NOT NULL, "
    //+ "ProductRefno INTEGER NOT NULL, "
    //+ "ProductSchemeRefno INTEGER NOT NULL, "

    //+ "CONSTRAINT tLocationMapWarehouseProductSchemeToLocationMap FOREIGN KEY (WarehouseLocationMapRefno) REFERENCES tLocationMapWarehouse(LocationMapRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT tLocationMapWarehouseProductSchemeToProduct FOREIGN KEY (ProductRefno) REFERENCES tProduct(ProductRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT tLocationMapWarehouseProductSchemeToProductScheme FOREIGN KEY (ProductSchemeRefno) REFERENCES tProductScheme(ProductSchemeRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating warehouse product scheme table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_WhsProdSchemeOnWhsRefnoAndProductRefnoAndSchemeRefno ON tLocationMapWarehouseProductScheme(WarehouseLocationMapRefno ASC, ProductRefno ASC, ProductSchemeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WhsProdSchemeOnWarehouseLocationMapRefno ON tLocationMapWarehouseProductScheme(WarehouseLocationMapRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WhsProdSchemeOnProductRefno ON tLocationMapWarehouseProductScheme(ProductRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_WhsProdSchemeOnProductSchemeRefno ON tLocationMapWarehouseProductScheme(ProductSchemeRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tLocationMapWarehouseProductScheme")]
    public class tLocationMapWarehouseProductSchemeModel
    {
        [NotNull, ForeignKey(typeof(tLocationMapWarehouseModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_WhsProdSchemeOnWarehouseLocationMapRefno", Order = 1, Unique = false)]
        public int WarehouseLocationMapRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_WhsProdSchemeOnProductRefno", Order = 1, Unique = false)]
        public int ProductRefno { get; set; }

        [NotNull, ForeignKey(typeof(tProductSchemeModel)), OneToMany(CascadeOperations = CascadeOperation.All), Indexed(Name = "FK_WhsProdSchemeOnProductSchemeRefno", Order = 1, Unique = false)]
        public int ProductSchemeRefno { get; set; }
    }
}
