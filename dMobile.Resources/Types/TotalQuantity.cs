// <copyright file="TotalQuantity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    using System;

    public class TotalQuantity
    {
        private decimal received;

        public decimal Ordered { get; set; }
        public decimal Received { get => this.received; set => this.received = value; }
    }
}
