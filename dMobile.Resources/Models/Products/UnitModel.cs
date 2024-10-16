// <copyright file="UnitModel.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Models.Products
{
    using SQLite;

    /// <summary>
    /// Represents a unit.
    /// </summary>
    [Table("tUnit")]
    public class UnitModel
    {
        /// <summary>
        /// Gets or sets the unique reference number for the unit.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [NotNull]
        public int UnitRefno { get; set; }

        /// <summary>
        /// Gets or sets the unique code of the unit.
        /// </summary>
        [NotNull]
        [Indexed(Name = "UQ_UnitOnUnitCode", Order = 1, Unique = true)]
        public string UnitCode { get; set; }
    }
}