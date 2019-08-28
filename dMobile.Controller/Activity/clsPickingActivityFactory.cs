namespace dMobile.Controller
{
  public class PickingActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new PickingActivity();
    }
  }
}
