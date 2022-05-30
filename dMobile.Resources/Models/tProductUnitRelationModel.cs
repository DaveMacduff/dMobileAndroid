// <copyright file="tProductUnitRelationModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tProductUnitRelation"
    //+ "("
    //+ "ProductUnit1Refno INTEGER NOT NULL,"
    //+ "ProductUnit2Refno INTEGER NOT NULL,"
    //+ "ProductUnitRelationFactor numeric(12, 6) NOT NULL, "

    //+ "CONSTRAINT ProductUnitRelationToProductUnitOnProductUnit1RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit1Refno) REFERENCES tProductUnit(ProductUnitRefno), "
    //+ "CONSTRAINT ProductUnitRelationToProductUnitOnProductUnit2RefnoAndProductUnitRefno FOREIGN KEY (ProductUnit2Refno) REFERENCES tProductUnit(ProductUnitRefno)"

    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating product unit relations table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS PK_ProductUnitRelationOnProductUnit1RefnoAndProductUnit2Refno ON tProductUnitRelation(ProductUnit1Refno, ProductUnit2Refno)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitRelationOnProductUnit1Refno ON tProductUnitRelation(ProductUnit1Refno)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ProductUnitRelationOnProductUnit2Refno ON tProductUnitRelation(ProductUnit2Refno)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tProductUnitRelation")]
    public class tProductUnitRelationModel
    {
        [NotNull, ForeignKey(typeof(tProductUnitModel)), Indexed(Name = "FK_ProductUnitRelationOnProductUnit1Refno", Order = 1, Unique = false)]
        public int ProductUnit1Refno { get; set; }

        [NotNull, ForeignKey(typeof(tProductUnitModel)), Indexed(Name = "FK_ProductUnitRelationOnProductUnit2Refno", Order = 1, Unique = false)]
        public int ProductUnit2Refno { get; set; }

        [NotNull]
        public decimal ProductUnitRelationFactor { get; set; }
    }
}
