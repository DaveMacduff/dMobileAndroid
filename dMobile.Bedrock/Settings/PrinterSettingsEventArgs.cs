// <copyright file="PrinterSettingsEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;

    /// <summary>
    /// Defines the <see cref="PrnSettingsArgs" />.
    /// </summary>
    public class PrnSettingsArgs : EventArgs
    {
        /// <summary>
        /// Defines the currentValue.
        /// </summary>
        private int currentValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrnSettingsArgs"/> class.
        /// </summary>
        public PrnSettingsArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrnSettingsArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public PrnSettingsArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        /// <summary>
        /// Gets or sets the StatusOutMsg.
        /// </summary>
        public string StatusOutMsg { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MinValue.
        /// </summary>
        public int MinValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the MaxValue.
        /// </summary>
        public int MaxValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CurrentValue.
        /// </summary>
        public int CurrentValue
        {
            get => this.currentValue;

            set => this.currentValue = value;
        }
    }
}
