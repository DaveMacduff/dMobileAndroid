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
        private string text = string.Empty;
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(CustomPicker), string.Empty);
        
        /// <summary>
        /// 
        /// Gets or sets the SelectedValue
        /// </summary>
        public int SelectedValue { get => this.selectedValue; set => this.selectedValue = value; }

		public string Image
		{
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

        public string Text { get => text; set => text = value; }
    }
}
