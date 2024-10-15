// <copyright file="ReasonCodeModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a reason code.
    /// </summary>
    [Table("tReasonCode")]
    public class ReasonCodeModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the reason code.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int ReasonCodeRefno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference number for the reason code.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityOnReasonCodeFKRefno", Order = 1, Unique = true)]
        public int ReasonCodeFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the reason code.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_ActivityOnReasonCodeCode", Order = 1, Unique = true)]
        public string ReasonCodeCode { get; set; }

        /// <summary>
        /// Gets or sets the description of the reason code.
        /// </summary>
        [NotNull]
        public string ReasonCodeDescription { get; set; }
    }
}