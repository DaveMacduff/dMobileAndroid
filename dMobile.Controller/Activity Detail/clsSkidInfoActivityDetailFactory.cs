namespace dMobile.Controller
{
  public class SkidInfoActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem activityItem)
    {
      return new SkidInfoActDetail(activityItem);
    }
  }
}
