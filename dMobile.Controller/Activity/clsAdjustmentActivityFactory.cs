namespace dMobile.Controller
{
  /// <summary>
  /// 
  /// </summary>
  public class AdjActivityFactory : ActivityFactory
  {
    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override Activity GetActivity()
    {
      return new AdjActivity();
    }

    #endregion
  }
}
