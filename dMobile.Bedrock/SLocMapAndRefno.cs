// <copyright file="SLocMapAndRefno.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    /// <summary>
    /// Location map and location map reference number.
    /// </summary>
    public class SLocMapAndRefno
    {
        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        public SLocMap SLocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the location map reference number.
        /// </summary>
        public int LocMapRefno { get; set; } = 0;
    }
}
