// <copyright file="tReasonCodeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using System;
    using SQLite;
    using SQLiteNetExtensions;

    [Table("tReasonCode")]
    public class tReasonCodeModel
    {
        [PrimaryKey, NotNull]
        public int ReasonCodeRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityOnReasonCodeFKRefno", Order = 1, Unique = true)]
        public int ReasonCodeFKRefno { get; set; }

        [NotNull, Indexed(Name = "UQ_ActivityOnReasonCodeCode", Order = 1, Unique = true)]
        public string ReasonCodeCode { get; set; }

        [NotNull]
        public string ReasonCodeDescription { get; set; }
    }
}
