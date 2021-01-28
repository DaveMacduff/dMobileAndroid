namespace dMobile.Controller
{
    public class POWorkSheetActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new POWorkSheetActItem(activity);
        }
    }
}
