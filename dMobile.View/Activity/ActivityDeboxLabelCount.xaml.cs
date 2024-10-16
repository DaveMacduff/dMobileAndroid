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
    using Bedrock;
    using CustomPages.Modal;
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
        public Task ScannerDisable()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ScannerEnable()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<bool> IsReadyToClose()
        {
            return Task.FromResult(true);
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
        public Task<bool> IsReadyToSave()
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<XDocument> GetXml()
        {
            var result = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                    + "<quantity>"
                    + this.Quantity
                    + "</quantity>");
            return Task.FromResult(result);
        }
    }
}