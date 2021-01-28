namespace dMobile.Controller
{
    public class PutAwaySkidActItemFactory : ActItemFactory
    {
        public override ActItem GetActItem(Activity activity)
        {
            return new PutAwaySkidActItem(activity);
        }
    }
}
