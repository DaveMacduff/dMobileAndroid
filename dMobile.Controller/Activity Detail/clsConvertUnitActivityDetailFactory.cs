namespace dMobile.Controller
{
  public class ConvertUnitActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem activityItem)
    {
      return new ConvertUnitActDetail(activityItem);
    }
  }
}
