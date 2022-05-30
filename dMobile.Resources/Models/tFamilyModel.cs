// <copyright file="tFamilyModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    //string sqlTable = "CREATE TABLE IF NOT EXISTS tFamily"
    //+ "("
    //+ "FamilyRefno INTEGER NOT NULL CONSTRAINT PK_FamilyOnFamilyRefno PRIMARY KEY,"
    //+ "FamilyCode nvarchar(20) NOT NULL"
    //+ ")";

    //if (this.OnNubStatOut != null)
    //{
    //    this.MsgEventArgs.StatusOutMsg = "Creating families table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_FamilyOnFamilyCode ON tFamily(FamilyCode ASC)";
    //await this.RunDDLCommand(sqlNdx);
    [Table("tFamily")]
    public class tFamilyModel
    {
        [PrimaryKey, NotNull]
        public int FamilyRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_FamilyOnFamilyCode", Order = 1, Unique = true)]
        public string FamilyCode { get; set; }
    }
}
