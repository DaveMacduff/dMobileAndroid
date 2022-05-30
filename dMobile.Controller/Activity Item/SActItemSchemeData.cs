// <copyright file="SActItemSchemeData.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using dMobile.Bedrock;

    /// <summary>
    ///  Structure for activity items scheme data.
    /// </summary>
    public struct SActItemSchemeData
    {
        /// <summary>
        /// Gets or sets the activity detail extra information.
        /// </summary>
        public ActDetailExtra ActDetailExtra { get; set; }

        /// <summary>
        /// Gets or sets the activity item foreign key reference number.
        /// </summary>
        /// <example>ar_oehdr.reference number.</example>
        public int ActItemFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the activity detail quantity. The alt_qty.
        /// </summary>
        public decimal ActDetailQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity product unit code.
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Gets or sets the activity detail base quantity. The pk_qty.
        /// </summary>
        public decimal ActDetailBaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the base quantity product unit code.
        /// </summary>
        public string BaseUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the activity detail weight quantity. The wt_qty.
        /// </summary>
        public decimal ActDetailWeightQuantity { get; set; }

        /// <summary>
        /// Gets or sets the weight quantity product unit code.
        /// </summary>
        public string WeightUnitCode { get; set; }

        /// <summary>
        /// Gets or sets the product tracking scheme data.
        /// </summary>
        public SProductSchemeData ProdSchemeData { get; set; }

        /// <summary>
        /// Gets or sets the warehouse location map foreign key reference number.
        /// </summary>
        /// <example>iv_locwh.wh_locat.</example>
        public int LocMapWhsFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the section location map foreign key reference number.
        /// </summary>
        /// <example>iv_sect.reference number.</example>
        public int LocMapSectFKRefno { get; set; }

        /// <summary>
        /// Gets or sets the slot location map foreign key reference number.
        /// </summary>
        /// <example>iv_slot.reference number.</example>
        public int LocMapSlotFKRefno { get; set; }
    }
}
