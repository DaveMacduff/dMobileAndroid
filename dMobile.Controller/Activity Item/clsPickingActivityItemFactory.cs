namespace dMobile.Controller
{
    public class PickingActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new PickingActItem(activity);
        }
    }
}
