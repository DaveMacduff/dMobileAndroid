namespace dMobile.View.Utilities
{
    using Android.Views;
    using dMobile.BarCodeScanner;
    using dMobile.Bedrock;
    using dMobile.Controller;
    using dMobile.Resources.Enums;
    using dMobile.View.CustomPages;
    using dMobile.View.Forms;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Color = Xamarin.Forms.Color;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ucAssignDefaultLocation : DMobileContentPage, IAppArea
    {
        private bool isControlLoaded = false;
        private bool changesMade = false;
        private SysEnvironment sysEnv;
        private LocationMap locMap;
        private BarCodeScanner scanner;
        private bool isBarCodeHandlerRegistered = false;
        private SysCommon.ActSubTypes actSubTypes;
        private Controller.Activity activity = null;
        private ActItem actItem = null;
        private ActDetail actDetail = null;
        private int currLocMapDefPickingRefno = 0;
        private int currLocMapDefRecvRefno = 0;
        private int newLocMapDefPickingRefno = 0;
        private int newLocMapDefRecvRefno = 0;
        private SLocMap locMapDefPicking;
        private SLocMap locMapDefRecv;
        private bool isFirstAppearance = true;

        public ucAssignDefaultLocation()
        {
            this.InitializeComponent();

            // TODO: Resource strings
            this.Title = this.GetControlDescription();
            this.cmdLocation.Text = "Set Location";
            this.cmdProduct.Text = "Select Product";
            this.lblCurrentLocation.Text = "Current Default Location(s)";
            this.lblDefaultPicking.Text = "Picking";
            this.lblDefaultReceiving.Text = "Receiving";
            this.lblCurrentReceiving.Text = "Receiving";
            this.lblCurrentReceivingLocation.Text = "Not Set";
            this.lblCurrentPicking.Text = "Picking";
            this.lblCurrentPickingLocation.Text = "Not Set";
            this.chkDefaultPicking.IsToggled = true;

            if (!DesignMode.IsDesignModeEnabled)
            {
                sysEnv = SysEnvironment.GetInstance();
                locMap = LocationMap.GetInstance();
                scanner = BarCodeSingleton.GetInstance().Scanner;
                locMapDefPicking = new SLocMap();
                locMapDefRecv = new SLocMap();

                this.updnBarCodeModifier1.Items.Clear();

                if (this.SysEnv.SysSettings.IsProductUnitBarCodeModifier1)
                {
                    this.updnBarCodeModifier1.Items.Add(string.Empty);
                    string[] modifiers = this.SysEnv.SysSettings.ProductUnitBarCodeModifier1Codes.Split(',');

                    foreach (string item in modifiers)
                    {
                        this.updnBarCodeModifier1.Items.Add(item);
                    }
                }
                else
                {
                    this.updnBarCodeModifier1.IsVisible = false;
                    this.updnBarCodeModifier1.IsEnabled = false;
                }

                this.updnBarCodeModifier1.SelectedItem = this.SysEnv.CurrentBarCodeModifier1;

                this.Activity = ActivityFacade.GetActivity(SysCommon.ActSubTypes.AssignDefaultLocation);
                this.ActItem = ActItemFacade.GetActItem(this.Activity, SysCommon.ActSubTypes.AssignDefaultLocation);
                this.ActDetail = ActDetailFacade.GetActDetail(this.ActItem, SysCommon.ActSubTypes.AssignDefaultLocation);
            }
        }


        /// <summary>
        /// The OnAppearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, EventTypes.OnKeyUp.ToString(), async (keyPressed) =>
            {
                // TODO: Verify keycodes
                Enum.TryParse(keyPressed, out Keycode keycode);

                if (keycode == Keycode.Enter || keycode == Keycode.F1)
                {
                    if (await this.SaveChanges())
                    {
                        await this.Close();
                    }
                }
            });

            try
            {
                if (this.isFirstAppearance)
                {
                    this.isFirstAppearance = false;

                    if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                    {
                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.LocMap.LoginWhsLocMapRefno);
                    }

                    // TODO: Resource strings
                    await Device.InvokeOnMainThreadAsync(() => this.cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(true));
                }

                await this.AssignDefaultLocation_GotFocus(this, new EventArgs());
            }
            catch (Exception ex)
            {
                // TODO: Logging
                Console.WriteLine("this.AssignDefaultLocation_GotFocus threw error " + ex);
            }
        }

        /// <summary>
        /// The OnDisappearing
        /// </summary>
        protected override async void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, EventTypes.OnKeyUp.ToString());
            await this.AssignDefaultLocation_LostFocus(this, new EventArgs());

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                if (await this.SaveChanges())
                {
                    await this.Close();
                }
                else if (await this.CancelChanges())
                {
                    await this.Close();
                }
            });

            return true;
        }

        private int CurrLocMapDefRecvRefno
        {
            get
            {
                return this.currLocMapDefRecvRefno;
            }

            set
            {
                this.currLocMapDefRecvRefno = value;
            }
        }

        private int CurrLocMapDefPickingRefno
        {
            get
            {
                return this.currLocMapDefPickingRefno;
            }

            set
            {
                this.currLocMapDefPickingRefno = value;
            }
        }

        private BarCodeScanner Scanner
        {
            get
            {
                return this.scanner;
            }

            set
            {
                this.scanner = value;
            }
        }

        private int NewLocMapDefRecvRefno
        {
            get
            {
                return this.newLocMapDefRecvRefno;
            }

            set
            {
                this.newLocMapDefRecvRefno = value;
            }
        }

        private int NewLocMapDefPickingRefno
        {
            get
            {
                return this.newLocMapDefPickingRefno;
            }

            set
            {
                this.newLocMapDefPickingRefno = value;
            }
        }

        private SLocMap LocMapDefRecv
        {
            get
            {
                return this.locMapDefRecv;
            }

            set
            {
                this.locMapDefRecv = value;
            }
        }

        private SLocMap LocMapDefPicking
        {
            get
            {
                return this.locMapDefPicking;
            }

            set
            {
                this.locMapDefPicking = value;
            }
        }

        private ActDetail ActDetail
        {
            get
            {
                return this.actDetail;
            }

            set
            {
                this.actDetail = value;
            }
        }

        private ActItem ActItem
        {
            get
            {
                return this.actItem;
            }

            set
            {
                this.actItem = value;
            }
        }

        private Controller.Activity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
            }
        }

        private SysCommon.ActSubTypes ActSubType
        {
            get
            {
                return this.actSubTypes;
            }

            set
            {
                this.actSubTypes = value;
            }
        }

        private LocationMap LocMap
        {
            get
            {
                return this.locMap;
            }

            set
            {
                this.locMap = value;
            }
        }

        private bool ChangesMade
        {
            get
            {
                return this.changesMade;
            }

            set
            {
                this.changesMade = value;
            }
        }

        private bool IsControlLoaded
        {
            get
            {
                return this.isControlLoaded;
            }

            set
            {
                this.isControlLoaded = value;
            }
        }

        private SysEnvironment SysEnv
        {
            get
            {
                return this.sysEnv;
            }

            set
            {
                this.sysEnv = value;
            }
        }

        private bool IsBarCodeHandlerRegistered
        {
            get
            {
                return this.isBarCodeHandlerRegistered;
            }

            set
            {
                this.isBarCodeHandlerRegistered = value;
            }
        }



        /// <summary>
        /// Unregister the bar code scan event handler and disable the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerDisable()
        {
            if (this.IsBarCodeHandlerRegistered && this.Scanner != null)
            {
                this.Scanner.BarCodeScan -= new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
                await this.Scanner.Stop();
                this.IsBarCodeHandlerRegistered = false;
            }
        }

        /// <summary>
        /// Registers the bar code scan event handler and enables the scanner.
        /// </summary>
        /// <remarks></remarks>
        public async Task ScannerEnable()
        {
            if (!this.IsBarCodeHandlerRegistered)
            {
                this.IsBarCodeHandlerRegistered = true;
                await this.Scanner.Start();
                this.Scanner.BarCodeScan += new BarCodeScanner.BarCodeScanEventHandler(this.BarCodeHandler);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToSave()
        {
            bool isReadyToSave = true;

            try
            {
                if (!await LocationMapTasks.IsCompleteLocMapRefno(this.ActItem.LocMap.CurrLocMapRefno))
                {
                    if (0 >= this.LocMap.CurrLocMap.WhsLocMapRefno)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Warehouse is required.");
                    }

                    if (this.LocMap.CurrLocMap.IsSectionTracked && 0 >= this.LocMap.CurrLocMap.SectLocMapRefno)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Section is required.");
                    }

                    if (this.LocMap.CurrLocMap.IsSlotTracked && 0 >= this.LocMap.CurrLocMap.SlotLocMapRefno)
                    {
                        // TODO: Resource strings
                        throw new Exceptions.MessageOnlyException("Slot is required.");
                    }
                }

                if (this.ActItem.ProdInfo.UnitRefno == 0)
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("A selected product is required.");
                }

                if (!(chkDefaultPicking.IsToggled || chkDefaultReceiving.IsToggled))
                {
                    // TODO: Resource strings
                    throw new Exceptions.MessageOnlyException("A selected default picking or receiving option is required.");
                }

                if (isReadyToSave && this.chkDefaultPicking.IsToggled && this.CurrLocMapDefPickingRefno == this.NewLocMapDefPickingRefno)
                {
                    // TODO: Resource strings
                    string msg = "The current and the new default picking locations are the same." + Environment.NewLine + "Are you sure that you want to update the location anyway?";

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        bool result = await DisplayAlert("Ready To Save", msg, "Yes", "No");

                        if (!result)
                        {
                            isReadyToSave = false;
                        }
                    });
                }

                if (isReadyToSave && this.chkDefaultReceiving.IsToggled && this.CurrLocMapDefRecvRefno == this.NewLocMapDefRecvRefno)
                {
                    // TODO: Resource strings
                    string msg = "The current and the new default receiving locations are the same." + Environment.NewLine + "Are you sure that you want to update the location anyway?";

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        bool result = await DisplayAlert("Ready To Save", msg, "Yes", "No");

                        if (!result)
                        {
                            isReadyToSave = false;
                        }
                    });
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                isReadyToSave = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                isReadyToSave = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });

            }

            return isReadyToSave;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> IsReadyToClose()
        {
            // NOTE: This code is needed to clean up the scanner driver.
            // Symbol had threading issues without this call when dMobile was terminating.
            // All user controls that initalize a bar code need this code.
            // The parent form (not a user control) will call scanner.terminate and dispose
            // of the scanner instance upon exiting dMobile.
            await this.ScannerDisable();

            return !this.ChangesMade;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetControlDescription()
        {
            // TODO: Resource strings
            return "Assign Default Location";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> CancelChanges()
        {
            bool cancelChanges = true;

            try
            {
                if (!await this.IsReadyToClose())
                {
                    this.LoadData();
                    cancelChanges = false;
                    this.ChangesMade = false;
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                ex.Show(this.ActSubType);
                cancelChanges = false;
            }
            catch (Exception ex)
            {
                cancelChanges = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return cancelChanges;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<XDocument> GetXML()
        {
            XDocument xmldocParams = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "</params>");

            return xmldocParams;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public async Task<bool> SaveChanges()
        {
            bool saveComplete = false;

            try
            {
                if (await this.IsReadyToSave())
                {
                    this.ChangesMade = false;

                    string sendRequest = string.Empty;
                    string data = string.Empty;
                    StringBuilder xmlString = new StringBuilder();
                    XmlWriter xml = XmlWriter.Create(xmlString);
                    XDocument request = null;

                    if (this.NewLocMapDefPickingRefno == 0)
                    {
                        this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefPickingRefno);
                    }
                    else
                    {
                        this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.NewLocMapDefPickingRefno);
                    }

                    if (this.NewLocMapDefRecvRefno == 0)
                    {
                        this.LocMapDefRecv = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefRecvRefno);
                    }
                    else
                    {
                        this.LocMapDefRecv = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.NewLocMapDefRecvRefno);
                    }

                    request = XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                        + "<imclient>"
                        + "<command>"
                        + "<action>SUBMIT ASSIGN DEFAULT PRODUCT LOCATION</action>"
                        + SysCommon.GetRequestDefaults(false)
                        + "</command>"
                        + "<receiving>"
                        + ("<prod_refno>" + this.ActItem.ProdInfo.FKRefno + "</prod_refno>")
                        + ("<is_update>" + this.chkDefaultReceiving.IsToggled + "</is_update>")
                        + ("<whs_id>" + this.LocMapDefRecv.WhsFKRefno + "</whs_id>")
                        + ("<section>" + this.LocMapDefRecv.SectName + "</section>")
                        + ("<slot>" + this.LocMapDefRecv.SlotName + "</slot>")
                        + "</receiving>"
                        + "<picking>"
                        + ("<prod_refno>" + this.ActItem.ProdInfo.FKRefno + "</prod_refno>")
                        + ("<is_update>" + this.chkDefaultPicking.IsToggled + "</is_update>")
                        + ("<whs_id>" + this.LocMapDefPicking.WhsFKRefno + "</whs_id>")
                        + ("<section>" + this.LocMapDefPicking.SectName + "</section>")
                        + ("<slot>" + this.LocMapDefPicking.SlotName + "</slot>")
                        + "</picking>"
                        + "</imclient>");

                    request.Save(xml);
                    xml.Flush();
                    xml.Close();

                    sendRequest = xmlString.ToString();

                    // Submit the xml request
                    data = SysCommon.SubmitComm(sendRequest);

                    if (data.Length > 0)
                    {
                        // check if result is an error message
                        if (data.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument docError = new XmlDocument();
                            docError.LoadXml(data);

                            string errorDetail = docError.DocumentElement["detail"].InnerText;

                            if (errorDetail.Contains("MSGBOX-"))
                            {
                                throw new Exceptions.MessageOnlyException(errorDetail);
                            }
                            else
                            {
                                throw new Exception(errorDetail);
                            }
                        }
                        else
                        {
                            int origLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
                            int origLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);

                            XDocument dataUpdate = XDocument.Parse(
                                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                                + "<param>"
                                + "<prod_refno>" + this.ActItem.ProdInfo.Refno + "</prod_refno>"
                                + "<receiving_is_update>" + this.chkDefaultReceiving.IsToggled + "</receiving_is_update>"
                                + "<receiving_warehouse_location_refno>" + this.LocMapDefRecv.WhsLocMapRefno
                                + "</receiving_warehouse_location_refno>"
                                + "<receiving_old_location_refno>" + origLocMapDefRecvRefno + "</receiving_old_location_refno>"
                                + "<receiving_new_location_refno>" + this.NewLocMapDefRecvRefno + "</receiving_new_location_refno>"
                                + "<picking_is_update>" + this.chkDefaultPicking.IsToggled + "</picking_is_update>"
                                + "<picking_warehouse_location_refno>" + this.LocMapDefPicking.WhsLocMapRefno
                                + "</picking_warehouse_location_refno>"
                                + "<picking_old_location_refno>" + origLocMapDefPickingRefno + "</picking_old_location_refno>"
                                + "<picking_new_location_refno>" + this.NewLocMapDefPickingRefno + "</picking_new_location_refno>"
                                + "</param>");

                            await this.ActItem.Update(dataUpdate);

                            this.CurrLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
                            this.LocMapDefPicking = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefPickingRefno);
                            this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);
                            this.LocMapDefRecv = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefRecvRefno);

                            await this.DisplayData();
                            this.lblCurrentPickingLocation.TextColor = Color.Black;
                            this.lblCurrentReceivingLocation.TextColor = Color.Black;

                            // TODO: Resource strings
                            await Device.InvokeOnMainThreadAsync(async () => await DisplayAlert("Assign Default Location", "Product default location(s) saved.", "OK"));
                        }
                    }
                    else
                    {
                        // TODO: Resource strings
                        throw new Exception("Assign Default Product Location failed.");
                    }

                    saveComplete = false; // Set false to stay in form
                }
            }
            catch (Exceptions.MessageOnlyException ex)
            {
                saveComplete = false;
                ex.Show(this.ActSubType);
            }
            catch (Exception ex)
            {
                saveComplete = false;

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                    await errRpt.ShowDialog();
                });
            }

            return saveComplete;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async Task AssignDefaultLocation_GotFocus(object sender, EventArgs e)
        {
            if (!SysEnvironment.GetInstance().IsApplicationExitInvoked)
            {
                this.LoadData();
                this.IsControlLoaded = true;
                this.cmdLocation.Focus();

                await this.ScannerEnable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void IdentifyChangesMade(object sender, EventArgs e)
        {
            if (this.IsControlLoaded)
            {
                this.ChangesMade = true;

                this.SysEnv.CurrentBarCodeModifier1 = this.updnBarCodeModifier1.SelectedItem.ToString();

                if (this.chkDefaultPicking.IsToggled)
                {
                    this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                }

                if (this.chkDefaultReceiving.IsToggled)
                {
                    this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                }
                else
                {
                    this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdLocation_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectLocationMap, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                {
                    // TODO: Resource strings
                    throw new Exception("Unexpected result this.LocMap.CurrLocMap.WhsLocMapRefno == 0.");
                }
                else
                {
                    this.ActItem.LocMap = this.LocMap;

                    if (this.chkDefaultPicking.IsToggled)
                    {
                        this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                    }
                    else
                    {
                        this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                    }

                    if (this.chkDefaultReceiving.IsToggled)
                    {
                        this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                    }
                    else
                    {
                        this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                    }
                }
            }

            await this.DisplayData();
            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private async void cmdProduct_Click(object sender, EventArgs e)
        {
            await this.ScannerDisable();

            FormDialogContainer form = new FormDialogContainer(FormDialogContainer.EDialogTypes.ucSelectProduct, SysCommon.ActSubTypes.NotApplicable);

            if (await form.DisplayControl.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDoc = await form.GetXML();
                int prodRefno = Convert.ToInt32(xmlDoc.Root.Element("ProductRefno").Value);

                if (prodRefno == 0)
                {
                    this.CurrLocMapDefPickingRefno = 0;
                    this.CurrLocMapDefRecvRefno = 0;
                }

                if (prodRefno != 0 && this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                {
                    this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromRefno(prodRefno);
                    this.CurrLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
                    this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);

                    // Assigning the new location map will only be done by the set location button.
                }

                await this.DisplayData();
            }

            await this.ScannerEnable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oBarCodeScanArgs"></param>
        /// <remarks></remarks>
        private async Task BarCodeHandler(object sender, BarCodeScannerEventArgs scanArgs)
        {
            int prodUnitRefno = 0;
            BarCodeParse parse = null;

            await Task.Run(async () =>
            {
                try
                {
                    await this.ScannerDisable();

                    parse = scanArgs.ParseInfo;

                    // Global location identifier scanned
                    if (!string.IsNullOrEmpty(parse.LocationIdentifier))
                    {
                        int locMapRefno = await LocationMapTasks.GetLocMapRefnoFromBarCodeGlobalLocIdentifier(parse.LocationIdentifier);

                        this.LocMap.CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                        if (this.LocMap.CurrLocMap.WhsLocMapRefno == 0)
                        {
                            throw new Exception("Unexpected result this.LocMap.CurrLocMap.WhsLocMapRefno == 0.");
                        }

                        this.ActItem.LocMap = this.LocMap;

                        if (this.chkDefaultPicking.IsToggled)
                        {
                            this.NewLocMapDefPickingRefno = this.LocMap.CurrLocMapRefno;
                        }
                        else
                        {
                            this.NewLocMapDefPickingRefno = this.CurrLocMapDefPickingRefno;
                        }

                        if (this.chkDefaultReceiving.IsToggled)
                        {
                            this.NewLocMapDefRecvRefno = this.LocMap.CurrLocMapRefno;
                        }
                        else
                        {
                            this.NewLocMapDefRecvRefno = this.CurrLocMapDefRecvRefno;
                        }

                        await Device.InvokeOnMainThreadAsync(async () => await this.DisplayData());
                    }
                    else
                    {
                        // Product identifier
                        if (!string.IsNullOrEmpty(parse.ProductIdentifier))
                        {
                            prodUnitRefno = await SysShared.IdentifyProductScanned(this.ActSubType, parse, prodUnitRefno);

                            if (prodUnitRefno == 0)
                            {
                                this.CurrLocMapDefPickingRefno = 0;
                                this.CurrLocMapDefRecvRefno = 0;
                            }
                            else
                            {
                                this.ActItem.ProdInfo = await ProductInfoTasks.GetInfoFromUnitRefno(prodUnitRefno);

                                if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
                                {
                                    this.CurrLocMapDefPickingRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Picking);
                                    this.CurrLocMapDefRecvRefno = await LocationMapTasks.GetDefaultLocMapRefnoFromProdRefno(this.LocMap.CurrLocMap.WhsLocMapRefno, this.ActItem.ProdInfo.Refno, SysCommon.ProdLocMapTypes.Receiving);
                                }
                            }

                            await this.DisplayData();
                        }

                        // Activity item fk refno
                        if (!string.IsNullOrEmpty(parse.ActItemFKRefno))
                        {
                            // TODO: Resource strings
                            throw new Exception("Scanning a line item bar code is not supported here!");
                        }

                        // Activity detail fk refno
                        if (!string.IsNullOrEmpty(parse.ActDetailFKRefno))
                        {
                            // TODO: Resource strings
                            throw new Exception("Scanning a detail bar code is not supported here!");
                        }

                        // Shipping container scanned, skid
                        if (!string.IsNullOrEmpty(parse.SSCC18))
                        {
                            // TODO: Resource strings
                            throw new Exception("Scanning an shipping container bar code is not supported here!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        FormErrorReport errRpt = new FormErrorReport(this.ActSubType, ex);
                        await errRpt.ShowDialog();
                    });
                }
                finally
                {
                    await this.ScannerEnable();
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private async Task DisplayData()
        {
            if (this.ActItem.ProdInfo.UnitRefno != 0)
            {
                // TODO: Resource strings
                this.cmdProduct.Text = "Product " + this.ActItem.ProdInfo.Code;
                this.lblProdDesc.Text = this.ActItem.ProdInfo.Desc;
            }
            else
            {
                // TODO: Resource strings
                this.cmdProduct.Text = "Select Product";
                this.lblProdDesc.Text = string.Empty;
            }

            if (this.LocMap.CurrLocMap.WhsLocMapRefno != 0)
            {
                // TODO: Resource strings
                cmdLocation.Text = LocationMapTasks.GetCurrentReadableLocation(true);
            }
            else
            {
                // TODO: Resource strings
                cmdLocation.Text = "Set Location";
            }

            SLocMap locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefPickingRefno);
            this.lblCurrentPickingLocation.Text = LocationMapTasks.GetCurrentReadableLocation(locMap, true);

            if (string.IsNullOrEmpty(this.lblCurrentPickingLocation.Text))
            {
                // TODO: Resource strings
                this.lblCurrentPickingLocation.Text = "Not Set";
            }

            locMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(this.CurrLocMapDefRecvRefno);
            this.lblCurrentReceivingLocation.Text = LocationMapTasks.GetCurrentReadableLocation(locMap, true);

            if (string.IsNullOrEmpty(this.lblCurrentReceivingLocation.Text))
            {
                // TODO: Resource strings
                this.lblCurrentReceivingLocation.Text = "Not Set";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        private void LoadData()
        {
            if (!this.IsControlLoaded)
            {
                //// Me.chkDisconnectMode.Checked = oEnvironment.SysSettings.IsDisconnectedMode
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task AssignDefaultLocation_LostFocus(object sender, EventArgs e)
        {
            if (this.Scanner != null)
            {
                await this.ScannerDisable();
            }
        }

        private async void CmdSave_Clicked(object sender, EventArgs e)
        {
            if (await this.SaveChanges())
            {
                await this.Close();
            }
        }
    }
}
