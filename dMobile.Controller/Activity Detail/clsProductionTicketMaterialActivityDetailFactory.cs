// <copyright file="clsProductionTicketMaterialActivityDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    /// <summary>
    /// <see cref="TicketMaterialDetailFactory" /> Production ticket material detail factory.
    /// </summary>
    public class TicketMaterialDetailFactory : ActDetailFactory
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
