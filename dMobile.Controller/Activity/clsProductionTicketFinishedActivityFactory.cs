namespace dMobile.Controller
{
	public class TicketFinishedActivityFactory : ActivityFactory
	{
		public override Activity GetActivity()
		{
			return new TicketFinishedActivity();
		}
	}
}
