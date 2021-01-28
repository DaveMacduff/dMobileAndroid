namespace dMobile.Controller
{
    public class BarCodeActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new BarCodeActItem(activity);
        }
    }
}
