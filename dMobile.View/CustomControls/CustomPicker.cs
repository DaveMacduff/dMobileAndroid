namespace dMobile.View.CustomControls
{
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="CustomPicker" />
    /// </summary>
    public class CustomPicker : Picker
    {
        /// <summary>
        /// Defines the selectedValue
        /// </summary>
        private int selectedValue;

        /// <summary>
        /// Gets or sets the SelectedValue
        /// </summary>
        public int SelectedValue { get => this.selectedValue; set => this.selectedValue = value; }
    }
}
