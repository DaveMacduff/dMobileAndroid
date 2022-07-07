// <copyright file="BarCodeAssocActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="BarCodeAssocActivityFactory" />.
    /// </summary>
    public class BarCodeAssocActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>.</returns>
        public override Activity GetActivity()
        {
            return new BarCodeAssocActivity();
        }
    }
}
