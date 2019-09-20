namespace dMobile.BarCodeScanner
{
  using System;
  using Intermec.DataCollection;
  using dMobile.Bedrock;

  /// <summary>
  /// This is the barcode scanner class for Intermec devices.
  /// </summary>
  public class IntermecBarCodeScanner : BarCodeScanner
  {
    #region Fields

    private BarcodeReader intermecReader = null;
    
    /// <summary>
    /// Created to synchronize the main thread and temporary threads.
    /// </summary>
    //private Control syncLock;
    #endregion

    #region Properties

    private BarcodeReader IntermecReader
    {
      get
      {
        return this.intermecReader;
      }

      set
      {
        this.intermecReader = value;
      }
    }

    #endregion


    #region Public Members

    /// <summary>
    /// Initiates the Intermec scanner.
    /// </summary>
    /// <returns>Whether initialization was successful</returns>
    public override bool Initialize()
    {
      bool isInitialized = false;
      //syncLock = new Control();
      //syncLock.Enabled = false;
      //syncLock.Visible = false;
      //syncLock.Width = 1;
      //syncLock.Height = 1;
      //syncLock.TabStop = false;

      // If scanner is already present then fail initialize
      if (this.IntermecReader == null)
      {
        this.IntermecReader = new BarcodeReader();

        // Setup reader
        this.IntermecReader.ThreadedRead(true);
        isInitialized = true;
      }

      return isInitialized;
    }

    /// <summary>
    /// Start a Intermec scan (not needed).
    /// </summary>
    public override void Start()
    {
      this.IntermecReader.BarcodeRead += new BarcodeReadEventHandler(intermecReader_BarcodeRead);

      // intermecReader.ScannerOn = true;
      this.IntermecReader.ScannerEnable = true;
      //this.IntermecReader.ThreadedRead(true);
    }

    /// <summary>
    /// Stop a Intermec scan (not needed).
    /// </summary>
    public override void Stop()
    {
      this.IntermecReader.ScannerEnable = false;
//        this.IntermecReader.ThreadedRead
      this.IntermecReader.BarcodeRead -= new BarcodeReadEventHandler(intermecReader_BarcodeRead);

      // intermecReader.ScannerOn = false;
    }

    /// <summary>
    /// Terminates the Intermec scanner.
    /// </summary>
    public override void Terminate()
    {
      // If we have a scanner
      if (this.IntermecReader != null)
      {
        // Free it up
        this.IntermecReader.Dispose();

        // Indicate we no longer have one
        this.IntermecReader = null;
      }
    }

    /// <summary>
    /// Event that calls the delegate.
    /// </summary>
    public void ThreadedBarCodeScan(BarCodeScannerEventArgs e)
    {
        // This is an Intermec specific issue.
        // The read event is blocking on Intermec, so to allow
        // the barcodehandlers to spawn forms that listen to the barcode handler
        // it has to be non-blocking. Otherwise, the scans won't get processed
        // until we exit the parent form's barcodehandler.
        // To overcome this, we delay the fire event in another thread. This allows
        // the primary handler to finish and process more scans.
        System.Threading.WaitCallback wcb = new System.Threading.WaitCallback(DelayedInvoke);
        System.Threading.ThreadPool.QueueUserWorkItem(wcb, e);
    }

    /// <summary>
    /// Waits for the main thread to leave the BarCodeRead function.
    /// </summary>
    /// <param name="param"></param>
    private void DelayedInvoke(object param)
    {
        BarCodeScannerEventArgs e = (BarCodeScannerEventArgs)param;
        System.Threading.Thread.Sleep(25); // Sleep to allow the main thread to exit the read event
        //if (syncLock.InvokeRequired)
        //{
        //    syncLock.Invoke(new BarCodeScanEventHandler(OnDelayedBarCodeScan), new object[] { this, e });
        //}
    }

    /// <summary>
    /// Fires the event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDelayedBarCodeScan(object sender, BarCodeScannerEventArgs e)
    {
        OnBarCodeScan(e);
    }

    #region IDisposable Members

    public override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Code to clean up managed resources
          Terminate();
          //syncLock.Dispose();
      }

      // Code to clean up unmanaged resources
    }

    #endregion

    #endregion

    #region Private Members

    /// <summary>
    /// Event that fires when a Intermec scanner has performed a scan.
    /// </summary>
    private void intermecReader_BarcodeRead(object sender, BarcodeReadEventArgs bre)
    {
//      try
//      {
//        // Raise read event to caller (with data)
//        BarCodeScannerEventArgs e = new BarCodeScannerEventArgs(bre.strDataBuffer);
//        this.ThreadedBarCodeScan(e); // wait for this thread to end, then fire event
//      }
//      catch (RuleViolationException ex)
//      {
//#if MULTILANGUAGE
//        MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeRuleViolation) + Environment.NewLine + ex.Message,
//            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
//#else
//        MessageBox.Show("Bar code rule violation." + Environment.NewLine + ex.Message,
//            "Scan Engine.",
//#endif
//            MessageBoxButtons.OK,
//            MessageBoxIcon.Exclamation,
//            MessageBoxDefaultButton.Button1);
//      }
//      catch (BarCodeParseException ex)
//      {
//          MessageBox.Show(
//#if MULTILANGUAGE
//              Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeParseException) + Environment.NewLine + ex.Message,
//#else
//              "Bar Code Parse Exception" + Environment.NewLine + ex.Message,
//#endif
//              "Scan Engine",
//              MessageBoxButtons.OK,
//              MessageBoxIcon.Exclamation,
//              MessageBoxDefaultButton.Button1);

//          this.Stop();
//          this.Start();
//      }
//      catch (Exception ex)
//      {
//        string msg = ex.Message;
//        this.Terminate();
//        this.Initialize();

//        MessageBox.Show(msg,
//#if MULTILANGUAGE
//            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
//#else
//            "Scan Engine.",
//#endif
//            MessageBoxButtons.OK,
//            MessageBoxIcon.Exclamation,
//            MessageBoxDefaultButton.Button1);
//      }
    }

    #endregion
  }
}
