namespace dMobile.Controller
{
  public class TicketFinishedActItemFactory : ActItemFactory
  {
    public override ActItem GetActItem(Activity activity)
    {
      return new TicketFinishedActItem(activity);
    }
  }
}
