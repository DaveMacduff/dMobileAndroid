// <copyright file="LocationMapInfo.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.CustomControls
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationMapInfo : ContentView
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationMapInfo"/> class.
        /// </summary>
        public LocationMapInfo()
        {
            this.InitializeComponent();

            this.lblWarehouse.Text = "Warehouse";
            this.lblWarehouseData.Text = "Unknown";
            this.lblSection.Text = "Section";
            this.lblSectionData.Text = "Unknown";
            this.lblSlot.Text = "Slot";
            this.lblSlotData.Text = "Unknown";

            this.btnSave.Clicked += this.BtnSave_Clicked;
            this.btnCancel.Clicked += this.BtnCancel_Clicked;
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

        public EventHandler BtnSaveClick { get; set; }
        public EventHandler BtnCancelClick { get; set; }
    }
}
