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
        /// Defines the row.
        /// </summary>
        private int row = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridEnableEventArgs"/> class.
        /// </summary>
        /// <param name="row">.</param>
        /// <param name="col">.</param>
        /// <param name="isMeetsCriteria">.</param>
        public DataGridEnableEventArgs(int row, int col, bool isMeetsCriteria)
        {
            this.Row = row;
            this.Column = col;
            this.MeetsCriteria = isMeetsCriteria;
        }

        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        public int Column { get; set; } = 0;

        /// <summary>
        /// Gets or sets the Row.
        /// </summary>
        public int Row
        {
            get => this.row;

            set => this.row = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether MeetsCriteria.
        /// </summary>
        public bool MeetsCriteria { get; set; } = false;
    }
}
