// <copyright file="ActItemSchemeData.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using dMobile.Bedrock;
using dMobile.Controller.Activity_Detail;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    ///  Structure for activity items scheme data.
    /// </summary>
    public class ActItemSchemeData
    {
        /// <summary>
        /// Gets or sets the activity detail extra information.
        /// </summary>
        public ActDetailExtra ActDetailExtra { get; set; } = new ActDetailExtra();

        /// <summary>
        /// Gets or sets the activity item foreign key reference number.
        /// </summary>
        /// <example>ar_oehdr.reference number.</example>
        public int ActItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity detail quantity. The alt_qty.
        /// </summary>
        public decimal ActDetailQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the quantity product unit code.
        /// </summary>
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail base quantity. The pk_qty.
        /// </summary>
        public decimal ActDetailBaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the base quantity product unit code.
        /// </summary>
        public string BaseUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the activity detail weight quantity. The wt_qty.
        /// </summary>
        public decimal ActDetailWeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the weight quantity product unit code.
        /// </summary>
        public string WeightUnitCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product tracking scheme data.
        /// </summary>
        public ProductSchemeData ProdSchemeData { get; set; } = new ProductSchemeData();

        /// <summary>
        /// Gets or sets the warehouse location map foreign key reference number.
        /// </summary>
        /// <example>iv_locwh.wh_locat.</example>
        public int LocMapWhsFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the section location map foreign key reference number.
        /// </summary>
        /// <example>iv_sect.reference number.</example>
        public int LocMapSectFKRefno { get; set; } = 0; 

        /// <summary>
        /// Gets or sets the slot location map foreign key reference number.
        /// </summary>
        /// <example>iv_slot.reference number.</example>
        public int LocMapSlotFKRefno { get; set; } = 0;
    }
}
