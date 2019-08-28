namespace dMobile.Controller
{
  public class ConvertUnitActivityFactory : ActivityFactory
  {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override Activity GetActivity()
    {
      return new ConvertUnitActivity();
    }
  }

}
