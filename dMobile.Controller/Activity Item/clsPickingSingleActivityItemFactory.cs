namespace dMobile.Controller
{
  public class PickSingleActItemFactory : ActItemFactory
  {
    public override ActItem GetActItem(Activity activity)
    {
      return new PickSingleActItem(activity);
    }
  }
}
