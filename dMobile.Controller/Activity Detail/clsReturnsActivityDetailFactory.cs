namespace dMobile.Controller
{
  public class ReturnsActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new ReturnsActDetail(actItem);
    }
  }
}
