namespace dMobile.Controller
{
  public class CycleCountActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new CycleCountActDetail(actItem);
    }
  }
}
