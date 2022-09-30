// <copyright file="ActDetailExtra.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// Defines the <see cref="ActDetailExtra" />.
    /// </summary>
    public class ActDetailExtra
    {
        /// <summary>
        /// Gets or sets the activity detail extra text 1.
        /// </summary>
        public string Text1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra text 2.
        /// </summary>
        public string Text2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra text 3.
        /// </summary>
        public string Text3 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra text 4.
        /// </summary>
        public string Text4 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail extra date time 1.
        /// </summary>
        public DateTime DateTime1 { get; set; } = new DateTime(1900, 1, 1);
    }
}
