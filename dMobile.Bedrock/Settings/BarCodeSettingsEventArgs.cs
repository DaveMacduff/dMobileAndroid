// <copyright file="BarCodeSettingsEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="BarCodeSettingsEventArgs" />.
    /// </summary>
    public class BarCodeSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeSettingsEventArgs"/> class.
        /// </summary>
        public BarCodeSettingsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCodeSettingsEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public BarCodeSettingsEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the status out message.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the progress bar minimum value.
        /// </summary>
        public int MinValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the progress bar maximum value.
        /// </summary>
        public int MaxValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the progress bar current value.
        /// </summary>
        public int CurrentValue { get; set; } = 0;
    }
}
