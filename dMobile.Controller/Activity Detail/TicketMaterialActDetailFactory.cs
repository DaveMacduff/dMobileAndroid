// <copyright file="TicketMaterialActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="TicketMaterialActDetailFactory" /> Production ticket material detail factory.
    /// </summary>
    public class TicketMaterialActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the production ticket material detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Production ticket material detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new TicketMaterialActDetail(actItem);
        }
    }
}
