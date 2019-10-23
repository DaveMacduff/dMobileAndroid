// TODO: Check for Symbol SDK for android
//namespace dMobile.BarCodeScanner
//{
//  using System;
//    using Symbol;
//    using Symbol.Barcode;
//    using dMobile.Bedrock;

//  /// <summary>
//  /// This is the barcode scanner class for Symbol devices.
//  /// </summary>
//  public class SymbolBarCodeScanner : BarCodeScanner
//  {
//    #region Fields

//    private Reader symbolReader = null;
//    private ReaderData symbolReaderData = null;

//    #endregion

//    #region Properties

//    private Reader SymbolReader
//    {
//      get
//      {
//        return this.symbolReader;
//      }

//      set
//      {
//        this.symbolReader = value;
//      }
//    }

//    private ReaderData SymbolReaderData
//    {
//      get
//      {
//        return this.symbolReaderData;
//      }

//      set
//      {
//        this.symbolReaderData = value;
//      }
//    }

//    #endregion

//    #region Public Members

//    /// <summary>
//    /// Initiates the Symbol scanner.
//    /// </summary>
//    /// <returns>Whether initialization was successful</returns>
//    public override bool Initialize()
//    {
//      bool isInitialized = false;

//      // If scanner is already present then fail initialize
//      if (this.SymbolReader == null)
//      {
//        // Create new scanner, first available scanner will be used.
//        this.SymbolReader = new Reader();

//        // Create scanner data
//        this.SymbolReaderData = new ReaderData(ReaderDataTypes.Text, ReaderDataLengths.MaximumLabel);

//        // Enable scanner, with wait cursor
//        this.SymbolReader.Actions.Enable();

//        // Setup scanner
//        this.SymbolReader.Parameters.CodeIdType = CodeIdTypes.AIM;
//        this.SymbolReader.Parameters.Feedback.Success.BeepTime = 0;
//        this.SymbolReader.Parameters.Feedback.Success.WaveFile = "\\windows\\alarm3.wav";
//        isInitialized = true;
//      }

//      return isInitialized;
//    }

//    /// <summary>
//    /// Start a Symbol scan.
//    /// </summary>
//    public override void Start()
//    {
//      // If we have both a scanner and data
//      if ((this.SymbolReader != null) && (this.SymbolReaderData != null))
//      {
//        // Submit a scan
//        // Flush (Cancel all pending scans)
//        this.SymbolReader.Actions.Flush();
//        this.SymbolReader.ReadNotify += new EventHandler(symbolReader_ReadNotify);
//        this.SymbolReader.Actions.Read(SymbolReaderData);
//      }
//    }

//    /// <summary>
//    /// Stop a Symbol scan.
//    /// </summary>
//    public override void Stop()
//    {
//      // If we have a scanner
//      if (this.SymbolReader != null)
//      {
//        // Flush (Cancel all pending scans)
//        this.SymbolReader.Actions.Flush();
//        this.SymbolReader.ReadNotify -= new EventHandler(symbolReader_ReadNotify);
//      }
//    }

//    /// <summary>
//    /// Terminates the Symbol scanner.
//    /// </summary>
//    public override void Terminate()
//    {
//      // If we have a scanner data object
//      if (this.SymbolReaderData != null)
//      {
//        // Free it up
//        this.SymbolReaderData.Dispose();

//        // Indicate we no longer have one
//        this.SymbolReaderData = null;
//      }

//      // If we have a scanner
//      if (this.SymbolReader != null)
//      {
//        // Flush the Data
//        this.SymbolReader.Actions.Flush();

//        // Disable the scanner
//        this.SymbolReader.Actions.Disable();

//        // Free it up
//        this.SymbolReader.Dispose();

//        // Indicate we no longer have one
//        this.SymbolReader = null;
//      }
//    }

//    #region IDisposable Members

//    public override void Dispose(bool disposing)
//    {
//      if (disposing)
//      {
//        // Code to clean up managed resources
//        Terminate();
//      }
//      // Code to clean up unmanaged resources
//    }

//    #endregion

//    #endregion

//    #region Private Members

//    /// <summary>
//    /// Event that fires when a Symbol scanner has performed a scan.
//    /// </summary>
//    private void symbolReader_ReadNotify(object sender, EventArgs e)
//    {
//      ReaderData readerData = this.SymbolReader.GetNextReaderData();

//      switch (readerData.Result)
//      {
//        case Results.SUCCESS:
//          try
//          {
//            // Raise scan event to caller (with data)
//            this.OnBarCodeScan(new BarCodeScannerEventArgs(readerData.Text));
//          }
//          catch (RuleViolationException ex)
//          {
//                        // TODO: Create pop up and add logging
//                        //#if MULTILANGUAGE
//                        //            MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeRuleViolation) + Environment.NewLine + ex.Message,
//                        //                Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
//                        //#else
//                        //            MessageBox.Show("Bar code rule violation." + Environment.NewLine + ex.Message,
//                        //                "Scan Engine.",
//                        //#endif
//                        //                MessageBoxButtons.OK,
//                        //                MessageBoxIcon.Exclamation,
//                        //                MessageBoxDefaultButton.Button1);
//          }
//          catch (BarCodeParseException ex)
//          {
//              this.Stop();
//                        // TODO: Create pop up and add logging
//                        //              MessageBox.Show(
//                        //#if MULTILANGUAGE
//                        //                  Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeParseException) + Environment.NewLine + ex.Message,
//                        //#else
//                        //                  "Bar Code Parse Exception" + Environment.NewLine + ex.Message,
//                        //#endif
//                        //                  "Scan Engine",
//                        //                  MessageBoxButtons.OK,
//                        //                  MessageBoxIcon.Exclamation,
//                        //                  MessageBoxDefaultButton.Button1);
//                    }
//                    catch (Exception ex)
//          {
//            string msg = ex.Message;
//            this.Stop();

//                        // TODO: Create pop up and add logging
//                        //            MessageBox.Show(msg,
//                        //#if MULTILANGUAGE
//                        //                Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
//                        //#else
//                        //                "Scan Engine.",
//                        //#endif
//                        //                MessageBoxButtons.OK,
//                        //                MessageBoxIcon.Exclamation,
//                        //                MessageBoxDefaultButton.Button1);
//                    }

//                    // Start the next scan
//                    this.Start();
//          break;

//        case Results.CANCELED:
//          break;

//        default:
//          // Flush (Cancel all pending scans)
//          this.SymbolReader.Actions.Flush();
//          break;
//      }
//    }

//    #endregion
//  }
//}
