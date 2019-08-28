namespace dMobile.Bedrock
{
    using System;

    public class ContactLoginEventArgs : EventArgs
    {
        #region Fields

        private string statusOutMsg = string.Empty;

        #endregion

        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public ContactLoginEventArgs()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataMsg"></param>
        public ContactLoginEventArgs(string msgData)
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

        #endregion
    }
}
