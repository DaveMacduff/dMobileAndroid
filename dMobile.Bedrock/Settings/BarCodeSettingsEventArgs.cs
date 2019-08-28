namespace dMobile.Bedrock
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class BarCodeSettingsEventArgs : EventArgs
  {
    private string statusOutMsg = string.Empty;
    private int minValue = 0;
    private int maxValue = 0;
    private int currentValue = 0;

    public BarCodeSettingsEventArgs()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataMsg"></param>
    public BarCodeSettingsEventArgs(string msgData)
    {
      this.StatusOutMsg = msgData;
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
  }
}
