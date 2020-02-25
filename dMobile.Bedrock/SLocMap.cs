// ----------------------------------------------------------------------
// <copyright file="SLocMap.cs" company="InterSect Business Systems Inc.">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;

    public struct SLocMap
    {
        private int whsLocMapRefno;
        private int whsFKRefno;
        private int whsCode;
        private string whsName;
        private bool isWhsWMS;
        private int sectLocMapRefno;
        private int sectFKRefno;
        private string sectName;
        private int sectCode;
        private int slotLocMapRefno;
        private int slotFKRefno;
        private int slotCode;
        private string slotName;
        private int slotCoord1LocMapRefno;
        private int slotCoord2LocMapRefno;
        private int slotCoord3LocMapRefno;
        private string slotCoord1Name;
        private string slotCoord2Name;
        private string slotCoord3Name;
        private bool isSectionTracked;
        private bool isSlotTracked;
        private bool isSlotCoord1Tracked;
        private bool isSlotCoord2Tracked;
        private bool isSlotCoord3Tracked;
        private string whsCodeMask;
        private string sectMask;
        private string slotMask;
        private string slotCoord1Mask;
        private string slotCoord2Mask;
        private string slotCoord3Mask;

        public string SlotCoord1Mask
        {
            get
            {
                return this.slotCoord1Mask;
            }

            set
            {
                this.slotCoord1Mask = value;
            }
        }

        public string SlotCoord2Mask
        {
            get
            {
                return this.slotCoord2Mask;
            }

            set
            {
                this.slotCoord2Mask = value;
            }
        }

        public string SlotCoord3Mask
        {
            get
            {
                return this.slotCoord3Mask;
            }

            set
            {
                this.slotCoord3Mask = value;
            }
        }

        public string SlotMask
        {
            get
            {
                return this.slotMask;
            }

            set
            {
                this.slotMask = value;
            }
        }

        public string SectMask
        {
            get
            {
                return this.sectMask;
            }

            set
            {
                this.sectMask = value;
            }
        }

        public string WhsCodeMask
        {
            get
            {
                return this.whsCodeMask;
            }

            set
            {
                this.whsCodeMask = value;
            }
        }

        public bool IsSlotCoord1Tracked
        {
            get
            {
                return this.isSlotCoord1Tracked;
            }

            set
            {
                this.isSlotCoord1Tracked = value;
            }
        }

        public bool IsSlotCoord2Tracked
        {
            get
            {
                return this.isSlotCoord2Tracked;
            }

            set
            {
                this.isSlotCoord2Tracked = value;
            }
        }

        public bool IsSlotCoord3Tracked
        {
            get
            {
                return this.isSlotCoord3Tracked;
            }

            set
            {
                this.isSlotCoord3Tracked = value;
            }
        }

        public bool IsSlotTracked
        {
            get
            {
                return this.isSlotTracked;
            }

            set
            {
                this.isSlotTracked = value;
            }
        }

        public bool IsSectionTracked
        {
            get
            {
                return this.isSectionTracked;
            }

            set
            {
                this.isSectionTracked = value;
            }
        }

        public string SlotCoord1Name
        {
            get
            {
                return this.slotCoord1Name;
            }

            set
            {
                this.slotCoord1Name = value;
                if (string.IsNullOrEmpty(this.slotCoord1Name))
                {
                    this.IsSlotCoord1Tracked = false;
                }
                else
                {
                    this.IsSlotCoord1Tracked = true;
                }
            }
        }

        public string SlotCoord2Name
        {
            get
            {
                return this.slotCoord2Name;
            }

            set
            {
                this.slotCoord2Name = value;
                if (string.IsNullOrEmpty(this.slotCoord2Name))
                {
                    this.IsSlotCoord2Tracked = false;
                }
                else
                {
                    this.IsSlotCoord2Tracked = true;
                }
            }
        }

        public string SlotCoord3Name
        {
            get
            {
                return this.slotCoord3Name;
            }

            set
            {
                this.slotCoord3Name = value;
                if (string.IsNullOrEmpty(this.slotCoord3Name))
                {
                    this.IsSlotCoord3Tracked = false;
                }
                else
                {
                    this.IsSlotCoord3Tracked = true;
                }
            }
        }

        public int SlotCode
        {
            get
            {
                return this.slotCode;
            }

            set
            {
                this.slotCode = value;
            }
        }

        public int WhsLocMapRefno
        {
            get
            {
                return this.whsLocMapRefno;
            }

            set
            {
                this.whsLocMapRefno = value;
            }
        }

        public int WhsFKRefno
        {
            get
            {
                return this.whsFKRefno;
            }

            set
            {
                this.whsFKRefno = value;
            }
        }

        public int WhsCode
        {
            get
            {
                return this.whsCode;
            }

            set
            {
                this.whsCode = value;
            }
        }

        public string WhsName
        {
            get
            {
                if (this.whsName == null)
                {
                    this.whsName = string.Empty;
                }

                return this.whsName;
            }

            set
            {
                this.whsName = value;
            }
        }

        public int SectLocMapRefno
        {
            get
            {
                return this.sectLocMapRefno;
            }

            set
            {
                this.sectLocMapRefno = value;
            }
        }

        public int SectFKRefno
        {
            get
            {
                return this.sectFKRefno;
            }

            set
            {
                this.sectFKRefno = value;
            }
        }

        public string SectName
        {
            get
            {
                if (this.sectName == null)
                {
                    this.sectName = string.Empty;
                }

                return this.sectName;
            }

            set
            {
                this.sectName = value;
            }
        }

        public int SectCode
        {
            get
            {
                return this.sectCode;
            }

            set
            {
                this.sectCode = value;
            }
        }

        public int SlotLocMapRefno
        {
            get
            {
                return this.slotLocMapRefno;
            }

            set
            {
                this.slotLocMapRefno = value;
            }
        }

        public int SlotFKRefno
        {
            get
            {
                return this.slotFKRefno;
            }

            set
            {
                this.slotFKRefno = value;
            }
        }

        public string SlotName
        {
            get
            {
                if (this.slotName == null)
                {
                    this.slotName = string.Empty;
                }

                return this.slotName;
            }

            set
            {
                this.slotName = value;
            }
        }

        public int SlotCoord1LocMapRefno
        {
            get
            {
                return this.slotCoord1LocMapRefno;
            }

            set
            {
                this.slotCoord1LocMapRefno = value;
            }
        }

        public int SlotCoord2LocMapRefno
        {
            get
            {
                return this.slotCoord2LocMapRefno;
            }

            set
            {
                this.slotCoord2LocMapRefno = value;
            }
        }

        public int SlotCoord3LocMapRefno
        {
            get
            {
                return this.slotCoord3LocMapRefno;
            }

            set
            {
                this.slotCoord3LocMapRefno = value;
            }
        }

        public bool IsWhsWMS
        {
            get
            {
                return this.isWhsWMS;
            }

            set
            {
                this.isWhsWMS = value;
            }
        }
    }
}
