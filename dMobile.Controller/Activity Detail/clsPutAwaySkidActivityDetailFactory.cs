namespace dMobile.Controller
{
  public class PutAwaySkidActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new PutAwaySkidActDetail(actItem);
    }
  }
}
