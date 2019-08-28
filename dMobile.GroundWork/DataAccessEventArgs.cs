namespace dMobile.GroundWork
{
  using System;

  public class DataAccessEventArgs : EventArgs
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

    /// <summary>
    /// 
    /// </summary>
    public DataAccessEventArgs()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataMsg"></param>
    public DataAccessEventArgs(string msgData)
    {
      this.statusOutMsg = msgData;
    }

    #endregion

    #region Properties

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
