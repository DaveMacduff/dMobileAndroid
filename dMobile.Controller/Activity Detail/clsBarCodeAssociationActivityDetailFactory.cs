namespace dMobile.Controller
{
  public class BarCodeAssocActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem activityItem)
    {
      return new BarCodeAssocActDetail(activityItem);
    }
  }
}
