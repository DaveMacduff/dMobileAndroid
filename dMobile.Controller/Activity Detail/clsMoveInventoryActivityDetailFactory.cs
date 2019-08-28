namespace dMobile.Controller
{
  public class clsMoveInventoryActDetailFactory : ActDetailFactory
  {
    public override ActDetail GetActDetail(ActItem actItem)
    {
      return new MoveInventoryActDetail(actItem);
    }
  }
}
