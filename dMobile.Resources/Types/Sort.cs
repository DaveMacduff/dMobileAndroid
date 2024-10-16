// <copyright file="Sort.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using Enums;

    /// <summary>
    /// Represents sorting criteria with a value and order.
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets or sets the value to sort by.
        /// </summary>
        public string SortValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order of the sort.
        /// </summary>
        public SortOrder SortOrder { get; set; } = SortOrder.ASC;
    }
}