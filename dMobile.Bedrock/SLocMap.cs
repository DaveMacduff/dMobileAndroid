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
    }
}
