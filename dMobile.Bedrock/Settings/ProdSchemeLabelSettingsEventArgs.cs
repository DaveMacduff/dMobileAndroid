// <copyright file="ProdSchemeLabelSettingsEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock.Settings
{
    using System;

    /// <summary>
    /// Defines the <see cref="ProdSchemeLabelSettingsEventArgs" />.
    /// </summary>
    public class ProdSchemeLabelSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettingsEventArgs"/> class.
        /// </summary>
        public ProdSchemeLabelSettingsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProdSchemeLabelSettingsEventArgs"/> class.
        /// </summary>
        /// <param name="msgData">.</param>
        public ProdSchemeLabelSettingsEventArgs(string msgData)
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
        /// Gets or sets the progress current value.
        /// </summary>
        public int CurrentValue { get; set; } = 0;
    }
}
