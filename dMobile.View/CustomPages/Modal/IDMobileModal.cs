namespace dMobile.View.CustomPages.Modal
{
    using System.Threading.Tasks;
    using dMobile.Resources.Enums;

    public interface IDMobileModal
    {
        DialogResult DialogResult { get; set; }
        public Task<DialogResult> ShowDialog();
        public Task Close();
    }
}
