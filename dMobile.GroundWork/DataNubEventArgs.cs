namespace dMobile.GroundWork
{
  using System;

  public class DataNubEventArgs : EventArgs
  {
    #region Fields

    private string statusOutMsg = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    public DataNubEventArgs()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msgData"></param>
    public DataNubEventArgs(string msgData)
    {
      this.StatusOutMsg = msgData;
    }

    #endregion

    #region Properties

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

    #endregion
  }
}
