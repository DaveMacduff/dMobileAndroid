// <copyright file="Message.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Message
    {
        private string messageValue;

        public int MessageIndex { get; set; }
        public string MessageValue { get => this.messageValue; set => this.messageValue = value; }
    }
}
