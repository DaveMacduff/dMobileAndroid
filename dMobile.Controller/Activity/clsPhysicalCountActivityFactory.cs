namespace dMobile.Controller
{
  public class PhysicalActivityFactory : ActivityFactory
  {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public override Activity GetActivity()
    {
      return new PhysicalActivity();
    }
  }
}
