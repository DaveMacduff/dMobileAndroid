// <copyright file="DataGridEnableEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="DataGridEnableEventArgs" />.
    /// </summary>
    public class DataGridEnableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridEnableEventArgs"/> class.
        /// </summary>
        /// <param name="row">Grid row.</param>
        /// <param name="col">Grid column.</param>
        /// <param name="isMeetsCriteria"> Is meets criteria.</param>
        public DataGridEnableEventArgs(int row, int col, bool isMeetsCriteria)
        {
            this.Row = row;
            this.Column = col;
            this.MeetsCriteria = isMeetsCriteria;
        }

        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the Row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether MeetsCriteria.
        /// </summary>
        public bool MeetsCriteria { get; set; }
    }
}
