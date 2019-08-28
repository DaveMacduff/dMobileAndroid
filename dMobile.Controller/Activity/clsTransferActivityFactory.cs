namespace dMobile.Controller
{
  public class TransferActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new TransferActivity();
    }
  }
}
