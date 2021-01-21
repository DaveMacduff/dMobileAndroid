using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dMobile.View.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationMapInfo : ContentView
    {
        private EventHandler btnSaveClick;
        private EventHandler btnCancelClick;

        public LocationMapInfo()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.lblWarehouse.Text = "Warehouse";
            this.lblWarehouseData.Text = "Unknown";
            this.lblSection.Text = "Section";
            this.lblSectionData.Text = "Unknown";
            this.lblSlot.Text = "Slot";
            this.lblSlotData.Text = "Unknown";

            this.btnSave.Clicked += BtnSave_Clicked;
            this.btnCancel.Clicked += BtnCancel_Clicked;
        }

        // lblWarehouseData functions
        public void SetWarehouseDataText(string warehouseData)
        {
            this.lblWarehouseData.Text = warehouseData;
        }

        public void SetWarehouseDataTextColor(Color color)
        {
            this.lblWarehouseData.TextColor = color;
        }

        // lblSectionData functions
        public void SetSectionDataText(string sectionData)
        {
            this.lblSectionData.Text = sectionData;
        }

        public void SetSectionDataTextColor(Color color)
        {
            this.lblSectionData.TextColor = color;
        }

        // lblSlotData functions
        public void SetSlotDataText(string slotData)
        {
            this.lblSlotData.Text = slotData;
        }

        public void SetSlotDataTextColor(Color color)
        {
            this.lblSlotData.TextColor = color;
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            this.BtnSaveClick?.Invoke(sender, e);
        }

        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            this.BtnCancelClick?.Invoke(sender, e);
        }

        public EventHandler BtnSaveClick { get => this.btnSaveClick; set => this.btnSaveClick = value; }
        public EventHandler BtnCancelClick { get => this.btnCancelClick; set => this.btnCancelClick = value; }
    }
}
