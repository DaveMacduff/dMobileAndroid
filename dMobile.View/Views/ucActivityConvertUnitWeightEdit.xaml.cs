// <copyright file="ucActivityConvertUnitWeightEdit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Views
{
    using System;
    using dMobile.Resources.Enums;
    using dMobile.Resources.Types;
    using dMobile.View.CustomPages.Modal;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="ucActivityConvertUnitWeightEdit" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertUnitWeightEdit : DMobileModal
    {
        /// <summary>
        /// Defines the convertUnitDetail.
        /// </summary>
        private ConvertUnitDetail convertUnitDetail;

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityConvertUnitWeightEdit"/> class.
        /// </summary>
        public ucActivityConvertUnitWeightEdit()
        {
            this.InitializeComponent();

            this.Title = "Unit Weight Edit";

            this.lblUnitNumber.Text = "Unit Number";
            this.lblUnitNumberValue.Text = string.Empty;

            this.lblQuantity.Text = "Quantity";
            this.lblQuantityValue.Text = string.Empty;

            this.lblWeight.Text = "Weight";
            this.entryWeight.Text = string.Empty;

            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ucActivityConvertUnitWeightEdit"/> class.
        /// </summary>
        /// <param name="convertUnitDetail">.</param>
        public ucActivityConvertUnitWeightEdit(ConvertUnitDetail convertUnitDetail)
        {
            this.InitializeComponent();

            this.convertUnitDetail = convertUnitDetail;

            this.Title = "Unit Weight Edit";

            this.lblUnitNumber.Text = "Unit Number";
            this.lblUnitNumberValue.Text = Convert.ToString(convertUnitDetail.CountNumber) ?? string.Empty;

            this.lblQuantity.Text = "Quantity";
            this.lblQuantityValue.Text = Convert.ToString(string.Format("{0:F3}", convertUnitDetail.ActivityDetailQuantity)) ?? string.Empty;

            this.lblWeight.Text = "Weight";
            this.entryWeight.Text = Convert.ToString(string.Format("{0:F3}", convertUnitDetail.ActivityDetailWeightQuantity)) ?? string.Empty;

            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";
        }

        /// <summary>
        /// Gets or sets the EditedWeight.
        /// </summary>
        public decimal EditedWeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether WeightHasChanged.
        /// </summary>
        public bool WeightHasChanged { get; set; } = false;

        /// <summary>
        /// The cmdSave_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            _ = decimal.TryParse(this.entryWeight.Text, out var result);

            if (this.convertUnitDetail != null && result != this.convertUnitDetail.ActivityDetailWeightQuantity)
            {
                this.EditedWeight = result;
                this.WeightHasChanged = true;
            }

            await this.Close();
        }

        /// <summary>
        /// The cmdCancel_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.WeightHasChanged = false;
            await this.Close();
        }
    }
}
