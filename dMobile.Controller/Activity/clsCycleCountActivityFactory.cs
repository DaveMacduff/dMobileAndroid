namespace dMobile.Controller
{
  public class CycleActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new CycleActivity();
    }
  }

}
