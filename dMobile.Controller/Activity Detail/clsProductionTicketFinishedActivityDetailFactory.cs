namespace dMobile.Controller
{
    public class TicketFinishedDetailFactory : ActDetailFactory
    {
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new TicketFinishedActDetail(actItem);
        }
    }
}
