namespace dMobile.BarCodeScanner
{
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using System.Text;

  class RuleViolationException : Exception
  {
    #region Constructors

    public RuleViolationException() 
      : base()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public RuleViolationException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public RuleViolationException(string message, Exception inner) 
      : base(message, inner)
    {
    }

    #endregion
  }
}
