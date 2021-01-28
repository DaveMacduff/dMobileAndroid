namespace dMobile.Controller
{
    public class ReturnsActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new ReturnsActItem(activity);
        }
    }
}
