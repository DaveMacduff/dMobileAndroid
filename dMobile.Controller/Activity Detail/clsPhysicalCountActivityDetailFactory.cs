namespace dMobile.Controller
{
  public class PhysicalCountActDetailFactory : ActDetailFactory
	{
		public override ActDetail GetActDetail(ActItem actItem)
		{
			return new PhysicalCountActDetail(actItem);
		}
	}
}
