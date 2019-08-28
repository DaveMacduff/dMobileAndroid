using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace dMobile.Bedrock
{
    public struct SSkidInfo
    {
        private ProductInfo prodInfo;
        private int locMapRefno;
        private SLocMap locMap;
        private decimal quantity;
        private decimal baseQuantity;
        private decimal weightQuantity;

        /// <summary>
        /// GS1 18 digit skid code. Including the company prefix.
        /// </summary>
        private string logisticUnitSkidCode;
        private int skidFKRefno;

        /// <summary>
        /// User entered or form generated skid code.
        /// </summary>
        private string skidCode;

        private bool isCHEP;
        private bool isHold;
        private int ti;
        private int hi;
        private decimal height;
        private string heightUnit;
        private decimal cube;
        private string cubeUnit;
        private bool isCompleted;
        private bool isConsumed;
        private bool isPutAway;
        private SProductSchemeData schemeData;

        public SProductSchemeData SchemeData
        {
            get
            {
                return this.schemeData;
            }

            set
            {
                this.schemeData = value;
            }
        }

        public bool IsPutAway
        {
            get
            {
                return this.isPutAway;
            }

            set
            {
                this.isPutAway = value;
            }
        }

        public bool IsConsumed
        {
            get
            {
                return this.isConsumed;
            }

            set
            {
                this.isConsumed = value;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }

            set
            {
                this.isCompleted = value;
            }
        }

        public int SkidFKRefno
        {
            get
            {
                return this.skidFKRefno;
            }

            set
            {
                this.skidFKRefno = value;
            }
        }

        public string CubeUnit
        {
            get
            {
                return this.cubeUnit;
            }

            set
            {
                this.cubeUnit = value;
            }
        }

        public decimal Cube
        {
            get
            {
                return this.cube;
            }

            set
            {
                this.cube = value;
            }
        }

        public string HeightUnit
        {
            get
            {
                return this.heightUnit;
            }

            set
            {
                this.heightUnit = value;
            }
        }

        public decimal Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
            }
        }

        public int Hi
        {
            get
            {
                return this.hi;
            }

            set
            {
                this.hi = value;
            }
        }

        public int Ti
        {
            get
            {
                return this.ti;
            }

            set
            {
                this.ti = value;
            }
        }

        public bool IsHold
        {
            get
            {
                return this.isHold;
            }

            set
            {
                this.isHold = value;
            }
        }

        public bool IsCHEP
        {
            get
            {
                return this.isCHEP;
            }

            set
            {
                this.isCHEP = value;
            }
        }

        /// <summary>
        /// Gets or sets the user entered or form generated skid code.
        /// </summary>
        public string SkidCode
        {
            get
            {
                return this.skidCode;
            }

            set
            {
                this.skidCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the GS1 18 digit skid code. Including the company prefix.
        /// </summary>
        public string LogisticUnitSkidCode
        {
            get
            {
                return this.logisticUnitSkidCode;
            }

            set
            {
                this.logisticUnitSkidCode = value;
            }
        }

        public decimal Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
            }
        }

        public decimal BaseQuantity
        {
            get
            {
                return this.baseQuantity;
            }

            set
            {
                this.baseQuantity = value;
            }
        }

        public decimal WeightQuantity
        {
            get
            {
                return this.weightQuantity;
            }

            set
            {
                this.weightQuantity = value;
            }
        }

        public ProductInfo ProdInfo
        {
            get
            {
                return this.prodInfo;
            }

            set
            {
                this.prodInfo = value;
            }
        }

        public int LocMapRefno
        {
            get
            {
                if (0 < this.LocMap.SlotLocMapRefno)
                {
                    this.locMapRefno = this.LocMap.SlotLocMapRefno;
                }
                else if (0 < this.LocMap.SectLocMapRefno)
                {
                    this.locMapRefno = this.LocMap.SectLocMapRefno;
                }
                else
                {
                    this.locMapRefno = this.LocMap.WhsLocMapRefno;
                }

                return this.locMapRefno;
            }

            set
            {
                this.locMapRefno = value;
            }
        }

        public SLocMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }
    }
}
