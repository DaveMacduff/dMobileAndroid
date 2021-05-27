using dMobile.Resources.Enums;

namespace dMobile.Resources.Types
{
    public class Sort
    {
        private string sortValue = string.Empty;
        private SortOrder sortOrder = SortOrder.ASC;

        public string SortValue { get => sortValue; set => sortValue = value; }
        public SortOrder SortOrder { get => sortOrder; set => sortOrder = value; }
    }
}
