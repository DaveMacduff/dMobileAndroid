namespace dMobile.Controller
{
  public class AssignDefaultLocActItemFactory : ActItemFactory
  {
    public override ActItem GetActItem(Activity activity)
    {
      return new AssignDefaultLocActItem(activity);
    }
  }
}
