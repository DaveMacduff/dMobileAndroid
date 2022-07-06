// <copyright file="tActivityItemProductionFinishedModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemProductionFinished")]
    public class tActivityItemProductionFinishedModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityItemProductionModel))]
        public int ActivityItemRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityItemProductionFinishedOnActivityItemFKRefno", Order = 1, Unique = true)]
        public int ActivityItemFKRefno { get; set; }

        [NotNull]
        public string ActivityItemStringFKRefno { get; set; }
    }
}
