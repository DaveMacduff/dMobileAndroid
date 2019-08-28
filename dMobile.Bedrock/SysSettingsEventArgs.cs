using System;

namespace dMobile.Bedrock
{
    public class SysSettingsEventArgs : EventArgs
    {
        #region Fields

        private string statusOutMsg = string.Empty;
        private int minValue = 0;
        private int maxValue = 0;
        private int currentValue = 0;

        #endregion

        #region Constructors

        public SysSettingsEventArgs()
        {
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataMsg"></param>
        public SysSettingsEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        #endregion

        #region Properties

        public string StatusOutMsg
        {
            get
            {
                return this.statusOutMsg;
            }

            set
            {
                this.statusOutMsg = value;
            }
        }

        public int MinValue
        {
            get
            {
                return this.minValue;
            }

            set
            {
                this.minValue = value;
            }
        }

        public int MaxValue
        {
            get
            {
                return this.maxValue;
            }

            set
            {
                this.maxValue = value;
            }
        }

        public int CurrentValue
        {
            get
            {
                return this.currentValue;
            }

            set
            {
                this.currentValue = value;
            }
        }

        #endregion
    }
}
