// <copyright file="FamilyModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models
{
    using SQLite;

    /// <summary>
    /// Represents a family in the family table.
    /// </summary>
    [Table("tFamily")]
    public class FamilyModel
    {
        /// <summary>
        /// Gets or sets the family reference number.
        /// </summary>
        [PrimaryKey, AutoIncrement, NotNull]
        public int FamilyRefno { get; set; }

        /// <summary>
        /// Gets or sets the family code.
        /// </summary>
        [Indexed(Name = "UQ_FamilyOnFamilyCode", Order = 1, Unique = true)]
        [NotNull]
        public string FamilyCode { get; set; }
    }
}