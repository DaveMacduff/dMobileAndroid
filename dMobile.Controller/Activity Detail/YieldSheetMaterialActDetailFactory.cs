// <copyright file="YieldSheetMaterialActDetailFactory.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Controller.Activity_Item;

namespace dMobile.Controller.Activity_Detail
{
    /// <summary>
    /// <see cref="YieldSheetMaterialActDetailFactory" /> cut sheet material activity detail factory.
    /// </summary>
    public class YieldSheetMaterialActDetailFactory : ActDetailFactory
    {
        /// <summary>
        /// Get the cut sheet finished material detail.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns><see cref="ActDetail"/> Cut sheet material activity detail.</returns>
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new YieldSheetMaterialActDetail(actItem);
        }
    }
}
