using dMobile.Resources.Types;

namespace dMobile.Bedrock
{
    public class ComboBoxMembers
    {
        private string displayMember = string.Empty;
        private string valueMember = string.Empty;
        private Sort sort = new Sort();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMember"></param>
        /// <param name="valueMember"></param>
        public ComboBoxMembers(string displayMember, Sort sort)
        {
            this.DisplayMember = displayMember;
            this.Sort = sort;
        }

        public ComboBoxMembers(string displayMember, string valueMember)
        {
            this.DisplayMember = displayMember;
            this.ValueMember = valueMember;
        }

        public string DisplayMember { get => displayMember; set => displayMember = value; }
        public string ValueMember { get => valueMember; set => valueMember = value; }
        public Sort Sort { get => sort; set => sort = value; }
    }
}
