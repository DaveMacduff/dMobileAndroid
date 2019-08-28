namespace dMobile.Controller
{
	public class TicketMaterialActivityFactory : ActivityFactory
	{
		public override Activity GetActivity()
		{
			return new TicketMaterialActivity();
		}
	}
}
