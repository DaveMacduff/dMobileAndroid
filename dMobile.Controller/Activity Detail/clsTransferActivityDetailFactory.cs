namespace dMobile.Controller
{
  public class TransferActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new TransferActDetail(actItem);
    }
  }
}
