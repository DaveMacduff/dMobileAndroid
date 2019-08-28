namespace dMobile.Controller
{
  using System;

  public class DataLayerEventArgs : EventArgs
  {
    #region Fields

    private string statusOutMsg = string.Empty;
    private int minValue = 0;
    private int maxValue = 0;
    private int currentValue = 0;
    private bool overrideDisconnectedMode = false;
    private bool setMinMaxValue = false;

    #endregion

    #region Constructors

    public DataLayerEventArgs()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataMsg"></param>
    public DataLayerEventArgs(string msgData)
    {
      this.StatusOutMsg = msgData;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public bool SetMinMaxValue
    {
      get
      {
        return this.setMinMaxValue;
      }

      set
      {
        this.setMinMaxValue = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool OverrideDisconnectedMode
    {
      get
      {
        return this.overrideDisconnectedMode;
      }

      set
      {
        this.overrideDisconnectedMode = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string StatusOutMsg
    {
      get
      {
        return this.statusOutMsg;
      }

      set
      {
        this.statusOutMsg = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MinValue
    {
      get
      {
        return this.minValue;
      }

      set
      {
        this.minValue = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MaxValue
    {
      get
      {
        return this.maxValue;
      }

      set
      {
        this.maxValue = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CurrentValue
    {
      get
      {
        return this.currentValue;
      }

      set
      {
        this.currentValue = value;
      }
    }

    #endregion
  }
}
