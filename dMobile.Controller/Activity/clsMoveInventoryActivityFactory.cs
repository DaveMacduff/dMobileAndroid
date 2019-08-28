namespace dMobile.Controller
{
  public class MoveInvActivityFactory : ActivityFactory
  {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override Activity GetActivity()
    {
      return new MoveInvActivity();
    }
  }
}
