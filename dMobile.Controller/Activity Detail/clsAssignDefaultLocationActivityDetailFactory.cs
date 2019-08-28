namespace dMobile.Controller
{
  public class AssignDefaultLocActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem activityItem)
    {
      return new AssignDefaultLocActDetail(activityItem);
    }
  }
}
