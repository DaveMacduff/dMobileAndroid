namespace dMobile.Controller
{
    public class TicketByProdActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new TicketByProdActItem(activity);
        }
    }
}
