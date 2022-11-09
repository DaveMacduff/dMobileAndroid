// <copyright file="DataAccessEventArgs.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;

    public class DataAccessEventArgs : EventArgs
    {
        private int currentValue = 0;
        private bool setMinMaxValue = false;

        /// <summary>
        ///
        /// </summary>
/// <summary>
/// Initializes a new instance of the <see cref="DataAccessEventArgs"/> class.
/// </summary>
        public DataAccessEventArgs()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataMsg"></param>
/// <summary>
/// Initializes a new instance of the <see cref="DataAccessEventArgs"/> class.
/// </summary>
/// <param name="msgData"></param>
        public DataAccessEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        public bool SetMinMaxValue
        {
            get => this.setMinMaxValue;

            set => this.setMinMaxValue = value;
        }

        public bool OverrideDisconnectedMode { get; set; } = false;

        public string StatusOutMsg { get; set; } = string.Empty;

        public int MinValue { get; set; } = 0;

        public int MaxValue { get; set; } = 0;

        public int CurrentValue
        {
            get => this.currentValue;

            set => this.currentValue = value;
        }
    }
}
