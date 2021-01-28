namespace dMobile.Controller
{
    public class TicketMaterialActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new TicketMaterialActItem(activity);
        }
    }
}
