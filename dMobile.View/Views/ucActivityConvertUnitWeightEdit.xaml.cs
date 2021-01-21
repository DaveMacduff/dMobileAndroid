using System;
using System.Collections.Generic;
using dMobile.Resources.Types;
using dMobile.Resources.Enums;
using dMobile.View.CustomPages.Modal;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucActivityConvertUnitWeightEdit : DMobileModal
    {
        private bool weightHasChanged = false;
        private decimal editedWeight;
        private ConvertUnitDetail convertUnitDetail;

        public ucActivityConvertUnitWeightEdit()
        {
            InitializeComponent();

            // TODO: Resource strings
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

        public ucActivityConvertUnitWeightEdit(ConvertUnitDetail convertUnitDetail)
        {
            InitializeComponent();

            this.convertUnitDetail = convertUnitDetail;

            // TODO: Resource strings
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

        public decimal EditedWeight { get => editedWeight; set => editedWeight = value; }
        public bool WeightHasChanged { get => weightHasChanged; set => weightHasChanged = value; }

        private async void cmdSave_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            decimal.TryParse(this.entryWeight.Text, out decimal result);

            if (this.convertUnitDetail != null && result != this.convertUnitDetail.ActivityDetailWeightQuantity)
            {
                this.EditedWeight = result;
                this.WeightHasChanged = true;
            }

            await this.Close();
        }

        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.WeightHasChanged = false;
            await this.Close();
        }
    }
}
