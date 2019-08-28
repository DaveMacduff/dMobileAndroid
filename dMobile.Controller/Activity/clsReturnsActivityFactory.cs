namespace dMobile.Controller
{
  public class ReturnsActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new ReturnsActivity();
    }
  }
}
