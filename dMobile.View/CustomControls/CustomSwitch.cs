// <copyright file="CustomSwitch.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomControls
{
    using System;
    using Xamarin.Forms;
    using Switch = Xamarin.Forms.Switch;

    /// <summary>
    /// Defines the <see cref="CustomSwitch" />.
    /// </summary>
    public class CustomSwitch : Switch
    {
        public EventHandler Click;

        public event EventHandler Clicked
        {
            add
            {
                lock (this)
                {
                    this.Click += value;
                }
            }
            remove
            {
                lock (this)
                {
                    this.Click -= value;
                }
            }
        }
    }
}
