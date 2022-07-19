// <copyright file="tActivityItemReasonCodeXRefModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions.Attributes;

    [Table("tActivityItemReasonCodeXRef")]
    public class tActivityItemReasonCodeXRefModel
    {
        [NotNull]
        [ForeignKey(typeof(tActivityItemModel))]
        [Indexed(Name = "FK_ActivityItemReasonCodeXRefOnActivityItemRefno", Order = 1, Unique = false)]
        public int ActivityItemRefno { get; set; }

        [NotNull]
        [ForeignKey(typeof(tReasonCodeModel))]
        [Indexed(Name = "FK_ActivityItemReasonCodeXRefOnReasonCodeRefno", Order = 1, Unique = false)]
        public int ReasonCodeRefno { get; set; }
    }
}
