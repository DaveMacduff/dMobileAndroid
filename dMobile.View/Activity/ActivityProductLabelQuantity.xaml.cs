// <copyright file="ActivityProductLabelQuantity.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Activity
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.View.CustomPages.Modal;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityProductLabelQuantity : DMobileModal, IAppArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProductLabelQuantity"/> class.
        /// </summary>
        public ActivityProductLabelQuantity()
        {
            this.InitializeComponent();

            this.Title = "Product Label Count";
            this.labelQuantity.Text = "Quantity";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProductLabelQuantity"/> class.
        /// </summary>
        /// <param name="labelQuantity">Label quantity.</param>
        public ActivityProductLabelQuantity(decimal labelQuantity)
        {
            this.InitializeComponent();

            this.Title = "Product Label Count";
            this.labelQuantity.Text = "Quantity";
            this.entryQuantity.Text = Convert.ToString(labelQuantity, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

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

        /// <inheritdoc/>
        public string GetControlDescription()
        {
            return "Product Label Quantity";
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChanges()
        {
            var saveComplete = false;

            if (await this.IsReadyToSave())
            {
                this.Quantity = Convert.ToInt32(this.entryQuantity.Text);
                saveComplete = true;
                ////var sendReq = await this.ActDetail.CreateSubmitProductLabelXml();
                ////_ = SysCommon.SubmitComm(sendReq);

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