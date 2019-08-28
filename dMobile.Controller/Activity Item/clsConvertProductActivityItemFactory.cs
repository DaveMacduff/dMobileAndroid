namespace dMobile.Controller
{
    public class ConvertProductActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new ConvertProductActItem(activity);
        }
    }
}
