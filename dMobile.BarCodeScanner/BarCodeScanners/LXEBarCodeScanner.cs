namespace dMobile.BarCodeScanner
{
  using System;
  using System.Threading;
  using System.Windows.Forms;
  using ManagedLXEAPI;
  using dMobile.Bedrock;

  /// <summary>
  /// This is the barcode scanner class for LXE devices.
  /// </summary>
  public class LXEBarCodeScanner : BarCodeScanner
  {
    #region Fields

    private ManagedLXEAPI.Scanner lxeReader = null;

    #endregion

    #region Properties

    private Scanner LXEReader
    {
      get 
      {
        return this.lxeReader; 
      }
      
      set 
      {
        this.lxeReader = value; 
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    /// Initiates the LXE scanner.
    /// </summary>
    /// <returns>Whether initialization was successful</returns>
    public override bool Initialize()
    {
      bool isInitialized = false;

      // If scanner is already present then fail initialize
      if (this.LXEReader == null)
      {
        // Create new scanner, first available scanner will be used.
        this.LXEReader = new Scanner();

        // Disable Scan Data as Keyboard Input
        this.LXEReader.ScannerKeysOff();

        // This function enables processing of ASCII values less than 0x20 (control codes) 
        // when scanner data is processed as keystroke messages.
        // The control code data is translated from the ASCII value to its equivalent control key sequence 
        // and is sent as keystroke messages just like the printable data.
        this.LXEReader.CtrlCodeOn();

        //// this.LXEReader.SymEnable("UCC/EAN128", true);

        this.LXEReader.ScannerEnable();

        // LXEScannerEnable() takes appox. 1.5 secs
        Thread.Sleep(2000);
      }

      return isInitialized;
    }

    /// <summary>
    /// Start a LXE scan.
    /// </summary>
    public override void Start()
    {
      // Add the event handler here not in the Initialize method. We need to be able to remove the event on the fly
      this.LXEReader.BarcodeScan += new Scanner.ScannerEventHandler(this.ScanMessage);
      this.LXEReader.InputEnable();
    }

    /// <summary>
    /// Stop a LXE scan.
    /// </summary>
    public override void Stop()
    {
      this.LXEReader.BarcodeScan -= new Scanner.ScannerEventHandler(this.ScanMessage);
      this.LXEReader.InputDisable();
    }

    /// <summary>
    /// Terminates the LXE scanner.
    /// </summary>
    public override void Terminate()
    {
      // If we have a scanner
      if (this.LXEReader != null)
      {
        // Indicate we no longer have one
        this.LXEReader = null;
      }
    }

    #region IDisposable Members

    public override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Code to clean up managed resources
        this.Terminate();
      }

      // Code to clean up unmanaged resources
    }

    #endregion

    #endregion

    #region Private Members

    /// <summary>
    /// Event that fires when a LXE scanner has performed a scan.
    /// </summary>
    private void ScanMessage(object sender)
    {
        try
        {
            int dataLen = 255;  // Maximum length of a 1-D Barcode is 255
            string scanData = this.LXEReader.GetData(ref dataLen);

            // Raise scan event to caller (with data)
            this.OnBarCodeScan(new BarCodeScannerEventArgs(scanData));
        }
        catch (RuleViolationException ex)
        {
#if MULTILANGUAGE
        MessageBox.Show(Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeRuleViolation) + Environment.NewLine + ex.Message, 
            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine), 
#else
            MessageBox.Show("Bar code rule violation." + Environment.NewLine + ex.Message,
                "Scan Engine.",
#endif
 MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
        }
        catch (BarCodeParseException ex)
        {
            MessageBox.Show(
#if MULTILANGUAGE
                Translator.GetInstance().Translate(TranslatorVocabulary.BarCodeParseException) + Environment.NewLine + ex.Message,
#else
                "Bar Code Parse Exception" + Environment.NewLine + ex.Message,
#endif
                "Scan Engine",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

            this.Stop();
            this.Start();
        }
        catch (Exception ex)
        {
            string msg = ex.Message + Environment.NewLine
#if MULTILANGUAGE
            + Translator.GetInstance().Translate(TranslatorVocabulary.EngineLastError) + this.LXEReader.GetLastError() + "\'";
#else
 + "Engine last error \'" + this.LXEReader.GetLastError() + "\'";
#endif
            this.Terminate();
            this.Initialize();

            MessageBox.Show(msg,
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.ScanEngine),
#else
 "Scan Engine.",
#endif
 MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
        }
    }

    #endregion
  }
}
