namespace dMobile.Controller
{
  public class AdjActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem activityItem)
    {
      return new AdjActDetail(activityItem);
    }
  }
}
