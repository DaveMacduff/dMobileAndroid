namespace dMobile.Controller
{
  public class InventoryInquiryActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new InventoryInquiryActDetail(actItem);
    }
  }
}
