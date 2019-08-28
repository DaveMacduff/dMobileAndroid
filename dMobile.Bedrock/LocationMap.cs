// ----------------------------------------------------------------------
// <copyright file="LocationMap.cs" company="InterSect Business Systems Inc.">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------
namespace dMobile.Bedrock
{
    using System;
    using System.Data;
    
    using Microsoft.VisualBasic;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class LocationMap
    {
        #region Fields

        private static LocationMap locMap;
        private static int currLocMapRefno = 0;
        private static int loginWhsLocMapRefno = 0;
        private static string locMapWhsCodeMask = string.Empty;
        private static bool isWhsLocMapTracked = false;
        private static bool isSectLocMapTracked = false;
        private static bool isSlotLocMapTracked = false;
        private static string locMapSectCodeMask = string.Empty;
        private static string locMapSlotCodeMask = string.Empty;
        private static string locMapSlotCodeCoord1Mask = string.Empty;
        private static string locMapSlotCodeCoord2Mask = string.Empty;
        private static string locMapSlotCodeCoord3Mask = string.Empty;
        private static bool isSlotCoord1LocMapTracked = false;
        private static bool isSlotCoord2LocMapTracked = false;
        private static bool isSlotCoord3LocMapTracked = false;

        // maintains last selected location for ucSetLocationMap 
        private static SLocMap currLocMap = new SLocMap();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        private LocationMap()
        {
        }

        #endregion

        #region Properties

        public SLocMap CurrLocMap
        {
            get
            {
                return currLocMap;
            }

            set
            {
                currLocMap = value;
            }
        }

        public bool IsWhsLocMapTracked
        {
            get
            {
                return isWhsLocMapTracked;
            }

            set
            {
                isWhsLocMapTracked = value;
            }
        }

        public bool IsSectLocMapTracked
        {
            get
            {
                return isSectLocMapTracked;
            }

            set
            {
                isSectLocMapTracked = value;
            }
        }

        public bool IsSlotLocMapTracked
        {
            get
            {
                return isSlotLocMapTracked;
            }

            set
            {
                isSlotLocMapTracked = value;
            }
        }

        public bool IsSlotCoord1LocMapTracked
        {
            get
            {
                return isSlotCoord1LocMapTracked;
            }

            set
            {
                isSlotCoord1LocMapTracked = value;
            }
        }

        public bool IsSlotCoord2LocMapTracked
        {
            get
            {
                return isSlotCoord2LocMapTracked;
            }

            set
            {
                isSlotCoord2LocMapTracked = value;
            }
        }

        public bool IsSlotCoord3LocMapTracked
        {
            get
            {
                return isSlotCoord3LocMapTracked;
            }

            set
            {
                isSlotCoord3LocMapTracked = value;
            }
        }

        public string LocMapWhsCodeMask
        {
            get
            {
                return locMapWhsCodeMask;
            }

            set
            {
                locMapWhsCodeMask = value;
                if (string.IsNullOrEmpty(locMapWhsCodeMask))
                {
                    this.IsWhsLocMapTracked = false;
                }
                else
                {
                    this.IsWhsLocMapTracked = true;
                }
            }
        }

        public string LocMapSectCodeMask
        {
            get
            {
                return locMapSectCodeMask;
            }

            set
            {
                locMapSectCodeMask = value;
                if (string.IsNullOrEmpty(locMapSectCodeMask))
                {
                    this.IsSectLocMapTracked = false;
                }
                else
                {
                    this.IsSectLocMapTracked = true;
                }
            }
        }

        public string LocMapSlotCodeMask
        {
            get
            {
                return locMapSlotCodeMask;
            }

            set
            {
                locMapSlotCodeMask = value;
                if (string.IsNullOrEmpty(locMapSlotCodeMask))
                {
                    this.IsSlotLocMapTracked = false;
                }
                else
                {
                    this.IsSlotLocMapTracked = true;
                }
            }
        }

        public string LocMapSlotCodeCoord1Mask
        {
            get
            {
                return locMapSlotCodeCoord1Mask;
            }

            set
            {
                locMapSlotCodeCoord1Mask = value;
                if (string.IsNullOrEmpty(locMapSlotCodeCoord1Mask))
                {
                    this.IsSlotCoord1LocMapTracked = false;
                }
                else
                {
                    this.IsSlotCoord1LocMapTracked = true;
                }
            }
        }

        public string LocMapSlotCodeCoord2Mask
        {
            get
            {
                return locMapSlotCodeCoord2Mask;
            }

            set
            {
                locMapSlotCodeCoord2Mask = value;
                if (string.IsNullOrEmpty(locMapSlotCodeCoord2Mask))
                {
                    this.IsSlotCoord2LocMapTracked = false;
                }
                else
                {
                    this.IsSlotCoord2LocMapTracked = true;
                }
            }
        }

        public string LocMapSlotCodeCoord3Mask
        {
            get
            {
                return locMapSlotCodeCoord3Mask;
            }

            set
            {
                locMapSlotCodeCoord3Mask = value;
                if (string.IsNullOrEmpty(locMapSlotCodeCoord3Mask))
                {
                    this.IsSlotCoord3LocMapTracked = false;
                }
                else
                {
                    this.IsSlotCoord3LocMapTracked = true;
                }
            }
        }

        public int CurrLocMapRefno
        {
            get
            {
                if (this.CurrLocMap.SlotLocMapRefno != 0)
                {
                    currLocMapRefno = this.CurrLocMap.SlotLocMapRefno;
                }
                else if (this.CurrLocMap.SectLocMapRefno != 0)
                {
                    currLocMapRefno = this.CurrLocMap.SectLocMapRefno;
                }
                else
                {
                    currLocMapRefno = this.CurrLocMap.WhsLocMapRefno;
                }

                return currLocMapRefno;
            }

            set
            {
                currLocMapRefno = value;
            }
        }

        public int LoginWhsLocMapRefno
        {
            get
            {
                return loginWhsLocMapRefno;
            }

            set
            {
                loginWhsLocMapRefno = value;
            }
        }

        private static LocationMap LocMap
        {
            get
            {
                return locMap;
            }

            set
            {
                locMap = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static LocationMap GetInstance()
        {
            if (LocMap == null)
            {
                LocMap = new LocationMap();
            }

            return LocMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        public void Load()
        {
            SysEnvironment sysEnv = SysEnvironment.GetInstance();

            this.LocMapWhsCodeMask = sysEnv.SysSettings.LocMapWarehouseMask;
            this.LocMapSectCodeMask = sysEnv.SysSettings.LocMapSectionMask;
            this.LocMapSlotCodeMask = sysEnv.SysSettings.LocMapSlotMask;
            this.LocMapSlotCodeCoord1Mask = sysEnv.SysSettings.LocMapSlotCoord1Mask;
            this.LocMapSlotCodeCoord2Mask = sysEnv.SysSettings.LocMapSlotCoord2Mask;
            this.LocMapSlotCodeCoord3Mask = sysEnv.SysSettings.LocMapSlotCoord3Mask;
        }

        #endregion
    }
}
