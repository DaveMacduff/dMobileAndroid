// <copyright file="ActivityConvertUnitWeightEdit.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Views
{
    using System;
    using CustomPages.Modal;
    using Resources.Enums;
    using Resources.Types;
    using Xamarin.Forms.Xaml;

    /// <summary>
    ///     Defines the <see cref="ActivityConvertUnitWeightEdit" />.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityConvertUnitWeightEdit : DMobileModal
    {
        /// <summary>
        ///     Defines the convertUnitDetail.
        /// </summary>
        private readonly ConvertUnitDetail convertUnitDetail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivityConvertUnitWeightEdit" /> class.
        /// </summary>
        public ActivityConvertUnitWeightEdit()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
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
        ///     Initializes a new instance of the <see cref="ActivityConvertUnitWeightEdit" /> class.
        /// </summary>
        /// <param name="convertUnitDetail">.</param>
        public ActivityConvertUnitWeightEdit(ConvertUnitDetail convertUnitDetail)
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.convertUnitDetail = convertUnitDetail;

            this.Title = "Unit Weight Edit";

            this.lblUnitNumber.Text = "Unit Number";
            this.lblUnitNumberValue.Text = Convert.ToString(convertUnitDetail.CountNumber) ?? string.Empty;

            this.lblQuantity.Text = "Quantity";
            this.lblQuantityValue.Text =
                Convert.ToString($"{convertUnitDetail.ActivityDetailQuantity:F3}") ?? string.Empty;

            this.lblWeight.Text = "Weight";
            this.entryWeight.Text =
                Convert.ToString($"{convertUnitDetail.ActivityDetailWeightQuantity:F3}") ?? string.Empty;

            this.cmdSave.Text = "Save";
            this.cmdCancel.Text = "Cancel";
        }

        /// <summary>
        ///     Gets or sets the EditedWeight.
        /// </summary>
        public decimal EditedWeight { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether WeightHasChanged.
        /// </summary>
        public bool WeightHasChanged { get; set; }

        /// <summary>
        ///     The cmdSave_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
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
        ///     The cmdCancel_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object" />.</param>
        /// <param name="e">The e<see cref="EventArgs" />.</param>
        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.WeightHasChanged = false;
            await this.Close();
        }
    }
}