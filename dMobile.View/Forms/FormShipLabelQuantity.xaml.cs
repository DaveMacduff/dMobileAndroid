using Java.Util.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using dMobile.Bedrock;
using dMobile.GroundWork;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dMobile.View.CustomPages.Modal;
using dMobile.Resources.Enums;
using Android.Views;

namespace dMobile.View.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormShipLabelQuantity : DMobileModal
    {
        bool isNumericTrapped = false;
        bool isEditTrapped = false;
        int quantity = 0;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public FormShipLabelQuantity()
        {
            InitializeComponent();

            // TODO: Resource strings
            this.Title = "Ship Label Quantity";
            this.cmdCancel.Text = "Cancel";
            this.cmdOK.Text = "Ok";
            this.lblQuantity.Text = "Quantity";

            if (this.txtQuantity.Text != null)
            {
                this.txtQuantity.CursorPosition = 0;
                this.txtQuantity.SelectionLength = this.txtQuantity.Text.Length;
            }
        }

        #endregion

        #region OnAppearing/OnDisappearing

        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                Enum.TryParse(keyPressed, out Keycode keycode);

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
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();
                this.DialogResult = DialogResult.None;
                await this.Close();
            });

            return true;
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            base.OnDisappearing();
        }

        #endregion

        #region Properties

        public int Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
            }
        }

        private bool IsNumericTrapped
        {
            get
            {
                return this.isNumericTrapped;
            }

            set
            {
                this.isNumericTrapped = value;
            }
        }

        private bool IsEditTrapped
        {
            get
            {
                return this.isEditTrapped;
            }

            set
            {
                this.isEditTrapped = value;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateEntry(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            int lblQty = Convert.ToInt32(this.txtQuantity.Text);

            if (lblQty < 0 || lblQty > 999)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Private Members

        private async void cmdOK_Click(object sender, EventArgs e)
        {
            this.Quantity = Convert.ToInt32(this.txtQuantity.Text);
            this.DialogResult = DialogResult.OK;
            await this.Close();
        }

        private async void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            await this.Close();
        }

        #endregion

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ValidateEntry(sender, new CancelEventArgs());
        }
    }
}