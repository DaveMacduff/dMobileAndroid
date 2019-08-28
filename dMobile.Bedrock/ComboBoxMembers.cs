namespace dMobile.Bedrock
{
    public class ComboBoxMembers
    {
        private string displayMember = string.Empty;
        private string valueMember = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMember"></param>
        /// <param name="valueMember"></param>
        public ComboBoxMembers(string displayMember, string valueMember)
        {
            this.DisplayMember = displayMember;
            this.ValueMember = valueMember;
        }

        public string DisplayMember
        {
            get
            {
                return this.displayMember;
            }

            set
            {
                this.displayMember = value;
            }
        }

        public string ValueMember
        {
            get
            {
                return this.valueMember;
            }

            set
            {
                this.valueMember = value;
            }
        }
    }
}
