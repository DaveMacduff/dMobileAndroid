namespace dMobile.Controller
{
  public class POWorkSheetActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new POWorkSheetActivity();
    }
  }
}
