namespace dMobile.Controller
{
    public class SkidInfoActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new SkidInfoActItem(activity);
        }
    }
}
