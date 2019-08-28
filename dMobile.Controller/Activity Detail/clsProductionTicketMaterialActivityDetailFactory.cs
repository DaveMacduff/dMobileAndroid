namespace dMobile.Controller
{
  public class TicketMaterialDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new TicketMaterialActDetail(actItem);
    }
  }
}
