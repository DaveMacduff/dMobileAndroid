// <copyright file="TicketFinishedActivityFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="TicketFinishedActivityFactory" />.
    /// </summary>
    public class TicketFinishedActivityFactory : ActivityFactory
    {
        /// <summary>
        /// The GetActivity.
        /// </summary>
        /// <returns>The <see cref="Activity"/>.</returns>
        public override Activity GetActivity()
        {
            return new TicketFinishedActivity();
        }
    }
}
