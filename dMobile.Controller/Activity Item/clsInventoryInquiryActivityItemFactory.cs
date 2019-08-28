namespace dMobile.Controller
{
  public class InvInquiryActItemFactory : ActItemFactory
  {
    public override ActItem GetActItem(Activity activity)
    {
      return new InvInquiryActItem(activity);
    }
  }
}
