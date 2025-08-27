// <copyright file="PrnSettingsArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;

    /// <summary>
    /// Defines the printer settings event arguments.
    /// </summary>
    public class PrnSettingsArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrnSettingsArgs"/> class.
        /// </summary>
        public PrnSettingsArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrnSettingsArgs"/> class.
        /// </summary>
        /// <param name="msgData"> Message data.</param>
        public PrnSettingsArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the status out message.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the progress minimum value.
        /// </summary>
        public int MinValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the progress maximum value.
        /// </summary>
        public int MaxValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the current progress value.
        /// </summary>
        public int CurrentValue { get; set; } = 0;
    }
}
