namespace dMobile.Bedrock
{
    using System;

    public class DataGridEnableEventArgs : EventArgs
    {
        #region Fields

        private int column = 0;
        private int row = 0;
        private bool isMeetsCriteria = false;

        #endregion

        #region Constructors

        public DataGridEnableEventArgs(int row, int col, bool isMeetsCriteria)
        {
            this.Row = row;
            this.Column = col;
            this.MeetsCriteria = isMeetsCriteria;
        }

        #endregion

        #region Public Properties

        public int Column
        {
            get
            {
                return this.column;
            }

            set
            {
                this.column = value;
            }
        }

        public int Row
        {
            get
            {
                return this.row;
            }

            set
            {
                this.row = value;
            }
        }

        public bool MeetsCriteria
        {
            get
            {
                return this.isMeetsCriteria;
            }

            set
            {
                this.isMeetsCriteria = value;
            }
        }

        #endregion
    }
}
