using dMobile.View.CustomPages.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using dMobile.Bedrock;
using dMobile.GroundWork;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;
using SQLite;
using dMobile.Resources.Enums;
using Android.Views;
using dMobile.Resources.Models;

namespace dMobile.View.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormAddManualDetailOverride : DMobileModal
    {
        #region Fields

        private int userCode = 0;
        private string password = string.Empty;
        private bool isAllowedAddManualDetail = false;
        private bool isEnableAllModuleBypassLicense = false;

        #endregion

        #region Constructors

        public FormAddManualDetailOverride()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = "Manual Detail Override";
            this.cmdCancel.Text = "Cancel";
            this.lblLoginPassword.Text = "Password";
            this.lblLoginID.Text = "LoginID";
            this.cmdOK.Text = "Ok";

            this.DialogResult = dMobile.Resources.Enums.DialogResult.None;

            if (SysEnvironment.GetInstance().IsEnableAllModuleBypassLicense)
            {
                this.txtLoginPassword.BackgroundColor = Color.Aqua;
                this.IsEnableAllModuleBypassLicense = true;
            }
            else
            {
                //this.txtLoginPassword.BackColor = SystemColors.Window;
                this.IsEnableAllModuleBypassLicense = false;
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
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (!this.Loading)
                {
                    if (keycode == Keycode.Escape)
                    {
                        await this.Close();
                    }
                    else if (keycode == Keycode.F1)
                    {
                        this.cmdOK_Click(null, new EventArgs());
                    }
                }
            });
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

        private bool IsEnableAllModuleBypassLicense
        {
            get
            {
                return this.isEnableAllModuleBypassLicense;
            }

            set
            {
                this.isEnableAllModuleBypassLicense = value;
            }
        }

        private bool IsAllowedAddManualDetail
        {
            get
            {
                return this.isAllowedAddManualDetail;
            }

            set
            {
                this.isAllowedAddManualDetail = value;
            }
        }

        private int UserCode
        {
            get
            {
                return this.userCode;
            }

            set
            {
                this.userCode = value;
            }
        }

        private string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdOK_Click(object sender, EventArgs e)
        {
            bool isValidData = true;

            if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
            {
                isValidData = false;
            }

            if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
            {
                isValidData = false;
            }

            if (this.Password != this.txtLoginPassword.Text)
            {
                isValidData = false;
            }

            if (isValidData && this.IsAllowedAddManualDetail)
            {
                this.DialogResult = dMobile.Resources.Enums.DialogResult.OK;
            }
            else
            {
                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Add Manual Detail Override", "Permission denied.", "OK"));
            }

            await this.Close();
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdCancel_Click(object sender, EventArgs e)
        {
            await this.Close();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            TableContactUsers tableContactUsers = new TableContactUsers();

            try
            {
                if (!string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    List<tContactUserModel> resultsContactUser = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<tContactUserModel>("SELECT * FROM tContactUser WHERE ContactUserCode = ?", Convert.ToInt32(txtLoginID.Text));

                    if (resultsContactUser?.Count > 0)
                    {
                        this.Password = resultsContactUser[0].ContactUserPassword;
                        this.UserCode = resultsContactUser[0].ContactUserCode;
                        this.IsAllowedAddManualDetail = resultsContactUser[0].ContactUserIsAllowedPickingModuleOverrideAddManualDetail;
                    }
                }

                if (true == string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    this.txtLoginPassword.Text = string.Empty;
                    this.Password = string.Empty;
                }
                else if (true == string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    this.Password = string.Empty;
                }
                else if (true == (this.UserCode != Convert.ToInt32(this.txtLoginID.Text)))
                {
                    // TODO: Resource strings
                    throw new MessageOnlyException(string.Format("Invalid login id {0}", this.txtLoginID.Text));
                }

                // Override the contact login password with the system one if bypass licenses is enabled.
                if (this.IsEnableAllModuleBypassLicense)
                {
                    this.Password = SysCommon.SystemLoginPassword;
                }

                if (true == string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    this.txtLoginPassword.Text = string.Empty;
                    this.Password = string.Empty;
                }
                else if (true == string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    this.Password = string.Empty;
                }
                else if (true == (this.Password != this.txtLoginPassword.Text))
                {
                    // TODO: Resource strings
                    throw new MessageOnlyException("Invalid login password.");
                }
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = true;
                // Did not call ex.Show(this, SysCommon.ActSubTypes.BarCodeAssociation) because frmAssociateBarCodeOverride
                // does not implement the IAppArea.
                //MessageBox.Show(ex.Message,"Add Manual Detail Override",MessageBoxButtons.OK,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1);

                // TODO: Resource strings
                await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Add Manual Detail Override", ex.Message, "OK"));
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(ex);
                    await errRpt.ShowDialog();
                });
            }
        }

        #endregion
    }
}