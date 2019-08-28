namespace dMobile.Controller
{
  public class ConvertUnitActItemFactory : ActItemFactory
  {
    public override ActItem GetActItem(Activity activity)
    {
      return new ConvertUnitActItem(activity);
    }
  }
}
