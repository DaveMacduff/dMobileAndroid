// <copyright file="CustomPicker.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomControls
{
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="CustomPicker" />.
    /// </summary>
    public class CustomPicker : Picker
    {
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(CustomPicker), string.Empty);

        /// <summary>
        ///
        /// Gets or sets the SelectedValue.
        /// </summary>
        public int SelectedValue { get; set; }

        public string Image
        {
            get => (string)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }

        public string Text { get; set; } = string.Empty;
    }
}
