namespace dMobile.Controller
{
    public class TransferActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new TransferActItem(activity);
        }
    }
}
