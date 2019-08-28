namespace dMobile.Controller
{
  public class SkidInfoActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new SkidInfoActivity();
    }
  }
}
