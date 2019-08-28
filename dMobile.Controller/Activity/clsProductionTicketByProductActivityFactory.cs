namespace dMobile.Controller
{
	public class TicketByProdActivityFactory : ActivityFactory
	{
		public override Activity GetActivity()
		{
			return new TicketByProdActivity();
		}
	}
}
