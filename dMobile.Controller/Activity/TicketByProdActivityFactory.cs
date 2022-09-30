// <copyright file="TicketByProdActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="TicketByProdActivityFactory" />.
    /// </summary>
    public class TicketByProdActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>The <see cref="Activity"/>.</returns>
        public override Activity GetActivity()
        {
            return new TicketByProdActivity();
        }
    }
}
