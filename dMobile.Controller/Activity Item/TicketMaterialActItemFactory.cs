// <copyright file="TicketMaterialActItemFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Defines the <see cref="TicketMaterialActItemFactory" />.
    /// </summary>
    public class TicketMaterialActItemFactory : ActItemFactory
    {
        /// <summary>
        /// The GetActItem.
        /// </summary>
        /// <param name="activity">The activity<see cref="Activity"/>.</param>
        /// <returns>The <see cref="ActItem"/>.</returns>
        public override ActItem GetActItem(Activity.Activity activity)
        {
            return new TicketMaterialActItem(activity);
        }
    }
}
