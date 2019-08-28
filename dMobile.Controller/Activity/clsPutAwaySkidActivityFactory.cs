namespace dMobile.Controller
{
  public class PutAwaySkidActivityFactory : ActivityFactory
  {
    public override Activity GetActivity()
    {
      return new PutAwaySkidActivity();
    }
  }
}
