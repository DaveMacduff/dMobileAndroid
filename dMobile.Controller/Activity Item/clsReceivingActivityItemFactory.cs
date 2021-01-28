namespace dMobile.Controller
{
    public class RecvActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new RecvActItem(activity);
        }
    }
}
