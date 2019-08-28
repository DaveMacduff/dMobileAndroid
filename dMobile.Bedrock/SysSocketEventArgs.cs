namespace dMobile.Bedrock
{
    using System;

    public class SysSocketEventArgs : EventArgs
    {
        // VARIABLES
        private string statusOutMsg = string.Empty;

        // CONSTRUCTORS
        public SysSocketEventArgs()
        {
        }

        public SysSocketEventArgs(string msgData)
        {
            this.StatusOutMsg = msgData;
        }

        // PROPERTIES
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
    }
}
