// <copyright file="POWorkSheetActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="POWorkSheetActDetailFactory" /> Purchase order work sheet activity detail factory.
    /// </summary>
    public class POWorkSheetActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the purchase order work sheet activity detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Purchase order work sheet activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new POWorkSheetActDetail(actItem);
        }
    }
}
