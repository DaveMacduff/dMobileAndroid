// <copyright file="Sort.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using dMobile.Resources.Enums;

    public class Sort
    {
        public string SortValue { get; set; } = string.Empty;
        public SortOrder SortOrder { get; set; } = SortOrder.ASC;
    }
}
