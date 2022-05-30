// <copyright file="clsProductionTicketFinishedActivityItemFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// Defines the <see cref="TicketFinishedActItemFactory" />.
    /// </summary>
    public class TicketFinishedActItemFactory : ActItemFactory
    {
        /// <summary>
        /// The GetActItem.
        /// </summary>
        /// <param name="activity">The activity<see cref="Activity"/>.</param>
        /// <returns>The <see cref="ActItem"/>.</returns>
        public override ActItem GetActItem(Activity activity)
        {
            return new TicketFinishedActItem(activity);
        }
    }
}
