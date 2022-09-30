// <copyright file="TicketFinishedActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="TicketFinishedDetailFactory" /> Production ticket finished activity detail factory.
    /// </summary>
    public class TicketFinishedDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the production ticket finished activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Production ticket finished activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new TicketFinishedActDetail(actItem);
        }
    }
}
