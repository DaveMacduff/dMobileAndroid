// <copyright file="DataAccessEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;

    /// <summary>
    /// Provides data for data access events.
    /// </summary>
    public class DataAccessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessEventArgs"/> class.
        /// </summary>
        public DataAccessEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set minimum and maximum values.
        /// </summary>
        public bool SetMinMaxValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to override disconnected mode.
        /// </summary>
        public bool OverrideDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets the status output message.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public int CurrentValue { get; set; }
    }
}