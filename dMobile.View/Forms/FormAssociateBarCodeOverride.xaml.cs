namespace dMobile.View.Forms
{
    using Android.Views;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Enums;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using dMobile.View.CustomPages.Modal;
    using Resource = dMobile.Resources.Properties.Resources;

    /// <summary>
    /// Defines the <see cref="FormAssociateBarCodeOverride" />
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormAssociateBarCodeOverride : DialogModal
    {
        /// <summary>
        /// Defines the userCode
        /// </summary>
        private int userCode = 0;

        /// <summary>
        /// Defines the password
        /// </summary>
        private string password = string.Empty;

        /// <summary>
        /// Defines the isAllowedAssocBarCode
        /// </summary>
        private bool isAllowedAssocBarCode = false;

        /// <summary>
        /// Defines the isEnableAllModuleBypassLicense
        /// </summary>
        private bool isEnableAllModuleBypassLicense = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormAssociateBarCodeOverride"/> class.
        /// </summary>
        public FormAssociateBarCodeOverride()
        {
            InitializeComponent();

            this.lblLoginID.Text = Resource.LoginID;
            this.lblLoginPassword.Text = Resource.Password;
            this.cmdCancel.Text = Resource.Cancel;
            this.cmdOK.Text = Resource.OK;

            if (!DesignMode.IsDesignModeEnabled)
            {
                if (SysEnvironment.GetInstance().IsEnableAllModuleBypassLicense)
                {
                    this.txtLoginPassword.BackgroundColor = Color.Aqua;
                    this.IsEnableAllModuleBypassLicense = true;
                }
                else
                {
                    //this.txtLoginPassword.BackgroundColor = SystemColors.Window;
                    this.IsEnableAllModuleBypassLicense = false;
                }
            }
        }
        
        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.txtLoginID.Focus();
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnableAllModuleBypassLicense
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether IsAllowedAssocBarCode
        /// </summary>
        private bool IsAllowedAssocBarCode
        {
            get
            {
                return this.isAllowedAssocBarCode;
            }

            set
            {
                this.isAllowedAssocBarCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the UserCode
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
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

        /// <summary>
        /// The cmdOK_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdOK_Click(object sender, EventArgs e)
        {
            bool isValidData = true;

            try
            {
                if (string.IsNullOrEmpty(this.txtLoginPassword.Text))
                {
                    isValidData = false;
                }
                else if (this.UserCode != Convert.ToInt32(this.txtLoginID.Text))
                {
                    isValidData = false;
                }
                else if (this.Password != this.txtLoginPassword.Text)
                {
                    isValidData = false;
                }
            }
            catch (Exception)
            {
                // Catch if convert to int fails.
                isValidData = false;
            }

            if (isValidData && this.IsAllowedAssocBarCode)
            {
                this.DialogResult = DialogResult.OK;
                await this.Close();
            }
            else
            {
                await DisplayAlert(Resource.BarCodeAssociation, Resource.PermissionDenied, Resource.OK);
            }
        }

        /// <summary>
        /// The cmdCancel_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cmdCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            await this.Close();
        }

        /// <summary>
        /// The ValidateEntry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ValidateEntry(object sender, CancelEventArgs e)
        {
            TableContactUsers tableContactUsers = new TableContactUsers();

            try
            {
                if (!string.IsNullOrEmpty(this.txtLoginID.Text))
                {
                    using (SQLiteCommand cmndUser = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                    {
                        cmndUser.CommandText = "SELECT * FROM tContactUser WHERE ContactUserCode = @contacUserCode";
                        cmndUser.Parameters.Add(new SQLiteParameter("@contacUserCode", Convert.ToInt32(this.txtLoginID.Text)));
                        //cmndUser.IndexName = "UQ_ContactUserOnContactUserCode";
                        cmndUser.CommandType = CommandType.Text;

                        using (SQLiteDataReader drUser = cmndUser.ExecuteReader())
                        {
                            if (drUser.Read())
                            {
                                this.Password = drUser.GetString((int)SysCommon.ContactUserFields.Password);
                                this.UserCode = drUser.GetInt32((int)SysCommon.ContactUserFields.Code);
                                this.IsAllowedAssocBarCode = drUser.GetBoolean((int)SysCommon.ContactUserFields.IsAllowedAssocProdBarCode);
                            }

                            drUser.Close();
                        }
                    }
                }

                if (sender == this.txtLoginID)
                {
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
#if MULTILANGUAGE
                    throw new MessageOnlyException(string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.InvalidLoginID), this.txtLoginID.Text));
#else
                        throw new MessageOnlyException(Resource.InvalidLoginId + " " + this.txtLoginID.Text);
#endif
                    }
                }

                if (sender == this.txtLoginPassword)
                {
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
#if MULTILANGUAGE
                    throw new MessageOnlyException(Translator.GetInstance().Translate(TranslatorVocabulary.InvalidLoginPassword));
#else
                        throw new MessageOnlyException(Resource.InvalidLoginPassword);
#endif
                    }
                }
            }
            catch (MessageOnlyException ex)
            {
                e.Cancel = true;

                await DisplayAlert(Resource.Barcode, ex.Message, Resource.OK);
            }
            catch (SQLiteException ex)
            {
                e.Cancel = true;
                
                await new FormErrorReport(ex).ShowDialog();
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                await new FormErrorReport(ex).ShowDialog();
            }
        }

        private async void CmdOK_Clicked(object sender, EventArgs e)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                await new FormErrorReport(ex).ShowDialog();
            }
        }
    }
}
