// <copyright file="DataLayerEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;

    /// <summary>
    /// Defines the <see cref="DataLayerEventArgs" />.
    /// </summary>
    public class DataLayerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLayerEventArgs"/> class.
        /// </summary>
        public DataLayerEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLayerEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">The msgData<see cref="string"/>.</param>
        public DataLayerEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this.MinValue or this.MaxValue was set.
        /// Because MinValue and MaxValue are integers, 0 is valid value. We needed a way
        /// to indicate to use them.
        /// </summary>
        public bool SetMinMaxValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OverrideDisconnectedMode.
        /// </summary>
        public bool OverrideDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets the status out messge.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MinValue. Used in a progress bar for example.
        /// </summary>
        public int MinValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the MaxValue. Used in a progress bar for example.
        /// </summary>
        public int MaxValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CurrentValue. Used in a progress bar for example.
        /// </summary>
        public int CurrentValue { get; set; } = 0;
    }
}
