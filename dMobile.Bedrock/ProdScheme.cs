// ----------------------------------------------------------------------
// <copyright file="ProdScheme.cs" company="InterSect Business Systems Inc.">
//     Copyright (2013). All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    /// <summary>
    /// 
    /// </summary>
    public struct ProdScheme
    {
        private int schemeRefno;
        private bool isSerialTracked;
        private bool isProductionDateTracked;
        private bool isLotTracked;
        private bool isBestBeforeDateTracked;
        private bool isVariantTracked;
        private bool isLotUniqueComponent;
        private bool isProductionDateUniqueComponent;
        private bool isBestBeforeDateUniqueComponent;
        private bool isVariantUniqueComponent;
        private int extra1Type;
        private string extra1Caption;
        private bool extra1IsRequired;
        private int extra2Type;
        private string extra2Caption;
        private bool extra2IsRequired;
        private int extra3Type;
        private string extra3Caption;
        private bool extra3IsRequired;
        private int extra4Type;
        private string extra4Caption;
        private bool extra4IsRequired;
        private int extra5Type;
        private string extra5Caption;
        private bool extra5IsRequired;
        private bool isLotTrackedDefault;
        private ProdSchemeLabelSettings.ProdSchemeLabelInfo labelInfo;
        private bool isProductionDateTrackedDefault;
        private bool isBestBeforeDateTrackedDefault;

        public bool IsBestBeforeDateTrackedDefault
        {
            get
            {
                return this.isBestBeforeDateTrackedDefault;
            }

            set
            {
                this.isBestBeforeDateTrackedDefault = value;
            }
        }

        public bool IsProductionDateTrackedDefault
        {
            get
            {
                return this.isProductionDateTrackedDefault;
            }

            set
            {
                this.isProductionDateTrackedDefault = value;
            }
        }

        public ProdSchemeLabelSettings.ProdSchemeLabelInfo LabelInfo
        {
            get
            {
                return this.labelInfo;
            }

            set
            {
                this.labelInfo = value;
            }
        }

        public int SchemeRefno
        {
            get
            {
                return this.schemeRefno;
            }

            set
            {
                this.schemeRefno = value;
            }
        }

        public bool IsLotTrackedDefault
        {
            get
            {
                return this.isLotTrackedDefault;
            }

            set
            {
                this.isLotTrackedDefault = value;
            }
        }

        public int Extra1Type
        {
            get
            {
                return this.extra1Type;
            }

            set
            {
                this.extra1Type = value;
            }
        }

        public string Extra1Caption
        {
            get
            {
                if (string.IsNullOrEmpty(this.extra1Caption))
                {
#if MULTILANGUAGE
          this.extra1Caption = Translator.GetInstance().Translate(TranslatorVocabulary.Extrazzzz);
#else
                    this.extra1Caption = "Extra 1";
#endif
                }

                return this.extra1Caption;
            }

            set
            {
                this.extra1Caption = value;
            }
        }

        public bool Extra1IsRequired
        {
            get
            {
                return this.extra1IsRequired;
            }

            set
            {
                this.extra1IsRequired = value;
            }
        }

        public int Extra2Type
        {
            get
            {
                return this.extra2Type;
            }

            set
            {
                this.extra2Type = value;
            }
        }

        public string Extra2Caption
        {
            get
            {
                return this.extra2Caption;
            }

            set
            {
                this.extra2Caption = value;
            }
        }

        public bool Extra2IsRequired
        {
            get
            {
                return this.extra2IsRequired;
            }

            set
            {
                this.extra2IsRequired = value;
            }
        }

        public int Extra3Type
        {
            get
            {
                return this.extra3Type;
            }

            set
            {
                this.extra3Type = value;
            }
        }

        public string Extra3Caption
        {
            get
            {
                return this.extra3Caption;
            }

            set
            {
                this.extra3Caption = value;
            }
        }

        public bool Extra3IsRequired
        {
            get
            {
                return this.extra3IsRequired;
            }

            set
            {
                this.extra3IsRequired = value;
            }
        }

        public int Extra4Type
        {
            get
            {
                return this.extra4Type;
            }

            set
            {
                this.extra4Type = value;
            }
        }

        public string Extra4Caption
        {
            get
            {
                return this.extra4Caption;
            }

            set
            {
                this.extra4Caption = value;
            }
        }

        public bool Extra4IsRequired
        {
            get
            {
                return this.extra4IsRequired;
            }

            set
            {
                this.extra4IsRequired = value;
            }
        }

        public int Extra5Type
        {
            get
            {
                return this.extra5Type;
            }

            set
            {
                this.extra5Type = value;
            }
        }

        public string Extra5Caption
        {
            get
            {
                return this.extra5Caption;
            }

            set
            {
                this.extra5Caption = value;
            }
        }

        public bool Extra5IsRequired
        {
            get
            {
                return this.extra5IsRequired;
            }

            set
            {
                this.extra5IsRequired = value;
            }
        }

        public bool IsSerialTracked
        {
            get
            {
                return this.isSerialTracked;
            }

            set
            {
                this.isSerialTracked = value;
            }
        }

        public bool IsProductionDateTracked
        {
            get
            {
                return this.isProductionDateTracked;
            }

            set
            {
                this.isProductionDateTracked = value;
            }
        }

        public bool IsLotTracked
        {
            get
            {
                return this.isLotTracked;
            }

            set
            {
                this.isLotTracked = value;
            }
        }

        public bool IsBestBeforeDateTracked
        {
            get
            {
                return this.isBestBeforeDateTracked;
            }

            set
            {
                this.isBestBeforeDateTracked = value;
            }
        }

        public bool IsVariantTracked
        {
            get
            {
                return this.isVariantTracked;
            }

            set
            {
                this.isVariantTracked = value;
            }
        }

        public bool IsLotUniqueComponent
        {
            get
            {
                return this.isLotUniqueComponent;
            }

            set
            {
                this.isLotUniqueComponent = value;
            }
        }

        public bool IsProductionDateUniqueComponent
        {
            get
            {
                return this.isProductionDateUniqueComponent;
            }

            set
            {
                this.isProductionDateUniqueComponent = value;
            }
        }

        public bool IsBestBeforeDateUniqueComponent
        {
            get
            {
                return this.isBestBeforeDateUniqueComponent;
            }

            set
            {
                this.isBestBeforeDateUniqueComponent = value;
            }
        }

        public bool IsVariantUniqueComponent
        {
            get
            {
                return this.isVariantUniqueComponent;
            }

            set
            {
                this.isVariantUniqueComponent = value;
            }
        }
    }
}
