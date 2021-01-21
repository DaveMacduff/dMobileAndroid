//using dMobile.Bedrock;
//using PsionTeklogix.Barcode;
//using PsionTeklogix.Barcode.ScannerServices;
//using System;

//namespace dMobile.BarCodeScanner
//{
//    public class PsionBarCodeScanner : BarCodeScanner
//    {

//        #region Fields
//        private PsionTeklogix.Barcode.Scanner psionReader = null;
//        private PsionTeklogix.Barcode.ScannerServices.ScannerServicesDriver driver;
//        #endregion

//        #region Properties

//        public Scanner PsionReader
//        {
//            get
//            {
//                return psionReader;
//            }
//            set
//            {
//                psionReader = value;
//            }
//        }

//        public ScannerServicesDriver Driver
//        {
//            get
//            {
//                return driver;
//            }

//            set
//            {
//                driver = value;
//            }
//        }

//        #endregion

//        public override bool Initialize()
//        {
//            bool isInitialized = false;

//            if (PsionReader == null)
//            {
//                PsionReader = new Scanner();

//                Driver = new ScannerServicesDriver();
//                PsionReader.Driver = Driver;

//                //int count = Driver.TotalSettingsCount;
//                //for (int i = 0; i < count; ++i)
//                //{
//                //    string setting = Driver.GetSettingName(i);
//                //    switch (setting)
//                //    {
//                //        case "a":
//                //            break;
//                //    }
//                //}

//                isInitialized = true;
//            }

//            return isInitialized;
//        }

//        /// <summary>
//        /// Start a Psion scan.
//        /// </summary>
//        public override void Start()
//        {
//            PsionReader.ScanCompleteEvent += new ScanCompleteEventHandler(PsionReader_ScanCompleteEvent);
//            PsionReader.ScanFailedEvent += new ScanFailedEventHandler(PsionReader_ScanFailedEvent);
//            PsionReader.ScanDoubleClickEvent += new ScanDoubleClickEventHandler(PsionReader_ScanDoubleClickEvent);
//            PsionReader.ScanEvent += new ScanEventHandler(PsionReader_ScanEvent);
//            PsionReader.ScanSingleClickEvent += new ScanSingleClickEventHandler(PsionReader_ScanSingleClickEvent);
//            PsionReader.ScanTriggerEvent += new ScanTriggerEventHandler(PsionReader_ScanTriggerEvent);

//            PsionReader.Enabled = true;
//        }

//        void PsionReader_ScanTriggerEvent(object sender, ScanTriggerEventArgs e)
//        {
//            //throw new NotImplementedException();
//        }

//        void PsionReader_ScanSingleClickEvent(object sender, ScanSingleClickEventArgs e)
//        {
//            //throw new NotImplementedException();
//        }

//        void PsionReader_ScanEvent(object sender, ScanEventArgs e)
//        {
//            //throw new NotImplementedException();
//        }

//        void PsionReader_ScanDoubleClickEvent(object sender, ScanDoubleClickEventArgs e)
//        {
//            //throw new NotImplementedException();
//        }

//        public override void Stop()
//        {
//            PsionReader.ScanCompleteEvent -= new ScanCompleteEventHandler(PsionReader_ScanCompleteEvent);
//            PsionReader.ScanFailedEvent -= new ScanFailedEventHandler(PsionReader_ScanFailedEvent);
//            PsionReader.ScanDoubleClickEvent -= new ScanDoubleClickEventHandler(PsionReader_ScanDoubleClickEvent);
//            PsionReader.ScanEvent -= new ScanEventHandler(PsionReader_ScanEvent);
//            PsionReader.ScanSingleClickEvent -= new ScanSingleClickEventHandler(PsionReader_ScanSingleClickEvent);
//            PsionReader.ScanTriggerEvent -= new ScanTriggerEventHandler(PsionReader_ScanTriggerEvent);

//            PsionReader.Enabled = false;
//        }

//        public override void Terminate()
//        {
//            // Adding potential memory leak here.
//            // Setting these properties to null.
//            // MdsdkNet.dll calls Terminate(), then does something like dispose which we can't handle, and then calls Dispose(false).
//            // With the mdsdkcore.dll shipped in the dMobile folder it will work; might still have memory leaks though.
//            // Without the mdsdkcore.dll (400KB) we can set the variables null in Terminate() and then they aren't attempting to dispose before our Dispose(false) call.
//            // It seems there is a PInvoke function declared that is only in the mdsdkcore.dll. But for 400KB we are leaving it out and seeing if a memory leak develops.
//            // If it does, we can add in the DLL and see if that fixes it.
//            //     mdsdkcore.dll is in dMobile.BarCodeScanner.DLLs
//            // RD 2016.11.17

//            // Addendum - just distribute the DLL
//        }

//        public override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {

//                if (PsionReader != null)
//                {
//                    PsionReader.Dispose();
//                }

//                if (driver != null)
//                {
//                    driver.Dispose();
//                }
//            }


//            Driver = null;
//            PsionReader = null;
//        }

//        void PsionReader_ScanFailedEvent(object sender, ScanFailedEventArgs e)
//        {
//            //MessageBox.Show("Scan failed with error code: " + e.ErrorCode);
//        }

//        void PsionReader_ScanCompleteEvent(object sender, ScanCompleteEventArgs e)
//        {
//            try
//            {
//                string scanData = e.Text;

//                // Raise scan event to caller (with data)
//                this.OnBarCodeScan(new BarCodeScannerEventArgs(scanData));
//            }
//            catch (RuleViolationException ex)
//            {
////#if MULTILANGUAGE
////        MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeRuleViolation) + Environment.NewLine + ex.Message, 
////            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine), 
////#else
////                MessageBox.Show("Bar code rule violation." + Environment.NewLine + ex.Message,
////                    "Scan Engine.",
////#endif
//// MessageBoxButtons.OK,
////                    MessageBoxIcon.Exclamation,
////                    MessageBoxDefaultButton.Button1);
//            }
//            catch (BarCodeParseException ex)
//            {
//                //                MessageBox.Show(
//                //#if MULTILANGUAGE
//                //                Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeParseException) + Environment.NewLine + ex.Message,
//                //#else
//                //"Bar Code Parse Exception" + Environment.NewLine + ex.Message,
//                //#endif
//                // "Scan Engine",
//                //                    MessageBoxButtons.OK,
//                //                    MessageBoxIcon.Exclamation,
//                //                    MessageBoxDefaultButton.Button1);

//                this.Stop();
//                this.Start();
//            }
//            catch (Exception ex)
//            {
//                string msg = ex.Message;
//                this.Terminate();
//                this.Initialize();
                
//                //                MessageBox.Show(msg,
//                //#if MULTILANGUAGE
//                //            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
//                //#else
//                // "Scan Engine.",
//                //#endif
//                // MessageBoxButtons.OK,
//                //                    MessageBoxIcon.Exclamation,
//                //                    MessageBoxDefaultButton.Button1);
//            }
//        }

//    }
//}