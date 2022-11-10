// <copyright file="ActivityDeboxLabelCount.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.View.CustomPages.Modal;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDeboxLabelCount : DMobileModal, IAppArea
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDeboxLabelCount"/> class.
        /// </summary>
        public ActivityDeboxLabelCount()
        {
            this.InitializeComponent();

            this.Title = "Debox Label Count";
            this.labelQuantity.Text = "Quantity";
        }

        public int Quantity { get; private set; }

        /// <inheritdoc/>
        public async Task ScannerDisable()
        {
        }

        /// <inheritdoc/>
        public async Task ScannerEnable()
        {
        }

        /// <inheritdoc/>
        public async Task<bool> IsReadyToClose()
        {
            return true;
        }

        public string GetControlDescription()
        {
            return "Debox Label Count";
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            if (await this.IsReadyToSave())
            {
                this.Quantity = Convert.ToInt32(this.entryQuantity.Text);
                saveComplete = true;
            }

            return saveComplete;
        }

        /// <inheritdoc/>
        public async Task<bool> CancelChanges()
        {
            return await this.IsReadyToClose();
        }

        /// <inheritdoc/>
        public async Task<bool> IsReadyToSave()
        {
            return true;
        }

        /// <inheritdoc/>
        public async Task<XDocument> GetXML()
        {
            var result = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<quantity>"
                    + this.Quantity
                    + "</quantity>");
            return result;
        }
    }
}