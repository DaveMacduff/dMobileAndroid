// <copyright file="FormShipLabelQuantity.xaml.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Forms
{
    using System;
    using System.ComponentModel;
    using Android.Views;
    using Resources.Enums;
    using CustomPages.Modal;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Defines the <see cref="FormShipLabelQuantity" /> ship label quantity model.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormShipLabelQuantity : DMobileModal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormShipLabelQuantity"/> class.
        /// </summary>
        public FormShipLabelQuantity()
        {
            this.InitializeComponent();

            this.IsIndicatorRunning = false;
            this.Title = "Ship Label Quantity";
            this.cmdCancel.Text = "Cancel";
            this.cmdOK.Text = "OK";
            this.lblQuantity.Text = "Quantity";

            if (this.txtQuantity.Text != null)
            {
                this.txtQuantity.CursorPosition = 0;
                this.txtQuantity.SelectionLength = this.txtQuantity.Text.Length;
            }
        }

        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        /// On appearing event handler.
        /// </summary>
        protected override void OnAppearing()
        {
            if (!this.IsIndicatorRunning)
            {
                this.IsIndicatorRunning = true;

                base.OnAppearing();

                MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
                {
                    _ = Enum.TryParse(keyPressed, out Keycode keycode);

                    if (keycode == Keycode.Enter || keycode == Keycode.F1)
                    {
                        if (this.txtQuantity.Text != null)
                        {
                            this.DialogResult = DialogResult.OK;
                            this.Quantity = Convert.ToInt32(this.txtQuantity.Text);
                            await this.Close();
                        }
                    }
                });

                this.IsIndicatorRunning = false;
            }
        }

        /// <summary>
        /// On disappearing event handler.
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
            this.IsIndicatorRunning = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// On back button pressed event handler.
        /// </summary>
        /// <returns>The <see cref="bool"/> True.</returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = base.OnBackButtonPressed();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// Validate user data entry.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="CancelEventArgs"/> Cancel event arguments.</param>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            var lblQty = Convert.ToInt32(this.txtQuantity.Text);

            if (lblQty < 0 || lblQty > 999)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Command OK click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdOK_Click(object sender, EventArgs e)
        {
            this.Quantity = Convert.ToInt32(this.txtQuantity.Text);
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        /// <summary>
        /// Cancel command click event handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Event arguments.</param>
        private async void CmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }

        /// <summary>
        /// The txtQuantity_TextChanged.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="TextChangedEventArgs"/> Text changed event arguments.</param>
        private void TxtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ValidateEntry(sender, new CancelEventArgs());
        }
    }
}
